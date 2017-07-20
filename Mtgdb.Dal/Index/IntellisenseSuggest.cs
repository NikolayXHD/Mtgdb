using Lucene.Net.Contrib;

namespace Mtgdb.Dal.Index
{
	public class IntellisenseSuggest
	{
		public IntellisenseSuggest(Token token, string[] values)
		{
			Token = token;
			Values = values;
		}

		public Token Token { get; private set; }
		public string[] Values { get; private set; }
	}
}