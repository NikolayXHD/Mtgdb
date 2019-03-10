using System;
using System.Collections.Generic;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb
{
	public static class BinarySearch
	{
		public static int BinarySearchFirstIndexOf<T>(this IList<T> list, Func<T, bool> predicate)
		{
			if (list.Count == 0)
				return -1;

			return binarySearchFirstIndex(list.AsReadOnlyList(), predicate, 0, list.Count);
		}

		public static int BinarySearchFirstIndexOf<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
		{
			if (list.Count == 0)
				return -1;

			return binarySearchFirstIndex(list, predicate, 0, list.Count);
		}

		public static int BinarySearchLastIndexOf<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
		{
			var predicateStopsBeingTrueAt = BinarySearchFirstIndexOf(list, _ => !predicate(_));
			if (predicateStopsBeingTrueAt == -1)
				return list.Count - 1;

			return predicateStopsBeingTrueAt - 1;
		}

		private static int binarySearchFirstIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate, int left, int count)
		{
			if (predicate(list[left]))
				return left;

			if (count == 1)
				return -1;

			var middle = left + count / 2;

			var searchRightHalfResult = binarySearchFirstIndex(list, predicate, middle, count - count / 2);

			if (searchRightHalfResult > middle)
				return searchRightHalfResult;

			if (searchRightHalfResult == -1)
				return -1;

			// searchRightHalfResult == middle

			var newCount = middle - left - 1;
			if (newCount == 0)
				return middle;

			var newLeft = left + 1;
			var searchLeftResult = binarySearchFirstIndex(list, predicate, newLeft, newCount);

			if (searchLeftResult == -1)
				return middle;

			return searchLeftResult;
		}
	}
}