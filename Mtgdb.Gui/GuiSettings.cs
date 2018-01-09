using System.Collections.Generic;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	[JsonObject]
	public class GuiSettings
	{
		public string Find { get; set; }

		public FilterValueState[] FilterMana { get; set; }
		public FilterValueState[] FilterManaAbility { get; set; }
		public FilterValueState[] FilterManaGenerated { get; set; }
		public FilterValueState[] FilterType { get; set; }
		public FilterValueState[] FilterRarity { get; set; }
		public FilterValueState[] FilterAbility { get; set; }
		public FilterValueState[] FilterCmc { get; set; }
		public FilterValueState[] FilterGrid { get; set; }

		[JsonProperty("Deck")]
		public Dictionary<string, int> MainDeckCount { get; set; } = new Dictionary<string, int>();

		[JsonProperty("DeckOrder")]
		public List<string> MainDeckOrder { get; set; } = new List<string>();

		[JsonProperty("SideDeck")]
		public Dictionary<string, int> SideDeckCount { get; set; } = new Dictionary<string, int>();

		[JsonProperty("SideDeckOrder")]
		public List<string> SideDeckOrder { get; set; } = new List<string>();

		public string Language { get; set; }
		public bool ShowDuplicates { get; set; }
		public bool HideTooltips { get; set; }
		public bool ExcludeManaAbilities { get; set; }
		public bool? ExcludeManaCost { get; set; }
		public bool ShowProhibit { get; set; }
		public string Sort { get; set; }

		[JsonProperty("Collection")]
		public Dictionary<string, int> CollectionCount { get; set; }

		public string LegalityFilterFormat { get; set; }
		public bool? LegalityAllowLegal { get; set; }
		public bool? LegalityAllowRestricted { get; set; }
		public bool? LegalityAllowBanned { get; set; }

		public string DeckFile { get; set; }
		public string DeckName { get; set; }

		public int? SearchResultScroll { get; set; }

		[JsonIgnore]
		public Deck Deck
		{
			get
			{
				var deck = Deck.Create(MainDeckCount, MainDeckOrder, SideDeckCount, SideDeckOrder);

				deck.Name = DeckName;
				deck.File = DeckFile;

				return deck;
			}
		}

		[JsonIgnore]
		public Deck Collection => Deck.Create(CollectionCount, CollectionCount?.Keys.ToList(), null, null);
	}
}