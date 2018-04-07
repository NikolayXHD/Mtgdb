using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class CardLayoutControlBase : LayoutControl
	{
		public UiModel Ui { get; set; }

		public override void CopyFrom(LayoutControl other)
		{
			base.CopyFrom(other);
			Ui = ((CardLayoutControlBase) other).Ui;
		}
	}
}