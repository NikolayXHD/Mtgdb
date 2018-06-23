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
								var valuesOr = original.RgbValues;

								byte dr = valuesOr[lOr];
								byte dg = valuesOr[lOr + 1];
								byte db = valuesOr[lOr + 2];
								byte da = valuesOr[lOr + 3];

								float dw = wX * wY;
								float dwc = dw * da;

								w += dw;
								wc += dwc;

								a += dw * da;
								r += dwc * dr;
								g += dwc * dg;
								b += dwc * db;
								
								if (wc.Equals(0f))
								{
									nr += dr * dw;
									ng += dg * dw;
									nb += db * dw;
								}
							}
						}

						if (wc.Equals(0f))
						{
							r = nr / w;
							g = ng / w;
							b = nb / w;
						}
						else
						{
							r /= wc;
							g /= wc;
							b /= wc;
						}

						a /= w;

						RgbValues[l] = toByte(r);
						RgbValues[l + 1] = toByte(g);
						RgbValues[l + 2] = toByte(b);
						RgbValues[l + 3] = toByte(a);
					}
				}
		}

		private static byte toByte(float r) =>
			(byte) Math.Max(0, Math.Min(255, Math.Round(r)));

		private readonly Bitmap _original;
		private readonly Rectangle _sourceRect;
	}
}