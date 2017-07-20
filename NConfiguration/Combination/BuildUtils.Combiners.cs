namespace NConfiguration.Combination
{
	internal static partial class BuildUtils
	{
		public class GenericClassCombiner<T> : ICombiner<T> where T : class, ICombinable<T>
		{
			public T Combine(ICombiner combiner, T x, T y)
			{
				if (x == null)
					return y;
				x.Combine(combiner, y);
				return x;
			}
		}

		public class ClassCombiner<T> : ICombiner<T> where T : class, ICombinable
		{
			public T Combine(ICombiner combiner, T x, T y)
			{
				if (x == null)
					return y;
				x.Combine(combiner, y);
				return x;
			}
		}

		public class GenericStructCombiner<T> : ICombiner<T> where T : struct, ICombinable<T>
		{
			public T Combine(ICombiner combiner, T x, T y)
			{
				x.Combine(combiner, y);
				return x;
			}
		}

		public class StructCombiner<T> : ICombiner<T> where T : struct, ICombinable
		{
			public T Combine(ICombiner combiner, T x, T y)
			{
				x.Combine(combiner, y);
				return x;
			}
		}
	}
}
