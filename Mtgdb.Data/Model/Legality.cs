using System.Collections.Generic;

namespace Mtgdb.Data
{
	public static class Legality
	{
		public const string Legal = "legal";
		public const string Future = "future";
		public const string Restricted = "restricted";
		public const string Banned = "banned";
		public const string Illegal = "illegal";

		public const string AnyFormat = "[ any format ]";

		public static readonly IReadOnlyList<string> Formats = new[]
		{
			"Standard",
			"Pioneer",
			"Historic",
			"Modern",
			"Legacy",
			"Vintage",

			"Brawl",
			"Commander",

			"Pauper",
			"Penny",
			"Future",
			"Duel",
			"Oldschool"
		};
	}
}
