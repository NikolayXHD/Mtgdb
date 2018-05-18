using System.Diagnostics;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Downloader;
using Mtgdb.Gui;
using Ninject;
using NLog;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class TestsBase
	{
		[SetUp]
		public void BaseSetup()
		{
			ApplicationCulture.SetCulture(Str.Culture);
		}

		[TearDown]
		public void TearDown()
		{
			LogManager.Flush();
		}

		private static void loadModules()
		{
			if (_loadedModules)
				return;

			Kernel = new StandardKernel();
			Kernel.Load<CoreModule>();
			Kernel.Load<DalModule>();
			Kernel.Load<DownloaderModule>();

			Repo = Kernel.Get<CardRepository>();
			ImgRepo = Kernel.Get<ImageRepository>();
			Ui = Kernel.Get<UiModel>();

			_loadedModules = true;
		}

		protected static void LoadCards()
		{
			if (_loadedCards)
				return;

			loadModules();

			var sw = new Stopwatch();
			sw.Start();

			Repo.LoadFile();
			Repo.Load();

			sw.Stop();
			_log.Info($"Cards loaded in {sw.ElapsedMilliseconds} ms");

			LogManager.Flush();

			_loadedCards = true;
		}

		protected static void LoadPrices()
		{
			if (_loadedPrices)
				return;

			LoadCards();

			var sw = new Stopwatch();
			sw.Start();

			var priceRepo = Kernel.Get<PriceRepository>();
			priceRepo.Load();

			Repo.SetPrices(priceRepo);

			sw.Stop();
			_log.Info($"Prices loaded in {sw.ElapsedMilliseconds} ms");

			_loadedPrices = true;
		}

		protected static void LoadTranslations()
		{
			if (_loadedTranslations)
				return;

			LoadCards();

			var sw = new Stopwatch();
			sw.Start();

			var locRepo = Kernel.Get<LocalizationRepository>();
			locRepo.LoadFile();
			locRepo.Load();

			Repo.FillLocalizations(locRepo);

			sw.Stop();
			_log.Info($"Translations loaded in {sw.ElapsedMilliseconds} ms");

			LogManager.Flush();

			_loadedTranslations = true;
		}

		protected static void LoadIndexes()
		{
			if (_loadedIndexes)
				return;

			LoadTranslations();

			Searcher = Kernel.Get<LuceneSearcher>();
			
			var sw = new Stopwatch();
			
			sw.Start();
			Searcher.LoadIndex();
			sw.Stop();
			_log.Info($"Searcher index loaded in {sw.ElapsedMilliseconds} ms");

			sw.Start();
			Searcher.LoadSpellcheckerIndex();
			sw.Stop();
			_log.Info($"Spellchecker index loaded in {sw.ElapsedMilliseconds} ms");

			Spellchecker = Searcher.Spellchecker;

			sw.Start();
			KeywordSearcher = Kernel.Get<KeywordSearcher>();
			KeywordSearcher.Load();
			sw.Stop();
			_log.Info($"Keyword searcher index loaded in {sw.ElapsedMilliseconds} ms");

			LogManager.Flush();

			_loadedIndexes = true;
		}

		protected static IKernel Kernel;
		protected static CardRepository Repo;
		protected static ImageRepository ImgRepo;
		protected static UiModel Ui;

		protected static LuceneSearcher Searcher;
		protected static LuceneSpellchecker Spellchecker;
		protected static KeywordSearcher KeywordSearcher;

		private static bool _loadedModules;
		private static bool _loadedCards;
		private static bool _loadedPrices;
		private static bool _loadedTranslations;
		private static bool _loadedIndexes;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		protected Logger Log => LogManager.GetLogger(GetType().FullName);
	}
}