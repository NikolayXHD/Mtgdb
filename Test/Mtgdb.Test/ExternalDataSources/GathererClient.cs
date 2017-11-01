using System.IO;
using Mtgdb.Dal;
using Mtgdb.Downloader;

namespace Mtgdb.Test
{
	public class GathererClient : WebClientBase
	{
		public void DownloadCard(Card card, string targetDirectory)
		{
			var setDirectory = Path.Combine(targetDirectory, card.SetCode);
			Directory.CreateDirectory(setDirectory);

			if (card.MultiverseId == null)
				return;

			DownloadFile(
				BaseUrl + card.MultiverseId,
				Path.Combine(setDirectory, card.ImageName + ".png"));
		}

		private const string BaseUrl = "http://gatherer.wizards.com/Handlers/Image.ashx?type=card&multiverseid=";
	}
}