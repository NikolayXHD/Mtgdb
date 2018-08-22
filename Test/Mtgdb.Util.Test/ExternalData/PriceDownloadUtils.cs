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

		[TestCase("hou", "158")]
		public void GetPrice(string setCode, string number)
		{
			var client = new PriceClient();
			var sid = client.DownloadSid(setCode, number);

			Assert.That(sid, Is.Not.Null);

			var result = client.DownloadPrice(sid);

			Assert.That(result, Is.Not.Null);

			Assert.That(result.Mid, Is.Not.Null);
			Assert.That(result.Mid.Value, Is.GreaterThan(0));
		}
	}
}