using System.Collections.Generic;

namespace Mtgdb.Dal
{
	internal class CardPatch
	{
		public string Text { get; set; }
		public List<string> GeneratedMana { get; set; }
		public bool FlipDuplicate { get; set; }
		public string MciNumber { get; set; }
		public string Loyalty { get; set; }
	}
}