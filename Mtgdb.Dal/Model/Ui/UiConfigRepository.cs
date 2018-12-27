using System.IO;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class UiConfigRepository
	{
		private UiConfig _config;
		public UiConfig Config => _config ?? (_config = readConfig());

		public int[] UiScaleValues => _uiScaleValues;

		public void Save()
		{
			var serialized = JsonConvert.SerializeObject(Config, Formatting.Indented);
			File.WriteAllText(_fileName, serialized);
		}

		private static UiConfig readConfig()
		{
			if (!File.Exists(_fileName))
				return new UiConfig();

			var serialized = File.ReadAllText(_fileName);
			var config = JsonConvert.DeserializeObject<UiConfig>(serialized);
			return config;
		}

		private static readonly string _fileName = AppDir.History.AddPath("ui.json");
		private static readonly int[] _uiScaleValues = { UiConfig.DefaultUiScalePercent, 125, 150, 200 };
	}
}