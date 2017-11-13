using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;

namespace Mtgdb.Dal.Index
{
	/// <summary>Normalizes token text to lower case.</summary>
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
				{
					char replaced;
					if (_replacements.TryGetValue(buffer[i], out replaced))
						buffer[i] = replaced;
					else
						buffer[i] = char.ToLowerInvariant(buffer[i]);
				}

				return true;
			}

			return false;
		}
	}
}