using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mtgdb
{
	public abstract class BmpProcessor
	{
		protected Rectangle Rect { get; }
		protected byte[] BgraValues { get; private set; }
		public bool ImageChanged { get; protected set; }

		protected BmpProcessor(Bitmap bmp)
		{
			_bmp = bmp;
			Rect = new Rectangle(location: default, size: _bmp?.Size ?? default);
		}

		public void Execute()
		{
			var bmpData = _bmp.LockBits(Rect, ImageLockMode.ReadWrite, PixelFormat);

			try
			{
				int numBytes = bmpData.Stride * bmpData.Height;
				BgraValues = new byte[numBytes];
				Marshal.Copy(bmpData.Scan0, BgraValues, 0, numBytes);

				ExecuteRaw();

				if (ImageChanged)
					Marshal.Copy(BgraValues, 0, bmpData.Scan0, numBytes);
			}
			finally
			{
				_bmp.UnlockBits(bmpData);
			}
		}

		protected abstract void ExecuteRaw();

		protected bool SameColor(int first, int second)
		{
			int delta =
				Math.Abs(BgraValues[first + B] - BgraValues[second + B]) +
				Math.Abs(BgraValues[first + G] - BgraValues[second + G]) +
				Math.Abs(BgraValues[first + R] - BgraValues[second + R]);

			return delta < ColorSimilarityThreshold;
		}

		protected bool SameColor(int location, byte b, byte g, byte r, byte a)
		{
			int delta =
				Math.Abs(BgraValues[location + B] - b) +
				Math.Abs(BgraValues[location + G] - g) +
				Math.Abs(BgraValues[location + R] - r) +
				Math.Abs(BgraValues[location + A] - a);

			return delta < ColorSimilarityThreshold;
		}

		protected int GetLocation(int x, int y) =>
			BytesPerPixel * (Rect.Width * y + x);

		protected virtual int ColorSimilarityThreshold => 20;
		private readonly Bitmap _bmp;

		public const int BytesPerPixel = 4;
		public const int B = 0;
		public const int G = 1;
		public const int R = 2;
		public const int A = 3;

		public const PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
	}
}