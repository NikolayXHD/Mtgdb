using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mtgdb
{
	public class SevenZip
	{
		public SevenZip(bool silent)
		{
			_silent = silent;
		}

		private Process _process;

		public bool Extract(FsPath archive, FsPath targetDirectory, IEnumerable<FsPath> excludedFiles = null)
		{
			excludedFiles ??= Enumerable.Empty<FsPath>();
			var argsBuilder = new StringBuilder($"x -aoa \"{archive}\" \"-o{targetDirectory}\"");

			foreach (FsPath excludedFile in excludedFiles.Append(_executable))
			{
				var relativePath = excludedFile.RelativeTo(targetDirectory);
				if (relativePath == FsPath.None)
					continue;

				argsBuilder.Append($" \"-x!{relativePath}\"");
			}

			string args = argsBuilder.ToString();

			return run(args);
		}

		public bool Compress(FsPath path, FsPath output)
		{
			return run($"a \"{output}\" -t7z \"{path}\" -r");
		}

		private bool run(string args)
		{
			if (_process != null)
				throw new InvalidOperationException("7za.exe is already running. Use another instance.");

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

			var exitCode = _process.ExitCode;

			abort();

			if (!_silent)
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
			if (_silent)
				return;

			if (string.IsNullOrEmpty(e.Data))
				return;

			Console.WriteLine(e.Data);
		}

		private readonly bool _silent;

		private static readonly FsPath _executable = Runtime.IsLinux
			? new FsPath("7z") // from p7zip-full package
			: AppDir.Update.Join("7z", "7za.exe");
	}
}
