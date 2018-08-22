using System;
using Lucene.Net.Store;
using Mtgdb.Index;

namespace Mtgdb.Dal.Index
{
	public class CardSpellchecker : LuceneSpellchecker<int, Card>
	{
		// C18
		private const string IndexVersion = "0.42";

		public CardSpellchecker(CardRepository repo, CardDocumentAdapter adapter)
			: base(adapter)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("suggest");
			_repo = repo;
		}

		protected override Directory CreateIndex(LuceneSearcherState<int, Card> searcherState)
		{
			Directory index;

			if (_version.IsUpToDate)
			{
				using (var fsDirectory = FSDirectory.Open(_version.Directory))
					index = new RAMDirectory(fsDirectory, IOContext.READ_ONCE);

				var spellchecker = CreateSpellchecker();
				spellchecker.Load(index);

				var state = CreateState(searcherState, spellchecker, loaded: true);
				Update(state);

				return index;
			}

			if (!_repo.IsLocalizationLoadingComplete)
				throw new InvalidOperationException($"{nameof(CardRepository)} must load localizations first");

			_version.CreateDirectory();
			index = base.CreateIndex(searcherState);

			if (index == null)
				return null;

			index.SaveTo(_version.Directory);
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
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVersion);
		}

		public string IndexDirectory => _version.Directory;
		public bool IsUpToDate => _version.IsUpToDate;

		private IndexVersion _version;
		private readonly CardRepository _repo;
	}
}