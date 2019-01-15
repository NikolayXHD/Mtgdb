using System.Drawing;

namespace Mtgdb.Controls
{
	public static class QuickFilterControlScaler
	{
		public static void ScaleDpi(this QuickFilterControl control)
		{
			_scaler.Setup(control);
			control.ScaleDpiFont();
		}

		private static readonly DpiScaler<QuickFilterControl, Size> _imageSizeScaler =
			new DpiScaler<QuickFilterControl, Size>(
				c => c.ImageSize,
				(c, s) => c.ImageSize = s,
				size => size.ByDpi());

		private static readonly DpiScaler<QuickFilterControl, Size> _hintTextShiftScaler =
			new DpiScaler<QuickFilterControl, Size>(
				c => c.HintTextShift,
				(c, s) => c.HintTextShift = s,
				size => size.ByDpi());

		private static readonly DpiScaler<QuickFilterControl, Bitmap> _hintIconScaler =
			new DpiScaler<QuickFilterControl, Bitmap>(
				c => c.HintIcon,
				(c, bmp) => c.HintIcon = bmp,
				bmp => bmp?.ResizeDpi());

		private static readonly DpiScaler<QuickFilterControl, float> _borderScaler =
			new DpiScaler<QuickFilterControl, float>(
				c => c.SelectionBorder,
				(c, b) => c.SelectionBorder = b,
				b => b.ByDpiWidth());

		private static readonly DpiScaler<QuickFilterControl, (Size, Size, Bitmap, float)> _scaler = DpiScalers.Combine(
			_imageSizeScaler,
			_hintTextShiftScaler,
			_hintIconScaler,
			_borderScaler);
	}
}