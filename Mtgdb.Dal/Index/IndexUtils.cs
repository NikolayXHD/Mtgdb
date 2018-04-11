using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Mtgdb.Dal.Index
{
	public static class IndexUtils
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

		public static IndexWriterConfig CreateWriterConfig(Analyzer analyzer)
		{
			var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
			{
				OpenMode = OpenMode.CREATE,
				RAMPerThreadHardLimitMB = 128,
				RAMBufferSizeMB = 128,
				MaxBufferedDocs = 1 << 16, //64k
				UseCompoundFile = false,
				MaxThreadStates = Environment.ProcessorCount
			};

			return config;
		}
	}
}