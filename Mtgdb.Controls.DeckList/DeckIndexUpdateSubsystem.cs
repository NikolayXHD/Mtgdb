using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public class DeckIndexUpdateSubsystem
	{
		[UsedImplicitly] // in GuiLoader
		public DeckIndexUpdateSubsystem(DeckSearcher searcher, DeckListModel listModel)
		{
			_searcher = searcher;
			_listModel = listModel;
			_listModel.Changed += modelChanged;
		}

		public void Start() =>
			_started = true;

		private void modelChanged()
		{
			_modelUpdatedTime = DateTime.UtcNow;
			TaskEx.Run(handleModelChanged);
		}

		private async Task handleModelChanged()
		{
			while (!_started)
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
		private readonly object _sync = new object();
		private bool _started;
	}
}