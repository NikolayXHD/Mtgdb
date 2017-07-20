using System.Collections.Generic;
using System.Linq;

namespace NConfiguration.Combination.Collections
{
	public class Union<T> :
		ICombiner<T[]>,
		ICombiner<IList<T>>,
		ICombiner<List<T>>,
		ICombiner<IEnumerable<T>>,
		ICombiner<ICollection<T>>
	{
		public T[] Combine(ICombiner combiner, T[] x, T[] y)
		{
			if (y == null || y.Length == 0)
				return x;

			if (x == null || x.Length == 0)
				return y;

			return x.Union(y).ToArray();
		}

		public IList<T> Combine(ICombiner combiner, IList<T> x, IList<T> y)
		{
			if (y == null || y.Count == 0)
				return x;

			if (x == null || x.Count == 0)
				return y;

			return x.Union(y).ToList();
		}

		public IEnumerable<T> Combine(ICombiner combiner, IEnumerable<T> x, IEnumerable<T> y)
		{
			if (x != null)
				foreach (var item in x)
					yield return item;

			if (y != null)
				foreach (var item in y)
					yield return item;
		}

		public List<T> Combine(ICombiner combiner, List<T> x, List<T> y)
		{
			if (y == null || y.Count == 0)
				return x;

			if (x == null || x.Count == 0)
				return y;

			return x.Union(y).ToList();
		}

		public ICollection<T> Combine(ICombiner combiner, ICollection<T> x, ICollection<T> y)
		{
			if (y == null || y.Count == 0)
				return x;

			if (x == null || x.Count == 0)
				return y;

			return x.Union(y).ToList();
		}
	}
}
