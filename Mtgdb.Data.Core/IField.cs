using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public interface IField<TObj>
	{
		string Name { get; }
		string Alias { get; }
		bool IsNumeric { get; }

		IOrderedEnumerable<TObj> OrderBy(IEnumerable<TObj> val, SortDirection order);
		IOrderedEnumerable<TObj> ThenOrderBy(IOrderedEnumerable<TObj> val, SortDirection order);
		List<KeyValuePair<List<object>, HashSet<TObj>>> GroupBy(IEnumerable<TObj> val, SortDirection order);
		List<KeyValuePair<List<object>, HashSet<TObj>>> ThenGroupBy(List<KeyValuePair<List<object>, HashSet<TObj>>> val, SortDirection order);

		int Count(HashSet<TObj> val);
		int CountDistinct(HashSet<TObj> val);
		object Sum(HashSet<TObj> val);
		object Min(HashSet<TObj> val);
		object Max(HashSet<TObj> val);
		object Average(HashSet<TObj> val);
	}
}