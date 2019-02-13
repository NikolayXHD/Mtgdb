using System;
using System.Collections.Generic;

namespace Mtgdb.Controls
{
	public class MenuItemsArgs : EventArgs
	{
		public MenuItemsArgs(IReadOnlyList<ButtonBase> menuItems) =>
			MenuItems = menuItems;

		public IReadOnlyList<ButtonBase> MenuItems { get; }
	}
}