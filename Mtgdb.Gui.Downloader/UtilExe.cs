using System;
using System.Diagnostics;

namespace Mtgdb.Downloader
{
	internal class UtilExe
	{
		public bool CreateShortcut(FsPath exePath, FsPath shortcutPath, FsPath iconPath) =>
			run($"-create_shortcut -source_path \"{shortcutPath}\" -target_path \"{exePath}\" -icon_path \"{iconPath}\"");

		private bool run(string args)
		{
			if (_process != null)
				throw new InvalidOperationException($"{_executable} is already running. Use another instance.");

			_process = new Process
			{
				StartInfo = new ProcessStartInfo(_executable.ToString(), args)
				{
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				},

				EnableRaisingEvents = true
			};

			_process.OutputDataReceived += outputReceived;
			_process.ErrorDataReceived += errorReceived;
			AppDomain.CurrentDomain.ProcessExit += processExit;

			_process.Start();
			_process.BeginOutputReadLine();
			_process.BeginErrorReadLine();
			_process.WaitForExit();

			int exitCode = _process.ExitCode;

			abort();

			Console.WriteLine();

			return exitCode == 0;
		}

		private void processExit(object sender, EventArgs e)
		{
			abort();
		}

		private void abort()
		{
			if (_process == null)
				return;

			_process.OutputDataReceived -= outputReceived;
			_process.ErrorDataReceived -= errorReceived;

			if (!_process.HasExited)
				_process.Kill();

			AppDomain.CurrentDomain.ProcessExit -= processExit;

			_process = null;
		}

		private static void errorReceived(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
				return;

			Console.WriteLine(e.Data);
		}

		private void outputReceived(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
				return;

			Console.WriteLine(e.Data);
		}

		private static readonly FsPath _executable = new FsPath("Mtgdb.Util.exe");

		private Process _process;
	}
}
