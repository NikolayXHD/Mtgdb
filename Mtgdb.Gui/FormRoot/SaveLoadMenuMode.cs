using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	internal class SaveLoadMenuMode
	{
		public CustomCheckBox TitleButton { get; set; }
		public CustomCheckBox[] MenuButtons { get; set; }
		public string MtgArenaButtonText { get; set; }
		public bool IsMtgArenaPaste { get; set; }
		public bool IsCurrent { get; set; }
	}
}