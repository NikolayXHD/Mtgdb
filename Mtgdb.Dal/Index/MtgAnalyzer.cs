using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Util;

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
			if (fieldName.IsNumericField() || DocumentFactory.NotAnalyzedFields.Contains(fieldName))
			{
				var tokenizer = new KeywordTokenizer(reader);
				var filter = new LowerCaseFilter(LuceneVersion.LUCENE_48, tokenizer);
				return new TokenStreamComponents(tokenizer, filter);
			}
			else
			{
				var tokenizer = new MtgTokenizer(reader);
				var lowerCaseFilter = new LowerCaseFilter(LuceneVersion.LUCENE_48, tokenizer);
				var replacementFilter = new ReplaceFilter(lowerCaseFilter, MtgAplhabet.Replacements);

				return new TokenStreamComponents(tokenizer, replacementFilter);
			}
		}
	}
}