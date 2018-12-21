using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
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
			CardRepositoryLegacy repoLegacy,
			NewsService newsService,
			DownloaderSubsystem downloaderSubsystem,
			DeckListModel deckListModel,
			DeckListLegacyConverter deckListLegacyConverter,
			HistoryLegacyConverter historyLegacyConverter,
			DeckSearcher deckSearcher)
		{
			_loader = loader;
			_repo = repo;
			_repoLegacy = repoLegacy;
			_deckListLegacyConverter = deckListLegacyConverter;
			_historyLegacyConverter = historyLegacyConverter;

			_loader.AddAction(newsService.FetchNews);
			_loader.AddAction(downloaderSubsystem.CalculateProgress);
			_loader.AddTask(async () =>
			{
				if (deckListLegacyConverter.IsConversionRequired)
					while (!deckListLegacyConverter.IsConversionCompleted)
						await TaskEx.Delay(100);

				deckListModel.Load();

				if (deckSearcher.IsIndexSaved)
					deckSearcher.LoadIndexes();
				else
				{
					while (!_repo.IsPriceLoadingComplete)
						await TaskEx.Delay(100);

					deckSearcher.LoadIndexes();
				}
			});
		}

		public Task AsyncRun()
		{
			_started = true;
			return _loader.AsyncRun();
		}

		public async Task AsyncConvertLegacyCardId()
		{
			if (!_started)
				throw new InvalidOperationException();

			var legacyHistoryFiles = _historyLegacyConverter.FindLegacyFiles().ToList();

			if (legacyHistoryFiles.Count == 0 && !_deckListLegacyConverter.IsConversionRequired)
				return;

			_repoLegacy.LoadFile();
			_repoLegacy.Load();

			while (!_repo.IsLoadingComplete)
				await TaskEx.Delay(100);

			foreach (var legacyHistoryFile in legacyHistoryFiles)
				_historyLegacyConverter.Convert(legacyHistoryFile);

			if (_deckListLegacyConverter.IsConversionRequired)
				_deckListLegacyConverter.Convert();
		}

		private readonly Loader _loader;
		private readonly CardRepository _repo;
		private readonly CardRepositoryLegacy _repoLegacy;
		private readonly DeckListLegacyConverter _deckListLegacyConverter;
		private readonly HistoryLegacyConverter _historyLegacyConverter;
		private bool _started;
	}
}