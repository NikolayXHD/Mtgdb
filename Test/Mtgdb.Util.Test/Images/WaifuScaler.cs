using System;
using System.Diagnostics;
using Mtgdb.Dev;

namespace Mtgdb.Util
{
	public static class WaifuScaler
	{
		public static void Scale(FsPath sourceFile, FsPath targetFile)
		{
			int parallelism = Environment.ProcessorCount;

			// download from https://yadi.sk/d/f1HuKUg7xW2FUQ/tools?w=1
			FsPath exe = DevPaths.MtgToolsDir.Join("waifu2x-converter-cpp", "waifu2x-converter-cpp.exe");
			string args = $"--jobs {parallelism} --mode scale -i \"{sourceFile}\" -o \"{targetFile}\"";
			FsPath workingDirectory = exe.Parent();

			var process = Process.Start(new ProcessStartInfo(exe.Value, args)
			{
				WorkingDirectory = workingDirectory.Value,
				UseShellExecute = false,
				CreateNoWindow = true
			});

			if (process == null)
				throw new Exception("Failed to start waifu2x-converter.exe");

			if (!process.WaitForExit(180_000))
				throw new TimeoutException("waifu2x-converter.exe timeout");
		}
	}
}
