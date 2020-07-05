using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomScrollbar
{
	[Designer(typeof(ScrollbarControlDesigner))]
	public class Scrollbar : UserControl
	{
		public new event EventHandler Scroll;
		public event EventHandler ValueChanged;

		public Scrollbar()
		{
			MouseMove += mouseMove;
			MouseUp += mouseUp;
			MouseEnter += mouseEnter;
			MouseLeave += mouseLeave;

			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.Selectable, false);

			PenWidth = 2;
		}


		private int getVerticalRange() =>
			getTrackHeight() - getThumbHeight();

		private int getTrackHeight() =>
			Height - getUpRect().Height - getDownRect().Height;

		private int getThumbHeight()
		{
			int trackHeight = getTrackHeight();

			if (Maximum == Minimum)
				return trackHeight;

			return Math.Min(
				Math.Max(Width, trackHeight * LargeChange / (LargeChange + Maximum - Minimum)),
				trackHeight);
		}

		private Rectangle getThumbRect() =>
			new Rectangle(0, getThumbRectTop(), Width, getThumbHeight());

		private int getThumbRectTop() =>
			ThumbTop + getUpRect().Height;

		private Rectangle getUpRect()
		{
			if (Height >= 2 * Width)
				return new Rectangle(0, 0, Width, Width);

			return new Rectangle(0, 0, Width, Height / 2);
		}

		private Rectangle getDownRect()
		{
			if (Height >= 2 * Width)
				return new Rectangle(0, Bottom - Width, Width, Width);

			var h = Height - Height / 2;
			return new Rectangle(0, Bottom - h, Width, h);
		}


		[Category("Behavior")]
		public int Value
		{
			get => _value;
			set
			{
				value = Math.Min(Math.Max(Minimum, value), Maximum);

				if (_value == value)
					return;

				_value = value;

				if (Maximum == Minimum)
					_thumbTop = 0;
				else
					_thumbTop = getVerticalRange() * (value - Minimum) / (Maximum - Minimum);

				Refresh();
				ValueChanged?.Invoke(this, EventArgs.Empty);
				Scroll?.Invoke(this, EventArgs.Empty);
			}
		}

		private int ThumbTop
		{
			get => _thumbTop;
			set
			{
				int verticalRange = getVerticalRange();

				value = Math.Min(Math.Max(0, value), verticalRange);

				if (_thumbTop == value)
					return;

				_thumbTop = value;

				var prevValue = Value;
				Value = Minimum + value * (Maximum - Minimum) / verticalRange;

				if (Value != prevValue)
					Refresh();
			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.FillRectangle(SystemBrushes.Control, Rectangle.FromLTRB(1, 1, Width - 1, Height - 1));

			var arrowBackBrush =
				_hover
					? SystemBrushes.ActiveBorder
					: SystemBrushes.Control;
			var arrowForeColor =
				_hover
					? SystemColors.Control
					: SystemColors.ButtonShadow;
			using (var arrowForePen = new Pen(arrowForeColor, PenWidth))
			{
				drawArrow(e.Graphics, getUpRect(), isUp: true, arrowBackBrush, arrowForePen);
				drawArrow(e.Graphics, getDownRect(), isUp: false, arrowBackBrush, arrowForePen);
			}

			if (Maximum == Minimum)
				return;

			int thumbHeight = getThumbHeight();
			if (thumbHeight <= 0)
				return;

			int top = getThumbRectTop();
			var rect = new Rectangle(0, top, Width, thumbHeight);
			var thumbBrush =
				_hover
					? SystemBrushes.ButtonShadow
					: SystemBrushes.ActiveBorder;
			e.Graphics.FillRectangle(thumbBrush, rect);
		}


		private void drawArrow(Graphics g, Rectangle rect, bool isUp, Brush backBrush, Pen forePen)
		{
			g.FillRectangle(backBrush, rect);
			var arrowPts = isUp
				? getUpArrowPolygon(rect)
				: getDownArrowPolygon(rect);

			g.DrawLines(forePen, arrowPts);
		}

		private static Point[] getUpArrowPolygon(Rectangle rect)
		{
			int arrowHeight = rect.Height / 4;
			int arrowWidth = arrowHeight * 2;
			int arrowLeft = rect.Left + (rect.Width - arrowWidth) / 2;
			int arrowTop = rect.Top + (rect.Height - arrowHeight) / 2;
			int x1 = arrowLeft;
			int x2 = arrowLeft + arrowWidth / 2;
			int x3 = arrowLeft + arrowWidth;
			int y1 = arrowTop;
			int y2 = arrowTop + arrowHeight;
			return new[]
			{
				new Point(x1, y2),
				new Point(x2, y1),
				new Point(x3, y2)
			};
		}

		private static Point[] getDownArrowPolygon(Rectangle rect)
		{
			int arrowHeight = rect.Height / 4;
			int arrowWidth = arrowHeight * 2;
			int arrowLeft = rect.Left + (rect.Width - arrowWidth) / 2;
			int arrowTop = rect.Top + (rect.Height - arrowHeight) / 2;
			int x1 = arrowLeft;
			int x2 = arrowLeft + (arrowWidth / 2);
			int x3 = arrowLeft + arrowWidth;
			int y1 = arrowTop;
			int y2 = arrowTop + arrowHeight;
			return new[]
			{
				new Point(x1, y1),
				new Point(x2, y2),
				new Point(x3, y1)
			};
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// base.OnMouseDown is not called intentionally
			// to prevent stealing focus from container

			if (e.Button != MouseButtons.Left)
				return;

			if (!ClientRectangle.Contains(e.Location))
				return;

			if (!Enabled)
				return;

			var thumbRect = getThumbRect();
			if (thumbRect.Contains(e.Location))
			{
				_clickThumbY = e.Y - ThumbTop;
				_thumbDown = true;
			}
			else
			{
				var upRect = getUpRect();
				var downRect = getDownRect();
				int delta;

				if (upRect.Contains(e.Location))
					delta = -SmallChange;
				else if (downRect.Contains(e.Location))
					delta = SmallChange;
				else if (e.Location.Y < thumbRect.Top)
					delta = -LargeChange;
				else if (e.Location.Y >= thumbRect.Bottom)
					delta = LargeChange;
				else
					return;

				Value += delta;
			}
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (!Enabled)
				return;

			if (_thumbDown)
				ThumbTop = e.Y - _clickThumbY;
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				_thumbDown = false;
		}

		private void mouseEnter(object sender, EventArgs e)
		{
			_hover = true;
			Refresh();
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			_hover = false;
			Refresh();
		}

		[Category("Behavior")]
		public int LargeChange
		{
			get => _largeChange;
			set
			{
				_largeChange = value;
				Invalidate();
			}
		}

		[Category("Behavior")]
		public int SmallChange
		{
			get => _smallChange;
			set
			{
				_smallChange = value;
				Invalidate();
			}
		}

		[Category("Behavior")]
		public int Minimum
		{
			get => _minimum;
			set
			{
				_minimum = value;
				Invalidate();
			}
		}

		[Category("Behavior")]
		public int Maximum
		{
			get => _maximum;
			set
			{
				_maximum = value;
				Invalidate();
			}
		}

		public int PenWidth { get; set; }

		private int _largeChange = 10;
		private int _smallChange = 1;
		private int _minimum;
		private int _maximum = 100;
		private int _value;

		private int _clickThumbY;
		private int _thumbTop;
		private bool _thumbDown;
		private bool _hover;
	}
}
