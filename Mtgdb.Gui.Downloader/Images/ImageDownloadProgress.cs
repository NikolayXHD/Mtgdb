using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Downloader
{
	public class ImageDownloadProgress
	{
		public ImageDownloadProgress(
			ImageSourcesConfig imageSource,
			QualityGroupConfig qualityGroup,
			ImageDirConfig dir,
			FileSignature[] imagesOnline)
		{
			ImageSource = imageSource;
			QualityGroup = qualityGroup;
			Dir = dir;

			TargetDirectory = QualityGroup.TargetDirectory.ToAppRootedPath();
			TargetSubdirectory = TargetDirectory.AddPath(Dir.Subdir);

			MegaUrl = string.IsNullOrEmpty(Dir.MegaId) ? null : ImageSource.MegaPrefix + Dir.MegaId;

			var existingSignatures = readExistingSignatures()
				?.ToDictionary(_ => _.Path, Str.Comparer);

			if (imagesOnline == null)
			{
				FilesDownloaded = existingSignatures;
				return;
			}

			FilesOnline = imagesOnline
				.Where(_ => _.IsRelativeTo(Dir.Subdir))
				.Select(_ => _.AsRelativeTo(Dir.Subdir))
				.ToDictionary(_ => _.Path, Str.Comparer);

			FilesDownloaded = new Dictionary<string, FileSignature>(Str.Comparer);
			FilesCorrupted = new Dictionary<string, FileSignature>(Str.Comparer);

			foreach (var onlineImage in FilesOnline.Values)
			{
				var existingSignature = existingSignatures?.TryGet(onlineImage.Path);

				if (existingSignature == null)
					continue;

				if (existingSignature.Md5Hash == onlineImage.Md5Hash)
					FilesDownloaded.Add(existingSignature.Path, existingSignature);
				else
					FilesCorrupted.Add(existingSignature.Path, existingSignature);
			}
		}

		private IList<FileSignature> readExistingSignatures()
		{
			var existingSignaturesFile = TargetSubdirectory.AddPath(Signer.SignaturesFile);
			var existingSignatures = Signer.ReadFromFile(existingSignaturesFile);
			return existingSignatures;
		}

		public ImageSourcesConfig ImageSource { get; }
		public QualityGroupConfig QualityGroup { get; }
		public ImageDirConfig Dir { get; }

		public Dictionary<string, FileSignature> FilesOnline { get; }
		public Dictionary<string, FileSignature> FilesCorrupted { get; }
		public Dictionary<string, FileSignature> FilesDownloaded { get; }

		public string TargetDirectory { get; }
		public string TargetSubdirectory { get; }

		public string MegaUrl { get; }

		public bool MayBeComplete
		{
			get
			{
				bool result = FilesOnline == null
					? FilesDownloaded?.Count > 0
					: FilesOnline.Count == FilesDownloaded?.Count;

				return result;
			}
		}

		public override string ToString()
		{
			return $"{QualityGroup.Quality} {Dir.Subdir} {FilesDownloaded?.Count ?? 0} / {FilesOnline?.Count ?? 0}";
		}
	}
}
