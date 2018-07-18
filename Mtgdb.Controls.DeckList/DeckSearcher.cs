using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Mtgdb.Dal.Index;
using Mtgdb.Index;

namespace Mtgdb.Controls
{
	public class DeckSearcher : LuceneSearcher<int, DeckModel>
	{
		private const string IndexVerision = "0";

		[UsedImplicitly]
		public DeckSearcher(
			DeckListAsnycUpdateSubsystem updateSubsystem,
			DeckSpellchecker spellchecker,
			DeckDocumentAdapter adapter)
			: base(spellchecker, adapter)
		{
			_updateSubsystem = updateSubsystem;
			_updateSubsystem.HandleModelsUpdated += handleModelsUpdated;

			IndexDirectoryParent = AppDir.Data.AddPath("index").AddPath("deck").AddPath("search");
		}

		protected override Func<IEnumerable<IEnumerable<Document>>> GetDocumentGroupsToIndex()
		{
			var models = _models;
			return () => Sequence.From(models.Select(Adapter.ToDocument));
		}

		public SearchResult<int> Search(string query) =>
			Search(query, language: null);

		public IntellisenseSuggest Suggest(TextInputState searchState) =>
			((DeckSpellchecker) Spellchecker).Suggest(searchState, language: null);

		public IntellisenseSuggest CycleValue(TextInputState input, bool backward) =>
			((DeckSpellchecker) Spellchecker).CycleValue(input, backward, language: null);

		protected override Directory CreateIndex(SearcherState<int, DeckModel> state)
		{
			Directory index;

			if (!_indexCreated)
			{
				var models = _updateSubsystem.GetModels();

				_models = models;
				((DeckSpellchecker) Spellchecker).Models = models;

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

		private void handleModelsUpdated(IReadOnlyList<DeckModel> models)
		{
			_models = models;
			((DeckSpellchecker) Spellchecker).Models = models;

			LoadIndexes();
		}

		private string IndexDirectoryParent
		{
			get => _version.Directory.Parent();
			set => _version = new IndexVersion(value, IndexVerision);
		}

		private IReadOnlyList<DeckModel> _models;

		private IndexVersion _version;
		private bool _indexCreated;

		public bool IsUpdating { get; private set; }

		private readonly DeckListAsnycUpdateSubsystem _updateSubsystem;
		private static readonly object _syncDirectory = new object();
	}
}