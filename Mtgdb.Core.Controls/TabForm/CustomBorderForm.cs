using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
			GotFocus += focusChanged;
			LostFocus += focusChanged;

			SizeBeforeMaximized = Size;

			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.ResizeRedraw,
				true);

			KeyPreview = true;

			KeyDown += keyDown;
			KeyUp += keyUp;

			CaptionHeight = 31.ByDpiHeight();
			Border = new Size(6, 6).ByDpi();
			ControlBoxButtonSize = new Size(31, 17).ByDpi();
		}

		protected override bool FixShadowTransparency => true;

		private void focusChanged(object sender, EventArgs e)
		{
			invalidateCaption();
			invalidateBorders();
		}

		private void layout(object sender, LayoutEventArgs e)
		{
			var controlBoxRect = getControlBoxRectangle();

			var rect = ClientRectangle;
			Rectangle client;
			int headerTop;

			if (IsMaximized)
			{
				client = Rectangle.FromLTRB(
					rect.Left,
					CaptionHeight - Border.Height,
					rect.Right,
					rect.Bottom);

				headerTop = rect.Top;
			}
			else
			{
				client = Rectangle.FromLTRB(
					rect.Left + Border.Width,
					CaptionHeight,
					rect.Right - Border.Width,
					rect.Bottom - Border.Height);

				headerTop = rect.Top + Border.Height;
			}

			_panelClient.Bounds = client;
			_panelHeader.Bounds = Rectangle.FromLTRB(client.Left, headerTop, controlBoxRect.Left, client.Top);
		}

		private void paint(object sender, PaintEventArgs e)
		{
			var title = getTitleRectangle();
			var controlBox = getControlBoxRectangle();

			paintTitle(e.Graphics, e.ClipRectangle, controlBox, title);
			paintBorders(e.Graphics, e.ClipRectangle, controlBox);
			drawBorderInterior(e.Graphics, title);
		}

		private void drawBorderInterior(Graphics g, Rectangle titleRect)
		{
			var pen = new Pen(BorderInteriorColor);

			int left = Border.Width - 1;
			int top = titleRect.Height - 1;
			int right = Width - Border.Width;
			int bottom = Height - Border.Height;

			if (IsMaximized)
			{
				left = 0;
				right = Width - 1;

				g.DrawLine(pen, left, top, right, top);
			}
			else
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
		}

		private void paintBorders(Graphics g, Rectangle clipRectangle, Rectangle controlBox)
		{
			foreach (Direction direction in getBorderDirections())
			{
				if (IsMaximized && (direction & Direction.Top) == 0)
					// there is always the top border, it doesn't always function as size changed though
					// therefore we always paint the top border
					continue;

				var borderElement = getBorderElement(direction);
				if (borderElement == null)
					continue;

				var areas = getBorders(direction);
				
				foreach (var border in areas)
				{
					if (!clipRectangle.IntersectsWith(border))
						continue;

					new VisualStyleRenderer(borderElement).DrawBackground(g, border);
				}
			}

			VisualStyleElement getBorderElement(Direction d)
			{
				if (Focused)
					switch (d)
					{
						case Direction.Left: return VisualStyleElement.Window.FrameLeft.Active;
						case Direction.Right: return VisualStyleElement.Window.FrameRight.Active;
						case Direction.Bottom: return VisualStyleElement.Window.FrameBottom.Active;
						case Direction.BottomLeft: return VisualStyleElement.Window.FrameLeftSizingTemplate.Normal;
						default: return null;
					}

				switch (d)
				{
					case Direction.Left: return VisualStyleElement.Window.FrameLeft.Inactive;
					case Direction.Right: return VisualStyleElement.Window.FrameRight.Inactive;
					case Direction.Bottom: return VisualStyleElement.Window.FrameBottom.Inactive;
					default: return null;
				}
			}
		}

		private void paintTitle(Graphics g, Rectangle clipRectangle, Rectangle controlBox, Rectangle titleRectangle)
		{
			if (clipRectangle.IntersectsWith(titleRectangle))
			{
				var element = getCaptionElement();
				var rect = titleRectangle;
				new VisualStyleRenderer(element).DrawBackground(g, rect, clipRectangle);
			}

			if (clipRectangle.IntersectsWith(controlBox))
			{
				var clientLocation = PointToClient(Cursor.Position);
				var images = getControlBoxImages(clientLocation);

				for (int i = images.Count - 1; i >= 0; i--)
				{
					var img = images[i];
					if (img.Bounds == default)
						continue;

					var element = getControlBoxElement(i, img);
					new VisualStyleRenderer(element).DrawBackground(g, img.Bounds);
				}

				VisualStyleElement getControlBoxElement(int i, (bool IsHovered, Rectangle Bounds) img)
				{
					if (img.IsHovered)
						switch (i)
						{
							case ControlBoxIndexClose: return VisualStyleElement.Window.CloseButton.Hot;
							case ControlBoxIndexRestore: return VisualStyleElement.Window.RestoreButton.Hot;
							case ControlBoxIndexMaximize: return VisualStyleElement.Window.MaxButton.Hot;
							case ControlBoxIndexMinimize: return VisualStyleElement.Window.MinButton.Hot;
							default: throw new ArgumentException();
						}
					
					switch (i)
					{
						case ControlBoxIndexClose: return VisualStyleElement.Window.CloseButton.Normal;
						case ControlBoxIndexRestore: return VisualStyleElement.Window.RestoreButton.Normal;
						case ControlBoxIndexMaximize: return VisualStyleElement.Window.MaxButton.Normal;
						case ControlBoxIndexMinimize: return VisualStyleElement.Window.MinButton.Normal;
						default: throw new ArgumentException();
					}
				}
			}

			VisualStyleElement getCaptionElement()
			{
				if (Focused)
					switch (IsMaximized)
					{
						case true: return VisualStyleElement.Window.MaxCaption.Active;
						default: return VisualStyleElement.Window.Caption.Active;
					}

				switch (IsMaximized)
				{
					case true: return VisualStyleElement.Window.MaxCaption.Inactive;
					default: return VisualStyleElement.Window.Caption.Inactive;
				}
			}
		}

		private IList<(bool IsHovered, Rectangle Bounds)> getControlBoxImages(Point clientLocation)
		{
			int x = Width;
			int y = Border.Height;

			var result = new List<(bool IsHovered, Rectangle Bounds)>();

			if (ShowCloseButton)
				add(clientLocation);
			else
				addEmpty();

			if (ShowMaximizeButton)
			{
				if (IsMaximized)
				{
					add(clientLocation);
					addEmpty();
				}
				else
				{
					addEmpty();
					add(clientLocation);
				}
			}
			else
			{
				addEmpty();
				addEmpty();
			}

			if (ShowMinimizeButton)
				add(clientLocation);
			else
				addEmpty();

			result.Reverse();
			return result;

			void add(Point location)
			{
				x -= ControlBoxButtonSize.Width + Border.Width;
				var rect = new Rectangle(x, y, ControlBoxButtonSize.Width, ControlBoxButtonSize.Height);
				result.Add((rect.Contains(location), rect));
			}

			void addEmpty() =>
				result.Add((default, default));
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
				invalidateControlBox();
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
				SnapTo(Direction.Left);
			else if (keyData == Keys.Right || keyData == Keys.NumPad6)
				SnapTo(Direction.Right);
			else if (keyData == Keys.Up)
			{
				if (!_isMaximizedBySystem)
					SnapTo(Direction.Top);
				else
					_isMaximizedBySystem = false;
			}
			else if (keyData == Keys.NumPad8)
				SnapTo(Direction.Top);
			else if (keyData == Keys.NumPad1)
				SnapTo(Direction.BottomLeft);
			else if (keyData == Keys.NumPad3)
				SnapTo(Direction.BottomRight);
			else if (keyData == Keys.NumPad9)
				SnapTo(Direction.TopRight);
			else if (keyData == Keys.NumPad7)
				SnapTo(Direction.TopLeft);
			else if (keyData == Keys.NumPad5)
				SnapTo(Direction.MiddleCenter);
			else if (keyData == Keys.Down || keyData == Keys.NumPad2)
			{
				if (IsMaximized)
					SnapTo(Direction.MiddleCenter);
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
				getResizeDirections().Any(IsSnappedTo);

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
				return Direction.Top;

			var snapCornerDistance = getSnappingCornerDistance(screenBounds);

			if (screenPosition.X - screenBounds.Left < getHorizontalSnapWidth())
			{
				var result = Direction.Left;

				if (screenPosition.Y - screenBounds.Top < snapCornerDistance)
					result |= Direction.Top;
				else if (screenBounds.Bottom - screenPosition.Y < snapCornerDistance)
					result |= Direction.Bottom;

				return result;
			}

			if (screenBounds.Right - screenPosition.X < getHorizontalSnapWidth())
			{
				var result = Direction.Right;

				if (screenPosition.Y - screenBounds.Top < snapCornerDistance)
					result |= Direction.Top;
				else if (screenBounds.Bottom - screenPosition.Y < snapCornerDistance)
					result |= Direction.Bottom;

				return result;
			}

			return Direction.MiddleCenter;
		}

		private Rectangle getSnappingRectangle(Point cursorPosition, Direction snapDirection)
		{
			var screenBounds = getScreenBounds(cursorPosition);

			if (snapDirection == Direction.Top)
				return screenBounds;


			if (snapDirection == Direction.Left)
				return new Rectangle(
					screenBounds.X,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height);

			if (snapDirection == Direction.TopLeft)
				return new Rectangle(
					screenBounds.X,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height / 2);

			if (snapDirection == Direction.BottomLeft)
				return new Rectangle(
					screenBounds.X,
					screenBounds.Bottom - screenBounds.Height / 2,
					screenBounds.Width / 2,
					screenBounds.Height / 2);


			if (snapDirection == Direction.Right)
				return new Rectangle(
					screenBounds.Right - screenBounds.Width / 2,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height);

			if (snapDirection == Direction.TopRight)
				return new Rectangle(
					screenBounds.Right - screenBounds.Width / 2,
					screenBounds.Y,
					screenBounds.Width / 2,
					screenBounds.Height / 2);

			if (snapDirection == Direction.BottomRight)
				return new Rectangle(
					screenBounds.Right - screenBounds.Width / 2,
					screenBounds.Bottom - screenBounds.Height / 2,
					screenBounds.Width / 2,
					screenBounds.Height / 2);


			if (snapDirection == Direction.MiddleCenter)
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

			if (snapDirection == Direction.Top)
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

			bool maximize = snapDirection == Direction.Top;

			if (maximize)
			{
				IsMaximized = true;
				if (WindowState != FormWindowState.Maximized)
					WindowState = FormWindowState.Maximized;
			}
			else
			{
				IsMaximized = false;

				if (WindowState != FormWindowState.Normal)
					WindowState = FormWindowState.Normal;

				setBounds(snapRectangle);
			}

			ResumeLayout(false);
			PerformLayout();

			invalidateCaption();

			invalidateBorders();
		}

		private void invalidateCaption() =>
			Invalidate(getTitleRectangle());

		private void invalidateControlBox() =>
			Invalidate(getControlBoxRectangle());

		private void invalidateBorders()
		{
			foreach (Direction direction in getBorderDirections())
				foreach (var rectangle in getBorders(direction))
					Invalidate(rectangle);
		}

		private static IEnumerable<Direction> getBorderDirections()
		{
			yield return Direction.Left;
			yield return Direction.Bottom;
			yield return Direction.Right;
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
			return (CaptionHeight - Border.Height) / 4;
		}

		private int getHorizontalSnapWidth()
		{
			return CaptionHeight - Border.Height;
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
				? new Rectangle(0, 0, Bounds.Width, CaptionHeight - Border.Height)
				: new Rectangle(0, 0, Bounds.Width, CaptionHeight);

		private Rectangle getControlBoxRectangle()
		{
			var images = getControlBoxImages(clientLocation: default);

			int left = images.Where(_ => _.Bounds != default).Select(_ => _.Bounds.Left)
				.DefaultIfEmpty(Right).Min();

			int top = images.Where(_ => _.Bounds != default).Select(_ => _.Bounds.Top)
				.DefaultIfEmpty(Top).Min();

			int right = images.Where(_ => _.Bounds != default).Select(_ => _.Bounds.Right)
				.DefaultIfEmpty(Right).Max();

			int bottom = images.Where(_ => _.Bounds != default).Select(_ => _.Bounds.Bottom)
				.DefaultIfEmpty(Top).Max();

			return Rectangle.FromLTRB(left, top, right, bottom);
		}

		private IEnumerable<Rectangle> getBorders(Direction direction)
		{
			var bounds = ClientRectangle;

			switch (direction)
			{
				case Direction.Left:
					yield return Rectangle.FromLTRB(0, CaptionHeight, Border.Width, bounds.Bottom - Border.Height);
					break;

				case Direction.Top:
					yield return Rectangle.FromLTRB(0, 0, bounds.Right, Border.Height);
					break;

				case Direction.Right:
					yield return Rectangle.FromLTRB(bounds.Right - Border.Width, CaptionHeight, bounds.Right, bounds.Bottom - Border.Height);
					break;

				case Direction.Bottom:
					yield return Rectangle.FromLTRB(bounds.Left, bounds.Bottom - Border.Height, bounds.Right, bounds.Bottom);
					break;

				case Direction.TopLeft:
					yield return Rectangle.FromLTRB(0, 0, CaptionHeight, Border.Height);
					yield return Rectangle.FromLTRB(0, 0, Border.Width, CaptionHeight);
					break;

				case Direction.TopRight:
					yield return Rectangle.FromLTRB(bounds.Right - CaptionHeight, 0, Bounds.Right, Border.Height);
					yield return Rectangle.FromLTRB(bounds.Right - Border.Width, 0, Bounds.Right, CaptionHeight);
					break;

				case Direction.BottomRight:
					yield return Rectangle.FromLTRB(bounds.Right - CaptionHeight, bounds.Bottom - Border.Height, Bounds.Right, bounds.Bottom);
					yield return Rectangle.FromLTRB(bounds.Right - Border.Width, bounds.Bottom - CaptionHeight, Bounds.Right, bounds.Bottom);
					break;

				case Direction.BottomLeft:
					yield return Rectangle.FromLTRB(0, bounds.Bottom - Border.Height, CaptionHeight, bounds.Bottom);
					yield return Rectangle.FromLTRB(0, bounds.Bottom - CaptionHeight, Border.Width, bounds.Bottom);
					break;
			}
		}



		private void move(Point screenPosition)
		{
			var direction = getSnappingDirection(screenPosition);

			if (direction == Direction.MiddleCenter)
				moveToDraggedLocation(screenPosition);
			else if (!IsSnappedTo(direction))
			{
				SnapTo(direction, screenPosition);
				if (direction == Direction.Top)
					_dragEnabledUnmaximizeThreshold = true;
			}
		}

		private void click(object sender, EventArgs e)
		{
			var screenLocation = Cursor.Position;

			var images = getControlBoxImages(PointToClient(screenLocation));

			if (images[ControlBoxIndexClose].IsHovered)
				Close();
			else if (images[ControlBoxIndexMaximize].IsHovered || images[ControlBoxIndexRestore].IsHovered)
				toggleMaximize();
			else if (images[ControlBoxIndexMinimize].IsHovered)
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
				SnapTo(Direction.MiddleCenter, screenLocation);
			else
				SnapTo(Direction.Top, screenLocation);
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
			invalidateCaption();
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

				var images = getControlBoxImages(clientLocation);

				if (!images.Any(_ => _.IsHovered))
					foreach (Direction direction in getResizeDirections())
						foreach (var border in getBorders(direction))
							if (border.Contains(clientLocation))
							{
								m.Result = (IntPtr) direction.ToWmNcHitTest();
								return;
							}
			}

			base.WndProc(ref m);
		}

		private static IEnumerable<Direction> getResizeDirections()
		{
			yield return Direction.TopLeft;
			yield return Direction.TopRight;
			yield return Direction.BottomRight;
			yield return Direction.BottomLeft;
			yield return Direction.Left;
			yield return Direction.Top;
			yield return Direction.Right;
			yield return Direction.Bottom;
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

		[Category("Settings"), DefaultValue(true)]
		public bool ShowMinimizeButton
		{
			get => _showMinimizeButton;
			set
			{
				_showMinimizeButton = value;
				Invalidate(true);
			}
		}

		[Category("Settings"), DefaultValue(true)]
		public bool ShowMaximizeButton
		{
			get => _showMaximizeButton;
			set
			{
				_showMaximizeButton = value;
				Invalidate(true);
			}
		}

		[Category("Settings")]
		public int CaptionHeight
		{
			get => _captionHeight;
			set
			{
				_captionHeight = value;
				OnLayout(new LayoutEventArgs(this, nameof(CaptionHeight)));
				Invalidate();
			}
		}

		private Size Border { get; }
		private Size ControlBoxButtonSize { get; }

		[Category("Settings"), DefaultValue(typeof(Color), "ActiveBorder")]
		public Color BorderInteriorColor
		{
			get => _panelHeader.BorderColor;
			[UsedImplicitly]
			set
			{
				_panelHeader.BorderColor = value;
				Invalidate();
			}
		}

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

		private Point? _dragFromLocation;
		private Size? _dragFromSize;
		private bool _isMaximized;
		private bool _isMaximizedBySystem;

		private bool _showCloseButton = true;
		private bool _showMinimizeButton = true;
		private bool _showMaximizeButton = true;
		private int _captionHeight;

		private readonly HashSet<Keys> _downKeys = new HashSet<Keys>();

		private static readonly Keys[] _winModifiers =
		{
			Keys.LWin,
			Keys.RWin
		};

		private const int ControlBoxIndexMinimize = 0;
		private const int ControlBoxIndexMaximize = 1;
		private const int ControlBoxIndexRestore = 2;
		private const int ControlBoxIndexClose = 3;

		private bool _dragEnabledUnmaximizeThreshold;
	}
}