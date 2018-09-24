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
			FlatAppearance.CheckedBackColor = Color.Transparent;
			FlatAppearance.MouseOverBackColor = Color.Transparent;
			FlatAppearance.MouseDownBackColor = Color.Transparent;

			HighlightBackColor = Color.Transparent;
			SystemColorsChanged += systemColorsChanged;
		}

		protected override bool ShowFocusCues => false;
		protected override bool ShowKeyboardCues => false;

		private void systemColorsChanged(object sender, EventArgs e) =>
			updateHighlightColor();

		[DefaultValue(48), Category("Settings")]
		public int HighlightOpacity
		{
			get => _highlightOpacity;
			set
			{
				if (_highlightOpacity != value)
				{
					_highlightOpacity = value;
					updateHighlightColor();
				}
			}
		}

		[DefaultValue(32), Category("Settings")]
		public int MouseoverOpacity
		{
			get => _mouseoverOpacity;
			set
			{
				if (_mouseoverOpacity != value)
				{
					_mouseoverOpacity = value;
					updateHighlightColor();
				}
			}
		}

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
			FlatAppearance.CheckedBackColor = FlatAppearance.MouseDownBackColor =
				Color.FromArgb(_highlightOpacity * _highlightBackColor.A / 255, _highlightBackColor);

			FlatAppearance.MouseOverBackColor =
				Color.FromArgb(_mouseoverOpacity * _highlightBackColor.A / 255, _highlightBackColor);
		}

		private int _highlightOpacity = 48;
		private int _mouseoverOpacity = 32;
		private Color _highlightBackColor;
	}
}