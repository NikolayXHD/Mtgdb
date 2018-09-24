using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class HighlightOptions
	{
		[Category("Settings")]
		public Color HighlightContextColor
		{
			get => _highlightContextColor ?? SystemColors.Info.TransformHsv(h: _ =>
				_ + Color.LightGoldenrodYellow.RotationTo(Color.Aqua));
			set => _highlightContextColor = value;
		}

		[Category("Settings")]
		public Color HighlightColor
		{
			get => _highlightColor ?? SystemColors.Info.TransformHsv(h: _ =>
				_ + Color.LightGoldenrodYellow.RotationTo(Color.Blue));
			set => _highlightColor = value;
		}

		[Category("Settings")]
		[DefaultValue(typeof(Color), "HotTrack")]
		public Color HighlightBorderColor
		{
			get => _highlightBorderColor ?? SystemColors.HotTrack;
			set => _highlightBorderColor = value;
		}

		private Color? _highlightColor;
		private Color? _highlightContextColor;
		private Color? _highlightBorderColor;

		public HighlightOptions Clone()
		{
			return new HighlightOptions
			{
				_highlightBorderColor = _highlightBorderColor,
				_highlightColor = _highlightColor,
				_highlightContextColor = _highlightContextColor
			};
		}
	}
}