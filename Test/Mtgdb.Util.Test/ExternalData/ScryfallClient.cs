using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Data;
using NLog;

namespace Mtgdb.Util
{
	public class ScryfallClient : ImageDownloaderBase
	{
		public bool Jpg
		{
			get;
			set;
		}

		public override async Task DownloadCardImage(Card card, FsPath targetPath, CancellationToken token)
		{
			if (string.IsNullOrEmpty(card.Identifiers.ScryfallId))
			{
				_log.Info("Emtpy scryfall id: {0}", card);
				return;
			}

			string format = Jpg ? "normal" : "png";
			if (card.MultiverseId.HasValue)
			{
				string urlMultiverseId = "https://api.scryfall.com/cards/multiverse/" + card.MultiverseId.Value +
					"?format=image&version=" + format;

				await downloadImage(urlMultiverseId, targetPath, token);
				if (targetPath.IsFile())
					return;
			}

			string urlScryfallId = "https://api.scryfall.com/cards/" +
				card.Identifiers.ScryfallId + "?format=image&version=" + format;
			if (Str.Equals(card.Side, CardSides.B))
				urlScryfallId += "&face=back";

			await downloadImage(urlScryfallId, targetPath, token);
		}

		private async Task downloadImage(string url, FsPath targetPath, CancellationToken token)
		{
			try
			{
				var stream = await DownloadStream(url, token);
				convertToPng(stream, targetPath);
			}
			catch (HttpRequestException ex)
			{
				_log.Info(ex,"Failed request to {0} {1}", url);
			}
		}

		private static void convertToPng(Stream jpgImage, FsPath targetPath)
		{
			var bmp = new Bitmap(jpgImage);
			var converted = new Bitmap(265, 370);
			var scaler = new BmpScaler(converted, bmp, new Rectangle(default, bmp.Size));
			scaler.Execute();
			var removeCorner = new BmpCornerRemoval(converted, force: true);
			removeCorner.Execute();
			converted.Save(targetPath.Value, ImageFormat.Png);
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
