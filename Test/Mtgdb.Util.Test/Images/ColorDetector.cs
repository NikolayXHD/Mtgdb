using System.Drawing;

namespace Mtgdb.Util
{
	public class ColorDetector : BmpProcessor
	{
		public ColorDetector(Bitmap bmp, bool[] detectedColors) : base(bmp)
		{
			_detectedColors = detectedColors;
		}

		protected override void ExecuteRaw()
		{
			for (int x = 0; x < Rect.Width; x++)
				for (int y = 0; y < Rect.Height; y++)
				{
					int l = GetLocation(x, y);
					var r = RgbValues[l];
					var g = RgbValues[l + 1];
					var b = RgbValues[l + 2];

					_detectedColors[r * 0x10000 + g * 0x100 + b] = true;
				}
		}

		private readonly bool[] _detectedColors;
	}
}