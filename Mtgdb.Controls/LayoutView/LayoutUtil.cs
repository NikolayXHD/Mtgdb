using System.Collections;
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

		public static void LayOutIn(this IList<ButtonLayout> elements, Rectangle containerBounds)
		{
			var start = new Dictionary<ContentAlignment, Point>
			{
				[ContentAlignment.TopLeft] = new Point(containerBounds.Left, containerBounds.Top),
				[ContentAlignment.BottomLeft] = new Point(containerBounds.Left, containerBounds.Bottom),
				[ContentAlignment.TopRight] = new Point(containerBounds.Right, containerBounds.Top),
				[ContentAlignment.BottomRight] = new Point(containerBounds.Right, containerBounds.Bottom)
			};

			var current = start.ToDictionary();

			var isFirst = new Dictionary<ContentAlignment, bool>
			{
				[ContentAlignment.TopLeft] = true,
				[ContentAlignment.BottomLeft] = true,
				[ContentAlignment.TopRight] = true,
				[ContentAlignment.BottomRight] = true
			};

			foreach (var element in elements)
			{
				if (element.Size == Size.Empty)
					continue;

				var alignment = element.Alignment;
				var location = current[alignment];

				if (isFirst[alignment])
				{
					location = location
						.Plus(element.Size.MultiplyBy(_firstSizeSign[alignment]))
						.Plus(element.Margin.MultiplyBy(_firstMarginSign[alignment]));

					isFirst[alignment] = false;
				}
				else
				{
					location = location.Plus(_deltaSign[alignment].MultiplyBy(element.Margin.Plus(element.Size)));
				}

				element.Location = location;

				if (element.BreaksLayout)
				{
					location = start[alignment].Plus(element.Size.MultiplyBy(_nextLineSign[alignment]));
					isFirst[alignment] = true;
				}

				current[alignment] = location;
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

		private static readonly Dictionary<ContentAlignment, Size> _nextLineSign =
			new Dictionary<ContentAlignment, Size>
			{
				[ContentAlignment.TopLeft] = new Size(0, 1),
				[ContentAlignment.BottomLeft] = new Size(0, -1),
				[ContentAlignment.TopRight] = new Size(0, 1),
				[ContentAlignment.BottomRight] = new Size(0, -1)
			};
	}
}