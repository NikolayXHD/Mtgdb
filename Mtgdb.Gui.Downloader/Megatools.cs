using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Mtgdb.Downloader
{
	public class Megatools
	{
		[UsedImplicitly] // by ninject
		public Megatools(AppSourceConfig config)
		{
			_megatoolsUrl = config.MegatoolsUrl;
		}

		public async Task Download(string storageUrl, string targetDirectory, string name, CancellationToken token, bool silent = false, int? timeoutSec = null)
		{
			if (_process != null)
				throw new InvalidOperationException("Download is already running. Use another instance to start new download.");

			_silent = silent;

			await _syncSelfDownload.WaitAsync(token);
			try
			{
				if (!File.Exists(MegadlExePath))
				{
					var webClient = new GdriveWebClient();
					await webClient.DownloadAndExtract(_megatoolsUrl, AppDir.Update, "megatools.7z", token);
				}
			}
			finally
			{
				_syncSelfDownload.Release();
			}

			DownloadedCount = 0;

			string arguments;

			Directory.CreateDirectory(targetDirectory);
			if (targetDirectory.Contains(' '))
				arguments = $@"--path=""{targetDirectory}"" --print-names {storageUrl}";
			else
				arguments = $@"--path={targetDirectory} --print-names {storageUrl}";

			_process = new Process
			{
				StartInfo = new ProcessStartInfo(MegadlExePath, arguments)
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

			if (!_silent)
				Console.WriteLine("Downloading {0} from {1} to {2}", name, storageUrl, targetDirectory);

			AppDomain.CurrentDomain.ProcessExit += processExit;
			_process.Start();
			_process.BeginOutputReadLine();
			_process.BeginErrorReadLine();

			if (timeoutSec.HasValue)
				_process.WaitForExit(timeoutSec.Value * 1000);
			else
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

			if (e.Data.StartsWith("ERROR: File already exists at "))
				return;

			Console.WriteLine(e.Data);
		}

		private void downloadOutputReceived(object sender, DataReceivedEventArgs e)
		{
			DownloadedCount++;
			FileDownloaded?.Invoke();

			if (_silent)
				return;

			if (string.IsNullOrEmpty(e.Data))
				return;

			if (e.Data.StartsWith("F "))
				return;

			if (e.Data.StartsWith("D "))
				return;

			Console.WriteLine(e.Data);
		}



		private static string MegadlExePath =>
			AppDir.Update.AddPath(@"megatools-1.9.98-win32\megadl.exe");

		public int DownloadedCount { get; private set; }

		public event Action FileDownloaded;

		private Process _process;
		private bool _silent;
		private readonly string _megatoolsUrl;
		private static readonly SemaphoreSlim _syncSelfDownload = new SemaphoreSlim(1);
	}
}
