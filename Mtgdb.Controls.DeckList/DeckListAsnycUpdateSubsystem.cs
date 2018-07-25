using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using Mtgdb.Dal;

namespace Mtgdb.Controls
{
	public class DeckListAsnycUpdateSubsystem
	{
		[UsedImplicitly]
		public DeckListAsnycUpdateSubsystem(
			CardRepository repo,
			DeckListModel deckList,
			IDeckTransformation transformation)
		{
			_repo = repo;
			_deckList = deckList;
			_transformation = transformation;
		}

		public IReadOnlyList<DeckModel> GetModels()
		{
			
			var result = _deckList.
				GetModels(_repo, collection, _transformation).ToReadOnlyList();
			return result;
		}

		public void ModelChanged(Card collectionCard, HashSet<Deck> decksAdded, Deck deckRemoved, Deck deckRenamed)
		{
			_modelUpdatedTime = DateTime.UtcNow;

			var models = getChangedModels(collectionCard);
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

		private IReadOnlyList<DeckModel> getChangedModels(Card card)
		{
			if (card == null || _models == null)
			{
				_models = GetModels();
				return _models;
			}

			var models = _models;
			foreach (var model in models)
			{
				if (model.IsAffectedByCollectionCard(card))
					model.ResetTransformedDeck();
			}

			return models;
		}

		private DateTime? _indexUpdatedTime;
		private DateTime _modelUpdatedTime = DateTime.UtcNow;

		private readonly object _sync = new object();

		private readonly CardRepository _repo;
		private readonly DeckListModel _deckList;
		private readonly IDeckTransformation _transformation;

		private IReadOnlyList<DeckModel> _models;

		public event Action<IReadOnlyList<DeckModel>> HandleModelsUpdated;
		public event Action<IReadOnlyList<DeckModel>> ModelsUpdated;
	}
}