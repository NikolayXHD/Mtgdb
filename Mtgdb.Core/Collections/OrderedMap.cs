using System;
using System.Collections.Generic;

namespace Mtgdb
{
	/// <summary>
	/// A map of <see cref="TKey"/>s to <see cref="TValue"/>s with <see cref="TKey"/>s ordered by a
	/// supplied <see cref="TOrderValue"/> value.
	/// </summary>
	public class OrderedMap<TKey, TValue, TOrderValue>
		where TOrderValue : IComparable<TOrderValue>
	{
		/// <summary>
		/// A map of <see cref="TKey"/>s to <see cref="TValue"/>s with <see cref="TKey"/>s ordered by a
		/// supplied <see cref="TOrderValue"/> value.
		/// </summary>
		public OrderedMap()
		{
			_keys = new SortedSet<TKey>(new CustomComparer<TKey>(compare));
		}

		/// <summary>
		/// Adds an <see cref="value"/> with a <see cref="key"/>, maintaining <see cref="key"/>s
		/// ordered by <see cref="orderValue"/> value
		/// </summary>
		public bool TryAdd(TKey key, TValue value, TOrderValue orderValue)
		{
			lock (_sync)
			{
				if (_map.ContainsKey(key))
					return false;

				_orderValues.Add(key, orderValue);
				_insertionOrder.Add(key, _counter++);
				_keys.Add(key);
				_map.Add(key, value);

				return true;
			}
		}

		/// <summary>
		/// Removes an <see cref="key"/> if it exists
		/// </summary>
		public bool TryRemove(TKey key)
		{
			lock (_sync)
			{
				if (!_orderValues.ContainsKey(key))
					return false;

				remove(key);
				return true;
			}
		}

		/// <summary>
		/// Removes and returns an <see cref="TKey"/> with minimum <see cref="TOrderValue"/>.
		/// When empty returns default(<see cref="TKey"/>).
		/// </summary>
		public (TKey Key, TValue Value) TryRemoveMin()
		{
			lock (_sync)
			{
				if (_keys.Count == 0)
					return (default(TKey), default(TValue));

				var key = _keys.Min;
				var value = _map[key];

				remove(key);
				return (key, value);
			}
		}



		private void remove(TKey key)
		{
			_keys.Remove(key);
			_orderValues.Remove(key);
			_insertionOrder.Remove(key);
			_map.Remove(key);
		}

		private int compare(TKey el1, TKey el2)
		{
			var compareOrder = _orderValues[el1].CompareTo(_orderValues[el2]);

			if (compareOrder != 0)
				return compareOrder;

			var compareInsertionOrder = _insertionOrder[el1].CompareTo(_insertionOrder[el2]);
			return compareInsertionOrder;
		}

		private readonly SortedSet<TKey> _keys;
		private readonly Dictionary<TKey, TValue> _map = new Dictionary<TKey, TValue>();
		private readonly Dictionary<TKey, TOrderValue> _orderValues = new Dictionary<TKey, TOrderValue>();
		private readonly Dictionary<TKey, long> _insertionOrder = new Dictionary<TKey, long>();

		private long _counter;

		private readonly object _sync = new object();
	}
}