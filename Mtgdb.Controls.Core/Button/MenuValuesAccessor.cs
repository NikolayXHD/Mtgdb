using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Controls
{
	internal class MenuValuesAccessor : IReadOnlyList<string>
	{
		public MenuValuesAccessor(MenuItemsAccessor menuItems) =>
			_menuItems = menuItems;

		public IEnumerator<string> GetEnumerator() =>
			_menuItems.Select(_ => _.Text).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

		public int Count =>
			_menuItems.Count;

		public string this[int index] =>
			_menuItems[index].Text;

		private readonly MenuItemsAccessor _menuItems;
	}
}