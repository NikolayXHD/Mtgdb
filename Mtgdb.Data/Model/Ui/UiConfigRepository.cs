using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class UiConfigRepository
	{
		static UiConfigRepository()
		{
			_serializer = new JsonSerializer();

			_serializer.Converters.Add(
				new UnformattedJsonConverter(type =>
					typeof(IDictionary<string, int>).IsAssignableFrom(type) ||
					typeof(IEnumerable<string>).IsAssignableFrom(type)));
		}

		public void Save()
		{
			using var writer = _fileName.CreateText();
			using var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' };
			_serializer.Serialize(jsonWriter, Config);
		}

		private static UiConfig readConfig()
		{
			if (!_fileName.IsFile())
				return new UiConfig();

			var serialized = _fileName.ReadAllText();
			var config = JsonConvert.DeserializeObject<UiConfig>(serialized);
			return config;
		}

		private UiConfig _config;
		public UiConfig Config => _config ??= readConfig();

		private static readonly FsPath _fileName = AppDir.History.Join("ui.json");
		private static readonly JsonSerializer _serializer;
	}
}
