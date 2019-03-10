using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public static class FieldsExtension
	{
		public static IOrderedEnumerable<TDoc> OrderBy<TDoc>(this IEnumerable<TDoc> docs, FieldSortInfo fieldSortInfo, Fields<TDoc> fields) =>
			fields.ByName[fieldSortInfo.FieldName].OrderBy(docs, fieldSortInfo.SortOrder);

		public static IOrderedEnumerable<TDoc> ThenBy<TDoc>(this IOrderedEnumerable<TDoc> docsOrdered, FieldSortInfo fieldSortInfo, Fields<TDoc> fields) =>
			fields.ByName[fieldSortInfo.FieldName].ThenOrderBy(docsOrdered, fieldSortInfo.SortOrder);
	}
}