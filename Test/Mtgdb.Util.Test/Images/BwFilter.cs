using System.Drawing;
using Mtgdb.Bitmaps;

namespace Mtgdb.Util
{
	public class BwFilter : BmpProcessor
	{
		public BwFilter(Bitmap bmp, float threshold) : base(bmp)
		{
			_threshold = threshold;
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			int minBr = int.MaxValue;
			int maxBr = int.MinValue;

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					int l = GetLocation(x, y);

					var br = RgbValues[l] + RgbValues[l + 1] + RgbValues[l + 2];

					if (br > maxBr)
						maxBr = br;

					if (br < minBr)
						minBr = br;
				}

			//int countDark = 0;
			//int countBright = 0;

			//float foregroundDetectionThreshold = 0.03f;

			//for (int x = 0; x < Rect.Width; ++x)
			//	for (int y = 0; y < Rect.Height; ++y)
			//	{
			//		int l = GetLocation(x, y);

			//		var br = RgbValues[l] + RgbValues[l + 1] + RgbValues[l + 2];

			//		if (br < minBr + (maxBr - minBr) * foregroundDetectionThreshold)
			//			countDark++;
			//		else if (br > minBr + (maxBr - minBr) * (1 - foregroundDetectionThreshold))
			//			countBright++;
			//	}

			//float threshold;
			//if (countDark < countBright)
			//	threshold = _threshold;
			//else
			//	threshold = 1 - _threshold;

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					int l = GetLocation(x, y);

					var br = RgbValues[l] + RgbValues[l + 1] + RgbValues[l + 2];

					if (br > minBr + (maxBr - minBr) * _threshold)
						RgbValues[l] = RgbValues[l + 1] = RgbValues[l + 2] = 255;
					else
						RgbValues[l] = RgbValues[l + 1] = RgbValues[l + 2] = 0;
				}
		}

		private readonly float _threshold;
	}
}