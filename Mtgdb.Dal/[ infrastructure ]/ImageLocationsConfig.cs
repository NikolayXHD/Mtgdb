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
		public string Zoom { get; set; }

		[DataMember(Name = "Art")]
		public bool? Art { get; set; }

		[DataMember(Name = "ReadMetadataFromAttributes")]
		public bool? ReadMetadataFromAttributes { get; set; }

		[DataMember(Name = "Set")]
		public string Set { get; set; }

		[DataMember(Name = "Exclude")]
		public string Exclude { get; set; }
	}

	public static class ImageType
	{
		public static bool Art(this DirectoryConfig c)
		{
			return c.Art == true;
		}

		public static bool Zoom(this DirectoryConfig c)
		{
			if (c.Art == true)
				return false;

			return Str.Equals(c.Zoom, "True") || Str.Equals(c.Zoom, "Any");
		}

		public static bool Small(this DirectoryConfig c)
		{
			if (c.Art == true)
				return false;

			return c.Zoom == null || Str.Equals(c.Zoom, "False") || Str.Equals(c.Zoom, "Any");
		}
	}
}
