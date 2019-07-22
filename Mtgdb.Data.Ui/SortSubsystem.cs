using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using NLog;

namespace Mtgdb.Ui
{
	public abstract class SortSubsystem<TId, TDoc>
	{
		public event Action SortChanged;

		protected SortSubsystem(
			LayoutViewControl view,
			Fields<TDoc> fields,
			SearchSubsystem<TId, TDoc> searchSubsystem)
		{
			_fields = fields;
			_searchSubsystem = searchSubsystem;
			_view = view;

			ApplySort(string.Empty);
		}

		public void SubscribeToEvents()
		{
			_view.SortChanged += sortChanged;
		}

		public void UnsubscribeFromEvents()
		{
			_view.SortChanged -= sortChanged;
		}

		public void ApplySort(string sort)
		{
			var sortInfos = parse(sort).ToList();
			_view.SortInfo = sortInfos;
		}

		public void Invalidate()
		{
			_sortedDocsByDefaultSort.Clear();
			SortChanged?.Invoke();
		}

		private void sortChanged(object sender)
		{
			var sortInfo = _view.SortInfo;
			string sortString = toString(sortInfo);

			if (sortString == SortString)
				return;

			SortInfo = sortInfo;
			SortString = sortString;

			Invalidate();
		}

		private List<TDoc> sort(IEnumerable<TDoc> docs, IList<FieldSortInfo> sortInfo, FieldSortInfo defaultSort)
		{
			var relevanceById = _searchSubsystem?.SearchResult?.RelevanceById;

			float getRelevance(TDoc c)
			{
				var id = GetId(c);
				return id == null
					? int.MaxValue
					: relevanceById?.TryGet(id) ?? 0f;
			}

			if (sortInfo.Count == 0)
			{
				return docs
					.OrderByDescending(getRelevance)
					.ThenBy(defaultSort, _fields)
					.ToList();
			}

			using (var enumerator = sortInfo.GetEnumerator())
			{
				enumerator.MoveNext();

				var cardsOrdered = docs.OrderBy(enumerator.Current, _fields);

				while (enumerator.MoveNext())
					cardsOrdered = cardsOrdered.ThenBy(enumerator.Current, _fields);

				cardsOrdered = cardsOrdered
					.ThenByDescending(getRelevance)
					.ThenBy(defaultSort, _fields);

				var result = cardsOrdered.ToList();
				return result;
			}
		}

		protected abstract TId GetId(TDoc doc);

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
				string[] descMarks = { $@" {SortOrder.Descending}", $" {SortDirection.Desc}" };
				string[] ascMarks = { $@" {SortOrder.Ascending}", $" {SortDirection.Asc}" };

				SortDirection sortOrder;
				string fieldName;

				var ascMark = ascMarks.FirstOrDefault(_ => sortExpression.EndsWith(_, Str.Comparison));
				if (ascMark != null)
				{
					sortOrder = SortDirection.Asc;
					fieldName = sortExpression.Substring(0, sortExpression.Length - ascMark.Length);
				}
				else
				{
					var descMark = descMarks.FirstOrDefault(_ => sortExpression.EndsWith(_, Str.Comparison));
					if (descMark != null)
					{
						sortOrder = SortDirection.Desc;
						fieldName = sortExpression.Substring(0, sortExpression.Length - descMark.Length);
					}
					else
					{
						_log.Error($"Invalid sort expression in history: {sort}");
						continue;
					}
				}

				yield return new FieldSortInfo(fieldName, sortOrder);
			}
		}

		public List<TDoc> SortedRecords =>
			getSortedCards(GetDefaultSort(duplicateAware: false));

		protected abstract FieldSortInfo GetDefaultSort(bool duplicateAware);

		public List<TDoc> DuplicateAwareSortedCards =>
			getSortedCards(GetDefaultSort(duplicateAware: true));

		private List<TDoc> getSortedCards(FieldSortInfo defaultSort)
		{
			if (_sortedDocsByDefaultSort.TryGetValue(defaultSort, out var result))
				return result;

			var docs = GetDocuments();

			result = new List<TDoc>(sort(docs, SortInfo, defaultSort));
			_sortedDocsByDefaultSort.Add(defaultSort, result);

			return result;
		}

		protected abstract IEnumerable<TDoc> GetDocuments();

		public string SortString { get; private set; }

		public IList<FieldSortInfo> SortInfo { get; set; } =
			new List<FieldSortInfo>();

		public bool IsLanguageDependent =>
			SortInfo.Any(_ => IsLocalizable(_.FieldName));

		public string GetTextualStatus()
		{
			var infos = SortInfo?.ToList() ?? new List<FieldSortInfo>();

			if (_searchSubsystem.SearchResult?.RelevanceById != null)
				infos.Add(new FieldSortInfo("Relevance", SortDirection.Desc));

			if (infos.Count == 0)
				return "none";

			return string.Join(" ", infos);
		}


		protected abstract bool IsLocalizable(string fieldName);

		private readonly LayoutViewControl _view;
		private readonly Fields<TDoc> _fields;
		private readonly SearchSubsystem<TId, TDoc> _searchSubsystem;

		private readonly Dictionary<FieldSortInfo, List<TDoc>> _sortedDocsByDefaultSort =
			new Dictionary<FieldSortInfo, List<TDoc>>();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}