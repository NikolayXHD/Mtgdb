using Mtgdb.Ui;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	public class JsonDeckFormatter : IDeckFormatter
	{
		public string Description => @"Mtgdb.Gui {type}";
		public string FileNamePattern => @"*.json";

		public Deck ImportDeck(string serialized)
		{
			var saved = JsonConvert.DeserializeObject<GuiSettings>(serialized);
			var result = saved.Deck;
			return result;
		}

		public string ExportDeck(string name, Deck current)
		{
			var settings = new GuiSettings
			{
				MainDeckCount = current.MainDeck?.Count,
				MainDeckOrder = current.MainDeck?.Order,
				SideDeckCount = current.Sideboard?.Count,
				SideDeckOrder = current.Sideboard?.Order
			};

			return JsonConvert.SerializeObject(settings, Formatting.Indented);
		}

		public bool ValidateFormat(string serialized)
		{
			return true;
		}

		public bool SupportsExport => true;
		public bool SupportsImport => true;
		public bool UseBom => false;
		public string FormatHint => null;
	}
}