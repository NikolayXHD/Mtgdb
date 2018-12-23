using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class Popup
	{
		public Popup(
			Control control,
			ButtonBase owner,
			HorizontalAlignment alignment = HorizontalAlignment.Left,
			bool openOnHover = true,
			bool closeMenuOnClick = false,
			bool borderOnHover = true,
			Control container = null,
			Action beforeShow = null)
		{
			Control = control;
			Container = container ?? control;
			OpenOnHover = openOnHover;
			CloseMenuOnClick = closeMenuOnClick;
			BorderOnHover = borderOnHover;
			Owner = owner;
			_alignment = alignment;
			_beforeShow = beforeShow;
		}

		public Control Control { get; }
		public Control Container { get; }

		public bool Shown { get; private set; }

		public void Show()
		{
			var location = getLocation();

			if (Control is ContextMenuStrip strip)
				show(strip, location);
			else
				show(location);
			
			Shown = true;
		}

		public void Hide()
		{
			Control.Hide();
			Shown = false;
		}

		public bool IsCursorInPopup()
		{
			var cursorPosition = Cursor.Position;

			if (Control is ContextMenuStrip)
			{
				var screenRect = new Rectangle(_screenLocation.Value, Container.Size);
				return screenRect.Contains(cursorPosition);
			}
			else
			{
				var clientPosition = Control.PointToClient(cursorPosition);
				return Control.ClientRectangle.Contains(clientPosition);
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
			_beforeShow?.Invoke();

			var parent = Control.Parent;
			location = parent.PointToClient(Owner, location);

			location = new Point(
				location.X.WithinRange(Control.Margin.Left, parent.Width - Control.Width - Control.Margin.Right),
				location.Y.WithinRange(Control.Margin.Top, parent.Height - Control.Height - Control.Margin.Bottom));

			Control.Location = location;
			Control.BringToFront();
			Control.Show();
			Control.Focus();
		}

		private void show(ContextMenuStrip contextMenuStrip, Point location)
		{
			ControlHelpers.SetForegroundWindow(
				new HandleRef(
					Control,
					Control.Handle));

			_screenLocation = Owner.PointToScreen(location);
			contextMenuStrip.Show(_screenLocation.Value);
		}

		private Point getLocation()
		{
			int top = Owner.Height + Control.Margin.Top;

			switch (_alignment)
			{
				case HorizontalAlignment.Left:
					return new Point(0, top);
				case HorizontalAlignment.Right:
					return new Point(Owner.Width - Control.Width, top);
				case HorizontalAlignment.Center:
					return new Point((Owner.Width - Control.Width) / 2, top);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}



		public bool OpenOnHover { get; }
		public bool CloseMenuOnClick { get; }
		public bool BorderOnHover { get; }
		public ButtonBase Owner { get; }
		private readonly HorizontalAlignment _alignment;
		private readonly Action _beforeShow;
		private Point? _screenLocation;
	}
}