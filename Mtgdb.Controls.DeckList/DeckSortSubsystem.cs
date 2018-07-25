using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckSortSubsystem : SortSubsystem<long, DeckModel>
	{
		public DeckSortSubsystem(
			LayoutViewControl layoutView, 
			DeckFields fields, 
			DeckSearchSubsystem searchSubsystem,
			DeckListAsnycUpdateSubsystem updateSubsystem)
			: base(layoutView, fields, searchSubsystem)
		{
			_updateSubsystem = updateSubsystem;
			_updateSubsystem.ModelsUpdated += modelsUpdated;
		}

		public void TransformDecks(Func<bool> interrupt)
		{
			if (_models == null)
				return;

			for (int i = 0; i < _models.Count; i++)
			{
				var model = _models[i];

				if (interrupt())
					return;

				model.UpdateTransformedDeck();
				DeckTransformed?.Invoke(i + 1, _models.Count);
			}
		}

		private void modelsUpdated(IReadOnlyList<DeckModel> models)
		{
			_models = models;
			Invalidate();
		}

		protected override long GetId(DeckModel doc) =>
			doc.Id;

		protected override bool IsLocalizable(string fieldName) =>
			false;

		protected override FieldSortInfo GetDefaultSort(bool duplicateAware) =>
			_sortFromNewestToOldest;

		protected override IEnumerable<DeckModel> GetDocuments() =>
			_models ?? Empty<DeckModel>.Sequence;

		private IReadOnlyList<DeckModel> _models;

		private readonly DeckListAsnycUpdateSubsystem _updateSubsystem;

		public event Action<int, int> DeckTransformed;

		private static readonly FieldSortInfo _sortFromNewestToOldest =
			new FieldSortInfo(nameof(Deck.Id), SortOrder.Descending);
	}
}