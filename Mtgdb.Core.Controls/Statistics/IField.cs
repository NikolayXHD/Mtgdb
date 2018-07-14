using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls.Statistics
{
	public interface IField<TObj>
	{
		string Name { get; }
		string Alias { get; }
		bool IsNumeric { get; }

		IOrderedEnumerable<TObj> OrderBy(IEnumerable<TObj> val, SortOrder order);
		IOrderedEnumerable<TObj> ThenOrderBy(IOrderedEnumerable<TObj> val, SortOrder order);
		List<KeyValuePair<List<object>, HashSet<TObj>>> GroupBy(IEnumerable<TObj> val, SortOrder order);
		List<KeyValuePair<List<object>, HashSet<TObj>>> ThenGroupBy(List<KeyValuePair<List<object>, HashSet<TObj>>> val, SortOrder order);

		int Count(HashSet<TObj> val);
		int CountDistinct(HashSet<TObj> val);
		object Sum(HashSet<TObj> val);
		object Min(HashSet<TObj> val);
		object Max(HashSet<TObj> val);
		object Average(HashSet<TObj> val);
	}
}