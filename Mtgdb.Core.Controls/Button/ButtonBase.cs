using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ButtonBase : Control
	{
		public ButtonBase()
		{
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.SupportsTransparentBackColor |
				ControlStyles.Selectable,
				value: true);

			BackColor = Color.Transparent;
			ForeColor = SystemColors.ControlText;

			Padding = new Padding(4);

			updateHighlightColors();
			updateDisabledBorder();

			EnabledChanged += enabledChanged;
			Paint += paint;
			MouseDown += mouseDown;
			MouseClick += mouseClick;
			MouseEnter += mouseEnter;
			MouseLeave += mouseLeave;
			GotFocus += gotFocus;
			LostFocus += lostFocus;
			KeyDown += keyDown;
			ColorSchemeController.SystemColorsChanged += systemColorsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			EnabledChanged -= enabledChanged;
			Paint -= paint;
			MouseDown -= mouseDown;
			MouseClick -= mouseClick;
			MouseEnter -= mouseEnter;
			MouseLeave -= mouseLeave;
			GotFocus -= gotFocus;
			LostFocus -= lostFocus;
			KeyDown -= keyDown;
			ColorSchemeController.SystemColorsChanged -= systemColorsChanged;

			base.Dispose(disposing);
		}

		private void paint(object sender, PaintEventArgs e)
		{
			var (imageSize, textSize) = measure(e.Graphics);
			var (textRect, imageRect) = layout();

			paintText();
			paintImage();
			paintBorder();
			paintFocusRectangle();

			void paintImage()
			{
				var image = Enabled ? Image : _imageDisabled;
				if (image != null)
					e.Graphics.DrawImage(image, imageRect);
			}

			void paintText()
			{
				if (!string.IsNullOrEmpty(Text))
				{
					var foreColor = Enabled ? ForeColor : DisabledForeColor;
					var format = TextFormat;

					switch (TextAlign)
					{
						case StringAlignment.Far:
							format |= TextFormatFlags.Right;
							break;

						case StringAlignment.Center:
							format |= TextFormatFlags.HorizontalCenter;
							break;
					}

					e.Graphics.DrawText(Text, Font, textRect, foreColor, format);
				}
			}

			void paintBorder()
			{
				var borderColor = Enabled ? BorderColor : _disabledBorderColor;
				this.PaintBorder(e.Graphics, VisibleBorders, borderColor, BorderStyle);
			}

			void paintFocusRectangle()
			{
				if (ContainsFocus && _focusBorderColor.A > 0)
				{
					var rectangle = new Rectangle(default, Size);
					int width = 2;
					rectangle.Inflate(-(width - 1), -(width - 1));
					var pen = new Pen(_focusBorderColor) { Width = width, DashStyle = DashStyle.Dot };

					e.Graphics.DrawRectangle(pen, rectangle);
				}
			}

			(Rectangle textRect, Rectangle imageRect) layout()
			{
				int imageIndex;
				int textIndex;
				switch (TextImageRelation)
				{
					case TextImageRelation.ImageAboveText:
					case TextImageRelation.ImageBeforeText:
						imageIndex = 0;
						textIndex = 1;
						break;
					default:
						imageIndex = 1;
						textIndex = 0;
						break;
				}

				Size[] sizes = new Size[2];
				sizes[imageIndex] = imageSize;
				sizes[textIndex] = textSize;

				var aligns = new StringAlignment[2];
				aligns[imageIndex] = ImagePosition;
				aligns[textIndex] = TextPosition;

				Size containerSize = Size;
				Padding containerPadding = Padding;
				bool vertical = false;
				switch (TextImageRelation)
				{
					case TextImageRelation.ImageAboveText:
					case TextImageRelation.TextAboveImage:
						vertical = true;
						containerSize = containerSize.Transpose();
						containerPadding = containerPadding.Transpose();
						sizes[0] = sizes[0].Transpose();
						sizes[1] = sizes[1].Transpose();
						break;
				}

				Rectangle[] rectangles = new Rectangle[2];

				int contentWidth = sizes.Sum(_ => _.Width);
				int containerWidth = containerSize.Width - containerPadding.Horizontal;
				int freeWidth = containerWidth - contentWidth;

				int left;
				if (aligns[0] == StringAlignment.Near)
					left = containerPadding.Left;
				else if (aligns[0] == StringAlignment.Center || aligns[0] == StringAlignment.Far && aligns[1] != aligns[0] && sizes[1].Width > 0)
					left = containerPadding.Left + freeWidth / 2;
				else // aligns[0] == StringAlignment.Far
					left = containerPadding.Left + freeWidth;

				rectangles[0] = centerVertically(sizes[0], left);

				int right;
				if (aligns[1] == StringAlignment.Far)
					right = containerSize.Width - containerPadding.Right;
				else if (aligns[1] == StringAlignment.Center || aligns[1] == StringAlignment.Near && aligns[0] != aligns[1] && sizes[0].Width > 0)
					right = containerSize.Width - containerPadding.Right - freeWidth / 2;
				else // aligns[1] == StringAlignment.Near
					right = containerSize.Width - containerPadding.Right - freeWidth;

				rectangles[1] = centerVertically(sizes[1], right - sizes[1].Width);

				if (vertical)
				{
					rectangles[imageIndex] = rectangles[imageIndex].Transpose();
					rectangles[textIndex] = rectangles[textIndex].Transpose();
				}

				return (textRect: rectangles[textIndex], imageRect: rectangles[imageIndex]);

				Rectangle centerVertically(Size itemSize, int x) =>
					new Rectangle(new Point(x, centerTop(itemSize)), itemSize);

				int centerTop(Size itemSize) =>
					containerPadding.Top + (containerSize.Height - containerPadding.Vertical - itemSize.Height) / 2;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs e) =>
			this.PaintPanelBack(e.Graphics, e.ClipRectangle, BackgroundImage, getBgColor(), PaintBackground);

		private Color getBgColor()
		{
			if (MouseOver)
				return _mouseOverBackColor;

			if (Checked)
				return _checkedBackColor;

			return BackColor;
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

		private void systemColorsChanged()
		{
			updateHighlightColors();
			updateDisabledBorder();
			updateDisabledImage();
			Invalidate();
		}

		private void enabledChanged(object sender, EventArgs e) =>
			Invalidate();

		private void updateSize()
		{
			if (AutoSize)
				Size = MeasureContent();
		}

		public Size MeasureContent()
		{
			var g = CreateGraphics();
			var (imageSize, textSize) = measure(g);

			switch (TextImageRelation)
			{
				case TextImageRelation.ImageBeforeText:
				case TextImageRelation.TextBeforeImage:
					return new Size(
						imageSize.Width + textSize.Width + Padding.Horizontal,
						Math.Max(imageSize.Height, textSize.Height) + Padding.Vertical);

				case TextImageRelation.ImageAboveText:
				case TextImageRelation.TextAboveImage:
					return new Size(
						Math.Max(imageSize.Width, textSize.Width) + Padding.Horizontal,
						imageSize.Height + textSize.Height + Padding.Vertical);

				case TextImageRelation.Overlay:
					return new Size(
						Math.Max(imageSize.Width, textSize.Width) + Padding.Horizontal,
						Math.Max(imageSize.Height, textSize.Height) + Padding.Vertical);

				default:
					throw new NotSupportedException();
			}
		}

		private void updateHighlightColors()
		{
			if (DesignMode)
				return;

			_mouseOverBackColor = blend(BackColor, _highlightBackColor, _highlightMouseOverOpacity);
			_checkedBackColor = blend(BackColor, _highlightBackColor, _highlightCheckedOpacity);
			_focusBorderColor = blend(BackColor, _highlightBackColor, _highlightFocusOpacity);
		}

		private static Color blend(Color bg, Color fore, int foreOpacity)
		{
			if (bg == Color.Empty || bg == Color.Transparent)
				return Color.FromArgb(foreOpacity, fore);

			byte o1 = bg.A;
			return Color.FromArgb(
				(255 * 255 - (255 - o1) * (255 - foreOpacity)) / 255,
				blendBytes(bg.R, fore.R),
				blendBytes(bg.G, fore.G),
				blendBytes(bg.B, fore.B));

			int blendBytes(byte b1, byte b2)
			{
				int share2 = 255 * foreOpacity;
				int share1 = o1 * (255 - foreOpacity);

				return (b1 * share1 + b2 * share2) / (share1 + share2);
			}
		}

		private void updateDisabledBorder() =>
			_disabledBorderColor = blend(Color.Transparent, _borderColor, _disabledOpacity);

		private void updateDisabledImage() =>
			_imageDisabled = _image?.SetOpacity((float) _disabledOpacity / 255);

		private (Size imageSize, Size textSize) measure(Graphics g)
		{
			var imageSize = Image?.Size ?? default;
			var textSize = string.IsNullOrEmpty(Text)
				? default
				: g.MeasureText(Text, Font, _infiniteSize, TextFormat);

			return (imageSize, textSize);
		}

		private void updateImage() =>
			Image = _buttonImages?.GetImage(Checked);


		public event EventHandler CheckedChanged;

		public event EventHandler PressDown;
		public event EventHandler Pressed;


		[Category("Settings"), DefaultValue("")]
		public override string Text
		{
			get => base.Text;
			set
			{
				base.Text = value;
				updateSize();
				Invalidate();
			}
		}

		private Bitmap _imageDisabled;
		private Bitmap _image;

		[Category("Settings"), DefaultValue(null)]
		public virtual Bitmap Image
		{
			get => _image;
			set
			{
				if (_image == value)
					return;

				_image = value;

				updateDisabledImage();
				updateSize();
				Invalidate();
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
		private Color _disabledBorderColor;
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

		private Color _borderColor = SystemColors.ActiveBorder;
		[Category("Settings"), DefaultValue(typeof(Color), "ActiveBorder")]
		public Color BorderColor
		{
			get => _borderColor;
			set
			{
				if (_borderColor == value)
					return;

				_borderColor = value;
				updateDisabledBorder();
			}
		}

		[Category("Settings"), DefaultValue(typeof(Color), "GrayText")]
		public Color DisabledForeColor { get; set; } = SystemColors.GrayText;

		private int _disabledOpacity = 128;
		[DefaultValue(128), Category("Settings")]
		public virtual int DisabledOpacity
		{
			get => _disabledOpacity;
			set
			{
				if (_disabledOpacity != value)
				{
					_disabledOpacity = value;
					updateDisabledBorder();
					updateDisabledImage();
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
				updateImage();
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
				if (_mouseOver != value)
				{
					_mouseOver = value;
					Invalidate();
				}
			}
		}

		[Category("Settings"), DefaultValue(false)]
		public override bool AutoSize
		{
			get => base.AutoSize;
			set => base.AutoSize = value;
		}

		[Category("Settings"), DefaultValue(typeof(AnchorStyles), "None")]
		public virtual AnchorStyles VisibleBorders { get; set; } = AnchorStyles.None;

		[Category("Settings"), DefaultValue(true)]
		public bool PaintBackground { get; set; } = true;

		[Category("Settings"), DefaultValue(true)]
		public virtual bool AutoCheck { get; set; } = true;

		[Category("Settings"), DefaultValue(false)]
		public virtual bool? VisibleAllBorders
		{
			get
			{
				switch (VisibleBorders)
				{
					case ControlHelpers.AnchorAll:
						return true;
					case AnchorStyles.None:
						return false;
					default:
						return null;
				}
			}

			set
			{
				if (!value.HasValue)
					return;

				if (value.Value)
					VisibleBorders = ControlHelpers.AnchorAll;
				else
					VisibleBorders = AnchorStyles.None;
			}
		}

		[Category("Settings"), DefaultValue(typeof(DashStyle), "Solid")]
		public DashStyle BorderStyle { get; set; } = DashStyle.Solid;

		private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;

		[Category("Settings"), DefaultValue(typeof(TextImageRelation), "ImageBeforeText")]
		public virtual TextImageRelation TextImageRelation
		{
			get => _textImageRelation;
			set
			{
				if (_textImageRelation == value)
					return;

				_textImageRelation = value;
				updateSize();
				Invalidate();
			}
		}

		private StringAlignment _textPosition = StringAlignment.Near;

		[Category("Settings"), DefaultValue(typeof(StringAlignment), "Near")]
		public virtual StringAlignment TextPosition
		{
			get => _textPosition;
			set
			{
				if (_textPosition == value)
					return;

				_textPosition = value;
				Invalidate();
			}
		}

		private StringAlignment _textAlign = StringAlignment.Near;

		[Category("Settings"), DefaultValue(typeof(StringAlignment), "Near")]
		public virtual StringAlignment TextAlign
		{
			get => _textAlign;
			set
			{
				if (_textAlign == value)
					return;

				_textAlign = value;
				Invalidate();
			}
		}

		private StringAlignment _imagePosition = StringAlignment.Center;

		[Category("Settings"), DefaultValue(typeof(StringAlignment), "Center")]
		public virtual StringAlignment ImagePosition
		{
			get => _imagePosition;
			set
			{
				if (_imagePosition == value)
					return;

				_imagePosition = value;
				Invalidate();
			}
		}

		private ButtonImages _buttonImages;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ButtonImages ButtonImages
		{
			get => _buttonImages;
			set
			{
				if (_buttonImages == value)
					return;

				_buttonImages = value;
				updateImage();
			}
		}

		private const TextFormatFlags TextFormat =
			TextFormatFlags.NoClipping |
			TextFormatFlags.NoPrefix |
			TextFormatFlags.VerticalCenter |
			TextFormatFlags.TextBoxControl;

		private static readonly Size _infiniteSize = new Size(int.MaxValue, int.MaxValue);
	}
}