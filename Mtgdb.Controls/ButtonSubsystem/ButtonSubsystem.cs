using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class ButtonSubsystem
	{
		public void SetupButton(ButtonBase control, ButtonImages buttonImages)
		{
			_images[control] = buttonImages;
			setCheckImage(control, (control as CheckBox)?.Checked ?? false, false);
		}

		public void SetupPopup(Popup popup)
		{
			_popups[popup.Owner] = popup;

			popup.Control.Visible = false;

			if (popup.BorderOnHover)
				foreach (var button in popup.Control.Controls.OfType<ButtonBase>())
				{
					button.SetTag(button.FlatAppearance.BorderColor);
					button.FlatAppearance.BorderColor = popup.Control.BackColor;
				}
		}



		private void mouseLeave(object sender, EventArgs e)
		{
			bool isChecked = (sender as CheckBox)?.Checked == true;
			setCheckImage((ButtonBase)sender, isChecked, false);
		}

		private void mouseEnter(object sender, EventArgs e)
		{
			bool isChecked = (sender as CheckBox)?.Checked == true;
			setCheckImage((ButtonBase)sender, isChecked, true);
		}



		private void checkedChanged(object sender, EventArgs e)
		{
			var checkButton = (CheckBox)sender;
			setCheckImage(checkButton, checkButton.Checked, false);
		}



		private void popupOwnerHover(object sender, EventArgs e)
		{
			var owner = (ButtonBase) sender;
			var popup = _popups[owner];

			if (popup.OpenOnHover && !popup.Visible)
			{
				popup.Control.SetTag("Owner", popup.Owner);
				popup.Show();
			}
		}

		private void popupOwnerClick(object sender, EventArgs e)
		{
			var owner = (ButtonBase)sender;
			var popup = _popups[owner];

			if (popup.Visible)
				popup.Hide();
			else if (!popup.OpenOnHover)
			{
				popup.Control.SetTag("Owner", popup.Owner);
				popup.Show();
			}
		}

		private void popupOwnerMouseLeave(object sender, EventArgs e)
		{
			var button = (ButtonBase) sender;
			var popup = _popups[button];
			if (!popup.Visible)
				return;

			if (!popup.IsCursorInPopup() && !popup.IsCursorInButton())
				popup.Hide();
		}

		private void popupMouseLeave(object sender, EventArgs e)
		{
			var popupControl = (Control) sender;
			var owner = popupControl.GetTag<ButtonBase>("Owner");
			var popup = _popups[owner];

			if (!popup.IsCursorInPopup() && !popup.IsCursorInButton())
				popup.Hide();
		}



		public void SubscribeToEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CheckBox)
					((CheckBox) control).CheckedChanged += checkedChanged;

				control.MouseEnter += mouseEnter;
				control.MouseLeave += mouseLeave;
			}

			foreach (var popup in _popups.Values.Distinct())
			{
				popup.Owner.MouseEnter += popupOwnerHover;
				popup.Owner.Click += popupOwnerClick;
				
				popup.Owner.MouseLeave += popupOwnerMouseLeave;

				foreach (var button in popup.Control.Controls.OfType<ButtonBase>())
				{
					button.Click += popupItemClick;
					button.MouseEnter += popupItemMouseEnter;
					button.MouseLeave += popupItemMouseLeave;
				}

				popup.Control.MouseLeave += popupMouseLeave;
			}
		}

		public void UnsubscribeFromEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CheckBox)
					((CheckBox) control).CheckedChanged -= checkedChanged;

				control.MouseEnter -= mouseEnter;
				control.MouseLeave -= mouseLeave;
			}

			foreach (var popup in _popups.Values.Distinct())
			{
				popup.Owner.MouseEnter -= popupOwnerHover;
				popup.Owner.Click -= popupOwnerClick;

				popup.Owner.MouseLeave -= popupOwnerMouseLeave;

				foreach (var button in popup.Control.Controls.OfType<ButtonBase>())
				{
					button.Click -= popupItemClick;
					button.MouseEnter -= popupItemMouseEnter;
					button.MouseLeave -= popupItemMouseLeave;
				}

				popup.Control.MouseLeave -= popupMouseLeave;
			}
		}


		private void popupItemClick(object sender, EventArgs e)
		{
			var button = (ButtonBase)sender;
			var menu = button.Parent;
			var owner = menu.GetTag<ButtonBase>("Owner");
			var popup = _popups[owner];

			if (popup.CloseOnMenuClick)
				popup.Hide();
		}

		private void popupItemMouseEnter(object sender, EventArgs e)
		{
			var button = (ButtonBase)sender;

			var menu = button.Parent;
			var owner = menu.GetTag<ButtonBase>("Owner");
			var popup = _popups[owner];

			if (popup.BorderOnHover)
				button.FlatAppearance.BorderColor = button.GetTag<Color>();
		}

		private void popupItemMouseLeave(object sender, EventArgs e)
		{
			var button = (ButtonBase)sender;
			var menu = button.Parent;
			var owner = menu.GetTag<ButtonBase>("Owner");
			var popup = _popups[owner];

			if (popup.BorderOnHover)
				button.FlatAppearance.BorderColor = menu.BackColor;

			if (!popup.IsCursorInPopup() && !popup.IsCursorInButton())
				popup.Hide();
		}

		private void setCheckImage(ButtonBase control, bool isChecked, bool hovered)
		{
			var images = _images[control];
			control.Image = images.GetImage(isChecked, hovered);
		}

		private readonly Dictionary<ButtonBase, ButtonImages> _images = new Dictionary<ButtonBase, ButtonImages>();
		private readonly Dictionary<ButtonBase, Popup> _popups = new Dictionary<ButtonBase, Popup>();
	}
}