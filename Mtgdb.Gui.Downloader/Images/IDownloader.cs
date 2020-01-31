using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb.Downloader
{
	internal interface IDownloader
	{
		bool CanDownload(ImageDownloadProgress task);

		Task<bool> Download(ImageDownloadProgress task, CancellationToken token);
	}
}