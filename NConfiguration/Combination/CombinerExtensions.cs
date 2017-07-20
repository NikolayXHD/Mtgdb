namespace NConfiguration.Combination
{
	public static class CombinerExtensions
	{
		public static T Combine<T>(this ICombiner combiner, T x, T y)
		{
			return combiner.Combine<T>(combiner, x, y);
		}
	}
}
