using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Mtgdb.Index;

namespace Mtgdb.Dal.Index
{
	public class CardSearcher : LuceneSearcher<int, Card>
	{
		// 0.39 bbd
		private const string IndexVerision = "0.39";

		public CardSearcher(CardRepository repository, CardDocumentAdapter adapter)
			: base(new CardSpellchecker(repository, adapter), adapter)
		{
			_repo = repository;
			_adapter = adapter;
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("search");
		}

		/// <summary>
		/// For test
		/// </summary>
		internal IEnumerable<Card> SearchCards(string queryStr, string language)
		{
			var query = ParseQuery(queryStr, language);
			return SearchCards(query);
		}

		/// <summary>
		/// For test
		/// </summary>
		internal IEnumerable<Card> SearchCards(Query query)
		{
			var searchResult = SearchIndex(query);

			foreach (var scoreDoc in searchResult.ScoreDocs)
			{
				var id = GetId(scoreDoc);
				var card = _repo.Cards[id];
				yield return card;
			}
		}



		protected override string GetDisplayField(string field)
		{
			DocumentFactory.DisplayFieldByIndexField.TryGetValue(field, out string displayField);
			return displayField ?? field;
		}

		protected override Analyzer CreateAnalyzer() =>
			new MtgAnalyzer(_adapter);

		protected override Directory CreateIndex()
		{
			if (_version.IsUpToDate)
				return FSDirectory.Open(_version.Directory);

			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			_version.CreateDirectory();
			
			var index = base.CreateIndex();

			if (_abort)
				return null;

			index.SaveTo(_version.Directory);

			_version.SetIsUpToDate();
			return index;
		}

		protected override IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex()
		{
			return _repo.SetsByCode.Values
				.TakeWhile(_ => !_abort)
				.Where(FilterSet)
				.Select(set => set.Cards
					.TakeWhile(_ => !_abort)
					.Select(Adapter.ToDocument));
		}

		public CardSpellchecker Spellchecker => (CardSpellchecker) LuceneSpellchecker;

		protected override QueryParser CreateQueryParser(string language, Analyzer analyzer) =>
			new CardQueryParser((MtgAnalyzer) analyzer, _repo, _adapter, language);



		public override void Dispose()
		{
			abortLoading();
			IsLoaded = false;

			base.Dispose();

			Disposed?.Invoke();
		}

		public void InvalidateIndex() =>
			_version.Invalidate();

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

		private bool _abort;

		public string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVerision);
		}
		public Func<Set, bool> FilterSet { get; set; } = set => true;
		public string IndexDirectory => _version.Directory;
		public bool IsUpToDate => _version.IsUpToDate;
		public int SetsAddedToIndex => GroupsAddedToIndex;



		private readonly CardRepository _repo;
		private readonly CardDocumentAdapter _adapter;
		private IndexVersion _version;
	}
}