using System;
using System.Collections.Generic;
using System.Threading;
using Mtgdb.Dal;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Controls
{
	public class DeckListAsnycUpdateSubsystem
	{
		public DeckListAsnycUpdateSubsystem(DeckListModel deckList, UiModelSnapshotFactory uiFactory)
		{
			_deckList = deckList;
			_uiFactory = uiFactory;
		}

		public IReadOnlyList<DeckModel> GetModels()
		{
			var ui = _uiFactory.Snapshot();
			var result = _deckList.GetModels(ui).ToReadOnlyList();
			return result;
		}

		public void ModelChanged()
		{
			_modelUpdatedTime = DateTime.UtcNow;

			var models = GetModels();
			ModelsUpdated?.Invoke(models);

			ThreadPool.QueueUserWorkItem(_ =>
			{
				lock (_sync)
				{
					var modelUpdatedTime = _modelUpdatedTime;

					if (_indexUpdatedTime == modelUpdatedTime)
						return;

					HandleModelsUpdated?.Invoke(models);

					_indexUpdatedTime = modelUpdatedTime;
				}
			});
		}

		private DateTime? _indexUpdatedTime;
		private DateTime _modelUpdatedTime = DateTime.UtcNow;

		private readonly object _sync = new object();

		private readonly DeckListModel _deckList;
		private readonly UiModelSnapshotFactory _uiFactory;

		public event Action<IReadOnlyList<DeckModel>> HandleModelsUpdated;
		public event Action<IReadOnlyList<DeckModel>> ModelsUpdated;
	}
}