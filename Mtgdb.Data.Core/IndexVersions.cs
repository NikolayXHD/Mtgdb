namespace Mtgdb.Data
{
	public static class IndexVersions
	{
		// mtgjson 4.4.0
		public const string CardSearcher = "1.10";
		public const string CardSpellchecker = "1.10";
		public const string KeywordSearcher = "1.10";

		// fixed deck list re-indexing, bump version to force update obsolete indexes
		public const string DeckSearcher = "1.06";
		public const string DeckSpellchecker = "1.06";
	}
}