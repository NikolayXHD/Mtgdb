using System;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public class CardLayoutControlBase : LayoutControl
	{
		public Func<UiModel> Ui { get; set; }
	}
}