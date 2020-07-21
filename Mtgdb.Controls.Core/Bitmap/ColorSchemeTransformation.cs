using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class ColorSchemeTransformation : BmpProcessor
	{
		private readonly HsvColor _textColor = SystemColors.WindowText.ToPerceptedHsv();
		private readonly HsvColor _bgColor = SystemColors.Window.ToPerceptedHsv();

		private const float MaxIconSaturationToInvert = 0.05f;

		public ColorSchemeTransformation(Bitmap bmp) : base(bmp)
		{
		}

		public bool IgnoreSaturation { get; set; }
		public bool AsBackground { get; set; }

		/// <summary> for test </summary>
		internal ColorSchemeTransformation(HsvColor text, HsvColor bg, Bitmap bmp = null)
			: base(bmp)
		{
			_textColor = text;
			_bgColor = bg;
		}

		protected override void ExecuteRaw()
		{
			if (!IgnoreSaturation)
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
			}

			ImageChanged = true;
			for (int i = Rect.Left; i < Rect.Right; i++)
			for (int j = Rect.Top; j < Rect.Bottom; j++)
				Transform(BgraValues, GetLocation(i, j));
		}

		public Color TransformColor(Color c)
		{
			if (c.IsSystemColor)
				return c;

			var hsv = c.ToHsv();
			if (!IgnoreSaturation)
			{
				if (hsv.S > MaxIconSaturationToInvert)
					return c;
			}

			return Color.FromArgb(c.A,
				c
					.ToPerceptedHsv()
					.Transform(v: tV)
					.ToActualHsv(c)
					.ToRgb()
			);
		}

		public void Transform(byte[] bgraValues, int location) =>
			Transform(bgraValues.ToHsv(location))
				.ToBgra(bgraValues, location);

		/// <summary> for test </summary>
		internal HsvColor Transform(HsvColor hsv) =>
			hsv.Transform(v: tV);

		private float tV(HsvColor c)
		{
			float delta = _bgColor.V - _textColor.V;
			if (Math.Abs(delta) < 0.01)
				return c.V;

			if (AsBackground)
				return _bgColor.V - delta * (1 - c.V);
			return _textColor.V + delta * c.V;
		}
	}
}
