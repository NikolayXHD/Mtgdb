using System.Collections.Generic;
using System.Linq;
using Mtgdb.Data;

namespace Mtgdb.Downloader
{
	internal class ImageDownloadQueue
	{
		public ImageDownloadQueue(CardRepository repo, List<IDownloader> downloaders, IEnumerable<ImageDownloadProgress> tasks)
		{
			_repo = repo;
			_queueByDownloader = downloaders.ToDictionary(_ => _, _ => new HashSet<ImageDownloadProgress>());
			_failedByDownloader = downloaders.ToDictionary(_ => _, _ => new HashSet<ImageDownloadProgress>());
			_commonQueue = new HashSet<ImageDownloadProgress>();

			foreach (var task in tasks)
			foreach (var (downloader, queue) in _queueByDownloader)
			{
				_commonQueue.Add(task);
				if (downloader.CanDownload(task))
					queue.Add(task);
			}
		}

		private string getReleaseDate(ImageDownloadProgress progress)
		{
			if (!_repo.IsLoadingComplete.Signaled)
				return null;

			return _repo.SetsByCode.TryGet(progress.Dir.Subdir)?.ReleaseDate;
		}

		public ImageDownloadProgress PopTaskFor(IDownloader downloader)
		{
			var queue = _queueByDownloader[downloader];
			ImageDownloadProgress max;
			lock (_sync)
			{
				max = queue.AtMax(getReleaseDate).FindOrDefault();
				if (max == null)
					return null;

				foreach (var q in _queueByDownloader.Values)
					q.Remove(max);
				_commonQueue.Remove(max);
			}

			return max;
		}

		public bool PushFailedTaskBack(IDownloader downloader, ImageDownloadProgress task)
		{
			bool success = false;
			lock (_sync)
			{
				_failedByDownloader[downloader].Add(task);
				foreach (var (dr, queue) in _queueByDownloader)
					if (!_failedByDownloader[dr].Contains(task))
					{
						success = true;
						queue.Add(task);
					}
			}

			return success;
		}

		public int Count
		{
			get
			{
				lock (_sync)
					return _commonQueue.Count;
			}
		}

		public int TotalOnlineFilesCount
		{
			get
			{
				lock (_sync)
					return _commonQueue.Sum(_ => _.FilesOnline.Count);
			}
		}

		private readonly CardRepository _repo;
		private readonly IDictionary<IDownloader, HashSet<ImageDownloadProgress>> _queueByDownloader;
		private readonly IDictionary<IDownloader, HashSet<ImageDownloadProgress>> _failedByDownloader;
		private readonly HashSet<ImageDownloadProgress> _commonQueue;
		private readonly object _sync = new object();
	}
}
