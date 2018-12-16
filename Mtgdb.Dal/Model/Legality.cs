using System.Collections.Generic;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Dal
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
			"Modern",
			"Legacy",
			"Vintage",
			"Commander",
			"Brawl",
			"1v1"
		}.AsReadOnlyList();
	}
}