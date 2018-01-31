using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mtgdb.Dal;

namespace Mtgdb.Downloader
{
	public class ImageDownloader
	{
		public ImageDownloader(CardRepository repository)
		{
			_repository = repository;
			_megatools = new Megatools();
			_megatools.FileDownloaded += fileDownloaded;
		}

		public void Download(string quality, IList<ImageDownloadProgress> allProgress)
		{
			var relevantProgress = allProgress
				.Where(_ => Str.Equals(_.QualityGroup.Quality, quality));

			if (_repository.IsLoadingComplete)
			{
				relevantProgress = relevantProgress.OrderByDescending(_ =>
					_repository.SetsByCode.TryGet(_.MegaDir.Subdirectory)?.ReleaseDate);
			}

			var qualityProgress = relevantProgress
				.ToArray();

			Console.WriteLine("Found {0} directories for quality '{1}' in configuration",
				qualityProgress.Length,
				quality);

			TotalCount = qualityProgress.Sum(_ => _.FilesOnline.Count);
			CountInDownloadedDirs = 0;

			ProgressChanged?.Invoke();

			foreach (var progress in qualityProgress)
			{
				if (_abort)
				{
					_abort = false;
					return;
				}

				bool alreadyDownloaded = isAlreadyDownloaded(progress);
				if (alreadyDownloaded)
				{
					Console.WriteLine("[Skip] {0}", progress.MegaDir.Subdirectory);
					CountInDownloadedDirs += progress.FilesOnline.Count;
					ProgressChanged?.Invoke();
					continue;
				}

				string targetDirAbsolute = ImageDownloadProgressReader.GetTargetDirAbsolute(progress);

				CountInDownloadedDirs += progress.FilesDownloaded.Count;
				_megatools.Download(progress.MegaDir.Subdirectory, progress.MegaDir.Url, targetDirAbsolute);
				CountInDownloadedDirs += progress.FilesOnline.Count - progress.FilesDownloaded.Count;

				ImageDownloadProgressReader.WriteExistingSignatures(progress);
			}
		}

		private static bool isAlreadyDownloaded(ImageDownloadProgress progress)
		{
			string targetDirAbsolute = ImageDownloadProgressReader.GetTargetDirAbsolute(progress);
			Directory.CreateDirectory(targetDirAbsolute);

			if (progress.FilesOnline == null)
				return false;

			bool alreadyDownloaded = true;
			
			var existingFiles = new HashSet<string>(Directory.GetFiles(targetDirAbsolute, "*.*", SearchOption.AllDirectories));
			var existingSignatures = new Dictionary<string, FileSignature>();

			foreach (var fileOnline in progress.FilesOnline.Values)
			{
				string filePath = Path.Combine(targetDirAbsolute, fileOnline.Path);

				if (!existingFiles.Contains(filePath))
				{
					alreadyDownloaded = false;
					continue;
				}

				var existingSignature =
					progress.FilesCorrupted.TryGet(fileOnline.Path) ??
					progress.FilesDownloaded.TryGet(fileOnline.Path);

				if (existingSignature == null)
				{
					//Console.WriteLine("Comparing local {0} to online copy", fileOnline.Path);
					existingSignature = Signer.CreateSignature(filePath, useAbsolutePath: true).AsRelativeTo(targetDirAbsolute);
				}

				if (existingSignature.Md5Hash != fileOnline.Md5Hash)
				{
					alreadyDownloaded = false;
					Console.WriteLine("Deleting modified or corrupted file {0}", filePath);

					lock (ImageLoader.SyncRoot)
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
				var relativePath = file.Substring(targetDirAbsolute.Length + 1);
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

		private void fileDownloaded()
		{
			ProgressChanged?.Invoke();
		}

		public event Action ProgressChanged;

		private int CountInDownloadedDirs { get; set; }
		public int DownloadedCount => CountInDownloadedDirs + _megatools.DownloadedCount;
		public int TotalCount { get; private set; }



		private readonly CardRepository _repository;
		private readonly Megatools _megatools;
		private bool _abort;
	}
}