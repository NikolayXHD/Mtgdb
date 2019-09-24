using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Store;

namespace Mtgdb.Data.Index
{
	public class CardSearcher : LuceneSearcher<int, Card>
	{
		public CardSearcher(CardRepository repository, CardDocumentAdapter adapter)
			: base(new CardSpellchecker(repository, adapter), adapter)
		{
			_repo = repository;
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("search");
		}

		/// <summary>
		/// For test
		/// </summary>
		internal IEnumerable<Card> SearchCards(string queryStr, string language)
		{
			var result = Search(queryStr, language);
			return result.RelevanceById.Keys.Select(_ => _repo.Cards[_]);
		}

		protected override string GetDisplayField(string field)
		{
			DocumentFactory.DisplayFieldByIndexField.TryGetValue(field, out string displayField);
			return displayField ?? field;
		}

		protected override LuceneSearcherState<int, Card> CreateState() =>
			new CardSearcherState((CardDocumentAdapter) Adapter, _repo);

		protected override Directory CreateIndex(LuceneSearcherState<int, Card> state)
		{
			if (_version.IsUpToDate)
				return FSDirectory.Open(_version.IndexDirectory);

			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			_version.CreateDirectory();
			var index = base.CreateIndex(state);

			if (index == null)
				return null;

			index.SaveTo(_version.IndexDirectory);
			_version.SetIsUpToDate();

			return index;
		}

		public new CardSpellchecker Spellchecker => (CardSpellchecker) base.Spellchecker;

		public void InvalidateIndex() =>
			_version.Invalidate();

		public string IndexDirectoryParent
		{
			get => _version.IndexDirectory.Parent();
			set => _version = new IndexVersion(value, IndexVersions.CardSearcher);
		}

		public string IndexDirectory => _version.IndexDirectory;
		public bool IsUpToDate => _version.IsUpToDate;
		public int SetsAddedToIndex => GroupsAddedToIndex;

		private readonly CardRepository _repo;
		private IndexVersion _version;
	}
}