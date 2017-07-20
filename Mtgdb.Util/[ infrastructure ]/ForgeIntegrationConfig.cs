using System.Runtime.Serialization;

namespace Mtgdb.Util
{
	[DataContract(Name = "ForgeIntegration")]
	public class ForgeIntegrationConfig
	{
		[DataMember(Name = "CardPicsPath")]
		public string CardPicsPath { get; set; }

		[DataMember(Name = "CardPicsBackupPath")]
		public string CardPicsBackupPath { get; set; }

		[DataMember(Name = "Verbose")]
		public bool? Verbose { get; set; }
	}
}