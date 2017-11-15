using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mtgdb.Downloader
{
	public class ImageDownloadProgressReader
	{
		private static readonly string _updateImgDir = AppDir.Update.AddPath("img");
		private readonly ImageSourcesConfig _config;
		private readonly Megatools _megatools;

		public ImageDownloadProgressReader(ImageSourcesConfig config)
		{
			_config = config;
			_megatools = new Megatools();
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
			string signaturesDir = Path.Combine(_updateImgDir, qualityGroup.Quality);
			string signaturesFile = Path.Combine(signaturesDir, Signer.SignaturesFile);
			string signaturesFileBak = signaturesFile + ".bak";

			if (qualityGroup.FileListUrl == null)
				return;

			if (File.Exists(signaturesFile))
			{
				if (File.Exists(signaturesFileBak))
					File.Delete(signaturesFileBak);

				File.Move(signaturesFile, signaturesFileBak);
			}

			_megatools.Download($"Signatures for {qualityGroup.Quality} images", qualityGroup.FileListUrl, signaturesDir, silent: true);

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

				foreach (var megaDir in qualityGroup.MegaDirs)
				{
					var progress = getProgress(qualityGroup, megaDir, imagesOnline);
					if (progress.FilesOnline != null && progress.FilesOnline.Count == 0)
						continue;

					result.Add(progress);
				}
			}

			return result;
		}

		private static ImageDownloadProgress getProgress(QualityGroupConfig qualityGroup, MegaDirConfig megaDir, FileSignature[] imagesOnline)
		{
			var progress = new ImageDownloadProgress
			{
				QualityGroup = qualityGroup,
				MegaDir = megaDir
			};

			var existingSignatures = readExistingSignatures(progress)
				?.ToDictionary(_ => _.Path);

			if (imagesOnline == null)
			{
				progress.FilesDownloaded = existingSignatures;
				return progress;
			}

			progress.FilesOnline = imagesOnline
				.Where(_ => _.IsRelativeTo(megaDir.Subdirectory))
				.Select(_ => _.AsRelativeTo(megaDir.Subdirectory))
				.ToDictionary(_ => _.Path, Str.Comparer);

			progress.FilesDownloaded = new Dictionary<string, FileSignature>(Str.Comparer);
			progress.FilesCorrupted = new Dictionary<string, FileSignature>(Str.Comparer);
			
			foreach (var onlineImage in progress.FilesOnline.Values)
			{
				var existingSignature = existingSignatures?.TryGet(onlineImage.Path);

				if (existingSignature == null)
					continue;

				if (existingSignature.Md5Hash == onlineImage.Md5Hash)
					progress.FilesDownloaded.Add(existingSignature.Path, existingSignature);
				else
					progress.FilesCorrupted.Add(existingSignature.Path, existingSignature);
			}

			return progress;
		}

		private static IList<FileSignature> readExistingSignatures(ImageDownloadProgress progress)
		{
			string targetDirAbsolute = GetTargetDirAbsolute(progress);
			var existingSignaturesFile = Path.Combine(targetDirAbsolute, Signer.SignaturesFile);

			var existingSignatures = Signer.ReadFromFile(existingSignaturesFile);
			return existingSignatures;
		}

		public static void WriteExistingSignatures(ImageDownloadProgress progress, IEnumerable<FileSignature> signatures = null)
		{
			string targetDirAbsolute = GetTargetDirAbsolute(progress);
			var existingSignaturesFile = Path.Combine(targetDirAbsolute, Signer.SignaturesFile);

			signatures = signatures ?? Signer.CreateSignatures(targetDirAbsolute);
			Signer.WriteToFile(existingSignaturesFile, signatures);
		}

		public static string GetTargetDirAbsolute(ImageDownloadProgress progress)
		{
			string targetDirRelative;
			if (progress.MegaDir.Subdirectory == null)
				targetDirRelative = progress.QualityGroup.TargetDirectory;
			else
				targetDirRelative = Path.Combine(progress.QualityGroup.TargetDirectory, progress.MegaDir.Subdirectory);

			var targetDirAbsolute = AppDir.GetRootPath(targetDirRelative);
			return targetDirAbsolute;
		}
	}
}