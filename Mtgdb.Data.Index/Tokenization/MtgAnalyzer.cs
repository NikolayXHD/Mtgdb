using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Util;

namespace Mtgdb.Data
{
	public class MtgAnalyzer : Analyzer
	{
		public MtgAnalyzer(IDocumentAdapterBase adapter)
		:base(PER_FIELD_REUSE_STRATEGY)
		{
			_adapter = adapter;
		}

		protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
		{
			if (_adapter.IsNumericField(fieldName)
				|| _adapter.IsNotAnalyzed(fieldName))
			{
				var tokenizer = new KeywordTokenizer(reader);
				var filter = new LowerCaseFilter(LuceneVersion.LUCENE_48, tokenizer);
				return new TokenStreamComponents(tokenizer, filter);
			}
			else
			{
				var tokenizer = new MtgTokenizer(reader);
				var lowerCaseFilter = new LowerCaseFilter(LuceneVersion.LUCENE_48, tokenizer);
				var replacementFilter = new ReplaceFilter(lowerCaseFilter, MtgAlphabet.Replacements);

				return new TokenStreamComponents(tokenizer, replacementFilter);
			}
		}

		private readonly IDocumentAdapterBase _adapter;
	}
}