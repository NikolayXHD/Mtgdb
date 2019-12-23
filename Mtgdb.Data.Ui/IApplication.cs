using System.Threading;

namespace Mtgdb.Ui
{
	public interface IApplication
	{
		CancellationToken CancellationToken { get; }
	}
}
