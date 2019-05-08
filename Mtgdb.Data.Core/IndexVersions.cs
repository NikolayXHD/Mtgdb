namespace Mtgdb.Data
{
	public static class IndexVersions
	{
		// keyword detain
		public const string CardSearcher = "1.11";
		public const string CardSpellchecker = "1.11";
		public const string KeywordSearcher = "1.11";

		// fixed deck list re-indexing, bump version to force update obsolete indexes
		public const string DeckSearcher = "1.06";
		public const string DeckSpellchecker = "1.06";
	}
}