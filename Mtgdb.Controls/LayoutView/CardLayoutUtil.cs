namespace Mtgdb.Controls
{
	public static class CardLayoutUtil
	{
		public static int GetVisibleCount(int viewSize, int cardSize, int interval, int threshold, bool allowPartialCards)
		{
			var cardEffectiveSize = cardSize + interval;

			var wholeCardsCount = viewSize / cardEffectiveSize;
			var partialSize = viewSize % cardEffectiveSize;

			if (wholeCardsCount > 0 && (!allowPartialCards || partialSize <= threshold))
				return wholeCardsCount;

			return wholeCardsCount + 1;
		}
	}
}