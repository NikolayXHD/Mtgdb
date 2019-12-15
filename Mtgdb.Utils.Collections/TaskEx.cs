using System;
using System.Threading.Tasks;

namespace Mtgdb
{
	public static class TaskEx
	{
		public static Task MayBeCanceled(this Task original) =>
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
	}
}
