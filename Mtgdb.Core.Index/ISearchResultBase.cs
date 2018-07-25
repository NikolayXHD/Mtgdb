using System.Collections.Generic;
using Lucene.Net.Contrib;

namespace Mtgdb.Index
{
	public interface ISearchResultBase
	{
		Dictionary<string, IReadOnlyList<Token>> HighlightTerms { get; }
		Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> HighlightPhrases { get; }
		string Query { get; }
		string ParseErrorMessage { get; }
		bool IndexNotBuilt { get; }
	}
}