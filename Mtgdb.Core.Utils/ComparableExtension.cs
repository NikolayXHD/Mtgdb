using System;

namespace Mtgdb
{
	public static class ComparableExtension
	{
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