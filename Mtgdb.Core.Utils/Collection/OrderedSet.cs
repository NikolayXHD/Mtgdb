using System;
using System.Collections;
using System.Collections.Generic;

namespace Mtgdb
{
	/// <summary>
	/// A set of <see cref="TValue"/>s ordered by a supplied <see cref="TOrderValue"/> value.
	/// </summary>
	public class OrderedSet<TValue, TOrderValue> : IEnumerable<TValue>
		where TOrderValue : IComparable<TOrderValue>
	{
		/// <summary>
		/// A set of <see cref="TValue"/>s ordered by a supplied <see cref="TOrderValue"/> value.
		/// </summary>
		public OrderedSet()
		{
			_values = new SortedSet<TValue>(new CustomComparer<TValue>(compare));
		}

		/// <summary>
		/// Adds a <see cref="value"/>, maintaining <see cref="value"/>s ordered by <see cref="orderValue"/>
		/// </summary>
		public bool TryAdd(TValue value, TOrderValue orderValue)
		{
			lock (_sync)
			{
				if (_orderValues.ContainsKey(value))
					return false;

				_orderValues.Add(value, orderValue);
				_insertionOrder.Add(value, _counter++);
				_values.Add(value);

				return true;
			}
		}

		/// <summary>
		/// Removes an <see cref="value"/> if it exists
		/// </summary>
		public bool TryRemove(TValue value)
		{
			lock (_sync)
			{
				if (!_orderValues.ContainsKey(value))
					return false;

				remove(value);
				return true;
			}
		}

		/// <summary>
		/// Removes and returns an <see cref="TValue"/> with minimum <see cref="TOrderValue"/>.
		/// When empty returns default(<see cref="TValue"/>).
		/// </summary>
		public TValue TryRemoveMin()
		{
			lock (_sync)
			{
				if (_values.Count == 0)
					return default;

				var key = _values.Min;

				remove(key);
				return key;
			}
		}

		public int Count => _values.Count;



		private void remove(TValue value)
		{
			_values.Remove(value);
			_orderValues.Remove(value);
			_insertionOrder.Remove(value);
		}

		private int compare(TValue el1, TValue el2)
		{
			var compareOrder = _orderValues[el1].CompareTo(_orderValues[el2]);

			if (compareOrder != 0)
				return compareOrder;

			var compareInsertionOrder = _insertionOrder[el1].CompareTo(_insertionOrder[el2]);
			return compareInsertionOrder;
		}

		private readonly SortedSet<TValue> _values;
		private readonly Dictionary<TValue, TOrderValue> _orderValues = new Dictionary<TValue, TOrderValue>();
		private readonly Dictionary<TValue, long> _insertionOrder = new Dictionary<TValue, long>();

		private long _counter;

		private readonly object _sync = new object();
		
		public IEnumerator<TValue> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}