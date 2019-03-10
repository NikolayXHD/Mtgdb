using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	[DataContract]
	public class ImageDirConfig
	{
		[DataMember(Name = "MegaId")]
		public string MegaId { get; set; }

		[DataMember(Name = "GdriveId")]
		public string GdriveId { get; set; }

		[DataMember(Name = "Subdir")]
		public string Subdirectory { get; set; }
	}
}
