using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class Popup
	{
		public Popup(
			Control menuControl,
			ButtonBase owner,
			HorizontalAlignment alignment = HorizontalAlignment.Left,
			bool closeMenuOnClick = true,
			bool borderOnHover = true,
			Control container = null,
			Action beforeShow = null)
		{
			MenuControl = menuControl;
			Container = container ?? menuControl;
			CloseMenuOnClick = closeMenuOnClick;
			BorderOnHover = borderOnHover;
			Owner = owner;
			_alignment = alignment;
			_beforeShow = beforeShow;
		}

		public Control MenuControl { get; }
		public Control Container { get; }

		public bool Shown { get; private set; }

		public void Show()
		{
			_beforeShow?.Invoke();

			var location = getLocation();

			if (MenuControl is ContextMenuStrip strip)
				show(strip, location);
			else
				show(location);
			
			Shown = true;
		}

		public void Hide()
		{
			MenuControl.Hide();
			Shown = false;
		}

		public bool IsCursorInPopup()
		{
			var cursorPosition = Cursor.Position;

			if (MenuControl is ContextMenuStrip)
			{
				var screenRect = new Rectangle(_screenLocation.Value, Container.Size);
				return screenRect.Contains(cursorPosition);
			}
			else
			{
				var clientPosition = MenuControl.PointToClient(cursorPosition);
				return MenuControl.ClientRectangle.Contains(clientPosition);
			}
		}

		public bool IsCursorInButton()
		{
			var cursorPosition = Cursor.Position;
			bool isCursorInButton = Owner.ClientRectangle.Contains(Owner.PointToClient(cursorPosition));
			return isCursorInButton;
		}



		private void show(Point location)
		{
			var parent = MenuControl.Parent;
			location = parent.PointToClient(Owner, location);

			location = new Point(
				location.X.WithinRange(MenuControl.Margin.Left, parent.Width - MenuControl.Width - MenuControl.Margin.Right),
				location.Y.WithinRange(MenuControl.Margin.Top, parent.Height - MenuControl.Height - MenuControl.Margin.Bottom));

			MenuControl.Location = location;
			MenuControl.BringToFront();
			MenuControl.Show();
			MenuControl.Focus();
		}

		private void show(ContextMenuStrip contextMenuStrip, Point location)
		{
			ControlHelpers.SetForegroundWindow(
				new HandleRef(
					MenuControl,
					MenuControl.Handle));

			_screenLocation = Owner.PointToScreen(location);
			contextMenuStrip.Show(_screenLocation.Value);
		}

		private Point getLocation()
		{
			int top = Owner.Height + MenuControl.Margin.Top;

			switch (_alignment)
			{
				case HorizontalAlignment.Left:
					return new Point(0, top);
				case HorizontalAlignment.Right:
					return new Point(Owner.Width - MenuControl.Width, top);
				case HorizontalAlignment.Center:
					return new Point((Owner.Width - MenuControl.Width) / 2, top);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}



		public bool CloseMenuOnClick { get; }
		public bool BorderOnHover { get; }
		public ButtonBase Owner { get; }
		private readonly HorizontalAlignment _alignment;
		private readonly Action _beforeShow;
		private Point? _screenLocation;
	}
}