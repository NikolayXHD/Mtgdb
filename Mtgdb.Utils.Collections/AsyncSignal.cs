using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb
{
    public class AsyncSignal : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public AsyncSignal() =>
            _semaphore = new SemaphoreSlim(0, 1);

        public void Signal() =>
            _semaphore.Release();

        public bool Signaled => _semaphore.CurrentCount > 0;

        public async Task Wait(CancellationToken token)
        {
            await _semaphore.WaitAsync(token);
            _semaphore.Release();
        }

        public void Dispose() =>
            _semaphore.Dispose();
    }
}
