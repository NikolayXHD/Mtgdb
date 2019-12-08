using System;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public static class FieldsExtension
	{
		public static IOrderedEnumerable<TDoc> OrderBy<TDoc>(
				this IEnumerable<TDoc> docs, FieldSortInfo fieldSortInfo, Fields<TDoc> fields)
		{
			if (fields.ByName.TryGetValue(fieldSortInfo.FieldName, out var field))
				return field.OrderBy(docs, fieldSortInfo.SortOrder);

			if (fields.SplitFieldsByName.TryGetValue(fieldSortInfo.FieldName, out var splitFields))
			{
				var result = splitFields[0].OrderBy(docs, fieldSortInfo.SortOrder);
				for (int i = 1; i < splitFields.Count; i++)
					result = splitFields[i].ThenOrderBy(result, fieldSortInfo.SortOrder);
				return result;
			}

			throw new ArgumentException($"Field {field.Name} not found");
		}

		public static IOrderedEnumerable<TDoc> ThenBy<TDoc>(
				this IOrderedEnumerable<TDoc> docsOrdered, FieldSortInfo fieldSortInfo, Fields<TDoc> fields)
		{
			if (fields.ByName.TryGetValue(fieldSortInfo.FieldName, out var field))
				return field.ThenOrderBy(docsOrdered, fieldSortInfo.SortOrder);

			if (fields.SplitFieldsByName.TryGetValue(fieldSortInfo.FieldName, out var splitFields))
			{
				var result = docsOrdered;
				foreach (var splitField in splitFields)
					result = splitField.ThenOrderBy(result, fieldSortInfo.SortOrder);
				return result;
			}

			throw new ArgumentException($"Field {field.Name} not found");
		}
	}
}
