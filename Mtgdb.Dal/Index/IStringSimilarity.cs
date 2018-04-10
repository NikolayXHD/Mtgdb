namespace Mtgdb.Dal.Index
{
	public interface IStringSimilarity
	{
		/// <summary>
		/// In fact it is not distance, it is similarity
		/// </summary>
		float GetSimilarity(string s1, string s2);
	}
}