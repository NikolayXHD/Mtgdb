using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mtgdb.Controls
{
	public abstract class BmpProcessor
	{
		protected Rectangle Rect { get; }
		protected byte[] RgbValues { get; private set; }
		public bool ImageChanged { get; protected set; }

		protected BmpProcessor(Bitmap bmp, Rectangle? rect = null)
		{
			_bmp = bmp;
			Rect = rect ?? new Rectangle(Point.Empty, _bmp.Size);
		}

		public void Execute()
		{
			const PixelFormat format = PixelFormat.Format32bppArgb;
			var bmpData = _bmp.LockBits(Rect, ImageLockMode.ReadWrite, format);

			try
			{
				// Declare an array to hold the bytes of the bitmap. 
				int numBytes = bmpData.Stride * bmpData.Height;
				RgbValues = new byte[numBytes];

				Marshal.Copy(bmpData.Scan0, RgbValues, 0, numBytes);

				ExecuteRaw();

				if (ImageChanged)
					// Copy the RGB values back to the bitmap
					Marshal.Copy(RgbValues, 0, bmpData.Scan0, numBytes);
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

			return delta < ColorSimilarityThreshold;
		}

		protected bool SameColor(int location, byte r, byte g, byte b, byte a)
		{
			int delta =
				Math.Abs(RgbValues[location] - r) +
				Math.Abs(RgbValues[location + 1] - g) +
				Math.Abs(RgbValues[location + 2] - b) +
				Math.Abs(RgbValues[location + 3] - a);

			return delta < ColorSimilarityThreshold;
		}

		protected int GetLocation(int x, int y)
		{
			return ByesPerPixel * (Rect.Width * y + x);
		}

		private const int ByesPerPixel = 4;
		private const int ColorSimilarityThreshold = 40;
		private readonly Bitmap _bmp;
	}
}