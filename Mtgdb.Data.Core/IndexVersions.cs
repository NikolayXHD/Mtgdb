namespace Mtgdb.Data
{
	public static class IndexVersions
	{
		public const string CardSearcher = "1.08"; // mtgjson 4.3.1
		public const string CardSpellchecker = "1.08";
		public const string KeywordSearcher = "1.08";

		// fixed deck list re-indexing, bump version to force update obsolete indexes
		public const string DeckSearcher = "1.06";
		public const string DeckSpellchecker = "1.06";
	}
}