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
			
			Kernel.Load<CoreModule>();
			Kernel.Load<DalModule>();
			Kernel.Load<DownloaderModule>();
			Kernel.Load<GuiModule>();

			var installer = Kernel.Get<Installer>();

			installer.MtgjsonFileUpdated += mtgjsonFileUpdated;
			installer.BeginInstall += beginInstall;
			installer.EndInstall += endInstall;

			Kernel.Get<PriceDownloader>().PricesDownloaded += pricesDownloaded;

			var formRoot = Kernel.Get<FormRoot>();
			formRoot.NewTab(null);
			formRoot.Show();
			Application.Run(formRoot);
		}

		private static void mtgjsonFileUpdated()
		{
			var luceneSearcher = Kernel.Get<LuceneSearcher>();

			luceneSearcher.InvalidateIndex();
			luceneSearcher.InvalidateSpellcheckerIndex();

			Kernel.Get<KeywordSearcher>().InvalidateIndex();
		}

		private static void beginInstall()
		{
		}

		private static void endInstall()
		{
		}

		private static void pricesDownloaded()
		{
			Kernel.Get<LuceneSearcher>().InvalidateIndex();
		}

		private static void threadException(object sender, ThreadExceptionEventArgs e)
		{
			Log.Error(e.Exception);
			LogManager.Flush();
		}

		private static void unhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log.Error(e.ExceptionObject);
			LogManager.Flush();
		}

		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private static readonly IKernel Kernel = new StandardKernel();
	}
}
