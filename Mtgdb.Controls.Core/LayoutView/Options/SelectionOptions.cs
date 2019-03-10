using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class SelectionOptions
	{
		[Category("Settings")]
		[DefaultValue(typeof(Color), "Control")]
		public Color HotTrackBackColor { get; set; } = SystemColors.Control;

		[Category("Settings")]
		[DefaultValue(typeof(Color), "ActiveBorder")]
		public Color HotTrackBorderColor { get; set; } = SystemColors.ActiveBorder;

		[Category("Settings")]
		[DefaultValue(true)]
		public bool Enabled { get; set; } = true;

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color RectBorderColor { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color RectFillColor { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Highlight")]
		public Color BackColor { get; set; } = SystemColors.Highlight;

		[Category("Settings")]
		[DefaultValue(typeof(Color), "WindowText")]
		public Color ForeColor { get; set; } = SystemColors.HighlightText;

		[Category("Settings")]
		[DefaultValue(0)]
		public byte RectAlpha { get; set; }

		[Category("Settings")]
		[DefaultValue(255)]
		public byte Alpha { get; set; } = 255;
	}
}