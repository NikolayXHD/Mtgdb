using System.Collections.Generic;
using Lucene.Net.Contrib;

namespace Mtgdb.Dal.Index
{
	public class IntellisenseSuggest
	{
		public IntellisenseSuggest(Token token, IList<string> values)
		{
			Token = token;
			Values = values;
		}

		public Token Token { get; }
		public IList<string> Values { get; }
	}
}