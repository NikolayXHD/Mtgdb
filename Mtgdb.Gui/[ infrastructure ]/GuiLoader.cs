using System;
using System.Linq;
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
			CardRepository42 repo42,
			CardRepositoryLegacy repoLegacy,
			NewsService newsService,
			DownloaderSubsystem downloaderSubsystem,
			DeckListModel deckListModel,
			DeckListLegacyConverter deckListConverter,
			HistoryLegacyConverter historyConverter,
			DeckSearcher deckSearcher)
		{
			_loader = loader;
			_repo = repo;
			_repo42 = repo42;
			_repoLegacy = repoLegacy;
			_deckListConverter = deckListConverter;
			_historyConverter = historyConverter;

			_loader.AddAction(newsService.FetchNews);
			_loader.AddAction(downloaderSubsystem.CalculateProgress);
			_loader.AddTask(async () =>
			{
				if (deckListConverter.IsLegacyConversionRequired)
					while (!deckListConverter.IsConversionCompleted)
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

			var legacyHistoryFiles = _historyConverter.FindLegacyFiles().ToList();
			var v2HistoryFiles = _historyConverter.FindV2Files().ToList();
			var v3HistoryFiles = _historyConverter.FindV3Files().ToList();

			bool legacyConversionRequired = legacyHistoryFiles.Count != 0 || _deckListConverter.IsLegacyConversionRequired;
			bool v2ConversionRequired = v2HistoryFiles.Count != 0 || _deckListConverter.IsV2ConversionRequired;
			bool v3ConversionRequired = v3HistoryFiles.Count != 0 || _deckListConverter.IsV3ConversionRequired;

			if (!legacyConversionRequired && !v2ConversionRequired && !v3ConversionRequired)
				return;

			_repoLegacy.LoadFile();
			_repoLegacy.Load();

			while (!_repo.IsLoadingComplete)
				await TaskEx.Delay(100);

			if (legacyConversionRequired)
			{
				foreach (var legacyHistoryFile in legacyHistoryFiles)
					_historyConverter.ConvertLegacyFile(legacyHistoryFile);

				if (_deckListConverter.IsLegacyConversionRequired)
					_deckListConverter.ConvertLegacyList();
			}

			if (v2ConversionRequired)
			{
				foreach (var v2HistoryFile in v2HistoryFiles)
					_historyConverter.ConvertV2File(v2HistoryFile);

				if (_deckListConverter.IsV2ConversionRequired)
					_deckListConverter.ConvertV2List();
			}

			if (v3ConversionRequired)
			{
				_repo42.LoadFile();
				_repo42.Load();

				foreach (var v3HistoryFile in v3HistoryFiles)
					_historyConverter.ConvertV3File(v3HistoryFile);

				if (_deckListConverter.IsV3ConversionRequired)
					_deckListConverter.ConvertV3List();
			}
		}

		private readonly Loader _loader;
		private readonly CardRepository _repo;
		private readonly CardRepository42 _repo42;
		private readonly CardRepositoryLegacy _repoLegacy;
		private readonly DeckListLegacyConverter _deckListConverter;
		private readonly HistoryLegacyConverter _historyConverter;
		private bool _started;
	}
}