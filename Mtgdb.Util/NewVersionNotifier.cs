using System;
using System.IO;
using System.Linq;

namespace Mtgdb.Util
{
	public class NewVersionNotifier
	{
		public static void Notify()
		{
			string releaseNotesFile = @"F:\Repo\Git\Mtgdb.wiki\Release-notes.rest";
			var lines = File.ReadAllLines(releaseNotesFile);

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
				Console.WriteLine($"Failed to parse {releaseNotesFile}");
				Console.WriteLine();
				return;
			}

			var content = lastVersionLines.Take(lastVersionLines.Count - 2)
				.ToList();

			Console.WriteLine();
			Console.WriteLine(string.Join(Environment.NewLine, content));
			Console.WriteLine();

			File.WriteAllLines(Path.Combine(@"f:\Repo\Git\Mtgdb.Notifications", lines[0] + ".txt"), content);
		}
	}
}