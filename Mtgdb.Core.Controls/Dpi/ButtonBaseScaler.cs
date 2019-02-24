using System.Drawing;

namespace Mtgdb.Controls
{
	public static class ButtonBaseScaler
	{
		public static void ScaleDpiAuto(this ButtonBase button)
		{
			ControlBaseScaler.ScaleDpiAuto(button);
			_focusBorderScaler.Setup(button);

			button.ImageChecked = button.ImageChecked?.ApplyColorScheme();
			button.ImageUnchecked = button.ImageUnchecked?.ApplyColorScheme();
		}

		public static void ScaleDpiAuto(this ButtonBase button, (Bitmap Image, Bitmap ImageDouble) img)
		{
			ControlBaseScaler.ScaleDpiAuto(button, img);
			_focusBorderScaler.Setup(button);
		}

		public static void ScaleDpiAuto(this ButtonBase button,
			(Bitmap Image, Bitmap ImageDouble) uncheckedImg,
			(Bitmap Image, Bitmap ImageDouble) checkedImg)
		{
			ControlBaseScaler.ScaleDpiAuto(button);
			_focusBorderScaler.Setup(button);

			new DpiScaler<ButtonBase>(b =>
			{
				if (Dpi.ScalePercent > 100)
				{
					b.ImageChecked = checkedImg.ImageDouble?.ApplyColorScheme();
					b.ImageUnchecked = uncheckedImg.ImageDouble?.ApplyColorScheme();
					b.ImageScale = 0.5f.ByDpiWidth();
				}
				else
				{
					b.ImageChecked = checkedImg.Image?.ApplyColorScheme();
					b.ImageUnchecked = uncheckedImg.Image?.ApplyColorScheme();
					b.ImageScale = 1f.ByDpiWidth();
				}
			}).Setup(button);
		}

		private static readonly DpiScaler<ButtonBase, int> _focusBorderScaler =
			new DpiScaler<ButtonBase, int>(
				b => b.FocusBorderWidth,
				(b, w) => b.FocusBorderWidth = w,
				w => w.ByDpiWidth());
	}
}