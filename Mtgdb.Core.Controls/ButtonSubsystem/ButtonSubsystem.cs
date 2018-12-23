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

		private void checkedChanged(object sender, EventArgs e)
		{
			var checkButton = (CheckBox)sender;
			setCheckImage(checkButton, checkButton.Checked);
		}



		private void popupOwnerClick(object sender, EventArgs e)
		{
			var popup = _popupsByOwner[(ButtonBase)sender];

			if (popup.Shown)
				popup.Hide();
			else
				show(popup);
		}

		private static void show(Popup popup)
		{
			popup.MenuControl.SetTag("Owner", popup.Owner);
			popup.Show();
		}

		private void popupKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
			{
				var control = (Control) sender;
				var owner = control.GetTag<ButtonBase>("Owner");
				var popup = _popupsByOwner[owner];

				popup.Hide();
			}
		}


		public void SetupComboBox(ComboBox menu)
		{
			const TextFormatFlags textFormat =
				TextFormatFlags.NoClipping |
				TextFormatFlags.NoPrefix |
				TextFormatFlags.VerticalCenter |
				TextFormatFlags.TextBoxControl;

			menu.DrawMode = DrawMode.OwnerDrawVariable;
			menu.FlatStyle = FlatStyle.Flat;

			menu.MeasureItem += (s, e) =>
				e.Graphics.MeasureText((string) menu.Items[e.Index], menu.Font, menu.Size, textFormat);

			menu.DrawItem += (s, e) =>
			{
				e.DrawBackground();

				if (e.Index >= 0 && e.Index < menu.Items.Count)
					e.Graphics.DrawText((string) menu.Items[e.Index], menu.Font, e.Bounds, menu.ForeColor, textFormat);

				e.DrawFocusRectangle();
			};
		}

		public void SubscribeToEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CheckBox box)
					box.CheckedChanged += checkedChanged;
			}

			foreach (var popup in _popupsByOwner.Values.Distinct())
			{
				popup.Owner.Click += popupOwnerClick;

				foreach (Control button in popup.MenuControl.Controls)
					button.Click += popupItemClick;

				popup.MenuControl.PreviewKeyDown += popupKeyDown;
			}

			Application.AddMessageFilter(this);
		}

		public void UnsubscribeFromEvents()
		{
			foreach (var control in _images.Keys)
			{
				if (control is CheckBox box)
					box.CheckedChanged -= checkedChanged;
			}

			foreach (var popup in _popupsByOwner.Values.Distinct())
			{
				popup.Owner.Click -= popupOwnerClick;

				foreach (var button in popup.MenuControl.Controls.OfType<ButtonBase>())
					button.Click -= popupItemClick;

				popup.MenuControl.PreviewKeyDown -= popupKeyDown;
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

		private void setCheckImage(ButtonBase control, bool isChecked) =>
			control.Image = _images[control]?.GetImage(isChecked);

		public bool PreFilterMessage(ref Message m)
		{
			switch (m.Msg)
			{
				// WM_LBUTTONDOWN, WM_MBUTTONDOWN, WM_RBUTTONDOWN
				case 0x0201:
				case 0x0207:
				case 0x0204:
					foreach (var popup in _popupsByOwner.Values)
						if (popup.Shown && !popup.IsCursorInPopup() && !popup.IsCursorInButton())
							popup.Hide();

					break;
			}

			return false;
		}

		private readonly Dictionary<ButtonBase, ButtonImages> _images = new Dictionary<ButtonBase, ButtonImages>();

		private readonly Dictionary<ButtonBase, Popup> _popupsByOwner = new Dictionary<ButtonBase, Popup>();
	}
}