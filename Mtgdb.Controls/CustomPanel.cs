using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class CustomPanel : Panel
	{
		public CustomPanel()
		{
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.ResizeRedraw,
				true);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(BackColor);
			var pen = new Pen(BorderColor);
			if (BackgroundImage != null)
			{
				var location = new Point(
					(Width - BackgroundImage.Width)  / 2,
					(Height - BackgroundImage.Height) / 2);

				e.Graphics.DrawImageUnscaled(BackgroundImage, location);
			}

			if ((VisibleBorders & AnchorStyles.Top) > 0)
				e.Graphics.DrawLine(pen, 0, 0, Width - 1, 0);

			if ((VisibleBorders & AnchorStyles.Bottom) > 0)
				e.Graphics.DrawLine(pen, 0, Height - 1, Width - 1, Height - 1);

			if ((VisibleBorders & AnchorStyles.Left) > 0)
				e.Graphics.DrawLine(pen, 0, 0, 0, Height - 1);

			if ((VisibleBorders & AnchorStyles.Right) > 0)
				e.Graphics.DrawLine(pen, Width - 1, 0, Width - 1, Height - 1);
		}

		[Category("Settings"), DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderColor { get; set; } = Color.DarkGray;

		[Category("Settings"), DefaultValue(typeof (AnchorStyles), "Top|Right|Bottom|Left")]
		public AnchorStyles VisibleBorders { get; set; } = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
	}
}