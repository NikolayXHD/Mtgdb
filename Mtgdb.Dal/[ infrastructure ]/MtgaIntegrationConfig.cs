using System.Runtime.Serialization;

namespace Mtgdb.Dal
{
	[DataContract(Name = "MtgaIntegration")]
	public class MtgaIntegrationConfig
	{
		[DataMember(Name = "CardLibraryFile")]
		public string CardLibraryFile { get; set; }

		[DataMember(Name = "LogFile")]
		public string LogFile { get; set; }
	}
}