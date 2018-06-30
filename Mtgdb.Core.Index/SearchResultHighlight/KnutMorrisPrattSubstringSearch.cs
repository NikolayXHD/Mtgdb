using System.Collections.Generic;

namespace Mtgdb.Index
{
	public class KnutMorrisPrattSubstringSearch<T>
	{
		public KnutMorrisPrattSubstringSearch(IReadOnlyList<T> pattern, IEqualityComparer<T> comparer)
		{
			_comparer = comparer;
			_pattern = pattern;
			_table = buildTable(pattern); // use a helper to build T
		}

		private int[] buildTable(IReadOnlyList<T> pattern)
		{
			int[] result = new int[pattern.Count];
			int pos = 2;
			int cnd = 0;
			result[0] = -1;
			result[1] = 0;
			while (pos < pattern.Count)
			{
				if (_comparer.Equals(pattern[pos - 1], pattern[cnd]))
				{
					++cnd;
					result[pos] = cnd;
					++pos;
				}
				else if (cnd > 0)
					cnd = result[cnd];
				else
				{
					result[pos] = 0;
					++pos;
				}
			}

			return result;
		}

		public IEnumerable<int> FindAll(IReadOnlyList<T> input)
		{
			int start = 0;

			while (start < input.Count)
			{
				int result = Find(input, start);

				if (result == -1)
					yield break;

				yield return result;

				start = result + _pattern.Count;
			}
		}

		public int Find(IReadOnlyList<T> input, int start)
		{
			int m = start;
			int i = 0;
			while (m + i < input.Count)
			{
				if (_comparer.Equals(_pattern[i], input[m + i]))
				{
					if (i == _pattern.Count - 1)
						return m;
					++i;
				}
				else
				{
					m = m + i - _table[i];
					if (_table[i] > -1)
						i = _table[i];
					else
						i = 0;
				}
			}

			return -1;
		}

		private readonly IEqualityComparer<T> _comparer;
		private readonly IReadOnlyList<T> _pattern; // the (small) pattern to search for
		private readonly int[] _table; // lets the search function to skip ahead
	}
}