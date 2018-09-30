using System.Drawing;

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

					var br = BgraValues[l + B] + BgraValues[l + G] + BgraValues[l + R];

					if (br > maxBr)
						maxBr = br;

					if (br < minBr)
						minBr = br;
				}

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					int l = GetLocation(x, y);

					var br = BgraValues[l + B] + BgraValues[l + G] + BgraValues[l + R];

					byte val = br > minBr + (maxBr - minBr) * _threshold ? (byte) 255 : (byte) 0;
					BgraValues[l + B] = BgraValues[l + G] = BgraValues[l + R] = val;
				}
		}

		private readonly float _threshold;
	}
}