using System;

namespace Mtgdb
{
	public static class ComparableExtension
	{
		public static (T val1, T val2) Swap<T>(this (T val1, T val2) vals) =>
			(vals.val2, vals.val1);

		public static float Modulo(this float v, int d)
		{
			v = v % d;
			if (v < 0)
				v += d;

			return v;
		}

		public static TVal WithinRange<TVal>(this TVal value, TVal min, TVal max) where TVal : IComparable<TVal> =>
			value.AtLeast(min).AtMost(max);

		public static TVal AtLeast<TVal>(this TVal value, TVal min) where TVal : IComparable<TVal> =>
			value.CompareTo(min) < 0 ? min : value;

		public static TVal AtMost<TVal>(this TVal value, TVal max) where TVal : IComparable<TVal> =>
			value.CompareTo(max) > 0 ? max : value;

		public static bool IsWithin<TVal>(this TVal value, TVal min, TVal max) where TVal : IComparable<TVal> =>
			min.CompareTo(value) < 0 && 0 < max.CompareTo(value);
	}
}