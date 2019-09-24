using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Mtgdb.Data;

namespace Mtgdb.Controls
{
	public class FieldControl : UserControl
	{
		public event Action<FieldControl> Invalid;

		public FieldControl()
		{
			init();
			Paint += paint;
			TextSelection.Changed += selectionChanged;
			BackColor = SystemColors.Window;
			ForeColor = SystemColors.WindowText;
		}

		private void paint(object sender, PaintEventArgs e)
		{
			var graphics = e.Graphics;

			var zeroPoint = Point.Empty;
			var area = getArea(zeroPoint, zeroPoint, new Padding(0));
			var contentArea = getArea(zeroPoint, zeroPoint, Padding);

			using (var borderPen = new Pen(Color.Black) {DashStyle = DashStyle.Dot})
			{
				graphics.DrawRectangle(borderPen,
					new Rectangle(
						area.Location,
						new Size(area.Width - 1, area.Height - 1)));

				graphics.DrawRectangle(borderPen,
					new Rectangle(
						contentArea.Location,
						new Size(contentArea.Width - 1, contentArea.Height - 1)));
			}

			if (Image != null)
				paintImage(graphics, contentArea);

			if (!string.IsNullOrEmpty(DataText) && contentArea.Width > 0 && contentArea.Height > 0)
			{
				paintText(
					graphics,
					new HighlightOptions(),
					new RectangularSelection(),
					new SelectionOptions(),
					contentArea,
					isTextSelecting: false);
			}
		}

		public void PaintSelf(
			Graphics graphics,
			Point parentLocation,
			HighlightOptions highlightOptions,
			RectangularSelection selection,
			SelectionOptions selectionOptions)
		{
			var contentArea = getArea(parentLocation, Location, Padding);
			var completeArea = getArea(parentLocation, Location, new Padding(0));

			if (Image != null)
				paintImage(graphics, contentArea);

			if (!string.IsNullOrEmpty(DataText) && contentArea.Width > 0 && contentArea.Height > 0)
			{
				bool isTextSelecting = selection.Selecting && completeArea.Contains(selection.Start);

				paintText(
					graphics,
					highlightOptions,
					selection,
					selectionOptions,
					contentArea,
					isTextSelecting);
			}
		}

		private void paintImage(Graphics graphics, Rectangle contentArea)
		{
			var imageArea = Image.Size.FitIn(contentArea);

			switch (HorizontalAlignment)
			{
				case HorizontalAlignment.Right:
					imageArea.Offset(contentArea.Width - imageArea.Width, 0);
					break;

				case HorizontalAlignment.Center:
					imageArea.Offset((contentArea.Width - imageArea.Width) / 2, 0);
					break;
			}

			graphics.DrawImage(Image, imageArea);
		}

		private void paintText(
			Graphics graphics,
			HighlightOptions highlightOptions,
			RectangularSelection selection,
			SelectionOptions selectionOptions,
			Rectangle contentArea,
			bool isTextSelecting)
		{
			_paintInProgress = true;

			if (selection.Selecting)
				TextSelection.Clear();

			var context = new RichTextRenderContext
			{
				Text = DataText,
				TextSelection = TextSelection.Clone(),
				HighlightRanges = HighlightRanges,
				Graphics = graphics,
				Rect = contentArea,
				HorizAlignment = HorizontalAlignment,
				Font = Font,
				ForeColor = ForeColor,

				HighlightContextColor = highlightOptions.HighlightContextColor,
				HighlightColor = highlightOptions.HighlightColor,
				HighlightBorderColor = highlightOptions.HighlightBorderColor,
				HighlightBorderWidth = 1f
			};

			if (isTextSelecting)
			{
				context.Selecting = true;
				context.SelectionStart = selection.Start;
				context.SelectionEnd = selection.End;
			}

			context.SelectionBackColor = selectionOptions.BackColor;
			context.SelectionForeColor = selectionOptions.ForeColor;
			context.SelectionAlpha = selectionOptions.Alpha;

			RichTextRenderer.Render(context, _iconRecognizer);

			if (selection.Selecting)
				TextSelection.SetSelection(context.TextSelection);

			_paintInProgress = false;
		}

		private Rectangle getArea(Point parentLocation, Point location, Padding padding)
		{
			return new Rectangle(
				new Point(
					parentLocation.X + location.X + padding.Left,
					parentLocation.Y + location.Y + padding.Top),
				new Size(
					Size.Width - padding.Horizontal,
					Size.Height - padding.Vertical));
		}

		private void init()
		{
			SuspendLayout();
			Name = "FieldControl";
			ResumeLayout(false);
		}



		private void selectionChanged(TextSelection obj)
		{
			if (_paintInProgress)
				return;

			Invalid?.Invoke(this);
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
					Invalidate();
				}
			}
		}

		[Category("Settings"), DefaultValue(true)]
		public bool AllowHotTrack { get; set; } = true;


		[Category("Settings"), DefaultValue(null)]
		public Image Image
		{
			get => _image;
			set
			{
				if (_image == value)
					return;

				_image = value;
				Invalid?.Invoke(this);
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public string DataText
		{
			get => Text;
			set
			{
				if (Text == value)
					return;

				Text = value;
				TextSelection.Text = value;
				TextSelection.Clear();

				Invalid?.Invoke(this);
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public string FieldName { get; set; }

		[Category("Settings"), DefaultValue(true)]
		public bool AllowSort { get; set; } = true;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SearchOptions SearchOptions { get; set; } = new SearchOptions();

		[Category("Settings"), DefaultValue(typeof(Color), "Window")]
		public override Color BackColor {
			get => base.BackColor;
			set => base.BackColor = value;
		}

		[Category("Settings"), DefaultValue(typeof(Color), "WindowText")]
		public override Color ForeColor {
			get => base.ForeColor;
			set => base.ForeColor = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
				if (!AllowHotTrack && value)
					return;

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
		public SortDirection SortOrder
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
		public TextSelection TextSelection { get; } = new TextSelection();

		private IconRecognizer _iconRecognizer;
		private Image _image;
		private IList<TextRange> _highlightRanges;
		private HorizontalAlignment _horizontalAlignment;

		private bool _isHotTracked;
		private bool _isSortHotTracked;
		private bool _isSortVisible;

		private bool _isSearchHotTracked;
		private bool _isSearchVisible;

		private SortDirection _sortOrder;
		private int _hotTrackedCustomButtonIndex = -1;
		private bool _paintInProgress;
	}
}