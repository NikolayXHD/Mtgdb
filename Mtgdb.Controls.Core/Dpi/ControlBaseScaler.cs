using System.Drawing;

namespace Mtgdb.Controls
{
	public static class ControlBaseScaler
	{
		public static void ScaleDpiAuto(this ControlBase control)
		{
			scaleAuto(control);

			_imageScaleScaler.Setup(control);
			control.Image = control.Image?.ApplyColorScheme();
		}

		public static void ScaleDpiAuto(this ControlBase button,
			(Bitmap Image, Bitmap ImageDouble) img)
		{
			scaleAuto(button);

			new DpiScaler<ControlBase>(b =>
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

		private static void scaleAuto(ControlBase control)
		{
			control.ScaleDpiFont();
			control.ScaleDpiMargin();
			control.ScaleDpiPadding();

			if (!control.AutoSize)
				control.ScaleDpiSize();
		}

		private static readonly DpiScaler<ControlBase, float> _imageScaleScaler =
			new DpiScaler<ControlBase, float>(
				c => c.ImageScale,
				(c, s) => c.ImageScale = s,
				s => s.ByDpiWidth());
	}
}