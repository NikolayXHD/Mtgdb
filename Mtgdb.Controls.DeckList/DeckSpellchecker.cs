using JetBrains.Annotations;
using Lucene.Net.Store;
using Mtgdb.Dal.Index;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckSpellchecker : LuceneSpellchecker<long, DeckModel>
	{
		private const string IndexVersion = "0";

		[UsedImplicitly]
		public DeckSpellchecker(DeckDocumentAdapter adapter)
			: base(adapter)
		{
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("deck").AddPath("suggest");
		}

		protected override Directory CreateIndex(LuceneSearcherState<long, DeckModel> searcherState)
		{
			Directory index;

			if (!_indexCreated && _version.IsUpToDate)
			{
				lock (_syncDirectory)
					using (var fsDirectory = FSDirectory.Open(_version.Directory))
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

			index.SaveTo(_version.Directory);
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

		public string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVersion);
		}

		private IndexVersion _version;
		private bool _indexCreated;

		private readonly object _syncDirectory = new object();
	}
}