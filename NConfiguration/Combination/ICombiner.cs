namespace NConfiguration.Combination
{
	public interface ICombiner
	{
		T Combine<T>(ICombiner combiner, T x, T y);
	}
}
