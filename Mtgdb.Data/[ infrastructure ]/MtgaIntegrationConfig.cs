using System.Runtime.Serialization;

namespace Mtgdb.Data
{
	[DataContract(Name = "MtgaIntegration")]
	public class MtgaIntegrationConfig
	{
		[DataMember(Name = "CardLibraryFile")]
		public FsPath CardLibraryFile { get; set; }

		[DataMember(Name = "LogFile")]
		public FsPath LogFile { get; set; }
	}
}
