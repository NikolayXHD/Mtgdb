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

		public static readonly IList<string> Formats = new[]
		{
			"Standard",
			"Modern",
			"Legacy",
			"Vintage",

			"Commander",
			"1v1",
			"Brawl",

			"Frontier",

			"Pauper",
			"Penny",
			"Future",
			"Duel"
		};
	}
}