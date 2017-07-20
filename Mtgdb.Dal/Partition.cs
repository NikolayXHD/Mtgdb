using System;
using System.Collections.Generic;

namespace Mtgdb.Dal
{
	public static class Partition
	{
		private static readonly Random _rand = new Random();

		public static TKey RandomizedSelect<TKey, TVal>(IList<TVal> values, IList<TKey> sortKeys, int count, IComparer<TKey> comparer = null)
			where TKey : IComparable<TKey>
		{
			return randomizedSelect(values, sortKeys, 0, values.Count - 1, count, comparer);
		}

		private static TKey randomizedSelect<TKey, TVal>(IList<TVal> values, IList<TKey> sortKeys, int minIndex, int maxIndex, int count, IComparer<TKey> comparer)
			where TKey : IComparable<TKey>
		{
			while (true)
			{
				if (minIndex == maxIndex)
					return sortKeys[minIndex];

				var q = randomizedPartition(values, sortKeys, minIndex, maxIndex, comparer);
				var k = q - minIndex + 1;

				if (count <= k)
				{
					maxIndex = q;
					continue;
				}

				minIndex = q + 1;
				count = count - k;
			}
		}

		private static int randomizedPartition<TKey, TVal>(IList<TVal> v, IList<TKey> a, int p, int r, IComparer<TKey> comparer)
			where TKey : IComparable<TKey>
		{
			int i = _rand.Next(p, r + 1);
			swap(v, a, p, i);
			return partition(v, a, p, r, comparer);
		}

		private static int partition<TKey, TVal>(IList<TVal> v, IList<TKey> a, int p, int r, IComparer<TKey> comparer)
			where TKey : IComparable<TKey>
		{
			var x = a[p];
			var i = p - 1;
			var j = r + 1;

			while (true)
			{
				do
				{
					j--;
				} while ((comparer?.Compare(a[j], x) ?? a[j].CompareTo(x)) > 0);

				do
				{
					i++;
				} while ((comparer?.Compare(a[i], x) ?? a[i].CompareTo(x)) < 0);

				if (i < j)
				{
					swap(v, a, i, j);
				}
				else return j;
			}
		}

		private static void swap<TKey, TVal>(IList<TVal> v, IList<TKey> a, int i, int j)
		{
			var buf1 = v[i];
			var buf2 = a[i];

			v[i] = v[j];
			a[i] = a[j];

			v[j] = buf1;
			a[j] = buf2;
		}
	}
}