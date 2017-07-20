using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class RichTextRenderContext
	{
		public Color BackgroundColor { get; set; }
		public Color ForeColor { get; set; }
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
	}
}