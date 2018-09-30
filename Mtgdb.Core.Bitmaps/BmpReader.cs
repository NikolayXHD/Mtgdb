using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mtgdb
{
	public class BmpReader : IDisposable
	{
		private readonly Bitmap _bmp;
		private readonly BitmapData _bmpData;

		public Rectangle Rect { get; }
		public byte[] BgraValues { get; }

		public BmpReader(Bitmap bmp, Rectangle rect)
		{

			Rect = rect;
			_bmp = bmp;
			_bmpData = _bmp.LockBits(Rect, ImageLockMode.ReadOnly, BmpProcessor.PixelFormat);

			int numBytes = _bmpData.Stride * _bmpData.Height;
			BgraValues = new byte[numBytes];
			Marshal.Copy(_bmpData.Scan0, BgraValues, 0, numBytes);
		}

		public int GetLocation(int x, int y) =>
			BmpProcessor.BytesPerPixel * (Rect.Width * y + x);

		public void Dispose() =>
			_bmp.UnlockBits(_bmpData);
	}
}