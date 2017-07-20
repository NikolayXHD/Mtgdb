using System.Collections.Generic;
using Mtgdb.Controls;
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

		public Dictionary<string, int> Deck { get; set; } = new Dictionary<string, int>();
		public List<string> DeckOrder { get; set; } = new List<string>();
		public Dictionary<string, int> SideDeck { get; set; } = new Dictionary<string, int>();
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
		public string DeckFile { get; set; }
		public int? SearchResultScroll { get; set; }
	}
}