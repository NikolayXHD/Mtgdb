using System;
using System.Diagnostics;
using Mtgdb.Dev;

namespace Mtgdb.Util
{
	public static class WaifuScaler
	{
		public static void Scale(FsPath sourceFile, FsPath targetFile)
		{
			FsPath exe = Runtime.IsLinux
				? getLinuxExecutable()
				: getWindowsExecutable();
			string args = $"--jobs {Environment.ProcessorCount} --mode scale -i \"{sourceFile}\" -o \"{targetFile}\"";
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

		private static FsPath getWindowsExecutable() =>
			// download from https://yadi.sk/d/f1HuKUg7xW2FUQ/tools?w=1
			DevPaths.MtgToolsDir.Join("waifu2x-converter-cpp", "waifu2x-converter-cpp.exe");

		private static FsPath getLinuxExecutable() =>
			// build from source & install
			// https://github.com/DeadSix27/waifu2x-converter-cpp/blob/master/BUILDING.md
			//
			// sudo apt install libopencv-dev
			// sudo apt install ocl-icd-opencl-dev # maybe redundant
			// sudo apt install nvidia-cuda-toolkit
			//
			// git clone "https://github.com/DeadSix27/waifu2x-converter-cpp"
			// cd waifu2x-converter-cpp
			// mkdir out && cd out
			// cmake ..
			// make -j4
			// sudo make install
			//
			// sudo ldconfig
			// waifu2x-converter-cpp --help
			new FsPath("waifu2x-converter-cpp");
	}
}
