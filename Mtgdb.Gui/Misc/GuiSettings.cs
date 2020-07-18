using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Data;
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
		public FilterValueState[] FilterCardType { get; set; }
		public FilterValueState[] FilterGrid { get; set; }

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

		public string Language { get; set; }
		public bool ShowDuplicates { get; set; }
		public bool HideTooltips { get; set; }
		public bool ExcludeManaAbilities { get; set; }
		public bool? ExcludeManaCost { get; set; }
		public bool ShowProhibit { get; set; }
		public string Sort { get; set; }

		[JsonConverter(typeof(InternedStringToIntDictionaryConverter))]
		public Dictionary<string, int> Collection { get; set; }

		public string LegalityFilterFormat { get; set; }
		public bool? LegalityAllowLegal { get; set; }
		public bool? LegalityAllowRestricted { get; set; }
		public bool? LegalityAllowBanned { get; set; }
		public bool? LegalityAllowFuture { get; set; }

		public FsPath? DeckFile { get; set; }
		public string DeckName { get; set; }

		public int? SearchResultScroll { get; set; }
		public bool? ShowTextualFields { get; set; }

		public bool? ShowDeck { get; set; }
		public bool? ShowPartialCards { get; set; }

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
				var deck = Deck.Create(MainDeckCount, MainDeckOrder, SideDeckCount, SideDeckOrder, MaybeDeckCount, MaybeDeckOrder, null, null);

				deck.Name = DeckName;
				deck.File = DeckFile.OrNone();

				return deck;
			}

			set
			{
				MainDeckCount = value.MainDeck.Count;
				MainDeckOrder = value.MainDeck.Order;
				SideDeckCount = value.Sideboard.Count;
				SideDeckOrder = value.Sideboard.Order;
				MaybeDeckCount = value.Maybeboard.Count;
				MaybeDeckOrder = value.Maybeboard.Order;
			}
		}

		[JsonIgnore]
		public Deck CollectionModel
		{
			get => Deck.Create(Collection, Collection?.Keys.ToList(), null, null, null, null, null, null);
			set => Collection = value.MainDeck.Count;
		}

		public class ZoomSettings
		{
			public bool ShowArt { get; set; }
			public bool ShowVariants { get; set; }
			public bool ShowOtherSet { get; set; }
		}
	}
}
