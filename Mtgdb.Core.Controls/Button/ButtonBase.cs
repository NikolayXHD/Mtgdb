using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ButtonBase : ControlBase
	{
		public ButtonBase()
		{
			SetStyle(ControlStyles.Selectable, value: true);

			updateHighlightColors();

			MouseDown += mouseDown;
			MouseClick += mouseClick;
			MouseEnter += mouseEnter;
			MouseLeave += mouseLeave;
			GotFocus += gotFocus;
			LostFocus += lostFocus;
			KeyDown += keyDown;
		}

		protected override void HandlePaint(object sender, PaintEventArgs e)
		{
			base.HandlePaint(sender, e);
			paintFocusRectangle();

			void paintFocusRectangle()
			{
				if (!ContainsFocus || _focusBorderColor.A == 0)
					return;

				var rectangle = new Rectangle(default, Size);
				int width = 2;
				rectangle.Inflate(-(width - 1), -(width - 1));
				var pen = new Pen(_focusBorderColor) { Width = width, DashStyle = DashStyle.Dot };

				e.Graphics.DrawRectangle(pen, rectangle);
			}
		}

		protected override Bitmap SelectImage()
		{
			return Enabled
				? Checked
					? _imageCheckedScaled ?? _imageScaled ?? _imageUncheckedScaled
					: _imageUncheckedScaled ?? _imageScaled ?? _imageCheckedScaled
				: Checked
					? _imageCheckedDisabledScaled ?? _imageDisabledScaled ?? _imageUncheckedDisabledScaled
					: _imageUncheckedDisabledScaled ?? _imageDisabledScaled ?? _imageCheckedDisabledScaled;
		}

		protected override void HandleImageScaleChange()
		{
			updateImages(unscaled: false);
			UpdateSize();
		}

		protected override Color GetBgColor()
		{
			if (MouseOver)
				return _mouseOverBackColor;

			if (Checked)
				return _checkedBackColor;

			return base.GetBgColor();
		}

		protected override void HandleSystemColorsChanged(object s, EventArgs e)
		{
			base.HandleSystemColorsChanged(s, e);
			updateHighlightColors();
			updateImages(true, true, true, true, true, true, true);
		}

		protected override void Dispose(bool disposing)
		{
			MouseDown -= mouseDown;
			MouseClick -= mouseClick;
			MouseEnter -= mouseEnter;
			MouseLeave -= mouseLeave;
			GotFocus -= gotFocus;
			LostFocus -= lostFocus;
			KeyDown -= keyDown;

			base.Dispose(disposing);
		}



		private void updateImages(
			bool unscaled = true, bool scaled = true,
			bool disabled = true, bool enabled = true,
			bool image = true, bool uncheckedImage = true, bool checkedImage = true)
		{
			if (image)
				UpdateImages(unscaled, scaled, disabled, enabled);

			if (unscaled)
			{
				if (disabled)
				{
					if (uncheckedImage)
						_imageUncheckedDisabled = ToDisabledImage(_imageUnchecked);

					if (checkedImage)
						_imageCheckedDisabled = ToDisabledImage(_imageChecked);
				}
			}

			if (scaled)
			{
				if (enabled)
				{
					if (uncheckedImage)
						_imageUncheckedScaled = _imageUnchecked?.ScaleBy(ImageScale);

					if (checkedImage)
						_imageCheckedScaled = _imageChecked?.ScaleBy(ImageScale);
				}

				if (disabled)
				{
					if (uncheckedImage)
						_imageUncheckedDisabledScaled = _imageUncheckedDisabled?.ScaleBy(ImageScale);

					if (checkedImage)
						_imageCheckedDisabledScaled = _imageCheckedDisabled?.ScaleBy(ImageScale);
				}
			}

			Invalidate();
		}

		private void mouseLeave(object sender, EventArgs e) =>
			MouseOver = false;

		private void mouseEnter(object sender, EventArgs e) =>
			MouseOver = true;

		private void mouseDown(object sender, MouseEventArgs e)
		{
			Focus();
			if (e.Button == MouseButtons.Left)
				PressDown?.Invoke(this, EventArgs.Empty);
		}

		private void lostFocus(object sender, EventArgs e) =>
			Invalidate();

		private void gotFocus(object sender, EventArgs e) =>
			Invalidate();

		private void mouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				onPressed();
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Enter:
				case Keys.Space:
					PressDown?.Invoke(this, EventArgs.Empty);
					onPressed();
					break;
			}
		}

		private void onPressed()
		{
			if (AutoCheck)
				Checked = !Checked;

			Pressed?.Invoke(this, EventArgs.Empty);
		}

		private void updateHighlightColors()
		{
			if (DesignMode)
				return;

			_mouseOverBackColor = BackColor.BlendWith(_highlightBackColor, _highlightMouseOverOpacity);
			_checkedBackColor = BackColor.BlendWith(_highlightBackColor, _highlightCheckedOpacity);
			_focusBorderColor = BackColor.BlendWith(_highlightBackColor, _highlightFocusOpacity);
		}

		public event EventHandler CheckedChanged;

		public event EventHandler PressDown;
		public event EventHandler Pressed;

		public override int DisabledOpacity
		{
			get => base.DisabledOpacity;
			set
			{
				if (base.DisabledOpacity == value)
					return;

				base.DisabledOpacity = value;
				updateImages(enabled: false);
				Invalidate();
			}
		}

		private Bitmap _imageUncheckedDisabled;
		private Bitmap _imageUnchecked;
		private Bitmap _imageUncheckedDisabledScaled;
		private Bitmap _imageUncheckedScaled;
		[Category("Settings"), DefaultValue(null)]
		public virtual Bitmap ImageUnchecked
		{
			get => _imageUnchecked;
			set
			{
				if (_imageUnchecked == value)
					return;

				_imageUnchecked = value;
				updateImages(image: false, checkedImage: false);
				UpdateSize();
			}
		}

		private Bitmap _imageCheckedDisabled;
		private Bitmap _imageChecked;
		private Bitmap _imageCheckedDisabledScaled;
		private Bitmap _imageCheckedScaled;
		[Category("Settings"), DefaultValue(null)]
		public virtual Bitmap ImageChecked
		{
			get => _imageChecked;
			set
			{
				if (_imageChecked == value)
					return;

				_imageChecked = value;
				updateImages(image: false, uncheckedImage: false);
				UpdateSize();
			}
		}

		public override Color BackColor
		{
			get => base.BackColor;
			set
			{
				if (base.BackColor == value)
					return;

				base.BackColor = value;
				updateHighlightColors();
			}
		}

		private Color _mouseOverBackColor;
		private Color _checkedBackColor;
		private Color _focusBorderColor;
		private Color _highlightBackColor = SystemColors.HotTrack;
		[Category("Settings"), DefaultValue(typeof(Color), "HotTrack")]
		public Color HighlightBackColor
		{
			get => _highlightBackColor;
			set
			{
				if (_highlightBackColor == value)
					return;

				_highlightBackColor = value;
				updateHighlightColors();
			}
		}

		private int _highlightMouseOverOpacity = 64;
		[DefaultValue(64), Category("Settings")]
		public virtual int HighlightMouseOverOpacity
		{
			get => _highlightMouseOverOpacity;
			set
			{
				if (_highlightMouseOverOpacity != value)
				{
					_highlightMouseOverOpacity = value;
					updateHighlightColors();
				}
			}
		}

		private int _highlightCheckedOpacity = 96;
		[DefaultValue(96), Category("Settings")]
		public virtual int HighlightCheckedOpacity
		{
			get => _highlightCheckedOpacity;
			set
			{
				if (_highlightCheckedOpacity != value)
				{
					_highlightCheckedOpacity = value;
					updateHighlightColors();
				}
			}
		}

		private int _highlightFocusOpacity = 96;
		[DefaultValue(96), Category("Settings")]
		public virtual int HighlightFocusOpacity
		{
			get => _highlightFocusOpacity;
			set
			{
				if (_highlightFocusOpacity != value)
				{
					_highlightFocusOpacity = value;
					updateHighlightColors();
				}
			}
		}

		private bool _checked;
		[Category("Settings"), DefaultValue(false)]
		public bool Checked
		{
			get => _checked;
			set
			{
				if (_checked == value)
					return;

				_checked = value;
				Invalidate();
				CheckedChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		private bool _mouseOver;
		private bool MouseOver
		{
			get => _mouseOver;
			set
			{
				if (_mouseOver == value)
					return;

				_mouseOver = value;
				Invalidate();
			}
		}
	}
}