using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mtgdb.Dal.Index;
using NLog;

namespace Mtgdb.Dal
{
	public class Loader
	{
		public Loader(
			CardRepository repository,
			ImageRepository imageRepository,
			LocalizationRepository localizationRepository,
			LuceneSearcher luceneSearcher,
			KeywordSearcher keywordSearcher,
			PriceRepository priceRepository)
		{
			_repository = repository;
			_imageRepository = imageRepository;
			_localizationRepository = localizationRepository;
			_luceneSearcher = luceneSearcher;
			_keywordSearcher = keywordSearcher;
			_priceRepository = priceRepository;

			_loadingActions = createLoadingActions();
		}

		public void Add(Action a)
		{
			_loadingActions.Add(a);
		}

		public void Run()
		{
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
					
					if (_keywordSearcher.IsUpToDate)
						_keywordSearcher.Load(null);

					if (_luceneSearcher.IsUpToDate)
						_luceneSearcher.LoadIndex(null);

					_imageRepository.LoadFiles();
					_imageRepository.Load();
					_imageRepository.LoadZoom();
					_imageRepository.LoadArt();

					_priceRepository.Load();
				},
				() =>
				{
					while (!_repository.IsFileLoadingComplete)
						Thread.Sleep(50);
					_repository.Load();

					while (!_imageRepository.IsLoadingZoomComplete)
						Thread.Sleep(50);
					_repository.SelectCardImages(_imageRepository);

					while (!_priceRepository.IsLoadingComplete)
						Thread.Sleep(50);
					_repository.SetPrices(_priceRepository);

					_localizationRepository.LoadFile();
					_localizationRepository.Load();
					_repository.FillLocalizations(_localizationRepository);

					_localizationRepository.Clear();

					if (!_keywordSearcher.IsUpToDate)
						_keywordSearcher.Load(_repository);

					if (!_luceneSearcher.IsUpToDate)
						_luceneSearcher.LoadIndex(_repository);

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
		private readonly LuceneSearcher _luceneSearcher;
		private readonly KeywordSearcher _keywordSearcher;
		private readonly PriceRepository _priceRepository;
	}
}