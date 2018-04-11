using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

		public static void SaveTo(this RAMDirectory index, string directory)
		{
			var files = index.ListAll();

			using (var storedIndex = FSDirectory.Open(directory))
			{
				foreach (string file in files)
					index.Copy(storedIndex, file, file, IOContext.READ_ONCE);
			}
		}

		public static IndexWriterConfig CreateWriterConfig(Analyzer analyzer)
		{
			var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer)
			{
				OpenMode = OpenMode.CREATE,
				RAMPerThreadHardLimitMB = 128,
				RAMBufferSizeMB = 128 * _maxParallelism,
				MaxBufferedDocs = 1 << 16, //64k
				//UseCompoundFile = false,
				//MaxThreadStates = _maxParallelism
			};

			return config;
		}

		private static readonly bool _useParallelism = false;

		private static readonly int _maxParallelism = _useParallelism
			? Math.Max(Environment.ProcessorCount - 1, 1)
			: 1;

		public static ParallelOptions ParallelOptions { get; } = new ParallelOptions
		{
			MaxDegreeOfParallelism = _maxParallelism
		};
	}
}