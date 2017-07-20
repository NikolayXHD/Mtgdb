using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	[DataContract]
	public class MegaDirConfig
	{
		[DataMember(Name = "Url")]
		public string Url { get; set; }

		[DataMember(Name = "Subdir")]
		public string Subdirectory { get; set; }
	}
}
