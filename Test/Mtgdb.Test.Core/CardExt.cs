using Mtgdb.Data;

namespace Mtgdb.Test
{
	public static class CardExt
	{
		public static string ToStringShort(this Card card) =>
			$"{card.SetCode} {card.NameEn} {card.Number}";
	}
}