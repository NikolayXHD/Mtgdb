using System.Collections.Generic;
using Lucene.Net.Index;
using Lucene.Net.Search.Suggest;
using Lucene.Net.Util;

namespace Mtgdb.Dal.Index
{
	internal class TermsEnumIterator : IInputIterator
	{
		public TermsEnumIterator(TermsEnum termsEnum)
		{
			_termsEnum = termsEnum;
		}

		private readonly TermsEnum _termsEnum;
		public BytesRef Next()
		{
			return _termsEnum.Next();
		}

		public IComparer<BytesRef> Comparer => null;
		public long Weight => 1;
		public BytesRef Payload => _termsEnum.Term;
		public bool HasPayloads => true;
		public IEnumerable<BytesRef> Contexts => null;
		public bool HasContexts => false;
	}
}