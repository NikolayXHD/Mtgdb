using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mtgdb.Data;

namespace Mtgdb.Downloader
{
	public class ImageDownloader
	{
		[UsedImplicitly]
		public ImageDownloader(CardRepository repository, Megatools megatools)
		{
			_repository = repository;
			_megatools = megatools;
		}

		public void Download(string quality, IList<ImageDownloadProgress> allProgress)
		{
			var progressEnumerable = allProgress
				.Where(_ => Str.Equals(_.QualityGroup.Quality, quality))
				.OrderBy(getReleaseDate);

			var commonStack = new ConcurrentStack<ImageDownloadProgress>();
			var gdriveStack = new Stack<ImageDownloadProgress>();
			var megaStack = new Stack<ImageDownloadProgress>();

			foreach (var progress in progressEnumerable)
			{
				if (progress.MegaUrl != null && progress.GdriveUrl != null)
					commonStack.Push(progress);
				else if (progress.MegaUrl != null)
					megaStack.Push(progress);
				else if (progress.GdriveUrl != null)
					gdriveStack.Push(progress);
			}

			Console.WriteLine("Found {0} directories for quality '{1}' in configuration",
				commonStack.Count + megaStack.Count + gdriveStack.Count,
				quality);

			TotalCount = commonStack.Concat(megaStack).Concat(gdriveStack).Sum(_ => _.FilesOnline.Count);

			_countInDownloadedDirs = 0;
			ProgressChanged?.Invoke();

			var webClient1 = new GdriveWebClient();
			var webClient2 = new GdriveWebClient();

			void megaFileDownloaded()
			{
				Interlocked.Increment(ref _countInDownloadedDirs);
				ProgressChanged?.Invoke();
			}

			_megatools.FileDownloaded += megaFileDownloaded;

			Parallel.Invoke(
				() => download(megaStack, commonStack, downloadFromMega),
				// google drive delays download start
				() => download(gdriveStack, commonStack, t=> downloadFromGdrive(t, webClient1)),
				() => download(gdriveStack, commonStack, t=> downloadFromGdrive(t, webClient2)));

			_megatools.FileDownloaded -= megaFileDownloaded;
		}

		private void download(
			Stack<ImageDownloadProgress> specificTasks,
			ConcurrentStack<ImageDownloadProgress> commonTasks,
			Action<ImageDownloadProgress> downloader)
		{
			while (true)
			{
				if (_abort)
					return;

				var selectedTask = selectTask(specificTasks, commonTasks);

				if (selectedTask == null)
					return;

				download(selectedTask, downloader);
			}
		}

		private ImageDownloadProgress selectTask(
			Stack<ImageDownloadProgress> specificTasks,
			ConcurrentStack<ImageDownloadProgress> commonTasks)
		{
			var specificTask = specificTasks.Count > 0
				? specificTasks.Pop()
				: null;

			commonTasks.TryPop(out var commonTask);

			if (specificTask == null && commonTask == null)
				return null;

			if (commonTask == null)
				return specificTask;

			if (specificTask == null)
				return commonTask;

			if (Str.Comparer.Compare(
				getReleaseDate(specificTask),
				getReleaseDate(commonTask)) > 0)
			{
				commonTasks.Push(commonTask);
				return specificTask;
			}

			specificTasks.Push(specificTask);
			return commonTask;
		}

		private void download(ImageDownloadProgress task, Action<ImageDownloadProgress> downloader)
		{
			if (isAlreadyDownloaded(task))
			{
				Console.WriteLine("[Skip] {0}", task.Dir.Subdirectory);

				Interlocked.Add(ref _countInDownloadedDirs, task.FilesOnline.Count);
				ProgressChanged?.Invoke();
			}
			else
			{
				Interlocked.Add(ref _countInDownloadedDirs, task.FilesDownloaded.Count);
				ProgressChanged?.Invoke();

				downloader(task);
				ImageDownloadProgressReader.WriteExistingSignatures(task);
			}
		}

