using System.Diagnostics;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Ninject;
using NLog;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class TestsBase
	{
		protected void LoadModules()
		{
			Kernel = new StandardKernel();
			Kernel.Load<CoreModule>();
			Kernel.Load<DalModule>();
			Kernel.Load<DownloaderModule>();

			Repo = Kernel.Get<CardRepository>();
			ImgRepo = Kernel.Get<ImageRepository>();
			PriceRepo = Kernel.Get<PriceRepository>();
		}

		protected void LoadCards()
		{
			var sw = new Stopwatch();
			sw.Start();

			Repo.LoadFile();
			Repo.Load();

			sw.Stop();
			Log.Debug($"Cards loaded in {sw.ElapsedMilliseconds} ms");
		}

		protected void LoadTranslations()
		{
			var sw = new Stopwatch();
			sw.Start();

			var locRepo = Kernel.Get<LocalizationRepository>();
			locRepo.LoadFile();
			locRepo.Load();

			Repo.FillLocalizations(locRepo);

			sw.Stop();
			Log.Debug($"Translations loaded in {sw.ElapsedMilliseconds} ms");
		}

		protected void LoadPrices()
		{
			var sw = new Stopwatch();
			sw.Start();

			PriceRepo.Load();
			Repo.SetPrices(PriceRepo);

			sw.Stop();
			Log.Debug($"Prices loaded in {sw.ElapsedMilliseconds} ms");
		}

		[TearDown]
		protected void TearDown()
		{
			LogManager.Flush();
		}

		protected IKernel Kernel;
		protected CardRepository Repo;
		protected ImageRepository ImgRepo;
		protected PriceRepository PriceRepo;

		protected static readonly Logger Log = LogManager.GetCurrentClassLogger();
	}
}