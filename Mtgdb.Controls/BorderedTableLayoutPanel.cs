using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class BorderedTableLayoutPanel : TableLayoutPanel
	{
		public BorderedTableLayoutPanel()
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
			this.PaintPanel(e.Graphics, VisibleBorders, BorderColor);
		}

		[Category("Settings"), DefaultValue(typeof(Color), "DarkGray")]
		public Color BorderColor { get; set; } = Color.DarkGray;

		[Category("Settings"), DefaultValue(typeof(AnchorStyles), "Top|Right|Bottom|Left")]
		public AnchorStyles VisibleBorders { get; set; } = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
	}
}