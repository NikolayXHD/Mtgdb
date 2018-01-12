using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Contrib;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;
using Token = Lucene.Net.Contrib.Token;

namespace Mtgdb.Dal.Index
{
	public class LuceneSearcher : IDisposable
	{
		public bool IsUpToDate => _version.IsUpToDate;

		public event Action Loaded;
		public event Action IndexingProgress;

		public int SetsAddedToIndex { get; private set; }
		
		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }

		public LuceneSearcher()
		{
			// 0.21 RIX
			_version = new IndexVersion(AppDir.Data.AddPath("index").AddPath("search"), "0.21");
			Spellchecker = new LuceneSpellchecker();
		}

		public string SearcherDirectory => _version.Directory;
		public string SpellcheckerDirectory => Spellchecker.Version.Directory;

		public void LoadIndex(CardRepository repository)
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			if (_version.IsUpToDate)
				_index = FSDirectory.Open(_version.Directory);
			else
				_index = createIndex(repository);
			
			if (_abort)
				return;

			_indexReader = DirectoryReader.Open(_index);
			_searcher = new IndexSearcher(_indexReader);
			
			IsLoaded = true;
			IsLoading = false;
			Loaded?.Invoke();

			Spellchecker.LoadIndex(createAnalyzer(), repository, _indexReader);
		}

		private Directory createIndex(CardRepository repository)
		{
			_version.CreateDirectory();

			var index = FSDirectory.Open(_version.Directory);

			var indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, createAnalyzer())
			{
				OpenMode = OpenMode.CREATE_OR_APPEND,
				RAMPerThreadHardLimitMB = 512,
				RAMBufferSizeMB = 512,
				CheckIntegrityAtMerge = false,
				MaxBufferedDocs = int.MaxValue,
				MergePolicy = new LogDocMergePolicy { MergeFactor = 300 }
			};

			using (var writer = new IndexWriter(index, indexWriterConfig))
				foreach (var set in repository.SetsByCode.Values)
				{
					if (_abort)
						return null;

					foreach (var card in set.Cards)
					{
						if (_abort)
							return null;

						writer.AddDocument(card.Document);
					}

					SetsAddedToIndex++;
					IndexingProgress?.Invoke();
				}

			_version.SetIsUpToDate();

			return index;
		}

		private static Analyzer createAnalyzer()
		{
			return new MtgdbAnalyzer();
		}

		private static NumericAwareQueryParser createParser(string language)
		{
			return new NumericAwareQueryParser(LuceneVersion.LUCENE_48, "*", createAnalyzer())
			{
				AllowLeadingWildcard = true,
				Language = language
			};
		}

		/// <summary>
		/// For test
		/// </summary>
		public IEnumerable<Card> SearchCards(string queryStr, string language, CardRepository repository)
		{
			var parser = createParser(language);
			var query = parser.Parse(queryStr);
			var searchResult = _searcher.Search(query, _indexReader.MaxDoc);

			foreach (var scoreDoc in searchResult.ScoreDocs)
			{
				var id = scoreDoc.GetId(_searcher);
				var card = repository.Cards[id];
				yield return card;
			}
		}

		public SearchResult Search(string queryStr, string language)
		{
			var highlightTerms = getHighlightTerms(queryStr);

			var parser = createParser(language);
			Query query;
			try
			{
				query = parser.Parse(queryStr);
			}
			catch (Exception ex)
			{
				return new SearchResult(ex.Message, highlightTerms);
			}

			fixNegativeClauses(query);

			if (!IsLoaded)
				return SearchResult.IndexNotBuiltResult;

			var searchResult = _searcher.Search(query, _indexReader.MaxDoc);
			
			var searchRankLookup = Enumerable.Range(0, searchResult.ScoreDocs.Length)
				.GroupBy(i => searchResult.ScoreDocs[i].GetId(_searcher))
				.ToDictionary(gr=>gr.Key, gr=>gr.ToList());

			//var badIds = searchRankLookup.Where(_ => _.Value.Count > 1).ToArray();

			var searchRankById = searchRankLookup.ToDictionary(_ => _.Key, _ => _.Value.Min());
			return new SearchResult(searchRankById, highlightTerms);
		}

		private static void fixNegativeClauses(Query query)
		{
			var boolean = query as BooleanQuery;
			if (boolean == null)
				return;

			bool existsPositive = false;
			foreach (var clause in boolean.Clauses)
			{
				if (clause.Occur != Occur.MUST_NOT)
					existsPositive = true;

				fixNegativeClauses(clause.Query);
			}

			if (!existsPositive)
				boolean.Add(new MatchAllDocsQuery(), Occur.MUST);
		}

		private static Dictionary<string, Token[]> getHighlightTerms(string queryStr)
		{
			var tokenizer = new TolerantTokenizer(queryStr);
			tokenizer.Parse();

			var highlightTerms = tokenizer.Tokens.Where(_ => _.Type.Is(TokenType.FieldValue|TokenType.AnyChar|TokenType.RegexBody))
				.GroupBy(getDisplayField, Str.Comparer)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.ToArray());

			return highlightTerms;
		}

		private static string getDisplayField(Token token)
		{
			if (token.ParentField == null)
				return string.Empty;

			string name;
			if (DocumentFactory.DisplayFieldByIndexField.TryGetValue(token.ParentField, out name))
				return name;
			
			return token.ParentField;
		}

		public LuceneSpellchecker Spellchecker { get; }

		public void InvalidateIndex()
		{
			_version.Invalidate();
		}

		public void InvalidateSpellcheckerIndex()
		{
			Spellchecker.Version.Invalidate();
		}

		public void Dispose()
		{
			abortLoading();
			IsLoaded = false;

			// Сначала Spellchecker, потому что он использует _index
			Spellchecker.Dispose();

			_indexReader.Dispose();
			_index.Dispose();

			Disposed?.Invoke();
		}

		private void abortLoading()
		{
			if (!IsLoading)
				return;

			_abort = true;

			while (IsLoading)
				Thread.Sleep(100);

			_abort = false;
		}

		public event Action Disposed;

		private Directory _index;
		private IndexSearcher _searcher;

		private readonly IndexVersion _version;

		private bool _abort;
		private DirectoryReader _indexReader;
	}
}