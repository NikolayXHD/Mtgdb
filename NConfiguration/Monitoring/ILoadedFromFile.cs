
namespace NConfiguration.Monitoring
{
	/// <summary>
	/// This configuration is loaded from the file.
	/// The original file info can be used to monitor changes in this file.
	/// </summary>
	public interface ILoadedFromFile : IConfigNodeProvider
	{
		/// <summary>
		/// Information about the properties of the readed file
		/// </summary>
		ReadedFileInfo FileInfo { get; }
	}
}
