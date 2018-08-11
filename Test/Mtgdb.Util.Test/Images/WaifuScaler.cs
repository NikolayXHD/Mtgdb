using System;
using System.Diagnostics;
using System.IO;

namespace Mtgdb.Util
{
	public static class WaifuScaler
	{
		public static void Scale(string sourceFile, string targetFile)
		{
			int parallelism = Environment.ProcessorCount;

			string exe = AppDir.Root.AddPath(@"..\tools\Waifu2x\waifu2x-converter-cpp.exe");
			string args = $"--jobs {parallelism} --mode scale -i \"{sourceFile}\" -o \"{targetFile}\"";
			string workingDirectory = Path.GetDirectoryName(exe);

			var process = Process.Start(new ProcessStartInfo(exe, args)
			{
				WorkingDirectory = workingDirectory,
				UseShellExecute = false,
				CreateNoWindow = true
			});

			if (process == null)
				throw new Exception("Failed to start waifu2x-converter.exe");

			if (!process.WaitForExit(180_000))
				throw new TimeoutException("waifu2x-converter.exe timeout");

			string outputFileNameDueToBug = targetFile + ".png";
			File.Move(outputFileNameDueToBug, targetFile);
		}
	}
}