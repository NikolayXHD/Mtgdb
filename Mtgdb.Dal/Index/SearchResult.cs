using System.Collections.Generic;
using Lucene.Net.Contrib;

namespace Mtgdb.Dal.Index
{
	public class SearchResult
	{
		public SearchResult(
			Dictionary<int, float> relevanceById,
			Dictionary<string, Token[]> highlightTerms,
			Dictionary<string, List<string[]>> highlightPhrases)
		{
			RelevanceById = relevanceById;
			HighlightTerms = highlightTerms;
			HighlightPhrases = highlightPhrases;
			
		}

		public SearchResult(
			string parseErrorMessage,
			Dictionary<string, Token[]> highlightTerms,
			Dictionary<string, List<string[]>> highlightPhrases)
		{
			ParseErrorMessage = parseErrorMessage;
			HighlightTerms = highlightTerms;
			HighlightPhrases = highlightPhrases;
		}

		private SearchResult()
		{
		}

		public static SearchResult IndexNotBuiltResult => new SearchResult
		{
			IndexNotBuilt = true
		};

		public Dictionary<string, Token[]> HighlightTerms { get; }
		public Dictionary<string, List<string[]>> HighlightPhrases { get; }
		public Dictionary<int, float> RelevanceById { get; }

		public string ParseErrorMessage { get; }
		public bool IndexNotBuilt { get; private set; }
	}
}