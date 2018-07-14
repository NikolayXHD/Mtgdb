using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
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

		public SortSubsystem(MtgLayoutView layoutViewCards, CardRepository repository, Fields fields, CardSearchSubsystem cardSearchSubsystem)
		{
			_fields = fields;
			_cardSearchSubsystem = cardSearchSubsystem;
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
			_sortedCardsByDefaultSort[_sortFromNewestToOldest].Clear();
			_sortedCardsByDefaultSort[_sortByIndexInFile].Clear();

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

		private List<Card> sort(IEnumerable<Card> cards, IList<FieldSortInfo> sortInfo, FieldSortInfo defaultSort)
		{
			var relevanceById = _cardSearchSubsystem?.SearchResult?.RelevanceById;

			float getRelevance(Card c) => 
				relevanceById?.TryGet(c.IndexInFile, int.MaxValue) ?? 0f;

			if (sortInfo.Count == 0)
			{
				return cards
					.OrderByDescending(getRelevance)
					.ThenBy(defaultSort, _fields)
					.ToList();
			}

			using (var enumerator = sortInfo.GetEnumerator())
			{
				enumerator.MoveNext();

				var cardsOrdered = cards.OrderBy(enumerator.Current, _fields);

				while (enumerator.MoveNext())
					cardsOrdered = cardsOrdered.ThenBy(enumerator.Current, _fields);

				cardsOrdered = cardsOrdered
					.ThenByDescending(getRelevance)
					.ThenBy(defaultSort, _fields);

				var result = cardsOrdered.ToList();
				return result;
			}
		}

		private static string toString(IList<FieldSortInfo> sortInfo)
		{
			string sort = string.Join(@",", sortInfo.Select(_ => $"{_.FieldName} {_.SortOrder}"));

			if (string.IsNullOrEmpty(sort))
				return string.Empty;

			return sort;
		}

		private static IEnumerable<FieldSortInfo> parse(string sort)
		{
			if (string.IsNullOrEmpty(sort))
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

		public List<Card> SortedCards =>
			getSortedCards(defaultSort: _sortByIndexInFile);

		public List<Card> DuplicateAwareSortedCards =>
			getSortedCards(defaultSort: _sortFromNewestToOldest);

		private List<Card> getSortedCards(FieldSortInfo defaultSort)
		{
			var result = _sortedCardsByDefaultSort[defaultSort];

			if (result.Count > 0)
				return result;

			var cards = _repository.IsLoadingComplete
				? _repository.Cards
				: Enumerable.Empty<Card>();

			result.AddRange(sort(cards, SortInfo, defaultSort: defaultSort));
			result.Capacity = result.Count;

			return result;
		}


		public string SortString { get; private set; }

		private readonly Dictionary<FieldSortInfo, List<Card>> _sortedCardsByDefaultSort =
			new Dictionary<FieldSortInfo, List<Card>>
			{
				[_sortByIndexInFile] = new List<Card>(),
				[_sortFromNewestToOldest] = new List<Card>()
			};

		public IList<FieldSortInfo> SortInfo { get; set; } = new List<FieldSortInfo>();

		public bool IsLanguageDependent => SortInfo.Any(_ => _localizableFields.Contains(_.FieldName));



		private static readonly FieldSortInfo _sortByIndexInFile =
			new FieldSortInfo(nameof(Card.IndexInFile), SortOrder.Ascending);

		private static readonly FieldSortInfo _sortFromNewestToOldest =
			new FieldSortInfo(nameof(Card.ReleaseDate), SortOrder.Descending);

		private readonly MtgLayoutView _layoutViewCards;
		private readonly CardRepository _repository;
		private readonly Fields _fields;
		private readonly CardSearchSubsystem _cardSearchSubsystem;
	}
}