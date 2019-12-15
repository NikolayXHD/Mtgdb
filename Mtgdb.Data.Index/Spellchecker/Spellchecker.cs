using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Similarities;
using Lucene.Net.Support;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;

namespace Mtgdb.Data
{
	public sealed class Spellchecker : IDisposable
	{
		public Spellchecker(Func<string, bool> isAnyField)
		{
			_isAnyField = isAnyField;
		}

		public void Load(Directory index)
		{
			_index = index;
			setSearcher();
		}

		private void setSearcher()
		{
			_reader = DirectoryReader.Open(_index);
			_searcher = new IndexSearcher(_reader)
			{
				Similarity = new SpellcheckerSimilarity()
			};
		}

		public void BeginIndex(Directory index)
		{
			var config = IndexUtils.CreateWriterConfig(new SpellcheckerAnalyzer());

			_index = index;
			_indexWriter = new IndexWriter(_index, config);
		}

		public void EndIndex()
		{
			_indexWriter.Flush(triggerMerge: true, applyAllDeletes: false);
			_indexWriter.Dispose();
			setSearcher();
		}

		public void Dispose()
		{
			if (_searcher == null)
				return;

			_searcher.IndexReader.Dispose();
			_searcher = null;
		}

		public IReadOnlyList<string> ReadAllValuesFrom(string discriminant)
		{
			var query = new TermQuery(new Term(DiscriminantField, discriminant));

			var searchResult = _searcher.SearchWrapper(query, _reader.MaxDoc);

			var result = searchResult.ScoreDocs
				.Select(_ => _searcher.Doc(_.Doc).Get(WordField))
				.Distinct()
				.OrderBy(Str.Comparer)
				.ToList();

			return result;
		}

		public IReadOnlyList<string> SuggestSimilar(string field, string word, int maxCount)
		{
			word = word.ToLower(Str.Culture);

			var alreadySeen = new HashSet<string>();
			var sugQueue = new OrderedSet<SuggestWord, SuggestWord>();

			var query = createQuery(word, field);

			var hits = _searcher.SearchWrapper(query, maxCount).ScoreDocs;

			int stop = Math.Min(hits.Length, maxCount);

			for (int i = 0; i < stop; i++)
			{
				float score = hits[i].Score;

				if (score < MinScore)
					continue;

				string suggestValue = _searcher.Doc(hits[i].Doc).Get(WordField);
				if (!alreadySeen.Add(suggestValue))
					continue;

				var sugWord = new SuggestWord
				{
					String = suggestValue,
					Score = score
				};

				sugQueue.TryAdd(sugWord, sugWord);

				if (sugQueue.Count == maxCount)
					break;
			}

		return sugQueue
				.Reverse()
				.Select(_ => _.String)
				.ToList();
		}

		private Query createQuery(string word, string field)
		{
			var ngramQuery = createNgramsQuery(word);

			if (_isAnyField(field))
				return ngramQuery;

			var queryDiscriminant = new TermQuery(new Term(DiscriminantField, field));

			return new BooleanQuery
			{
				{ ngramQuery, Occur.MUST },
				{ queryDiscriminant, Occur.MUST }
			};
		}

		private static Query createNgramsQuery(string word)
		{
			var middleTerms = new Dictionary<string, int>();
			var startTerms = new Dictionary<string, int>();

			int max = ((int) Math.Round(word.Length * 0.75)).WithinRange(1, 8);

			for (int n = 1; n <= word.Length; n++)
			{
				string key = word.Substring(0, n);

				//if (!key.Contains(' '))
				startTerms[key] = 1;
			}

			for (int n = 1; n <= max; n++)
			{
				int right = word.Length - n;
				int step = (int) Math.Ceiling(0.5f * n).AtLeast(1);

				string key;
				for (int i = 0; i < right; i+= step)
				{
					key = word.Substring(i, n);

					//if (!key.Contains(' '))
					middleTerms[key] = middleTerms.TryGet(key) + 1;
				}

				key = word.Substring(right, n);

				//if (!key.Contains(' '))
				middleTerms[key] = middleTerms.TryGet(key) + 1;
			}

			var query = new BooleanQuery();

			query.Clauses.AddRange(middleTerms.Select(pair =>
				new BooleanClause(createNgramQuery(pair.Key, boost: pair.Key.Length * pair.Value), Occur.SHOULD)));

			query.Clauses.AddRange(startTerms.Select(pair =>
				new BooleanClause(createNgramQuery(BeginWordChar + pair.Key, boost: 2f * pair.Key.Length * pair.Value), Occur.SHOULD)));

			return query;
		}

		private static Query createNgramQuery(string phrase, float boost)
		{
			const int maxStoredNgram = 2;

			if (phrase.Length <= maxStoredNgram)
				return new TermQuery(new Term(GramField, phrase)) { Boost = boost };

			int last = phrase.Length - maxStoredNgram;

			var result = new PhraseQuery { Boost = boost };

			for (int i = 0; i < last; i += maxStoredNgram)
				result.Add(new Term(GramField, phrase.Substring(i, maxStoredNgram)), i * maxStoredNgram);

			result.Add(new Term(GramField, phrase.Substring(last, maxStoredNgram)), last * maxStoredNgram);

			return result;
		}



		public void IndexWord(string discriminant, string word)
		{
			if (string.IsNullOrEmpty(word))
				return;

			var doc = createDocument(word, discriminant);
			_indexWriter.AddDocument(doc);
		}

		private static Document createDocument(string text, string discriminant)
		{
			string textWithBorders = BeginWordChar + text;

			var doc = new Document
			{
				new Field(WordField, text, new FieldType(IndexUtils.StringFieldType) { IsIndexed = false, IsStored = true }),
				new Field(DiscriminantField, discriminant, new FieldType(IndexUtils.StringFieldType) { IsTokenized = false }),
				new Field(GramField, textWithBorders, new FieldType(IndexUtils.StringFieldType)
				{
					IndexOptions = IndexOptions.DOCS_AND_FREQS_AND_POSITIONS
				})
			};

			return doc;
		}


		private readonly Func<string, bool> _isAnyField;
		private DirectoryReader _reader;

		private const string WordField = "w";
		private const string DiscriminantField = "d";
		private const string GramField = "g";

		private const char BeginWordChar = '▶';

		private Directory _index;
		private IndexSearcher _searcher;
		private IndexWriter _indexWriter;

		private const float MinScore = 0.2f;

		private class SpellcheckerAnalyzer : Analyzer
		{
			public SpellcheckerAnalyzer()
				: base(PER_FIELD_REUSE_STRATEGY)
			{
			}

			protected override TokenStreamComponents CreateComponents(string fieldName, TextReader input)
			{
				if (fieldName == GramField)
				{
					var tokenizer = new NGramTokenizer(LuceneVersion.LUCENE_48, input, 1, 2);
					var lowercaseFilter = new LowerCaseFilter(LuceneVersion.LUCENE_48, tokenizer);
					return new TokenStreamComponents(tokenizer, lowercaseFilter);
				}

				return new TokenStreamComponents(new KeywordTokenizer(input));
			}
		}

		private class SpellcheckerSimilarity : DefaultSimilarity
		{
			public override float Idf(long docFreq, long numDocs) => 1f;
			public override float Tf(float freq) => Math.Sign(freq);
		}
	}
}
