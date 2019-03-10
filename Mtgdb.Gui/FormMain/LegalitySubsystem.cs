using System;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using ButtonBase = Mtgdb.Controls.ButtonBase;

namespace Mtgdb.Gui
{
	public class LegalitySubsystem
	{
		public event Action FilterChanged;

		public LegalitySubsystem(
			DropDown menuLegalityFormat,
			ButtonBase buttonLegalityAllowLegal,
			ButtonBase buttonLegalityAllowRestricted,
			ButtonBase buttonLegalityAllowBanned,
			ButtonBase buttonLegalityAllowFuture)
		{
			_menuLegalityFormat = menuLegalityFormat;
			_menuLegalityFormat.SetMenuValues(Legality.Formats.Prepend(Legality.AnyFormat));

			_buttonLegalityAllowLegal = buttonLegalityAllowLegal;
			_buttonLegalityAllowRestricted = buttonLegalityAllowRestricted;
			_buttonLegalityAllowBanned = buttonLegalityAllowBanned;
			_buttonLegalityAllowFuture = buttonLegalityAllowFuture;
		}

		public void SubscribeToEvents()
		{
			updateLegalitySelectorEnabled();

			_menuLegalityFormat.SelectedIndexChanged += handleLegalityControlChanged;
			_buttonLegalityAllowLegal.CheckedChanged += handleLegalityControlChanged;
			_buttonLegalityAllowRestricted.CheckedChanged += handleLegalityControlChanged;
			_buttonLegalityAllowBanned.CheckedChanged += handleLegalityControlChanged;
			_buttonLegalityAllowFuture.CheckedChanged += handleLegalityControlChanged;

			_buttonLegalityAllowLegal.MouseUp += handleMouseClick;
			_buttonLegalityAllowRestricted.MouseUp += handleMouseClick;
			_buttonLegalityAllowBanned.MouseUp += handleMouseClick;
			_menuLegalityFormat.MouseUp += handleMouseClick;
		}

		private void handleMouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				Reset();
		}

		public bool Reset()
		{
			if (_menuLegalityFormat.SelectedIndex == 0 &&
				_buttonLegalityAllowLegal.Checked &&
				_buttonLegalityAllowRestricted.Checked &&
				!_buttonLegalityAllowBanned.Checked &&
				_buttonLegalityAllowFuture.Checked)
			{
				return false;
			}

			_resetting = true;

			_menuLegalityFormat.SelectedIndex = 0;
			_buttonLegalityAllowLegal.Checked = true;
			_buttonLegalityAllowRestricted.Checked = true;
			_buttonLegalityAllowBanned.Checked = false;
			_buttonLegalityAllowFuture.Checked = true;

			_resetting = false;

			handleLegalityChanged();
			return true;
		}

		private void handleLegalityControlChanged(object sender, EventArgs e)
		{
			if (_resetting)
				return;

			handleLegalityChanged();
		}

		private void handleLegalityChanged()
		{
			FilterFormat = getFilterFormat();
			AllowLegal = _buttonLegalityAllowLegal.Checked;
			AllowRestricted = _buttonLegalityAllowRestricted.Checked;
			AllowBanned = _buttonLegalityAllowBanned.Checked;
			AllowFuture = _buttonLegalityAllowFuture.Checked;

			updateLegalitySelectorEnabled();
			FilterChanged?.Invoke();
		}

		private void updateLegalitySelectorEnabled()
		{
			_buttonLegalityAllowLegal.Enabled =
				_buttonLegalityAllowRestricted.Enabled =
					_buttonLegalityAllowBanned.Enabled =
						_buttonLegalityAllowFuture.Enabled =
						!string.IsNullOrEmpty(FilterFormat);
		}

		public bool MatchesLegalityFilter(Card c)
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

			if (AllowFuture)
				if (c.IsFutureIn(FilterFormat))
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

		private string getFilterFormat()
		{
			if (_menuLegalityFormat.SelectedIndex <= 0)
				return null;

			var selectedItem = _menuLegalityFormat.MenuValues[_menuLegalityFormat.SelectedIndex];
			return selectedItem;
		}

		public bool AllowLegal { get; private set;}
		public bool AllowRestricted { get; private set; }
		public bool AllowBanned { get; private set; }
		public bool AllowFuture { get; private set; }

		public void SetFilterFormat(string value)
		{
			for (int i = 0; i < _menuLegalityFormat.MenuValues.Count; i++)
				if (Str.Equals(_menuLegalityFormat.MenuValues[i], value))
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

		public void SetAllowFuture(bool value)
		{
			_buttonLegalityAllowFuture.Checked = value;
		}

		public string FilterFormat { get; private set; } 

		private bool _resetting;

		private readonly DropDown _menuLegalityFormat;
		private readonly ButtonBase _buttonLegalityAllowLegal;
		private readonly ButtonBase _buttonLegalityAllowRestricted;
		private readonly ButtonBase _buttonLegalityAllowBanned;
		private readonly ButtonBase _buttonLegalityAllowFuture;
	}
}