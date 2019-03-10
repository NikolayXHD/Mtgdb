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
			ImageRepository imageRepository,
			LocalizationRepository localizationRepository,
			CardSearcher cardSearcher,
			KeywordSearcher keywordSearcher,
			PriceRepository priceRepository,
			IApplication application)
		{
			_repository = repository;
			_imageRepository = imageRepository;
			_localizationRepository = localizationRepository;
			_cardSearcher = cardSearcher;
			_keywordSearcher = keywordSearcher;
			_priceRepository = priceRepository;
			_application = application;

			createLoadingActions();
		}

		public void AddAction(Action a) =>
			_loadingActions.Add(wrap(a));

		public void AddTask(Func<Task> a) =>
			_loadingTasks.Add(wrap(a));

		private Action wrap(Action a) =>
			() =>
			{
				try
				{
					using (_application.CancellationToken.Register(Thread.CurrentThread.Abort))
						a.Invoke();
				}
				catch (Exception ex)
				{
					_log.Error(ex);
				}
			};

		private Func<Task> wrap(Func<Task> a) =>
			async () =>
			{
				try
				{
					using (_application.CancellationToken.Register(Thread.CurrentThread.Abort))
						await a.Invoke();
				}
				catch (Exception ex)
				{
					_log.Error(ex);
				}
			};

		public Task AsyncRun()
		{
			_indexesUpToDate = _cardSearcher.IsUpToDate && _cardSearcher.Spellchecker.IsUpToDate;
			return TaskEx.WhenAll(_loadingTasks.Select(TaskEx.Run).Concat(_loadingActions.Select(TaskEx.Run)));
		}

		private void createLoadingActions()
		{
			AddAction(() =>
			{
				_repository.LoadFile();

				_imageRepository.LoadFiles();

				_imageRepository.LoadSmall();
				_imageRepository.LoadZoom();

				if (_indexesUpToDate)
					_cardSearcher.LoadIndexes();

				if (_keywordSearcher.IsUpToDate)
					_keywordSearcher.Load();

				_priceRepository.Load();
			});

			AddTask(async () =>
			{
				while (!_repository.IsFileLoadingComplete)
					await TaskEx.Delay(50);

				_repository.Load();

				while (!_imageRepository.IsLoadingZoomComplete)
					await TaskEx.Delay(50);

				while (!_priceRepository.IsLoadingComplete)
					await TaskEx.Delay(50);

				_repository.FillPrices(_priceRepository);

				_localizationRepository.LoadFile();
				_localizationRepository.Load();

				_repository.FillLocalizations(_localizationRepository);
				_localizationRepository.Clear();

				if (!_keywordSearcher.IsUpToDate)
					_keywordSearcher.Load();

				if (!_indexesUpToDate)
					_cardSearcher.LoadIndexes();

				_imageRepository.LoadArt();

				GC.Collect();
			});
		}

		private readonly CardRepository _repository;
		private readonly ImageRepository _imageRepository;
		private readonly LocalizationRepository _localizationRepository;
		private readonly CardSearcher _cardSearcher;
		private readonly KeywordSearcher _keywordSearcher;
		private readonly PriceRepository _priceRepository;
		private readonly IApplication _application;
		private bool _indexesUpToDate;

		private readonly IList<Func<Task>> _loadingTasks = new List<Func<Task>>();
		private readonly IList<Action> _loadingActions = new List<Action>();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}