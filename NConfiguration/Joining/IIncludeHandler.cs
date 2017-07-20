using System.Collections.Generic;

namespace NConfiguration.Joining
{
	public interface IIncludeHandler<T>
	{
		IEnumerable<IIdentifiedSource> TryLoad(IConfigNodeProvider owner, T includeConfig, string searchPath);
	}
}
