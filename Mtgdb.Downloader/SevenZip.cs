using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mtgdb.Downloader
{
	public class SevenZip
	{
		private Process _process;

		public bool Extract(string arhive, string targetDirectory, IEnumerable<string> excludedFiles)
		{
			if (_process != null)
				throw new InvalidOperationException("7za.exe is already running. Use another instance to start new download.");

			var argsBuilder = new StringBuilder($"x -aoa \"{arhive}\" \"-o{targetDirectory}\"");

			string executableName = AppDir.Update.AddPath(@"7z\7za.exe");

			foreach (string excludedFile in excludedFiles.Concat(Sequence.From(executableName)))
			{
				if (!excludedFile.StartsWith(targetDirectory))
					throw new ArgumentException($"excluded file {excludedFile} is not in target directory {targetDirectory}");

				var relativePath = excludedFile.Substring(targetDirectory.Length + 1);
				argsBuilder.Append($" \"-x!{relativePath}\"");
			}
			
			_process = new Process
			{
				StartInfo = new ProcessStartInfo(executableName, argsBuilder.ToString())
				{
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				},

				EnableRaisingEvents = true
			};

			_process.OutputDataReceived += downloadOutputReceived;
			_process.ErrorDataReceived += downloadErrorReceived;

			AppDomain.CurrentDomain.ProcessExit += processExit;
			_process.Start();
			_process.BeginOutputReadLine();
			_process.BeginErrorReadLine();
			_process.WaitForExit();

			var exitCode = _process.ExitCode;

			Abort();

			Console.WriteLine();
			return exitCode == 0;
		}

		private void processExit(object sender, EventArgs e)
		{
			Abort();
		}

		public void Abort()
		{
			if (_process == null)
				return;

			_process.OutputDataReceived -= downloadOutputReceived;
			_process.ErrorDataReceived -= downloadErrorReceived;

			if (!_process.HasExited)
				_process.Kill();

			AppDomain.CurrentDomain.ProcessExit -= processExit;

			_process = null;
		}

		private static void downloadErrorReceived(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
				return;

			Console.WriteLine(e.Data);
		}

		private static void downloadOutputReceived(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
				return;

			Console.WriteLine(e.Data);
		}
	}
}