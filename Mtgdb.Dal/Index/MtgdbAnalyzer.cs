using System.IO;
using Lucene.Net.Analysis;

namespace Mtgdb.Dal.Index
{
	public class MtgdbAnalyzer : Analyzer
	{
		public MtgdbAnalyzer()
		{
		}

		protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
		{
			var source = new MtgdbTokenizer(reader);
			var result = new ReplaceFilter(source, MtgdbTokenizerPatterns.Replacements);

			return new TokenStreamComponents(source, result);
		}
	}
}