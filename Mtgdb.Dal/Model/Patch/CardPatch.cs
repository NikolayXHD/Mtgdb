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
		public string Type { get; set; }
		public List<string> Types { get; set; }
		public List<string> Subtypes { get; set; }
		public string OriginalType { get; set; }
	}
}