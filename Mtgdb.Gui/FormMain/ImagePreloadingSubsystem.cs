using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Controls;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	public class ImagePreloadingSubsystem
	{
		public ImagePreloadingSubsystem(
			LayoutViewControl viewCards,
			LayoutViewControl viewDeck,
			UiConfigRepository uiConfigRepository)
		{
			_viewCards = viewCards;
			_viewDeck = viewDeck;
			_uiConfigRepository = uiConfigRepository;
		}

		public void Reset()
		{
			var cardsToPreloadImage = new List<Card>();
			addCardsToPreview(_viewDeck, cardsToPreloadImage);
			addCardsToPreview(_viewCards, cardsToPreloadImage);

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

		private void addCardsToPreview(LayoutViewControl view, List<Card> cardsToPreloadImage)
		{
			var pageSize = view.GetPageSize();
			int index = view.CardIndex;

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

				startReached |= !add(index - i - 1);
				endReached |= !add(index + pageSize + i);

				i++;
			}

			bool add(int j)
			{
				if (j < 0 || j >= view.Count)
					return false;

				var card = (Card) view.FindRow(j);
				if (card == null)
					return false;

				cardsToPreloadImage.Add(card);
				return true;
			}
		}

		public UiModel Ui { get; set; }
		private CancellationTokenSource _cts;

		private readonly LayoutViewControl _viewCards;
		private readonly LayoutViewControl _viewDeck;
		private readonly UiConfigRepository _uiConfigRepository;

		private List<Card> _cardsToPreloadImage;
		private List<Card> _cardsToPreloadImageStarted;
	}
}