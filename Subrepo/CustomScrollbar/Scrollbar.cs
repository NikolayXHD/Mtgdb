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
			MouseDown += mouseDown;
			MouseMove += mouseMove;
			MouseUp += mouseUp;

			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);

			ChannelColor = SystemColors.ScrollBar;
			BorderColor = SystemColors.Control;
			UpArrowImage = Resource.uparrow;
			DownArrowImage = Resource.downarrow;


			ThumbBottomImage = Resource.ThumbBottom;
			ThumbBottomSpanImage = Resource.ThumbSpanBottom;
			ThumbTopImage = Resource.ThumbTop;
			ThumbTopSpanImage = Resource.ThumbSpanTop;
			ThumbMiddleImage = Resource.ThumbMiddle;

			Width = UpArrowImage.Width + 2;
			MinimumSize = new Size(
				UpArrowImage.Width + 2,
				UpArrowImage.Height + DownArrowImage.Height + getThumbHeight());
		}

		private int getSpanHeight() =>
			(getThumbHeight() - (ThumbMiddleImage.Height + ThumbTopImage.Height + ThumbBottomImage.Height)) / 2;

		private int getValueRange() =>
			Maximum - Minimum - LargeChange;

		private int getVerticalRange() =>
			getTrackHeight() - getThumbHeight();

		private Rectangle getUpRect() =>
			new Rectangle(1, 0, Width - 2, UpArrowImage.Height);

		private Rectangle getDownRect() =>
			new Rectangle(1, Bottom - DownArrowImage.Height, Width - 2, DownArrowImage.Height);

		private Rectangle getThumbRect()
		{
			int top = _thumbTop + UpArrowImage.Height;
			return new Rectangle(1, top, Width - 2, getThumbHeight());
		}

		private int getThumbHeight()
		{
			int trackH = getTrackHeight();
			return Math.Min(Math.Max(ThumbMiddleImage.Height * 14 / 10, trackH * LargeChange / Maximum), trackH);
		}

		private int getTrackHeight() =>
			Height - (UpArrowImage.Height + DownArrowImage.Height);

		private int getValue() =>
			_thumbTop * (Maximum - LargeChange) / getVerticalRange();



		[Category("Behavior")]
		public int Value
		{
			get => _value;
			set
			{
				_value = value;

				int valueRange = getValueRange();

				float percent = 0.0f;
				if (valueRange != 0)
					percent = value / (float) valueRange;

				_thumbTop = (int) (percent * getVerticalRange());

				Invalidate();
			}
		}



		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

			e.Graphics.DrawRectangle(new Pen(BorderColor), Rectangle.FromLTRB(0, 0, Width - 1, Height - 1));
			e.Graphics.FillRectangle(new SolidBrush(ChannelColor), Rectangle.FromLTRB(1, 1, Width - 1, Height - 1));

			e.Graphics.DrawImage(UpArrowImage, getUpRect());
			e.Graphics.DrawImage(DownArrowImage, getDownRect());

			int width = Width - 2;
			int top = getThumbRect().Top;
			int left = 1;

			// draw top
			var rect = new Rectangle(left, top, width, ThumbTopImage.Height);
			e.Graphics.DrawImage(ThumbTopImage, rect);
			top += rect.Height;

			int spanHeight = getSpanHeight();
			rect = new Rectangle(left, top, width, spanHeight);
			e.Graphics.DrawImage(ThumbTopSpanImage, makeDoubleHeight(rect));
			top += rect.Height;

			//draw middle
			rect = new Rectangle(left, top, width, ThumbMiddleImage.Height);
			e.Graphics.DrawImage(ThumbMiddleImage, rect);
			top += rect.Height;

			rect = new Rectangle(left, top, width, spanHeight);
			e.Graphics.DrawImage(ThumbBottomSpanImage, makeDoubleHeight(rect));
			top += rect.Height;

			//draw bottom
			rect = new Rectangle(left, top, width, ThumbBottomImage.Height);
			e.Graphics.DrawImage(ThumbBottomImage, rect);

			// workaround the NearestNeighbor consequence that bottom half is left transparent
			Rectangle makeDoubleHeight(Rectangle src) =>
				new Rectangle(src.Location, new Size(src.Width, src.Height * 2));
		}

		private void mouseDown(object sender, MouseEventArgs e)
		{
			var cursor = PointToClient(Cursor.Position);
			if (!ClientRectangle.Contains(cursor))
				return;

			var thumbRect = getThumbRect();
			thumbRect.Inflate(1, 0);

			if (thumbRect.Contains(cursor))
			{
				_clickValue = cursor.Y - thumbRect.Top;
				_thumbDown = true;
				return;
			}

			int vertRange = getVerticalRange();
			if (vertRange <= 0)
				return;

			var upRect = getUpRect();
			upRect.Inflate(1, 0);

			var downRect = getDownRect();
			downRect.Inflate(1, 0);

			if (upRect.Contains(cursor))
				_thumbTop = Math.Max(0, _thumbTop - SmallChange);
			else if (downRect.Contains(cursor))
				_thumbTop = Math.Min(vertRange, _thumbTop + SmallChange);
			else if (cursor.Y <= thumbRect.Top)
				_thumbTop = Math.Max(0, _thumbTop - LargeChange);
			else if (cursor.Y > thumbRect.Bottom)
				_thumbTop = Math.Min(vertRange, _thumbTop + LargeChange);
			else
				return;

			_value = getValue();
			Refresh();
			ValueChanged?.Invoke(this, new EventArgs());
			Scroll?.Invoke(this, new EventArgs());
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			if (_thumbDown)
				_thumbDragging = true;

			if (!_thumbDragging)
				return;

			int valueInterval = Maximum - Minimum;
			int vertRange = getVerticalRange();

			if (_thumbDown && valueInterval > 0 && vertRange > 0)
			{
				_thumbTop = Math.Min(Math.Max(0, e.Y - (UpArrowImage.Height + _clickValue)), vertRange);

				_value = getValue();
				Refresh();
				ValueChanged?.Invoke(this, new EventArgs());
				Scroll?.Invoke(this, new EventArgs());
			}
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			_thumbDown = false;
			_thumbDragging = false;
		}



		public override bool AutoSize
		{
			get => base.AutoSize;
			set
			{
				base.AutoSize = value;
				if (value)
					Width = UpArrowImage.Width + 2;
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
		public Image UpArrowImage { get; set; }

		[Category("Skin")]
		public Image DownArrowImage { get; set; }

		[Category("Skin")]
		public Image ThumbTopImage { get; set; }

		[Category("Skin")]
		public Image ThumbTopSpanImage { get; set; }

		[Category("Skin")]
		public Image ThumbBottomImage { get; set; }

		[Category("Skin")]
		public Image ThumbBottomSpanImage { get; set; }

		[Category("Skin")]
		public Image ThumbMiddleImage { get; set; }

		private int _largeChange = 10;
		private int _smallChange = 1;
		private int _minimum;
		private int _maximum = 100;
		private int _value;

		private int _clickValue;
		private int _thumbTop;
		private bool _thumbDown;
		private bool _thumbDragging;
	}
}