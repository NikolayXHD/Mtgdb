using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Ui;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	[JsonObject]
	public class GuiSettings
	{
		public GuiSettings()
		{
			ShowDuplicates = true;
		}

		public string Find { get; set; }

		public FilterValueState[] FilterMana { get; set; }
		public FilterValueState[] FilterManaAbility { get; set; }
		public FilterValueState[] FilterManaGenerated { get; set; }
		public FilterValueState[] FilterType { get; set; }
		public FilterValueState[] FilterRarity { get; set; }
		public FilterValueState[] FilterAbility { get; set; }
		public FilterValueState[] FilterCastKeyword { get; set; }
		public FilterValueState[] FilterCmc { get; set; }
		public FilterValueState[] FilterLayout { get; set; }
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

		public Dictionary<string, int> Collection { get; set; }

		public string LegalityFilterFormat { get; set; }
		public bool? LegalityAllowLegal { get; set; }
		public bool? LegalityAllowRestricted { get; set; }
		public bool? LegalityAllowBanned { get; set; }
		public bool? LegalityAllowFuture { get; set; }

		public string DeckFile { get; set; }
		public string DeckName { get; set; }

		public int? SearchResultScroll { get; set; }
		public bool? ShowTextualFields { get; set; }

		public bool? ShowDeck { get; set; }
		public bool? ShowPartialCards { get; set; }

		public bool? ShowFilterPanels { get; set; }
		public bool? ShowScroll { get; set; }

		public Direction? WindowSnapDirection { get; set; }
		public Rectangle? WindowArea { get; set; }

		public FilterByDeckMode? FilterByDeckMode { get; set; }

		public ZoomSettings Zoom { get; set; }

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
		public Deck CollectionModel => Deck.Create(Collection, Collection?.Keys.ToList(), null, null);

		public class ZoomSettings
		{
			public bool ShowArt { get; set; }
			public bool ShowVariants { get; set; }
			public bool ShowOtherSet { get; set; }
		}
	}
}