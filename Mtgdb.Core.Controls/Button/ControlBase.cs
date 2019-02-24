using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ControlBase : Control
	{
		public ControlBase()
		{
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.SupportsTransparentBackColor,
				value: true);

			Padding = DefaultPadding;

			BackColor = Color.Transparent;
			ForeColor = SystemColors.ControlText;

			updateDisabledBorder();

			Paint += paint;
			EnabledChanged += enabledChanged;
			ColorSchemeController.SystemColorsChanged += HandleSystemColorsChanged;
			Layout += layout;
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



		protected override void Dispose(bool disposing)
		{
			Layout -= layout;
			Paint -= paint;
			EnabledChanged -= enabledChanged;
			ColorSchemeController.SystemColorsChanged -= HandleSystemColorsChanged;

			base.Dispose(disposing);
		}

		protected virtual void HandleSystemColorsChanged()
		{
			updateDisabledBorder();
			UpdateImages();
			Invalidate();
		}

		private void paint(object sender, PaintEventArgs e) =>
			HandlePaint(e.Graphics);

		protected virtual void HandlePaint(Graphics g)
		{
			var (imageSize, textSize) = measure(g);
			var (textRect, imageRect) = layout();

			paintText();
			paintImage();
			PaintBorder(g);

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

			void paintText()
			{
				if (string.IsNullOrEmpty(Text))
					return;

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

				g.DrawText(Text, Font, textRect, ActualForeColor, format);
			}

			void paintImage()
			{
				var image = SelectImage();
				if (image != null)
					g.DrawImage(image, imageRect);
			}
		}

		protected virtual void PaintBorder(Graphics g) =>
			this.PaintBorder(g, VisibleBorders, ActualBorderColor, BorderStyle);

		protected virtual Bitmap SelectImage()
		{
			return Enabled
				? _imageScaled
				: _imageDisabledScaled;
		}

		protected void UpdateSize()
		{
			if (AutoSize)
				Size = MeasureContent();
		}

		protected Bitmap ToDisabledImage(Bitmap value) =>
			value?.SetOpacity((float) DisabledOpacity / 255);

		protected void UpdateImages(
			bool unscaled = true, bool scaled = true,
			bool disabled = true, bool enabled = true)
		{
			if (unscaled)
			{
				if (disabled)
					_imageDisabled = ToDisabledImage(_image);
			}

			if (scaled)
			{
				if (enabled)
					_imageScaled = _image?.ScaleBy(ImageScale);

				if (disabled)
					_imageDisabledScaled = _imageDisabled?.ScaleBy(ImageScale);
			}

			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs e) =>
			this.PaintPanelBack(e.Graphics, e.ClipRectangle, BackgroundImage, ActualBackColor, PaintBackground);

		protected virtual void HandleImageScaleChange()
		{
			UpdateImages(unscaled: false);
			UpdateSize();
		}


		private (Size imageSize, Size textSize) measure(Graphics g)
		{
			var imageSize = SelectImage()?.Size ?? default;

			var textSize = string.IsNullOrEmpty(Text)
				? default
				: g.MeasureText(Text, Font, _infiniteSize, TextFormat);

			return (imageSize, textSize);
		}

		private void updateDisabledBorder() =>
			_disabledBorderColor = Color.Transparent.BlendWith(_borderColor, DisabledOpacity);

		private void enabledChanged(object sender, EventArgs e) =>
			Invalidate();

		private void layout(object sender, LayoutEventArgs e)
		{
			if (AutoSize)
				Size = MeasureContent();
		}

		protected override Padding DefaultPadding => new Padding(4);

		[Category("Settings"), DefaultValue(typeof(Color), "Transparent")]
		public override Color BackColor
		{
			get => base.BackColor;
			set => base.BackColor = value;
		}

		[Category("Settings"), DefaultValue(typeof(Color), "ControlText")]
		public override Color ForeColor
		{
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		private Color _disabledBorderColor;
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
				Invalidate();
			}
		}

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
					Invalidate();
				}
			}
		}

		[Category("Settings"), DefaultValue(typeof(Color), "GrayText")]
		public Color DisabledForeColor { get; set; } = SystemColors.GrayText;

		private Bitmap _image;
		private Bitmap _imageDisabled;
		protected Bitmap _imageScaled;
		protected Bitmap _imageDisabledScaled;
		[Category("Settings"), DefaultValue(null)]
		public virtual Bitmap Image
		{
			get => _image;
			set
			{
				if (_image == value)
					return;

				_image = value;
				UpdateImages();
				UpdateSize();
			}
		}

		private float _imageScale = 1f;
		[Category("Settings"), DefaultValue(1f)]
		public float ImageScale
		{
			get => _imageScale;
			set
			{
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (_imageScale == value)
					return;

				_imageScale = value;
				HandleImageScaleChange();
			}
		}

		[Category("Settings"), DefaultValue("")]
		public override string Text
		{
			get => base.Text;
			set
			{
				base.Text = value;
				UpdateSize();
				Invalidate();
			}
		}

		[Category("Settings")]
		public override Font Font
		{
			get => base.Font;
			set
			{
				if (Equals(base.Font, value))
					return;

				base.Font = value;
				UpdateSize();
				Invalidate();
			}
		}

		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Category("Settings"), DefaultValue(false)]
		public override bool AutoSize
		{
			get => base.AutoSize;
			set => base.AutoSize = value;
		}

		[Category("Settings"), DefaultValue(true)]
		public bool PaintBackground { get; set; } = true;

		[Category("Settings"), DefaultValue(typeof(AnchorStyles), "None")]
		public virtual AnchorStyles VisibleBorders { get; set; } = AnchorStyles.None;

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
				UpdateSize();
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
		public StringAlignment TextAlign
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



		protected Color ActualBorderColor => Enabled ? BorderColor : _disabledBorderColor;

		protected Color ActualForeColor => Enabled ? ForeColor : DisabledForeColor;

		protected virtual Color ActualBackColor => BackColor;

		private const TextFormatFlags TextFormat =
			TextFormatFlags.NoClipping |
			TextFormatFlags.NoPrefix |
			TextFormatFlags.VerticalCenter |
			TextFormatFlags.TextBoxControl;

		private static readonly Size _infiniteSize = new Size(int.MaxValue, int.MaxValue);
	}
}