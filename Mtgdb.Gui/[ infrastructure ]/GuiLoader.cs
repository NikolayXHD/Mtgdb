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

			_loader.AddTask(token => newsService.FetchNews());
			_loader.AddTask(token => downloaderSubsystem.CalculateProgress());
			_loader.AddTask(async token =>
			{
				deckListModel.Load();

				if (!deckSearcher.IsIndexSaved)
					await _repo.IsLoadingComplete.Wait(token);

				deckSearcher.LoadIndexes();
				deckIndexUpdateSubsystem.SubscribeToEvents();
			});
		}

		public Task AsyncRun() =>
			_loader.AsyncRun();

		private readonly Loader _loader;
		private readonly CardRepository _repo;
	}
}
