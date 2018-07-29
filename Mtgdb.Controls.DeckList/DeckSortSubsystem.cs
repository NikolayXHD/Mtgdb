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
			DeckListModel deckListModel)
			: base(layoutView, fields, searchSubsystem)
		{
			_deckListModel = deckListModel;
			_deckListModel.Loaded += deckListLoaded;
			_deckListModel.Changed += deckListChanged;
		}

		private void deckListLoaded() =>
			Invalidate();

		private void deckListChanged() =>
			Invalidate();

		protected override long GetId(DeckModel doc) =>
			doc.Id;

		protected override bool IsLocalizable(string fieldName) =>
			false;

		protected override FieldSortInfo GetDefaultSort(bool duplicateAware) =>
			_sortFromNewestToOldest;

		protected override IEnumerable<DeckModel> GetDocuments() =>
			_deckListModel.IsLoaded
				? _deckListModel.GetModels()
				: Empty<DeckModel>.Sequence;

		private static readonly FieldSortInfo _sortFromNewestToOldest =
			new FieldSortInfo(nameof(Deck.Saved), SortOrder.Descending);

		private readonly DeckListModel _deckListModel;
	}
}