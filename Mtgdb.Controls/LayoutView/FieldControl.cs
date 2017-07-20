using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class FieldControl : UserControl
	{
		public event Action<FieldControl> Invalid;

		public FieldControl()
		{
			init();
			Paint += paint;
		}

		private void paint(object sender, PaintEventArgs e)
		{
			var graphics = e.Graphics;

			var area = getArea(new Point(0, 0), new Padding(0));
			var contentArea = getArea(new Point(0, 0), Padding);

			Pen borderPen = new Pen(Color.Black)
			{
				DashStyle = DashStyle.Dot
			};

			graphics.DrawRectangle(borderPen,
				new Rectangle(
					area.Location,
					new Size(area.Width - 1, area.Height - 1)));

			graphics.DrawRectangle(borderPen,
				new Rectangle(
					contentArea.Location,
					new Size(contentArea.Width - 1, contentArea.Height - 1)));
		}

		public void PaintSelf(Graphics graphics, Point parentLocation, HighlightSettings highlightSettings)
		{
			var contentArea = getArea(parentLocation, Padding);

			if (!string.IsNullOrEmpty(Text))
			{
				var context = new RichTextRenderContext
				{
					Text = Text,
					HighlightRanges = HighlightRanges,
					Graphics = graphics,
					Rect = contentArea,
					HorizAlignment = HorizontalAlignment,
					StringFormat = new StringFormat(default(StringFormatFlags)),
					Font = Font,
					ForeColor = ForeColor,
					BackgroundColor = BackColor,
					HighlightContextColor = highlightSettings.HighlightContextColor,
					HighlightColor = highlightSettings.HighlightColor,
					HighlightBorderColor = highlightSettings.HighlightBorderColor,
					HighlightBorderWidth = 1f
				};

				RichTextRenderer.Render(context, _iconRecognizer);
			}
			else if (Image != null)
				graphics.DrawImageUnscaledAndClipped(Image, contentArea);
		}

		private Rectangle getArea(Point parentLocation, Padding padding)
		{
			return new Rectangle(
				new Point(
					parentLocation.X + Location.X + padding.Left,
					parentLocation.Y + Location.Y + padding.Top),
				new Size(
					Size.Width - padding.Horizontal,
					Size.Height - padding.Vertical));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				components?.Dispose();

			base.Dispose(disposing);
		}

		private void init()
		{
			SuspendLayout();
			Name = "FieldControl";
			ResumeLayout(false);
		}



		[Category("Settings"), DefaultValue(typeof(HorizontalAlignment), "Left")]
		public HorizontalAlignment HorizontalAlignment
		{
			get { return _horizontalAlignment; }
			set
			{
				if (_horizontalAlignment != value)
				{
					_horizontalAlignment = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public Image Image
		{
			get { return _image; }
			set
			{
				if (_image != value)
				{
					_image = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public override string Text
		{
			get { return base.Text; }
			set
			{
				if (base.Text != value)
				{
					base.Text = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public string FieldName { get; set; }

		[Category("Settings"), DefaultValue(true)]
		public bool AllowSort { get; set; } = true;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsHotTracked
		{
			get { return _isHotTracked; }
			set
			{
				if (_isHotTracked != value)
				{
					_isHotTracked = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IList<TextRange> HighlightRanges
		{
			get { return _highlightRanges; }
			set
			{
				_highlightRanges = value;
				Invalid?.Invoke(this);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IconRecognizer IconRecognizer
		{
			get { return _iconRecognizer; }
			set
			{
				_iconRecognizer = value;
				Invalid?.Invoke(this);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SortOrder SortOrder
		{
			get { return _sortOrder; }
			set
			{
				if (_sortOrder != value)
				{
					_sortOrder = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSortHotTracked
		{
			get { return _isSortHotTracked; }
			set
			{
				if (_isSortHotTracked != value)
				{
					_isSortHotTracked = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSortVisible
		{
			get { return _isSortVisible; }
			set
			{
				if (_isSortVisible != value)
				{
					_isSortVisible = value;
					Invalid?.Invoke(this);
				}
			}
		}


		private readonly IContainer components = null;
		private IconRecognizer _iconRecognizer;
		private Image _image;
		private IList<TextRange> _highlightRanges;
		private HorizontalAlignment _horizontalAlignment;

		private bool _isHotTracked;
		private bool _isSortHotTracked;
		private bool _isSortVisible;
		private SortOrder _sortOrder;
	}
}