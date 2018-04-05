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

			if (!Searcher.IsUpToDate)
			{
				var sw = new Stopwatch();
				sw.Restart();
				Searcher.LoadIndexes();
				sw.Stop();

				_log.Debug($"Index created in {sw.ElapsedMilliseconds} ms");
			}
			else
			{
				var sw = new Stopwatch();
				sw.Start();

				Searcher.LoadIndexes();

				sw.Stop();
				_log.Debug($"Index created in {sw.ElapsedMilliseconds} ms");
			}

			Spellchecker = Searcher.Spellchecker;

			LogManager.Flush();

			_loadedIndexes = true;
		}

		protected static IKernel Kernel;
		protected static CardRepository Repo;
		protected static ImageRepository ImgRepo;
		protected static UiModel Ui;

		protected static LuceneSearcher Searcher;
		protected static LuceneSpellchecker Spellchecker;

		private static bool _loadedModules;
		private static bool _loadedCards;
		private static bool _loadedTranslations;
		private static bool _loadedIndexes;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		protected Logger Log => LogManager.GetLogger(GetType().FullName);
	}
}