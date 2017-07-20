using System;
using System.Threading;
using System.Threading.Tasks;

namespace NConfiguration.Monitoring
{
	public class PeriodicFileChecker : FileChecker
	{
		private readonly TimeSpan _delay;
		private readonly CheckMode _checkMode;
		private readonly CancellationTokenSource _cts;

		public PeriodicFileChecker(ReadedFileInfo fileInfo, TimeSpan delay, CheckMode checkMode)
			: base(fileInfo)
		{
			if (delay <= TimeSpan.FromMilliseconds(1))
				throw new ArgumentOutOfRangeException("delay should be greater of 1 ms");

			_delay = delay;
			_checkMode = checkMode;
			_cts = new CancellationTokenSource();
			Task.Factory.StartNew(checkLoop).ThrowUnhandledException("Error while file checking.");
		}

		private void checkLoop()
		{
			do
			{
				try
				{
					Thread.Sleep(_delay);
				}
				catch (OperationCanceledException)
				{
					return;
				}
			} while (!checkFile(_checkMode));

			onChanged();
		}

		public override void Dispose()
		{
			_cts.Cancel();
			base.Dispose();
		}
	}
}
