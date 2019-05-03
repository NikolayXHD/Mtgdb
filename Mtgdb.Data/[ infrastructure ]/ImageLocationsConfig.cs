using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Mtgdb.Data
{
	[DataContract(Name = "ImageLocations")]
	public class ImageLocationsConfig
	{
		[DataMember(Name = "Directory")]
		public DirectoryConfig[] Directories { get; set; }

		[DataMember(Name = "Root")]
		public RootConfig[] Roots { get; set; }

		public IList<DirectoryConfig> GetEnabledDirectories(IEnumerable<string> enabledGroups = null)
		{
			var rootsByName = Roots.ToDictionary(_ => _.Name, Str.Comparer);

			foreach (var directory in Directories)
			{
				if (!string.IsNullOrEmpty(directory.Root))
				{
					directory.Path = Path.Combine(
							rootsByName[directory.Root].Path,
							directory.Path)
						.NullIfEmpty();

					directory.Exclude = string.Join(";",
						Sequence.From(
								rootsByName[directory.Root].Exclude,
								directory.Exclude)
							.Where(exclude => !string.IsNullOrEmpty(exclude)));
				}

				directory.Path = directory.Path.ToAppRootedPath();
			}

			if (EnabledGroups == null)
				return Directories;

			var groups = new HashSet<string>(enabledGroups ?? EnabledGroups.Split(';', ' ', ',', '|'));

			return Directories
				.Where(_ => groups.Contains(_.Group ?? string.Empty))
				.ToList();
		}

		[DataMember(Name = "EnabledGroups")]
		public string EnabledGroups { get; set; }
	}

	[DataContract]
	public class DirectoryConfig
	{
		[DataMember(Name = "Root")]
		public string Root { get; set; }

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

		[DataMember(Name = "Group")]
		public string Group { get; set; }
	}

	public class RootConfig
	{
		[DataMember(Name = "Name")]
		public string Name { get; set; }

		[DataMember(Name = "Path")]
		public string Path { get; set; }

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
