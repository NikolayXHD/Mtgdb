using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class Popup : IComponent
	{
		public Popup(
			Control menuControl,
			ButtonBase owner,
			HorizontalAlignment alignment = HorizontalAlignment.Left,
			bool closeMenuOnClick = true,
			Action beforeShow = null)
		{
			MenuControl = menuControl;
			CloseMenuOnClick = closeMenuOnClick;
			Owner = owner;
			_alignment = alignment;
			_beforeShow = beforeShow;

			MenuControl.Visible = false;

			Owner.PressDown += popupOwnerPressed;
			Owner.KeyDown += popupOwnerKeyDown;
			Owner.PreviewKeyDown += popupOwnerPreviewKeyDown;

			foreach (var button in MenuControl.Controls.OfType<ButtonBase>())
				subscribeToEvents(button);

			MenuControl.ControlAdded += controlAdded;
			MenuControl.ControlRemoved += controlRemoved;

			PopupSubsystem.Instance.GlobalMouseDown += globalMouseDown;
		}

		public void OpenPopup() =>
			show(focus: false);

		public void ClosePopup() =>
			hide(focus: false);

		private void show(bool focus)
		{
			var prevOwner = MenuControl.GetTag<ButtonBase>("Owner");
			if (prevOwner != null && prevOwner is ButtonBase prevCheck)
				prevCheck.Checked = false;

			if (Owner is ButtonBase check)
				check.Checked = true;

			MenuControl.SetTag("Owner", Owner);

			_beforeShow?.Invoke();

			var location = getLocation();

			if (MenuControl is ContextMenuStrip strip)
				showContextMenu(strip, location);
			else
				showRegularMenu(location);

			Shown = true;

			if (focus)
				FocusFirstMenuItem();
		}

		private void hide(bool focus)
		{
			if (Owner is ButtonBase check)
				check.Checked = false;

			hide();

			if (focus && Owner.TabStop && Owner.Enabled)
				Owner.Focus();
		}



		private void popupItemPressed(object sender, EventArgs eventArgs)
		{
			var owner = MenuControl.GetTag<ButtonBase>("Owner");

			if (owner != Owner)
				return;

			if (CloseMenuOnClick)
				hide(false);
		}

		private static void popupItemPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Right:
				case Keys.Left:
					e.IsInputKey = true;
					break;
			}
		}

		private void popupItemKeyDown(object sender, KeyEventArgs e)
		{
			var owner = MenuControl.GetTag<ButtonBase>("Owner");
			if (Owner != owner)
				return;

			switch (e.KeyData)
			{
				case Keys.Escape:
					hide(focus: true);
					break;

				case Keys.Right:
					hide(focus: true);
					SendKeys.Send("{TAB}");
					break;

				case Keys.Left:
					hide(focus: true);
					SendKeys.Send("+{TAB}"); // Alt
					break;
			}
		}



		private void popupOwnerPressed(object sender, EventArgs e)
		{
			if (Shown)
				hide(false);
			else
				show(false);
		}

		private static void popupOwnerPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Up:
				case Keys.Down:
					e.IsInputKey = true;
					break;
			}
		}

		private void popupOwnerKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
				case Keys.Down:
					if (!Shown)
						show(focus: true);
					else
						FocusFirstMenuItem();
					break;

				case Keys.Up:
					FocusLastMenuItem();
					break;

				case Keys.Escape:
					hide(focus: false);
					break;
			}
		}

		private void hide()
		{
			MenuControl.Hide();
			Shown = false;
		}

		public bool IsCursorInPopup()
		{
			var cursorPosition = Cursor.Position;

			if (MenuControl is ContextMenuStrip)
			{
				var screenRect = new Rectangle(_screenLocation.Value, MenuControl.Size);
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

		public void FocusFirstMenuItem()
		{
			MenuControl.Controls.OfType<ButtonBase>()
				.Where(_ => _.TabStop && _.Enabled)
				.AtMin(_ => _.TabIndex)
				.FindOrDefault()
				?.Focus();
		}

		public void FocusLastMenuItem()
		{
			MenuControl.Controls.OfType<ButtonBase>()
				.Where(_ => _.TabStop && _.Enabled)
				.AtMax(_ => _.TabIndex)
				.FindOrDefault()
				?.Focus();
		}

		private void showRegularMenu(Point location)
		{
			var parent = MenuControl.Parent;
			location = parent.PointToClient(Owner, location);

			location = new Point(
				location.X.WithinRange(MenuControl.Margin.Left, parent.Width - MenuControl.Width - MenuControl.Margin.Right),
				location.Y.WithinRange(MenuControl.Margin.Top, parent.Height - MenuControl.Height - MenuControl.Margin.Bottom));

			MenuControl.Location = location;
			MenuControl.BringToFront();
			MenuControl.Show();
		}

		private void showContextMenu(ContextMenuStrip contextMenuStrip, Point location)
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

		private void globalMouseDown(object sender, EventArgs e)
		{
			if (Shown && !IsCursorInPopup() && !IsCursorInButton())
				hide(focus: false);
		}

		private void controlAdded(object sender, ControlEventArgs e)
		{
			if (e.Control is ButtonBase button)
				subscribeToEvents(button);
		}

		private void controlRemoved(object sender, ControlEventArgs e)
		{
			if (e.Control is ButtonBase button)
				unsubscribeFromEvents(button);
		}

		private void subscribeToEvents(ButtonBase button)
		{
			button.Pressed += popupItemPressed;
			button.KeyDown += popupItemKeyDown;
			button.PreviewKeyDown += popupItemPreviewKeyDown;
		}

		private void unsubscribeFromEvents(ButtonBase button)
		{
			button.Pressed -= popupItemPressed;
			button.KeyDown -= popupItemKeyDown;
			button.PreviewKeyDown -= popupItemPreviewKeyDown;
		}



		public void Dispose()
		{
			Owner.PressDown -= popupOwnerPressed;
			Owner.KeyDown -= popupOwnerKeyDown;
			Owner.PreviewKeyDown -= popupOwnerPreviewKeyDown;

			foreach (var button in MenuControl.Controls.OfType<ButtonBase>())
				unsubscribeFromEvents(button);

			MenuControl.ControlAdded -= controlAdded;
			MenuControl.ControlRemoved -= controlRemoved;

			PopupSubsystem.Instance.GlobalMouseDown -= globalMouseDown;

			Disposed?.Invoke(this, EventArgs.Empty);
		}

		private Control MenuControl { get; }
		private bool Shown { get; set; }

		private bool CloseMenuOnClick { get; }
		private ButtonBase Owner { get; }

		private readonly HorizontalAlignment _alignment;
		private readonly Action _beforeShow;
		private Point? _screenLocation;

		public ISite Site { get; set; }
		public event EventHandler Disposed;
	}
}