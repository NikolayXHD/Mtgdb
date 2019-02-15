using System;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public static class ComboBoxScaler
	{
		public static void ScaleDpi(this ComboBox comboBox)
		{
			ControlScaler.ScaleDpi(comboBox);

			new DpiScaler<ComboBox>(cb =>
			{
				cb.ButtonImages = ButtonImages.ScaleDpi((null, Resources.drop_down_48));
			}).Setup(comboBox);

			comboBox.MenuItemsCreated += menuItemsCreated;
			comboBox.Disposed += disposed;

			comboBox.MenuItems.ForEach(item => item.ScaleDpi());

			void menuItemsCreated(object s, EventArgs e) =>
				comboBox.MenuItems.ForEach(item => item.ScaleDpi());

			void disposed(object s, EventArgs e)
			{
				comboBox.Disposed -= disposed;
				comboBox.MenuItemsCreated -= menuItemsCreated;
			}
		}
	}
}