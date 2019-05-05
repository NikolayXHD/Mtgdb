using System;
using System.Drawing;

namespace Mtgdb.Data
{
	internal class BmpCornerRemoval : BmpProcessor
	{
		public BmpCornerRemoval(Bitmap bmp, bool force)
			: base(bmp)
		{
			_force = force;
		}

		protected override void ExecuteRaw()
		{
			int size = Math.Max(Rect.Width, Rect.Height);

			if (!_force)
			{
				int margin = (int) Math.Ceiling(size / 150f);
				int ltTop = GetLocation(0, 0);

				if (SameColor(ltTop, 0, 0, 0, 0))
					return;

				int mdTop = GetLocation(Rect.Width / 2, margin);
				int rtTop = GetLocation(Rect.Width - 1, 0);
				int rtMdl = GetLocation(Rect.Width - 1 - margin, Rect.Height / 2);
				int rtBtm = GetLocation(Rect.Width - 1, Rect.Height - 1);
				int mdBtm = GetLocation(Rect.Width / 2, Rect.Height - 1 - margin);
				int ltBtm = GetLocation(0, Rect.Height - 1);
				int ltMdl = GetLocation(margin, Rect.Height / 2);

				bool hasCorner =
					SameColor(ltTop, rtTop) && SameColor(rtTop, rtBtm) && SameColor(rtBtm, ltBtm) &&
					(
						!SameColor(ltTop, mdTop) ||
						!SameColor(ltTop, rtMdl) ||
						!SameColor(ltTop, mdBtm) ||
						!SameColor(ltTop, ltMdl));

				if (!hasCorner)
					return;
			}

			ImageChanged = true;

			double radius = 13f / 370f * size;

			modify(+radius, +radius, 0, 0);
			modify(+radius, -radius, 0, Rect.Height - 1);
			modify(-radius, +radius, Rect.Width - 1, 0);
			modify(-radius, -radius, Rect.Width - 1, Rect.Height - 1);
		}

		private void modify(
			double radiusX,
			double radiusY,
			int left,
			int top)
		{
			//int modified = 0;

			int signX = Math.Sign(radiusX);
			int signY = Math.Sign(radiusY);
			double r = Math.Round(Math.Abs(radiusX));

			makeTransparentSmooth(left, top, r, signX, signY);
		}

		private void makeTransparentSmooth(int left, int top, double r, int signX, int signY)
		{
			for (int x = 0; x < r + 2; x++)
			{
				double maxY = r + 2 - Math.Sqrt(r * r - (x - r) * (x - r));
				for (int y = 0; y < maxY; y++)
				{
					int i = left + signX * x;
					int j = top + signY * y;
					int counter = GetLocation(i, j);

					double alphaRel = getAlphaRel(x, y, r);

					if (alphaRel >= 1)
						continue;

					byte alpha = (byte) (255 * alphaRel);
					BgraValues[counter + A] = alpha;
				}
			}
		}

		private static double getAlphaRel(int x, int y, double r)
		{
			double dist = Math.Sqrt((x - r) * (x - r) + (y - r) * (y - r));

			double outness = dist - r - 0.5;

			if (outness > 1)
				outness = 1;

			double alphaRel = 1 - outness;
			return alphaRel;
		}

		protected override int ColorSimilarityThreshold => 60;
		private readonly bool _force;
	}
}