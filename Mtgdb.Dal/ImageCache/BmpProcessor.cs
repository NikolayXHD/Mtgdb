using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mtgdb.Dal
{
	public abstract class BmpProcessor
	{
		private readonly Bitmap _bmp;

		protected int BytesPerPixel { get; private set; }
		protected Rectangle Rect { get; private set; }
		protected byte[] RgbValues { get; private set; }
		protected bool ImageChanged { get; set; }

		protected BmpProcessor(Bitmap bmp)
		{
			_bmp = bmp;
		}

		public void Execute()
		{
			const PixelFormat pixelFormat = PixelFormat.Format32bppArgb;

			// Lock the bitmap's bits.
			Rect = new Rectangle(0, 0, _bmp.Width, _bmp.Height);
			var bmpData = _bmp.LockBits(Rect, ImageLockMode.ReadWrite, pixelFormat);

			try
			{
				// Declare an array to hold the bytes of the bitmap. 
				BytesPerPixel = 4;
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
			return 30 >
				Math.Abs(RgbValues[first] - RgbValues[second + 0]) +
					Math.Abs(RgbValues[first + 1] - RgbValues[second + 1]) +
					Math.Abs(RgbValues[first + 2] - RgbValues[second + 2]);
		}

		protected int GetLocation(int x, int y)
		{
			return BytesPerPixel * (Rect.Width * y + x);
		}
	}
}