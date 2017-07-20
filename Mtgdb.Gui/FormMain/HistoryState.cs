using System.Collections.Generic;

namespace Mtgdb.Gui
{
	internal class HistoryState
	{
		public List<GuiSettings> SettingsHistory { get; set; }
		public int SettingsIndex { get; set; }
	}
}