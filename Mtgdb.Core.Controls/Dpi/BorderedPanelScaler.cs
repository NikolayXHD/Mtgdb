using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public static class BorderedPanelScaler
	{
		public static void ScaleDpi(this BorderedPanel panel, Func<Bitmap, Bitmap> iconScaler)
		{
			panel.ScaleDpiSize();

			new DpiScaler<BorderedPanel, Bitmap>(
				c => (Bitmap) c.BackgroundImage,
				(c, bmp) => c.BackgroundImage = bmp,
				iconScaler).Setup(panel);
		}
	}
}