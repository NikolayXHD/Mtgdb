using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;

namespace Mtgdb.Dal.Index
{
	internal sealed class ReplaceFilter : TokenFilter
	{
		private readonly IDictionary<char, char> _replacements;

		public ReplaceFilter(TokenStream @in, IDictionary<char, char> replacements)
			: base(@in)
		{
			_replacements = replacements;
			_termAtt = AddAttribute<ICharTermAttribute>();
		}

		private readonly ICharTermAttribute _termAtt;

		public override bool IncrementToken()
		{
			if (m_input.IncrementToken())
			{
				char[] buffer = _termAtt.Buffer;
				int length = _termAtt.Length;

				for (int i = 0; i < length; i++)
					if (_replacements.TryGetValue(buffer[i], out char replaced))
						buffer[i] = replaced;

				return true;
			}

			return false;
		}
	}
}