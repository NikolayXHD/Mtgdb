using System;
using System.Diagnostics;

namespace Mtgdb.Downloader
{
	internal class UtilExe
	{
		public bool CreateShortcut(FsPath exePath, FsPath iconPath, FsPath shortcutPath)
		{
			bool useBackup = false;
			FsPath backupPath = FsPath.None;
			if (shortcutPath.IsFile())
			{
				backupPath = shortcutPath.WithName(_ => _ + ".bak");
				useBackup = !backupPath.IsFile();
				if (useBackup)
					shortcutPath.MoveFileTo(backupPath);
				else
					shortcutPath.DeleteFile();
			}

			bool success = run($"\"{_script}\" \"{shortcutPath}\" \"{exePath}\" \"{iconPath}\"");

			if (!success && useBackup)
				backupPath.MoveFileTo(shortcutPath);

			return success;
		}

		private bool run(string args)
		{
			if (_process != null)
				throw new InvalidOperationException($"{_script} is already running. Use another instance.");

			if (!_script.IsFile())
			{
				Console.Write(_script + " not found");
				return false;
			}

			_errorReceived = false;

			_process = new Process
			{
				StartInfo = new ProcessStartInfo("cscript", args)
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

		private static readonly FsPath _script = AppDir.Update.Join("shortcut.vbs");
		private bool _errorReceived;
		private Process _process;
	}
}
