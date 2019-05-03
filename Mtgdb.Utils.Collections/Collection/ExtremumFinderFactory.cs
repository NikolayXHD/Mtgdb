using System;
using System.Collections.Generic;

namespace Mtgdb
{
	public static class ExtremumFinderFactory
	{
		public static ExtremumFinder<TElement> AtMin<TElement, TValue>(
			this IEnumerable<TElement> enumerable,
			Func<TElement, TValue> valueSelector,
			IComparer<TValue> customComparer = null)
			where TValue : IComparable<TValue>
		{
			return ExtremumFinder<TElement>.AtMin(enumerable, valueSelector, customComparer);
		}

		public static ExtremumFinder<TElement> AtMax<TElement, TValue>(
			this IEnumerable<TElement> enumerable,
			Func<TElement, TValue> valueSelector,
			IComparer<TValue> customComparer = null)
			where TValue : IComparable<TValue>
		{
			return ExtremumFinder<TElement>.AtMax(enumerable, valueSelector, customComparer);
		}
	}
}