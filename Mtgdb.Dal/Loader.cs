using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mtgdb.Dal.Index;
using NLog;
using ThreadState = System.Threading.ThreadState;

namespace Mtgdb.Dal
{
	public class Loader
	{
		public Loader(
			CardRepository repository,
			ImageRepository imageRepository,
			LocalizationRepository localizationRepository,
			CardSearcher cardSearcher,
			KeywordSearcher keywordSearcher,
			PriceRepository priceRepository)
		{
			_repository = repository;
			_imageRepository = imageRepository;
			_localizationRepository = localizationRepository;
			_cardSearcher = cardSearcher;
			_keywordSearcher = keywordSearcher;
			_priceRepository = priceRepository;

			_loadingActions = createLoadingActions();
		}

		public void Add(Action a) =>
			_loadingActions.Add(a);

		public void Run()
		{
			_indexesUpToDate = _cardSearcher.IsUpToDate && _cardSearcher.Spellchecker.IsUpToDate;

			_loadingThreads = _loadingActions.Select(createLoadingThread)
				.ToList();

			foreach (var loadingThread in _loadingThreads)
				loadingThread.Start();
		}

		public void Abort()
		{
			foreach (var thread in _loadingThreads)
				if (thread.ThreadState == ThreadState.Running)
					thread.Abort();
		}

		private IList<Action> createLoadingActions()
		{
			var loadingDelegates = new List<Action>
			{
				() =>
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
				},
				() =>
				{
					while (!_repository.IsFileLoadingComplete)
						Thread.Sleep(50);

					_repository.Load();

					while (!_imageRepository.IsLoadingZoomComplete)
						Thread.Sleep(50);

					while (!_priceRepository.IsLoadingComplete)
						Thread.Sleep(50);

					_repository.SetPrices(_priceRepository);

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
				}
			};

			return loadingDelegates;
		}

		private static Thread createLoadingThread(Action action)
		{
			return new Thread(() =>
			{
				try
				{
					action();
				}
				catch (ThreadAbortException ex)
				{
					_log.Info(ex, "Thread aborted");
				}
				catch (Exception ex)
				{
					_log.Error(ex);
					LogManager.Flush();
				}
			});
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly IList<Action> _loadingActions;
		private List<Thread> _loadingThreads;

		private readonly CardRepository _repository;
		private readonly ImageRepository _imageRepository;
		private readonly LocalizationRepository _localizationRepository;
		private readonly CardSearcher _cardSearcher;
		private readonly KeywordSearcher _keywordSearcher;
		private readonly PriceRepository _priceRepository;
		private bool _indexesUpToDate;
	}
}