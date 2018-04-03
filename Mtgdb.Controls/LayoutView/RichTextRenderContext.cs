using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class RichTextRenderContext
	{
		public Color BackColor { get; set; }
		public Color ForeColor { get; set; }

		public Color SelectionBackColor { get; set; }
		public Color SelectionForeColor { get; set; }
		public byte SelectionAlpha { get; set; }

		public RectangleF Rect { get; set; }
		public string Text { get; set; }
		public Graphics Graphics { get; set; }
		public Font Font { get; set; }
		public HorizontalAlignment HorizAlignment { get; set; }
		public StringFormat StringFormat { get; set; }
		public IList<TextRange> HighlightRanges { get; set; }

		public Color HighlightContextColor { get; set; }
		public Color HighlightColor { get; set; }
		public Color HighlightBorderColor { get; set; }
		public float HighlightBorderWidth { get; set; }

		public bool RectSelected { get; set; }
		public Point SelectionEnd { get; set; }
		public Point SelectionStart { get; set; }

		public int SelectionStartIndex { get; set; } = -1;
		public int SelectionLength { get; set; } = -1;

		public bool SelectionIsAll { get; set; }

		public string SelectedText
		{
			get
			{
				if (SelectionIsAll)
					return Text;

				if (SelectionStartIndex < 0)
					return null;

				return Text.Substring(SelectionStartIndex, SelectionLength);
			}
		}
	}
}