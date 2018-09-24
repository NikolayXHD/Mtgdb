using System.Drawing;

namespace Mtgdb.Controls
{
	public class AdaptBrightnessTransformation : BmpProcessor
	{
		private const float MaxSaturation = 0.075f;
		private const float MinValue = 0.35f;

		public AdaptBrightnessTransformation(Bitmap bmp) : base(bmp)
		{
		}

		protected override void ExecuteRaw()
		{
			if (!isTransformationNecessary())
				return;

			float meanSaturation = 0;
			long alphaSum = 0;

			for (int i = Rect.Left; i < Rect.Right; i++)
				for (int j = Rect.Top; j < Rect.Bottom; j++)
				{
					int location = GetLocation(i, j);
					byte alpha = RgbValues[location + 3];
					meanSaturation += RgbValues.ToHsv(location).S * alpha;
					alphaSum += alpha;
				}

			meanSaturation /= alphaSum;

			if (meanSaturation > MaxSaturation)
				return;

			ImageChanged = true;
			for (int i = Rect.Left; i < Rect.Right; i++)
				for (int j = Rect.Top; j < Rect.Bottom; j++)
				{
					int location = GetLocation(i, j);

					RgbValues.ToHsv(location)
						.Transform(v: _ => 1f - _)
						.ToRgb(RgbValues, location);
				}
		}

		private static bool isTransformationNecessary() =>
			SystemColors.Control.ToHsv().V < MinValue;

		public static Color Transform(Color c)
		{
			if (!isTransformationNecessary())
				return c;

			var hsv = c.ToHsv();
			if (hsv.S > MaxSaturation)
				return c;

			return Color.FromArgb(c.A, hsv.Transform(v: _ => 1 - _).ToRgb());
		}
	}
}