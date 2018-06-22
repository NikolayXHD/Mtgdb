using System.Drawing;

namespace Mtgdb.Bitmaps
{
	public class BmpAlphaToBackgroundColorTransformation : BmpProcessor
	{
		public BmpAlphaToBackgroundColorTransformation(Bitmap bmp, Color bg) : base(bmp) =>
			_bg = new[] { bg.R, bg.G, bg.B };

		protected override void ExecuteRaw()
		{
			for (int i = 0; i < Rect.Width; i++)
				for (int j = 0; j < Rect.Height; j++)
				{
					var location = GetLocation(i, j);

					int alphaIndex = location + 3;

					byte a = RgbValues[alphaIndex];
					int bgProportion = byte.MaxValue - a;

					if (bgProportion == 0)
						continue;

					ImageChanged = true;
					
					RgbValues[alphaIndex] = byte.MaxValue;

					for (int c = 0; c < 3; c++)
					{
						RgbValues[location + c] = (byte) ((
								_bg[c] * bgProportion +
								RgbValues[location + c] * (byte.MaxValue - bgProportion)) /
							byte.MaxValue);
					}
				}
		}

		private readonly byte[] _bg;
	}
}