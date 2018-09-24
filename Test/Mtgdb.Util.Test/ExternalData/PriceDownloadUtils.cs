using System.Linq;
using Mtgdb.Downloader;
using Mtgdb.Test;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class PriceDownloadUtils: TestsBase
	{
		[Test]
		public void DownloadPrices()
		{
			LoadCards();
			var downloader = Kernel.Get<PriceDownloader>();
			downloader.LoadPendingProgress();
			downloader.Download();
		}

		[TestCase("hou", "Adorned Pouncer")]
		public void GetPrice(string setCode, string name)
		{
			LoadCards();

			var card = Repo.SetsByCode[setCode].CardsByName[name].First();

			var client = new PriceClient();
			var sid = client.DownloadSid(card);

			Assert.That(sid, Is.Not.Null);

			var result = client.DownloadPrice(sid);

			Assert.That(result, Is.Not.Null);

			Assert.That(result.Mid, Is.Not.Null);
			Assert.That(result.Mid.Value, Is.GreaterThan(0));

			Assert.That(result.Low, Is.Not.Null);
			Assert.That(result.Low.Value, Is.GreaterThan(0));

			Assert.That(result.High, Is.Not.Null);
			Assert.That(result.High.Value, Is.GreaterThan(0));
		}
	}
}