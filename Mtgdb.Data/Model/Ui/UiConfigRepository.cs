using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class UiConfigRepository
	{
		static UiConfigRepository()
		{
			_serializer = new JsonSerializer();

			_serializer.Converters.Add(
				new UnformattedJsonConverter(objectType =>
					typeof(IDictionary<string, int>).IsAssignableFrom(objectType) ||
					typeof(IEnumerable<string>).IsAssignableFrom(objectType)));
		}

		public void Save()
		{
			using (var writer = File.CreateText(_fileName))
			using (var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' })
				_serializer.Serialize(jsonWriter, Config);
		}

		private static UiConfig readConfig()
		{
			if (!File.Exists(_fileName))
				return new UiConfig();

			var serialized = File.ReadAllText(_fileName);
			var config = JsonConvert.DeserializeObject<UiConfig>(serialized);
			return config;
		}

		private UiConfig _config;
		public UiConfig Config => _config ?? (_config = readConfig());

		private static readonly string _fileName = AppDir.History.AddPath("ui.json");
		private static readonly JsonSerializer _serializer;
	}
}