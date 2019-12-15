using System.Collections.Generic;

namespace Mtgdb
{
	public static class ReadOnlyList
	{
		public static IReadOnlyList<T> From<T>(params T[] values) => values;
		public static IReadOnlyList<T> Empty<T>() => Mtgdb.Empty<T>.ReadOnlyList;
	}
}
