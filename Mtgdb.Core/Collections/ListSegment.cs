using System;
using System.Collections;
using System.Collections.Generic;

namespace Mtgdb
{
	public class ListSegment<T> : IReadOnlyList<T>
	{
		private readonly IReadOnlyList<T> _source;
		private readonly int _offset;

		public ListSegment(IReadOnlyList<T> source, int offset, int count)
		{
			_source = source;
			_offset = Math.Min(offset, source.Count);
			Count = Math.Min(count, source.Count - _offset);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new ListEnumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ListEnumerator(this);
		}

		public int Count { get; }

		public T this[int index] => _source[_offset + index];

		private class ListEnumerator : IEnumerator<T>
		{
			private readonly IReadOnlyList<T> _source;
			private readonly int _lastIndexPlusOne;
			private int _currentIndex;

			public ListEnumerator(IReadOnlyList<T> source)
			{
				_source = source;
				_lastIndexPlusOne = source.Count;
				((IEnumerator) this).Reset();
			}

			public bool MoveNext()
			{
				if (_currentIndex >= _lastIndexPlusOne)
					return false;
				++_currentIndex;
				return _currentIndex < _lastIndexPlusOne;
			}

			void IEnumerator.Reset()
			{
				_currentIndex = - 1;
			}

			public void Dispose()
			{
			}

			public T Current
			{
				get
				{
					if (_currentIndex < 0)
						throw new InvalidOperationException("Enumeration has not started");

					if (_currentIndex >= _lastIndexPlusOne)
						throw new InvalidOperationException("Enumeration ended");

					return _source[_currentIndex];
				}
			}

			object IEnumerator.Current => Current;
		}
	}
}