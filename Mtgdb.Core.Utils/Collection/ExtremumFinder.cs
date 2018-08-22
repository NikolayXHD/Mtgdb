using System;
using System.Collections.Generic;

namespace Mtgdb
{
	public class ExtremumFinder<TElement>
	{
		internal static ExtremumFinder<TElement> AtMin<TValue>(
			IEnumerable<TElement> sequence,
			Func<TElement, TValue> valueSelector)
			where TValue : IComparable<TValue>
		{
			return new ExtremumFinder<TElement>(
				sequence,
				new ExtremumSearcher<TValue>(valueSelector, parent: null, sign: -1));
		}

		internal static ExtremumFinder<TElement> AtMax<TValue>(
			IEnumerable<TElement> sequence,
			Func<TElement, TValue> valueSelector)
			where TValue : IComparable<TValue>
		{
			return new ExtremumFinder<TElement>(
				sequence,
				new ExtremumSearcher<TValue>(valueSelector, parent: null, sign: 1));
		}

		private ExtremumFinder(IEnumerable<TElement> sequence, IExtremumSearcher searcher)
		{
			_sequence = sequence;
			_searcher = searcher;
		}

		public ExtremumFinder<TElement> ThenAtMin<TValue>(Func<TElement, TValue> valueSelector)
			where TValue : IComparable<TValue>
		{
			_searcher = new ExtremumSearcher<TValue>(valueSelector, _searcher, sign: -1);
			return this;
		}

		public ExtremumFinder<TElement> ThenAtMax<TValue>(Func<TElement, TValue> valueSelector)
			where TValue : IComparable<TValue>
		{
			_searcher = new ExtremumSearcher<TValue>(valueSelector, _searcher, sign: 1);
			return this;
		}

		public TElement FindOrDefault()
		{
			using (var enumerator = _sequence.GetEnumerator())
			{
				if (!enumerator.MoveNext())
					return default;

				TElement result = enumerator.Current;
				_searcher.Compare(result);

				while (enumerator.MoveNext())
				{
					TElement element = enumerator.Current;

					if (_searcher.Compare(element) > 0)
						result = element;
				}

				return result;
			}
		}

		public TElement Find()
		{
			using (var enumerator = _sequence.GetEnumerator())
			{
				if (!enumerator.MoveNext())
					throw new ArgumentException("Sequence contains no elements");

				TElement result = enumerator.Current;
				_searcher.Compare(result);

				while (enumerator.MoveNext())
				{
					TElement element = enumerator.Current;

					if (_searcher.Compare(element) > 0)
						result = element;
				}

				return result;
			}
		}

		private readonly IEnumerable<TElement> _sequence;
		private IExtremumSearcher _searcher;



		private interface IExtremumSearcher
		{
			int Compare(TElement el);
		}

		private class ExtremumSearcher<TValue> : IExtremumSearcher
			where TValue : IComparable<TValue>
		{
			public ExtremumSearcher(Func<TElement, TValue> valueSelector, IExtremumSearcher parent, int sign)
			{
				_valueSelector = valueSelector;
				_parent = parent;
				_sign = sign;
			}

			public int Compare(TElement el)
			{
				TValue value;

				if (_parent != null)
				{
					var parentCompare = _parent.Compare(el);

					if (parentCompare < 0)
						return parentCompare;

					value = _valueSelector(el);

					if (parentCompare > 0)
					{
						_max = value;
						_hasValue = true;
						return parentCompare;
					}
				}
				else
				{
					value = _valueSelector(el);
				}

				if (!_hasValue)
				{
					_max = value;
					_hasValue = true;
					return 1;
				}

				int compare = value.CompareTo(_max);

				compare *= _sign;

				if (compare > 0)
					_max = value;

				return compare;
			}

			private readonly Func<TElement, TValue> _valueSelector;
			private readonly IExtremumSearcher _parent;
			private readonly int _sign;
			private TValue _max;
			private bool _hasValue;
		}
	}
}