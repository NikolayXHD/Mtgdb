using System;
using Mtgdb.Controls;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	public class CardLayoutControlBase : LayoutControl
	{
		public Func<UiModel> Ui { get; set; }
	}
}