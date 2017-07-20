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
		private readonly CardRepository _repository;
		private readonly ImageRepository _imageRepository;
		private readonly LocalizationRepository _localizationRepository;
		private readonly LuceneSearcher _luceneSearcher;
		private readonly KeywordSearcher _keywordSearcher;

		public Loader(
			CardRepository repository,
			ImageRepository imageRepository,
			LocalizationRepository localizationRepository,
			LuceneSearcher luceneSearcher,
			KeywordSearcher keywordSearcher)
		{
			_repository = repository;
			_imageRepository = imageRepository;
			_localizationRepository = localizationRepository;
			_luceneSearcher = luceneSearcher;
			_keywordSearcher = keywordSearcher;

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
				},
				() =>
				{
					while (!_repository.IsFileLoadingComplete)
						Thread.Sleep(50);

					_repository.Load();

					while (!_imageRepository.IsLoadingComplete)
						Thread.Sleep(50);

					_repository.SelectCardImages(_imageRepository);

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
					Log.Info(ex, "Thread aborted");
				}
				catch (Exception ex)
				{
					Log.Error(ex);
					LogManager.Flush();
				}
			});
		}

		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private readonly IList<Action> _loadingActions;
		private List<Thread> _loadingThreads;
	}
}