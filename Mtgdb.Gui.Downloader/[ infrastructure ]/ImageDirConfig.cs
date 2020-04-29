using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	[DataContract]
	public class ImageDirConfig
	{
		[DataMember(Name = "MegaId")]
		public string MegaId { get; set; }

		[DataMember(Name = "Subdir")]
		public FsPath Subdir { get; set; }
	}
}
