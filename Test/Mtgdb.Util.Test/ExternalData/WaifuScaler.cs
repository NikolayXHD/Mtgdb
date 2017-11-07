﻿using System;
using System.Diagnostics;

namespace Mtgdb.ImageProcessing
{
	public static class WaifuScaler
	{
		public static void Scale(string sourceFile, string targetFile)
		{
			string exe = AppDir.Root.AddPath(@"..\..\..\Test\Waifu2x\waifu2x-converter.exe");
			string args = $"--jobs 2 --mode scale -i \"{sourceFile}\" -o \"{targetFile}\"";

			var process = Process.Start(new ProcessStartInfo(exe, args)
			{
				WorkingDirectory = AppDir.GetBinPath(@"Waifu2x"),
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