using System.Runtime.Serialization;

namespace Mtgdb.Dal
{
	[DataContract(Name = "ImageLocations")]
	public class ImageLocationsConfig
	{
		[DataMember(Name = "Directory")]
		public DirectoryConfig[] Directories { get; set; }
	}

	[DataContract]
	public class DirectoryConfig
	{
		[DataMember(Name = "Path")]
		public string Path { get; set; }

		[DataMember(Name = "Zoom")]
		public bool? Zoom { get; set; }

		[DataMember(Name = "Art")]
		public bool? Art { get; set; }

		[DataMember(Name = "ReadMetadataFromAttributes")]
		public bool? ReadMetadataFromAttributes { get; set; }
	}
}
