using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ButtonSubsystem : IMessageFilter
	{
		public void SetupButton(CustomCheckBox control, ButtonImages buttonImages)
		{
			_images[control] = buttonImages;
			setCheckImage(control, control.Checked);
		}

		public void SetupPopup(Popup popup)
		{
			_popupsByOwner[popup.Owner] = popup;
			popup.MenuControl.Visible = false;
		}

		public void OpenPopup(CustomCheckBox popupButton)
		{
			var popup = _popupsByOwner[popupButton];
			show(popup, focus: false);
		}

		private void checkedChanged(object sender, EventArgs e)
		{
			var checkButton = (CustomCheckBox)sender;
			setCheckImage(checkButton, checkButton.Checked);
		}



		private static void hide(Popup popup, bool focus)
		{
			if (popup.Owner is CustomCheckBox check)
				check.Checked = false;

			popup.Hide();

			if (focus && popup.Owner.TabStop && popup.Owner.Enabled)
				popup.Owner.Focus();
		}

		private static void show(Popup popup, bool focus)
		{
			var prevOwner = popup.MenuControl.GetTag<CustomCheckBox>("Owner");
			if (prevOwner != null && prevOwner is CustomCheckBox prevCheck)
				prevCheck.Checked = false;

			if (popup.Owner is CustomCheckBox check)
				check.Checked = true;

			popup.MenuControl.SetTag("Owner", popup.Owner);

			popup.Show();

			if (focus)
				popup.FocusFirstMenuItem();
		}



		public void SubscribeToEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CustomCheckBox box)
					box.CheckedChanged += checkedChanged;
			}

			foreach (var popup in _popupsByOwner.Values.Distinct())
			{
				popup.Owner.PressDown += popupOwnerPressed;
				popup.Owner.KeyDown += popupOwnerKeyDown;
				popup.Owner.PreviewKeyDown += popupOwnerPreviewKeyDown;

				foreach (var button in popup.MenuControl.Controls.OfType<CustomCheckBox>())
				{
					button.Pressed += popupItemPressed;
					button.KeyDown += popupItemKeyDown;
					button.PreviewKeyDown += popupItemPreviewKeyDown;
				}
			}

			Application.AddMessageFilter(this);
		}

		public void UnsubscribeFromEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CustomCheckBox box)
					box.CheckedChanged -= checkedChanged;
			}

			foreach (var popup in _popupsByOwner.Values.Distinct())
			{
				popup.Owner.PressDown -= popupOwnerPressed;
				popup.Owner.KeyDown -= popupOwnerKeyDown;
				popup.Owner.PreviewKeyDown -= popupOwnerPreviewKeyDown;

				foreach (var button in popup.MenuControl.Controls.OfType<CustomCheckBox>())
				{
					button.Pressed -= popupItemPressed;
					button.KeyDown -= popupItemKeyDown;
					button.PreviewKeyDown -= popupItemPreviewKeyDown;
				}
			}

			Application.RemoveMessageFilter(this);
		}



		private void popupItemPressed(object sender, EventArgs eventArgs)
		{
			var button = (CustomCheckBox) sender;
			var container = button.Parent;
			var owner = container.GetTag<CustomCheckBox>("Owner");
			var popup = _popupsByOwner[owner];

			if (popup.CloseMenuOnClick)
				hide(popup, false);
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
			var button = (CustomCheckBox) sender;
			var container = button.Parent;
			var owner = container.GetTag<CustomCheckBox>("Owner");
			var popup = _popupsByOwner[owner];

			switch (e.KeyData)
			{
				case Keys.Escape:
					hide(popup, focus: true);
					break;

				case Keys.Right:
					hide(popup, focus: true);
					SendKeys.Send("{TAB}");
					break;

				case Keys.Left:
					hide(popup, focus: true);
					SendKeys.Send("+{TAB}"); // Alt
					break;
			}
		}



		private void popupOwnerPressed(object sender, EventArgs e)
		{
			var popup = _popupsByOwner[(CustomCheckBox) sender];
			if (popup.Shown)
				hide(popup, false);
			else
				show(popup, false);
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
			var button = (CustomCheckBox) sender;
			var popup = _popupsByOwner[button];

			switch (e.KeyData)
			{
				case Keys.Down:
					if (!popup.Shown)
						show(popup, focus: true);
					else
						popup.FocusFirstMenuItem();
					break;

				case Keys.Up:
					popup.FocusLastMenuItem();
					break;

				case Keys.Escape:
					hide(popup, focus: false);
					break;
			}
		}



		private void setCheckImage(CustomCheckBox control, bool isChecked) =>
			control.Image = _images[control]?.GetImage(isChecked);

		public bool PreFilterMessage(ref Message m)
		{
			// ReSharper disable InconsistentNaming
			// ReSharper disable IdentifierTypo
			const int WM_LBUTTONDOWN = 0x0201;
			const int WM_MBUTTONDOWN = 0x0207;
			const int WM_RBUTTONDOWN = 0x0204;
			// ReSharper restore IdentifierTypo
			// ReSharper restore InconsistentNaming

			switch (m.Msg)
			{
				case WM_LBUTTONDOWN:
				case WM_MBUTTONDOWN:
				case WM_RBUTTONDOWN:
					foreach (var popup in _popupsByOwner.Values)
						if (popup.Shown && !popup.IsCursorInPopup() && !popup.IsCursorInButton())
							hide(popup, focus: false);

					break;
			}

			return false;
		}

		private readonly Dictionary<CustomCheckBox, ButtonImages> _images = new Dictionary<CustomCheckBox, ButtonImages>();

		private readonly Dictionary<CustomCheckBox, Popup> _popupsByOwner = new Dictionary<CustomCheckBox, Popup>();
	}
}