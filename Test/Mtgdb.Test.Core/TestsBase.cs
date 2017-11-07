using System;
using System.Diagnostics;
using Mtgdb.Dal;
using Ninject;

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
		}

		protected void LoadCards()
		{
			var sw = new Stopwatch();
			sw.Start();

			Repo.LoadFile();
			Repo.Load();

			sw.Stop();
			Console.WriteLine($"Cards loaded in {sw.ElapsedMilliseconds} ms");
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
			Console.WriteLine($"Translations loaded in {sw.ElapsedMilliseconds} ms");
		}



		protected IKernel Kernel;
		protected CardRepository Repo;
		protected ImageRepository ImgRepo;
	}
}