using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;

namespace Mtgdb.Dal.Index
{
	public class MtgAnalyzer : Analyzer
	{
		public MtgAnalyzer()
		:base(PER_FIELD_REUSE_STRATEGY)
		{
		}

		protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
		{
			if (fieldName.IsNumericField() || fieldName.IsNotAnalyzedField())
				return new TokenStreamComponents(new KeywordTokenizer(reader));
			
			var tokenizer = new MtgdbTokenizer(reader);
			var filter = new ReplaceFilter(tokenizer, MtgdbTokenizerPatterns.Replacements);

			return new TokenStreamComponents(tokenizer, filter);
		}
	}
}