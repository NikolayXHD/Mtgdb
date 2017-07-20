using System.Collections.Generic;

namespace Mtgdb.Downloader
{
	public class ImageDownloadProgress
	{
		public ImageDownloadProgress()
		{
		}

		public QualityGroupConfig QualityGroup { get; set; }
		public MegaDirConfig MegaDir { get; set; }

		public Dictionary<string, FileSignature> FilesOnline { get; set; }
		public Dictionary<string, FileSignature> FilesCorrupted { get; set; }
		public Dictionary<string, FileSignature> FilesDownloaded { get; set; }

		public bool MayBeComplete
		{
			get
			{
				bool result = FilesOnline == null ? FilesDownloaded?.Count > 0 : FilesOnline.Count == FilesDownloaded?.Count;
				return result;
			}
		}
	}
}