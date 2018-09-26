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
		}

		protected override bool ShowFocusCues => false;
		protected override bool ShowKeyboardCues => false;

		private void systemColorsChanging() =>
			updateHighlightColor();

		private int _highlightMouseoverOpacity = 32;
		[DefaultValue(32), Category("Settings")]
		public int HighlightMouseoverOpacity
		{
			get => _highlightMouseoverOpacity;
			set
			{
				if (_highlightMouseoverOpacity != value)
				{
					_highlightMouseoverOpacity = value;
					updateHighlightColor();
				}
			}
		}

		private int _highlightMouseDownOpacity = 40;
		[DefaultValue(40), Category("Settings")]
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

		private int _highlightCheckedOpacity = 48;
		[DefaultValue(48), Category("Settings")]
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
			FlatAppearance.MouseOverBackColor =
				Color.FromArgb(_highlightMouseoverOpacity * _highlightBackColor.A / 255, _highlightBackColor);

			FlatAppearance.MouseDownBackColor =
				Color.FromArgb(_highlightMouseDownOpacity * _highlightBackColor.A / 255, _highlightBackColor);

			FlatAppearance.CheckedBackColor =
				Color.FromArgb(_highlightCheckedOpacity * _highlightBackColor.A / 255, _highlightBackColor);
		}
	}
}