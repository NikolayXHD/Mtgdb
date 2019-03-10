using System;

namespace Mtgdb.Controls
{
	public class MenuItemEventArgs : EventArgs
	{
		public MenuItemEventArgs(ButtonBase menuItem)
		{
			MenuItem = menuItem;
		}

		public ButtonBase MenuItem { get; }
	}
}