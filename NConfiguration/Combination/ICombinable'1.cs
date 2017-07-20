namespace NConfiguration.Combination
{
	/// <summary>
	/// object implementing this interface can be reduced at boot
	/// </summary>
	public interface ICombinable<in T>
	{
		/// <summary>
		/// combine by other instance
		/// </summary>
		/// <param name="combiner"></param>
		/// <param name="other">The object to combine with the current object.</param>
		void Combine(ICombiner combiner, T other);
	}
}
