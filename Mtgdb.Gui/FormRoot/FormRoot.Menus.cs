using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using ButtonBase = Mtgdb.Controls.ButtonBase;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupButtonClicks()
		{
			_buttonTooltips.Checked = true;
			_buttonTooltips.CheckedChanged += tooltipsChecked;

			_buttonUpdate.Enabled = false;
			_buttonUpdate.Pressed += updateClick;

			_buttonMenuOpenDeck.Pressed += openDeckClick;
			_buttonMenuSaveDeck.Pressed += saveDeckClick;

			_buttonMenuOpenCollection.Pressed += openCollectionClick;
			_buttonMenuSaveCollection.Pressed += saveCollectionClick;

			_buttonStat.Pressed += statClick;
			_buttonPrint.Pressed += printClick;
			_buttonClear.Pressed += clearClick;
			_buttonUndo.Pressed += undoClick;
			_buttonRedo.Pressed += redoClick;

			_buttonMenuEditConfig.Pressed += configClick;

			_buttonOpenWindow.Pressed += openWindowClick;
			_buttonMenuPasteDeck.Pressed += pasteClick;
			_buttonMenuPasteDeckAppend.Pressed += pasteClick;
			_buttonMenuPasteCollection.Pressed += pasteClick;
			_buttonMenuPasteCollectionAppend.Pressed += pasteClick;
			_buttonMenuCopyCollection.Pressed += pasteClick;
			_buttonMenuCopyDeck.Pressed += pasteClick;
			_buttonImportExportToMtgArena.Pressed += buttonImportExportToMtgArenaClick;

			_menuColors.Items[0].Click += buttonColorSchemeClick;

			_buttonImportMtgArenaCollection.Pressed += buttonImportMtgArenaCollectionClick;
			_buttonRestoreCollection.Pressed += buttonRestoreCollectionClick;
		}

		private void buttonColorSchemeClick(object sender, EventArgs e)
		{
			if (!_colorSchemeEditor.Visible)
				_colorSchemeEditor.Show();

			if (!_colorSchemeEditor.Focused)
				_colorSchemeEditor.Focus();
		}

		private void pasteClick(object sender, EventArgs e)
		{
			var form = SelectedTab;
			if (form == null)
				return;

			if (sender == _dropdownPaste || sender == _buttonMenuPasteDeck)
				form.PasteDeck(append: false);
			else if (sender == _buttonMenuPasteDeckAppend)
				form.PasteDeck(append: true);
			else if (sender == _buttonMenuPasteCollection)
				form.PasteCollection(append: false);
			else if (sender == _buttonMenuPasteCollectionAppend)
				form.PasteCollection(append: true);
			else if (sender == _buttonMenuCopyDeck)
				form.CopyDeck();
			else if (sender == _buttonMenuCopyCollection)
				form.CopyCollection();
		}

		private void buttonImportExportToMtgArenaClick(object sender, EventArgs e)
		{
			var form = SelectedTab;
			if (form == null)
				return;

			if (SaveLoadMenuMode.IsMtgArenaPaste)
				form.PasteDeck(append: false);
			else
				form.CopyDeckInMtgArenaFormat();
		}

		private void buttonRestoreCollectionClick(object sender, EventArgs e) =>
			SelectedTab?.RestoreCollectionBeforeMtgArenaImport();

		private void buttonImportMtgArenaCollectionClick(object sender, EventArgs e) =>
			SelectedTab?.ImportMtgArenaCollection();

		private void openWindowClick(object sender, EventArgs e) =>
			_app.StartForm();

		private static void configClick(object sender, EventArgs e) =>
			System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.xml"));

		private void tooltipsChecked(object sender, EventArgs e)
		{
			HideTooltips = !((ButtonBase) sender).Checked;

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain) _tabs.TabIds[i];
				formMain?.ButtonTooltip();
			}
		}

		private void redoClick(object sender, EventArgs e)
		{
			if (_undoingOrRedoing)
				return;

			_undoingOrRedoing = true;
			SelectedTab?.ButtonRedo();
			_undoingOrRedoing = false;
		}

		private void undoClick(object sender, EventArgs e)
		{
			if (_undoingOrRedoing)
				return;

			_undoingOrRedoing = true;
			SelectedTab?.ButtonUndo();
			_undoingOrRedoing = false;
		}

		private void clearClick(object sender, EventArgs e) =>
			SelectedTab?.ButtonClearDeck();

		private void printClick(object sender, EventArgs e) =>
			SelectedTab?.ButtonPrint();

		private void statClick(object sender, EventArgs e) =>
			SelectedTab?.ButtonPivot();

		private void saveDeckClick(object sender, EventArgs e) =>
			SelectedTab?.ButtonSaveDeck();

		private void openDeckClick(object sender, EventArgs e) =>
			SelectedTab?.ButtonLoadDeck();

		private void saveCollectionClick(object sender, EventArgs e) =>
			SelectedTab?.ButtonSaveCollection();

		private void openCollectionClick(object sender, EventArgs e) =>
			SelectedTab?.ButtonLoadCollection();

		private void updateClick(object sender, EventArgs e) =>
			_downloaderSubsystem.ShowDownloader(this, auto: false);



		private void setupLanguageMenu()
		{
			_dropdownLanguage.AutoCheck = false;

			updateButtonLanguage();
			UiModel.LanguageController.LanguageChanged += updateButtonLanguage;

			foreach (var langMenuItem in getLanguageMenuItems())
				langMenuItem.MouseClick += languageMenuClick;
		}

		private void languageMenuClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				var button = (ButtonBase) sender;
				UiModel.LanguageController.Language = button.Text.ToLower(Str.Culture);
			}
		}

		private void updateButtonLanguage()
		{
			var language = UiModel.LanguageController.Language;

			var menuItem = getLanguageMenuItems()
				.Single(_ => Str.Equals(_.Text, language));

			_dropdownLanguage.Text = menuItem.Text;
			_dropdownLanguage.Image = menuItem.Image;
		}

		private IEnumerable<ButtonBase> getLanguageMenuItems() =>
			_menuLanguage.Controls.OfType<ButtonBase>();



		private void setupExternalLinks()
		{
			_buttonVisitForge.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=26");
			_buttonVisitForge.Pressed += buttonVisitClick;

			_buttonVisitXMage.SetTag(@"http://www.xmage.de");
			_buttonVisitXMage.Pressed += buttonVisitClick;

			_buttonVisitMagarena.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=82");
			_buttonVisitMagarena.Pressed += buttonVisitClick;

			_buttonVisitCockatrice.SetTag(@"https://cockatrice.github.io/");
			_buttonVisitCockatrice.Pressed += buttonVisitClick;

			_buttonVisitDotP2014.SetTag(@"https://www.slightlymagic.net/forum/viewtopic.php?f=99&t=10999&start=270#p213467");
			_buttonVisitDotP2014.Pressed += buttonVisitClick;

			_buttonMenuDonatePayPal.SetTag(@"http://paypal.me/nidalgo");
			_buttonMenuDonatePayPal.Pressed += buttonVisitClick;

			_buttonMenuDonateYandexMoney.SetTag(@"https://money.yandex.ru/to/410012387625926?_openstat=template%3Bipulldown%3Btopupme");
			_buttonMenuDonateYandexMoney.Pressed += buttonVisitClick;

			_buttonVisitMtgo.SetTag(AppDir.Root.AddPath("https://github.com/NikolayXHD/Mtgdb/wiki/2.2-Import-collection-&-decks-from-Magic-The-Gathering-Online"));
			_buttonVisitMtgo.Pressed += buttonVisitClick;

			_buttonSupport.SetTag(_appSourceConfig.ForumUrl);
			_buttonSupport.Pressed += buttonVisitClick;

			_buttonVisitMtgArena.SetTag(@"https://magic.wizards.com/en/mtgarena");
			_buttonVisitMtgArena.Pressed += buttonVisitClick;

			_buttonVisitDeckedBuilder.SetTag(@"http://www.deckedbuilder.com/");
			_buttonVisitDeckedBuilder.Pressed += buttonVisitClick;

			_buttonHelp.SetTag("https://github.com/NikolayXHD/Mtgdb/wiki");
			_buttonHelp.Pressed += buttonVisitClick;
		}

		private static void buttonVisitClick(object sender, EventArgs e)
		{
			var control = (Control) sender;
			var url = control.GetTag<string>();
			System.Diagnostics.Process.Start(url);
		}

		private void setMenuMode(ButtonBase sender)
		{
			_menuOpen.SuspendLayout();

			foreach (var state in _saveLoadMenuModes)
			{
				state.IsCurrent = state.TitleButton == sender;

				if (state.IsCurrent)
					_buttonImportExportToMtgArena.Text = state.MtgArenaButtonText;

				foreach (var menuButton in state.MenuButtons)
					menuButton.Visible = state.IsCurrent;
			}

			_buttonRestoreCollection.Enabled = _uiConfigRepository.Config.CollectionBeforeImportMtga != null;

			_menuOpen.ResumeLayout(false);
			_menuOpen.PerformLayout();
		}

		private void updateMenuColors()
		{
			for (int i = _menuColors.Items.Count - 1; i > 0; i--)
			{
				_menuColors.Items[i].Click -= menuColorsClick;
				_menuColors.Items.RemoveAt(i);
			}

			const int maxSchemesCount = 16;
			foreach (var schemeName in _colorSchemeEditor.GetSavedSchemeNames().Take(maxSchemesCount))
			{
				var item = new ToolStripMenuItem(schemeName);
				item.Click += menuColorsClick;
				_menuColors.Items.Add(item);
			}
		}

		private void menuColorsClick(object s, EventArgs e) =>
			_colorSchemeEditor.LoadSavedScheme(((ToolStripMenuItem) s).Text);



		private static void previewKeyDown(object sender, PreviewKeyDownEventArgs e) =>
			e.IsInputKey = true;

		private void formKeyDown(object sender, KeyEventArgs e)
		{
			var form = SelectedTab;
			if (form == null)
				return;

			bool handled = true;

			if (e.KeyData == (Keys.Control | Keys.F4))
				CloseTab();
			else if (e.KeyData == (Keys.Control | Keys.Tab))
				SelectNextTab();
			else if (e.KeyData == (Keys.Control | Keys.Shift | Keys.Tab))
				SelectPreviousTab();
			else if (e.KeyData == (Keys.Control | Keys.T))
				AddTab();
			else if (e.KeyData == (Keys.Alt | Keys.Left) || e.KeyData == (Keys.Control | Keys.Z))
				form.ButtonUndo();
			else if (e.KeyData == (Keys.Alt | Keys.Right) || e.KeyData == (Keys.Control | Keys.Y))
				form.ButtonRedo();
			else if (e.KeyData == (Keys.Control | Keys.F))
				form.FocusSearch();
			else if (e.KeyData == Keys.Escape)
			{
				if (form.IsDraggingCard)
					form.StopDragging();
				else
					handled = false;
			}
			else if (e.KeyData == (Keys.Control | Keys.S))
				form.ButtonSaveDeck();
			else if (e.KeyData == (Keys.Control | Keys.O))
				form.ButtonLoadDeck();
			else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.S))
				form.ButtonSaveCollection();
			else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.O))
				form.ButtonLoadCollection();
			else if (e.KeyData == (Keys.Control | Keys.P))
				form.ButtonPrint();
			else if (e.KeyData == (Keys.Control | Keys.Shift | Keys.V))
				form.PasteDeck(append: true);
			else if (e.KeyData == (Keys.Control | Keys.V) || e.KeyData == (Keys.Shift | Keys.Insert))
			{
				if (form.IsTextInputFocused())
					handled = false;
				else
					form.PasteDeck(append: false);
			}
			else if (e.KeyData == (Keys.Alt | Keys.Shift | Keys.V))
				form.PasteCollection(append: true);
			else if (e.KeyData == (Keys.Alt | Keys.V))
				form.PasteCollection(append: false);
			else if (e.KeyData == (Keys.Control | Keys.C))
			{
				if (!form.IsTextInputFocused())
					form.CopyDeck();

				handled = false;
			}
			else if (e.KeyData == (Keys.Alt | Keys.C))
			{
				if (form.IsTextInputFocused())
					handled = false;
				else
					form.CopyCollection();
			}
			else if (e.KeyData == Keys.F1)
				form.ShowFindExamples();
			else
				handled = false;

			e.Handled = handled;
			e.SuppressKeyPress = handled;
		}

		private readonly ButtonBase[] _deckButtons;

		private readonly List<SaveLoadMenuMode> _saveLoadMenuModes;

		private SaveLoadMenuMode SaveLoadMenuMode =>
			_saveLoadMenuModes.First(_ => _.IsCurrent);
	}
}
