using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class CardSortSubsystem : SortSubsystem<int, Card>
	{
		private static readonly HashSet<string> _localizableFields = new HashSet<string>
		{
			nameof(Card.Name),
			nameof(Card.Type)
		};

		public CardSortSubsystem(
			LayoutViewControl layoutView,
			CardRepository repo,
			CardFields fields,
			CardSearchSubsystem searchSubsystem)
			: base(layoutView, fields, searchSubsystem)
		{
			_repo = repo;
		}

		protected override FieldSortInfo GetDefaultSort(bool duplicateAware) =>
			_sortFromNewestToOldest;

		protected override IEnumerable<Card> GetDocuments() =>
			_repo.IsLoadingComplete
				? _repo.Cards
				: Enumerable.Empty<Card>();

		protected override int GetId(Card card) =>
			card.IndexInFile;

		protected override bool IsLocalizable(string fieldName) =>
			_localizableFields.Contains(fieldName);

		private static readonly FieldSortInfo _sortFromNewestToOldest =
			new FieldSortInfo(nameof(Card.ReleaseDate), SortOrder.Descending);

		private readonly CardRepository _repo;
	}
}