
namespace NConfiguration
{
	/// <summary>
	/// store application settings
	/// </summary>
	public interface IIdentifiedSource : IConfigNodeProvider
	{
		/// <summary>
		/// source identifier the application settings
		/// </summary>
		string Identity { get; }
	}
}
