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

			var bgr = new List<byte>(3);

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					int l = GetLocation(x, y);

					bgr.Clear();
					bgr.Add(BgraValues[l + B]);
					bgr.Add(BgraValues[l + G]);
					bgr.Add(BgraValues[l + R]);

					bgr.Sort();

					BgraValues[l + B] = BgraValues[l + G] = BgraValues[l + R] =
						(byte) ((bgr[2] * 2 - bgr[1] - bgr[0]) / 2);
				}
		}
	}
}