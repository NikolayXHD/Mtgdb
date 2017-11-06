namespace Mtgdb.Dal
{
	internal class CardPatch
	{
		public string Text { get; set; }
		public string GeneratedMana { get; set; }
		public bool FlipDuplicate { get; set; }
		public string MciNumber { get; set; }
	}
}