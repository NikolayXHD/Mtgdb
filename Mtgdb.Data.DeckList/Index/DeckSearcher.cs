﻿using JetBrains.Annotations;
using Lucene.Net.Store;
using Mtgdb.Data.Model;

namespace Mtgdb.Data.Index
{
	public class DeckSearcher : LuceneSearcher<long, DeckModel>
	{
		[UsedImplicitly]
		public DeckSearcher(
			DeckListModel deckListModel,
			DeckSpellchecker spellchecker,
			DeckDocumentAdapter adapter)
			: base(spellchecker, adapter)
		{
			_deckListModel = deckListModel;
			IndexDirectoryParent = AppDir.Data.Join("index", "deck", "search");
		}

		public SearchResult<long> Search(string query) =>
			Search(query, language: null);

		public IntellisenseSuggest Suggest(TextInputState searchState) =>
			((DeckSpellchecker) Spellchecker).Suggest(searchState, language: null);

		public IntellisenseSuggest CycleValue(TextInputState input, bool backward) =>
			((DeckSpellchecker) Spellchecker).CycleValue(input, backward, language: null);

		protected override LuceneSearcherState<long, DeckModel> CreateState() =>
			new DeckSearcherState((DeckDocumentAdapter) Adapter, _deckListModel.GetModelCopies());

		protected override Directory CreateIndex(LuceneSearcherState<long, DeckModel> state)
		{
			lock (_syncDirectory)
				_version.RemoveObsoleteIndexes();

			Directory index;

			if (!_indexCreated && _version.IsUpToDate)
				lock (_syncDirectory)
				{
					using var fsDirectory = FSDirectory.Open(_version.IndexDirectory.Value);
					index = new RAMDirectory(fsDirectory, IOContext.READ_ONCE);
					_indexCreated = true;
					return index;
				}

			IsUpdating = true;

			index = base.CreateIndex(state);

			if (index != null)
			{
				lock (_syncDirectory)
				{
					_version.CreateDirectory();
					index.SaveTo(_version.IndexDirectory.Value);
					_version.SetIsUpToDate();
				}

				_indexCreated = true;
			}

			IsUpdating = false;
			InvokeIndexingProgressEvent();

			return index;
		}

		public FsPath IndexDirectoryParent
		{
			get => _version.IndexDirectory.Parent();
			set => _version = new IndexVersion(value, IndexVersions.DeckSearcher);
		}

		public void InvalidateIndex() =>
			_version.Invalidate();

		public bool IsIndexSaved => _version.IsUpToDate;

		private IndexVersion _version;
		private bool _indexCreated;

		public bool IsUpdating { get; private set; }

		private readonly DeckListModel _deckListModel;
		private static readonly object _syncDirectory = new object();
	}
}
