using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mtgdb.Bitmaps
{
	public class BmpReader : IDisposable
	{
		private const int ByesPerPixel = 4;

		private readonly Bitmap _bmp;
		private readonly BitmapData _bmpData;

		public Rectangle Rect { get; }
		public byte[] RgbValues { get; }

		public BmpReader(Bitmap bmp, Rectangle rect)
		{
			const PixelFormat format = PixelFormat.Format32bppArgb;

			Rect = rect;
			_bmp = bmp;
			_bmpData = _bmp.LockBits(Rect, ImageLockMode.ReadOnly, format);

			int numBytes = _bmpData.Stride * _bmpData.Height;
			RgbValues = new byte[numBytes];
			Marshal.Copy(_bmpData.Scan0, RgbValues, 0, numBytes);
		}

		public int GetLocation(int x, int y)
		{
			return ByesPerPixel * (Rect.Width * y + x);
		}

		public void Dispose()
		{
			_bmp.UnlockBits(_bmpData);
		}
	}
}