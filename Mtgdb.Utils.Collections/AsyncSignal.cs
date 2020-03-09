using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb
{
	public class AsyncSignal : IDisposable
	{
		private readonly SemaphoreSlim _semaphore;

		public bool Signaled { get; private set; }

		public AsyncSignal(bool multiple = false) =>
			_semaphore = new SemaphoreSlim(0, multiple ? int.MaxValue : 1);

		public void Signal()
		{
			_semaphore.Release();
			Signaled = true;
		}

		public async Task Wait(CancellationToken token)
		{
			await _semaphore.WaitAsync(token);
			_semaphore.Release();
		}

		public void Dispose() =>
			_semaphore.Dispose();
	}
}
