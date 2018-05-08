using System.Collections.Generic;

namespace Mtgdb
{
	public static class Array
	{
		public static T[] Empty<T>() => Mtgdb.Empty<T>.Array;
		public static T[] From<T>(params T[] values) => values;
		public static int IndexOf<T>(this T[] values, T val) => ((IList<T>) values).IndexOf(val);
	}
}