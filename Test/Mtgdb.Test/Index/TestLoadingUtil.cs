using System;
using System.Diagnostics;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Ninject;

namespace Mtgdb.Test
{
	internal static class TestLoadingUtil
	{
		private static StandardKernel _kernel = new StandardKernel();
		public static LuceneSearcher Searcher { get; private set; }
		public static CardRepository CardRepository { get; private set; }

		public static void LoadModules()
		{
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();

			CardRepository = _kernel.Get<CardRepository>();
			Searcher = _kernel.Get<LuceneSearcher>();
		}

		public static void LoadSearcher()
		{
			if (!Searcher.IsUpToDate)
			{
				LoadCardRepository();

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
			if (CardRepository.IsLoadingComplete)
				return;

			var sw = new Stopwatch();
			sw.Start();

			var localizationRepository = _kernel.Get<LocalizationRepository>();

			CardRepository.LoadFile();
			CardRepository.Load();

			Console.WriteLine($"Cards repository loaded in {sw.ElapsedMilliseconds} ms");

			sw.Restart();
			localizationRepository.LoadFile();
			localizationRepository.Load();

			Console.WriteLine($"Localization repository loaded in {sw.ElapsedMilliseconds} ms");

			CardRepository.FillLocalizations(localizationRepository);
		}
	}
}