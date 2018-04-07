using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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

		public void PaintSelf(
			Graphics graphics,
			Point parentLocation,
			HighlightOptions highlightOptions,
			SelectionState selection,
			SelectionOptions selectionOptions)
		{
			var contentArea = getArea(parentLocation, Padding);
			var completeArea = getArea(parentLocation, new Padding(0));

			if (Image != null)
			{
				graphics.DrawImage(Image, Image.Size.FitIn(contentArea));
				return;
			}

			if (string.IsNullOrEmpty(Text))
				return;

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

				BackColor = IsHotTracked
					? selectionOptions.HotTrackBackColor
					: BackColor,

				HighlightContextColor = highlightOptions.HighlightContextColor,
				HighlightColor = highlightOptions.HighlightColor,
				HighlightBorderColor = highlightOptions.HighlightBorderColor,
				HighlightBorderWidth = 1f
			};

			if (completeArea.Contains(selection.Start))
			{
				context.RectSelected = true;

				context.SelectionEnd = selection.End;
				context.SelectionStart = selection.Start;
				context.SelectionIsAll = selection.SelectAll;

				context.SelectionBackColor = selectionOptions.BackColor;
				context.SelectionForeColor = selectionOptions.ForeColor;
				context.SelectionAlpha = selectionOptions.Alpha;
			}

			RichTextRenderer.Render(context, _iconRecognizer);
			SelectedText = context.SelectedText;
		}

		public void CopyFrom(FieldControl other)
		{
			Location = other.Location;
			Size = other.Size;
			Font = other.Font;
			BackColor = other.BackColor;
			ForeColor = other.ForeColor;
			HorizontalAlignment = other.HorizontalAlignment;
			IconRecognizer = other.IconRecognizer;
			SearchOptions = other.SearchOptions.Clone();
			CustomButtons = other.CustomButtons.Select(_ => _.Clone()).ToList();
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
			get => _horizontalAlignment;
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
			get => _image;
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
			get => base.Text;
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

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public SearchOptions SearchOptions { get; set; } = new SearchOptions();

		[Category("Settings")]
		public List<ButtonOptions> CustomButtons { get; set; } = new List<ButtonOptions>();



		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int HotTrackedCustomButtonIndex
		{
			get => _hotTrackedCustomButtonIndex;
			set
			{
				if (_hotTrackedCustomButtonIndex != value)
				{
					_hotTrackedCustomButtonIndex = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsHotTracked
		{
			get => _isHotTracked;
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
			get => _highlightRanges;
			set
			{
				_highlightRanges = value;
				Invalid?.Invoke(this);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IconRecognizer IconRecognizer
		{
			get => _iconRecognizer;
			set
			{
				_iconRecognizer = value;
				Invalid?.Invoke(this);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SortOrder SortOrder
		{
			get => _sortOrder;
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
			get => _isSortHotTracked;
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
		public bool IsSearchHotTracked
		{
			get => _isSearchHotTracked;
			set
			{
				if (_isSearchHotTracked != value)
				{
					_isSearchHotTracked = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSearchVisible
		{
			get => _isSearchVisible;
			set
			{
				if (_isSearchVisible != value)
				{
					_isSearchVisible = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsSortVisible
		{
			get => _isSortVisible;
			set
			{
				if (_isSortVisible != value)
				{
					_isSortVisible = value;
					Invalid?.Invoke(this);
				}
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string SelectedText { get; private set; }



		private readonly IContainer components = null;
		private IconRecognizer _iconRecognizer;
		private Image _image;
		private IList<TextRange> _highlightRanges;
		private HorizontalAlignment _horizontalAlignment;

		private bool _isHotTracked;
		private bool _isSortHotTracked;
		private bool _isSortVisible;

		private bool _isSearchHotTracked;
		private bool _isSearchVisible;

		private SortOrder _sortOrder;
		private int _hotTrackedCustomButtonIndex = -1;
	}
}