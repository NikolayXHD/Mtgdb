using Lucene.Net.QueryParsers.ComplexPhrase;
using Lucene.Net.Search.Spans;

namespace Mtgdb.Dal.Index
{
	public class RewriteableComplexPhraseQuery : ComplexPhraseQueryParser.ComplexPhraseQuery
	{
		public RewriteableComplexPhraseQuery(string field, string phrasedQueryStringContents, int slopFactor, bool inOrder) : base(field, phrasedQueryStringContents, slopFactor, inOrder)
		{
		}

		protected override SpanNearQuery NewSpanNearQuery(SpanQuery[] spanTermQueries, int slop, bool inOrder)
		{
			return new RewriteableSpanNearQuery(spanTermQueries, slop, inOrder);
		}
	}
}