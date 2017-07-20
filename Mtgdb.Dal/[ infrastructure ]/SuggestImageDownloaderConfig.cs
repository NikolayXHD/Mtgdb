using System.Runtime.Serialization;

namespace Mtgdb.Dal
{
	[DataContract(Name = "SuggestImageDownloader")]
	public class SuggestImageDownloaderConfig
	{
		[DataMember(Name = "Enabled")]
		public bool? Enabled { get; set; }
	}
}
