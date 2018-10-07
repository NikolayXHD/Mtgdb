using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Lucene.Net.Contrib;
using Mtgdb.Dal;
using Mtgdb.Index;

namespace Mtgdb.Gui
{
	public class KeywordHighlighter : IKeywordHighlighter
	{
		public void AddKeywordPatterns(
			string query,
			IReadOnlyList<Token> terms,
			IDictionary<string, (HashSet<string> TokenFields, Regex Pattern)> patternsSet)
		{
			var keywordTerms = terms.Where(t => Str.Equals(t.ParentField, nameof(KeywordDefinitions.Keywords)));

			foreach (var keywordTerm in keywordTerms)
			{
				string termText = StringEscaper.Unescape(keywordTerm.GetPhraseText(query));

				if (!KeywordDefinitions.PatternsByDisplayText[KeywordDefinitions.KeywordsIndex].TryGetValue(termText, out var regex) &&
					!KeywordDefinitions.PatternsByDisplayText[KeywordDefinitions.CastKeywordsIndex].TryGetValue(termText, out regex))
					continue;

				string pattern = regex.ToString();

				if (patternsSet.TryGetValue(pattern, out var tokenPattern))
				{
					tokenPattern.TokenFields.Add(keywordTerm.ParentField);
					continue;
				}

				tokenPattern = (new HashSet<string>(Str.Comparer) { keywordTerm.ParentField }, regex);
				patternsSet.Add(pattern, tokenPattern);
			}
		}

		public bool IsKeywordField(string field)
		{
			return Str.Equals(field, nameof(KeywordDefinitions.Keywords));
		}
	}
}