using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	internal class MenuItemsAccessor : IReadOnlyList<ButtonBase>
	{
		public MenuItemsAccessor(FlowLayoutPanel menu) =>
			_menu = menu;

		public IEnumerator<ButtonBase> GetEnumerator() =>
			_menu.Controls.Cast<ButtonBase>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

		public int Count =>
			_menu.Controls.Count;

		public ButtonBase this[int index] =>
			(ButtonBase) _menu.Controls[index];

		private readonly FlowLayoutPanel _menu;
	}
}