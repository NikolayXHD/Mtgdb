using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ButtonSubsystem : IMessageFilter
	{
		public void SetupButton(ButtonBase control, ButtonImages buttonImages)
		{
			_images[control] = buttonImages;
			setCheckImage(control, (control as CheckBox)?.Checked ?? false, false);
		}

		public void SetupPopup(Popup popup)
		{
			_popupsByOwner[popup.Owner] = popup;

			popup.Control.Visible = false;

			if (popup.BorderOnHover)
				foreach (var button in popup.Container.Controls.OfType<ButtonBase>())
				{
					button.SetTag(button.FlatAppearance.BorderColor);
					button.FlatAppearance.BorderColor = popup.Control.BackColor;
				}
		}

		public void OpenPopup(ButtonBase popupButton)
		{
			var popup = _popupsByOwner[popupButton];
			show(popup);
		}

		public void ClosePopup(ButtonBase popupButton)
		{
			var popup = _popupsByOwner[popupButton];
			popup.Hide();
		}

		private void mouseLeave(object sender, EventArgs e)
		{
			if (sender is ButtonBase box)
			{
				bool isChecked = (box as CheckBox)?.Checked == true;
				setCheckImage(box, isChecked, false);
			}
		}

		private void mouseEnter(object sender, EventArgs e)
		{
			bool isChecked = (sender as CheckBox)?.Checked == true;
			setCheckImage((ButtonBase)sender, isChecked, true);
		}



		private void checkedChanged(object sender, EventArgs e)
		{
			var cursorPosition = Cursor.Position;
			var checkButton = (CheckBox)sender;
			bool hovered = checkButton.ClientRectangle.Contains(checkButton.PointToClient(cursorPosition));
			setCheckImage(checkButton, checkButton.Checked, hovered);
		}



		private void popupOwnerHover(object sender, EventArgs e)
		{
			var owner = (ButtonBase) sender;
			var popup = _popupsByOwner[owner];

			if (popup.OpenOnHover && !popup.Visible)
				show(popup);
		}

		private void popupOwnerClick(object sender, EventArgs e)
		{
			var owner = (ButtonBase)sender;
			var popup = _popupsByOwner[owner];

			if (popup.Visible)
			{
				popup.Hide();
				return;
			}

			if (!popup.OpenOnHover)
				show(popup);
		}

		private static void show(Popup popup)
		{
			popup.Control.SetTag("Owner", popup.Owner);
			popup.Container.SetTag("Owner", popup.Owner);
			popup.Show();
		}

		private void popupOwnerMouseLeave(object sender, EventArgs e)
		{
			var button = (ButtonBase) sender;
			var popup = _popupsByOwner[button];
			
			if (!popup.Visible)
				return;

			if (!popup.IsCursorInPopup() && !popup.IsCursorInButton() && popup.OpenOnHover)
				popup.Hide();
		}

		private void popupMouseLeave(object sender, EventArgs e)
		{
			var control = (Control) sender;
			var owner = control.GetTag<ButtonBase>("Owner");
			var popup = _popupsByOwner[owner];

			if (!popup.IsCursorInPopup() && !popup.IsCursorInButton() && popup.OpenOnHover)
				popup.Hide();
		}

		private void popupKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyData != Keys.Escape)
				return;

			var control = (Control) sender;
			var owner = control.GetTag<ButtonBase>("Owner");
			var popup = _popupsByOwner[owner];

			popup.Hide();
		}



		public void SubscribeToEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CheckBox box)
					box.CheckedChanged += checkedChanged;

				control.MouseEnter += mouseEnter;
				control.MouseLeave += mouseLeave;
			}

			foreach (var popup in _popupsByOwner.Values.Distinct())
			{
				popup.Owner.MouseEnter += popupOwnerHover;
				popup.Owner.Click += popupOwnerClick;
				
				popup.Owner.MouseLeave += popupOwnerMouseLeave;

				foreach (Control button in popup.Container.Controls)
				{
					button.Click += popupItemClick;
					button.MouseEnter += popupItemMouseEnter;
					button.MouseLeave += popupItemMouseLeave;
				}

				popup.Container.MouseLeave += popupMouseLeave;
				popup.Control.MouseLeave += popupMouseLeave;

				popup.Control.PreviewKeyDown += popupKeyDown;
			}

			Application.AddMessageFilter(this);
		}

		public void UnsubscribeFromEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CheckBox box)
					box.CheckedChanged -= checkedChanged;

				control.MouseEnter -= mouseEnter;
				control.MouseLeave -= mouseLeave;
			}

			foreach (var popup in _popupsByOwner.Values.Distinct())
			{
				popup.Owner.MouseEnter -= popupOwnerHover;
				popup.Owner.Click -= popupOwnerClick;

				popup.Owner.MouseLeave -= popupOwnerMouseLeave;

				foreach (var button in popup.Container.Controls.OfType<ButtonBase>())
				{
					button.Click -= popupItemClick;
					button.MouseEnter -= popupItemMouseEnter;
					button.MouseLeave -= popupItemMouseLeave;
				}

				popup.Container.MouseLeave -= popupMouseLeave;
				popup.Control.MouseLeave -= popupMouseLeave;
				popup.Control.PreviewKeyDown -= popupKeyDown;
			}

			Application.RemoveMessageFilter(this);
		}


		private void popupItemClick(object sender, EventArgs e)
		{
			if (!(sender is ButtonBase))
				return;

			var button = (ButtonBase)sender;
			var container = button.Parent;
			var owner = container.GetTag<ButtonBase>("Owner");
			var popup = _popupsByOwner[owner];

			if (popup.CloseMenuOnClick)
				popup.Hide();
		}

		private void popupItemMouseEnter(object sender, EventArgs e)
		{
			if (!(sender is ButtonBase))
				return;

			var button = (ButtonBase)sender;

			var container = button.Parent;
			var owner = container.GetTag<ButtonBase>("Owner");
			var popup = _popupsByOwner[owner];

			if (popup.BorderOnHover)
				button.FlatAppearance.BorderColor = button.GetTag<Color>();
		}

		private void popupItemMouseLeave(object sender, EventArgs e)
		{
			var button = (Control)sender;
			var container = button.Parent;
			var owner = container.GetTag<ButtonBase>("Owner");
			var popup = _popupsByOwner[owner];

			if (popup.BorderOnHover && sender is ButtonBase)
				((ButtonBase) button).FlatAppearance.BorderColor = container.BackColor;

			if (!popup.IsCursorInPopup() && !popup.IsCursorInButton() && popup.OpenOnHover)
				popup.Hide();
		}

		private void setCheckImage(ButtonBase control, bool isChecked, bool hovered)
		{
			var images = _images[control];
			control.Image = images.GetImage(isChecked, hovered);
		}

		public bool PreFilterMessage(ref Message m)
		{
			// WM_LBUTTONDOWN, WM_MBUTTONDOWN, WM_RBUTTONDOWN 
			if (m.Msg == 0x0201 || m.Msg == 0x0207 || m.Msg == 0x0204)
			{
				foreach (Popup popup in _popupsByOwner.Values)
				{
					if (!popup.Visible)
						continue;

					if (!popup.IsCursorInPopup() && !popup.IsCursorInButton())
						popup.Hide();
				}
			}

			return false;
		}

		private readonly Dictionary<ButtonBase, ButtonImages> _images = new Dictionary<ButtonBase, ButtonImages>();

		private readonly Dictionary<ButtonBase, Popup> _popupsByOwner = new Dictionary<ButtonBase, Popup>();
	}
}