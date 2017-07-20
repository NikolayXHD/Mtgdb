using System;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class ScrollSubsystem
	{
		public event Action<LayoutView> Scrolled;

		private readonly LayoutView _layoutViewDeck;
		private readonly LayoutView _layoutViewCards;

		public ScrollSubsystem(
			LayoutView layoutViewDeck,
			LayoutView layoutViewCards)
		{
			_layoutViewDeck = layoutViewDeck;
			_layoutViewCards = layoutViewCards;
		}

		public void SubscribeToEvents()
		{
			
			_layoutViewCards.VisibleRecordIndexChanged += visibleRecordIndexChanged;
			_layoutViewDeck.VisibleRecordIndexChanged += visibleRecordIndexChanged;
		}

		public void UnsubscribeFromEvents()
		{
			

			_layoutViewCards.VisibleRecordIndexChanged -= visibleRecordIndexChanged;
			_layoutViewDeck.VisibleRecordIndexChanged -= visibleRecordIndexChanged;
		}

		private void visibleRecordIndexChanged(object sender)
		{
			var layoutView = getView(sender);
			Scrolled?.Invoke(layoutView);
		}

		

		public int GetPageSize(LayoutView view)
		{
			var visibleColumnsCount = getVisibleColumnsCount(view);
			var visibleRowsCount = getVisibleRowsCount(view);
			return visibleColumnsCount*visibleRowsCount;
		}

		private static int getVisibleColumnsCount(LayoutView view)
		{
			return getVisibleCount(
				view.Control.Width - view.ScrollWidth,
				view.CardMinSize.Width,
				view.CardHorzInterval,
				view.PartialCardHorizontal,
				view.AllowPartialCards);
		}

		private static int getVisibleRowsCount(LayoutView view)
		{
			return getVisibleCount(
				view.Control.Height,
				view.CardMinSize.Height,
				view.CardVertInterval,
				view.PartialCardVertical,
				view.AllowPartialCards);
		}

		

		public void EnsureCardVisibility(Card card, LayoutView view)
		{
			var index = getIndexInView(card, view);

			if (index < 0)
				return;

			var visibleCardsCount = GetPageSize(view);

			if (index >= view.VisibleRecordIndex && index < view.VisibleRecordIndex + visibleCardsCount)
				return;

			var screenIndex = index/visibleCardsCount;
			view.VisibleRecordIndex = Math.Min(view.RowCount - visibleCardsCount, screenIndex*visibleCardsCount);
		}

		private static int getIndexInView(Card card, LayoutView view)
		{
			var handle = view.FindRow(card);
			var index = view.GetVisibleIndex(handle);
			return index;
		}

		private static int getVisibleCount(int viewSize, int cardSize, int interval, int threshold, bool allowPartialCards)
		{
			var cardEffectiveSize = cardSize + interval;

			var wholeCardsCount = viewSize / cardEffectiveSize;
			var partialSize = viewSize % cardEffectiveSize;

			if (wholeCardsCount > 0 && (!allowPartialCards || partialSize <= threshold))
				return wholeCardsCount;

			return wholeCardsCount + 1;
		}



		private LayoutView getView(object view)
		{
			if (_layoutViewCards.Wraps(view))
				return _layoutViewCards;

			if (_layoutViewDeck.Wraps(view))
				return _layoutViewDeck;

			throw new Exception(@"wrapper not found");
		}
	}
}