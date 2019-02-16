using System;

namespace Mtgdb.Controls
{
	public static class DropDownBaseScaler
	{
		public static void ScaleDpi(this DropDownBase dropDown)
		{
			ControlScaler.ScaleDpi(dropDown);

			dropDown.MenuItemsCreated += menuItemsCreated;
			dropDown.Disposed += disposed;

			dropDown.MenuItems.ForEach(item => item.ScaleDpi());

			void menuItemsCreated(object s, EventArgs e) =>
				dropDown.MenuItems.ForEach(item => item.ScaleDpi());

			void disposed(object s, EventArgs e)
			{
				dropDown.Disposed -= disposed;
				dropDown.MenuItemsCreated -= menuItemsCreated;
			}
		}
	}
}