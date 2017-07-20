using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	public class JsonDeckFormatter : IDeckFormatter
	{
		public string Description => @"Mtgdb.Gui deck";
		public string FileNamePattern => @"*.json";

		public GuiSettings ImportDeck(string serialized)
		{
			var result = new GuiSettings();
			var saved = JsonConvert.DeserializeObject<GuiSettings>(serialized);
			result.Deck = saved.Deck ?? new Dictionary<string, int>();
			result.DeckOrder = saved.DeckOrder ?? new List<string>();
			result.SideDeck = saved.SideDeck ?? new Dictionary<string, int>();
			result.SideDeckOrder = saved.SideDeckOrder ?? new List<string>();

			return result;
		}

		public string ExportDeck(string name, GuiSettings current)
		{
			return JsonConvert.SerializeObject(current, Formatting.Indented);
		}

		public bool ValidateFormat(string serialized)
		{
			return true;
		}

		public bool SupportsExport => true;
		public bool SupportsImport => true;
	}
}