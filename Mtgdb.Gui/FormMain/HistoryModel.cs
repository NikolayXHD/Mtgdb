using System;
using System.Collections.Generic;
using System.IO;
using Mtgdb.Controls;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	public class HistoryModel
	{
		static HistoryModel()
		{
			_jsonSerializerSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented
			};

			_jsonSerializerSettings.Converters.Add(new CustomConverter());
		}

		public HistoryModel(string language, int? maxDepth)
		{
			_maxDepth = maxDepth ?? 100;
			Directory.CreateDirectory(AppDir.History);

			if (File.Exists(HistoryFile))
			{
				var serialized = File.ReadAllText(HistoryFile);
				var state = JsonConvert.DeserializeObject<HistoryState>(serialized);

				_settingsHistory = state.SettingsHistory;
				_settingsIndex = state.SettingsIndex;
			}
			else
			{
				_settingsHistory = new List<GuiSettings>();
				var defaultSettings = new GuiSettings { Language = language };
				Add(defaultSettings);
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
			var state = getState();
			string serialized = JsonConvert.SerializeObject(state, _jsonSerializerSettings);
			File.WriteAllText(HistoryFile, serialized);
		}

		private HistoryState getState()
		{
			int minSaveIndex = _settingsIndex - _maxDepth;
			int saveCount = 1 + 2 * _maxDepth;

			if (minSaveIndex < 0)
				minSaveIndex = 0;

			if (saveCount > _settingsHistory.Count - minSaveIndex)
				saveCount = _settingsHistory.Count - minSaveIndex;

			var state = new HistoryState
			{
				SettingsHistory = _settingsHistory.SkipFirst(minSaveIndex).TakeFirst(saveCount),
				SettingsIndex = _settingsIndex - minSaveIndex
			};

			return state;
		}

		private void validate()
		{
			if (_settingsIndex < 0 || _settingsIndex >= _settingsHistory.Count)
				throw new IndexOutOfRangeException();
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

		private string HistoryFile => AppDir.History.AddPath($"{Id}.json");
		public string Id { get; set; }

		public string DeckFile { get; set; }
		public string DeckName { get; set; }



		private int _settingsIndex;

		private readonly List<GuiSettings> _settingsHistory;
		private readonly int _maxDepth;
		private static readonly JsonSerializerSettings _jsonSerializerSettings;
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