using System.Collections.Generic;
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

		public IList<DirectoryConfig> GetEnabledDirectories(IEnumerable<string> overrideEnabledGroups = null)
		{
			var rootsByName = Roots.ToDictionary(_ => _.Name, Str.Comparer);

			foreach (var directory in Directories)
			{
				if (!string.IsNullOrEmpty(directory.Root))
				{
					directory.Path = getRootPath(rootsByName[directory.Root]).Join(directory.Path);
					if (!directory.Path.HasValue())
						directory.Path = FsPath.None;

					directory.Exclude = string.Join(";",
						Sequence.From(
								rootsByName[directory.Root].Exclude,
								directory.Exclude)
							.Where(exclude => !string.IsNullOrEmpty(exclude)));
				}

				directory.Path = directory.Path.ToAppRootedPath();
			}

			var enabledGroups = Runtime.IsLinux
				? EnabledGroupsLinux ?? EnabledGroups
				: EnabledGroups;

			if (enabledGroups == null)
				return Directories;

			var groups = new HashSet<string>(overrideEnabledGroups ?? enabledGroups.Split(';', ' ', ',', '|'));

			return Directories
				.Where(_ => groups.Contains(_.Group ?? string.Empty))
				.ToList();

			FsPath getRootPath(RootConfig root)
			{
				var basePath = string.IsNullOrEmpty(root.Root)
					? null
					: (FsPath?)getRootPath(rootsByName[root.Root]);

				var path = root.LinuxPath.HasValue() && Runtime.IsLinux
					? root.LinuxPath
					: root.Path;

				if (basePath.HasValue)
					return basePath.Value.Join(path);

				return path;
			}
		}

		[DataMember(Name = "EnabledGroups")]
		private string EnabledGroups { get; set; }

		[DataMember(Name = "EnabledGroupsLinux")]
		private string EnabledGroupsLinux { get; set; }
	}

	[DataContract]
	public class DirectoryConfig
	{
		[DataMember(Name = "Root")]
		public string Root { get; set; }

		[DataMember(Name = "Path")]
		public FsPath Path { get; set; }

		[DataMember(Name = "Zoom")]
		public string Zoom { get; set; }

		[DataMember(Name = "Art")]
		public bool? Art { get; set; }

		[DataMember(Name = "ReadMetadataFromAttributes")]
		public bool? ReadMetadataFromAttributes { get; set; }

		[DataMember(Name = "Set")]
		public string Set { get; set; }

		[DataMember(Name = "Priority")]
		public int? Priority { get; set; }

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
		public FsPath Path { get; set; }

		[DataMember(Name = "LinuxPath")]
		public FsPath LinuxPath { get; set; }

		[DataMember(Name = "Root")]
		public string Root { get; set; }

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
