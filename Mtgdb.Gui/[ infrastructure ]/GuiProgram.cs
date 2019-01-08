using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Downloader;
using Ninject;
using NLog;

namespace Mtgdb.Gui
{
	internal static class GuiProgram
	{
		[STAThread]
		[LoaderOptimization(LoaderOptimization.MultiDomainHost)]
		public static void Main(string[] args)
		{
			ShadowCopy.RunMain(main, args);
		}

		private static void main(string[] args)
		{
			System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			AppDomain.CurrentDomain.UnhandledException += unhandledException;
			System.Windows.Forms.Application.ThreadException += threadException;

			System.Windows.Forms.Application.EnableVisualStyles();
			System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();
			_kernel.Load<DownloaderModule>();
			_kernel.Load<GuiModule>();

			Dpi.Set(_kernel.Get<UiConfigRepository>().Config.UiScalePercent);

			var installer = _kernel.Get<Installer>();

			installer.MtgjsonFileUpdated += mtgjsonFileUpdated;
			installer.BeginInstall += beginInstall;
			installer.EndInstall += endInstall;

			_kernel.Get<PriceDownloader>().PricesDownloaded += pricesDownloaded;

			var loader = _kernel.Get<GuiLoader>();
			loader.AsyncRun();
			TaskEx.Run(loader.AsyncConvertLegacyCardId).Wait();

			var colorSchemeEditorForm = _kernel.Get<ColorSchemeEditor>();
			colorSchemeEditorForm.SaveDirectory = AppDir.ColorSchemes;
			colorSchemeEditorForm.LoadCurrentColorScheme();

			var application = _kernel.Get<Application>();
			application.MigrateHistoryFiles();
			application.CreateForm();

			System.Windows.Forms.Application.Run(application);

			application.Cancel();
		}

		private static void mtgjsonFileUpdated()
		{
			var luceneSearcher = _kernel.Get<CardSearcher>();
			var keywordSearcher = _kernel.Get<KeywordSearcher>();

			luceneSearcher.InvalidateIndex();
			luceneSearcher.Spellchecker.InvalidateIndex();
			keywordSearcher.InvalidateIndex();
		}

		private static void beginInstall()
		{
		}

		private static void endInstall()
		{
		}

		private static void pricesDownloaded()
		{
			_kernel.Get<CardSearcher>().InvalidateIndex();
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

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		private static readonly IKernel _kernel = new StandardKernel();
	}
}
