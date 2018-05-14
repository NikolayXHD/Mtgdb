using Lucene.Net.QueryParsers.ComplexPhrase;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;

namespace Mtgdb.Dal.Index
{
	public class RewriteableComplexPhraseQuery : ComplexPhraseQueryParser.ComplexPhraseQuery
	{
		public RewriteableComplexPhraseQuery(string field, string phrasedQueryStringContents, int slopFactor, bool inOrder)
			: base(field, phrasedQueryStringContents, slopFactor, inOrder)
		{
		}

		protected override SpanNearQuery NewSpanNearQuery(SpanQuery[] spanTermQueries, int slop, bool inOrder) =>
			new RewriteableSpanNearQuery(spanTermQueries, slop, inOrder);

		protected override SpanQuery Wrap(Query qc) =>
			qc is EmptyPhraseQuery eq
				? new EmptyPhraseSpanQuery(eq.Field)
				: base.Wrap(qc);
	}
}