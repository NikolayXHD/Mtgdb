namespace Mtgdb.Dal
{
	public static class CardExtensions
	{
		public static int MaxDeckCount(this Card c)
		{
			if (Str.Equals(c.Rarity, "Basic Land"))
				return int.MaxValue;

			return 4;
		}

		public static int MinDeckCount(this Card c)
		{
			return 0;
		}
	}
}