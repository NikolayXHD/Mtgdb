using Newtonsoft.Json;

namespace Mtgdb.Ui
{
	public class JsonDeckFormatter : IDeckFormatter
	{
		public string Description => @"Mtgdb.Gui {type}";
		public string FileNamePattern => @"*.json";

		public Deck ImportDeck(string serialized, bool exact = false)
		{
			var saved = JsonConvert.DeserializeObject<DeckSettings>(serialized);
			var result = saved.GetDeck();
			return result;
		}

		public string ExportDeck(string name, Deck current, bool exact = false)
		{
			var settings = new DeckSettings
			{
				MainDeckCount = current.MainDeck?.Count,
				MainDeckOrder = current.MainDeck?.Order,
				SideDeckCount = current.Sideboard?.Count,
				SideDeckOrder = current.Sideboard?.Order,
				MaybeDeckCount = current.Maybeboard?.Count,
				MaybeDeckOrder = current.Maybeboard?.Order
			};

			return JsonConvert.SerializeObject(settings, Formatting.Indented);
		}

		public bool ValidateFormat(string serialized)
		{
			return true;
		}

		public bool SupportsExport => true;
		public bool SupportsImport => true;
		public bool SupportsFile => true;
		public bool UseBom => false;
		public string FormatHint => null;
	}
}
