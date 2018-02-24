using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Controls.Statistics;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class SortSubsystem
	{
		public event Action SortChanged;

		private static readonly HashSet<string> _localizableFields = new HashSet<string>
		{
			nameof(Card.Name),
			nameof(Card.Type)
		};

		public SortSubsystem(LayoutView layoutViewCards, CardRepository repository)
		{
			_layoutViewCards = layoutViewCards;
			_repository = repository;

			ApplySort(string.Empty);
		}

		public void SubscribeToEvents()
		{
			_layoutViewCards.SortChanged += sortChanged;
		}

		public void UnsubscribeFromEvents()
		{
			_layoutViewCards.SortChanged -= sortChanged;
		}



		public void ApplySort(string sort)
		{
			var sortInfos = parse(sort).ToList();
			_layoutViewCards.SortInfo = sortInfos;
		}

		public void Invalidate()
		{
			_sortedCards = null;
			SortChanged?.Invoke();
		}

		private void sortChanged(object sender)
		{
			var sortInfo = _layoutViewCards.SortInfo;
			string sortString = toString(sortInfo);

			if (sortString == SortString)
				return;

			SortInfo = sortInfo;
			SortString = sortString;

			Invalidate();
		}

		private List<Card> sort(IEnumerable<Card> cards, IEnumerable<FieldSortInfo> sortInfo)
		{
			sortInfo = sortInfo.Concat(Enumerable.Repeat(_defaultSort, 1));
			using (var enumerator = sortInfo.GetEnumerator())
			{
				enumerator.MoveNext();
				var cardsOrdered = Fields.ByName[enumerator.Current.FieldName].OrderBy(cards, enumerator.Current.SortOrder);

				while (enumerator.MoveNext())
					cardsOrdered = Fields.ByName[enumerator.Current.FieldName].ThenOrderBy(cardsOrdered, enumerator.Current.SortOrder);

				var result = cardsOrdered.ToList();
				return result;
			}
		}

		private static string toString(IList<FieldSortInfo> sortInfo)
		{
			string sort = String.Join(@",", sortInfo.Select(_ => $"{_.FieldName} {_.SortOrder}"));

			if (String.IsNullOrEmpty(sort))
				return String.Empty;

			return sort;
		}

		private static IEnumerable<FieldSortInfo> parse(string sort)
		{
			if (String.IsNullOrEmpty(sort))
				yield break;

			var sortExpressions = sort.Split(',');
			for (int i = 0; i < sortExpressions.Length; i++)
			{
				var sortExpression = sortExpressions[i];
				string descMark = $@" {SortOrder.Descending}";
				string ascMark = $@" {SortOrder.Ascending}";

				SortOrder sortOrder;
				string fieldName;

				if (sortExpression.EndsWith(ascMark))
				{
					sortOrder = SortOrder.Ascending;
					fieldName = sortExpression.Substring(0, sortExpression.Length - ascMark.Length);
				}
				else
				{
					if (sortExpression.EndsWith(descMark))
					{
						sortOrder = SortOrder.Descending;
						fieldName = sortExpression.Substring(0, sortExpression.Length - descMark.Length);
					}
					else
						throw new Exception($"Invalid sort expression in history: {sort}");
				}

				yield return new FieldSortInfo(fieldName, sortOrder);
			}
		}

		public List<Card> SortedCards
		{
			get
			{
				if (_sortedCards != null)
					return _sortedCards;

				var cards = _repository.IsLoadingComplete
					? _repository.Cards
					: Enumerable.Empty<Card>();

				_sortedCards = sort(cards, SortInfo);
				_sortedCards.Capacity = _sortedCards.Count;

				return _sortedCards;
			}
		}

		private readonly LayoutView _layoutViewCards;
		private readonly CardRepository _repository;
		private List<Card> _sortedCards;
		private static readonly FieldSortInfo _defaultSort = new FieldSortInfo(nameof(Card.IndexInFile), SortOrder.Ascending);
		public string SortString { get; private set; }

		private IList<FieldSortInfo> SortInfo { get; set; } = new List<FieldSortInfo>();

		public bool IsLanguageDependent => SortInfo.Any(_ => _localizableFields.Contains(_.FieldName));

		public Fields Fields { get; set; }
	}
}