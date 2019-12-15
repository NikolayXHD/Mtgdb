using System.Collections.Generic;
using System.Linq;

namespace Mtgdb
{
	public static class Empty<T>
	{
		public static T[] Array { get; } = new T[0];
		public static IReadOnlyList<T> ReadOnlyList { get; } = Array;
		public static IEnumerable<T> Sequence { get; } = Enumerable.Empty<T>();
		public static HashSet<T> Set { get; } = new HashSet<T>();
	}
}
