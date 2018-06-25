using System.Collections.Generic;
using Lucene.Net.Contrib;

namespace Mtgdb.Index
{
	public class SearchResult<TId>
	{
		public SearchResult(
			string query,
			Dictionary<TId, float> relevanceById,
			Dictionary<string, IReadOnlyList<Token>> highlightTerms,
			Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> highlightPhrases)
		{
			RelevanceById = relevanceById;
			HighlightTerms = highlightTerms;
			HighlightPhrases = highlightPhrases;
			Query = query;
		}

		public SearchResult(
			string query,
			string parseErrorMessage,
			Dictionary<string, IReadOnlyList<Token>> highlightTerms,
			Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> highlightPhrases)
		{
			Query = query;
			ParseErrorMessage = parseErrorMessage;
			HighlightTerms = highlightTerms;
			HighlightPhrases = highlightPhrases;
		}

		private SearchResult()
		{
		}

		public static SearchResult<TId> IndexNotBuiltResult => new SearchResult<TId>
		{
			IndexNotBuilt = true
		};

		public Dictionary<string, IReadOnlyList<Token>> HighlightTerms { get; }
		public Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> HighlightPhrases { get; }
		public Dictionary<TId, float> RelevanceById { get; }

		public string Query { get; }
		public string ParseErrorMessage { get; }
		public bool IndexNotBuilt { get; private set; }
	}
}