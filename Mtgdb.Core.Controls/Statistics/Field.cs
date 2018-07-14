using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls.Statistics
{
	public class Field<TObj, TVal> : IField<TObj>
	{
		private readonly Func<TObj, TVal> _getter;
		public string Name { get; }
		public string Alias { get; }

		public bool IsNumeric => typeof (TVal) != typeof (string);
		
		public Field(Func<TObj, TVal> getter, string name, string alias)
		{
			_getter = getter;
			Name = name;
			Alias = alias;
		}

		public IOrderedEnumerable<TObj> OrderBy(IEnumerable<TObj> val, SortOrder order)
		{
			if (order == SortOrder.Ascending)
				return val.OrderBy(_ => _getter(_) == null).ThenBy(_getter);

			if (order == SortOrder.Descending)
				return val.OrderBy(_ => _getter(_) == null).ThenByDescending(_getter);

			throw new ArgumentOutOfRangeException(nameof(order));
		}

		public IOrderedEnumerable<TObj> ThenOrderBy(IOrderedEnumerable<TObj> val, SortOrder order)
		{
			if (order == SortOrder.Ascending)
				return val.ThenBy(_ => _getter(_) == null).ThenBy(_getter);

			if (order == SortOrder.Descending)
				return val.ThenBy(_ => _getter(_) == null).ThenByDescending(_getter);

			throw new ArgumentOutOfRangeException(nameof(order));
		}

		public List<KeyValuePair<List<object>, HashSet<TObj>>> GroupBy(IEnumerable<TObj> val, SortOrder order)
		{
			IEnumerable<IGrouping<TVal, TObj>> ordered;

			if (order == SortOrder.Ascending)
				ordered = val.GroupBy(_getter).OrderByDescending(_ => _.Key != null).ThenBy(_ => _.Key);
			else if (order == SortOrder.Descending)
				ordered = val.GroupBy(_getter).OrderBy(_=>_.Key != null).ThenByDescending(_ => _.Key);
			else
				ordered = val.GroupBy(_getter);

			return ordered
				.Select(grp => new KeyValuePair<List<object>, HashSet<TObj>>(
					new List<object> { grp.Key }, 
					new HashSet<TObj>(grp)))
				.ToList();
		}

		public List<KeyValuePair<List<object>, HashSet<TObj>>> ThenGroupBy(List<KeyValuePair<List<object>, HashSet<TObj>>> val, SortOrder order)
		{
			var result = new List<KeyValuePair<List<object>, HashSet<TObj>>>();

			foreach (var entry in val)
			{
				var key = entry.Key;
				var values = entry.Value;

				IEnumerable<IGrouping<TVal, TObj>> groups;

				if (order == SortOrder.Ascending)
					groups = values.GroupBy(_getter).OrderByDescending(_ => _.Key != null).ThenBy(_ => _.Key);
				else if (order == SortOrder.Descending)
					groups = values.GroupBy(_getter).OrderBy(_ => _.Key != null).ThenByDescending(_ => _.Key);
				else
					groups = values.GroupBy(_getter);

				foreach (var grp in groups)
				{
					var subkey = key.ToList();
					subkey.Add(grp.Key);
					result.Add(new KeyValuePair<List<object>, HashSet<TObj>>(
						subkey,
						new HashSet<TObj>(grp)));
				}
			}

			return result;
		}

		public int Count(HashSet<TObj> val)
		{
			return val.Count;
		}

		public int CountDistinct(HashSet<TObj> val)
		{
			return val.Select(_getter).Distinct().Count();
		}

		public object Sum(HashSet<TObj> val)
		{
			var values = val.Select(_ => _getter(_)).Where(_ => _ != null);

			var type = typeof (TVal);
			if (type == typeof(float) || type == typeof(float?))
				return values.Select(i=>Convert.ToSingle(i)).Cast<float?>().DefaultIfEmpty().Sum();

			if (type == typeof (int) || type == typeof (int?) ||
				type == typeof (short) || type == typeof (short?) ||
				type == typeof (byte) || type == typeof (byte?) ||
				type == typeof (bool) || type == typeof (bool?) ||
				type == typeof(long) || type == typeof(long?))
			{
				return values.Select(i=>Convert.ToInt64(i)).Cast<long?>().DefaultIfEmpty().Sum();
			}

			if (type == typeof(decimal) || type == typeof(decimal?))
				return values.Select(i=>Convert.ToDecimal(i)).Cast<decimal?>().DefaultIfEmpty().Sum();

			if (type == typeof(double) || type == typeof(double?))
				return values.Select(i=>Convert.ToDouble(i)).Cast<double?>().DefaultIfEmpty().Sum();

			throw new InvalidOperationException();
		}

		public object Min(HashSet<TObj> val)
		{
			var values = val.Select(_ => _getter(_)).Where(_ => _ != null);

			var type = typeof(TVal);
			if (type == typeof(float) || type == typeof(float?))
				return values.Select(i=>Convert.ToSingle(i)).Cast<float?>().DefaultIfEmpty().Min();

			if (type == typeof(int) || type == typeof(int?) ||
				type == typeof(short) || type == typeof(short?) ||
				type == typeof(byte) || type == typeof(byte?) ||
				type == typeof(bool) || type == typeof(bool?))
			{
				return values.Select(i=>Convert.ToInt32(i)).Cast<int?>().DefaultIfEmpty().Min();
			}

			if (type == typeof(decimal) || type == typeof(decimal?))
				return values.Select(i=>Convert.ToDecimal(i)).Cast<decimal?>().DefaultIfEmpty().Min();

			if (type == typeof(double) || type == typeof(double?))
				return values.Select(i=>Convert.ToDouble(i)).Cast<double?>().DefaultIfEmpty().Min();

			if (type == typeof(long) || type == typeof(long?))
				return values.Select(i=>Convert.ToInt64(i)).Cast<long?>().DefaultIfEmpty().Min();

			throw new InvalidOperationException();
		}

		public object Max(HashSet<TObj> val)
		{
			if (val.Count == 0)
				return null;

			var values = val.Select(_ => _getter(_)).Where(_ => _ != null);

			var type = typeof(TVal);
			if (type == typeof(float) || type == typeof(float?))
				return values.Select(i=>Convert.ToSingle(i)).Cast<float?>().DefaultIfEmpty().Max();

			if (type == typeof(int) || type == typeof(int?) ||
				type == typeof(short) || type == typeof(short?) ||
				type == typeof(byte) || type == typeof(byte?) ||
				type == typeof(bool) || type == typeof(bool?))
			{
				return values.Select(i=>Convert.ToInt32(i)).Cast<int?>().DefaultIfEmpty().Max();
			}

			if (type == typeof(decimal) || type == typeof(decimal?))
				return values.Select(i=>Convert.ToDecimal(i)).Cast<decimal?>().DefaultIfEmpty().Max();

			if (type == typeof(double) || type == typeof(double?))
				return values.Select(i=>Convert.ToDouble(i)).Cast<double?>().DefaultIfEmpty().Max();

			if (type == typeof(long) || type == typeof(long?))
				return values.Select(i=>Convert.ToInt64(i)).Cast<long?>().DefaultIfEmpty().Max();

			throw new InvalidOperationException();
		}

		public object Average(HashSet<TObj> val)
		{
			var values = val.Select(_ => _getter(_)).Where(_ => _ != null);

			var type = typeof(TVal);
			if (type == typeof(float) || type == typeof(float?))
				return values.Select(i=>Convert.ToSingle(i)).Cast<float?>().DefaultIfEmpty().Average();

			if (type == typeof(int) || type == typeof(int?) ||
				type == typeof(short) || type == typeof(short?) ||
				type == typeof(byte) || type == typeof(byte?) ||
				type == typeof(bool) || type == typeof(bool?))
			{
				return values.Select(i=>Convert.ToInt32(i)).Cast<int?>().DefaultIfEmpty().Average();
			}

			if (type == typeof(decimal) || type == typeof(decimal?))
				return values.Select(i=>Convert.ToDecimal(i)).Cast<decimal?>().DefaultIfEmpty().Average();

			if (type == typeof(double) || type == typeof(double?))
				return values.Select(i=>Convert.ToDouble(i)).Cast<double?>().DefaultIfEmpty().Average();

			if (type == typeof(long) || type == typeof(long?))
				return values.Select(i=>Convert.ToInt64(i)).Cast<long?>().DefaultIfEmpty().Average();

			throw new InvalidOperationException();
		}
	}
}