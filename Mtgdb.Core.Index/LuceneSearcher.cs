using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Contrib;
using Lucene.Net.Search;
using Lucene.Net.Store;
using ReadOnlyCollectionsExtensions;
using Token = Lucene.Net.Contrib.Token;

namespace Mtgdb.Index
{
	public abstract class LuceneSearcher<TId, TDoc> : IDisposable
	{
		protected LuceneSearcher(LuceneSpellchecker<TId, TDoc> spellchecker, IDocumentAdapter<TId, TDoc> adapter)
		{
			Adapter = adapter;
			Spellchecker = spellchecker;
		}

		public void LoadIndexes()
		{
			var newState = CreateState();

			LoadIndex(newState);

			if (newState.IsLoaded)
				Spellchecker.LoadIndex(newState);
		}

		protected internal abstract LuceneSearcherState<TId, TDoc> CreateState();

		internal void LoadIndex(LuceneSearcherState<TId, TDoc> state)
		{
			BeginLoad?.Invoke();

			bool stateExisted = State != null;

			if (!stateExisted)
				State = state;

			var index = CreateIndex(state);

			if (index == null)
				return;

			state.Load(index);

			if (stateExisted)
			{
				State.Dispose();
				State = state;
			}

			Loaded?.Invoke();
		}

		public SearchResult<TId> Search(string queryStr, string language)
		{
			var (highlightTerms, highlightPhrases) = getHighlightElements(queryStr);

			Query query;

			try
			{
				query = parseQuery(queryStr, language);
			}
			catch (Exception ex)
			{
				return new SearchResult<TId>(queryStr, ex.Message, highlightTerms, highlightPhrases);
			}

			fixNegativeClauses(query);

			if (!IsLoaded)
				return SearchResult<TId>.IndexNotBuiltResult;

			var relevanceById = State.Search(query);

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

		public void Dispose()
		{
			// Spellchecker first, because it uses _index
			Spellchecker.Dispose();
			State?.Dispose();

			Disposed?.Invoke();
		}

		protected virtual Directory CreateIndex(LuceneSearcherState<TId, TDoc> state)
		{
			void progressHandler() =>
				IndexingProgress?.Invoke();

			state.IndexingProgress += progressHandler;
			var result = state.CreateIndex();
			state.IndexingProgress -= progressHandler;

			return result;
		}

		private Query parseQuery(string queryStr, string language)
		{
			Query query;
			var parser = Adapter.CreateQueryParser(language, QueryParserAnalyzer);

			lock (_syncQueryParser)
				query = parser.Parse(queryStr);

			return query;
		}

		protected virtual string GetDisplayField(string field) => field;



		public bool IsLoading => State?.IsLoading ?? false;
		public bool IsLoaded => State?.IsLoaded ?? false;
		protected int GroupsAddedToIndex => State?.GroupsAddedToIndex ?? 0;

		public event Action IndexingProgress;

		public readonly LuceneSpellchecker<TId, TDoc> Spellchecker;
		protected readonly IDocumentAdapter<TId, TDoc> Adapter;
		internal LuceneSearcherState<TId, TDoc> State { get; private set; }

		public event Action BeginLoad;
		public event Action Loaded;
		public event Action Disposed;

		private Analyzer QueryParserAnalyzer =>
			_queryParserAnalyzer ?? (_queryParserAnalyzer = Adapter.CreateAnalyzer());

		private Analyzer _queryParserAnalyzer;

		private readonly object _syncQueryParser = new object();
	}
}