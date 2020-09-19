using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;

namespace Mtgdb.Downloader
{
	public class ImageDownloadProgressReader
	{
		private static readonly FsPath _updateImgDir = AppDir.Update.Join("img");
		private readonly ImageSourcesConfig _config;
		private readonly Megatools _megatools;

		[UsedImplicitly] // by ninject
		public ImageDownloadProgressReader(ImageSourcesConfig config, Megatools megatools)
		{
			_config = config;
			_megatools = megatools;
		}

		public async Task DownloadSignatures(string quality, CancellationToken token)
		{
			foreach (var qualityGroup in _config.QualityGroups)
				if (Str.Equals(quality, qualityGroup.Quality))
					await downloadSignatures(qualityGroup, token);
		}

		public bool SignaturesFileExist(QualityGroupConfig qualityGroup) =>
			getSignaturesFile(qualityGroup).File.IsFile();

		private async Task downloadSignatures(QualityGroupConfig qualityGroup, CancellationToken token)
		{
			if (qualityGroup.FileListMegaId == null && qualityGroup.YandexName == null)
				return;

			(FsPath signaturesDir, FsPath signaturesFile) = getSignaturesFile(qualityGroup);
			FsPath signaturesFileBak = signaturesFile.Concat(".bak");

			if (signaturesFile.IsFile())
			{
				if (signaturesFileBak.IsFile())
					signaturesFileBak.DeleteFile();

				signaturesFile.MoveFileTo(signaturesFileBak);
			}

			signaturesDir.CreateDirectory();

			if (qualityGroup.FileListMegaId != null)
			{
				string megaUrl = _config.MegaPrefix + qualityGroup.FileListMegaId;
				await _megatools.Download(megaUrl, signaturesDir,
					name: $"Signatures for {qualityGroup.Quality} images", silent: true, token: token);
			}
			else if (qualityGroup.YandexName != null)
			{
				FsPath fileListArchive = signaturesDir.Join("filelist.7z");
				var client = new YandexDiskClient();
				Console.Write("{0} filelist.7z: get YandexDisk download url ... ", qualityGroup.Name);
				var url = await client.GetFilelistDownloadUrl(_config, qualityGroup, token);
				Console.Write("downloading ... ");

				bool success;
				try
				{
					await client.DownloadFile(url, fileListArchive, token);
					Console.WriteLine($"done");
					success = true;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"failed: {ex.Message}");
					_log.Warn(ex, $"Failed download {fileListArchive} from {url}");
					success = false;
				}

				if (success)
				{
					if (fileListArchive.IsFile())
						new SevenZip(silent: true).Extract(fileListArchive, signaturesDir);
				}
			}
			else
				throw new ArgumentException($"No downloader can get filelist for quality {qualityGroup.Quality}");


			if (!signaturesFile.IsFile())
			{
				if (signaturesFileBak.IsFile())
				{
					Console.WriteLine("Failed to unzip signatures");

					Console.WriteLine("Move {0} {1}", signaturesFileBak, signaturesFile);
					signaturesFileBak.MoveFileTo(signaturesFile);
				}
			}
			else
			{
				signaturesFileBak.DeleteFile();
			}
		}

		public IReadOnlyList<ImageDownloadProgress> GetProgress()
		{
			var result = new List<ImageDownloadProgress>();
			foreach (var qualityGroup in _config.QualityGroups)
			{
				var imagesOnline = GetOnlineImages(qualityGroup);

				if (qualityGroup.YandexName != null && imagesOnline != null)
					qualityGroup.Dirs = imagesOnline
						.Select(_ => _.Path.Parent())
						.Distinct()
						.OrderBy(_ => _.Value, Str.Comparer)
						.Select(_ => new ImageDirConfig { Subdir = _ }).ToArray();

				foreach (var dir in qualityGroup.Dirs)
				{
					var progress = new ImageDownloadProgress(_config, qualityGroup, dir, imagesOnline);

					if (progress.FilesOnline != null && progress.FilesOnline.Count == 0)
						continue;

					result.Add(progress);
				}
			}

			return result;
		}

		internal ImageSourcesConfig Config =>
			_config;

		internal FileSignature[] GetOnlineImages(QualityGroupConfig qualityGroup)
		{
			(_, FsPath signaturesFile) = getSignaturesFile(qualityGroup);
			// do not intern image path here because it contains parent directory
			var imagesOnline = Signer.ReadFromFile(signaturesFile, internPath: false);
			return imagesOnline;
		}

		public static void WriteExistingSignatures(ImageDownloadProgress progress, IEnumerable<FileSignature> signatures = null)
		{
			FsPath targetSubdirectory = progress.TargetSubdirectory;
			FsPath existingSignaturesFile = targetSubdirectory.Join(Signer.SignaturesFile);

			signatures ??= Signer.CreateSignatures(targetSubdirectory);
			Signer.WriteToFile(existingSignaturesFile, signatures);
		}

		private static (FsPath Directory, FsPath File) getSignaturesFile(QualityGroupConfig qualityGroup)
		{
			FsPath dir = getSignaturesDir(qualityGroup);
			return (Directory: dir, File: dir.Join(Signer.SignaturesFile));
		}

		private static FsPath getSignaturesDir(QualityGroupConfig qualityGroup) =>
			_updateImgDir.Join(qualityGroup.Name);

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
