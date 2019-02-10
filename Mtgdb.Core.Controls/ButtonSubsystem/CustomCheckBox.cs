using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class CustomCheckBox : Control
	{
		public CustomCheckBox()
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

			TabStop = false;
			BackColor = Color.Transparent;
			ForeColor = SystemColors.ControlText;
			Padding = new Padding(4);

			updateHighlightColors();
			updateDisabledBorder();

			ColorSchemeController.SystemColorsChanging += systemColorsChanging;
			EnabledChanged += enabledChanged;
			Paint += paint;
			MouseClick += mouseClick;
			MouseEnter += mouseEnter;
			MouseLeave += mouseLeave;
		}

		private void paint(object sender, PaintEventArgs e)
		{
			var image = Enabled ? Image : _imageDisabled;

			var (imageSize, textSize) = measure(e.Graphics);

			int x = Padding.Left + (Width - Padding.Horizontal - textSize.Width - imageSize.Width) / 2;
			int y = Padding.Top + (Height - Padding.Vertical - textSize.Height - imageSize.Height) / 2;

			Rectangle textRect;
			Rectangle imageRect;

			switch (TextImageRelation)
			{
				case TextImageRelation.ImageBeforeText:
					imageRect = alignVertically(imageSize);
					x = imageRect.Right;
					textRect = alignVertically(textSize);
					break;

				case TextImageRelation.TextBeforeImage:
					textRect = alignVertically(textSize);
					x = textRect.Right;
					imageRect = alignVertically(imageSize);
					break;

				case TextImageRelation.ImageAboveText:
					imageRect = alignHorizontally(imageSize);
					y = imageRect.Bottom;
					textRect = alignHorizontally(textSize);
					break;

				case TextImageRelation.TextAboveImage:
					textRect = alignHorizontally(textSize);
					y = textRect.Bottom;
					imageRect = alignHorizontally(imageSize);
					break;

				default:
					throw new NotSupportedException();
			}

			if (image != null)
				e.Graphics.DrawImage(image, imageRect);

			if (!string.IsNullOrEmpty(Text))
			{
				var foreColor = Enabled ? ForeColor : DisabledForeColor;
				e.Graphics.DrawText(Text, Font, textRect, foreColor, TextFormat);
			}

			paintBorder();

			void paintBorder()
			{
				var borderColor = Enabled ? BorderColor : _disabledBorderColor;
				this.PaintBorder(e.Graphics, VisibleBorders, borderColor, BorderDashStyle);
			}

			Rectangle alignVertically(Size itemSize) =>
				new Rectangle(new Point(x, Padding.Top + (Height - Padding.Vertical - itemSize.Height) / 2), itemSize);

			Rectangle alignHorizontally(Size itemSize) =>
				new Rectangle(new Point(Padding.Left + (Width - Padding.Horizontal - itemSize.Width) / 2, y), itemSize);
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

		private void mouseClick(object sender, MouseEventArgs e)
		{
			if (Enabled && AutoCheck)
				Checked = !Checked;
		}

		private void systemColorsChanging()
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
			if (!AutoSize)
				return;

			var g = CreateGraphics();
			var (imageSize, textSize) = measure(g);

			switch (TextImageRelation)
			{
				case TextImageRelation.ImageBeforeText:
				case TextImageRelation.TextBeforeImage:
					Size = new Size(
						imageSize.Width + textSize.Width + Padding.Horizontal,
						Math.Max(imageSize.Height, textSize.Height) + Padding.Vertical);
					break;
				case TextImageRelation.ImageAboveText:
				case TextImageRelation.TextAboveImage:
					Size = new Size(
						Math.Max(imageSize.Width, textSize.Width) + Padding.Horizontal,
						imageSize.Height + textSize.Height + Padding.Vertical);
					break;
				case TextImageRelation.Overlay:
					Size = new Size(
						Math.Max(imageSize.Width, textSize.Width) + Padding.Horizontal,
						Math.Max(imageSize.Height, textSize.Height) + Padding.Vertical);
					break;
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

		public event EventHandler CheckedChanged;

		[Category("Settings"), DefaultValue("")]
		public override string Text
		{
			get => base.Text;
			set
			{
				base.Text = value;
				updateSize();
			}
		}

		private Bitmap _imageDisabled;
		private Bitmap _image;
		public Bitmap Image
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

		private Color _mouseOverBackColor;
		private Color _checkedBackColor;
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
		public int HighlightMouseOverOpacity
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
		public int DisabledOpacity
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

		[Category("Settings"), DefaultValue(typeof(AnchorStyles), "None")]
		public AnchorStyles VisibleBorders { get; set; } = AnchorStyles.None;

		[Category("Settings"), DefaultValue(true)]
		public bool PaintBackground { get; set; } = true;

		[Category("Settings"), DefaultValue(typeof(DashStyle), "Solid")]
		public DashStyle BorderDashStyle { get; set; } = DashStyle.Solid;

		[Category("Settings"), DefaultValue(true)]
		public bool AutoCheck { get; set; } = true;

		[Category("Settings"), DefaultValue(false)]
		public bool? VisibleAllBorders
		{
			get
			{
				switch (VisibleBorders)
				{
					case AnchorAll:
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
					VisibleBorders = AnchorAll;
				else
					VisibleBorders = AnchorStyles.None;
			}
		}

		private TextImageRelation _textImageRelation = TextImageRelation.ImageBeforeText;
		[Category("Settings"), DefaultValue(typeof(TextImageRelation), "ImageBeforeText")]
		public TextImageRelation TextImageRelation
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

		private const TextFormatFlags TextFormat =
			TextFormatFlags.NoClipping |
			TextFormatFlags.NoPrefix |
			TextFormatFlags.VerticalCenter |
			TextFormatFlags.TextBoxControl;

		private const AnchorStyles AnchorAll =
			AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

		private static readonly Size _infiniteSize = new Size(int.MaxValue, int.MaxValue);
	}
}