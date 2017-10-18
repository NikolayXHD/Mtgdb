using System;
using System.Diagnostics;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Ninject;

namespace Mtgdb.Test
{
	internal static class TestLoader
	{
		private static readonly StandardKernel _kernel = new StandardKernel();

		public static LuceneSearcher Searcher { get; private set; }
		public static CardRepository CardRepository { get; private set; }
		public static ImageRepository ImageRepository { get; private set; }

		public static void LoadModules()
		{
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();

			CardRepository = _kernel.Get<CardRepository>();
			Searcher = _kernel.Get<LuceneSearcher>();
			ImageRepository = _kernel.Get<ImageRepository>();
		}

		public static void LoadSearcher()
		{
			if (!Searcher.IsUpToDate)
			{
				var sw = new Stopwatch();
				sw.Restart();
				Searcher.LoadIndex(CardRepository);
				sw.Stop();

				Console.WriteLine($"Index created in {sw.ElapsedMilliseconds} ms");
			}
			else
			{
				var sw = new Stopwatch();
				sw.Start();

				Searcher.LoadIndex(null);

				sw.Stop();
				Console.WriteLine($"Index created in {sw.ElapsedMilliseconds} ms");
			}
		}

		public static void LoadCardRepository()
		{
			var sw = new Stopwatch();
			sw.Start();

			CardRepository.LoadFile();
			CardRepository.Load();

			Console.WriteLine($"Cards repository loaded in {sw.ElapsedMilliseconds} ms");
			sw.Stop();
		}

		public static void LoadLocalizations()
		{
			var sw = new Stopwatch();
			sw.Start();

			var localizationRepository = _kernel.Get<LocalizationRepository>();
			localizationRepository.LoadFile();
			localizationRepository.Load();

			sw.Stop();
	
			Console.WriteLine($"Localization repository loaded in {sw.ElapsedMilliseconds} ms");

			CardRepository.FillLocalizations(localizationRepository);
		}
	}
}