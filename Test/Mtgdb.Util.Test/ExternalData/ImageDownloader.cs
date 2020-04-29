using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Data;
using Mtgdb.Downloader;

namespace Mtgdb.Util
{
	public abstract class ImageDownloaderBase : WebClientBase
	{
		public abstract Task DownloadCardImage(Card card, FsPath targetPath, CancellationToken token);
	}
}
