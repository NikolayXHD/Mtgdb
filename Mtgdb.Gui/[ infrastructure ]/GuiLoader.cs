using System.Threading.Tasks;
using JetBrains.Annotations;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using Mtgdb.Data.Model;
using Mtgdb.Downloader;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	public class GuiLoader
	{
		[UsedImplicitly] // by ninject
		public GuiLoader(
			Loader loader,
			CardRepository repo,
			NewsService newsService,
			DownloaderSubsystem downloaderSubsystem,
			DeckListModel deckListModel,
			DeckSearcher deckSearcher,
			DeckIndexUpdateSubsystem deckIndexUpdateSubsystem)
		{
			_loader = loader;
			_repo = repo;

			_loader.AddAction(newsService.FetchNews);
			_loader.AddAction(downloaderSubsystem.CalculateProgress);
			_loader.AddTask(async () =>
			{
				deckListModel.Load();

				if (!deckSearcher.IsIndexSaved)
					while (!_repo.IsLoadingComplete)
						await Task.Delay(100);

				deckSearcher.LoadIndexes();
				deckIndexUpdateSubsystem.SubscribeToEvents();
			});
		}

		public Task AsyncRun()
		{
			return _loader.AsyncRun();
		}

		private readonly Loader _loader;
		private readonly CardRepository _repo;
	}
}
