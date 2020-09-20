using System;
using System.Diagnostics;

namespace Mtgdb.Downloader
{
	internal class UtilExe
	{
		public bool CreateShortcut(FsPath exePath, FsPath iconPath, FsPath shortcutPath) =>
			run($"-create_shortcut -exe_path \"{exePath}\" -ico_path \"{iconPath}\" -src_path \"{shortcutPath}\"");

		private bool run(string args)
		{
			if (_process != null)
				throw new InvalidOperationException($"{_executable} is already running. Use another instance.");

			_errorReceived = false;

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

			abort();
			Console.WriteLine();

			return !_errorReceived;
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

		private void errorReceived(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
				return;

			Console.WriteLine(e.Data);
			_errorReceived = true;
		}

		private void outputReceived(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
				return;

			Console.WriteLine(e.Data);
		}

		private static readonly FsPath _executable = new FsPath("Mtgdb.Util.Win.exe");
		private bool _errorReceived;
		private Process _process;
	}
}
