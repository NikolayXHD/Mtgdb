using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Util
{
	public class SaturationFilter : BmpProcessor
	{
		public SaturationFilter(Bitmap bmp) : base(bmp)
		{
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			var rgb = new List<byte>(3);

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					int l = GetLocation(x, y);

					rgb.Clear();
					rgb.Add(RgbValues[l]);
					rgb.Add(RgbValues[l + 1]);
					rgb.Add(RgbValues[l + 2]);
					
					rgb.Sort();

					RgbValues[l] = RgbValues[l + 1] = RgbValues[l + 2] =
						(byte) ((rgb[2] * 2 - rgb[1] - rgb[0]) / 2);
				}
		}
	}
}