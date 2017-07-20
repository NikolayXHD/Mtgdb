using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	public class QualityGroupConfig
	{
		[DataMember(Name = "Quality")]
		public string Quality { get; set; }

		[DataMember(Name = "TargetDirectory")]
		public string TargetDirectory { get; set; }

		[DataMember(Name = "FileListUrl")]
		public string FileListUrl { get; set; }

		[DataMember(Name = "MegaDir")]
		public MegaDirConfig[] MegaDirs { get; set; }
	}
}