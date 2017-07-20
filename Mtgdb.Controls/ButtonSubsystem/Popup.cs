using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class Popup
	{
		public Popup(Control control, ButtonBase owner, HorizontalAlignment alignment = HorizontalAlignment.Left, bool openOnHover = true, bool closeOnMenuClick = false, bool borderOnHover = true)
		{
			Control = control;
			OpenOnHover = openOnHover;
			CloseOnMenuClick = closeOnMenuClick;
			BorderOnHover = borderOnHover;
			Owner = owner;
			_alignment = alignment;
		}

		public Control Control { get; }
		
		public bool Visible => Control.Visible;
		public void Hide() => Control.Hide();

		public void Show()
		{
			var location = getLocation();

			if (Control is ContextMenuStrip)
				show((ContextMenuStrip)Control, location);
			else
				show(Control, location);
		}

		private void show(Control control, Point location)
		{
			var parent = control.Parent;
			control.Location = parent.PointToClient(Owner, location);
			control.BringToFront();
			control.Show();
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

		public bool IsCursorInPopup()
		{
			var cursorPosition = Cursor.Position;

			if (Control is ContextMenuStrip)
			{
				var screenRect = new Rectangle(_screenLocation.Value, Control.Size);
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



		private Point getLocation()
		{
			Point ownerLocation;
			switch (_alignment)
			{
				case HorizontalAlignment.Left:
					ownerLocation = new Point(0, Owner.Height);
					break;
				case HorizontalAlignment.Right:
					ownerLocation = new Point(Owner.Width - Control.Width, Owner.Height);
					break;
				case HorizontalAlignment.Center:
					ownerLocation = new Point((Owner.Width - Control.Width) / 2, Owner.Height);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return ownerLocation;
		}


		public bool OpenOnHover { get; }
		public bool CloseOnMenuClick { get; set; }
		public bool BorderOnHover { get; set; }
		public ButtonBase Owner { get; }
		private readonly HorizontalAlignment _alignment;
		private Point? _screenLocation;
	}
}