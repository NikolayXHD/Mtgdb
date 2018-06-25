using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util.Automaton;

namespace Mtgdb.Index
{
	public class MtgRegexpQuery : RegexpQuery
	{
		public Term Term { get; }

		public MtgRegexpQuery(Term term) : base(term, RegExpSyntax.ALL)
		{
			Term = term;
		}
	}
}