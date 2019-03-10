using System;
using System.Collections.Generic;

namespace Mtgdb
{
	public static class ExtremumFinderFactory
	{
		public static ExtremumFinder<TElement> AtMin<TElement, TValue>(
			this IEnumerable<TElement> enumerable,
			Func<TElement, TValue> valueSelector)
			where TValue : IComparable<TValue>
		{
			return ExtremumFinder<TElement>.AtMin(enumerable, valueSelector);
		}

		public static ExtremumFinder<TElement> AtMax<TElement, TValue>(
			this IEnumerable<TElement> enumerable,
			Func<TElement, TValue> valueSelector)
			where TValue : IComparable<TValue>
		{
			return ExtremumFinder<TElement>.AtMax(enumerable, valueSelector);
		}
	}
}