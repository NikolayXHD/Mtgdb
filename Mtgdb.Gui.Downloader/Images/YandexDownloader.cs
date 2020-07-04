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
			var client = new YandexDiskClientWrapper(_client, task.ImageSource.YandexKey, _syncOutput);

			string remotePath = string.Format(
				task.ImageSource.YandexDirPath, task.QualityGroup.YandexName, task.Dir.Subdir);
			bool success = await client.DownloadAndExtract(remotePath, task.TargetDirectory, task.Dir.Subdir.Concat(".7z"), token);

			if (success)
				ProgressChanged?.Invoke(task);

			return success;
		}

		public event Action<ImageDownloadProgress> ProgressChanged;

		private readonly object _syncOutput;
		private readonly YandexDiskClient _client;
	}
}
