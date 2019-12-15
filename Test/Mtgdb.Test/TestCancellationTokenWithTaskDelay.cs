using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class TestTaskCancellation
	{
		[Test]
		public void When_delay_canceled_before_timeout_Then_exception_is_raised()
		{
			// this is why
			// await Task.Delay(int, CancellationToken)
			// is unusable
			var cts = new CancellationTokenSource();
			var delay = Task.Delay(100, cts.Token);

			Assert.ThrowsAsync<TaskCanceledException>(async () => await Task.WhenAll(delay, interrupt()));

			async Task interrupt()
			{
				// ReSharper disable once MethodSupportsCancellation
				await Task.Delay(50);
				cts.Cancel();
			}
		}

		[Test]
		public void When_delay_canceled_within_task_run_with_same_token_Then_exception_is_raised()
		{
			var cts = new CancellationTokenSource();
			var task = Task.Run(async () =>
			{
				await Task.Delay(100, cts.Token);
			}, cts.Token);


			Assert.ThrowsAsync<TaskCanceledException>(async () =>
			{
				await Task.WhenAll(task, interrupt());
			});

			async Task interrupt()
			{
				// ReSharper disable once MethodSupportsCancellation
				await Task.Delay(50);
				cts.Cancel();
			}
		}

		[Test]
		public async Task When_using_catch_with_delay_Then_exception_is_swallowed()
		{
			var cts = new CancellationTokenSource();
			var delay = Task.Delay(100, cts.Token);

			await Task.WhenAll(delay.Catch<TaskCanceledException>(), interrupt());
			Assert.That(delay.Status, Is.EqualTo(TaskStatus.Canceled));

			async Task interrupt()
			{
				// ReSharper disable once MethodSupportsCancellation
				await Task.Delay(50);
				cts.Cancel();
			}
		}

		[Test]
		public async Task When_canceling_task_run_Then_exception_is_not_raised()
		{
			var cts = new CancellationTokenSource();
			var task = Task.Run(async () =>
			{
				if (cts.Token.IsCancellationRequested)
					return;

				// ReSharper disable once MethodSupportsCancellation
				await Task.Delay(100);
			}, cts.Token);

			await Task.WhenAll(task, interrupt());
			Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));

			async Task interrupt()
			{
				// ReSharper disable once MethodSupportsCancellation
				await Task.Delay(50);
				cts.Cancel();
			}
		}

		[Test]
		public async Task When_using_catch_with_task_run_containing_delay_Then_exception_is_swallowed()
		{
			var cts = new CancellationTokenSource();
			var task = Task.Run(async () =>
			{
				await Task.Delay(100, cts.Token);
			}, cts.Token);

			await Task.WhenAll(task.Catch<TaskCanceledException>(), interrupt());
			Assert.That(task.Status, Is.EqualTo(TaskStatus.Canceled));

			async Task interrupt()
			{
				// ReSharper disable once MethodSupportsCancellation
				await Task.Delay(50);
				cts.Cancel();
			}
		}
	}
}
