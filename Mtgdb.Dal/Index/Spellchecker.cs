using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Dal.Index
{
	/// <summary>  <p>
	/// Spell Checker class  (Main class) <br/>
	/// (initially inspired by the David Spencer code).
	/// </p>
	/// 
	/// <p>Example Usage:</p>
	/// 
	/// <pre>
	/// SpellChecker spellchecker = new SpellChecker(spellIndexDirectory);
	/// // To index a field of a user index:
	/// spellchecker.indexDictionary(new LuceneDictionary(my_lucene_reader, a_field));
	/// // To index a file containing words:
	/// spellchecker.indexDictionary(new PlainTextDictionary(new File("myfile.txt")));
	/// String[] suggestions = spellchecker.suggestSimilar("misspelt", 5);
	/// </pre>
	/// 
	/// </summary>
	/// <author>  Nicolas Maisonneuve
	/// </author>
	/// <version>  1.0
	/// </version>
	public sealed class Spellchecker : IDisposable
	{
		public Spellchecker(IStringSimilarity similarity)
		{
			_similarity = similarity;
		}

		public void Load(string directory)
		{
			_index = FSDirectory.Open(directory);
			_reader = DirectoryReader.Open(_index);
			_searcher = new IndexSearcher(_reader);
		}

		public void BeginIndex()
		{
			var config = IndexUtils.CreateWriterConfig(new LowercaseKeywordAnalyzer());

			_index = new RAMDirectory();
			_indexWriter = new IndexWriter(_index, config);
		}

		public void EndIndex(string directory)
		{
			_indexWriter.Flush(triggerMerge: true, applyAllDeletes: false);
			_indexWriter.Dispose();

			_index.SaveTo(directory);

			_reader = DirectoryReader.Open(_index);
			_searcher = new IndexSearcher(_reader);
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

			var searchResult = _searcher.Search(query, _reader.MaxDoc);

			var result = searchResult.ScoreDocs
				.Select(_ => _searcher.Doc(_.Doc).Get(WordField))
				.Distinct()
				.OrderBy(Str.Comparer)
				.ToReadOnlyList();

			return result;
		}

		public IReadOnlyList<string> SuggestSimilar(
			string word,
			int maxCount,
			ICollection<string> discriminants,
			ICollection<string> fields,
			IndexReader reader)
		{
			word = word.ToLower(Str.Culture);

			float min = MinScore;

			var alreadySeen = new HashSet<string>();
			var sugQueue = new OrderedSet<SuggestWord, SuggestWord>();

			var nonExclusiveDiscriminants = new List<string>();
			var exclusiveDiscriminants = new List<string>();

			foreach (string discriminant in discriminants)
				if (SpellcheckerDefinitions.IsDiscriminantExclusiveToField(discriminant))
					exclusiveDiscriminants.Add(discriminant);
				else
					nonExclusiveDiscriminants.Add(discriminant);

			void add(ICollection<string> discriminantsSubset, bool checkUserIndex)
			{
				var query = createQuery(word, discriminantsSubset);

				int maxHits = SearchCountMultiplier * maxCount;
				var hits = _searcher.Search(query, maxHits).ScoreDocs;
				int stop = Math.Min(hits.Length, maxHits);

				for (int i = 0; i < stop; i++)
				{
					string suggestValue = _searcher.Doc(hits[i].Doc).Get(WordField);
					float score = _similarity.GetSimilarity(word, suggestValue);

					if (score < min)
						continue;

					var sugWord = new SuggestWord
					{
						String = suggestValue,
						Score = score
					};

					if (checkUserIndex && reader != null && fields != null)
					{
						var matchingField = fields.FirstOrDefault(f => reader.DocFreq(new Term(f, sugWord.String)) > 0);
						if (matchingField == null)
							continue;
					}

					if (!alreadySeen.Add(sugWord.String))
						continue;

					sugQueue.TryAdd(sugWord, sugWord);

					if (sugQueue.Count > maxCount)
						min = sugQueue.TryRemoveMin().Score;
				}
			}

			if (exclusiveDiscriminants.Count > 0)
				add(exclusiveDiscriminants, checkUserIndex: false);

			if (nonExclusiveDiscriminants.Count > 0)
				add(nonExclusiveDiscriminants, checkUserIndex: true);

			return sugQueue
				.Reverse()
				.Select(_ => _.String)
				.ToReadOnlyList();
		}

		private static Query createQuery(string word, ICollection<string> discriminants)
		{
			var ngramQuery = createNgramQuery(word);

			if (discriminants.Count == 0)
				return ngramQuery;

			var queryDiscriminant = createDiscriminantQuery(discriminants);

			return new BooleanQuery
			{
				{ ngramQuery, Occur.MUST },
				{ queryDiscriminant, Occur.MUST }
			};
		}

		private static Query createDiscriminantQuery(ICollection<string> discriminants)
		{
			var filterDiscriminants = new BooleanQuery();

			foreach (string discriminant in discriminants)
				filterDiscriminants.Add(new TermQuery(new Term(DiscriminantField, discriminant)), Occur.SHOULD);

			return filterDiscriminants;
		}

		private static Query createNgramQuery(string word)
		{
			int min = getMin(word.Length);
			int max = getMax(word.Length);

			var query = new BooleanQuery();
			for (var n = min; n <= max; n++)
			{
				string gramField = "ng" + n;
				string startField = "s" + n;
				string endField = "f" + n;

				int right = word.Length - n;

				for (int i = 0; i <= right; i++)
				{
					string ngram = word.Substring(i, n);

					if (i == 0)
						query.Add(new TermQuery(new Term(startField, ngram)) { Boost = BoostStart }, Occur.SHOULD);

					query.Add(new TermQuery(new Term(gramField, ngram)), Occur.SHOULD);

					if (i == right)
						query.Add(new TermQuery(new Term(endField, ngram)) { Boost = BoostEnd }, Occur.SHOULD);
				}
			}

			return query;
		}



		public void IndexWord(string discriminant, string word)
		{
			int min = getMin(word.Length);
			int max = getMax(word.Length);

			if (word.Length < min)
				return;

			var doc = createDocument(word, min, max, discriminant);
			_indexWriter.AddDocument(doc);
		}

		private static int getMin(int l)
		{
			return 1;
		}

		private static int getMax(int l)
		{
			return Math.Min(l, 3);
		}


		private static Document createDocument(string text, int minNgram, int maxNgram, string discriminant)
		{
			var doc = new Document
			{
				new Field(WordField, text, new FieldType(StringField.TYPE_STORED) { IsIndexed = false }),
				new Field(DiscriminantField, discriminant, IndexUtils.StringFieldType)
			};

			for (int n = minNgram; n <= maxNgram; n++)
			{
				string gramField = "ng" + n;
				string startField = "s" + n;
				string endField = "f" + n;

				int right = text.Length - n;

				for (int i = 0; i <= right; i++)
				{
					string gram = text.Substring(i, n);

					if (i == 0)
					{
						doc.Add(new Field(startField, gram, IndexUtils.StringFieldType));
					}

					if (i == right)
						doc.Add(new Field(endField, gram, IndexUtils.StringFieldType));

					doc.Add(new Field(gramField, gram, IndexUtils.StringFieldType));
				}
			}

			return doc;
		}



		private DirectoryReader _reader;

		private const float BoostStart = 2f;
		private const float BoostEnd = 1.25f;
		private const int SearchCountMultiplier = 2;

		private const float MinScore = 0.5f;

		private const string WordField = "word";
		private const string DiscriminantField = "discr";

		private Directory _index;
		private IndexSearcher _searcher;
		private IndexWriter _indexWriter;

		private readonly IStringSimilarity _similarity;
	}
}