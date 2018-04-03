using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.TokenAttributes;

namespace Mtgdb.Dal.Index
{
	public class MtgdbAnalyzer : Analyzer
	{
		public MtgdbAnalyzer()
		{
		}

		protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
		{
			var source = DocumentFactory.NotAnalyzedFields.Contains(fieldName)
				? (Tokenizer) new KeywordTokenizer(reader)
				: new MtgdbTokenizer(reader);

			var result = new ReplaceFilter(source, MtgdbTokenizerPatterns.Replacements);
			return new TokenStreamComponents(source, result);
		}

		public IEnumerable<(string Term, int Offset)> GetTokens(string field, string value)
		{
			using (var tokenStream = GetTokenStream(field, value))
			{
				tokenStream.Reset();

				while (tokenStream.IncrementToken())
				{
					string term = tokenStream.GetAttribute<ICharTermAttribute>().ToString();
					int offset = tokenStream.GetAttribute<IOffsetAttribute>().StartOffset;

					yield return (term, offset);
				}
			}
		}
	}
}