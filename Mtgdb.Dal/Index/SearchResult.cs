using System.Collections.Generic;
using Lucene.Net.Contrib;

namespace Mtgdb.Dal.Index
{
	public class SearchResult
	{
		public SearchResult(Dictionary<int, int> searchRankById, Dictionary<string, Token[]> highlightTerms)
		{
			SearchRankById = searchRankById;
			HighlightTerms = highlightTerms;
		}

		public SearchResult(string parseErrorMessage, Dictionary<string, Token[]> highlightTerms)
		{
			ParseErrorMessage = parseErrorMessage;
			HighlightTerms = highlightTerms;
		}

		private SearchResult()
		{
		}

		public static SearchResult IndexNotBuiltResult => new SearchResult
		{
			IndexNotBuilt = true
		};

		public Dictionary<string, Token[]> HighlightTerms { get; }
		public Dictionary<int, int> SearchRankById { get; }
		public string ParseErrorMessage { get; }
		public bool IndexNotBuilt { get; private set; }
	}
}