		private void downloadFromMega(ImageDownloadProgress task)
		{
			lock (_syncOutput)
				Console.WriteLine($"Downloading {task.Dir.Subdirectory} from {task.MegaUrl}");

			_megatools.Download(
				task.MegaUrl,
				task.TargetSubdirectory,
				name: task.Dir.Subdirectory,
				silent: true);
		}

		private void downloadFromGdrive(ImageDownloadProgress task, GdriveWebClient webClient)
		{
			string fileName = task.Dir.Subdirectory + ".7z";
			string targetDirectory = task.TargetDirectory;
			string gdriveUrl = task.GdriveUrl;

			lock (_syncOutput)
				Console.WriteLine($"Downloading {task.Dir.Subdirectory} from {gdriveUrl}");

			if (!webClient.DownloadAndExtract(gdriveUrl, targetDirectory, fileName))
				return;

			Interlocked.Add(ref _countInDownloadedDirs, task.FilesOnline.Count - task.FilesDownloaded.Count);
			ProgressChanged?.Invoke();
		}

		private string getReleaseDate(ImageDownloadProgress progress)
		{
			if (!_repository.IsLoadingComplete.Signaled)
				return null;

			return _repository.SetsByCode.TryGet(progress.Dir.Subdirectory)?.ReleaseDate;
		}

		private static bool isAlreadyDownloaded(ImageDownloadProgress progress)
		{
			string targetSubdirectory = progress.TargetSubdirectory;
			Directory.CreateDirectory(targetSubdirectory);

			if (progress.FilesOnline == null)
				return false;

			bool alreadyDownloaded = true;

			var existingFiles = new HashSet<string>(
				Directory.GetFiles(targetSubdirectory, "*.*", SearchOption.AllDirectories),
				Str.Comparer);

			var existingSignatures = new Dictionary<string, FileSignature>(Str.Comparer);

			foreach (var fileOnline in progress.FilesOnline.Values)
			{
				string filePath = Path.Combine(targetSubdirectory, fileOnline.Path);

				if (!existingFiles.Contains(filePath))
				{
					alreadyDownloaded = false;
					continue;
				}

				var existingSignature =
					progress.FilesCorrupted.TryGet(fileOnline.Path) ??
					progress.FilesDownloaded.TryGet(fileOnline.Path) ??
					Signer.CreateSignature(filePath, useAbsolutePath: true).AsRelativeTo(targetSubdirectory);

				if (existingSignature.Md5Hash != fileOnline.Md5Hash)
				{
					alreadyDownloaded = false;
					Console.WriteLine("Deleting modified or corrupted file {0}", filePath);

					lock (ImageLoader.SyncIo)
					{
						try
						{
							File.Delete(filePath);
						}
						catch (IOException ex)
						{
							Console.WriteLine($"Failed to remove {filePath}. {ex.Message}");
						}
					}
				}
				else
				{
					existingSignatures.Add(existingSignature.Path, existingSignature);
				}
			}

			foreach (string file in existingFiles)
			{
				var relativePath = file.Substring(targetSubdirectory.Length + 1);
				if (!progress.FilesOnline.ContainsKey(relativePath) && !Str.Equals(relativePath, Signer.SignaturesFile))
				{
					Console.WriteLine("Deleting {0}", file);
					File.Delete(file);
				}
			}

			if (alreadyDownloaded)
				ImageDownloadProgressReader.WriteExistingSignatures(progress, existingSignatures.Values);

			return alreadyDownloaded;
		}

		public void Abort()
		{
			_abort = true;
			_megatools.Abort();
		}

		public event Action ProgressChanged;

		private int CountInDownloadedDirs => _countInDownloadedDirs;

		public int DownloadedCount => CountInDownloadedDirs + _megatools.DownloadedCount;
		public int TotalCount { get; private set; }

		private readonly object _syncOutput = new object();

		private readonly CardRepository _repository;
		private readonly Megatools _megatools;
		private bool _abort;
		private int _countInDownloadedDirs;
	}
}
