using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class HighlightOptions
	{
		[Category("Settings")]
		[DefaultValue(typeof(Color), "LightCyan")]
		public Color HighlightContextColor { get; set; } = Color.LightCyan;

		[Category("Settings")]
		[DefaultValue(typeof(Color), "LightBlue")]
		public Color HighlightColor { get; set; } = Color.LightBlue;

		[Category("Settings")]
		[DefaultValue(typeof(Color), "CadetBlue")]
		public Color HighlightBorderColor { get; set; } = Color.CadetBlue;

		public HighlightOptions Clone()
		{
			return new HighlightOptions
			{
				HighlightBorderColor = HighlightBorderColor,
				HighlightColor = HighlightColor,
				HighlightContextColor = HighlightContextColor
			};
		}
	}
}