using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public static class DropDownBaseScaler
	{
		public static void ScaleDpi(this DropDownBase dropDown)
		{
			ControlScaler.ScaleDpi(dropDown);
			dropDown.ScaleDpiImages();

			dropDown.MenuItems.ForEach(scale);

			dropDown.MenuItemCreated += menuItemCreated;
			dropDown.Disposed += disposed;
			Dpi.AfterChanged += afterDpiChange;

			void menuItemCreated(object s, MenuItemEventArgs e) =>
				scale(e.MenuItem);

			void scale(ButtonBase b)
			{
				b.ScaleDpiFont();
				b.ScaleDpiPadding();
			}

			void afterDpiChange() =>
				dropDown.UpdateMenuSize();

			void disposed(object s, EventArgs e)
			{
				dropDown.Disposed -= disposed;
				dropDown.MenuItemCreated -= menuItemCreated;
				Dpi.AfterChanged -= afterDpiChange;
			}
		}
	}
}