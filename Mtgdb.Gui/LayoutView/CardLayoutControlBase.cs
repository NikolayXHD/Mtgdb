using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class CardLayoutControlBase : LayoutControl
	{
		public UiModel Ui { get; set; }

		public override void CopyTo(LayoutControl other)
		{
			base.CopyTo(other);
			((CardLayoutControlBase) other).Ui = Ui;
		}
	}
}