using System.Collections.Generic;

namespace Mtgdb.Dal
{
	internal class CardDelta
	{
		public List<LegalityNote> Legality { get; set; }
		public string Text { get; set; }
		public string GeneratedMana { get; set; }
		public bool FlipDuplicate { get; set; }
	}
}