using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class SemiTransparentShadowTransformation : BmpProcessor
	{
		public SemiTransparentShadowTransformation(Bitmap bmp, Color backColor, float opaqueBorder, float fadeBorder) : base(bmp)
		{
			_backColor = backColor;
			_opaqueBorder = opaqueBorder;
			_fadeBorder = fadeBorder;
		}

		protected override void ExecuteRaw()
		{
			var opacityMask = new float[Rect.Width, Rect.Height];

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					var d = nearestFeatureDist(x, y);
					opacityMask[x, y] = Math.Max(0, Math.Min(1, 1f - (d - _opaqueBorder) / (_fadeBorder - _opaqueBorder)));
				}

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					int l = GetLocation(x, y);

					float opacity = opacityMask[x, y];
					if (opacity != 1f)
					{
						ImageChanged = true;
						BgraValues[l + A] = (byte)(BgraValues[l + A] * opacity);
					}
				}
		}

		private float nearestFeatureDist(int x0, int y0)
		{
			int minX = Math.Max(0, (int) Math.Floor(x0 - _fadeBorder));
			int maxX = Math.Min(Rect.Width - 1, (int) Math.Ceiling(x0 + _fadeBorder));

			int minY = Math.Max(0, (int) Math.Floor(y0 - _fadeBorder));
			int maxY = Math.Min(Rect.Height - 1, (int) Math.Ceiling(y0 + _fadeBorder));

			float result = float.MaxValue;

			for (int x = minX; x <= maxX; x++)
			{
				float maxD = _fadeBorder * _fadeBorder;

				for (int y = minY; y <= maxY; y++)
				{
					int dx = x - x0;
					int dy = y - y0;

					int d = dx * dx + dy * dy;

					if (d > maxD)
						continue;

					int l = GetLocation(x, y);

					if (
						BgraValues[l + B] == _backColor.B &&
						BgraValues[l + G] == _backColor.G &&
						BgraValues[l + R] == _backColor.R &&
						BgraValues[l + A] == _backColor.A)
						continue;

					if (
						BgraValues[l + B] == 0 &&
						BgraValues[l + G] == 0 &&
						BgraValues[l + R] == 0 &&
						BgraValues[l + A] == 0)
						continue;

					if (d < result)
						result = d;

					if (result == 0f)
						return 0f;
				}
			}

			if (result == float.MaxValue)
				return _fadeBorder;

			return (float) Math.Sqrt(result);
		}

		private readonly Color _backColor;
		private readonly float _fadeBorder;
		private readonly float _opaqueBorder;
	}
}