using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class Dropdown : ButtonBase
	{
		public Dropdown()
		{
			AutoCheck = false;

			PressDown += popupOwnerPressed;
			KeyDown += popupOwnerKeyDown;
			PreviewKeyDown += popupOwnerPreviewKeyDown;
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

			Checked = true;

			MenuControl.SetTag("Owner", this);

			BeforeShow?.Invoke();

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
			Checked = false;

			hide();

			if (focus && TabStop && Enabled)
				Focus();
		}



		private void popupItemPressed(object sender, EventArgs eventArgs)
		{
			var owner = MenuControl.GetTag<ButtonBase>("Owner");

			if (owner != this)
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
			if (owner != this)
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
			bool isCursorInButton = ClientRectangle.Contains(PointToClient(cursorPosition));
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
			location = parent.PointToClient(this, location);

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

			_screenLocation = PointToScreen(location);
			contextMenuStrip.Show(_screenLocation.Value);
		}

		private Point getLocation()
		{
			int top = Height + (MarginTop ?? MenuControl.Margin.Top);

			switch (MenuAlignment)
			{
				case HorizontalAlignment.Left:
					return new Point(0, top);
				case HorizontalAlignment.Right:
					return new Point(Width - MenuControl.Width, top);
				case HorizontalAlignment.Center:
					return new Point((Width - MenuControl.Width) / 2, top);
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

		private void subscribeToEvents(Control menuControl)
		{
			menuControl.Visible = false;

			foreach (var button in menuControl.Controls.OfType<ButtonBase>())
				subscribeToEvents(button);

			menuControl.ControlAdded += controlAdded;
			menuControl.ControlRemoved += controlRemoved;
		}

		private void unsubscribeFromEvents(Control menuControl)
		{
			foreach (var button in menuControl.Controls.OfType<ButtonBase>())
				unsubscribeFromEvents(button);

			menuControl.ControlAdded -= controlAdded;
			menuControl.ControlRemoved -= controlRemoved;
		}

		protected override void Dispose(bool disposing)
		{
			PressDown -= popupOwnerPressed;
			KeyDown -= popupOwnerKeyDown;
			PreviewKeyDown -= popupOwnerPreviewKeyDown;

			unsubscribeFromEvents(_menuControl);

			PopupSubsystem.Instance.GlobalMouseDown -= globalMouseDown;

			base.Dispose(disposing);
		}

		[Category("Settings"), DefaultValue(null)]
		public virtual int? MarginTop { get; set; }

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoCheck
		{
			get => base.AutoCheck;
			set => base.AutoCheck = value;
		}


		private Control _menuControl;
		[Category("Settings")]
		public virtual Control MenuControl
		{
			get => _menuControl;
			set
			{
				if (_menuControl == value)
					return;

				_menuControl?.Invoke0(unsubscribeFromEvents);
				_menuControl = value;
				_menuControl?.Invoke0(subscribeToEvents);
			}
		}



		private bool Shown { get; set; }

		[Category("Settings"), DefaultValue(true)]
		public bool CloseMenuOnClick { get; set; } = true;

		[Category("Settings"), DefaultValue(typeof(HorizontalAlignment), "Left")]
		public HorizontalAlignment MenuAlignment { get; set; } = HorizontalAlignment.Left;
		
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Action BeforeShow { get; set; }

		private Point? _screenLocation;
	}
}