using System.Collections.Generic;
using System.Linq;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public static class FieldsExtension
	{
		public static IOrderedEnumerable<Card> OrderBy(this IEnumerable<Card> cards, FieldSortInfo fieldSortInfo, Fields fields)
		{
			return fields.ByName[fieldSortInfo.FieldName].OrderBy(cards, fieldSortInfo.SortOrder);
		}

		public static IOrderedEnumerable<Card> ThenBy(this IOrderedEnumerable<Card> cardsOrdered, FieldSortInfo fieldSortInfo, Fields fields)
		{
			return fields.ByName[fieldSortInfo.FieldName].ThenOrderBy(cardsOrdered, fieldSortInfo.SortOrder);
		}
	}
}