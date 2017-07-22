using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;

namespace Mtgdb.Dal.Index
{
	public class MtgdbAnalyzer : Analyzer
	{
		public MtgdbAnalyzer()
		{
		}

		public sealed override TokenStream TokenStream(string fieldName, TextReader reader)
		{
			var result = new ReplaceFilter(new MtgdbTokenizer(reader), Replacements);
			return result;
		}

		public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
		{
			/* tokenStream() is final, no back compat issue */
			SavedStreams streams = (SavedStreams)PreviousTokenStream;
			if (streams == null)
			{
				streams = new SavedStreams();
				streams.source = new MtgdbTokenizer(reader);
				streams.result = new ReplaceFilter(streams.source, Replacements);
				PreviousTokenStream = streams;
			}
			else
			{
				streams.source.Reset(reader);
			}

			return streams.result;
		}

		private static readonly Dictionary<char, char> Replacements = new Dictionary<char, char>
		{
			{ '−', '-' }
		};

		private class SavedStreams
		{
			protected internal Tokenizer source;
			protected internal TokenStream result;
		};
	}
}