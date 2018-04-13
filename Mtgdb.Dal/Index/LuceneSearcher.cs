using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Contrib;
using Lucene.Net.Util;
using ReadOnlyCollectionsExtensions;
using Directory = Lucene.Net.Store.Directory;
using Token = Lucene.Net.Contrib.Token;

namespace Mtgdb.Dal.Index
{
	public class LuceneSearcher : IDisposable
	{
		public LuceneSearcher(CardRepository repository)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("search");

			_repo = repository;
			_queryParserAnalyzer = new MtgAnalyzer();

			Spellchecker = new LuceneSpellchecker(repository);
		}

		public string IndexDirectoryParent
		{
			get => Version.Directory.Parent();

			// 0.33 do not store duplicate OriginalXxx fields
			set => Version = new IndexVersion(value, "0.33");
		}

		public void LoadIndexes()
		{
			LoadIndex();
			LoadSpellcheckerIndex();
		}

		public void LoadIndex()
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			if (Version.IsUpToDate)
				_index = FSDirectory.Open(Version.Directory);
			else
				_index = createIndex();

			if (_abort)
				return;

			_indexReader = DirectoryReader.Open(_index);
			_searcher = new IndexSearcher(_indexReader);

			IsLoaded = true;
			IsLoading = false;
			Loaded?.Invoke();
		}

		public void LoadSpellcheckerIndex()
		{
			Spellchecker.LoadIndex(_indexReader);
		}

		private Directory createIndex()
		{
			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localiztions first");

			Version.CreateDirectory();
			var index = new RAMDirectory();

			var indexWriterAnalyzer = new MtgAnalyzer();
			var config = IndexUtils.CreateWriterConfig(indexWriterAnalyzer);

			using (var writer = new IndexWriter(index, config))
			{
				void indexSet(Set set)
				{
					if (_abort)
						return;

					if (!FilterSet(set))
						return;

					foreach (var card in set.Cards)
					{
						if (_abort)
							return;

						writer.AddDocument(card.Document);
					}

					Interlocked.Increment(ref _setsAddedToIndex);
					IndexingProgress?.Invoke();
				}

				var parallelOptions = IndexUtils.ParallelOptions;
				if (parallelOptions.MaxDegreeOfParallelism > 1)
					Parallel.ForEach(_repo.SetsByCode.Values, parallelOptions, indexSet);
				else
					foreach (var set in _repo.SetsByCode.Values)
						indexSet(set);

				writer.Flush(triggerMerge: true, applyAllDeletes: false);
				writer.Commit();
			}

			if (_abort)
				return null;

			index.SaveTo(Version.Directory);
			Version.SetIsUpToDate();

			return index;
		}

		private MtgQueryParser createParser(string language)
		{
			return new MtgQueryParser(LuceneVersion.LUCENE_48, "*", _queryParserAnalyzer, _repo)
			{
				AllowLeadingWildcard = true,
				Language = language
			};
		}

		/// <summary>
		/// For test
		/// </summary>
		internal IEnumerable<Card> SearchCards(string queryStr, string language)
		{
			var parser = createParser(language);

			Query query;
			lock (_syncQueryParser)
				query = parser.Parse(queryStr);

			var searchResult = _searcher.Search(query, _indexReader.MaxDoc);

			foreach (var scoreDoc in searchResult.ScoreDocs)
			{
				var id = scoreDoc.GetId(_searcher);
				var card = _repo.Cards[id];
				yield return card;
			}
		}

		public SearchResult Search(string queryStr, string language)
		{
			var (highlightTerms, highlightPhrases) = getHighlightElements(queryStr);

			var parser = createParser(language);
			Query query;
			try
			{
				lock (_syncQueryParser)
					query = parser.Parse(queryStr);
			}
			catch (Exception ex)
			{
				return new SearchResult(queryStr, ex.Message, highlightTerms, highlightPhrases);
			}

			fixNegativeClauses(query);

			if (!IsLoaded)
				return SearchResult.IndexNotBuiltResult;

			var searchResult = _searcher.Search(query, _indexReader.MaxDoc);

			var relevanceById = searchResult.ScoreDocs
				.GroupBy(d => d.GetId(_searcher))
				.ToDictionary(gr => gr.Key, gr => gr.First().Score);

			return new SearchResult(queryStr, relevanceById, highlightTerms, highlightPhrases);
		}

		private static void fixNegativeClauses(Query query)
		{
			if (!(query is BooleanQuery boolean))
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

		private (Dictionary<string, IReadOnlyList<Token>> Terms, Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> Phrases) getHighlightElements(
			string queryStr)
		{
			var tokenizer = new TolerantTokenizer(queryStr);
			tokenizer.Parse();

			Dictionary<Token, IReadOnlyList<string>> analyzedTokens;

			lock (_syncQueryParser)
			{
				analyzedTokens = tokenizer.Tokens.ToDictionary(
					_ => _,
					_ => getAnalyzedTokens(_, queryStr));
			}

			var highlightTerms = tokenizer.Tokens
				.Where(_ => analyzedTokens[_]?.Count == 1)
				.GroupBy(getDisplayField, Str.Comparer)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.ToReadOnlyList());

			var highlightPhrases = tokenizer.Tokens
				.Where(_ => analyzedTokens[_]?.Count > 1)
				.GroupBy(getDisplayField, Str.Comparer)
				.ToDictionary(
					gr => gr.Key,
					gr => gr
						.Select(_ => analyzedTokens[_])
						.ToReadOnlyList());

			return (highlightTerms, highlightPhrases);
		}

		private IReadOnlyList<string> getAnalyzedTokens(Token t, string queryStr)
		{
			if (t.IsPhrase && !t.IsPhraseStart)
				return null;

			if (!t.Type.IsAny(TokenType.FieldValue | TokenType.AnyChar | TokenType.RegexBody))
				return null;

			if (t.IsPhraseStart && !t.Type.IsAny(TokenType.FieldValue))
				return null;

			string text = t.GetPhraseText(queryStr);

			var result = _queryParserAnalyzer
				.GetTokens(t.ParentField, StringEscaper.Unescape(text))
				.Select(_ => _.Term)
				.ToReadOnlyList();

			return result;
		}



		private static string getDisplayField(Token token)
		{
			string field = token.ParentField ?? string.Empty;
			DocumentFactory.DisplayFieldByIndexField.TryGetValue(field, out string displayField);
			return displayField ?? field;
		}

		public LuceneSpellchecker Spellchecker { get; }

		public void InvalidateIndex()
		{
			Version.Invalidate();
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

		public Func<Set, bool> FilterSet { get; set; } = set => true;
		public string IndexDirectory => Version.Directory;
		public bool IsUpToDate => Version.IsUpToDate;

		public event Action Disposed;
		public event Action Loaded;
		public event Action IndexingProgress;

		public int SetsAddedToIndex => _setsAddedToIndex;
		private int _setsAddedToIndex;

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }

		private Directory _index;
		private IndexSearcher _searcher;

		private IndexVersion Version { get; set; }

		private bool _abort;
		private DirectoryReader _indexReader;

		private readonly object _syncQueryParser = new object();
		private readonly MtgAnalyzer _queryParserAnalyzer;
		private readonly CardRepository _repo;
	}
}