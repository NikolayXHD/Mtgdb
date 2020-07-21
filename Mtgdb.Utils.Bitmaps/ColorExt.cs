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

		private static Rgb toRgb(this HsvColor c)
		{
			float r;
			float g;
			float b;

			if (c.S.Equals(0))
				r = g = b = c.V; // achromatic
			else
			{
				var q = c.V < 0.5
					? c.V * (1 + c.S)
					: c.V + c.S - c.V * c.S;
				var p = 2 * c.V - q;
				r = hue2Rgb(p, q, c.H + 1 / 3f);
				g = hue2Rgb(p, q, c.H);
				b = hue2Rgb(p, q, c.H - 1 / 3f);
			}

			return new Rgb(toByte(r), toByte(g), toByte(b));

			static float hue2Rgb(float p, float q, float t)
			{
				if (t < 0)
					t++;

				if (t > 1)
					t--;

				if (t < 1 / 6f)
					return p + (q - p) * 6 * t;

				if (t < 1 / 2f)
					return q;

				if (t < 2 / 3f)
					return p + (q - p) * (2 / 3f - t) * 6;

				return p;
			}

			byte toByte(float x) =>
				(byte) Math.Floor(x * 256).AtMost(255f);
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

			float max = Math.Max(r, Math.Max(g, b));
			float min = Math.Min(r, Math.Min(g, b));

			float h;
			float s;
			float l = (max + min) / 2;

			if (max.Equals(min))
				h = s = 0; // achromatic
			else
			{
				float d = max - min;
				s = l > 0.5
					? d / (2 - max - min)
					: d / (max + min);

				if (max.Equals(r))
					h = (g - b) / d + (g < b ? 6 : 0);
				else if (max.Equals(g))
					h = (b - r) / d + 2;
				else // if (max.Equals(b))
					h = (r - g) / d + 4;

				h /= 6;
			}

			return new HsvColor(h, s, l);
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

		public static HsvColor ToPerceptedHsv(this Color rgb)
		{
			HsvColor hsv = rgb.ToHsv();
			return hsv.Transform(v: v => perceptedV(rgb, v), s: s => perceptedS(hsv, s));
		}

		public static HsvColor ToActualHsv(this HsvColor hsl, Color rgb) =>
			hsl.Transform(v: v => actualV(rgb, v), s: s=> actualS(hsl, s));

		private static float perceptedV(Color rgb, float v) =>
			gammaTransform(v, gamma(rgb));

		private static float actualV(Color rgb, float percepted) =>
			gammaTransform(percepted, 1f / gamma(rgb));

		private static float perceptedS(HsvColor hsv, float s)
		{
			// black v - 0 white v = 1
			// for this value we cannot perceive color => visible saturation = 0
			// at v = 0.5 we get pure R, G, B colors => visible saturation = s
			float q = 1f - 2f * Math.Abs(0.5f - hsv.V);
			return s * q;
		}

		private static float actualS(HsvColor hsv, float s)
		{
			// black v - 0 white v = 1
			// for this value we cannot perceive color => visible saturation = 0
			// at v = 0.5 we get pure R, G, B colors => visible saturation = s
			float q = 1f - 2f * Math.Abs(0.5f - hsv.V);
			if (q == 0f)
				return 0;

			return s / q;
		}

		private static float gammaTransform(float v, float gamma)
		{
			float l0 = gamma / (gamma + 1);
			if (v < l0)
				return v / gamma;

			return 1 + gamma * (v - 1);
		}

		private static float gamma(Color c)
		{
			if (c.R == c.G && c.G == c.B)
				return 1;

			const float r = 0.8f;
			const float g = 1.75f;
			const float b = 0.45f;

			return (c.R + c.G + c.B) / ((c.R * r) + (c.G * g) + (c.B * b));
		}
	}

	internal readonly struct Rgb
	{
		public readonly byte R;
		public readonly byte G;
		public readonly byte B;

		public Rgb(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
		}
	}
}
