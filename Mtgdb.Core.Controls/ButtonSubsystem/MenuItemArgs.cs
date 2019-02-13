using System;
using System.Collections.Generic;

namespace Mtgdb.Controls
{
	public class MenuItemsArgs : EventArgs
	{
		public MenuItemsArgs(IReadOnlyList<CustomCheckBox> menuItems) =>
			MenuItems = menuItems;

		public IReadOnlyList<CustomCheckBox> MenuItems { get; }
	}
}