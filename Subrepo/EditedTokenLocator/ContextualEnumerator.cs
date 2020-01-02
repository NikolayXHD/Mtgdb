using System;
using System.Collections;
using System.Collections.Generic;

namespace Lucene.Net.Contrib
{
	public sealed class ContextualEnumerator<T> : IEnumerator<T>
	{
		public ContextualEnumerator(IEnumerator<T> enumerator)
		{
			_enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));

			HasNext = _enumerator.MoveNext();

			if (HasNext)
				_next = _enumerator.Current;
		}

		public void Reset() =>
			_enumerator.Reset();

		object IEnumerator.Current => Current;

		public T Current
		{
			get
			{
				if (!_hasCurrent)
					throw new InvalidOperationException("Enumeration is not started or already finished");

				return _current;
			}
		}

		public T Previous
		{
			get
			{
				if (!HasPrevious)
					throw new InvalidOperationException("There is no previous element");

				return _previous;
			}
		}

		public T Next
		{
			get
			{
				if (!HasNext)
					throw new InvalidOperationException("There is no next element");

				return _next;
			}
		}

		public bool HasPrevious { get; private set; }

		public bool HasNext { get; private set; }

		public bool MoveNext()
		{
			if (!HasNext)
				return false;

			_previous = _current;
			HasPrevious = _hasCurrent;
			_current = _next;
			_hasCurrent = true;
			HasNext = _enumerator.MoveNext();

			if (HasNext)
				_next = _enumerator.Current;

			return true;
		}

		public void Dispose() =>
			_enumerator.Dispose();

		private readonly IEnumerator<T> _enumerator;

		private T _current;
		private T _previous;
		private T _next;
		private bool _hasCurrent;
	}
}