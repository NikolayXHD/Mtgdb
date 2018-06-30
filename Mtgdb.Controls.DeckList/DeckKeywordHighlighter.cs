using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lucene.Net.Contrib;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckKeywordHighlighter : IKeywordHighlighter
	{
		public void AddKeywordPatterns(string query, IReadOnlyList<Token> terms, IDictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet)
		{
		}

		public bool IsKeywordField(string field) => false;
	}
}