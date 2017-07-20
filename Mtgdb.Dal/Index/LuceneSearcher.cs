using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Contrib;
using Directory = Lucene.Net.Store.Directory;
using Token = Lucene.Net.Contrib.Token;
using Version = Lucene.Net.Util.Version;

namespace Mtgdb.Dal.Index
{
	public class LuceneSearcher : IDisposable
	{
		public bool IsUpToDate => _version.IsUpToDate && Spellchecker.Version.IsUpToDate;

		public event Action Loaded;
		public event Action IndexingProgress;

		public int SetsAddedToIndex { get; private set; }
		
		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }

		public LuceneSearcher()
		{
			// 0.2 -> 0.3 new sets
			_version = new IndexVersion(AppDir.Data.AddPath("index").AddPath("search"), "0.3");
			Spellchecker = new LuceneSpellchecker();
		}

		public void LoadIndex(CardRepository repository)
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			if (_version.IsUpToDate)
				_index = FSDirectory.Open(_version.Directory);
			else
				_index = createIndex(repository);
			
			_searcher = new IndexSearcher(_index);
			
			IsLoaded = true;
			IsLoading = false;
			Loaded?.Invoke();

			Spellchecker.LoadIndex(createAnalyzer(), _index);
		}

		private Directory createIndex(CardRepository repository)
		{
			_version.CreateDirectory();

			var index = FSDirectory.Open(_version.Directory);

			using (var writer = new IndexWriter(index, createAnalyzer(), create: true, mfl: IndexWriter.MaxFieldLength.UNLIMITED))
				foreach (var set in repository.SetsByCode.Values)
				{
					foreach (var card in set.Cards)
						writer.AddDocument(card.ToDocument());

					SetsAddedToIndex++;
					IndexingProgress?.Invoke();
				}

			_version.SetIsUpToDate();

			return index;
		}

		private static StandardAnalyzer createAnalyzer()
		{
			return new StandardAnalyzer(Version.LUCENE_30, new HashSet<string>());
		}

		private static NumericAwareQueryParser createParser(string language)
		{
			return new NumericAwareQueryParser(Version.LUCENE_30, "*", createAnalyzer())
			{
				AllowLeadingWildcard = true,
				Language = language
			};
		}

		public IEnumerable<Card> SearchCards(string queryStr, string language, CardRepository repository)
		{
			var parser = createParser(language);
			var query = parser.Parse(queryStr);
			var searchResult = _searcher.Search(query, _searcher.MaxDoc);

			foreach (var scoreDoc in searchResult.ScoreDocs)
			{
				var id = scoreDoc.GetId(_searcher);
				var card = repository.CardsById[id];
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
			catch (ParseException ex)
			{
				return new SearchResult(ex.Message, highlightTerms);
			}

			fixNegativeClauses(query);

			if (!IsLoaded)
				return SearchResult.IndexNotBuiltResult;

			var searchResult = _searcher.Search(query, _searcher.MaxDoc);
			
			var searchRankById = Enumerable.Range(0, searchResult.ScoreDocs.Length)
				.ToDictionary(i => searchResult.ScoreDocs[i].GetId(_searcher));
			
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

			var highlightTerms = tokenizer.Tokens.Where(_ => _.Type.Is(TokenType.FieldValue|TokenType.AnyChar))
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
			Spellchecker.Version.Invalidate();
		}

		public void Dispose()
		{
			_searcher.Dispose();
			_index.Dispose();
		}



		private Directory _index;
		private IndexSearcher _searcher;

		private readonly IndexVersion _version;
	}
}