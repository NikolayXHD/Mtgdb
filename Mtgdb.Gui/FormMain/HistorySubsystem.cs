using System;
using System.Collections.Generic;
using System.IO;
using Mtgdb.Controls;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	public class HistorySubsystem
	{
		static HistorySubsystem()
		{
			_jsonSerializerSettings = new JsonSerializerSettings
			{
				Formatting = Formatting.Indented
			};

			_jsonSerializerSettings.Converters.Add(new CustomConverter());
		}

		public HistorySubsystem(UndoConfig undoConfig)
		{
			_maxDepth = undoConfig.MaxDepth ?? 100;
		}

		public void LoadHistory(string historyDirectory, string tabId)
		{
			IsLoaded = true;

			Directory.CreateDirectory(historyDirectory);
			string historyFile = getHistoryFile(tabId, historyDirectory);

			if (File.Exists(historyFile))
			{
				var serialized = File.ReadAllText(historyFile);
				var state = JsonConvert.DeserializeObject<HistoryState>(serialized);

				_settingsHistory = state.SettingsHistory;
				_settingsIndex = state.SettingsIndex;
			}
			else
			{
				_settingsHistory = new List<GuiSettings>();
				var defaultSettings = new GuiSettings();
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

		public void Save(string historyDirectory, string tabId)
		{
			var state = getState();
			string serialized = JsonConvert.SerializeObject(state, _jsonSerializerSettings);
			File.WriteAllText(getHistoryFile(tabId, historyDirectory), serialized);
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

		private string getHistoryFile(string tabId, string historyDirectory) => historyDirectory.AddPath($"{tabId}.json");

		public string DeckFile { get; set; }
		public string DeckName { get; set; }

		public bool IsLoaded { get; private set; }

		private int _settingsIndex;
		private List<GuiSettings> _settingsHistory;
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