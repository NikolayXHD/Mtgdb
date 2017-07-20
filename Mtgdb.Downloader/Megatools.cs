using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Mtgdb.Downloader
{
	public class Megatools
	{
		private Process _process;

		public void Download(string name, string storageUrl, string targetDirectory)
		{
			if (_process != null)
				throw new InvalidOperationException("Download is already running. Use another instance to start new download");

			var megadlExePath = Path.Combine(AppDir.Update, @"megatools-1.9.98-win32\megadl.exe");

			string arguments;

			if (targetDirectory.Contains(' '))
				arguments = $@"--path=""{targetDirectory}"" --print-names {storageUrl}";
			else
				arguments = $@"--path={targetDirectory} --print-names {storageUrl}";

			_process = new Process
			{
				StartInfo = new ProcessStartInfo(megadlExePath, arguments)
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
			
			Console.WriteLine("Downloading {0} from {1} to {2}", name, storageUrl, targetDirectory);

			AppDomain.CurrentDomain.ProcessExit += processExit;
			_process.Start();
			_process.BeginOutputReadLine();
			_process.BeginErrorReadLine();
			_process.WaitForExit();

			Abort();
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

			Console.WriteLine(e.Data.Replace("ERROR: File already exists at ", "[Skip] "));
		}

		private static void downloadOutputReceived(object sender, DataReceivedEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Data))
				return;

			if (e.Data.StartsWith("F "))
				return;

			Console.WriteLine(e.Data);
		}
	}
}