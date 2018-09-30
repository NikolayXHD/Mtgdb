using System.Collections;
using System.Drawing;

namespace Mtgdb.Util
{
	public class ColorDetector : BmpProcessor
	{
		public ColorDetector(Bitmap bmp, BitArray detectedColors) : base(bmp)
		{
			_detectedColors = detectedColors;
		}

		protected override void ExecuteRaw()
		{
			for (int x = 0; x < Rect.Width; x++)
				for (int y = 0; y < Rect.Height; y++)
				{
					int l = GetLocation(x, y);
					var g = BgraValues[l + G];
					var b = BgraValues[l + B];
					var r = BgraValues[l + R];

					_detectedColors[(b << 16) | (g << 8) | r] = true;
				}
		}

		private readonly BitArray _detectedColors;
	}
}