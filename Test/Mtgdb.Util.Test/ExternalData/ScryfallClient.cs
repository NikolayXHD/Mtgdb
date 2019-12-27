using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Data;
using NLog;

namespace Mtgdb.Util
{
	public class ScryfallClient : ImageDownloaderBase
	{
		public override async Task DownloadCardImage(Card card, string targetPath, CancellationToken token)
		{
			if (string.IsNullOrEmpty(card.ScryfallId))
			{
				_log.Info("Emtpy scryfall id: {0}", card);
				return;
			}


			if (card.MultiverseId.HasValue)
			{
				string urlMultiverseId = "https://api.scryfall.com/cards/multiverse/" + card.MultiverseId.Value +
					"?format=image" +
					"&version=png";

				try
				{
					await DownloadFile(urlMultiverseId, targetPath, token);
					return;
				}
				catch (HttpRequestException ex)
				{
					_log.Info(ex,"Failed request to {0} {1}", urlMultiverseId);
				}
			}

			string urlScryfallId = "https://api.scryfall.com/cards/" +
				card.ScryfallId +
				"?format=image" +
				"&version=png";
			if (card.IsTransform() && card == card.Faces[1])
				urlScryfallId += "&face=back";

			try
			{
				await DownloadFile(urlScryfallId, targetPath, token);
			}
			catch (HttpRequestException ex)
			{
				_log.Info(ex, "Failed request to {0}", urlScryfallId);
			}
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
