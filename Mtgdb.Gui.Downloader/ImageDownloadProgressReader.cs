using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

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
			if (qualityGroup.FileListMegaId == null && qualityGroup.FileListGdriveId == null)
				return;

			(string signaturesDir, string signaturesFile) = getSignaturesFile(qualityGroup);
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
				await _megatools.Download(megaUrl, signaturesDir,
					name: $"Signatures for {qualityGroup.Quality} images", silent: true, token: token);
			}
			else if (qualityGroup.FileListGdriveId != null)
			{
				var webClient = new GdriveWebClient();
				string gdriveUrl = _config.GdrivePrefix + qualityGroup.FileListGdriveId;
				string fileListArchive = signaturesDir.AddPath("filelist.7z");
				Console.Write($"Downloading {fileListArchive} from {gdriveUrl} ... ");
				await webClient.DownloadFromGdrive(gdriveUrl, signaturesDir, token);
				if (File.Exists(fileListArchive))
				{
					Console.WriteLine("done");
					new SevenZip(silent: true).Extract(fileListArchive, signaturesDir);
				}
				else
					Console.WriteLine("FAILED");
			}

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
	}
}
