using System;
using System.ComponentModel;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupTooltips()
		{
			TooltipController.SetTooltip(this,
				"Undo: Ctrl+Z, Alt+Left",
				"Click to undo your last action",
				_buttonUndo);

			TooltipController.SetTooltip(this,
				"Redo: Ctrl+Y, Alt+Right",
				"Click to repeat the action cancelled with undo",
				_buttonRedo);

			TooltipController.SetTooltip(this,
				"Deck statistics",
				"Opens a Pivot report window. Use it to view \r\n" +
				"mana curve, price breakdown, or create \r\n" +
				"a custom report by moving field captions between\r\n" +
				"Row, Column and Summary areas of grid.",
				_buttonStat);

			TooltipController.SetTooltip(this,
				"Print deck: Ctrl+P",
				"The print buttons doesn't actually print, instead\r\n" +
				"it creates images of cards by groups of 8\r\n" +
				"that can be printed on A4 paper.",
				_buttonPrint);

			TooltipController.SetTooltip(this,
				"Clear deck",
				"Use it to start creating a new deck from scratch",
				_buttonClear);

			TooltipController.SetTooltip(this,
				"Enable / disable tooltips",
				"Tooltips are helpful but also annoying.\r\n" +
				"Uncheck this button to disable tooltips.\r\n" +
				"\r\n" +
				"Tooltips on card text have selectable text. " +
				"They can be used to select a part of a long text not fitting into the field area.\r\n" +
				"\r\n" +
				"Hold Alt key to temporarily disable / enable tooltips.\r\n" +
				"\r\n" +
				"When selecting text in a small field disabling tooltip by holding Alt key helps avoid distraction from tooltips.\r\n" +
				"\r\n" +
				"Temporarily enabling tooltip by holding Alt key helps to see one particular tooltip for a very large " +
				"text field, e.g. some long Rulings, without enabling tooltips in general.",
				_buttonTooltips);

			TooltipController.SetTooltip(this,
				"Show / hide filter panels",
				"filter panels are located on top and right edges of the window.",
				_buttonShowFilterPanels);

			TooltipController.SetTooltip(this,
				"Update",
				"Shows a window where you can\r\n" +
				"  * Check for a new version of Mtgdb.Gui\r\n" +
				"  * Download the most recent cards database from Mtgjson.com\r\n" +
				"  * Download card images\r\n" +
				"  * Download artworks",
				_buttonDownload);

			TooltipController.SetTooltip(this,
				"Advanced settings",
				"Opens configuration file.\r\n" +
				"Use it to tell the program where to find your custom card images or tweak some other settings.\r\n" +
				"\r\n" +
				"Configuration file is opened by whatever application you have associated with *.xml files. " +
				"If it's Internet Explorer, you need to assign *.xml extension to a text editor instead, " +
				"because Internet Explorer only displays the file without allowing to edit." +
				"I recommend using an editor with XML syntax highlighting e.g. Notepad++.\r\n" +
				"\r\n" +
				"To apply your changes save the modified configuration file and restart the program.",
				_buttonConfig
			);

			TooltipController.SetTooltip(this,
				"Open new window",
				"You can drag-n-drop Cards and Tabs between windows.",
				_buttonOpenWindow);

			TooltipController.SetTooltip(this,
				"Color scheme",
				"Select Mtgdb.Gui color scheme.\r\n\r\n" +
				"NOTE: you will see some remnants of previous colors until you restart Mtgdb.Gui",
				_buttonColorScheme);

			TooltipController.SetTooltip(this,
				null,
				"Clipboard operations menu",
				_buttonPaste);

			TooltipController.SetTooltip(this,
				null,
				"Open file menu",
				_buttonOpenDeck);

			TooltipController.SetTooltip(this,
				null,
				"Save to file menu",
				_buttonSaveDeck);

			var tabHeadersTooltip = new TabHeadersTooltip(_tabs, this);
			tabHeadersTooltip.SubscribeToEvents();

			TooltipController.SetCustomTooltip(tabHeadersTooltip);

			Load += loadTooltips;
			Closing += closeTooltips;
		}

		private void loadTooltips(object sender, EventArgs e)
		{
			TooltipController.SubscribeToEvents();
			TooltipController.StartThread();
		}

		private void closeTooltips(object sender, CancelEventArgs e)
		{
			TooltipController.AbortThread();
		}
	}
}