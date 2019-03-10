using System;
using System.Drawing;

namespace Mtgdb
{
	public class BmpScaler : BmpProcessor
	{
		public BmpScaler(Bitmap bmp, Bitmap original, Rectangle sourceRect) : base(bmp)
		{
			_original = original;
			_sourceRect = sourceRect;
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			float scaleX = (float) _sourceRect.Width / Rect.Width;
			float scaleY = (float) _sourceRect.Height / Rect.Height;

			using (var original = new BmpReader(_original, _sourceRect))
				for (int i = 0; i < Rect.Width; i++)
				{
					float left = i * scaleX;
					float right = left + scaleX;

					for (int j = 0; j < Rect.Height; j++)
					{
						float top = j * scaleY;
						float bottom = top + scaleY;

						var l = GetLocation(i, j);

						float r = 0f, g = 0f, b = 0f, a = 0f, w = 0f, wc = 0f, nr = 0f, ng = 0f, nb = 0f;

						int iOrMin = Math.Max((int) Math.Floor(left), 0);
						int iOrMax = Math.Min((int) Math.Ceiling(right), _sourceRect.Width);

						int jOrMin = Math.Max((int) Math.Floor(top), 0);
						int jOrMax = Math.Min((int) Math.Ceiling(bottom), _sourceRect.Height);

						for (int iOr = iOrMin; iOr < iOrMax; iOr++)
						{
							float wX = Math.Min(right, iOr + 1) - Math.Max(left, iOr);

							for (int jOr = jOrMin; jOr < jOrMax; jOr++)
							{
								float wY = Math.Min(bottom, jOr + 1) - Math.Max(top, jOr);

								var lOr = original.GetLocation(iOr, jOr);
								var valuesOr = original.BgraValues;

								byte db = valuesOr[lOr + B];
								byte dg = valuesOr[lOr + G];
								byte dr = valuesOr[lOr + R];
								byte da = valuesOr[lOr + A];

								float dw = wX * wY;
								float dwc = dw * da;

								w += dw;
								wc += dwc;

								g += dwc * dg;
								b += dwc * db;
								r += dwc * dr;
								a += dw * da;

								if (wc.Equals(0f))
								{
									nb += db * dw;
									ng += dg * dw;
									nr += dr * dw;
								}
							}
						}

						if (wc.Equals(0f))
						{
							b = nb / w;
							g = ng / w;
							r = nr / w;
						}
						else
						{
							g /= wc;
							b /= wc;
							r /= wc;
						}

						a /= w;

						BgraValues[l + B] = toByte(b);
						BgraValues[l + G] = toByte(g);
						BgraValues[l + R] = toByte(r);
						BgraValues[l + A] = toByte(a);
					}
				}
		}

		private static byte toByte(float r) =>
			(byte) Math.Round(r).WithinRange(0, 255);

		private readonly Bitmap _original;
		private readonly Rectangle _sourceRect;
	}
}