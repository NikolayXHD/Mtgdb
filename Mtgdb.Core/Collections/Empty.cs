using System.Collections.Generic;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb
{
	internal static class Empty<T>
	{
		public static T[] Array { get; } = new T[0];
		public static IReadOnlyList<T> ReadOnlyList { get; } = Array.AsReadOnlyList();
	}
}