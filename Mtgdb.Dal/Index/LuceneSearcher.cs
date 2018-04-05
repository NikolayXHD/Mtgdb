using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
		public LuceneSearcher(CardRepository repository)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("search");

			_repo = repository;
			_queryParserAnalyzer = new MtgdbAnalyzer();
			var spellcheckerAnalyzer = new MtgdbAnalyzer();

			Spellchecker = new LuceneSpellchecker(repository, spellcheckerAnalyzer);
		}

		public string IndexDirectoryParent
		{
			get => Version.Directory.Parent();

			// 0.28 analyze all fields
			set => Version = new IndexVersion(value, "0.28");
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

			var index = FSDirectory.Open(Version.Directory);

			var indexWriterAnalyzer = new MtgdbAnalyzer();

			var indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, indexWriterAnalyzer)
			{
				OpenMode = OpenMode.CREATE_OR_APPEND,
				RAMPerThreadHardLimitMB = 512,
				RAMBufferSizeMB = 512,
				CheckIntegrityAtMerge = false,
				MaxBufferedDocs = int.MaxValue,
				MergePolicy = new LogDocMergePolicy { MergeFactor = 300 }
			};

			using (var writer = new IndexWriter(index, indexWriterConfig))
				foreach (var set in _repo.SetsByCode.Values)
				{
					if (_abort)
						return null;

					if (!FilterSet(set))
						continue;

					foreach (var card in set.Cards)
					{
						if (_abort)
							return null;

						writer.AddDocument(card.Document);
					}

					SetsAddedToIndex++;
					IndexingProgress?.Invoke();
				}

			Version.SetIsUpToDate();

			return index;
		}

		private NumericAwareQueryParser createParser(string language)
		{
			return new NumericAwareQueryParser(LuceneVersion.LUCENE_48, "*", _queryParserAnalyzer, _repo)
			{
				AllowLeadingWildcard = true,
				Language = language
			};
		}

		/// <summary>
		/// For test
		/// </summary>
		internal IEnumerable<Card> SearchCards(string queryStr, string language, CardRepository repository)
		{
			var parser = createParser(language);

			Query query;
			lock (_syncQueryParser)
				query = parser.Parse(queryStr);

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
				return new SearchResult(ex.Message, highlightTerms, highlightPhrases);
			}

			fixNegativeClauses(query);

			if (!IsLoaded)
				return SearchResult.IndexNotBuiltResult;

			var searchResult = _searcher.Search(query, _indexReader.MaxDoc);

			var relevanceById = searchResult.ScoreDocs
				.GroupBy(d => d.GetId(_searcher))
				.ToDictionary(gr => gr.Key, gr => gr.First().Score);

			return new SearchResult(relevanceById, highlightTerms, highlightPhrases);
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

		private (Dictionary<string, Token[]> Terms, Dictionary<string, List<string[]>> Phrases) getHighlightElements(string queryStr)
		{
			var tokenizer = new TolerantTokenizer(queryStr);
			tokenizer.Parse();

			Dictionary<Token, List<(string Term, int Offset)>> analyzedTokens;

			lock (_syncQueryParser)
			{
				analyzedTokens = tokenizer.Tokens
					.Where(_ => _.Type.IsAny(TokenType.FieldValue))
					.ToDictionary(
						_ => _,
						_ => _queryParserAnalyzer.GetTokens(
							_.ParentField,
							StringEscaper.Unescape(_.Value)).ToList());
			}

			string[] getAnalyzedTokens(Token t)
			{
				if (t.IsPhraseStart)
					return t.GetPhraseTokens()
						.SelectMany(p => analyzedTokens[p].Select(_ => _.Term))
						.ToArray();

				return analyzedTokens[t].Select(_ => _.Term).ToArray();
			}

			var highlightTerms = tokenizer.Tokens
				.Where(_ => !_.IsPhrase && (
					_.Type.IsAny(TokenType.FieldValue) && analyzedTokens[_].Count < 2 ||
					_.Type.IsAny(TokenType.AnyChar | TokenType.RegexBody)))
				.GroupBy(getDisplayField, Str.Comparer)
				.ToDictionary(
					gr => gr.Key,
					gr => gr.ToArray());

			var highlightPhrases = tokenizer.Tokens
				.Where(_ => _.IsPhraseStart ||
					!_.IsPhrase &&
					_.Type.IsAny(TokenType.FieldValue) &&
					analyzedTokens[_].Count > 1)
				.GroupBy(getDisplayField, Str.Comparer)
				.ToDictionary(gr => gr.Key, gr => gr.Select(getAnalyzedTokens).ToList());

			return (highlightTerms, highlightPhrases);
		}

		private static string getDisplayField(Token token)
		{
			if (token.ParentField == null)
				return string.Empty;

			if (DocumentFactory.DisplayFieldByIndexField.TryGetValue(token.ParentField, out string name))
				return name;

			return token.ParentField;
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

			// ������� Spellchecker, ������ ��� �� ���������� _index
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

		public int SetsAddedToIndex { get; private set; }

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }

		private Directory _index;
		private IndexSearcher _searcher;

		private IndexVersion Version { get; set; }

		private bool _abort;
		private DirectoryReader _indexReader;

		private readonly object _syncQueryParser = new object();
		private readonly MtgdbAnalyzer _queryParserAnalyzer;
		private readonly CardRepository _repo;
	}
}