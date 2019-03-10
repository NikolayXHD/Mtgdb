using System.Drawing;

namespace Mtgdb.Controls
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

			for (int counter = 0; counter < BgraValues.Length; counter += BytesPerPixel)
				BgraValues[counter + A] = (byte) (BgraValues[counter + A] * _opacity);
		}

		private readonly float _opacity;
	}
}