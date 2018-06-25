using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Mtgdb.Dal.Index;
using Mtgdb.Index;

namespace Mtgdb.Dal
{
	public class KeywordSearcher
	{
		// 0.39 bbd
		private const string IndexVerision = "0.39";

		public KeywordSearcher(CardRepository repo)
		{
			_repo = repo;
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("keywords");
		}

		public void Load()
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			if (_version.IsUpToDate)
				_index = new RAMDirectory(FSDirectory.Open(_version.Directory), IOContext.READ_ONCE);
			else
				_index = new RAMDirectory(createKeywordsFrom(_repo), IOContext.READ_ONCE);

			_indexReader = DirectoryReader.Open(_index);
			_searcher = new IndexSearcher(_indexReader);

			IsLoaded = true;
			IsLoading = false;
			Loaded?.Invoke();
		}

		public IEnumerable<int> GetCardIds(IEnumerable<KeywordQueryTerm> andTerms, IEnumerable<KeywordQueryTerm> orTerms, IEnumerable<KeywordQueryTerm> notTerms)
		{
			var query = toLuceneQuery(andTerms, orTerms, notTerms);
			var searchResult = _searcher.Search(query, filter: null, n: _indexReader.MaxDoc);

			foreach (var scoreDoc in searchResult.ScoreDocs)
			{
				var id = getId(scoreDoc);
				yield return id;
			}
		}

		private int getId(ScoreDoc scoreDoc)
		{
			var doc = _searcher.Doc(scoreDoc.Doc);
			string value = doc.Get(nameof(Card.IndexInFile).ToLower(Str.Culture));
			return int.Parse(value);
		}

		internal IList<int> GetCardIds(string keywordName, string value)
		{
			var query = new TermQuery(new Term(keywordName.ToLower(Str.Culture), value.ToLower(Str.Culture)));
			var searchResult = _searcher.Search(query, filter: null, n: _indexReader.MaxDoc);

			var result = searchResult.ScoreDocs
				.Select(getId)
				.ToArray();

			return result;
		}

		private static BooleanQuery toLuceneQuery(
			IEnumerable<KeywordQueryTerm> andTerms,
			IEnumerable<KeywordQueryTerm> orTerms,
			IEnumerable<KeywordQueryTerm> notTerms)
		{
			var query = new BooleanQuery(disableCoord: true);

			foreach (var keywordQueryTerm in andTerms)
				if (keywordQueryTerm.Values != null)
					foreach (string andValue in keywordQueryTerm.Values)
					{
						string fieldName = keywordQueryTerm.FieldName.ToLower(Str.Culture);

						if (string.IsNullOrEmpty(andValue))
							query.Add(new WildcardQuery(new Term(fieldName, CardQueryParser.AnyValue)), Occur.MUST_NOT);
						else
							query.Add(new TermQuery(new Term(fieldName, andValue.ToLower(Str.Culture))), Occur.MUST);
					}

			foreach (var keywordQueryTerm in notTerms)
				if (keywordQueryTerm.Values != null)
					foreach (string notValue in keywordQueryTerm.Values)
					{
						string fieldName = keywordQueryTerm.FieldName.ToLower(Str.Culture);

						if (string.IsNullOrEmpty(notValue))
							query.Add(new WildcardQuery(new Term(fieldName, CardQueryParser.AnyValue)), Occur.MUST);
						else
							query.Add(new TermQuery(new Term(fieldName, notValue.ToLower(Str.Culture))), Occur.MUST_NOT);
					}

			foreach (var keywordQueryTerm in orTerms)
				if (keywordQueryTerm.Values != null && keywordQueryTerm.Values.Count > 0)
				{
					var queryTermOr = new BooleanQuery(disableCoord: true);
					foreach (string orValue in keywordQueryTerm.Values)
					{
						string fieldName = keywordQueryTerm.FieldName.ToLower(Str.Culture);

						if (string.IsNullOrEmpty(orValue))
						{
							var booleanQuery = new BooleanQuery();
							booleanQuery.Add(new BooleanClause(new WildcardQuery(new Term(fieldName, CardQueryParser.AnyValue)), Occur.MUST_NOT));
							queryTermOr.Add(booleanQuery, Occur.SHOULD);
						}
						else
							queryTermOr.Add(new TermQuery(new Term(fieldName, orValue.ToLower(Str.Culture))), Occur.SHOULD);
					}

					query.Add(queryTermOr, Occur.MUST);
				}

			if (query.Clauses.All(_ => _.IsProhibited))
				query.Add(new MatchAllDocsQuery(), Occur.SHOULD);

			return query;
		}

		private FSDirectory createKeywordsFrom(CardRepository repository)
		{
			if (!repository.IsLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must be loaded first");

			var keywordsList = new List<CardKeywords>(repository.Cards.Count);

			foreach (var set in repository.SetsByCode.Values)
			{
				if (!FilterSet(set))
					continue;

				var setKeywords = new CardKeywords[set.Cards.Count];

				IndexUtils.For(0, set.Cards.Count, i => setKeywords[i] = set.Cards[i].GetAllKeywords());

				keywordsList.AddRange(setKeywords);

				SetsCount++;
				LoadingProgress?.Invoke();
			}

			_version.CreateDirectory();

			var fsIndex = FSDirectory.Open(_version.Directory);
			var indexWriterConfig = IndexUtils.CreateWriterConfig(new LowercaseKeywordAnalyzer());

			using (var writer = new IndexWriter(fsIndex, indexWriterConfig))
			{
				IndexUtils.ForEach(keywordsList,
					keyword =>
					{
						var doc = keyword.ToDocument();
						// ReSharper disable once AccessToDisposedClosure
						writer.AddDocument(doc);
					});
			}

			_version.SetIsUpToDate();

			return fsIndex;
		}



		public string IndexDirectory => _version.Directory;

		public string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVerision);
		}

		public void InvalidateIndex()
		{
			_version.Invalidate();
		}

		public bool IsUpToDate => _version.IsUpToDate;


		public Func<Set, bool> FilterSet { get; set; } = set => true;

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }
		public event Action Loaded;
		public event Action LoadingProgress;

		public int SetsCount { get; private set; }



		private IndexVersion _version;
		private RAMDirectory _index;
		private IndexSearcher _searcher;
		private DirectoryReader _indexReader;

		private readonly CardRepository _repo;
	}
}