using System;
using System.Linq;
using Mtgdb.Dev;

namespace Mtgdb.Util
{
	public static class NewVersionNotifier
	{
		public static void Notify()
		{
			var lines = DevPaths.ReleaseNotesFile.ReadAllLines();

			int headersCount = 0;
			bool success = false;

			var lastVersionLines = lines.TakeWhile(l =>
			{
				if (l.Length > 0 && l.All(c => c == '-'))
					headersCount++;

				if (headersCount == 2)
					success = true;

				return headersCount < 2;
			}).ToList();

			if (!success || lastVersionLines.Count <= 2)
			{
				Console.WriteLine($"Failed to parse {DevPaths.ReleaseNotesFile}");
				Console.WriteLine();
				return;
			}

			var content = lastVersionLines.Take(lastVersionLines.Count - 2)
				.ToList();

			Console.WriteLine();
			Console.WriteLine(string.Join(Str.Endl, content));
			Console.WriteLine();

			DevPaths.NotificationsRepo.Join(lines[0] + ".txt").WriteAllLines(content);
		}
	}
}
