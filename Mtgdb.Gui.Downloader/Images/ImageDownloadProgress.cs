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
			TargetSubdirectory = Dir.Subdir.Value == null
				? TargetDirectory
				: TargetDirectory.Join(Dir.Subdir);

			MegaUrl = string.IsNullOrEmpty(Dir.MegaId) ? null : ImageSource.MegaPrefix + Dir.MegaId;

			var existingSignatures = readExistingSignatures()
				?.ToDictionary(_ => _.Path);

			if (imagesOnline == null)
			{
				FilesDownloaded = existingSignatures;
				return;
			}

			FilesOnline = Dir.Subdir.Value == null
				? imagesOnline.ToDictionary(_ => _.Path)
				: imagesOnline.Where(_ => Dir.Subdir.IsParentOf(_.Path))
				.Select(_ => new FileSignature
				{
					Path = _.Path.RelativeTo(Dir.Subdir).Intern(true),
					Md5Hash = _.Md5Hash
				})
				.ToDictionary(_ => _.Path);

			FilesDownloaded = new Dictionary<FsPath, FileSignature>();
			FilesCorrupted = new Dictionary<FsPath, FileSignature>();

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
			FsPath existingSignaturesFile = TargetSubdirectory.Join(Signer.SignaturesFile);
			var existingSignatures = Signer.ReadFromFile(existingSignaturesFile, internPath: true);
			return existingSignatures;
		}

		public ImageSourcesConfig ImageSource { get; }
		public QualityGroupConfig QualityGroup { get; }
		public ImageDirConfig Dir { get; }

		public Dictionary<FsPath, FileSignature> FilesOnline { get; }
		public Dictionary<FsPath, FileSignature> FilesCorrupted { get; }
		public Dictionary<FsPath, FileSignature> FilesDownloaded { get; }

		public FsPath TargetDirectory { get; }
		public FsPath TargetSubdirectory { get; }

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
