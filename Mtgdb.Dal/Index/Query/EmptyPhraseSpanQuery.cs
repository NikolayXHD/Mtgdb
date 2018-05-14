using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search.Spans;
using Lucene.Net.Util;

namespace Mtgdb.Dal.Index
{
	public class EmptyPhraseSpanQuery : SpanQuery
	{
		public EmptyPhraseSpanQuery(string field)
		{
			Field = field;
		}

		public override string ToString(string field) => field + ": \"\"";
			

		public override Spans GetSpans(AtomicReaderContext context, IBits acceptDocs, IDictionary<Term, TermContext> termContexts) => TermSpans.EMPTY_TERM_SPANS;

		public override string Field { get; }
	}
}