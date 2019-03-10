using System;

namespace Mtgdb.Data
{
	public static class CardExtensions
	{
		public static int MaxCountInDeck(this Card c)
		{
			if (Str.Equals(c.Rarity, "Basic Land"))
				return int.MaxValue;

			if (c.TextEn?.IndexOf("deck can have any number of cards named " + c.NameEn, Str.Comparison) >= 0)
				return int.MaxValue;

			return 4;
		}

		public static int MinCountInDeck(this Card c) => 0;

		public static bool IsFlipped(this Card c) =>
			Str.Equals(c.Layout, CardLayouts.Flip) && c.Faces[1] == c;

		internal static string GetFaceName(this Card c, int i)
		{
			if (c.Names == null || c.Names.Count == 0)
				if (i == 0)
					return c.NameNormalized;
				else
					throw new IndexOutOfRangeException();

			return c.Names[i];
		}
	}
}