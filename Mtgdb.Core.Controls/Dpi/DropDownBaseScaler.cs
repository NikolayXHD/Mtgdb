using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public static class DropDownBaseScaler
	{
		public static void ScaleDpi(this DropDownBase dropDown)
		{
			ButtonBaseScaler.ScaleDpi(dropDown);

			dropDown.MenuItemCreated += menuItemsCreated;
			dropDown.Disposed += disposed;

			dropDown.MenuItems.ForEach(scale);

			void menuItemsCreated(object s, ControlEventArgs e) =>
				scale((ButtonBase) e.Control);

			void disposed(object s, EventArgs e)
			{
				dropDown.Disposed -= disposed;
				dropDown.MenuItemCreated -= menuItemsCreated;
			}

			_menuSizeScaler.Setup(dropDown);

			void scale(ButtonBase b)
			{
				b.ScaleDpiFont();
				b.ScaleDpiPadding();
			}
		}

		private static readonly DpiScaler<DropDownBase> _menuSizeScaler =
			new DpiScaler<DropDownBase>(d => d.UpdateMenuSize());
	}
}