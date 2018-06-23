using System.Collections.Generic;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb
{
	public static class Empty<T>
	{
		public static T[] Array { get; } = new T[0];
		public static IReadOnlyList<T> ReadOnlyList { get; } = Array.AsReadOnlyList();
	}
}