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

			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.Selectable, false);

			ChannelColor = SystemColors.ScrollBar;
			BorderColor = SystemColors.Control;
			UpArrowImage = Resource.uparrow;
			DownArrowImage = Resource.downarrow;


			ThumbBottomImage = Resource.ThumbBottom;
			ThumbBottomSpanImage = Resource.ThumbSpanBottom;
			ThumbTopImage = Resource.ThumbTop;
			ThumbTopSpanImage = Resource.ThumbSpanTop;
			ThumbMiddleImage = Resource.ThumbMiddle;

			Width = UpArrowImage.Width;
		}



		private int getVerticalRange() => getTrackHeight() - getThumbHeight();

		private int getTrackHeight() => Height - getUpRect().Height - getDownRect().Height;
		private int getThumbHeight()
		{
			int trackHeight = getTrackHeight();

			if (Maximum == Minimum)
				return trackHeight;

			return Math.Min(Math.Max(
				ThumbMiddleImage.Height + ThumbTopSpanImage.Height + ThumbTopImage.Height + ThumbBottomSpanImage.Height + ThumbBottomImage.Height,
				trackHeight * LargeChange / (LargeChange + Maximum - Minimum)),
				trackHeight);
		}

		private Rectangle getThumbRect() => new Rectangle(0, getThumbRectTop(), Width, getThumbHeight());
		private int getThumbRectTop() => ThumbTop + getUpRect().Height;

		private Rectangle getUpRect()
		{
			if (Height >= UpArrowImage.Height + DownArrowImage.Height)
				return new Rectangle(0, 0, Width, UpArrowImage.Height);

			return new Rectangle(0, 0, Width, Height * UpArrowImage.Height / (UpArrowImage.Height + DownArrowImage.Height));
		}

		private Rectangle getDownRect()
		{
			if (Height >= UpArrowImage.Height + DownArrowImage.Height)
				return new Rectangle(0, Bottom - DownArrowImage.Height, Width, DownArrowImage.Height);

			var h = Height - Height * UpArrowImage.Height / (UpArrowImage.Height + DownArrowImage.Height);
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
				ValueChanged?.Invoke(this, new EventArgs());
				Scroll?.Invoke(this, new EventArgs());
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

				if (Value == prevValue)
					Refresh();
			}
		}


		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

			e.Graphics.DrawRectangle(new Pen(BorderColor), Rectangle.FromLTRB(0, 0, Width - 1, Height - 1));
			e.Graphics.FillRectangle(new SolidBrush(ChannelColor), Rectangle.FromLTRB(1, 1, Width - 1, Height - 1));

			e.Graphics.DrawImage(UpArrowImage, getUpRect());
			e.Graphics.DrawImage(DownArrowImage, getDownRect());

			int top = getThumbRectTop();

			int thumbHeight = getThumbHeight();
			if (thumbHeight <= 0)
				return;

			int fullSpan = Math.Max(0, thumbHeight - ThumbMiddleImage.Height - ThumbTopImage.Height - ThumbBottomImage.Height);
			int topSpan = fullSpan / 2;
			int bottomSpan = fullSpan - topSpan;
			int middleHeight = thumbHeight - ThumbBottomImage.Height - ThumbTopImage.Height - fullSpan;

			// draw top
			var rect = new Rectangle(0, top, Width, ThumbTopImage.Height);
			e.Graphics.DrawImage(ThumbTopImage, rect);
			top += rect.Height;

			if (topSpan > 0)
			{
				rect = new Rectangle(0, top, Width, topSpan);
				e.Graphics.DrawImage(ThumbTopSpanImage, makeDoubleHeight(rect));
				top += rect.Height;
			}

			if (middleHeight > 0)
			{
				//draw middle
				rect = new Rectangle(0, top, Width, Math.Min(ThumbMiddleImage.Height, middleHeight));
				e.Graphics.DrawImage(ThumbMiddleImage, rect);
				top += rect.Height;
			}

			if (bottomSpan > 0)
			{
				rect = new Rectangle(0, top, Width, bottomSpan);
				e.Graphics.DrawImage(ThumbBottomSpanImage, makeDoubleHeight(rect));
				top += rect.Height;
			}

			//draw bottom
			rect = new Rectangle(0, top, Width, ThumbBottomImage.Height);
			e.Graphics.DrawImage(ThumbBottomImage, rect);

			// workaround the NearestNeighbor consequence that bottom half is left transparent
			Rectangle makeDoubleHeight(Rectangle src) =>
				new Rectangle(src.Location, new Size(src.Width, src.Height * 2));
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			// base.OnMouseDown is not called intentionally
			// to prevent stealing focus from container

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

		private void mouseUp(object sender, MouseEventArgs e) =>
			_thumbDown = false;

		public override bool AutoSize
		{
			get => base.AutoSize;
			set
			{
				base.AutoSize = value;
				if (value)
					Width = UpArrowImage.Width;
			}
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

		[Category("Skin")]
		public Color ChannelColor { get; set; }

		[Category("Skin")]
		public Color BorderColor { get; set; }

		[Category("Skin")]
		public Bitmap UpArrowImage { get; set; }

		[Category("Skin")]
		public Bitmap DownArrowImage { get; set; }

		[Category("Skin")]
		public Bitmap ThumbTopImage { get; set; }

		[Category("Skin")]
		public Bitmap ThumbTopSpanImage { get; set; }

		[Category("Skin")]
		public Bitmap ThumbBottomImage { get; set; }

		[Category("Skin")]
		public Bitmap ThumbBottomSpanImage { get; set; }

		[Category("Skin")]
		public Bitmap ThumbMiddleImage { get; set; }

		private int _largeChange = 10;
		private int _smallChange = 1;
		private int _minimum;
		private int _maximum = 100;
		private int _value;

		private int _clickThumbY;
		private int _thumbTop;
		private bool _thumbDown;
	}
}