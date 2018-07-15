using System.Threading;
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
			DeckSearcher deckSearcher,
			CardRepository repo,
			UiModel ui)
		{
			_loader = loader;

			_loader.Add(newsService.FetchNews);
			_loader.Add(downloaderSubsystem.CalculateProgress);
			_loader.Add(() =>
			{
				deckListModel.Load();

				while (!repo.IsPriceLoadingComplete)
					Thread.Sleep(100);

				deckSearcher.LoadIndexes(ui);
			});
		}

		public void Run() =>
			_loader.Run();

		public void Abort() => _loader.Abort();

		private readonly Loader _loader;
	}
}