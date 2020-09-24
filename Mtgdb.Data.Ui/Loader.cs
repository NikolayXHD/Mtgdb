using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using NLog;

namespace Mtgdb.Ui
{
	public class Loader
	{
		[UsedImplicitly] // by ninject
		public Loader(
			CardRepository repository,
			PriceRepository priceRepository,
			ImageRepository imageRepository,
			CardSearcher cardSearcher,
			KeywordSearcher keywordSearcher,
			IApplication app)
		{
			_repository = repository;
			_priceRepository = priceRepository;
			_imageRepository = imageRepository;
			_cardSearcher = cardSearcher;
			_keywordSearcher = keywordSearcher;
			_app = app;

			createLoadingActions();
		}

		public void AddTask(Action<CancellationToken> a)
		{
			_loadingTasks.Add(token =>
			{
				a(token);
				return Task.CompletedTask;
			});
		}

		public void AddTask(Func<CancellationToken, Task> a) =>
			_loadingTasks.Add(a);

		public async Task AsyncRun()
		{
			try
			{
				await Task.WhenAll(_loadingTasks.Select(task => _app.CancellationToken.Run(task)));
			}
			catch (TaskCanceledException ex) when (ex.CancellationToken == _app.CancellationToken)
			{
				_log.Info(ex);
			}
			catch (Exception ex)
			{
				_log.Error(ex);
			}
		}

		private void createLoadingActions()
		{
			AddTask(async token =>
			{
				await _repository.DownloadFile(token);
				_repository.LoadFile();

				if (_cardSearcher.IsUpToDate && _cardSearcher.Spellchecker.IsUpToDate)
					_cardSearcher.LoadIndexes();

				if (_keywordSearcher.IsUpToDate)
					_keywordSearcher.Load();

				_imageRepository.LoadFiles();
				_imageRepository.LoadSmall();
				_imageRepository.LoadZoom();
			});

			AddTask(async token =>
			{
				await _repository.IsDownloadComplete.Wait(token);
				await _priceRepository.DownloadPriceFile(token);
				_priceRepository.LoadPriceFile();
				_priceRepository.LoadPrice();
				await _repository.IsLoadingComplete.Wait(token);
				_priceRepository.FillPrice(_repository);
				_priceRepository.SaveCache(_repository);
				_priceRepository.Clear();
			});

			AddTask(async token =>
			{
				await _repository.IsFileLoadingComplete.Wait(token);
				_repository.Load();
				_repository.FillLocalizations();

				await _imageRepository.IsLoadingZoomComplete.Wait(token);

				if (!_keywordSearcher.IsUpToDate)
					_keywordSearcher.Load();

				await _priceRepository.IsLoadingPriceComplete.Wait(token);

				if (!(_cardSearcher.IsUpToDate && _cardSearcher.Spellchecker.IsUpToDate))
					_cardSearcher.LoadIndexes();

				_imageRepository.LoadArt();
				GC.Collect();
			});
		}

		private readonly CardRepository _repository;
		private readonly PriceRepository _priceRepository;
		private readonly ImageRepository _imageRepository;
		private readonly CardSearcher _cardSearcher;
		private readonly KeywordSearcher _keywordSearcher;
		private readonly IApplication _app;

		private readonly IList<Func<CancellationToken, Task>> _loadingTasks =
			new List<Func<CancellationToken, Task>>();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
