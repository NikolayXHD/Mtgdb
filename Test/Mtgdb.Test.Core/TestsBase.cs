using System.Diagnostics;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using Mtgdb.Dev;
using Mtgdb.Downloader;
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

		protected static void LoadModules()
		{
			lock (_syncLoadModules)
			{
				if (_loadedModules)
					return;

				FsPathPersistence.RegisterPathSubstitution(DevPaths.DriveSubstitution);

				Kernel = new StandardKernel();
				Kernel.Load<DalModule>();
				Kernel.Load<CoreModule>();
				Kernel.Load<DownloaderModule>();
				Kernel.Load<DeckListModule>();

				Repo = Kernel.Get<CardRepository>();
				PriceRepo = Kernel.Get<PriceRepository>();
				ImgRepo = Kernel.Get<ImageRepository>();
				Ui = Kernel.Get<UiModel>();
				Formatter = Kernel.Get<CardFormatter>();

				_loadedModules = true;
			}
		}

		protected static void LoadCards()
		{
			lock (_syncLoadCards)
			{
				if (_loadedCards)
					return;

				LoadModules();

				var sw = new Stopwatch();
				sw.Start();

				Repo.LoadFile();
				Repo.Load();
				Repo.FillLocalizations();

				sw.Stop();
				_log.Info($"Cards loaded in {sw.ElapsedMilliseconds} ms");

				LogManager.Flush();

				_loadedCards = true;
			}
		}

		protected static void LoadPrices()
		{
			lock (_syncLoadPrices)
			{
				if (_loadedPrices || _loadedMtgjsonPrices)
					return;

				LoadCards();

				var sw = new Stopwatch();
				sw.Start();

				PriceRepo.LoadPriceFile();
				PriceRepo.LoadPrice();
				PriceRepo.FillPrice(Repo);

				sw.Stop();
				_log.Info($"Prices loaded in {sw.ElapsedMilliseconds} ms");

				LogManager.Flush();

				_loadedPrices = true;
				if (!PriceRepo.PriceCacheExists())
					_loadedMtgjsonPrices = true;
			}
		}

		protected static void LoadMtgjsonPrices()
		{
			lock (_syncLoadMtgjsonPrices)
			{
				if (_loadedMtgjsonPrices)
					return;

				LoadCards();

				var sw = new Stopwatch();
				sw.Start();

				PriceRepo.LoadPriceFile(ignoreCache: true);
				PriceRepo.LoadPrice(ignoreCache: true);
				if (!PriceRepo.IsLoadingPriceComplete.Signaled)
					PriceRepo.FillPrice(Repo);

				sw.Stop();
				_log.Info($"Mtgjson prices loaded in {sw.ElapsedMilliseconds} ms");

				LogManager.Flush();

				_loadedMtgjsonPrices = true;
			}
		}

		protected static void LoadIndexes()
		{
			lock (_syncLoadIndexes)
			{
				if (_loadedIndexes)
					return;

				LoadPrices();

				Searcher = Kernel.Get<CardSearcher>();

				var sw = new Stopwatch();

				sw.Start();
				Searcher.LoadIndexes();
				sw.Stop();
				_log.Info($"Searcher indexes loaded in {sw.ElapsedMilliseconds} ms");

				Spellchecker = Searcher.Spellchecker;

				sw.Start();
				KeywordSearcher = Kernel.Get<KeywordSearcher>();
				KeywordSearcher.Load();
				sw.Stop();
				_log.Info($"Keyword searcher index loaded in {sw.ElapsedMilliseconds} ms");

				LogManager.Flush();

				_loadedIndexes = true;
			}

		}

		protected static void LoadSmallAndZoomImages()
		{
			lock (_syncLoadImages)
			{
				if (_loadedSmallAndZoomImages)
					return;

				ImgRepo.LoadFiles();
				ImgRepo.LoadZoom();

				_loadedSmallAndZoomImages = true;
			}
		}

		protected static IKernel Kernel;
		protected static CardRepository Repo { get; private set; }
		protected static PriceRepository PriceRepo { get; private set; }
		protected static ImageRepository ImgRepo { get; private set; }
		protected static UiModel Ui { get; private set; }

		protected static CardSearcher Searcher { get; private set; }
		protected static CardSpellchecker Spellchecker { get; private set; }
		protected static KeywordSearcher KeywordSearcher { get; private set; }
		protected static CardFormatter Formatter { get; set; }

		private static bool _loadedModules;
		private static bool _loadedCards;
		private static bool _loadedPrices;
		private static bool _loadedMtgjsonPrices;
		private static bool _loadedIndexes;
		private static bool _loadedSmallAndZoomImages;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		protected Logger Log => LogManager.GetLogger(GetType().FullName);

		private static readonly object _syncLoadModules = new object();
		private static readonly object _syncLoadCards = new object();
		private static readonly object _syncLoadPrices = new object();
		private static readonly object _syncLoadMtgjsonPrices = new object();
		private static readonly object _syncLoadIndexes = new object();
		private static readonly object _syncLoadImages = new object();
	}
}
