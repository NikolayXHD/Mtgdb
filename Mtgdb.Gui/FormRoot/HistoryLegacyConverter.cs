using System;
using System.Collections.Generic;
using System.IO;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class HistoryLegacyConverter
	{
		public HistoryLegacyConverter(DeckConverter deckConverter)
		{
			_deckConverter = deckConverter;
		}

		public IEnumerable<(int FormId, int TabId)> FindLegacyFiles()
		{
			if (!Directory.Exists(AppDir.History))
				yield break;

			foreach (var subdir in Directory.GetDirectories(AppDir.History))
				if (int.TryParse(subdir.LastPathSegment(), out int formId))
					foreach (var file in Directory.GetFiles(subdir, "*.json"))
						if (int.TryParse(Path.GetFileNameWithoutExtension(file), out int tabId))
							if (!File.Exists(Application.GetHistoryFile(formId, tabId)) && !File.Exists(getV2File(formId, tabId)))
								yield return (formId, tabId);
		}

		public IEnumerable<(int FormId, int TabId)> FindV2Files()
		{
			if (!Directory.Exists(AppDir.History))
				yield break;

			foreach (var subdir in Directory.GetDirectories(AppDir.History))
				if (int.TryParse(subdir.LastPathSegment(), out int formId))
				{
					const string searchPattern = "*.v2.json";

					foreach (var file in Directory.GetFiles(subdir, searchPattern))
					{
						string name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file));
						if (int.TryParse(name, out int tabId))
							if (!File.Exists(Application.GetHistoryFile(formId, tabId)))
								yield return (formId, tabId);
					}
				}
		}

		public void ConvertLegacyFile((int FormId, int TabId) legacyFile)
		{
			(int formId, int tabId) = legacyFile;

			var file = Application.GetHistoryFile(formId, tabId);
			var fileLegacy = getLegacyFile(formId, tabId);

			if (!File.Exists(fileLegacy) || File.Exists(file))
				throw new InvalidOperationException();

			var state = HistorySubsystem.ReadHistory(fileLegacy);

			foreach (var settings in state.SettingsHistory)
			{
				var converted = _deckConverter.ConvertLegacyDeck(settings.CollectionModel);
				settings.CollectionModel = converted;
				settings.Deck = _deckConverter.ConvertLegacyDeck(settings.Deck);
			}

			HistorySubsystem.WriteHistory(file, state);
		}

		public void ConvertV2File((int FormId, int TabId) v2File)
		{
			(int formId, int tabId) = v2File;

			var file = Application.GetHistoryFile(formId, tabId);
			var fileV2 = getV2File(formId, tabId);

			if (!File.Exists(fileV2) || File.Exists(file))
				throw new InvalidOperationException();

			var state = HistorySubsystem.ReadHistory(fileV2);

			foreach (var settings in state.SettingsHistory)
			{
				var converted = _deckConverter.ConvertV2Deck(settings.CollectionModel);
				settings.CollectionModel = converted;
				settings.Deck = _deckConverter.ConvertV2Deck(settings.Deck);
			}

			HistorySubsystem.WriteHistory(file, state);
		}

		private static string getLegacyFile(int formId, int tabId) => AppDir.History.AddPath($"{formId}\\{tabId}.json");
		private static string getV2File(int formId, int tabId) => AppDir.History.AddPath($"{formId}\\{tabId}.v2.json");

		private readonly DeckConverter _deckConverter;
	}
}