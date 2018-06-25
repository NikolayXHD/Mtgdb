using System.Collections.Generic;
using Lucene.Net.Contrib;

namespace Mtgdb.Index
{
	public class IntellisenseSuggest
	{
		public IntellisenseSuggest(Token token, IReadOnlyList<string> values, IReadOnlyList<TokenType> types)
		{
			Token = token;
			Values = values;
			Types = types;
		}

		public Token Token { get; }
		public IReadOnlyList<string> Values { get; }
		public IReadOnlyList<TokenType> Types { get; }
	}
}