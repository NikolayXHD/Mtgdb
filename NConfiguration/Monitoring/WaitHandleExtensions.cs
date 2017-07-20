using System;
using System.Threading;
using System.Threading.Tasks;

namespace NConfiguration.Monitoring
{
	internal static class WaitHandleExtensions
	{
		public static void ThrowUnhandledException<TTask>(this TTask task, string errorMessage) where TTask : Task
		{
			task.ContinueWith(
				t => ThreadPool.QueueUserWorkItem(throwWork, new Exception(errorMessage, t.Exception.InnerException)),
				TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
		}

		private static void throwWork(object arg)
		{
			throw (Exception) arg;
		}

		public static Task<bool> AsTask(this WaitHandle wait, TimeSpan timeout)
		{
			return new HandleContext(wait, timeout).Task;
		}

		private class HandleContext
		{
			private readonly TaskCompletionSource<bool> _tcs = new TaskCompletionSource<bool>();
			private volatile bool _raized = false;
			private readonly RegisteredWaitHandle _handle = null;

			public HandleContext(WaitHandle wait, TimeSpan timeout)
			{
				_handle = ThreadPool.RegisterWaitForSingleObject(wait, Callback, null, timeout, executeOnlyOnce: true);

				if (_raized)
					_handle.Unregister(null);
			}

			private void Callback(object state, bool timedOut)
			{
				_raized = true;
				_tcs.TrySetResult(timedOut);
				if (_handle != null)
					_handle.Unregister(null);
			}

			public Task<bool> Task
			{
				get
				{
					return _tcs.Task;
				}
			}
		}
	}
}
