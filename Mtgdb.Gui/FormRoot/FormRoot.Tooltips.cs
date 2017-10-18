using System;
using System.ComponentModel;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupTooltips()
		{
			_tooltipController.SetTooltip("Undo: Ctrl+Z, Alt+Left",
				"Click to undo your last action",
				_buttonUndo);

			_tooltipController.SetTooltip("Redo: Ctrl+Y, Alt+Right",
				"Click to repeat the action cancelled with undo",
				_buttonRedo);

			_tooltipController.SetTooltip("Tabbed Document Interface (TDI)",
				"Add tab: Ctrl+T, click '+' button\r\n" +
				"Remove tab: Ctrl+F4, click 'x' button, Middle mouse click\r\n" +
				"Select next tab: Ctrl+Tab\r\n" +
				"Use drag-n-drop to reorder tabs.\r\n" +
				"Drag the card here to select or create another tab\r\n" +
				"where you want to drop the card.",
				_tabs);

			_tooltipController.SetTooltip("Deck statistics",
				"Opens a Pivot report window. Use it to view \r\n" +
				"mana curve, price breakdown, or create \r\n" +
				"a custom report by moving field captions between\r\n" +
				"Row, Column and Summary areas of grid.",
				_buttonStat);

			_tooltipController.SetTooltip("Print deck: Ctrl+P",
				"The print buttons doesn't actually print, instead\r\n" +
				"it creates images of cards by groups of 8\r\n" +
				"that can be printed on A4 paper.",
				_buttonPrint);

			_tooltipController.SetTooltip("Clear deck",
				"Use it to start creating a new deck from scratch",
				_buttonClear);

			_tooltipController.SetTooltip("Enable / disable tooltips",
				"Tooltips are helpful but also annoying.\r\n" +
				"Uncheck this button to disable tooltips.",
				_buttonTooltips);

			_tooltipController.SetTooltip("Update",
				"Shows a window where you can\r\n" +
				"  * Check for a new version of Mtgdb.Gui\r\n" +
				"  * Download the most recent cards database from Mtgjson.com\r\n" +
				"  * Download card images\r\n" +
				"  * Download artworks",
				_buttonDownload);

			_tooltipController.SetTooltip(string.Empty,
				"Edit General settings to tell the program where to find\r\n" +
				"your custom card images or tweak some other settings.\r\n\r\n" +
				"NOTE: To apply your changes save the modified configuration\r\n" +
				"file and restart the program.",
				_buttonMenuGeneralSettings);

			_tooltipController.SetTooltip(string.Empty,
				"Edit Display settings to\r\n" +
				"  * change image size\r\n" +
				"  * hide texutal card details\r\n" +
				"  * disallow showing non-entirely-fit-to-screen card image\r\n\r\n" +
				"NOTE: To apply your changes save the modified configuration\r\n" +
				"file and restart the program.",
				_buttonMenuDisplaySettings);

			Load += loadTooltips;
			Closing += closeTooltips;
		}

		private void loadTooltips(object sender, EventArgs e)
		{
			_tooltipController.SubscribeToEvents();
			_tooltipController.StartThread();
		}

		private void closeTooltips(object sender, CancelEventArgs e)
		{
			_tooltipController.AbortThread();
		}
	}
}
