using System;
using System.Collections.Generic;

namespace Mtgdb.Controls
{
	public class EventFiringMap<TKey, TVal>
	{
		public event Action<TKey, TVal, TVal> Changed;

		public TVal this[TKey key]
		{
			get => _visibilityByDirection.TryGet(key);
			set
			{
				var prevValue = _visibilityByDirection.TryGet(key);

				if (Equals(prevValue, value))
					return;

				_visibilityByDirection[key] = value;
				Changed?.Invoke(key, prevValue, value);
			}
		}

		private readonly Dictionary<TKey, TVal> _visibilityByDirection = new Dictionary<TKey, TVal>();
	}
}