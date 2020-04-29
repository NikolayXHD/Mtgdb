using NConfiguration;
using NConfiguration.Joining;
using NConfiguration.Xml;
using Ninject.Activation;
using Ninject.Modules;

namespace Mtgdb
{
	public class CoreModule : NinjectModule
	{
		public override void Load()
		{
			// ReSharper disable once PossibleNullReferenceException
			Kernel.Bind<IAppSettings>()
				.ToMethod(loadSettings)
				.InSingletonScope();

			ApplicationCulture.SetCulture(Str.Culture);
		}

		private static IAppSettings loadSettings(IContext ctx)
		{
			var systemSettings = new XmlSystemSettings("ExtConfigure", AppDir.Executable.Parent().Value);
			var settingsLoader = new SettingsLoader();
			settingsLoader.XmlFileByExtension();
			var settings = settingsLoader.LoadSettings(systemSettings).Joined.ToAppSettings();

			return settings;
		}
	}
}
