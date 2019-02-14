using System.Threading;

namespace Mtgdb.Ui
{
	public interface IApplication
	{
		void CancelAllTasks();
		CancellationToken CancellationToken { get; }
	}
}