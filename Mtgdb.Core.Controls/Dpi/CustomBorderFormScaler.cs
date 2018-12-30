using System.Drawing;
using System.Linq;

namespace Mtgdb.Controls
{
	public static class CustomBorderFormScaler
	{
		public static void ScaleDpi(this CustomBorderForm form)
		{
			form.ScaleDpiSize();
			_scaler.Setup(form);
		}

		private static readonly DpiScaler<CustomBorderForm, Size> _borderScaler =
			new DpiScaler<CustomBorderForm, Size>(
				c => c.BorderSize,
				(c, s) => c.BorderSize = s,
				s => s.ByDpi());

		private static readonly DpiScaler<CustomBorderForm, Size> _buttonScaler =
			new DpiScaler<CustomBorderForm, Size>(
				c => c.ControlBoxButtonSize,
				(c, s) => c.ControlBoxButtonSize = s,
				s => s.ByDpi());

		private static readonly DpiScaler<CustomBorderForm, int> _captionHeightScaler =
			new DpiScaler<CustomBorderForm, int>(
				c => c.CaptionHeight,
				(c, h) => c.CaptionHeight = h,
				h => h.ByDpiHeight());

		private static readonly DpiScaler<CustomBorderForm, Bitmap[]> _captionIconsScaler =
			new DpiScaler<CustomBorderForm, Bitmap[]>(
				c => c.CaptionButtonImages,
				(c, bs) => c.CaptionButtonImages = bs,
				bs => bs.Select(b => b.ResizeDpi()).ToArray());

		private static readonly DpiScaler<CustomBorderForm, (Size, Size, int, Bitmap[])> _scaler =
			Scalers.Combine(
				_borderScaler,
				_buttonScaler,
				_captionHeightScaler,
				_captionIconsScaler);
	}
}