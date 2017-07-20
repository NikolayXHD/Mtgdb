using System;
using System.Runtime.Serialization;

namespace NConfiguration.Monitoring
{
	[DataContract(Name="WatchFile")]
	public class WatchFileConfig
	{
		[DataMember(Name = "Mode", IsRequired = false)]
		public WatchMode Mode { get; set; }

		[DataMember(Name = "Delay", IsRequired = false)]
		public TimeSpan? Delay { get; set;}

		[DataMember(Name = "Check", IsRequired = false)]
		public CheckMode? Check { get; set; }
	}
}

