using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Controls
{
	public static class LayoutUtil
	{
		public static int GetVisibleCardsCount(int viewSize, int cardSize, int interval, int threshold, bool allowPartialCards)
		{
			var cardEffectiveSize = cardSize + interval;

			var wholeCardsCount = viewSize / cardEffectiveSize;
			var partialSize = viewSize % cardEffectiveSize;

			if (wholeCardsCount > 0 && (!allowPartialCards || partialSize <= threshold))
				return wholeCardsCount;

			return wholeCardsCount + 1;
		}

		public static void LayOutIn(this IEnumerable<ILayoutElement> elements, Rectangle containerBounds)
		{
			var current = new Dictionary<ContentAlignment, Point>
			{
				[ContentAlignment.TopLeft] = new Point(containerBounds.Left, containerBounds.Top),
				[ContentAlignment.BottomLeft] = new Point(containerBounds.Left, containerBounds.Bottom),
				[ContentAlignment.TopRight] = new Point(containerBounds.Right, containerBounds.Top),
				[ContentAlignment.BottomRight] = new Point(containerBounds.Right, containerBounds.Bottom)
			};

			using (var enumerator = elements.GetEnumerator())
			{
				if (!enumerator.MoveNext())
					return;

				var element = enumerator.Current;

				if (element.Size != Size.Empty)
				{
					var alignment = element.Alignment;
					var location = current[alignment];
					
					location = location
						.Plus(element.Size.MultiplyBy(_firstSizeSign[alignment]))
						.Plus(element.Margin.MultiplyBy(_firstMarginSign[alignment]));
					
					element.Location = location;
					current[alignment] = location;
				}

				while (enumerator.MoveNext())
				{
					element = enumerator.Current;
					
					if (element.Size == Size.Empty)
						continue;
					
					var alignment = element.Alignment;
					var location = current[alignment];
						
					location = location.Plus(_deltaSign[alignment].MultiplyBy(element.Margin.Plus(element.Size)));
						
					element.Location = location;
					current[alignment] = location;
				}
			}
		}

		private static readonly Dictionary<ContentAlignment, Size> _firstMarginSign =
			new Dictionary<ContentAlignment, Size>
			{
				[ContentAlignment.TopLeft] = new Size(1, 1),
				[ContentAlignment.BottomLeft] = new Size(1, -1),
				[ContentAlignment.TopRight] = new Size(-1, 1),
				[ContentAlignment.BottomRight] = new Size(-1, -1)
			};

		private static readonly Dictionary<ContentAlignment, Size> _firstSizeSign =
			new Dictionary<ContentAlignment, Size>
			{
				[ContentAlignment.TopLeft] = new Size(0, 0),
				[ContentAlignment.BottomLeft] = new Size(0, -1),
				[ContentAlignment.TopRight] = new Size(-1, 0),
				[ContentAlignment.BottomRight] = new Size(-1, -1)
			};

		private static readonly Dictionary<ContentAlignment, Size> _deltaSign =
			new Dictionary<ContentAlignment, Size>
			{
				[ContentAlignment.TopLeft] = new Size(1, 0),
				[ContentAlignment.BottomLeft] = new Size(1, 0),
				[ContentAlignment.TopRight] = new Size(-1, 0),
				[ContentAlignment.BottomRight] = new Size(-1, 0)
			};
	}
}