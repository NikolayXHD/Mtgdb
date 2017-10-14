using System;
using System.Collections.Generic;
using System.IO;
using Mtgdb.Controls;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	public class HistoryModel
	{
		private List<GuiSettings> _settingsHistory;
		private int _settingsIndex;
		private readonly int _maxDepth;

		private string HistoryFile => AppDir.History.AddPath($"{Id}.json");

		public string Id { get; set; }

		public HistoryModel(string id, string language, int? maxDepth)
		{
			_maxDepth = maxDepth ?? 100;
			Id = id;
			Directory.CreateDirectory(AppDir.History);

			if (File.Exists(HistoryFile))
				loadState();
			else
			{
				_settingsHistory = new List<GuiSettings>();
				var defaultSettings = new GuiSettings { Language = language };
				Add(defaultSettings);
			}
		}

		public GuiSettings Current => _settingsHistory[_settingsIndex];

		public bool CanRedo
		{
			get
			{
				validate();
				return _settingsIndex < _settingsHistory.Count - 1;
			}
		}

		public bool CanUndo
		{
			get
			{
				validate();
				return _settingsIndex > 0;
			}
		}


		public void Add(GuiSettings settings)
		{
			_settingsHistory.Add(settings);
			
			_settingsIndex = _settingsHistory.Count - 1;
		}

		public bool Undo()
		{
			validate();

			if (_settingsIndex > 0)
			{
				_settingsIndex--;
				return true;
			}

			return false;
		}

		public bool Redo()
		{
			validate();

			if (_settingsIndex < _settingsHistory.Count - 1)
			{
				_settingsIndex++;
				return true;
			}

			return false;
		}

		public void Save()
		{
			int index = _settingsIndex;
			string serialized = serialize(_settingsHistory, _maxDepth, index);

			File.WriteAllText(HistoryFile, serialized);
		}

		private static string serialize(List<GuiSettings> settingsHistory, int maxDepth, int settingsIndex)
		{
			int minSaveIndex = settingsIndex - maxDepth;
			int saveCount = 1 + 2*maxDepth;

			if (minSaveIndex < 0)
				minSaveIndex = 0;

			if (saveCount > settingsHistory.Count - minSaveIndex)
				saveCount = settingsHistory.Count - minSaveIndex;

			var state = new HistoryState
			{
				SettingsHistory = settingsHistory.SkipFirst(minSaveIndex).TakeFirst(saveCount),
				SettingsIndex = settingsIndex - minSaveIndex
			};

			var settings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
			};

			settings.Converters.Add(new CustomConverter());

			string serialized = JsonConvert.SerializeObject(state, settings);
			return serialized;
		}

		private void loadState()
		{
			var serialized = File.ReadAllText(HistoryFile);
			var state = JsonConvert.DeserializeObject<HistoryState>(serialized);

			_settingsHistory = state.SettingsHistory;
			_settingsIndex = state.SettingsIndex;
		}

		private void validate()
		{
			if (_settingsIndex < 0 || _settingsIndex >= _settingsHistory.Count)
				throw new IndexOutOfRangeException();
		}

		public string DeckFile { get; set; }
		public string DeckName { get; set; }
	}

	internal class CustomConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return
				typeof (IEnumerable<FilterValueState>).IsAssignableFrom(objectType) ||
				typeof (IDictionary<string, int>).IsAssignableFrom(objectType) ||
				typeof (IEnumerable<string>).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
		}
	}
}