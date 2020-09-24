using System;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using Mtgdb.Downloader;
using Ninject;
using NLog;

namespace Mtgdb.Gui
{
	internal static class GuiProgram
	{
		[STAThread]
		public static void Main(string[] args)
		{
			ShadowCopy.RunMain(main, args);
		}

		private static void main(string[] args)
		{
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			AppDomain.CurrentDomain.UnhandledException += unhandledException;
			Application.ThreadException += threadException;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();
			_kernel.Load<DownloaderModule>();
			_kernel.Load<DeckListModule>();
			_kernel.Load<GuiModule>();

			Dpi.Set(_kernel.Get<UiConfigRepository>().Config.UiScalePercent);

			var installer = _kernel.Get<Installer>();

			installer.MtgjsonFileUpdated += mtgjsonFileUpdated;
			installer.BeginInstall += beginInstall;
			installer.EndInstall += endInstall;

			_kernel.Get<GuiLoader>().AsyncRun();

			var colorSchemeEditorForm = _kernel.Get<ColorSchemeEditor>();
			colorSchemeEditorForm.SaveDirectory = AppDir.ColorSchemes;
			colorSchemeEditorForm.LoadCurrentColorScheme();

			_kernel.Get<TooltipConfiguration>().Setup();
			Application.AddMessageFilter(MessageFilter.Instance);

			var application = _kernel.Get<App>();
			application.MigrateHistoryFiles();
			application.StartForm();

			Application.Run(application);
			Application.RemoveMessageFilter(MessageFilter.Instance);
			application.CancelAllTasks();
		}

		private static void mtgjsonFileUpdated()
		{
			var luceneSearcher = _kernel.Get<CardSearcher>();
			var keywordSearcher = _kernel.Get<KeywordSearcher>();

			luceneSearcher.InvalidateIndex();
			luceneSearcher.Spellchecker.InvalidateIndex();
			keywordSearcher.InvalidateIndex();

			var priceRepo = _kernel.Get<PriceRepository>();
			if (priceRepo.PriceCacheExists())
				priceRepo.DeletePriceCache();
		}

		private static void beginInstall()
		{
		}

		private static void endInstall()
		{
		}

		private static void threadException(object sender, ThreadExceptionEventArgs e)
		{
			_log.Error(e.Exception);
			LogManager.Flush();
		}

		private static void unhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			_log.Error(e.ExceptionObject);
			LogManager.Flush();
		}

		private static readonly IKernel _kernel = new StandardKernel();
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
