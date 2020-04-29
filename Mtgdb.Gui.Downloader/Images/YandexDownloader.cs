using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb.Downloader
{
	public class YandexDownloader : IDownloader
	{
		public YandexDownloader(object syncOutput, YandexDiskClient client)
		{
			_syncOutput = syncOutput;
			_client = client;
		}

		public bool CanDownload(ImageDownloadProgress task) =>
			task.QualityGroup.YandexName != null;

		public async Task<bool> Download(ImageDownloadProgress task, CancellationToken token)
		{
			lock (_syncOutput)
				Console.Write("{0}.7z: get YandexDisk download url  ... ", task.Dir.Subdir);

			string url = await _client.GetSubdirDownloadUrl(task.ImageSource, task.QualityGroup, task.Dir, token);

			lock (_syncOutput)
				Console.Write("downloading ... ");

			FsPath fileName = task.Dir.Subdir.Concat(".7z");
			FsPath targetDirectory = task.TargetDirectory;

			if (!await _client.DownloadAndExtract(url, targetDirectory, fileName, token))
				return false;

			Console.WriteLine("done");

			ProgressChanged?.Invoke(task);
			return true;
		}

		public event Action<ImageDownloadProgress> ProgressChanged;

		private readonly object _syncOutput;
		private readonly YandexDiskClient _client;
	}
}
