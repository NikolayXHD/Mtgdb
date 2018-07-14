using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Mtgdb.Controls
{
	public static class Dpi
	{
		public static void Initialize()
		{
			if (Environment.OSVersion.Version.Major >= 6)
				SetProcessDPIAware();

			_scale = getScale();
			_scaleHalf = _scale.MultiplyBy(0.5f);
		}

		public static int ByDpiWidth(this int width)
		{
			return (int) (width * _scale.Width);
		}

		public static int ByDpiHeight(this int height)
		{
			return (int)(height * _scale.Height);
		}

		public static float ByDpiHeight(this float height)
		{
			return (int)(height * _scale.Height);
		}

		public static Size ByDpi(this Size original)
		{
			return original.MultiplyBy(_scale).Round();
		}

		public static Point ByDpi(this Point original)
		{
			return original.MultiplyBy(_scale).Round();
		}

		public static SizeF ByDpi(this SizeF original)
		{
			return original.MultiplyBy(_scale);
		}

		public static Size HalfByDpi(this Size original)
		{
			return original.MultiplyBy(_scaleHalf).Round();
		}

		private static SizeF getScale()
		{
			var g = Graphics.FromHwnd(IntPtr.Zero);
			var desktopPtr = g.GetHdc();

			//http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
			int dpiX = GetDeviceCaps(desktopPtr, 88);
			int dpiY = GetDeviceCaps(desktopPtr, 90);
			
			return new SizeF((float) dpiX / 96, (float) dpiY / 96);
		}

		public static int ScalePercent => (int)Math.Ceiling(100 * Math.Max(_scale.Width, _scale.Height));

		[DllImport("user32.dll")]
		private static extern bool SetProcessDPIAware();

		[DllImport("gdi32.dll")]
		private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

		private static SizeF _scale = new SizeF(1f, 1f);
		private static SizeF _scaleHalf = new SizeF(0.5f, 0.5f);
	}
}