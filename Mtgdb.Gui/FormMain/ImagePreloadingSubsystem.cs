using System;
using System.Collections.Generic;
using System.Threading;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class ImagePreloadingSubsystem
	{
		private readonly LayoutView _layoutViewCards;
		private readonly LayoutView _layoutViewDeck;
		private readonly ScrollSubsystem _scrollSubsystem;
		private Thread _preloadImageThread;

		private List<Card> _cardsToPreloadImage;
		private List<Card> _cardsToPreloadImageStarted;
		private readonly int _imageCacheCapacity;

		public ImagePreloadingSubsystem(LayoutView layoutViewCards, LayoutView layoutViewDeck, ScrollSubsystem scrollSubsystem, ImageCacheConfig imageCacheConfig)
		{
			_layoutViewCards = layoutViewCards;
			_layoutViewDeck = layoutViewDeck;
			_scrollSubsystem = scrollSubsystem;
			_imageCacheCapacity = imageCacheConfig.GetCacheCapacity();
		}

		public void Reset()
		{
			var cardsToPreloadImage = getCardsToPreview(_layoutViewDeck);
			cardsToPreloadImage.AddRange(getCardsToPreview(_layoutViewCards));

			_cardsToPreloadImage = cardsToPreloadImage;
		}

		public void StartThread()
		{
			if (_preloadImageThread?.ThreadState == ThreadState.Running)
				throw new InvalidOperationException("Already started");

			_preloadImageThread = new Thread(_ => preloadImageThread());
			_preloadImageThread.Start();
		}

		public void AbortThread()
		{
			_preloadImageThread.Abort();
		}

		private List<Card> getCardsToPreview(LayoutView view)
		{
			var pageSize = _scrollSubsystem.GetPageSize(view);
			
			int visibleRecordIndex = view.VisibleRecordIndex;

			var cardsToPreloadImage = new List<Card>();

			bool endReached = false;
			bool startReached = false;
			int i = 0;

			while (true)
			{
				if (startReached && endReached)
					break;

				if (cardsToPreloadImage.Count > _imageCacheCapacity - pageSize)
					break;

				startReached = startReached || !preload(cardsToPreloadImage, view, visibleRecordIndex - i - 1);
				endReached = endReached || !preload(cardsToPreloadImage, view, visibleRecordIndex + pageSize + i);

				i++;
			}

			return cardsToPreloadImage;
		}

		private static bool preload(List<Card> cardsToPreloadImage, LayoutView view, int i)
		{
			if (i < 0 || i >= view.RowCount)
				return false;

			int handle = view.GetVisibleRowHandle(i);
			var card = (Card) view.GetRow(handle);

			if (card == null)
				return false;

			cardsToPreloadImage.Add(card);
			return true;
		}

		private void preloadImageLoopIteration()
		{
			if (_cardsToPreloadImage == null || _cardsToPreloadImage == _cardsToPreloadImageStarted)
			{
				Thread.Sleep(200);
				return;
			}

			_cardsToPreloadImageStarted = _cardsToPreloadImage;

			foreach (var card in _cardsToPreloadImageStarted)
			{
				if (_cardsToPreloadImage != _cardsToPreloadImageStarted)
					break;

				card.PreloadImage(Ui);
			}
		}

		private void preloadImageThread()
		{
			try
			{
				while (true)
					preloadImageLoopIteration();
			}
			catch (ThreadAbortException)
			{
			}
		}

		public UiModel Ui { get; set; }
	}
}