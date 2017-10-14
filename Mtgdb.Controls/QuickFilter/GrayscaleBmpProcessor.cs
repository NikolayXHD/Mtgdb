using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	class GrayscaleBmpProcessor : BmpProcessor
	{
		public GrayscaleBmpProcessor(Bitmap bmp, float colorFactor, float whiteFactor, float grayFactor, float opacity)
			: base(bmp)
		{
			_colorFactor = colorFactor;
			_whiteFactor = whiteFactor;
			_grayFactor = grayFactor;
			_opacity = opacity;
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			float white = Byte.MaxValue * _whiteFactor;
			var transparent = (byte)(Byte.MaxValue * _opacity);
			for (int counter = 0; counter < RgbValues.Length; counter += 4)
			{
				var min = white + _grayFactor * (RgbValues[counter] + RgbValues[counter + 1] + RgbValues[counter + 2]) / 3f;

				RgbValues[counter] = (byte)(min + RgbValues[counter] * _colorFactor);
				RgbValues[counter + 1] = (byte)(min + RgbValues[counter + 1] * _colorFactor);
				RgbValues[counter + 2] = (byte)(min + RgbValues[counter + 2] * _colorFactor);
				RgbValues[counter + 3] = Math.Min(transparent, RgbValues[counter + 3]);
			}
		}

		private readonly float _colorFactor;
		private readonly float _whiteFactor;
		private readonly float _grayFactor;
		private readonly float _opacity;
	}
}