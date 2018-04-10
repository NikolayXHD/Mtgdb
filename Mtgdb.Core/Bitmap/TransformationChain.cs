using System;
using System.Collections.Generic;

namespace Mtgdb.Controls
{
	public class TransformationChain<TValue> : IDisposable
		where TValue : IDisposable
	{
		public TransformationChain(TValue original, Func<TValue, TValue> clone)
		{
			_clone = clone;
			_values.Add(original);
			_copying.Add(null);
			_inplace.Add(null);
		}

		public void TransformCopying(Func<TValue, TValue> copying)
		{
			TValue transformed;

			try
			{
				transformed = copying(Result);
			}
			catch (Exception ex)
			{
				TransformationException?.Invoke(ex);
				return;
			}

			if (transformed.Equals(Result))
				return;

			_values.Add(transformed);
			_copying.Add(copying);
			_inplace.Add(null);
		}

		public void TransformInplace(Action<TValue> inplace)
		{
			if (!HasPrevious)
			{
				TransformCopying(toCopying(inplace));
				return;
			}

			try
			{
				inplace(Result);
			}
			catch (Exception ex)
			{
				TransformationException?.Invoke(ex);
				Result = restoreCurrent();
				return;
			}

			_copying.Add(null);
			_inplace.Add(inplace);
		}

		private Func<TValue, TValue> toCopying(Action<TValue> inplaceTransformation)
		{
			return val =>
			{
				var cloned = _clone(val);
				inplaceTransformation(cloned);
				return cloned;
			};
		}

		private TValue restoreCurrent()
		{
			var previousValue = Previous;
			int lastCopyingIndex = _copying.FindLastIndex(t => t != null);

			var lastCopying = _copying[lastCopyingIndex];

			var firstInplaceIndex = lastCopyingIndex + 1;

			var lastInplace = _inplace.GetRange(
				firstInplaceIndex,
				_inplace.Count - firstInplaceIndex);

			var value = lastCopying(previousValue);

			foreach (var inplace in lastInplace)
				inplace(value);

			return value;
		}



		public void Dispose()
		{
			for (int i = 1; i < _values.Count - 1; i++)
				_values[i].Dispose();
		}

		public void DisposeDifferentOriginal()
		{
			if (_values.Count > 1)
				_values[0].Dispose();
		}

		public void DisposeDifferentResult()
		{
			if (_values.Count > 1)
				Result.Dispose();
		}

		public TValue Result
		{
			get => _values[_values.Count - 1];
			private set => _values[_values.Count - 1] = value;
		}

		private TValue Previous => _values[_values.Count - 2];
		private bool HasPrevious => _values.Count > 1;


		public event Action<Exception> TransformationException;

		private readonly List<TValue> _values = new List<TValue>();
		private readonly List<Func<TValue, TValue>> _copying = new List<Func<TValue, TValue>>();
		private readonly List<Action<TValue>> _inplace = new List<Action<TValue>>();
		private readonly Func<TValue, TValue> _clone;
	}
}