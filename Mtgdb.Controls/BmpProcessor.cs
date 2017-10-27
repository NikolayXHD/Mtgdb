using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mtgdb
{
	public abstract class BmpProcessor
	{
		private const int ByesPerPixel = 4;
		private readonly Bitmap _bmp;

		protected Rectangle Rect { get; private set; }
		protected byte[] RgbValues { get; private set; }
		public bool ImageChanged { get; protected set; }

		protected BmpProcessor(Bitmap bmp)
		{
			_bmp = bmp;
		}

		public void Execute()
		{
			const PixelFormat pixelFormat = PixelFormat.Format32bppArgb;

			// Lock the bitmap's bits.
			Rect = new Rectangle(Point.Empty, _bmp.Size);
			var bmpData = _bmp.LockBits(Rect, ImageLockMode.ReadWrite, pixelFormat);

			try
			{
				// Declare an array to hold the bytes of the bitmap. 
				int numBytes = bmpData.Stride * Rect.Height;
				RgbValues = new byte[numBytes];

				var firstBytePtr = bmpData.Scan0;
				Marshal.Copy(firstBytePtr, RgbValues, 0, numBytes);

				ExecuteRaw();

				if (ImageChanged)
					// Copy the RGB values back to the bitmap
					Marshal.Copy(RgbValues, 0, firstBytePtr, numBytes);
			}
			finally
			{
				// Unlock the bits.
				_bmp.UnlockBits(bmpData);
			}
		}

		/// <summary>
		/// Выполняет некоторую обработку изображения.
		/// Это может быть анализ и сохранение его результата в свойство класса,
		/// а может быть преобразование изображения.
		/// </summary>
		protected abstract void ExecuteRaw();

		protected bool SameColor(int first, int second)
		{
			int delta =
				Math.Abs(RgbValues[first] - RgbValues[second + 0]) +
				Math.Abs(RgbValues[first + 1] - RgbValues[second + 1]) +
				Math.Abs(RgbValues[first + 2] - RgbValues[second + 2]);

			return 50 > delta;
		}

		protected bool SameColor(int location, byte r, byte g, byte b, byte a)
		{
			int delta =
				Math.Abs(RgbValues[location] - r) +
				Math.Abs(RgbValues[location + 1] - g) +
				Math.Abs(RgbValues[location + 2] - b) +
				Math.Abs(RgbValues[location + 3] - a);

			return 50 > delta;
		}

		protected int GetLocation(int x, int y)
		{
			return ByesPerPixel * (Rect.Width * y + x);
		}
	}

	public class BmpReader : IDisposable
	{
		private const int ByesPerPixel = 4;

		private readonly Bitmap _bmp;
		private readonly BitmapData _bitmapData;

		protected Rectangle Rect { get; }
		public byte[] RgbValues { get; }
		
		public BmpReader(Bitmap bmp)
		{
			_bmp = bmp;
			const PixelFormat pixelFormat = PixelFormat.Format32bppArgb;
			// Lock the bitmap's bits.
			Rect = new Rectangle(Point.Empty, _bmp.Size);

			_bitmapData = _bmp.LockBits(Rect, ImageLockMode.ReadOnly, pixelFormat);
			
			int numBytes = _bitmapData.Stride * Rect.Height;
			RgbValues = new byte[numBytes];

			var firstBytePtr = _bitmapData.Scan0;
			Marshal.Copy(firstBytePtr, RgbValues, 0, numBytes);
		}

		public int GetLocation(int x, int y)
		{
			return ByesPerPixel * (Rect.Width * y + x);
		}

		public void Dispose()
		{
			_bmp.UnlockBits(_bitmapData);
		}
	}

	public class BmpScaler : BmpProcessor
	{
		public BmpScaler(Bitmap bmp, Bitmap original) : base(bmp)
		{
			_original = original;
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			float scaleX = (float)_original.Width / Rect.Width;
			float scaleY = (float)_original.Height / Rect.Height;
			
			using (var original = new BmpReader(_original))
				for (int i = 0; i < Rect.Width; i++)
				{
					float left = i * scaleX;
					float right = left + scaleX;

					for (int j = 0; j < Rect.Height; j++)
					{
						float top = j * scaleY;
						float bottom = top + scaleY;

						var l = GetLocation(i, j);

						float r = 0, g = 0, b = 0, a = 0, w = 0, wc = 0, nr = 0, ng = 0, nb = 0;

						for (int iOr = (int) Math.Floor(left); iOr < Math.Ceiling(right); iOr++)
						{
							float wX = Math.Min(right, iOr + 1) - Math.Max(left, iOr);

							for (int jOr = (int) Math.Floor(top); jOr < Math.Ceiling(bottom); jOr++)
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

								if (wc == 0)
								{
									nr += dr * dw;
									ng += dg * dw;
									nb += db * dw;
								}
							}
						}

						if (wc == 0)
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

		private static byte toByte(float r)
		{
			return (byte) Math.Round(r).WithinRange(0, 255);
		}

		private readonly Bitmap _original;
	}
}