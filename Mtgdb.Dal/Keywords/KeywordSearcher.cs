using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Mtgdb.Dal.Index;

namespace Mtgdb.Dal
{
	public class KeywordSearcher
	{
		public KeywordSearcher()
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("keywords");
		}

		public string IndexDirectory => _version.Directory;

		public string IndexDirectoryParent
		{
			get => _version.Directory.Parent();

			// 0.28 refactoring
			set => _version = new IndexVersion(value, "0.28");
		}

		public void InvalidateIndex()
		{
			_version.Invalidate();
		}

		public bool IsUpToDate => _version.IsUpToDate;

		public void Load(CardRepository repository)
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			if (_version.IsUpToDate)
				_index = new RAMDirectory(FSDirectory.Open(_version.Directory), IOContext.READ_ONCE);
			else
				_index = new RAMDirectory(createKeywordsFrom(repository), IOContext.READ_ONCE);

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
				var id = scoreDoc.GetId(_searcher);
				yield return id;
			}
		}

		private static BooleanQuery toLuceneQuery(IEnumerable<KeywordQueryTerm> andTerms, IEnumerable<KeywordQueryTerm> orTerms, IEnumerable<KeywordQueryTerm> notTerms)
		{
			var query = new BooleanQuery(disableCoord: true);

			foreach (var keywordQueryTerm in andTerms)
				if (keywordQueryTerm.Values != null)
					foreach (string andValue in keywordQueryTerm.Values)
					{
						string fieldName = keywordQueryTerm.FieldName.ToLowerInvariant();

						if (string.IsNullOrEmpty(andValue))
							query.Add(new WildcardQuery(new Term(fieldName, MtgQueryParser.AnyValue)), Occur.MUST_NOT);
						else
							query.Add(new TermQuery(new Term(fieldName, andValue.ToLowerInvariant())), Occur.MUST);
					}

			foreach (var keywordQueryTerm in notTerms)
				if (keywordQueryTerm.Values != null)
					foreach (string notValue in keywordQueryTerm.Values)
					{
						string fieldName = keywordQueryTerm.FieldName.ToLowerInvariant();

						if (string.IsNullOrEmpty(notValue))
							query.Add(new WildcardQuery(new Term(fieldName, MtgQueryParser.AnyValue)), Occur.MUST);
						else
							query.Add(new TermQuery(new Term(fieldName, notValue.ToLowerInvariant())), Occur.MUST_NOT);
					}

			foreach (var keywordQueryTerm in orTerms)
				if (keywordQueryTerm.Values != null && keywordQueryTerm.Values.Count > 0)
				{
					var queryTermOr = new BooleanQuery(disableCoord: true);
					foreach (string orValue in keywordQueryTerm.Values)
					{
						string fieldName = keywordQueryTerm.FieldName.ToLowerInvariant();

						if (string.IsNullOrEmpty(orValue))
						{
							var booleanQuery = new BooleanQuery();
							booleanQuery.Add(new BooleanClause(new WildcardQuery(new Term(fieldName, MtgQueryParser.AnyValue)), Occur.MUST_NOT));
							queryTermOr.Add(booleanQuery, Occur.SHOULD);
						}
						else
							queryTermOr.Add(new TermQuery(new Term(fieldName, orValue.ToLowerInvariant())), Occur.SHOULD);
					}

					query.Add(queryTermOr, Occur.MUST);
				}

			if (query.Clauses.All(_=>_.IsProhibited))
				query.Add(new MatchAllDocsQuery(), Occur.SHOULD);

			return query;
		}

		private FSDirectory createKeywordsFrom(CardRepository repository)
		{
			if (!repository.IsLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must be loaded first");

			var keywordsList = new List<CardKeywords>();

			foreach (var set in repository.SetsByCode.Values)
			{
				if (!FilterSet(set))
					continue;

				foreach (var card in set.Cards)
				{
					var keywords = new CardKeywords
					{
						IndexInFile = card.IndexInFile
					};

					keywords.Parse(card);
					keywordsList.Add(keywords);
				}

				SetsCount++;
				LoadingProgress?.Invoke();
			}

			_version.CreateDirectory();

			var fsIndex = FSDirectory.Open(_version.Directory);
			using (var writer = new IndexWriter(fsIndex, new IndexWriterConfig(LuceneVersion.LUCENE_48, new KeywordAnalyzer())))
				foreach (var keyword in keywordsList)
				{
					var doc = keyword.ToDocument();
					writer.AddDocument(doc);
				}

			_version.SetIsUpToDate();

			return fsIndex;
		}

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
	}
}