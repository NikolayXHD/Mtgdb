using System;
using System.Drawing;

namespace Mtgdb.Dal
{
	internal class BmpFrameDetector : BmpProcessor
	{
		public BmpFrameDetector(Bitmap bmp, Size sizeCropped)
			: base(bmp)
		{
			_sizeCropped = sizeCropped;
		}

		protected override void ExecuteRaw()
		{
			int middleTop = GetLocation(Rect.Width / 2, 0);
			int rightMiddle = GetLocation(Rect.Width - 1, Rect.Height / 2);
			int middleBottom = GetLocation(Rect.Width / 2, Rect.Height - 1);
			int leftMiddle = GetLocation(0, Rect.Height / 2);

			if (!SameColor(middleTop, rightMiddle))
				return;

			if (!SameColor(middleTop, middleBottom))
				return;

			if (!SameColor(middleTop, leftMiddle))
				return;


			int frameHeight = 0;
			int frameWidth = 0;

			// More than x2 reserve. On practice /25
			int cornerSize = Rect.Width / 10;
			int maxFrame = Rect.Width / 5;

			for (int i = 0; i < maxFrame; i++)
			{
				if (!isRectangleSameColor(middleTop, new Rectangle(i, cornerSize, 1, Rect.Height - 2 * cornerSize)) ||
					!isRectangleSameColor(middleTop, new Rectangle(Rect.Width - 1 - i, cornerSize, 1, Rect.Height - 2 * cornerSize)))
				{
					frameWidth = i;
					break;
				}
			}

			for (int j = 0; j < maxFrame; j++)
			{
				if (!isRectangleSameColor(middleTop, new Rectangle(cornerSize, j, Rect.Width - 2 * cornerSize, 1)) ||
					!isRectangleSameColor(middleTop, new Rectangle(cornerSize, Rect.Height - 1 - j, Rect.Width - 2 * cornerSize, 1)))
				{
					frameHeight = j;
					break;
				}
			}

			var sizeCropped = _sizeCropped;
			float proportion = (float) sizeCropped.Height / sizeCropped.Width;
			float resultProportion = (float) (Rect.Size.Height - frameHeight) / (Rect.Size.Width - frameWidth);

			if (resultProportion > proportion)
				frameWidth = Math.Max(0, Rect.Width - (int) Math.Round((Rect.Height - frameHeight) / proportion));
			else if (resultProportion < proportion)
				frameHeight = Math.Max(0, Rect.Height - (int) Math.Round((Rect.Width - frameWidth) * proportion));

			Frame = new Size(frameWidth, frameHeight);
		}

		private bool isRectangleSameColor(int originalColorLocation, Rectangle rect)
		{
			for (int i = rect.Left; i < rect.Right; i++)
				for (int j = rect.Top; j < rect.Bottom; j++)
					if (!SameColor(GetLocation(i, j), originalColorLocation))
						return false;

			return true;
		}

		public Size Frame { get; private set; }
		protected override int ColorSimilarityThreshold => 40;
		private readonly Size _sizeCropped;
	}
}