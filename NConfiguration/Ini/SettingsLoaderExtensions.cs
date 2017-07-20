using NConfiguration.Joining;

namespace NConfiguration.Ini
{
	public static class SettingsLoaderExtensions
	{
		public static FileSearcher IniFileByExtension(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(IniFileSettings.Create, "ini");
			loader.AddHandler<IncludeFileConfig>("IncludeFile", searcher);
			return searcher;
		}

		public static FileSearcher IniFileBySection(this SettingsLoader loader)
		{
			var searcher = new FileSearcher(IniFileSettings.Create);
			loader.AddHandler<IncludeFileConfig>("IncludeIniFile", searcher);
			return searcher;
		}
	}
}

