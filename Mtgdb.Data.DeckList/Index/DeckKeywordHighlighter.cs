using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lucene.Net.Contrib;
using Mtgdb.Ui;

namespace Mtgdb.Data.Index
{
	public class DeckKeywordHighlighter : IKeywordHighlighter
	{
		public void AddKeywordPatterns(string query, IReadOnlyList<Token> terms, IDictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet)
		{
		}

		public bool IsKeywordField(string field) => false;
	}
}