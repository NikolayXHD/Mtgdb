using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;

namespace Mtgdb.Dal.Index
{
	public static class IndexUtils
	{
		public static IEnumerable<(string Term, int Offset)> GetTokens(this Analyzer analyzer, string field, string value)
		{
			if (String.IsNullOrEmpty(value))
				yield break;

			var tokenStream = analyzer.GetTokenStream(field ?? String.Empty, value);

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

		public static IEnumerable<BytesRef> ReadRawValuesFrom(this IndexReader reader, string field)
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

		public static bool IsFloat(this string queryText)
		{
			return Single.TryParse(queryText, NumberStyles.Float, Str.Culture, out _);
		}

		public static bool IsInt(this string queryText)
		{
			return Int32.TryParse(queryText, NumberStyles.Integer, Str.Culture, out _);
		}

		public static float? TryParseFloat(this BytesRef val)
		{
			if (val == null)
				return null;

			if (val.Length < 6)
				return null;

			int intVal = NumericUtils.PrefixCodedToInt32(val);
			float result = NumericUtils.SortableInt32ToSingle(intVal);
			return result;
		}

		public static int? TryParseInt(this BytesRef val)
		{
			if (val == null)
				return null;

			if (val.Length < 6)
				return null;

			int intVal = NumericUtils.PrefixCodedToInt32(val);
			return intVal;
		}

		public static bool TryParseInt(BytesRef bytes, out int f)
		{
			var s = bytes.Utf8ToString();
			if (s.StartsWith("$"))
				return Int32.TryParse(s.Substring(1), NumberStyles.Integer, Str.Culture, out f);

			return Int32.TryParse(s, NumberStyles.Integer, Str.Culture, out f);
		}

		public static bool TryParseFloat(BytesRef bytes, out float f)
		{
			var s = bytes.Utf8ToString();
			if (s.StartsWith("$"))
				return Single.TryParse(s.Substring(1), NumberStyles.Float, Str.Culture, out f);

			return Single.TryParse(s, NumberStyles.Float, Str.Culture, out f);
		}

		public static void For(int min, int max, Action<int> action)
		{
			var options = ParallelOptions;

			if (options.MaxDegreeOfParallelism > 1)
				Parallel.For(min, max, options, action);
			else
				for (int i = min; i < max; i++)
					action(i);
		}

		public static void ForEach<T>(IEnumerable<T> elements, Action<T> action)
		{
			var options = ParallelOptions;

			if (options.MaxDegreeOfParallelism > 1)
				Parallel.ForEach(elements, options, action);
			else
				foreach (var el in elements)
					action(el);
		}

		private static readonly bool _useParallelism = false;

		private static readonly int _maxParallelism = _useParallelism
			? Math.Max(Environment.ProcessorCount - 1, 1)
			: 1;

		private static ParallelOptions ParallelOptions => new ParallelOptions
		{
			MaxDegreeOfParallelism = _maxParallelism
		};

		public static readonly FieldType StringFieldType = new FieldType(StringField.TYPE_NOT_STORED) { IsTokenized = true };
	}
}