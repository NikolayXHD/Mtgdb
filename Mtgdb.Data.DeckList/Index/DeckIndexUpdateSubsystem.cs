using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mtgdb.Data.Model;

namespace Mtgdb.Data.Index
{
	public class DeckIndexUpdateSubsystem
	{
		[UsedImplicitly] // in GuiLoader
		public DeckIndexUpdateSubsystem(
			DeckSearcher searcher,
			CardRepository repo,
			DeckListModel listModel)
		{
			_searcher = searcher;
			_repo = repo;
			_listModel = listModel;
		}

		public void SubscribeToEvents()
		{
			_listModel.Changed += modelChanged;
		}

		private void modelChanged()
		{
			_modelUpdatedTime = DateTime.UtcNow;
			TaskEx.Run(handleModelChanged);
		}

		private async Task handleModelChanged()
		{
			while (!_repo.IsLoadingComplete)
				await TaskEx.Delay(100);

			lock (_sync)
			{
				var modelUpdatedTime = _modelUpdatedTime;

				if (_indexUpdatedTime != modelUpdatedTime)
				{
					_searcher.LoadIndexes();
					_indexUpdatedTime = modelUpdatedTime;
				}
			}
		}

		private DateTime? _indexUpdatedTime;
		private DateTime _modelUpdatedTime = DateTime.UtcNow;

		private readonly DeckSearcher _searcher;
		private readonly DeckListModel _listModel;
		private readonly CardRepository _repo;
		private readonly object _sync = new object();
	}
}