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
			IApplication app)
		{
			_repository = repository;
			_imageRepository = imageRepository;
			_localizationRepository = localizationRepository;
			_cardSearcher = cardSearcher;
			_keywordSearcher = keywordSearcher;
			_app = app;

			createLoadingActions();
		}

		public void AddTask(Action<CancellationToken> a) =>
			_loadingTasks.Add(wrap(a));

		public void AddTask(Func<CancellationToken, Task> a) =>
			_loadingTasks.Add(wrap(a));

		private Func<CancellationToken, Task> wrap(Action<CancellationToken> a) =>
			token => Task.Run(() => a(token), token);

		private Func<CancellationToken, Task> wrap(Func<CancellationToken, Task> a) =>
			token => Task.Run(() => a(token), token);

		public async Task AsyncRun()
		{
			_indexesUpToDate = _cardSearcher.IsUpToDate && _cardSearcher.Spellchecker.IsUpToDate;
			try
			{
				await Task.WhenAll(_loadingTasks.Select(_ => _(_app.CancellationToken)));
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
			AddTask(token =>
			{
				_repository.LoadFile();
				_imageRepository.LoadFiles();
				_imageRepository.LoadSmall();
				_imageRepository.LoadZoom();

				if (_indexesUpToDate)
					_cardSearcher.LoadIndexes();

				if (_keywordSearcher.IsUpToDate)
					_keywordSearcher.Load();
			});

			AddTask(async token =>
			{
				await _repository.IsFileLoadingComplete.Wait(token);
				_repository.Load();

				await _imageRepository.IsLoadingZoomComplete.Wait(token);
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
		private readonly IApplication _app;
		private bool _indexesUpToDate;

		private readonly IList<Func<CancellationToken, Task>> _loadingTasks =
			new List<Func<CancellationToken, Task>>();

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
