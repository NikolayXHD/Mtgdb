using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	public class ImagePreloadingSubsystem
	{
		public ImagePreloadingSubsystem(
			MtgLayoutView layoutViewCards,
			MtgLayoutView layoutViewDeck,
			ScrollSubsystem scrollSubsystem,
			UiConfigRepository uiConfigRepository)
		{
			_layoutViewCards = layoutViewCards;
			_layoutViewDeck = layoutViewDeck;
			_scrollSubsystem = scrollSubsystem;
			_uiConfigRepository = uiConfigRepository;
		}

		public void Reset()
		{
			var cardsToPreloadImage = new List<Card>();
			addCardsToPreview(_layoutViewDeck, cardsToPreloadImage);
			addCardsToPreview(_layoutViewCards, cardsToPreloadImage);

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

		private void addCardsToPreview(MtgLayoutView view, List<Card> cardsToPreloadImage)
		{
			var pageSize = _scrollSubsystem.GetPageSize(view);
			
			int visibleRecordIndex = view.VisibleRecordIndex;

			bool endReached = false;
			bool startReached = false;
			int i = 0;

			while (true)
			{
				if (startReached && endReached)
					return;

				int maxPreload = Math.Min(_uiConfigRepository.Config.ImageCacheCapacity * 3 / 4 - pageSize, pageSize * 10);
				if (cardsToPreloadImage.Count > maxPreload)
					return;

				startReached |= !add(visibleRecordIndex - i - 1);
				endReached |= !add(visibleRecordIndex + pageSize + i);

				i++;
			}

			bool add(int j)
			{
				if (j < 0 || j >= view.RowCount)
					return false;

				int handle = view.GetVisibleRowHandle(j);
				var card = (Card) view.FindRow(handle);

				if (card == null)
					return false;

				cardsToPreloadImage.Add(card);
				return true;
			}
		}

		public UiModel Ui { get; set; }
		private CancellationTokenSource _cts;

		private readonly MtgLayoutView _layoutViewCards;
		private readonly MtgLayoutView _layoutViewDeck;
		private readonly ScrollSubsystem _scrollSubsystem;
		private readonly UiConfigRepository _uiConfigRepository;

		private List<Card> _cardsToPreloadImage;
		private List<Card> _cardsToPreloadImageStarted;
	}
}