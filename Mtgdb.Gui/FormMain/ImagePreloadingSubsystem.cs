using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Dal;
using NLog;

namespace Mtgdb.Gui
{
	public class ImagePreloadingSubsystem
	{
		private readonly MtgLayoutView _layoutViewCards;
		private readonly MtgLayoutView _layoutViewDeck;
		private readonly ScrollSubsystem _scrollSubsystem;

		private List<Card> _cardsToPreloadImage;
		private List<Card> _cardsToPreloadImageStarted;
		private readonly int _imageCacheCapacity;

		public ImagePreloadingSubsystem(MtgLayoutView layoutViewCards, MtgLayoutView layoutViewDeck, ScrollSubsystem scrollSubsystem, ImageCacheConfig imageCacheConfig)
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
			if (_cts != null && !_cts.IsCancellationRequested)
				throw new InvalidOperationException("Already started");

			var cts = new CancellationTokenSource();
			TaskEx.Run(async () =>
			{
				while (!cts.IsCancellationRequested)
				{
					if (_cardsToPreloadImage == null || _cardsToPreloadImage == _cardsToPreloadImageStarted)
					{
						await TaskEx.Delay(200);
						continue;
					}

					_cardsToPreloadImageStarted = _cardsToPreloadImage;

					foreach (var card in _cardsToPreloadImageStarted)
					{
						if (_cardsToPreloadImage != _cardsToPreloadImageStarted)
							break;

						card.PreloadImage(Ui);
					}
				}
			});

			_cts = cts;
		}

		public void AbortThread() =>
			_cts?.Cancel();

		private List<Card> getCardsToPreview(MtgLayoutView view)
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

		private static bool preload(List<Card> cardsToPreloadImage, MtgLayoutView view, int i)
		{
			if (i < 0 || i >= view.RowCount)
				return false;

			int handle = view.GetVisibleRowHandle(i);
			var card = (Card) view.FindRow(handle);

			if (card == null)
				return false;

			cardsToPreloadImage.Add(card);
			return true;
		}

		public UiModel Ui { get; set; }
		private CancellationTokenSource _cts;
	}
}