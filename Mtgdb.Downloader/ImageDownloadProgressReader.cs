using System;
using System.Collections.Generic;
using System.IO;

namespace Mtgdb.Downloader
{
	public class ImageDownloadProgressReader
	{
		private static readonly string _updateImgDir = AppDir.Update.AddPath("img");
		private readonly ImageSourcesConfig _config;
		private readonly Megatools _megatools;

		public ImageDownloadProgressReader(ImageSourcesConfig config, Megatools megatools)
		{
			_config = config;
			_megatools = megatools;
		}

		public void DownloadSignatures(string quality)
		{
			foreach (var qualityGroup in _config.QualityGroups)
			{
				if (!Str.Equals(quality, qualityGroup.Quality))
					continue;

				downloadSignatures(qualityGroup);
			}
		}

		private void downloadSignatures(QualityGroupConfig qualityGroup)
		{
			if (qualityGroup.FileListMegaId == null && qualityGroup.FileListGdriveId == null)
				return;

			string signaturesDir = Path.Combine(_updateImgDir, qualityGroup.Quality);
			string signaturesFile = Path.Combine(signaturesDir, Signer.SignaturesFile);
			string signaturesFileBak = signaturesFile + ".bak";

			if (File.Exists(signaturesFile))
			{
				if (File.Exists(signaturesFileBak))
					File.Delete(signaturesFileBak);

				File.Move(signaturesFile, signaturesFileBak);
			}

			if (qualityGroup.FileListMegaId != null)
			{
				string megaUrl = _config.MegaPrefix + qualityGroup.FileListMegaId;
				_megatools.Download(megaUrl, signaturesDir, $"Signatures for {qualityGroup.Quality} images", silent: true);
			}
			else if (qualityGroup.FileListGdriveId != null)
			{
				var webClient = new WebClientBase();
				string gdriveUrl = _config.GdrivePrefix + qualityGroup.FileListGdriveId;
				string fileListArchive = signaturesDir.AddPath("filelist.7z");
				Console.WriteLine($"Downloading {fileListArchive} from {gdriveUrl}");
				webClient.DownloadFile(gdriveUrl, fileListArchive);
				new SevenZip(silent: true).Extract(fileListArchive, signaturesDir);
			}

			if (!File.Exists(signaturesFile))
			{
				if (File.Exists(signaturesFileBak))
				{
					Console.WriteLine("Failed to download signatures");

					Console.WriteLine("Move {0} {1}", signaturesFileBak, signaturesFile);
					File.Move(signaturesFileBak, signaturesFile);
				}
			}
			else
			{
				File.Delete(signaturesFileBak);
			}
		}

		public IList<ImageDownloadProgress> GetProgress()
		{
			var result = new List<ImageDownloadProgress>();
			foreach (var qualityGroup in _config.QualityGroups)
			{
				string signaturesDir = Path.Combine(_updateImgDir, qualityGroup.Quality);
				string signaturesFile = Path.Combine(signaturesDir, Signer.SignaturesFile);

				var imagesOnline = Signer.ReadFromFile(signaturesFile);

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

			var existingSignaturesFile = Path.Combine(targetSubdirectory, Signer.SignaturesFile);

			signatures = signatures ?? Signer.CreateSignatures(targetSubdirectory);
			Signer.WriteToFile(existingSignaturesFile, signatures);
		}
	}
}