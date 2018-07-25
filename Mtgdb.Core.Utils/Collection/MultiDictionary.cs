using System.Collections.Generic;

namespace Mtgdb
{
	public class MultiDictionary<TKey, TVal>
	{
		public MultiDictionary(
			IEqualityComparer<TKey> keyComparer = null,
			IEqualityComparer<TVal> valComparer = null)
		{
			_valComparer = valComparer ?? EqualityComparer<TVal>.Default;
			_dict = new Dictionary<TKey, HashSet<TVal>>(keyComparer ?? EqualityComparer<TKey>.Default);
		}

		public bool Add(TKey key, TVal val)
		{
			if (!_dict.TryGetValue(key, out var vals))
			{
				vals = new HashSet<TVal>(_valComparer);
				_dict.Add(key, vals);
			}

			return vals.Add(val);
		}

		public bool Remove(TKey key, TVal val)
		{
			if (!_dict.TryGetValue(key, out var vals))
				return false;

			bool result = vals.Remove(val);

			if (vals.Count == 0)
				_dict.Remove(key);

			return result;
		}

		public bool TryGetValues(TKey key, out HashSet<TVal> vals) =>
			_dict.TryGetValue(key, out vals);

		private readonly Dictionary<TKey, HashSet<TVal>> _dict;
		private readonly IEqualityComparer<TVal> _valComparer;
	}
}