using Mtgdb.Dal;
using Mtgdb.Downloader;

namespace Mtgdb.Gui
{
	public class GuiLoader
	{
		public GuiLoader(
			Loader loader,
			NewsService newsService,
			DownloaderSubsystem downloaderSubsystem)
		{
			_loader = loader;

			_loader.Add(newsService.FetchNews);
			_loader.Add(downloaderSubsystem.CalculateProgress);
		}

		public void Run() => _loader.Run();
		public void Abort() => _loader.Abort();

		private readonly Loader _loader;
	}
}