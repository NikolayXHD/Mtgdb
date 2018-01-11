using System;
using System.Threading;
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
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[LoaderOptimization(LoaderOptimization.MultiDomainHost)]
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

			Dpi.Initialize();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			
			_kernel.Load<CoreModule>();
			_kernel.Load<DalModule>();
			_kernel.Load<DownloaderModule>();
			_kernel.Load<GuiModule>();

			var installer = _kernel.Get<Installer>();

			installer.MtgjsonFileUpdated += mtgjsonFileUpdated;
			installer.BeginInstall += beginInstall;
			installer.EndInstall += endInstall;

			_kernel.Get<PriceDownloader>().PricesDownloaded += pricesDownloaded;

			var formRoot = _kernel.Get<FormRoot>();

			formRoot.NewTab(onCreated: null);
			formRoot.Show();
			Application.Run(formRoot);
		}

		private static void mtgjsonFileUpdated()
		{
			var luceneSearcher = _kernel.Get<LuceneSearcher>();
			var keywordSearcher = _kernel.Get<KeywordSearcher>();

			luceneSearcher.InvalidateIndex();
			luceneSearcher.InvalidateSpellcheckerIndex();

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
			_kernel.Get<LuceneSearcher>().InvalidateIndex();
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
