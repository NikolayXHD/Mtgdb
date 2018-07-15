using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Index;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class DeckSearcher : LuceneSearcher<int, DeckModel>
	{
		private const string IndexVerision = "0";

		[UsedImplicitly]
		public DeckSearcher(
			DeckListModel deckList,
			DeckSpellchecker spellchecker,
			DeckDocumentAdapter adapter,
			UiModelSnapshotFactory uiFactory)
			: base(spellchecker, adapter)
		{
			_deckList = deckList;
			_uiFactory = uiFactory;
			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("deck").AddPath("search");
		}

		protected override IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex() =>
			Sequence.From(Models.Select(Adapter.ToDocument));

		public SearchResult<int> Search(string query) =>
			Search(query, language: null);

		public IntellisenseSuggest Suggest(TextInputState searchState) =>
			((DeckSpellchecker) Spellchecker).Suggest(searchState, language: null);

		public IntellisenseSuggest CycleValue(TextInputState input, bool backward) =>
			((DeckSpellchecker) Spellchecker).CycleValue(input, backward, language: null);

		public void ModelChanged()
		{
			_modelUpdatedTime = DateTime.UtcNow;

			ThreadPool.QueueUserWorkItem(_ =>
			{
				lock (_sync)
				{
					var modelUpdatedTime = _modelUpdatedTime;

					if (_indexUpdatedTime == modelUpdatedTime)
						return;

					updateModels();
					LoadIndexes();

					_indexUpdatedTime = modelUpdatedTime;
				}
			});
		}

		protected override Directory CreateIndex(SearcherState<int, DeckModel> state)
		{
			Directory index;

			if (!_indexCreated)
			{
				updateModels();

				if (_version.IsUpToDate)
					lock (_syncDirectory)
						using (var fsDirectory = FSDirectory.Open(_version.Directory))
						{
							index = new RAMDirectory(fsDirectory, IOContext.READ_ONCE);
							_indexCreated = true;
							return index;
						}
			}

			IsUpdating = true;

			index = base.CreateIndex(state);

			if (index != null)
			{
				lock (_syncDirectory)
					_version.CreateDirectory();

				index.SaveTo(_version.Directory);
				_version.SetIsUpToDate();

				_indexCreated = true;
			}

			IsUpdating = false;

			return index;
		}

		private void updateModels()
		{
			var ui = _uiFactory.Snapshot();
			var models = _deckList.GetModels(ui).ToReadOnlyList();

			Models = models;
			((DeckSpellchecker) Spellchecker).Models = models;
		}

		private string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVerision);
		}

		private DateTime? _indexUpdatedTime;
		private DateTime _modelUpdatedTime = DateTime.UtcNow;

		private IReadOnlyList<DeckModel> Models { get; set; }

		private readonly UiModelSnapshotFactory _uiFactory;
		private readonly DeckListModel _deckList;
		private readonly object _sync = new object();

		private IndexVersion _version;
		private bool _indexCreated;

		public bool IsUpdating { get; private set; }

		private static readonly object _syncDirectory = new object();
	}
}