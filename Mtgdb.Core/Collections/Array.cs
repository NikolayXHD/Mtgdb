using System.Collections.Generic;

namespace Mtgdb
{
	public static class Array
	{
		public static T[] From<T>(params T[] values) => values;

		public static int IndexOf<T>(this T[] values, T val)
		{
			return ((IList<T>) values).IndexOf(val);
		}
	}
}