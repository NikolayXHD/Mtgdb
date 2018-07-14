using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class TooltipModel
	{
		public TooltipModel()
		{
			Created = DateTime.Now;
		}

		public object Id { get; set; }
		public Control Control { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public DateTime Created { get; private set; }
		public bool Clickable { get; set; }
		public IList<TextRange> HighlightRanges { get; set; }
		public HighlightOptions HighlightOptions { get; set; }
		public Rectangle ObjectBounds { get; set; }

		public DateTime? Abandoned { get; set; }
	}
}