using NConfiguration.Joining;

namespace NConfiguration.Json
{
	public static class SettingsLoaderExtensions
	{
		public static FileSearcher JsonFileByExtension(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(JsonFileSettings.Create, "js", "json");
			loader.AddHandler<IncludeFileConfig>("IncludeFile", searcher);
			return searcher;
		}

		public static FileSearcher JsonFileBySection(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(JsonFileSettings.Create);
			loader.AddHandler<IncludeFileConfig>("IncludeJsonFile", searcher);
			return searcher;
		}
	}
}

