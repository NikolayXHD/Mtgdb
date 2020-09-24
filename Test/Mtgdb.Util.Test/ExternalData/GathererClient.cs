using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Data;
using NLog;

namespace Mtgdb.Util
{
	public class GathererClient : ImageDownloaderBase
	{
		public GathererClient()
		{
		}

		public override Task DownloadCardImage(Card card, FsPath targetFile, CancellationToken token)
		{
			if (card.MultiverseId.HasValue)
				return DownloadFile(BaseUrl + ImagePath + card.MultiverseId.Value, targetFile, token);

			_log.Info("Empty multiverse id: {0}", card);
			return Task.CompletedTask;
		}

		private const string BaseUrl = "http://gatherer.wizards.com/";
		private const string ImagePath = "Handlers/Image.ashx?type=card&multiverseid=";

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
