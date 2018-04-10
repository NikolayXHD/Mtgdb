using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Store;

namespace Mtgdb.Dal.Index
{
	public static class LuceneExtensions
	{
		public static IEnumerable<(string Term, int Offset)> GetTokens(this Analyzer analyzer, string field, string value)
		{
			var tokenStream = analyzer.GetTokenStream(field ?? string.Empty, value);

			using (tokenStream)
			{
				tokenStream.Reset();

				while (tokenStream.IncrementToken())
				{
					string term = tokenStream.GetAttribute<ICharTermAttribute>().ToString();
					int offset = tokenStream.GetAttribute<IOffsetAttribute>().StartOffset;

					yield return (term, offset);
				}

				tokenStream.End();
			}
		}

		public static void SaveTo(this RAMDirectory ramIndex, string versionDirectory)
		{
			var persistedIndex = FSDirectory.Open(versionDirectory);

			foreach (string file in ramIndex.ListAll())
				ramIndex.Copy(persistedIndex, file, file, IOContext.READ_ONCE);
		}
	}
}