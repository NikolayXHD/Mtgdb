using System;
using System.Drawing;

namespace Mtgdb
{
	public static class ColorExt
	{
		public static Color ToRgb(this HsvColor c)
		{
			var rgb = c.toRgb();
			return Color.FromArgb(rgb.R, rgb.G, rgb.B);
		}


		public static void ToBgra(this HsvColor c, byte[] rgbArr, int i)
		{
			var rgb = c.toRgb();
			rgbArr[i + BmpProcessor.B] = rgb.B;
			rgbArr[i + BmpProcessor.G] = rgb.G;
			rgbArr[i + BmpProcessor.R] = rgb.R;
		}

		private static (byte R, byte G, byte B) toRgb(this HsvColor c)
		{
			float h = c.H;
			float s = c.S;
			float v = c.V;

			float r, g, b;
			if (v <= 0)
				r = g = b = 0;
			else if (s <= 0)
				r = g = b = v;
			else
			{
				float hf = h / 60f;
				int i = (int) Math.Floor(hf);
				float f = hf - i;
				float pv = v * (1 - s);
				float qv = v * (1 - s * f);
				float tv = v * (1 - s * (1 - f));
				switch (i)
				{
					// Red is the dominant color

					case 0:
						r = v;
						g = tv;
						b = pv;
						break;

					// Green is the dominant color

					case 1:
						r = qv;
						g = v;
						b = pv;
						break;
					case 2:
						r = pv;
						g = v;
						b = tv;
						break;

					// Blue is the dominant color

					case 3:
						r = pv;
						g = qv;
						b = v;
						break;
					case 4:
						r = tv;
						g = pv;
						b = v;
						break;

					// Red is the dominant color

					case 5:
						r = v;
						g = pv;
						b = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						r = v;
						g = tv;
						b = pv;
						break;
					case -1:
						r = v;
						g = pv;
						b = qv;
						break;

					// The color is not defined, we should throw an error.

					default:
						// ("i Value error in Pixel conversion, Value is %d", i);
						r = g = b = v; // Just pretend its black/white
						break;
				}
			}

			byte toByte(float val) => (byte) Math.Round(val.WithinRange(0, 1) * 255f);

			return (toByte(r), toByte(g), toByte(b));
		}

		public static float RotationTo(this Color c, Color t) =>
			t.ToHsv().H - c.ToHsv().H;

		public static HsvColor ToHsv(this Color c) =>
			toHsv(c.R, c.G, c.B);

		public static HsvColor ToHsv(this byte[] bgraArr, int i) =>
			toHsv(bgraArr[i + BmpProcessor.R], bgraArr[i + BmpProcessor.G], bgraArr[i + BmpProcessor.B]);

		private static HsvColor toHsv(int rB, int gB, int bB)
		{
			float r = rB / 255f;
			float g = gB / 255f;
			float b = bB / 255f;

			float v = r.AtLeast(g).AtLeast(b);
			float min = r.AtMost(g).AtMost(b);

			float h, s;
			float d = v - min;

			if (v == min)
				h = 0;
			else if (v == r)
				h = (g - b) / d % 6 * 60;
			else if (v == g)
				h = ((b - r) / d + 2) * 60;
			else // v == b
				h = ((r - g) / d + 4) * 60;

			if (v == 0)
				s = 0;
			else
				s = d / v;

			return new HsvColor(h, s, v);
		}

		public static Color TransformHsv(this Color c, Func<float, float> h = null, Func<float, float> s = null, Func<float, float> v = null) =>
			c.ToHsv().Transform(h, s, v).ToRgb();

		public static Color BlendWith(this Color bg, Color fore, int opacity)
		{
			if (bg == Color.Empty || bg == Color.Transparent)
				return Color.FromArgb(opacity, fore);

			byte o1 = bg.A;
			return Color.FromArgb(
				(255 * 255 - (255 - o1) * (255 - opacity)) / 255,
				blendBytes(bg.R, fore.R),
				blendBytes(bg.G, fore.G),
				blendBytes(bg.B, fore.B));

			int blendBytes(byte b1, byte b2)
			{
				int share2 = 255 * opacity;
				int share1 = o1 * (255 - opacity);

				return (b1 * share1 + b2 * share2) / (share1 + share2);
			}
		}
	}
}