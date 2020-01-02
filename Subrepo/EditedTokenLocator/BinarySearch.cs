using System;
using System.Collections.Generic;

namespace Lucene.Net.Contrib
{
	internal static class BinarySearch
	{
		public static int BinarySearchFirstIndexOf<T>(this IList<T> list, Func<T, bool> predicate)
		{
			if (list.Count == 0)
				return -1;

			return binarySearchFirstIndex(list, predicate, left: 0, count: list.Count);
		}

		private static int binarySearchFirstIndex<T>(this IList<T> list, Func<T, bool> predicate, int left, int count)
		{
			if (predicate(list[left]))
				return left;

			if (count == 1)
				return -1;

			int middle = left + count / 2;

			int searchRightHalfResult = binarySearchFirstIndex(list, predicate, middle, count - count / 2);

			if (searchRightHalfResult > middle)
				return searchRightHalfResult;

			if (searchRightHalfResult == -1)
				return -1;

			// searchRightHalfResult == middle

			int newCount = middle - left - 1;
			if (newCount == 0)
				return middle;

			int newLeft = left + 1;
			int searchLeftResult = binarySearchFirstIndex(list, predicate, newLeft, newCount);

			if (searchLeftResult == -1)
				return middle;

			return searchLeftResult;
		}
	}
}