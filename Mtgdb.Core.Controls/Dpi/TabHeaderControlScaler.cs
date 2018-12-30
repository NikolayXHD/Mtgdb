using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public static class TabHeaderControlScaler
	{
		public static void ScaleDpi(this TabHeaderControl tab, Func<Bitmap, Bitmap> transformIcon)
		{
			_scaler.Setup(tab);
			tab.ScaleDpiFont();

			var iconsScaler = Scalers.Combine(
				new DpiScaler<TabHeaderControl, Bitmap>(
					c => c.CloseIcon,
					(c, bmp) => c.CloseIcon = bmp,
					transformIcon),
				new DpiScaler<TabHeaderControl, Bitmap>(
					c => c.CloseIconHovered,
					(c, bmp) => c.CloseIconHovered = bmp,
					transformIcon),
				new DpiScaler<TabHeaderControl, Bitmap>(
					c => c.AddIcon,
					(c, bmp) => c.AddIcon = bmp,
					transformIcon),
				new DpiScaler<TabHeaderControl, Bitmap>(
					c => c.DefaultIcon,
					(c, bmp) => c.DefaultIcon = bmp,
					transformIcon));

			iconsScaler.Setup(tab);

			_textWidthScaler.Setup(tab);
		}

		private static readonly DpiScaler<TabHeaderControl, Size> _slopeSizeScaler =
			new DpiScaler<TabHeaderControl, Size>(
				c => c.SlopeSize,
				(c, size) => c.SlopeSize = size,
				size => size.ByDpi());

		private static readonly DpiScaler<TabHeaderControl, int> _heightScaler =
			new DpiScaler<TabHeaderControl, int>(
				c => c.Height,
				(c, h) => c.Height = h,
				h => h.ByDpiHeight());

		private static readonly DpiScaler<TabHeaderControl, Size> _addSlopeSizeScaler =
			new DpiScaler<TabHeaderControl, Size>(
				c => c.AddButtonSlopeSize,
				(c, size) => c.AddButtonSlopeSize = size,
				size => size.ByDpi());

		private static readonly DpiScaler<TabHeaderControl, int> _addWidthScaler =
			new DpiScaler<TabHeaderControl, int>(
				c => c.AddButtonWidth,
				(c, w) => c.AddButtonWidth = w,
				w => w.ByDpiWidth());

		private static readonly DpiScaler<TabHeaderControl, int> _textPaddingScaler =
			new DpiScaler<TabHeaderControl, int>(
				c => c.TextPadding,
				(c, p) => c.TextPadding = p,
				p => p.ByDpiWidth());

		private static readonly DpiScaler<TabHeaderControl, (Size, Size, int, int, int)> _scaler = Scalers.Combine(
			_slopeSizeScaler,
			_addSlopeSizeScaler,
			_heightScaler,
			_addWidthScaler,
			_textPaddingScaler);

		private static readonly DpiScaler<TabHeaderControl> _textWidthScaler =
			new DpiScaler<TabHeaderControl>(c => c.RecalculateWidths());
	}
}