using System.Threading;

namespace Mtgdb.Ui
{
	public interface IApplication
	{
		void Cancel();
		CancellationToken CancellationToken { get; }
	}
}