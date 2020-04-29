using JetBrains.Annotations;
using Lucene.Net.Store;
using Mtgdb.Data.Model;

namespace Mtgdb.Data.Index
{
	public class DeckSpellchecker : LuceneSpellchecker<long, DeckModel>
	{
		[UsedImplicitly]
		public DeckSpellchecker(DeckDocumentAdapter adapter)
			: base(adapter)
		{
			IndexDirectoryParent = AppDir.Data.Join("index", "deck", "suggest");
		}

		protected override Directory CreateIndex(LuceneSearcherState<long, DeckModel> searcherState)
		{
			lock (_syncDirectory)
				_version.RemoveObsoleteIndexes();

			Directory index;

			if (!_indexCreated && _version.IsUpToDate)
			{
				lock (_syncDirectory)
					using (var fsDirectory = FSDirectory.Open(_version.IndexDirectory.Value))
						index = new RAMDirectory(fsDirectory, IOContext.READ_ONCE);

				var spellchecker = CreateSpellchecker();
				spellchecker.Load(index);

				var state = CreateState(searcherState, spellchecker, loaded: true);
				Update(state);

				_indexCreated = true;
				return index;
			}

			index = base.CreateIndex(searcherState);

			if (index == null)
				return null;

			lock (_syncDirectory)
				_version.CreateDirectory();

			index.SaveTo(_version.IndexDirectory.Value);
			_version.SetIsUpToDate();

			_indexCreated = true;
			return index;
		}

		protected override LuceneSpellcheckerState<long, DeckModel> CreateState(
			LuceneSearcherState<long, DeckModel> searcherState,
			Spellchecker spellchecker,
			bool loaded) =>
			new DeckSpellcheckerState(
				spellchecker,
				(DeckSearcherState) searcherState,
				(DeckDocumentAdapter) Adapter,
				() => MaxCount,
				loaded);

		public FsPath IndexDirectoryParent
		{
			get => _version.IndexDirectory.Parent();
			set => _version = new IndexVersion(value, IndexVersions.DeckSpellchecker);
		}

		private IndexVersion _version;
		private bool _indexCreated;

		private readonly object _syncDirectory = new object();
	}
}
