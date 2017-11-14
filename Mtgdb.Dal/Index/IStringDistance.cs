namespace Mtgdb.Dal.Index
{
	public interface IStringDistance
	{
		/// <summary>
		/// In fact it is not distance, it is similarity
		/// </summary>
		float GetDistance(string s1, string s2);
	}
}