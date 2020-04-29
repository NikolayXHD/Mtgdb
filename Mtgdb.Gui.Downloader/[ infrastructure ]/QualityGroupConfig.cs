using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	public class QualityGroupConfig
	{
		[DataMember(Name = "Quality")]
		public string Quality { get; set; }

		[DataMember(Name = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "TargetDirectory")]
		public FsPath TargetDirectory { get; set; }

		[DataMember(Name = "FileListGdriveId")]
		public string FileListGdriveId { get; set; }

		[DataMember(Name = "FileListMegaId")]
		public string FileListMegaId { get; set; }

		[DataMember(Name = "Dir")]
		public ImageDirConfig[] Dirs { get; set; }

		[DataMember(Name = "YandexName")]
		public string YandexName { get; set; }
	}
}
