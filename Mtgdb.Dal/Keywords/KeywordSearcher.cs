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
			// 0.22 V17
			_version = new IndexVersion(AppDir.Data.AddPath("index").AddPath("keywords"), "0.22");
			_version.Directory.AddPath("keywords.json");
		}

		public string Directory => _version.Directory;

		public void Load(CardRepository repository)
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			if (_version.IsUpToDate)
				_index = new RAMDirectory(FSDirectory.Open(_version.Directory), IOContext.READ_ONCE);
			else
				_index = createKeywordsFrom(repository);

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
						query.Add(new TermQuery(new Term(keywordQueryTerm.FieldName.ToLowerInvariant(), toIndexedValue(andValue))), Occur.MUST);

			foreach (var keywordQueryTerm in notTerms)
				if (keywordQueryTerm.Values != null)
					foreach (string notValue in keywordQueryTerm.Values)
						query.Add(new TermQuery(new Term(keywordQueryTerm.FieldName.ToLowerInvariant(), toIndexedValue(notValue))), Occur.MUST_NOT);

			foreach (var keywordQueryTerm in orTerms)
				if (keywordQueryTerm.Values != null && keywordQueryTerm.Values.Count > 0)
				{
					var queryTermOr = new BooleanQuery(disableCoord: true);
					foreach (string orValue in keywordQueryTerm.Values)
						queryTermOr.Add(new TermQuery(new Term(keywordQueryTerm.FieldName.ToLowerInvariant(), toIndexedValue(orValue))), Occur.SHOULD);

					query.Add(queryTermOr, Occur.MUST);
				}

			if (query.Clauses.All(_ => _.Occur == Occur.MUST_NOT))
				query.Add(new MatchAllDocsQuery(), Occur.MUST);

			return query;
		}

		private static string toIndexedValue(string value)
		{
			if (string.IsNullOrEmpty(value))
				return CardKeywords.NoneKeyword;

			return value.ToLowerInvariant();
		}

		private RAMDirectory createKeywordsFrom(CardRepository repository)
		{
			var keywordsList = new List<CardKeywords>();

			foreach (var set in repository.SetsByCode.Values)
			{
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

			var index = new RAMDirectory(fsIndex, IOContext.READ_ONCE);
			return index;
		}

		public void InvalidateIndex()
		{
			_version.Invalidate();
		}



		public bool IsUpToDate => _version.IsUpToDate;

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }
		public event Action Loaded;
		public event Action LoadingProgress;

		public int SetsCount { get; private set; }



		private readonly IndexVersion _version;
		private RAMDirectory _index;
		private IndexSearcher _searcher;
		private DirectoryReader _indexReader;
	}
}