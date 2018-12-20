using System;
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

			EnabledChanged += enabledChanged;
		}

		protected override bool ShowFocusCues => false;
		protected override bool ShowKeyboardCues => false;

		private void systemColorsChanging() =>
			updateHighlightColor();

		private void enabledChanged(object sender, EventArgs e) =>
			updateHighlightColor();

		private void updateHighlightColor()
		{
			if (DesignMode)
				return;

			bool enabled = Enabled;

			FlatAppearance.MouseOverBackColor = getBlendedBgColor(_highlightMouseOverOpacity, enabled);
			FlatAppearance.MouseDownBackColor = getBlendedBgColor(_highlightMouseDownOpacity, enabled);
			FlatAppearance.CheckedBackColor = getBlendedBgColor(_highlightCheckedOpacity, enabled);
		}

		private Color getBlendedBgColor(int o2, bool enabled)
		{
			Color bg = enabled
				? BackColor
				: blend(BackColor, SystemColors.Control, 128);

			return blend(bg, _highlightBackColor, o2);
		}

		private static Color blend(Color bg, Color fore, int foreOpacity)
		{
			if (bg == Color.Empty || bg == Color.Transparent)
				return Color.FromArgb(foreOpacity, fore);

			byte o1 = bg.A;
			return Color.FromArgb(
				blendBytes(o1, (byte) foreOpacity),
				blendBytes(bg.R, fore.R),
				blendBytes(bg.G, fore.G),
				blendBytes(bg.B, fore.B));

			int blendBytes(byte b1, byte b2) =>
				(b2 * foreOpacity * 255 + b1 * o1 * (255 - foreOpacity)) / (255 * 255);
		}

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
	}
}