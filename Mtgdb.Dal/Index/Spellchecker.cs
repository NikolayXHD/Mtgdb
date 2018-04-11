using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Core;
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

		/// <summary>
		/// Use the given directory as a spell checker index. The directory
		/// is created if it doesn't exist yet.
		/// </summary>
		/// <param name="spellIndex">the spell index directory</param>
		public void Load(FSDirectory spellIndex)
		{
			ensureOpen();
			swapSearcher(spellIndex);
		}

		public IReadOnlyList<string> SuggestSimilar(
			string word,
			int numSug,
			ICollection<string> discriminators,
			ICollection<string> fields,
			IndexReader reader)
		{
			// obtainSearcher calls ensureOpen
			var indexSearcher = obtainSearcher();
			try
			{
				float min = MinScore;
				int lengthWord = word.Length;

				var query = new BooleanQuery();

				var alreadySeen = new HashSet<string>();
				for (var len = getMin(lengthWord); len <= getMax(lengthWord); len++)
				{
					string key = "gram" + len;

					var grams = formGrams(word, len);

					if (BoostStart > 0)
						add(query, "start" + len, grams[0], BoostStart);

					if (BoostEnd > 0)
						// should we boost suffixes
						add(query, "end" + len, grams[grams.Length - 1], BoostEnd); // matches end of word

					for (int i = 0; i < grams.Length; i++)
						add(query, key, grams[i]);

					var filterDiscriminators = new BooleanQuery();
					foreach (string discriminator in discriminators)
						filterDiscriminators.Add(new TermQuery(new Term(DiscriminatorField, discriminator)), Occur.SHOULD);

					query.Add(filterDiscriminators, Occur.MUST);
				}

				int maxHits = SearchCountMultiplier * numSug;

				var hits = indexSearcher.Search(query, maxHits).ScoreDocs;

				var sugQueue = new OrderedSet<SuggestWord, SuggestWord>();

				// go thru more than 'maxr' matches in case the distance filter triggers
				int stop = Math.Min(hits.Length, maxHits);
				var sugWord = new SuggestWord();
				for (int i = 0; i < stop; i++)
				{
					sugWord.String = indexSearcher.Doc(hits[i].Doc).Get(WordField); // get orig word

					// edit distance
					sugWord.Score = _similarity.GetSimilarity(word, sugWord.String);

					if (sugWord.Score < min)
						continue;

					if (reader != null && fields != null)
					{
						var matchingField = fields.FirstOrDefault(f => reader.DocFreq(new Term(f.ToLowerInvariant(), sugWord.String)) > 0);
						if (matchingField == null)
							continue;
					}

					if (alreadySeen.Add(sugWord.String) == false) // we already seen this word, no point returning it twice
						continue;

					sugQueue.TryAdd(sugWord, sugWord);

					if (sugQueue.Count == numSug)
						min = sugQueue.TryRemoveMin().Score;

					sugWord = new SuggestWord();
				}

				return sugQueue.Reverse()
					.Select(_ => _.String)
					.ToReadOnlyList();
			}
			finally
			{
				releaseSearcher(indexSearcher);
			}
		}


		/// <summary> Add a clause to a boolean query.</summary>
		private static void add(BooleanQuery q, string k, string v, float boost)
		{
			Query tq = new TermQuery(new Term(k, v));
			tq.Boost = boost;
			q.Add(new BooleanClause(tq, Occur.SHOULD));
		}


		/// <summary> Add a clause to a boolean query.</summary>
		private static void add(BooleanQuery q, string k, string v)
		{
			q.Add(new BooleanClause(new TermQuery(new Term(k, v)), Occur.SHOULD));
		}


		/// <summary> Form all ngrams for a given word.</summary>
		/// <param name="text">the word to parse
		/// </param>
		/// <param name="ng">the ngram length e.g. 3
		/// </param>
		/// <returns> an array of all ngrams in the word and note that duplicates are not removed
		/// </returns>
		private static string[] formGrams(string text, int ng)
		{
			int len = text.Length;
			string[] res = new string[len - ng + 1];

			for (int i = 0; i < len - ng + 1; i++)
				res[i] = text.Substring(i, ng);

			return res;
		}

		public void IndexWord(string discriminator, string word)
		{
			int len = word.Length;

			// ok index the word
			int min = getMin(len);
			int max = getMax(len);

			if (len < min)
				return;

			var doc = createDocument(word, min, max, discriminator);
			_indexWriter.AddDocument(doc);
		}

		public void BeginIndex(Directory index)
		{
			ensureOpen();

			var config = IndexUtils.CreateWriterConfig(new KeywordAnalyzer());

			_spellcheckerIndex = index;
			_indexWriter = new IndexWriter(_spellcheckerIndex, config);
		}

		public void EndIndex()
		{
			_indexWriter.Flush(triggerMerge: true, applyAllDeletes: false);
			_indexWriter.Dispose();

			// also re-open the spell index to see our own changes when the next suggestion
			// is fetched:
			swapSearcher(_spellcheckerIndex);
		}

		private static int getMin(int l)
		{
			return 1;
		}

		private static int getMax(int l)
		{
			return Math.Min(l, 3);
		}


		private static Document createDocument(string text, int minNgram, int maxNgram, string field)
		{
			var doc = new Document
			{
				new StringField(WordField, text, Field.Store.YES),
				new StringField(DiscriminatorField, field.ToLowerInvariant(), Field.Store.NO)
			};

			int originalLen = text.Length;

			for (int len = minNgram; len <= maxNgram; len++)
			{
				string key = "gram" + len;
				string end = null;

				for (int i = 0; i < originalLen - len + 1; i++)
				{
					string gram = text.Substring(i, len);
					doc.Add(new StringField(key, gram, Field.Store.NO));

					if (i == 0)
						doc.Add(new StringField("start" + len, gram, Field.Store.NO));

					end = gram;
				}

				if (end != null)
					doc.Add(new StringField("end" + len, end, Field.Store.NO));
			}

			return doc;
		}


		private IndexSearcher obtainSearcher()
		{
			lock (_syncSearcher)
			{
				ensureOpen();
				_searcher.IndexReader.IncRef();
				return _searcher;
			}
		}

		private static void releaseSearcher(IndexSearcher aSearcher)
		{
			// don't check if open - always decRef 
			// don't decrement the private searcher - could have been swapped
			aSearcher.IndexReader.DecRef();
		}

		private void ensureOpen()
		{
			if (_closed)
			{
				throw new InvalidOperationException("Spellchecker has been closed");
			}
		}

		private void close()
		{
			lock (_syncSearcher)
			{
				ensureOpen();
				_closed = true;
				_searcher?.IndexReader.Dispose();
				_searcher = null;
			}
		}

		private void swapSearcher(Directory dir)
		{
			/*
             * opening a searcher is possibly very expensive.
             * We rather close it again if the Spellchecker was closed during
             * this operation than block access to the current searcher while opening.
             */
			IndexSearcher indexSearcher = createSearcher(dir);
			lock (_syncSearcher)
			{
				if (_closed)
				{
					indexSearcher.IndexReader.Dispose();
					throw new InvalidOperationException("Spellchecker has been closed");
				}

				_searcher?.IndexReader.Dispose();
				// set the spellindex in the sync block - ensure consistency.
				_searcher = indexSearcher;
				_spellcheckerIndex = dir;
			}
		}

		/// <summary>
		/// Creates a new read-only IndexSearcher (for testing purposes)
		/// </summary>
		/// <param name="dir">dir the directory used to open the searcher</param>
		/// <returns>a new read-only IndexSearcher. (throws IOException f there is a low-level IO error)</returns>
		private static IndexSearcher createSearcher(Directory dir)
		{
			return new IndexSearcher(DirectoryReader.Open(dir));
		}

		~Spellchecker()
		{
			dispose(false);
		}

		public void Dispose()
		{
			dispose(true);
			GC.SuppressFinalize(this);
		}

		private void dispose(bool disposeOfManagedResources)
		{
			if (disposeOfManagedResources)
			{
				if (!_closed)
					close();
			}
		}

		private float BoostStart { get; } = 2f;
		private float BoostEnd { get; } = 1.25f;

		private const int SearchCountMultiplier = 3;
		internal const float MinScore = 0.5f;

		private const string WordField = "word";
		private const string DiscriminatorField = "discr";
		private IndexSearcher _searcher;
		private Directory _spellcheckerIndex;
		private IndexWriter _indexWriter;
		private readonly IStringSimilarity _similarity;

		private volatile bool _closed;

		private static readonly object _syncSearcher = new object();
	}
}