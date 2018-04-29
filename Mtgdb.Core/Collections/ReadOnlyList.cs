using System.Collections.Generic;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb
{
	public static class ReadOnlyList
	{
		public static IReadOnlyList<T> From<T>(params T[] values) => values.AsReadOnlyList();
	}
}