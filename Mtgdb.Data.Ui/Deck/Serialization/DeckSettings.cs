using System.Collections.Generic;
using Mtgdb.Data;
using Newtonsoft.Json;

namespace Mtgdb.Ui
{
	public class DeckSettings: IDeckSettings
	{
		[JsonProperty("Deck")]
		[JsonConverter(typeof(InternedStringToIntDictionaryConverter))]
		public Dictionary<string, int> MainDeckCount { get; set; } = new Dictionary<string, int>();

		[JsonProperty("DeckOrder")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public List<string> MainDeckOrder { get; set; } = new List<string>();

		[JsonProperty("SideDeck")]
		[JsonConverter(typeof(InternedStringToIntDictionaryConverter))]
		public Dictionary<string, int> SideDeckCount { get; set; } = new Dictionary<string, int>();

		[JsonProperty("SideDeckOrder")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public List<string> SideDeckOrder { get; set; } = new List<string>();

		[JsonProperty("MaybeDeck")]
		[JsonConverter(typeof(InternedStringToIntDictionaryConverter))]
		public Dictionary<string, int> MaybeDeckCount { get; set; } = new Dictionary<string, int>();

		[JsonProperty("MaybeDeckOrder")]
		[JsonConverter(typeof(InternedStringArrayConverter))]
		public List<string> MaybeDeckOrder { get; set; } = new List<string>();

		public FsPath? DeckFile { get; set; }

		public string DeckName { get; set; }
	}
}
