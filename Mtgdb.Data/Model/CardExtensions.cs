namespace Mtgdb.Data
{
	public static class CardExtensions
	{
		public static int MaxCountInDeck(this Card c)
		{
			if (c.IsToken)
				return int.MaxValue;

			if (Str.Equals(c.Rarity, "Basic Land"))
				return int.MaxValue;

			if (c.TextEn?.IndexOf("deck can have any number of cards named " + c.NameEn, Str.Comparison) >= 0)
				return int.MaxValue;

			return 4;
		}

		public static int MinCountInDeck(this Card c) => 0;

		public static bool IsFlipped(this Card c) =>
			c.IsFlip() && Str.Equals(c.Side, CardSides.B);

		public static bool IsAdventureAttachment(this Card c) =>
			c.IsAdventure() && !c.IsCreature(); // instant or sorcery
	}
}
