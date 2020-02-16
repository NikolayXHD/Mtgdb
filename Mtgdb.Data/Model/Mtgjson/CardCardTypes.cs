using System.Collections.Generic;

namespace Mtgdb.Data
{
	public static class CardCardTypes
	{
		public const string Normal = CardLayouts.Normal;
		public const string Card = CardTypes.Card;
		public const string Emblem = CardTypes.Emblem;
		public const string Token = CardTypes.Token;
		public const string Manifest = CardNames.Manifest;

		public static readonly Dictionary<string, string> ByName =
			new Dictionary<string, string>(Str.Comparer)
			{
				[CardNames.Manifest] = Manifest, [CardNames.Morph] = Manifest
			};

		public static readonly Dictionary<string, string> ByType =
			new Dictionary<string, string>(Str.Comparer)
			{
				[CardTypes.Token] = Token, [CardTypes.Card] = Card, [CardTypes.Emblem] = Emblem
			};
	}
}
