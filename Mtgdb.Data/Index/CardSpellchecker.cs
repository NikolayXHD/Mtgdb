using System;
using Lucene.Net.Store;

namespace Mtgdb.Data.Index
{
	public class CardSpellchecker : LuceneSpellchecker<int, Card>
	{
		public CardSpellchecker(CardRepository repo, CardDocumentAdapter adapter)
			: base(adapter)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("suggest");
			_repo = repo;
		}

		protected override Directory CreateIndex(LuceneSearcherState<int, Card> searcherState)
		{
			_version.RemoveObsoleteIndexes();

			Directory index;

			if (_version.IsUpToDate)
			{
				using var directory = FSDirectory.Open(_version.IndexDirectory);
				index = new RAMDirectory(directory, IOContext.READ_ONCE);

				var spellchecker = CreateSpellchecker();
				spellchecker.Load(index);

				var state = CreateState(searcherState, spellchecker, loaded: true);
				Update(state);

				return index;
			}

			if (!_repo.IsLocalizationLoadingComplete.Signaled)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			_version.CreateDirectory();
			index = base.CreateIndex(searcherState);

			if (index == null)
				return null;

			index.SaveTo(_version.IndexDirectory);
			_version.SetIsUpToDate();

			return index;
		}

		protected override LuceneSpellcheckerState<int, Card> CreateState(LuceneSearcherState<int, Card> searcherState, Spellchecker spellchecker, bool loaded) =>
			new CardSpellcheckerState(
				_repo,
				spellchecker,
				(CardSearcherState) searcherState, (CardDocumentAdapter) Adapter,
				() => MaxCount,
				loaded);

		public void InvalidateIndex() =>
			_version.Invalidate();



		public string IndexDirectoryParent
		{
			get => _version.IndexDirectory.Parent();
			set => _version = new IndexVersion(value, IndexVersions.CardSpellchecker);
		}

		public string IndexDirectory => _version.IndexDirectory;
		public bool IsUpToDate => _version.IsUpToDate;

		private IndexVersion _version;
		private readonly CardRepository _repo;
	}
}
