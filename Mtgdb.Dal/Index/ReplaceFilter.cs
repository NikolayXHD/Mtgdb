using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

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
			_termAtt = AddAttribute<ITermAttribute>();
		}

		private readonly ITermAttribute _termAtt;

		public override bool IncrementToken()
		{
			if (input.IncrementToken())
			{
				char[] buffer = _termAtt.TermBuffer();
				int length = _termAtt.TermLength();

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