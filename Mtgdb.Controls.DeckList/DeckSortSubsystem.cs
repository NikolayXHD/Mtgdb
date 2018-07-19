using System.Collections.Generic;
using System.Windows.Forms;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public class DeckSortSubsystem : SortSubsystem<int, DeckModel>
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

		private void modelsUpdated(IReadOnlyList<DeckModel> models)
		{
			_models = models;
			Invalidate();
		}

		protected override int GetId(DeckModel doc) =>
			doc.Id;

		protected override bool IsLocalizable(string fieldName) =>
			false;

		protected override FieldSortInfo GetDefaultSort(bool duplicateAware) =>
			_sortFromNewestToOldest;

		protected override IEnumerable<DeckModel> GetDocuments() =>
			_models ?? Empty<DeckModel>.Sequence;

		private IReadOnlyList<DeckModel> _models;

		private readonly DeckListAsnycUpdateSubsystem _updateSubsystem;

		private static readonly FieldSortInfo _sortFromNewestToOldest =
			new FieldSortInfo(nameof(Deck.Id), SortOrder.Descending);
	}
}