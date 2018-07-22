using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Mtgdb;

namespace ScaleUtil
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 4 || !int.TryParse(args[3], out var processorCount))
			{
				Console.WriteLine("Usage:");
				string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
				Console.WriteLine($"	{name}.exe <path to waifu2x-converter.exe> <source directory> <target directory> <threads count>");
				return;
			}

			string waifuPath = args[0];
			string sourceDir = args[1];
			string targetDir = args[2];

			scaleDir(sourceDir, targetDir, waifuPath, processorCount);
		}

		private static void scaleDir(string sourceDir, string targetDir, string waifuPath, int processorCount)
		{
			Console.WriteLine("Source: " + sourceDir);
			Console.WriteLine("Target: " + targetDir);

			foreach (string sourceFile in Directory.GetFiles(sourceDir, "*.png", SearchOption.AllDirectories))
			{
				var targetFile = targetDir + sourceFile.Substring(sourceDir.Length);

				Console.WriteLine("	" + targetFile.Substring(targetDir.Length));

				Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

				if (File.Exists(targetFile))
					continue;

				scale(waifuPath, sourceFile, targetFile, processorCount);
			}
		}

		private static void scale(string waifuPath, string sourceFile, string targetFile, int parallelism)
		{
			string tempFile = makeWhiteBackground(sourceFile, targetFile);
			string args = $"--jobs {parallelism} --mode scale -i \"{tempFile}\" -o \"{targetFile}\"";
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
			{
				process.Kill();
				File.Delete(tempFile);
				throw new TimeoutException("waifu2x-converter.exe timeout");
			}

			File.Delete(tempFile);
		}

		private static string makeWhiteBackground(string sourceFile, string targetFile)
		{
			using (var original = new Bitmap(sourceFile))
			{
				new BmpAlphaToBackgroundColorTransformation(original, Color.White)
					.Execute();

				string targetDir = Path.GetDirectoryName(targetFile);
				string name = Path.GetFileNameWithoutExtension(targetFile);
				string extension = Path.GetExtension(targetFile);
				string tempFile = Path.Combine(targetDir, name + ".temp" + extension);

				if (File.Exists(tempFile))
					File.Delete(tempFile);

				original.Save(tempFile);

				return tempFile;
			}
		}
	}
}