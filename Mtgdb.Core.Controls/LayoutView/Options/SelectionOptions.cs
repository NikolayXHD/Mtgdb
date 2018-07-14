using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class SelectionOptions
	{
		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color HotTrackBackColor { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color HotTrackBorderColor { get; set; }

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
		[DefaultValue(typeof(Color), "Transparent")]
		public Color BackColor { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(Color), "Transparent")]
		public Color ForeColor { get; set; }

		[Category("Settings")]
		[DefaultValue(0)]
		public byte RectAlpha { get; set; }

		[Category("Settings")]
		[DefaultValue(255)]
		public byte Alpha { get; set; } = 255;
	}
}