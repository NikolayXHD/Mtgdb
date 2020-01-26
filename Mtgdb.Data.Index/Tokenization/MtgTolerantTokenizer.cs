using Lucene.Net.Contrib;

namespace Mtgdb.Data
{
	public class MtgTolerantTokenizer : TolerantTokenizer
	{
		public MtgTolerantTokenizer(string query)
			:base(query, MtgAlphabet.EmptyPhraseChar)
		{
		}
	}
}
