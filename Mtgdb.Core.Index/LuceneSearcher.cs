using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Contrib;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using ReadOnlyCollectionsExtensions;
using Token = Lucene.Net.Contrib.Token;

namespace Mtgdb.Index
{
	public abstract class LuceneSearcher<TId, TObj> : IDisposable
	{
		protected LuceneSearcher(LuceneSpellchecker<TId, TObj> spellchecker, IDocumentAdapter<TId, TObj> adapter)
		{
			Adapter = adapter;
			Spellchecker = spellchecker;
		}

		public void LoadIndexes()
		{
			LoadIndex();
			LoadSpellcheckerIndex();
		}

		public virtual void LoadIndex()
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			_index = CreateIndex();

			if (_index == null)
				return;

			_indexReader = DirectoryReader.Open(_index);
			_searcher = new IndexSearcher(_indexReader);

			IsLoaded = true;
			IsLoading = false;

			Loaded?.Invoke();
		}

		public void LoadSpellcheckerIndex() =>
			Spellchecker.LoadIndex(_indexReader);

		public SearchResult<TId> Search(string queryStr, string language)
		{
			var (highlightTerms, highlightPhrases) = getHighlightElements(queryStr);

			Query query;

			try
			{
				query = ParseQuery(queryStr, language);
			}
			catch (Exception ex)
			{
				return new SearchResult<TId>(queryStr, ex.Message, highlightTerms, highlightPhrases);
			}

			fixNegativeClauses(query);

			if (!IsLoaded)
				return SearchResult<TId>.IndexNotBuiltResult;

			var searchResult = SearchIndex(query);

			var relevanceById = searchResult.ScoreDocs
				.GroupBy(GetId)
				.ToDictionary(gr => gr.Key, gr => gr.First().Score);

			return new SearchResult<TId>(queryStr, relevanceById, highlightTerms, highlightPhrases);
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

		private (Dictionary<string, IReadOnlyList<Token>> Terms,
			Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> Phrases) getHighlightElements(
				string queryStr)
		{
			var tokenizer = new MtgTolerantTokenizer(queryStr);
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
			if (t.Type.IsAny(TokenType.RegexBody))
				return ReadOnlyList.From(t.Value);

			if (!t.Type.IsAny(TokenType.FieldValue))
				return null;

			string text = !t.IsPhrase || t.IsPhraseComplex || t.PhraseHasSlop
				? t.Value
				: t.GetPhraseText(queryStr);

			var result = QueryParserAnalyzer
				.GetTokens(t.ParentField, StringEscaper.Unescape(text))
				.Select(_ => _.Term)
				.ToReadOnlyList();

			return result;
		}



		private string getDisplayField(Token token) =>
			GetDisplayField(token.ParentField ?? string.Empty);

		public virtual void Dispose()
		{
			// Сначала Spellchecker, потому что он использует _index
			Spellchecker.Dispose();

			_indexReader.Dispose();
			_index.Dispose();
		}

		protected abstract IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex();
		protected abstract Analyzer CreateAnalyzer();

		protected virtual Directory CreateIndex()
		{
			var index = new RAMDirectory();

			var indexWriterAnalyzer = CreateAnalyzer();
			var config = IndexUtils.CreateWriterConfig(indexWriterAnalyzer);

			using (var writer = new IndexWriter(index, config))
			{
				void indexDocumentGroup(IEnumerable<Document> documents)
				{
					foreach (var doc in documents)
						writer.AddDocument(doc);

					Interlocked.Increment(ref GroupsAddedToIndex);
					IndexingProgress?.Invoke();
				}

				IndexUtils.ForEach(GetDocumentGroupsToIndex(), indexDocumentGroup);

				writer.Flush(triggerMerge: true, applyAllDeletes: false);
				writer.Commit();
			}

			return index;
		}
		
		
		protected Query ParseQuery(string queryStr, string language)
		{
			Query query;
			var parser = CreateQueryParser(language, QueryParserAnalyzer);

			lock (_syncQueryParser)
				query = parser.Parse(queryStr);

			return query;
		}

		protected abstract QueryParser CreateQueryParser(string language, Analyzer analyzer);



		protected virtual string GetDisplayField(string field) => field;

		protected TId GetId(ScoreDoc d) => Adapter.GetId(_searcher.Doc(d.Doc));

		protected TopDocs SearchIndex(Query query) =>
			_searcher.SearchWrapper(query, _indexReader.MaxDoc);

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; protected set; }

		protected int GroupsAddedToIndex;

		public readonly LuceneSpellchecker<TId, TObj> Spellchecker;

		protected readonly IDocumentAdapter<TId, TObj> Adapter;

		public event Action IndexingProgress;
		public event Action Loaded;

		private Directory _index;
		private IndexSearcher _searcher;



		private DirectoryReader _indexReader;

		private Analyzer QueryParserAnalyzer =>
			_queryParserAnalyzer ?? (_queryParserAnalyzer = CreateAnalyzer());

		private Analyzer _queryParserAnalyzer;

		private readonly object _syncQueryParser = new object();
	}
}