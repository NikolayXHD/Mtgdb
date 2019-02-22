using System.Drawing;

namespace Mtgdb.Controls
{
	public static class ControlBaseScaler
	{
		public static void ScaleDpiImages(this ControlBase control)
		{
			_imageScaleScaler.Setup(control);
			control.Image = control.Image?.ApplyColorScheme();
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

		private static readonly DpiScaler<ControlBase, float> _imageScaleScaler =
			new DpiScaler<ControlBase, float>(
				c => c.ImageScale,
				(c, s) => c.ImageScale = s,
				s => s.ByDpiWidth());
	}
}