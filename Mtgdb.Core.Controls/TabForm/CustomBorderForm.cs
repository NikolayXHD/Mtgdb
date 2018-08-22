using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public partial class CustomBorderForm : ShadowedForm
	{
		public event Action<object, MouseEventArgs, CancelEventArgs> QueryHandleDrag;

		public CustomBorderForm()
		{
			InitializeComponent();

			Paint += paint;
			Click += click;

			RegisterDragControl(this);
			RegisterDragControl(_panelHeader);

			Resize += resize;
			Layout += layout;

			SizeBeforeMaximized = Size;
			_panelHeader.BackColor = _titleBackgroundColor;

			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.ResizeRedraw,
				true);

			KeyPreview = true;

			KeyDown += keyDown;
			KeyUp += keyUp;
		}

		private void layout(object sender, LayoutEventArgs e)
		{
			var controlBoxRect = getControlBoxRectangle();
			Rectangle clientBounds;
			Rectangle headerBounds;

			if (IsMaximized)
			{
				headerBounds = new Rectangle(0, 0, Width - controlBoxRect.Width, TitleHeight - Border);
				clientBounds = new Rectangle(0, TitleHeight - Border, Width, Height - (TitleHeight - Border));
			}
			else
			{
				headerBounds = new Rectangle(Border, Border, Width - controlBoxRect.Width - Border, TitleHeight - Border);
				clientBounds = new Rectangle(Border, TitleHeight, Width - 2 * Border, Height - TitleHeight - Border);
			}

			_panelClient.Bounds = clientBounds;
			_panelHeader.Bounds = headerBounds;
		}

		private void paint(object sender, PaintEventArgs e)
		{
			var title = getTitleRectangle();
			var controlBox = getControlBoxRectangle();

			paintBorders(e.Graphics, e.ClipRectangle, controlBox);
			paintTitle(e.Graphics, e.ClipRectangle, controlBox, title);
			drawBorderInterior(e.Graphics, title);
		}

		private void drawBorderInterior(Graphics g, Rectangle titleRect)
		{
			if (BorderInteriorColor != TitleBackgroundColor)
			{
				var pen = new Pen(BorderInteriorColor);

				int left = Border - 1;
				int top = titleRect.Height - 1;
				int right = Width - Border;
				int bottom = Height - Border;

				if (!IsMaximized)
				{
					var points = new[]
					{
						new Point(left, top),
						new Point(left, bottom),
						new Point(right, bottom),
						new Point(right, top)
					};

					g.DrawLine(pen, points[0], points[1]);
					g.DrawLine(pen, points[1], points[2]);
					g.DrawLine(pen, points[2], points[3]);
					g.DrawLine(pen, points[3], points[0]);
				}
				else
				{
					left = 0;
					right = Width - 1;

					g.DrawLine(pen, left, top, right, top);
				}
			}
		}

		private void paintBorders(Graphics g, Rectangle clipRectangle, Rectangle controlBox)
		{
			var directions = Enum.GetValues(typeof(Direction));
			foreach (Direction direction in directions)
			{
				if (IsMaximized && (direction & Direction.North) == 0)
					// there is always the top border, it doesn't always function as size changed though
					// therefore we always paint the top border
					continue;

				var areas = getBorders(direction);

				foreach (var border in areas)
				{
					if (IsMaximized && border.Bottom > Border)
						// vertical parts of top borders
						continue;

					if (clipRectangle.IntersectsWith(border))
					{
						var current = border;

						if (controlBox.Contains(current))
							continue;

						if (controlBox.IntersectsWith(current))
						{
							if (current.Right > controlBox.X && current.Left < controlBox.X)
								current = new Rectangle(
									current.Location,
									new Size(controlBox.X - current.X, current.Height));

							if (current.Top < controlBox.Bottom - 1 && current.Bottom > controlBox.Bottom)
								current = new Rectangle(
									new Point(current.X, controlBox.Bottom),
									new Size(current.Width, current.Bottom - controlBox.Bottom));
						}

						g.FillRectangle(new SolidBrush(TitleBackgroundColor), current);
					}
				}
			}
		}

		private void paintTitle(Graphics g, Rectangle clipRectangle, Rectangle controlBox, Rectangle titleRectangle)
		{
			titleRectangle = new Rectangle(
				_panelHeader.Width,
				controlBox.Top,
				titleRectangle.Right - _panelHeader.Width,
				titleRectangle.Height - controlBox.Top);

			if (clipRectangle.IntersectsWith(titleRectangle))
				g.FillRectangle(new SolidBrush(TitleBackgroundColor), titleRectangle);

			if (clipRectangle.IntersectsWith(controlBox))
			{
				int x = Bounds.Width;

				var clientLocation = PointToClient(Cursor.Position);
				var images = getControlBoxImages(clientLocation, out _, out _, out _);
				for (int i = images.Count - 1; i >= 0; i--)
				{
					x -= images[i].Width;

					Point location = new Point(x, 0);
					g.DrawImage(images[i], new Rectangle(location, images[i].Size));
				}
			}
		}

		private IList<Bitmap> getControlBoxImages(Point clientLocation, out bool hoveredClose, out bool hoveredMaximize, out bool hoveredMinimize)
		{
			hoveredClose = hoveredMaximize = hoveredMinimize = false;

			int x = Width;
			var result = new List<Bitmap>();

			if (ShowCloseButton)
				hoveredClose = addControlBoxImage(_imageClose, ref x, clientLocation, result);

			if (ShowMaximizeButton)
			{
				if (IsMaximized)
					hoveredMaximize = addControlBoxImage(_imageNormalize, ref x, clientLocation, result);
				else
					hoveredMaximize = addControlBoxImage(_imageMaximize, ref x, clientLocation, result);
			}

			if (ShowMinimizeButton)
				hoveredMinimize = addControlBoxImage(_imageMinimize, ref x, clientLocation, result);

			result.Reverse();
			return result;
		}

		private static bool addControlBoxImage(Bitmap[] images, ref int x, Point location, List<Bitmap> result)
		{
			if (images[0] == null)
				return false;

			var rect = new Rectangle(x - images[0].Width, 0, images[0].Width, images[0].Height);
			bool hovered = rect.Contains(location);

			if (hovered)
				result.Add(images[1]);
			else
				result.Add(images[0]);

			x -= images[0].Width;
			return hovered;
		}



		private void mouseDown(object sender, MouseEventArgs e)
		{
			var cancelArgs = new CancelEventArgs();
			QueryHandleDrag?.Invoke(sender, e, cancelArgs);

			if (cancelArgs.Cancel)
				return;

			if (e.Button != MouseButtons.Left)
				return;

			var clientPosition = PointToClient(Cursor.Position);

			if (getTitleRectangle().Contains(clientPosition) && !getControlBoxRectangle().Contains(clientPosition))
				titleBeginDrag(clientPosition);
		}

		private void mouseMove(object sender, MouseEventArgs e)
		{
			var screenPosition = Cursor.Position;

			if (titleIsDragging())
			{
				if (_dragEnabledUnmaximizeThreshold)
				{
					var clientPosition = PointToClient(screenPosition);
					if (clientPosition.Y - _dragFromLocation.Value.Y > getVerticalSnapHeight())
					{
						_dragEnabledUnmaximizeThreshold = false;
						move(screenPosition);
					}
				}
				else
					move(screenPosition);
			}
			else
				Invalidate(getControlBoxRectangle());
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			Invalidate(getControlBoxRectangle());
		}

		private void mouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			if (titleIsDragging())
				titleEndDrag();
		}

		private void keyDown(object sender, KeyEventArgs e)
		{
			_downKeys.Add(e.KeyData);
		}

		private void keyUp(object sender, KeyEventArgs e)
		{
			bool winModifier = _winModifiers.Any(_downKeys.Contains);
			_downKeys.Remove(e.KeyData);

			if (!winModifier)
				return;

			Keys keyData = e.KeyData;

			if (keyData == Keys.Left || keyData == Keys.NumPad4)
				SnapTo(Direction.West);
			else if (keyData == Keys.Right || keyData == Keys.NumPad6)
				SnapTo(Direction.East);
			else if (keyData == Keys.Up)
			{
				if (!_isMaximizedBySystem)
					SnapTo(Direction.North);
				else
					_isMaximizedBySystem = false;
			}
			else if (keyData == Keys.NumPad8)
				SnapTo(Direction.North);
			else if (keyData == Keys.NumPad1)
				SnapTo(Direction.SouthWest);
			else if (keyData == Keys.NumPad3)
				SnapTo(Direction.SouthEast);
			else if (keyData == Keys.NumPad9)
				SnapTo(Direction.NorthEast);
			else if (keyData == Keys.NumPad7)
				SnapTo(Direction.NorthWest);
			else if (keyData == Keys.NumPad5)
				SnapTo(Direction.None);
			else if (keyData == Keys.Down || keyData == Keys.NumPad2)
			{
				if (IsMaximized)
					SnapTo(Direction.None);
				else
					minimize();
			}
		}



		private void resize(object sender, EventArgs e)
		{
			_isMaximizedBySystem = !IsMaximized && WindowState == FormWindowState.Maximized;
			IsMaximized = WindowState == FormWindowState.Maximized;

			bool isForcedSize =
				WindowState == FormWindowState.Minimized ||
				WindowState == FormWindowState.Maximized ||
				Enum.GetValues(typeof(Direction))
					.Cast<Direction>()
					.Where(_ => _ != Direction.None)
					.Any(IsSnappedTo);

			if (!isForcedSize)
				SizeBeforeMaximized = Size;
		}

		private void titleBeginDrag(Point clientPosition)
		{
			_dragFromLocation = clientPosition;
			_dragFromSize = Size;

			_dragEnabledUnmaximizeThreshold = IsMaximized;
		}

		private bool titleIsDragging()
		{
			return _dragFromLocation.HasValue;
		}

		private void titleEndDrag()
		{
			_dragFromLocation = null;
			_dragFromSize = null;
		}



		private Direction getSnappingDirection(Point screenPosition)
		{
			var screenBounds = getScreenBounds(screenPosition);
			if (screenPosition.Y - screenBounds.Top <= 0)
				return Direction.North;

			var snapCornerDistance = getSnappingCornerDistance(screenBounds);

			if (screenPosition.X - screenBounds.Left < getHorizontalSnapWidth())
			{
				var result = Direction.West;

				if (screenPosition.Y - screenBounds.Top < snapCornerDistance)
					result |= Direction.North;
				else if (screenBounds.Bottom - screenPosition.Y < snapCornerDistance)
					result |= Direction.South;

				return result;
			}

			if (screenBounds.Right - screenPosition.X < getHorizontalSnapWidth())
			{
				var result = Direction.East;

				if (screenPosition.Y - screenBounds.Top < snapCornerDistance)
					result |= Direction.North;
				else if (screenBounds.Bottom - screenPosition.Y < snapCornerDistance)
					result |= Direction.South;

				return result;
			}

			return Direction.None;
		}

		private Rectangle getSnappingRectangle(Point cursorPosition, Direction snapDirection)
		{
			var screenBounds = getScreenBounds(cursorPosition);

			if (snapDirection == Direction.North)
				return screenBounds;


			if (snapDirection == Direction.West)
				return new Rectangle(
					screenBounds.X,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height);

			if (snapDirection == Direction.NorthWest)
				return new Rectangle(
					screenBounds.X,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height / 2);

			if (snapDirection == Direction.SouthWest)
				return new Rectangle(
					screenBounds.X,
					screenBounds.Bottom - screenBounds.Height / 2,
					screenBounds.Width / 2,
					screenBounds.Height / 2);


			if (snapDirection == Direction.East)
				return new Rectangle(
					screenBounds.Right - screenBounds.Width / 2,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height);

			if (snapDirection == Direction.NorthEast)
				return new Rectangle(
					screenBounds.Right - screenBounds.Width / 2,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height / 2);

			if (snapDirection == Direction.SouthEast)
				return new Rectangle(
					screenBounds.Right - screenBounds.Width / 2,
					screenBounds.Bottom - screenBounds.Height / 2,
					screenBounds.Width / 2,
					screenBounds.Height / 2);


			if (snapDirection == Direction.None)
				return new Rectangle(
					screenBounds.Left + (screenBounds.Width - SizeBeforeMaximized.Width) / 2,
					screenBounds.Top + (screenBounds.Height - SizeBeforeMaximized.Height) / 2,
					SizeBeforeMaximized.Width,
					SizeBeforeMaximized.Height);

			return Rectangle.Empty;
		}

		protected bool IsSnappedTo(Direction snapDirection)
		{
			var rect = getSnappingRectangle(PointToScreen(new Point(0, 0)), snapDirection);

			if (Bounds != rect)
				return false;

			if (snapDirection == Direction.North)
				return WindowState == FormWindowState.Maximized;

			return WindowState == FormWindowState.Normal;
		}

		protected void SnapTo(Direction snapDirection, Point? cursorPosition = null)
		{
			Rectangle snapRectangle;

			IList<Screen> screens = Screen.AllScreens;

			if (IsSnappedTo(snapDirection) && screens.Count > 1)
			{
				var screen = Screen.FromPoint(PointToScreen(new Point(0, 0)));
				var nextScreen = screens[(screens.IndexOf(screen) + 1) % screens.Count];
				snapRectangle = getSnappingRectangle(nextScreen.Bounds.Location, snapDirection);
			}
			else
			{
				snapRectangle = getSnappingRectangle(cursorPosition ?? PointToScreen(new Point(0, 0)), snapDirection);
			}

			if (snapRectangle == Rectangle.Empty)
				return;

			SuspendLayout();

			if (WindowState != FormWindowState.Normal)
				WindowState = FormWindowState.Normal;

			setBounds(snapRectangle);

			IsMaximized = snapDirection == Direction.North;
			if (IsMaximized)
				WindowState = FormWindowState.Maximized;

			ResumeLayout(false);
			PerformLayout();

			Invalidate(getTitleRectangle());

			var directions = Enum.GetValues(typeof(Direction));
			foreach (Direction direction in directions)
				foreach (var rectangle in getBorders(direction))
					Invalidate(rectangle);
		}

		private void moveToDraggedLocation(Point screenPosition)
		{
			if (WindowState != FormWindowState.Normal)
				WindowState = FormWindowState.Normal;

			var draggedLocation = getDraggedFormLocation(screenPosition);

			var targetBounds = new Rectangle(
				draggedLocation.X,
				draggedLocation.Y,
				SizeBeforeMaximized.Width,
				SizeBeforeMaximized.Height);

			setBounds(targetBounds);
			IsMaximized = false;
		}

		private Point getDraggedFormLocation(Point screenPosition)
		{
			var desiredLocation = new Point(
				_dragFromLocation.Value.X * Width / Math.Max(1, _dragFromSize.Value.Width),
				_dragFromLocation.Value.Y);

			var controlBox = getControlBoxRectangle();
			var title = getTitleRectangle();

			if (!title.Contains(desiredLocation))
				desiredLocation = desiredLocation.ProjectTo(title);

			if (controlBox.Contains(desiredLocation))
				desiredLocation = new Point(controlBox.Left - 1, desiredLocation.Y);

			var clientPosition = PointToClient(screenPosition);

			var location = new Point(
				Location.X + clientPosition.X - desiredLocation.X,
				Location.Y + clientPosition.Y - desiredLocation.Y);

			return location;
		}



		private int getVerticalSnapHeight()
		{
			return (TitleHeight - Border) / 4;
		}

		private int getHorizontalSnapWidth()
		{
			return TitleHeight - Border;
		}

		private static int getSnappingCornerDistance(Rectangle screenBounds)
		{
			return screenBounds.Height / 10;
		}

		private static Rectangle getScreenBounds(Point cursorPosition)
		{
			var screen = Screen.FromPoint(cursorPosition);
			var screenBounds = screen.WorkingArea;
			return screenBounds;
		}



		private Rectangle getTitleRectangle() =>
			IsMaximized
				? new Rectangle(0, 0, Bounds.Width, TitleHeight - Border)
				: new Rectangle(0, 0, Bounds.Width, TitleHeight);

		private Rectangle getControlBoxRectangle()
		{
			var images = getControlBoxImages(clientLocation: default, out _, out _, out _);

			var width = images.Count > 0
				? images.Sum(_ => _.Width)
				: 0;

			int height = images.Count > 0
				? images.Max(_ => _.Height)
				: 0;

			return new Rectangle(Bounds.Width - width, 0, width, height);
		}

		private IEnumerable<Rectangle> getBorders(Direction direction)
		{
			var bounds = Bounds;

			switch (direction)
			{
				case Direction.None:
					yield break;
				case Direction.North:
					yield return new Rectangle(Corner, 0, bounds.Width - 2 * Corner, Border);

					break;
				case Direction.NorthEast:
					yield return new Rectangle(bounds.Width - Corner, 0, Corner, Border);
					yield return new Rectangle(bounds.Width - Border, 0, Border, Corner);

					break;
				case Direction.East:
					yield return new Rectangle(bounds.Width - Border, Corner, Border, bounds.Height - 2 * Corner);

					break;
				case Direction.SouthEast:
					yield return new Rectangle(bounds.Width - Border, bounds.Height - Corner, Border, Corner);
					yield return new Rectangle(bounds.Width - Corner, bounds.Height - Border, Corner, Border);

					break;
				case Direction.South:
					yield return new Rectangle(Corner, bounds.Height - Border, bounds.Width - 2 * Corner, Border);

					break;
				case Direction.SouthWest:
					yield return new Rectangle(0, bounds.Height - Border, Corner, Border);
					yield return new Rectangle(0, bounds.Height - Corner, Border, Corner);

					break;
				case Direction.West:
					yield return new Rectangle(0, Corner, Border, bounds.Height - 2 * Corner);

					break;
				case Direction.NorthWest:
					yield return new Rectangle(0, 0, Border, Corner);
					yield return new Rectangle(0, 0, Corner, Border);

					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}



		private void move(Point screenPosition)
		{
			var direction = getSnappingDirection(screenPosition);

			if (direction == Direction.None)
				moveToDraggedLocation(screenPosition);
			else if (!IsSnappedTo(direction))
			{
				SnapTo(direction, screenPosition);
				if (direction == Direction.North)
					_dragEnabledUnmaximizeThreshold = true;
			}
		}

		private void click(object sender, EventArgs e)
		{
			var screenLocation = Cursor.Position;

			getControlBoxImages(PointToClient(screenLocation),
				out bool hoveredClose,
				out bool hoveredMaximize,
				out bool hoveredMinimize);

			if (hoveredClose)
				Close();
			else if (hoveredMaximize)
				toggleMaximize();
			else if (hoveredMinimize)
				minimize();
		}

		private void minimize()
		{
			WindowState = FormWindowState.Minimized;
		}

		private void doubleClick(object sender, MouseEventArgs e)
		{
			toggleMaximize();
		}

		private void toggleMaximize()
		{
			var screenLocation = Cursor.Position;

			if (IsMaximized)
				SnapTo(Direction.None, screenLocation);
			else
				SnapTo(Direction.North, screenLocation);
		}

		private void setBounds(Rectangle rect)
		{
			var controlBox = getControlBoxRectangle();
			if (rect.Width < controlBox.Width)
				rect = new Rectangle(rect.Location, new Size(controlBox.Width, rect.Height));

			var title = getTitleRectangle();
			if (rect.Height < title.Height)
				rect = new Rectangle(rect.Location, new Size(rect.Width, title.Height));

			Bounds = rect;

			var screen = Screen.FromControl(this);
			var workingArea = screen.WorkingArea;
			var screenArea = screen.Bounds;

			workingArea.Offset(-screenArea.X, -screenArea.Y);

			MaximizedBounds = workingArea;
			Invalidate(getTitleRectangle());
		}



		protected void RegisterDragControl(Control c)
		{
			c.MouseDown += mouseDown;
			c.MouseUp += mouseUp;
			c.MouseMove += mouseMove;
			c.MouseDoubleClick += doubleClick;
			c.MouseLeave += mouseLeave;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x84 && !IsMaximized)
			{
				var screenLocation = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
				var clientLocation = PointToClient(screenLocation);

				getControlBoxImages(clientLocation,
					out bool hoveredClose,
					out bool hoveredMaximize,
					out bool hoveredMinimize);

				if (!hoveredMinimize && !hoveredMaximize && !hoveredClose)
					foreach (Direction direction in Enum.GetValues(typeof(Direction)))
						foreach (var border in getBorders(direction))
							if (border.Contains(clientLocation))
							{
								m.Result = (IntPtr) direction.ToWmNcHitTest();
								return;
							}
			}

			base.WndProc(ref m);
		}



		[Category("Settings"), DefaultValue(null)]
		public Bitmap ImageClose
		{
			get => _imageClose[0];
			set
			{
				if (_imageClose[0] == value)
					return;

				_imageClose[0] = value;
				_imageClose[1] = value.TransformColors(ControlBoxHoverSaturation, ControlBoxHoverBrightness);
			}
		}

		[Category("Settings"), DefaultValue(true)]
		public bool ShowCloseButton
		{
			get => _showCloseButton;
			set
			{
				_showCloseButton = value;
				Invalidate(true);
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public Bitmap ImageMinimize
		{
			get => _imageMinimize[0];
			set
			{
				if (_imageMinimize[0] == value)
					return;

				_imageMinimize[0] = value;
				_imageMinimize[1] = value.TransformColors(ControlBoxHoverSaturation, ControlBoxHoverBrightness);
			}
		}

		[Category("Settings"), DefaultValue(true)]
		public bool ShowMinimizeButton
		{
			get => _showMinimizeButton;
			set
			{
				_showMinimizeButton = value;
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public Bitmap ImageMaximize
		{
			get => _imageMaximize[0];
			set
			{
				if (_imageMaximize[0] == value)
					return;

				_imageMaximize[0] = value;
				_imageMaximize[1] = value.TransformColors(ControlBoxHoverSaturation, ControlBoxHoverBrightness);
			}
		}

		[Category("Settings"), DefaultValue(true)]
		public bool ShowMaximizeButton
		{
			get => _showMaximizeButton;
			set
			{
				_showMaximizeButton = value;
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(null)]
		public Bitmap ImageNormalize
		{
			get => _imageNormalize[0];
			set
			{
				if (_imageNormalize[0] == value)
					return;

				_imageNormalize[0] = value;
				_imageNormalize[1] = value.TransformColors(ControlBoxHoverSaturation, ControlBoxHoverBrightness);
			}
		}

		[Category("Settings"), DefaultValue(24)]
		public int TitleHeight
		{
			get => _titleHeight;
			set
			{
				_titleHeight = value;
				OnLayout(new LayoutEventArgs(this, nameof(TitleHeight)));
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(4)]
		public int Border
		{
			get => _border;
			set
			{
				_border = value;
				OnLayout(new LayoutEventArgs(this, nameof(Border)));
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(24)]
		public int Corner { get; set; } = 24;

		[Category("Settings"), DefaultValue(typeof(Color), "210, 210, 220")]
		public Color TitleBackgroundColor
		{
			get => _titleBackgroundColor;
			[UsedImplicitly]
			set
			{
				_titleBackgroundColor = value;
				_panelHeader.BackColor = value;
				Invalidate();
			}
		}

		[Category("Settings"), DefaultValue(typeof(Color), "166, 166, 166")]
		public Color BorderInteriorColor { get; set; } = Color.FromArgb(166, 166, 166);

		[Category("Settings"), DefaultValue(1.25f)]
		public float ControlBoxHoverSaturation { get; set; } = 1.25f;

		[Category("Settings"), DefaultValue(1.05f)]
		public float ControlBoxHoverBrightness { get; set; } = 1.05f;

		[Browsable(false)]
		private bool IsMaximized
		{
			get => _isMaximized;
			set
			{
				if (_isMaximized != value)
				{
					_isMaximized = value;
					OnLayout(new LayoutEventArgs(this, nameof(IsMaximized)));
					Invalidate(getControlBoxRectangle());
				}
			}
		}

		private Size SizeBeforeMaximized { get; set; }

		private readonly Bitmap[] _imageMinimize = new Bitmap[2];
		private readonly Bitmap[] _imageClose = new Bitmap[2];
		private readonly Bitmap[] _imageMaximize = new Bitmap[2];
		private readonly Bitmap[] _imageNormalize = new Bitmap[2];

		private Point? _dragFromLocation;
		private Size? _dragFromSize;
		private bool _isMaximized;
		private bool _isMaximizedBySystem;

		private bool _showCloseButton = true;
		private bool _showMinimizeButton = true;
		private bool _showMaximizeButton = true;
		private int _titleHeight = 24;
		private int _border = 4;
		private Color _titleBackgroundColor = Color.FromArgb(210, 210, 220);

		private readonly HashSet<Keys> _downKeys = new HashSet<Keys>();

		private static readonly Keys[] _winModifiers =
		{
			Keys.LWin,
			Keys.RWin
		};

		private bool _dragEnabledUnmaximizeThreshold;
	}
}