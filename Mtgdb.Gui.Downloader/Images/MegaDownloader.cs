using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb.Downloader
{
	public class MegaDownloader : IDownloader
	{
		public MegaDownloader(Megatools megatools, object syncOutput)
		{
			_megatools = megatools;
			_syncOutput = syncOutput;
		}

		public bool CanDownload(ImageDownloadProgress task) =>
			task.MegaUrl != null;

		public async Task<bool> Download(ImageDownloadProgress task, CancellationToken token)
		{
			lock (_syncOutput)
				Console.WriteLine($"Downloading {task.Dir.Subdir} from {task.MegaUrl}");

			await _megatools.Download(
				task.MegaUrl,
				task.TargetSubdirectory,
				name: task.Dir.Subdir.Value,
				silent: true,
				token: token);

			return true;
		}

		private readonly Megatools _megatools;
		private readonly object _syncOutput;
	}
}
