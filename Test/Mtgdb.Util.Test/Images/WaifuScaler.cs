using System;
using System.Diagnostics;
using System.IO;

namespace Mtgdb.ImageProcessing
{
	public static class WaifuScaler
	{
		public static void Scale(string sourceFile, string targetFile)
		{
			string exe = AppDir.Root.AddPath(@"..\tools\Waifu2x\waifu2x-converter.exe");
			string args = $"--jobs 2 --mode scale -i \"{sourceFile}\" -o \"{targetFile}\"";
			string workingDirectory = Path.GetDirectoryName(exe);

			var process = Process.Start(new ProcessStartInfo(exe, args)
			{
				WorkingDirectory = workingDirectory,
				UseShellExecute = false,
				CreateNoWindow = true
			});

			if (process == null)
				throw new Exception("Failed to start waifu2x-converter.exe");

			if (!process.WaitForExit(90000))
				throw new TimeoutException("waifu2x-converter.exe timeout");
		}
	}
}