using System.Threading;

namespace Mtgdb.Data
{
	public interface IApplication
	{
		CancellationToken CancellationToken { get; }
	}
}
