using System.Drawing;

namespace Mtgdb.Controls
{
	public static class ButtonBaseScaler
	{
		public static void ScaleDpi(this ButtonBase button)
		{
			_imageScaleScaler.Setup(button);
			ControlScaler.ScaleDpi(button);

			button.ImageChecked = button.ImageChecked?.ApplyColorScheme();
			button.ImageUnchecked = button.ImageUnchecked?.ApplyColorScheme();
			button.Image = button.Image?.ApplyColorScheme();
		}

		public static void ScaleDpiImages(this ButtonBase button,
			(Bitmap Image, Bitmap ImageDouble) uncheckedImg,
			(Bitmap Image, Bitmap ImageDouble) checkedImg)
		{
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

		public static void ScaleDpiImages(this ButtonBase button,
			(Bitmap Image, Bitmap ImageDouble) img)
		{
			new DpiScaler<ButtonBase>(b =>
			{
				if (Dpi.ScalePercent > 100)
				{
					b.Image = img.ImageDouble?.ApplyColorScheme();
					b.ImageScale = 0.5f.ByDpiWidth();
				}
				else
				{
					b.Image = img.Image?.ApplyColorScheme();
					b.ImageScale = 1f.ByDpiWidth();
				}
			}).Setup(button);
		}

		private static readonly DpiScaler<ButtonBase, float> _imageScaleScaler =
			new DpiScaler<ButtonBase, float>(
				b => b.ImageScale,
				(b, s) => b.ImageScale = s,
				s => s.ByDpiWidth());
	}
}