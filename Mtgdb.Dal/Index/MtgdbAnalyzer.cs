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
			var result = new ReplaceFilter(new MtgdbTokenizer(reader), _replacements);
			return result;
		}

		public override TokenStream ReusableTokenStream(string fieldName, TextReader reader)
		{
			/* tokenStream() is final, no back compat issue */
			SavedStreams streams = (SavedStreams)PreviousTokenStream;
			if (streams == null)
			{
				streams = new SavedStreams();
				streams.Source = new MtgdbTokenizer(reader);
				streams.Result = new ReplaceFilter(streams.Source, _replacements);
				PreviousTokenStream = streams;
			}
			else
			{
				streams.Source.Reset(reader);
			}

			return streams.Result;
		}

		private static readonly Dictionary<char, char> _replacements = new Dictionary<char, char>
		{
			{ '−', '-' },
			{ '–', '-' },
			{ 'û', 'u' },
			{ 'ö', 'o' },
			{ '’', '\'' }
		};

		private class SavedStreams
		{
			protected internal Tokenizer Source;
			protected internal TokenStream Result;
		};
	}
}