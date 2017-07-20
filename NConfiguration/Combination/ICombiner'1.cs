namespace NConfiguration.Combination
{
	public interface ICombiner<T>
	{
		T Combine(ICombiner combiner, T x, T y);
	}
}
