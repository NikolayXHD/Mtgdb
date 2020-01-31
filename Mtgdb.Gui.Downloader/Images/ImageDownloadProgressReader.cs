using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;

namespace Mtgdb.Downloader
{
	public class ImageDownloadProgressReader
	{
		private static readonly string _updateImgDir = AppDir.Update.AddPath("img");
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
			File.Exists(getSignaturesFile(qualityGroup).File);

		private async Task downloadSignatures(QualityGroupConfig qualityGroup, CancellationToken token)
		{
			if (qualityGroup.FileListMegaId == null && qualityGroup.YandexName == null)
				return;

			(string signaturesDir, string signaturesFile) = getSignaturesFile(qualityGroup);
			string signaturesFileBak = signaturesFile + ".bak";

			if (File.Exists(signaturesFile))
			{
				if (File.Exists(signaturesFileBak))
					File.Delete(signaturesFileBak);

				File.Move(signaturesFile, signaturesFileBak);
			}

			Directory.CreateDirectory(signaturesDir);

			if (qualityGroup.FileListMegaId != null)
			{
				string megaUrl = _config.MegaPrefix + qualityGroup.FileListMegaId;
				await _megatools.Download(megaUrl, signaturesDir,
					name: $"Signatures for {qualityGroup.Quality} images", silent: true, token: token);
			}
			else if (qualityGroup.YandexName != null)
			{
				string fileListArchive = signaturesDir.AddPath("filelist.7z");
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
					if (File.Exists(fileListArchive))
						new SevenZip(silent: true).Extract(fileListArchive, signaturesDir);
				}
			}
			else
				throw new ArgumentException($"No downloader can get filelist for quality {qualityGroup.Quality}");


			if (!File.Exists(signaturesFile))
			{
				if (File.Exists(signaturesFileBak))
				{
					Console.WriteLine("Failed to unzip signatures");

					Console.WriteLine("Move {0} {1}", signaturesFileBak, signaturesFile);
					File.Move(signaturesFileBak, signaturesFile);
				}
			}
			else
			{
				File.Delete(signaturesFileBak);
			}
		}

		public IReadOnlyList<ImageDownloadProgress> GetProgress()
		{
			var result = new List<ImageDownloadProgress>();
			foreach (var qualityGroup in _config.QualityGroups)
			{
				(_, string signaturesFile) = getSignaturesFile(qualityGroup);
				var imagesOnline = Signer.ReadFromFile(signaturesFile);

				if (qualityGroup.YandexName != null && imagesOnline != null)
					qualityGroup.Dirs = imagesOnline
						.Select(_ => Path.GetDirectoryName(_.Path))
						.Distinct()
						.OrderBy(_ => _, Str.Comparer)
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

		public static void WriteExistingSignatures(ImageDownloadProgress progress, IEnumerable<FileSignature> signatures = null)
		{
			string targetSubdirectory = progress.TargetSubdirectory;
			string existingSignaturesFile = Path.Combine(targetSubdirectory, Signer.SignaturesFile);

			signatures ??= Signer.CreateSignatures(targetSubdirectory);
			Signer.WriteToFile(existingSignaturesFile, signatures);
		}

		private static (string Directory, string File) getSignaturesFile(QualityGroupConfig qualityGroup)
		{
			string dir = getSignaturesDir(qualityGroup);
			return (Directory: dir, File: Path.Combine(dir, Signer.SignaturesFile));
		}

		private static string getSignaturesDir(QualityGroupConfig qualityGroup) =>
			Path.Combine(_updateImgDir, qualityGroup.Name);

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
