using Lucene.Net.Contrib;

namespace Mtgdb.Index
{
	public class MtgTolerantTokenizer : TolerantTokenizer
	{
		public MtgTolerantTokenizer(string query)
			:base(query, '>')
		{
		}
	}
}