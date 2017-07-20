using System;

namespace Mtgdb
{
	public static class ComparableExtension
	{
		public static TVal WhithinRange<TVal>(this TVal value, TVal? min, TVal? max)
			where TVal : struct, IComparable<TVal>
		{
			if (min.HasValue && value.CompareTo(min.Value) < 0)
				return min.Value;

			if (max.HasValue && value.CompareTo(max.Value) > 0)
				return max.Value;

			return value;
		}
	}
}