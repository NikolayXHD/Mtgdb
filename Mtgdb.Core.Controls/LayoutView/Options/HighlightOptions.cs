using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class HighlightOptions
	{
		[Category("Settings"), DefaultValue(typeof(Color), "GradientInactiveCaption")]
		public Color HighlightContextColor { get; set; } = SystemColors.GradientInactiveCaption;

		[Category("Settings"), DefaultValue(typeof(Color), "GradientActiveCaption")]
		public Color HighlightColor { get; set; } = SystemColors.GradientActiveCaption;

		[Category("Settings"), DefaultValue(typeof(Color), "HotTrack")]
		public Color HighlightBorderColor { get; set; } = SystemColors.HotTrack;

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