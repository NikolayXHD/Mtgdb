using System.Configuration;

namespace NConfiguration.Xml.Protected
{
	public interface IProviderCollection
	{
		ProtectedConfigurationProvider Get(string name);
		void Set(string name, ProtectedConfigurationProvider provider);
		void Clear();
		bool Remove(string name);
	}
}

