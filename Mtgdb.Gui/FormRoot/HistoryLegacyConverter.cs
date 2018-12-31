using System;
using System.Collections.Generic;
using System.IO;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class HistoryLegacyConverter
	{
		public HistoryLegacyConverter(DeckLegacyConverter deckLegacyConverter)
		{
			_deckConverter = deckLegacyConverter;
		}

		public IEnumerable<(int FormId, int TabId)> FindLegacyFiles()
		{
			if (!Directory.Exists(AppDir.History))
				yield break;

			foreach (var subdir in Directory.GetDirectories(AppDir.History))
				if (int.TryParse(subdir.LastPathSegment(), out int formId))
					foreach (var file in Directory.GetFiles(subdir, "*.json"))
						if (int.TryParse(Path.GetFileNameWithoutExtension(file), out int tabId))
							if (!File.Exists(Application.GetHistoryFile(formId, tabId)))
								yield return (formId, tabId);
		}

		public void Convert((int FormId, int TabId) legacyFile)
		{
			(int formId, int tabId) = legacyFile;

			var file = Application.GetHistoryFile(formId, tabId);
			var fileLegacy = getHistoryFile(formId, tabId);

			if (!File.Exists(fileLegacy) || File.Exists(file))
				throw new InvalidOperationException();

			var state = HistorySubsystem.ReadHistory(fileLegacy);

			foreach (var settings in state.SettingsHistory)
			{
				var converted = _deckConverter.Convert(settings.CollectionModel);
				settings.CollectionModel = converted;
				settings.Deck = _deckConverter.Convert(settings.Deck);
			}

			HistorySubsystem.WriteHistory(file, state);
		}

		private static string getHistoryFile(int formId, int tabId) => AppDir.History.AddPath($"{formId}\\{tabId}.json");

		private readonly DeckLegacyConverter _deckConverter;
	}
}