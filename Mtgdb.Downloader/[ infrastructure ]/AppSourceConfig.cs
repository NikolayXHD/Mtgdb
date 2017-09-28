using System.Runtime.Serialization;

namespace Mtgdb.Downloader
{
	[DataContract(Name = "AppSource")]
	public class AppSourceConfig
	{
		[DataMember(Name = "FileListUrl")]
		public string FileListUrl { get; set; }

		[DataMember(Name = "ZipUrl")]
		public string ZipUrl { get; set; }

		[DataMember(Name = "NotificationsUrl")]
		public string NotificationsUrl { get; set; }
	}
}