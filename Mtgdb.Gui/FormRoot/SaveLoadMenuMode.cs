using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	internal class SaveLoadMenuMode
	{
		public ButtonBase TitleButton { get; set; }
		public ButtonBase[] MenuButtons { get; set; }
		public string MtgArenaButtonText { get; set; }
		public bool IsMtgArenaPaste { get; set; }
		public bool IsCurrent { get; set; }
	}
}