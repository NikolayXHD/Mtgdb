using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using ReadOnlyCollectionsExtensions;

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

		public static IReadOnlyList<string> ReadAllValuesFrom(this DirectoryReader reader, string field)
		{
			var rawValues = reader.getStoredValues(field);

			IEnumerable<string> enumerable;

			if (field.IsFloatField())
				enumerable = rawValues.Select(term => term.TryParseFloat())
					.Where(val => val.HasValue)
					.Select(val => val.Value)
					.Distinct()
					.OrderBy(val => val)
					.Select(val => val.ToString(Str.Culture));

			else if (field.IsIntField())
				enumerable = rawValues.Select(term => term.TryParseInt())
					.Where(val => val.HasValue)
					.Select(val => val.Value)
					.Distinct()
					.OrderBy(val => val)
					.Select(val => val.ToString(Str.Culture));
			else
				enumerable = rawValues.Select(term => term.Utf8ToString())
					.Distinct()
					.OrderBy(Str.Comparer);

			var result = enumerable.ToReadOnlyList();
			return result;
		}

		private static IEnumerable<BytesRef> getStoredValues(this IndexReader reader, string field)
		{
			var terms = MultiFields.GetTerms(reader, field);

			if (terms == null)
				yield break;

			var iterator = terms.GetIterator(reuse: null);

			BytesRef value;

			while ((value = iterator.Next()) != null)
				yield return value;
		}

		public static void SaveTo(this Directory index, string directory)
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

		public static readonly FieldType StringFieldType = new FieldType(StringField.TYPE_NOT_STORED) { IsTokenized = true };
	}
}