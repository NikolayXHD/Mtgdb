using System;
using System.Collections.Generic;
using System.IO;
using Mtgdb.Controls;
using Mtgdb.Data;
using Newtonsoft.Json;
using NLog;

namespace Mtgdb.Gui
{
	public class HistorySubsystem
	{
		static HistorySubsystem()
		{
			_serializer = new JsonSerializer();

			_serializer.Converters.Add(
				new UnformattedJsonConverter(objectType =>
					typeof(IEnumerable<FilterValueState>).IsAssignableFrom(objectType) ||
					typeof(IDictionary<string, int>).IsAssignableFrom(objectType) ||
					typeof(IEnumerable<string>).IsAssignableFrom(objectType) ||
					objectType == typeof(GuiSettings.ZoomSettings)));
		}

		public HistorySubsystem(UiConfigRepository uiConfigRepository)
		{
			_uiConfigRepository = uiConfigRepository;
		}

		public void LoadHistory(string file)
		{
			string directory = Path.GetDirectoryName(file);

			if (string.IsNullOrEmpty(directory))
				throw new ArgumentException($"parent directory not found for path: {file}", nameof(file));

			Directory.CreateDirectory(directory);
			if (TryReadHistory(file, out var state))
			{
				_settingsHistory = state.SettingsHistory;
				_settingsIndex = state.SettingsIndex;
			}
			else
			{
				_settingsHistory = new List<GuiSettings>();
				var defaultSettings = new GuiSettings();
				Add(defaultSettings);
			}

			IsLoaded = true;
			Loaded?.Invoke();
		}

		internal bool TryReadHistory(string file, out HistoryState state)
		{
			state = null;
			if (!File.Exists(file))
				return false;
			try
			{
				using (var fileReader = File.OpenText(file))
				using (var jsonReader = new JsonTextReader(fileReader))
					state = _serializer.Deserialize<HistoryState>(jsonReader);
				return true;
			}
			catch (Exception ex)
			{
				_log.Error(ex, "Failed to read history");
				return false;
			}
		}

		internal static void WriteHistory(string file, HistoryState state)
		{
			using (var writer = File.CreateText(file))
			using (var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' })
				_serializer.Serialize(jsonWriter, state);
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

		public void Save(string file)
		{
			string directory = Path.GetDirectoryName(file);

			if (string.IsNullOrEmpty(directory))
				throw new ArgumentException($"Parent directory not found for path: {file}", nameof(file));

			Directory.CreateDirectory(directory);

			var state = getState();

			WriteHistory(file, state);
		}

		private HistoryState getState()
		{
			var undoDepth = _uiConfigRepository.Config.UndoDepth;

			int minSaveIndex = _settingsIndex - undoDepth;
			int saveCount = 1 + 2 * undoDepth;

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

		public GuiSettings Current => _settingsHistory?[_settingsIndex];

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

		public event Action Loaded;
		public bool IsLoaded { get; private set; }

		private readonly UiConfigRepository _uiConfigRepository;

		private int _settingsIndex;
		private List<GuiSettings> _settingsHistory;

		private static readonly JsonSerializer _serializer;
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}