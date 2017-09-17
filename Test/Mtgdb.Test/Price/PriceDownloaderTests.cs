using Mtgdb.Dal;
using Mtgdb.Downloader;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class PriceDownloaderTests
	{
		private readonly IKernel _kernel = new StandardKernel();

		[OneTimeSetUp]
		public void Setup()
		{
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();
		}

		[Test]
		public void DownloadPrices()
		{
			var repo = _kernel.Get<CardRepository>();
			var priceRepo = _kernel.Get<DownloaderPriceRepository>();

			repo.LoadFile();
			repo.Load();

			var downloader = new PriceDownloader(repo, priceRepo);
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
