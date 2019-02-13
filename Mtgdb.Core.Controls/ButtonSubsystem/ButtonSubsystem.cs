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
				popup.Owner.MouseClick += popupOwnerClick;
				popup.Owner.KeyDown += popupOwnerKeyDown;

				foreach (Control button in popup.MenuControl.Controls)
				{
					button.MouseClick += popupItemClick;
					button.KeyDown += popupItemKeyDown;
					button.PreviewKeyDown += popupItemPreviewKeyDown;
				}
				
				popup.Owner.PreviewKeyDown += popupOwnerPreviewKeyDown;
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
				popup.Owner.MouseClick -= popupOwnerClick;
				popup.Owner.KeyDown -= popupOwnerKeyDown;

				foreach (var button in popup.MenuControl.Controls.OfType<CustomCheckBox>())
				{
					button.MouseClick -= popupItemClick;
					button.KeyDown -= popupItemKeyDown;
					button.PreviewKeyDown -= popupItemPreviewKeyDown;
				}

				popup.Owner.PreviewKeyDown -= popupOwnerPreviewKeyDown;
			}

			Application.RemoveMessageFilter(this);
		}



		private void popupItemClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			var button = (CustomCheckBox) sender;
			var container = button.Parent;
			var owner = container.GetTag<CustomCheckBox>("Owner");
			var popup = _popupsByOwner[owner];

			popupItemPressed(popup, focus: false);
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
				case Keys.Space:
				case Keys.Enter:
					popupItemPressed(popup, focus: true);
					break;

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

		private static void popupItemPressed(Popup popup, bool focus)
		{
			if (popup.CloseMenuOnClick)
				hide(popup, focus);
		}



		private void popupOwnerClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
				return;

			var popup = _popupsByOwner[(CustomCheckBox) sender];
			popupOwnerPressed(popup, focus: false);
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
				case Keys.Space:
				case Keys.Enter:
					popupOwnerPressed(popup, focus: true);
					break;
				
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

		private static void popupOwnerPressed(Popup popup, bool focus)
		{
			
			if (popup.Shown)
				hide(popup, focus);
			else
				show(popup, focus);
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