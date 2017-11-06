using System;
using System.Windows.Forms;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class LegalitySubsystem
	{
		private readonly ComboBox _menuLegalityFormat;
		private readonly CheckBox _buttonLegalityAllowLegal;
		private readonly CheckBox _buttonLegalityAllowRestricted;
		private readonly CheckBox _buttonLegalityAllowBanned;

		public event Action FilterChanged;

		public LegalitySubsystem(
			ComboBox menuLegalityFormat,
			CheckBox buttonLegalityAllowLegal,
			CheckBox buttonLegalityAllowRestricted,
			CheckBox buttonLegalityAllowBanned)
		{
			_menuLegalityFormat = menuLegalityFormat;
			_buttonLegalityAllowLegal = buttonLegalityAllowLegal;
			_buttonLegalityAllowRestricted = buttonLegalityAllowRestricted;
			_buttonLegalityAllowBanned = buttonLegalityAllowBanned;
			AnyFormat = (string) _menuLegalityFormat.Items[0];
		}

		public void SubscribeToEvents()
		{
			updateLegalitySelectorEnabled();

			_menuLegalityFormat.SelectedIndexChanged += legalityChanged;
			_buttonLegalityAllowLegal.CheckedChanged += legalityChanged;
			_buttonLegalityAllowRestricted.CheckedChanged += legalityChanged;
			_buttonLegalityAllowBanned.CheckedChanged += legalityChanged;
		}

		private void legalityChanged(object sender, EventArgs e)
		{
			FilterFormat = getFilterFormat();
			AllowLegal = _buttonLegalityAllowLegal.Checked;
			AllowRestricted = _buttonLegalityAllowRestricted.Checked;
			AllowBanned = _buttonLegalityAllowBanned.Checked;

			updateLegalitySelectorEnabled();
			FilterChanged?.Invoke();
		}

		private void updateLegalitySelectorEnabled()
		{
			_buttonLegalityAllowLegal.Enabled =
				_buttonLegalityAllowRestricted.Enabled =
					_buttonLegalityAllowBanned.Enabled =
						!string.IsNullOrEmpty(FilterFormat);
		}

		public bool IsAllowedInFormat(Card c)
		{
			if (FilterFormat == null)
				return true;

			if (AllowLegal)
				if (c.IsLegalIn(FilterFormat))
					return true;

			if (AllowRestricted)
				if (c.IsRestrictedIn(FilterFormat))
					return true;

			if (AllowBanned)
				if (c.IsBannedIn(FilterFormat))
					return true;

			return false;
		}

		public string GetWarning(Card c)
		{
			if (FilterFormat == null)
				return null;

			if (c.IsLegalIn(FilterFormat))
				return null;

			if (c.IsRestrictedIn(FilterFormat))
				return Legality.Restricted;

			if (c.IsBannedIn(FilterFormat))
				return Legality.Banned;

			return Legality.Illegal;
		}

		public readonly string AnyFormat;

		private string getFilterFormat()
		{
			if (_menuLegalityFormat.SelectedIndex <= 0)
				return null;

			var selectedItem = _menuLegalityFormat.Items[_menuLegalityFormat.SelectedIndex];
			string selectedFormat = (string) selectedItem;
			return selectedFormat;
		}

		public bool AllowLegal { get; private set;}
		public bool AllowRestricted { get; private set; }
		public bool AllowBanned { get; private set; }

		public void SetFilterFormat(string value)
		{
			for (int i = 0; i < _menuLegalityFormat.Items.Count; i++)
				if (Str.Equals((string)_menuLegalityFormat.Items[i], value))
				{
					_menuLegalityFormat.SelectedIndex = i;
					return;
				}

			_menuLegalityFormat.SelectedIndex = 0;
		}

		public void SetAllowLegal(bool value)
		{
			_buttonLegalityAllowLegal.Checked = value;
		}

		public void SetAllowRestricted(bool value)
		{
			_buttonLegalityAllowRestricted.Checked = value;
		}

		public void SetAllowBanned(bool value)
		{
			_buttonLegalityAllowBanned.Checked = value;
		}

		public string FilterFormat { get; private set; } 
	}
}