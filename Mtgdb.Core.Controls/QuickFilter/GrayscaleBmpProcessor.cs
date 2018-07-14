using System.Drawing;

namespace Mtgdb.Bitmaps
{
	public class GrayscaleBmpProcessor : BmpProcessor
	{
		public GrayscaleBmpProcessor(Bitmap bmp, float opacity)
			: base(bmp)
		{
			_opacity = opacity;
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			for (int counter = 0; counter < RgbValues.Length; counter += 4)
			{
				RgbValues[counter + 3] = (byte) (RgbValues[counter + 3] * _opacity);
			}
		}

		private readonly float _opacity;
	}
}