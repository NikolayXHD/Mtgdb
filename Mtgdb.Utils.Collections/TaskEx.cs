using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb
{
	public static class TaskEx
	{
		public static Task CatchCanceled(this Task original) =>
			original.Catch<TaskCanceledException>();

		public static async Task Catch<TException>(this Task original)
			where TException: Exception
		{
			try
			{
				await original;
			}
			catch (TException)
			{
			}
		}

		public static Task Run(this CancellationToken token, Func<CancellationToken, Task> task) =>
			Task.Run(async () => await task(token), token);

		public static Task Run(this CancellationToken token, Action<CancellationToken> task) =>
			Task.Run(() => task(token), token);

		public static IDeferredCallback When(this CancellationToken token, AsyncSignal signal) =>
			new DeferredCallback(token, signal);

		private class DeferredCallback : IDeferredCallback
		{
			private readonly CancellationToken _token;
			private readonly AsyncSignal _signal;

			public DeferredCallback(CancellationToken token, AsyncSignal signal)
			{
				_token = token;
				_signal = signal;
			}

			public Task Run(Action callback) =>
				Task.Run(async () =>
				{
					await _signal.Wait(_token);
					callback();
				}, _token);
		}
	}

	public interface IDeferredCallback
	{
		Task Run(Action action);
	}
}
