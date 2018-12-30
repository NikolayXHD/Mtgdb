using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public static class ControlScaler
	{
		public static void ScaleDpi(this Control control) =>
			_scaler.Setup(control);

		public static void ScaleDpiSize(this Control control) =>
			_sizeScaler.Setup(control);

		public static void ScaleDpiFont(this Control control) =>
			_fontScaler.Setup(control);

		public static void ScaleDpiHeight(this Control control) =>
			_heightScaler.Setup(control);

		public static void ScaleDpiWidth(this Control control) =>
			_widthScaler.Setup(control);

		private static readonly DpiScaler<Control, Size> _sizeScaler =
			new DpiScaler<Control, Size>(
				c => c.Size,
				(c, s) => c.Size = s,
				size => size.ByDpi());

		private static readonly DpiScaler<Control, Font> _fontScaler =
			new DpiScaler<Control, Font>(
				c => c.Font,
				(c, f) => c.Font = f,
				font => font.ByDpi());

		private static readonly DpiScaler<Control, (Size, Font)> _scaler =
			Scalers.Combine(_sizeScaler, _fontScaler);

		private static readonly DpiScaler<Control, int> _heightScaler =
			new DpiScaler<Control, int>(
				c => c.Height,
				(c, h) => c.Height = h,
				h => h.ByDpiHeight());

		private static readonly DpiScaler<Control, int> _widthScaler =
			new DpiScaler<Control, int>(
				c => c.Width,
				(c, w) => c.Width = w,
				w => w.ByDpiWidth());
	}
}