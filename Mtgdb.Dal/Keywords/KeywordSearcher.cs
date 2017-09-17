using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Mtgdb.Dal.Index;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class KeywordSearcher
	{
		public bool IsUpToDate => _version.IsUpToDate;

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }
		public event Action Loaded;
		public event Action LoadingProgress;

		private readonly string _file;
		private readonly IndexVersion _version;

		public int SetsCount { get; private set; }

		private RAMDirectory _index;
		private IndexSearcher _searcher;

		public KeywordSearcher()
		{
			// new cards
			_version = new IndexVersion(AppDir.Data.AddPath("index").AddPath("keywords"), "0.5");
			_file = _version.Directory.AddPath("keywords.json");
		}

		public void Load(CardRepository repository)
		{
			if (IsLoaded || IsLoading)
				return;

			IsLoading = true;

			IList<CardKeywords> keywords;

			if (_version.IsUpToDate)
				keywords = loadKeywordsFromFile();
			else
				keywords = createKeywordsFrom(repository);

			_index = new RAMDirectory();
			using (var writer = new IndexWriter(_index, new KeywordAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED))
				foreach (var keyword in keywords)
				{
					var doc = keyword.ToDocument();
					writer.AddDocument(doc);
				}

			_searcher = new IndexSearcher(_index, readOnly: true);

			IsLoaded = true;
			IsLoading = false;
			Loaded?.Invoke();
		}

		public IEnumerable<string> GetCardIds(IEnumerable<KeywordQueryTerm> andTerms, IEnumerable<KeywordQueryTerm> orTerms, IEnumerable<KeywordQueryTerm> notTerms)
		{
			var query = toLuceneQuery(andTerms, orTerms, notTerms);
			var searchResult = _searcher.Search(query, _searcher.MaxDoc);

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

		private List<CardKeywords> createKeywordsFrom(CardRepository repository)
		{
			var keywordsList = new List<CardKeywords>();

			foreach (var set in repository.SetsByCode.Values)
			{
				foreach (var card in set.Cards)
				{
					var keywords = new CardKeywords
					{
						Id = card.Id
					};

					keywords.LoadKeywordsFrom(card);
					keywordsList.Add(keywords);
				}

				SetsCount++;
				LoadingProgress?.Invoke();
			}

			var serialized = JsonConvert.SerializeObject(keywordsList);

			_version.CreateDirectory();
			File.WriteAllText(_file, serialized);
			_version.SetIsUpToDate();

			return keywordsList;
		}

		private List<CardKeywords> loadKeywordsFromFile()
		{
			var serialized = File.ReadAllText(_file);
			var keywords = JsonConvert.DeserializeObject<List<CardKeywords>>(serialized);
			return keywords;
		}

		public void InvalidateIndex()
		{
			_version.Invalidate();
		}
	}
}