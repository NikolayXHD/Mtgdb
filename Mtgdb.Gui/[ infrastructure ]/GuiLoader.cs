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
			DeckListModel deckListModel)
		{
			_loader = loader;
			_deckListModel = deckListModel;

			_loader.Add(newsService.FetchNews);
			_loader.Add(downloaderSubsystem.CalculateProgress);
		}

		public void Run()
		{
			_loader.Run();
			_deckListModel.Load();
		}

		public void Abort() => _loader.Abort();

		private readonly Loader _loader;
		private readonly DeckListModel _deckListModel;
	}
}