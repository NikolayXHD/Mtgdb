using System.Drawing;

namespace Mtgdb.Controls
{
	public class ColorSchemeTransformation : BmpProcessor
	{
		private readonly HsvColor _textColor = SystemColors.WindowText.ToHsv();
		private readonly HsvColor _bgColor = SystemColors.Window.ToHsv();

		private const float MaxIconSaturationToInvert = 0.05f;

		public ColorSchemeTransformation(Bitmap bmp) : base(bmp)
		{
		}

		/// <summary> for test </summary>
		internal ColorSchemeTransformation(HsvColor text, HsvColor bg, Bitmap bmp = null)
			: base(bmp)
		{
			_textColor = text;
			_bgColor = bg;
		}

		protected override void ExecuteRaw()
		{
			float meanSaturation = 0;
			long alphaSum = 0;

			for (int i = Rect.Left; i < Rect.Right; i++)
				for (int j = Rect.Top; j < Rect.Bottom; j++)
				{
					int location = GetLocation(i, j);
					byte alpha = BgraValues[location + A];
					meanSaturation += BgraValues.ToHsv(location).S * alpha;
					alphaSum += alpha;
				}

			meanSaturation /= alphaSum;

			if (meanSaturation > MaxIconSaturationToInvert)
				return;

			ImageChanged = true;
			for (int i = Rect.Left; i < Rect.Right; i++)
				for (int j = Rect.Top; j < Rect.Bottom; j++)
					Transform(BgraValues, GetLocation(i, j));
		}

		public Color Transform(Color c)
		{
			if (c.IsSystemColor)
				return c;

			var hsv = c.ToHsv();
			if (hsv.S > MaxIconSaturationToInvert)
				return c;

			return Color.FromArgb(c.A, Transform(hsv).ToRgb());
		}

		public void Transform(byte[] bgraValues, int location) =>
			Transform(bgraValues.ToHsv(location))
				.ToBgra(bgraValues, location);

		/// <summary> for test </summary>
		internal HsvColor Transform(HsvColor hsv) =>
			hsv.Transform(tH, tS, tV);

		private float tH(HsvColor c)
		{
			float left = _textColor.H;
			float right = _bgColor.H;
			right = (right - left + 180).Modulo(360) - 180 + left;

			return left + (right - left) * c.V;
		}

		private float tS(HsvColor c) =>
			_textColor.S + (_bgColor.S - _textColor.S) * c.V;

		private float tV(HsvColor c) =>
			_textColor.V + (_bgColor.V - _textColor.V) * c.V;
	}
}