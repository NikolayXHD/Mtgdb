using System.Diagnostics;
using Mtgdb.Dal;
using Ninject;
using NLog;

namespace Mtgdb.Test
{
	public class TestsBase
	{
		protected void LoadModules()
		{
			Kernel = new StandardKernel();
			Kernel.Load<CoreModule>();
			Kernel.Load<DalModule>();

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
			Log.Info($"Cards loaded in {sw.ElapsedMilliseconds} ms");
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
			Log.Info($"Translations loaded in {sw.ElapsedMilliseconds} ms");
		}

		protected void LoadPrices()
		{
			var sw = new Stopwatch();
			sw.Start();

			PriceRepo.Load();
			Repo.SetPrices(PriceRepo);

			sw.Stop();
			Log.Info($"Prices loaded in {sw.ElapsedMilliseconds} ms");
		}


		protected IKernel Kernel;
		protected CardRepository Repo;
		protected ImageRepository ImgRepo;
		protected PriceRepository PriceRepo;

		protected static readonly Logger Log = LogManager.GetCurrentClassLogger();
	}
}