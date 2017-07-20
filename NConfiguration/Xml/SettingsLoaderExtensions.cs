using NConfiguration.Joining;

namespace NConfiguration.Xml
{
	public static class SettingsLoaderExtensions
	{
		public static FileSearcher XmlFileByExtension(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(XmlFileSettings.Create, "xml", "config");
			loader.AddHandler<IncludeFileConfig>("IncludeFile", searcher);
			return searcher;
		}

		public static FileSearcher XmlFileBySection(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(XmlFileSettings.Create);
			loader.AddHandler<IncludeFileConfig>("IncludeXmlFile", searcher);
			return searcher;
		}
	}
}

