using System.Collections.Generic;
using System.Text.RegularExpressions;
using Lucene.Net.Contrib;

namespace Mtgdb.Ui
{
	public interface IKeywordHighlighter
	{
		void AddKeywordPatterns(
			string query,
			IReadOnlyList<Token> terms,
			IDictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet);

		bool IsKeywordField(string field);
	}
}