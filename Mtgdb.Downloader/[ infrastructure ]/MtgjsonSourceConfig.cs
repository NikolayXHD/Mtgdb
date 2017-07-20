using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	[DataContract(Name = "MtgjsonSource")]
	public class MtgjsonSourceConfig
	{
		[DataMember(Name = "Url")]
		public string Url { get; set; }
	}
}