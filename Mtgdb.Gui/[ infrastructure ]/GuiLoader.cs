using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Mtgdb.Ui;
using Ninject;

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
			DeckListLegacyConverter deckListConverter,
			HistoryLegacyConverter historyConverter,
			DeckSearcher deckSearcher,
			[Named(GuiModule.DefaultTooltipScope)] TooltipForm defaultTooltip,
			[Named(GuiModule.QuickFilterTooltipScope)] TooltipForm quickFilterTooltip,
			[Named(GuiModule.QuickFilterTooltipScope)] TooltipController quickFilterTooltipController)
		{
			_loader = loader;
			_repo = repo;
			_repoLegacy = repoLegacy;
			_deckListConverter = deckListConverter;
			_historyConverter = historyConverter;

			_loader.AddAction(newsService.FetchNews);
			_loader.AddAction(downloaderSubsystem.CalculateProgress);
			_loader.AddTask(async () =>
			{
				if (deckListConverter.IsLegacyConversionRequired)
					while (!deckListConverter.IsLegacyConversionCompleted)
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

			setupTooltips(quickFilterTooltip, quickFilterTooltipController);
		}

		private static void setupTooltips(TooltipForm quickFilterTooltip, TooltipController quickFilterTooltipController)
		{
			quickFilterTooltip.BackColor = SystemColors.Control;
			quickFilterTooltip.TooltipBorderStyle = DashStyle.Dash;
			quickFilterTooltip.TextPadding = new Padding(1, 1, 1, 1);

			quickFilterTooltipController.DelayMs = 0;
			quickFilterTooltipController.ToggleOnAlt = false;
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

			if (legacyHistoryFiles.Count != 0 || _deckListConverter.IsLegacyConversionRequired)
			{
				_repoLegacy.LoadFile();
				_repoLegacy.Load();

				while (!_repo.IsLoadingComplete)
					await TaskEx.Delay(100);

				foreach (var legacyHistoryFile in legacyHistoryFiles)
					_historyConverter.ConvertLegacyFile(legacyHistoryFile);

				if (_deckListConverter.IsLegacyConversionRequired)
					_deckListConverter.ConvertLegacyList();
			}

			foreach (var v2HistoryFile in v2HistoryFiles)
				_historyConverter.ConvertV2File(v2HistoryFile);

			if (_deckListConverter.IsV2ConversionRequired)
				_deckListConverter.ConvertV2List();
		}

		private readonly Loader _loader;
		private readonly CardRepository _repo;
		private readonly CardRepositoryLegacy _repoLegacy;
		private readonly DeckListLegacyConverter _deckListConverter;
		private readonly HistoryLegacyConverter _historyConverter;
		private bool _started;
	}
}