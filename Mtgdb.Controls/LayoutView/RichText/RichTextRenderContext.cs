using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class RichTextRenderContext
	{
		public string Text { get; set; }
		public TextSelection TextSelection { get; set; }
		public Color ForeColor { get; set; }

		public Color SelectionBackColor { get; set; }
		public Color SelectionForeColor { get; set; }
		public byte SelectionAlpha { get; set; }

		public RectangleF Rect { get; set; }
		
		public Graphics Graphics { get; set; }
		public Font Font { get; set; }
		public HorizontalAlignment HorizAlignment { get; set; }
		public IList<TextRange> HighlightRanges { get; set; }

		public Color HighlightContextColor { get; set; }
		public Color HighlightColor { get; set; }
		public Color HighlightBorderColor { get; set; }
		public float HighlightBorderWidth { get; set; }

		public bool Selecting { get; set; }
		public Point SelectionEnd { get; set; }
		public Point SelectionStart { get; set; }
	}
}