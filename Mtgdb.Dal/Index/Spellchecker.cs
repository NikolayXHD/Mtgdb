using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;

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
		/// <summary> Field name for each word in the ngram index.</summary>
		public const string FWord = "word";

		/// <summary> the spell index</summary>
		internal Directory Spellindex;

		/// <summary> Boost value for start and end grams</summary>
		private static readonly float _bStart = 2.0f;

		private static readonly float _bEnd = 2.0f;

		// don't use this searcher directly - see #swapSearcher()
		private IndexSearcher _searcher;

		/// <summary>
		/// this locks all modifications to the current searcher. 
		/// </summary>
		private static readonly object _searcherLock = new object();

		/*
         * this lock synchronizes all possible modifications to the 
         * current index directory. It should not be possible to try modifying
         * the same index concurrently. Note: Do not acquire the searcher lock
         * before acquiring this lock! 
        */
		private static readonly object _modifyCurrentIndexLock = new object();

		private volatile bool _closed;

		internal const float MinScore = 0.5f; //LUCENENET-359 Spellchecker accuracy gets overwritten

		private IStringDistance _sd;
		private IndexWriter _indexWriter;

		/// <summary>
		/// Use the given directory as a spell checker index. The directory
		/// is created if it doesn't exist yet.
		/// </summary>
		/// <param name="spellIndex">the spell index directory</param>
		/// <param name="sd">the <see cref="IStringDistance"/> measurement to use </param>
		public Spellchecker(Directory spellIndex, IStringDistance sd)
		{
			SetSpellIndex(spellIndex);
			setStringDistance(sd);
		}

		/// <summary>
		/// Use a different index as the spell checker index or re-open
		/// the existing index if <c>spellIndex</c> is the same value
		/// as given in the constructor.
		/// </summary>
		/// <param name="spellIndexDir">spellIndexDir the spell directory to use </param>
		/// <throws>AlreadyClosedException if the Spellchecker is already closed</throws>
		/// <throws>IOException if spellchecker can not open the directory</throws>
		public void SetSpellIndex(Directory spellIndexDir)
		{
			// this could be the same directory as the current spellIndex
			// modifications to the directory should be synchronized 
			lock (_modifyCurrentIndexLock)
			{
				ensureOpen();
				if (!DirectoryReader.IndexExists(spellIndexDir))
				{
					var writer = new IndexWriter(spellIndexDir, new IndexWriterConfig(LuceneVersion.LUCENE_48, null));
					writer.Dispose();
				}
				swapSearcher(spellIndexDir);
			}
		}

		/// <summary>
		/// Sets the <see cref="IStringDistance"/> implementation for this
		/// <see cref="Spellchecker"/> instance.
		/// </summary>
		/// <param name="distance">the <see cref="IStringDistance"/> implementation for this
		/// <see cref="Spellchecker"/> instance.</param>
		private void setStringDistance(IStringDistance distance)
		{
			_sd = distance;
		}


		/// <summary> Suggest similar words (restricted or not to a field of a user index)</summary>
		/// <param name="word">String the word you want a spell check done on
		/// </param>
		/// <param name="numSug">int the number of suggest words
		/// </param>
		/// <param name="ir">the indexReader of the user index (can be null see field param)
		/// </param>
		/// <param name="field">String the field of the user index: if field is not null, the suggested
		/// words are restricted to the words present in this field.
		/// </param>
		/// <throws>  IOException </throws>
		/// <returns> String[] the sorted list of the suggest words with this 2 criteria:
		/// first criteria: the edit distance, second criteria (only if restricted mode): the popularity
		/// of the suggest words in the field of the user index
		/// </returns>
		public List<string> SuggestSimilar(string word, int numSug, IndexReader ir, string field)
		{
			// obtainSearcher calls ensureOpen
			var indexSearcher = obtainSearcher();
			try
			{
				float min = MinScore;
				int lengthWord = word.Length;

				var query = new BooleanQuery();

				var alreadySeen = new HashSet<string>();
				for (var ng = getMin(lengthWord); ng <= getMax(lengthWord); ng++)
				{
					string key = "gram" + ng;

					var grams = formGrams(word, ng);

					if (grams.Length == 0)
					{
						continue; // hmm
					}

					if (_bStart > 0)
					{
						// should we boost prefixes?
						add(query, "start" + ng, grams[0], _bStart); // matches start of word
					}

					if (_bEnd > 0)
					{
						// should we boost suffixes
						add(query, "end" + ng, grams[grams.Length - 1], _bEnd); // matches end of word
					}
					for (int i = 0; i < grams.Length; i++)
					{
						add(query, key, grams[i]);
					}
				}

				int maxHits = 30 * numSug;

				
				var hits = indexSearcher.Search(query, maxHits).ScoreDocs;
				
				var sugQueue = new SortedSet<SuggestWord>();

				// go thru more than 'maxr' matches in case the distance filter triggers
				int stop = Math.Min(hits.Length, maxHits);
				var sugWord = new SuggestWord();
				for (int i = 0; i < stop; i++)
				{
					sugWord.String = indexSearcher.Doc(hits[i].Doc).Get(FWord); // get orig word

					// edit distance
					sugWord.Score = _sd.GetDistance(word, sugWord.String);

					if (sugWord.Score < min)
						continue;

					if (ir != null && field != null)
					{
						// use the user index
						sugWord.Freq = ir.DocFreq(new Term(field, sugWord.String)); // freq in the index
						// don't suggest a word that is not present in the field
						if (sugWord.Freq < 1)
							continue;
					}

					if (alreadySeen.Add(sugWord.String) == false) // we already seen this word, no point returning it twice
						continue;

					if (sugQueue.Count == numSug)
					{
						// if queue full, maintain the minScore score
						min = sugQueue.Min.Score;
						sugQueue.Remove(sugQueue.Min);
					}

					sugQueue.Add(sugWord);
					sugWord = new SuggestWord();
				}

				return sugQueue.Reverse().Select(_=>_.String)
					.ToList();
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
			{
				res[i] = text.Substring(i, ng);
			}
			return res;
		}

		/// <summary> Check whether the word exists in the index.</summary>
		/// <param name="word">String
		/// </param>
		/// <throws>  IOException </throws>
		/// <returns> true iff the word exists in the index
		/// </returns>
		public bool Exist(string word)
		{
			// obtainSearcher calls ensureOpen
			IndexSearcher indexSearcher = obtainSearcher();
			try
			{
				return indexSearcher.IndexReader.DocFreq(new Term(FWord, new BytesRef(word))) > 0;
			}
			finally
			{
				releaseSearcher(indexSearcher);
			}
		}

		public void IndexWord(string word)
		{
			lock (_modifyCurrentIndexLock)
			{
				int len = word.Length;
				if (len == 0)
					return;

				// ok index the word
				var doc = createDocument(word, getMin(len), getMax(len));
				_indexWriter.AddDocument(doc);
			}
		}

		public void BeginIndex()
		{
			ensureOpen();

			var indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, new KeywordAnalyzer())
			{
				OpenMode = OpenMode.CREATE_OR_APPEND,
				RAMPerThreadHardLimitMB = 512,
				RAMBufferSizeMB = 512,
				CheckIntegrityAtMerge = false,
				MaxBufferedDocs = int.MaxValue,
				MergePolicy = NoMergePolicy.COMPOUND_FILES
			};

			_indexWriter = new IndexWriter(Spellindex, indexWriterConfig);
		}

		public void EndIndex()
		{
			_indexWriter.Flush(triggerMerge: true, applyAllDeletes: false);
			_indexWriter.Dispose();
			// also re-open the spell index to see our own changes when the next suggestion
			// is fetched:
			swapSearcher(Spellindex);
		}

		private static int getMin(int l)
		{
			if (l <= 5)
				return 1;

			return 2;
		}


		private static int getMax(int l)
		{
			return Math.Min(l, 4);
		}


		private static Document createDocument(string text, int ng1, int ng2)
		{
			var doc = new Document { new StringField(FWord, text, Field.Store.YES) };
			// orig term
			int len = text.Length;
			for (int ng = ng1; ng <= ng2; ng++)
			{
				string key = "gram" + ng;
				string end = null;
				for (int i = 0; i < len - ng + 1; i++)
				{
					string gram = text.Substring(i, ng);
					doc.Add(new StringField(key, gram, Field.Store.NO));
					if (i == 0)
					{
						doc.Add(new StringField("start" + ng, gram, Field.Store.NO));
					}
					end = gram;
				}
				if (end != null)
				{
					// may not be present if len==ng1
					doc.Add(new StringField("end" + ng, end, Field.Store.NO));
				}
			}
			return doc;
		}


		private IndexSearcher obtainSearcher()
		{
			lock (_searcherLock)
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

		public void Close()
		{
			lock (_searcherLock)
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
			IndexSearcher indexSearcher = CreateSearcher(dir);
			lock (_searcherLock)
			{
				if (_closed)
				{
					indexSearcher.IndexReader.Dispose();
					throw new InvalidOperationException("Spellchecker has been closed");
				}
				_searcher?.IndexReader.Dispose();
				// set the spellindex in the sync block - ensure consistency.
				_searcher = indexSearcher;
				Spellindex = dir;
			}
		}

		/// <summary>
		/// Creates a new read-only IndexSearcher (for testing purposes)
		/// </summary>
		/// <param name="dir">dir the directory used to open the searcher</param>
		/// <returns>a new read-only IndexSearcher. (throws IOException f there is a low-level IO error)</returns>
		public IndexSearcher CreateSearcher(Directory dir)
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
					Close();
			}
		}
	}
}