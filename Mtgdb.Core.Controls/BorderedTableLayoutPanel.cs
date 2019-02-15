using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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

		protected override void OnPaint(PaintEventArgs e) =>
			this.PaintBorder(e.Graphics, VisibleBorders, BorderColor, BorderDashStyle);

		protected override void OnPaintBackground(PaintEventArgs e) =>
			this.PaintPanelBack(e.Graphics, e.ClipRectangle, BackgroundImage, BackColor, PaintBackground);

		[Category("Settings"), DefaultValue(typeof(Color), "ActiveBorder")]
		public Color BorderColor { get; set; } = SystemColors.ActiveBorder;

		[Category("Settings"), DefaultValue(typeof(AnchorStyles), "Top, Bottom, Left, Right")]
		public AnchorStyles VisibleBorders { get; set; } = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;

		[Category("Settings"), DefaultValue(true)]
		public bool PaintBackground { get; set; } = true;

		[Category("Settings"), DefaultValue(typeof(DashStyle), "Solid")]
		public DashStyle BorderDashStyle { get; set; } = DashStyle.Solid;
	}
}