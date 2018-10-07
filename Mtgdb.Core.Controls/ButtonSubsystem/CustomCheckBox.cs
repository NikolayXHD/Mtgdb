using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class CustomCheckBox : CheckBox
	{
		public CustomCheckBox()
		{
			TabStop = false;
			BackColor = Color.Transparent;
			FlatStyle = FlatStyle.Flat;
			FlatAppearance.BorderColor = SystemColors.ActiveBorder;
			FlatAppearance.MouseOverBackColor = Color.Transparent;
			FlatAppearance.MouseDownBackColor = Color.Transparent;
			FlatAppearance.CheckedBackColor = Color.Transparent;

			HighlightBackColor = Color.Transparent;
			ColorSchemeController.SystemColorsChanging += systemColorsChanging;
		}

		protected override bool ShowFocusCues => false;
		protected override bool ShowKeyboardCues => false;

		private void systemColorsChanging() =>
			updateHighlightColor();

		private int _highlightMouseOverOpacity = 64;
		[DefaultValue(64), Category("Settings")]
		public int HighlightMouseOverOpacity
		{
			get => _highlightMouseOverOpacity;
			set
			{
				if (_highlightMouseOverOpacity != value)
				{
					_highlightMouseOverOpacity = value;
					updateHighlightColor();
				}
			}
		}

		private int _highlightMouseDownOpacity = 92;
		[DefaultValue(92), Category("Settings")]
		public int HighlightMouseDownOpacity
		{
			get => _highlightMouseDownOpacity;
			set
			{
				if (_highlightMouseDownOpacity != value)
				{
					_highlightMouseDownOpacity = value;
					updateHighlightColor();
				}
			}
		}

		private int _highlightCheckedOpacity = 128;
		[DefaultValue(128), Category("Settings")]
		public int HighlightCheckedOpacity
		{
			get => _highlightCheckedOpacity;
			set
			{
				if (_highlightCheckedOpacity != value)
				{
					_highlightCheckedOpacity = value;
					updateHighlightColor();
				}
			}
		}

		private Color _highlightBackColor = Color.Transparent;
		[Category("Settings"), DefaultValue(typeof(Color), "Transparent")]
		public Color HighlightBackColor
		{
			get => _highlightBackColor;
			set
			{
				if (_highlightBackColor != value)
				{
					_highlightBackColor = value;
					updateHighlightColor();
				}
			}
		}

		private void updateHighlightColor()
		{
			if (DesignMode)
				return;

			FlatAppearance.MouseOverBackColor = applyOpacity(_highlightMouseOverOpacity);
			FlatAppearance.MouseDownBackColor = applyOpacity(_highlightMouseDownOpacity);
			FlatAppearance.CheckedBackColor = applyOpacity(_highlightCheckedOpacity);
		}

		private Color applyOpacity(int o2)
		{
			var c1 = BackColor;
			var c2 = _highlightBackColor;

			if (c1 == Color.Empty || c1 == Color.Transparent)
				return Color.FromArgb(o2, c2);

			byte o1 = c1.A;
			return Color.FromArgb(
				blendBytes(o1, (byte) o2),
				blendBytes(c1.R, c2.R),
				blendBytes(c1.G, c2.G),
				blendBytes(c1.B, c2.B));

			int blendBytes(byte b1, byte b2) =>
				(b2 * o2 * 255 + b1 * (255 - o2) * o1) / (255 * 255);
		}
	}
}