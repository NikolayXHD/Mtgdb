using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Downloader;

namespace Mtgdb.Gui
{
	public class GuiLoader
	{
		public GuiLoader(
			Loader loader,
			NewsService newsService,
			DownloaderSubsystem downloaderSubsystem,
			DeckListModel deckListModel,
			DeckSearcher deckSearcher)
		{
			_loader = loader;

			_loader.Add(newsService.FetchNews);
			_loader.Add(downloaderSubsystem.CalculateProgress);
			_loader.Add(() =>
			{
				deckListModel.Load();
				deckSearcher.LoadIndexes();
			});
		}

		public void Run() =>
			_loader.Run();

		public void Abort() => _loader.Abort();

		private readonly Loader _loader;
	}
}