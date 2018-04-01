using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class CustomDrawArgs
	{
		public int RowHandle { get; set; }
		public Graphics Graphics { get; set; }
		public bool Handled { get; set; }
		public Rectangle Bounds { get; set; }
		public string FieldName { get; set; }
		public string DisplayText { get; set; }
		public HorizontalAlignment HAlignment { get; set; }
		public Font Font { get; set; }
		public Color ForeColor { get; set; }
		public SelectionState Selection { get; set; }
	}
}