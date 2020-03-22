using System;
using System.Collections.Generic;

namespace Mtgdb
{
	public static class BinarySearch
	{
		public static int BinarySearchLastIndex<T>(this IList<T> list, Func<T, bool> predicate) =>
			SearchLast(0, list.Count, i => predicate(list[i]));

		public static int BinarySearchLastIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate) =>
			SearchLast(0, list.Count, i => predicate(list[i]));

		public static int BinarySearchFirstIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate) =>
			SearchFirst(0, list.Count, i => predicate(list[i]));

		public static int BinarySearchFirstIndex<T>(this IList<T> list, Func<T, bool> predicate) =>
			SearchFirst(0, list.Count, i => predicate(list[i]));

		public static int SearchLast(int left, int count, Func<int, bool> predicate)
		{
			var next = SearchFirst(left, count, _ => !predicate(_));
			if (next == -1)
				return count - 1;
			return next - 1;
		}

		public static int SearchFirst(int left, int count, Func<int, bool> predicate)
		{
			int right = count;

			while (left < right)
			{
				int middle = (left + right) / 2;
				if (predicate(middle))
					right = middle;
				else
					left = middle + 1;
			}

			if (left == count)
				return -1;

			return left;
		}
	}
}
