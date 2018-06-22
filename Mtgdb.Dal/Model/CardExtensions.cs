namespace Mtgdb.Dal
{
	public static class CardExtensions
	{
		public static int MaxCountInDeck(this Card c)
		{
			if (Str.Equals(c.Rarity, "Basic Land"))
				return int.MaxValue;

			if (c.TextEn.IndexOf("a deck can have any number of cards named " + c.NameEn, Str.Comparison) >= 0)
				return int.MaxValue;

			return 4;
		}

		public static int MinCountInDeck(this Card c)
		{
			return 0;
		}
	}
}