using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	[DataContract(Name = "ImageSources")]
	public class ImageSourcesConfig
	{
		[DataMember(Name = "QualityGroup")]
		public QualityGroupConfig[] QualityGroups { get; set; }
	}
}