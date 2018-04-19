using System;
using System.Diagnostics;
using System.IO;

namespace ScaleUtil
{
	class Program
	{
		static void Main(string[] args)
		{
			var waifuPath = args[0];
			var sourceDir = args[1];
			var targetDir = args[2];
			int processorCount = int.Parse(args[3]);

			foreach (string sourceFile in Directory.GetFiles(sourceDir, "*.png", SearchOption.AllDirectories))
			{
				var targetFile = targetDir + sourceFile.Substring(sourceDir.Length);

				if (File.Exists(targetFile))
					continue;

				scale(waifuPath, sourceFile, targetFile, processorCount);
			}
		}

		private static void scale(string waifuPath, string sourceFile, string targetFile, int parallelism)
		{
			string args = $"--jobs {parallelism} --mode scale -i \"{sourceFile}\" -o \"{targetFile}\"";
			string workingDirectory = Path.GetDirectoryName(waifuPath);

			var process = Process.Start(new ProcessStartInfo(waifuPath, args)
			{
				WorkingDirectory = workingDirectory,
				UseShellExecute = false,
				CreateNoWindow = true
			});

			if (process == null)
				throw new Exception("Failed to start waifu2x-converter.exe");

			bool success = process.WaitForExit(600_000);
			
			if (!success)
				throw new TimeoutException("waifu2x-converter.exe timeout");
		}
	}
}
