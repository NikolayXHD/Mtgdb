using NConfiguration.Combination;
using NConfiguration.Serialization;

namespace NConfiguration
{
	/// <summary>
	/// Store application settings
	/// </summary>
	public interface IAppSettings : IConfigNodeProvider, IDeserializer, ICombiner
	{
	}
}
