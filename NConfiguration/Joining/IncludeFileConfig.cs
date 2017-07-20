using System.Runtime.Serialization;


namespace NConfiguration.Joining
{
	[DataContract(Name = "IncludeFile")]
	public class IncludeFileConfig
	{
		[DataMember(Name = "Path", IsRequired = true)]
		public string Path { get; set;}

		[DataMember(Name = "Search", IsRequired = false)]
		public SearchMode Search { get; set;}

		[DataMember(Name = "Include", IsRequired = false)]
		public IncludeMode Include { get; set;}

		[DataMember(Name = "Required", IsRequired = false)]
		public bool Required { get; set;}
	}
}

