﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupButtonClicks()
		{
			_buttonTooltips.Checked = true;
			_buttonShowFilterPanels.Checked = true;
			_buttonUpdate.Enabled = false;

			_buttonUpdate.Click += updateClick;

			_buttonMenuOpenDeck.Click += openDeckClick;
			_buttonMenuSaveDeck.Click += saveDeckClick;

			_buttonMenuOpenCollection.Click += openCollectionClick;
			_buttonMenuSaveCollection.Click += saveCollectionClick;

			_buttonStat.Click += statClick;
			_buttonPrint.Click += printClick;
			_buttonClear.Click += clearClick;
			_buttonUndo.Click += undoClick;
			_buttonRedo.Click += redoClick;

			_buttonHelp.Click += helpClick;

			_buttonMenuEditConfig.Click += configClick;
			_buttonTooltips.CheckedChanged += tooltipsChecked;
			_buttonShowFilterPanels.CheckedChanged += filterPanelsChecked;

			_buttonOpenWindow.Click += openWindowClick;
			_buttonMenuPasteDeck.Click += pasteClick;
			_buttonMenuPasteDeckAppend.Click += pasteClick;
			_buttonMenuPasteCollection.Click += pasteClick;
			_buttonMenuPasteCollectionAppend.Click += pasteClick;
			_buttonMenuCopyCollection.Click += pasteClick;
			_buttonMenuCopyDeck.Click += pasteClick;
			_buttonImportExportToMtgArena.Click += buttonImportExportToMtgArenaClick;

			_menuColors.Items[0].Click += buttonColorSchemeClick;

			_buttonImportMtgArenaCollection.Click += buttonImportMtgArenaCollectionClick;
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

			if (sender == _buttonPaste || sender == _buttonMenuPasteDeck)
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

		private void buttonImportMtgArenaCollectionClick(object sender, EventArgs e) =>
			SelectedTab?.ImportMtgArenaCollection();

		private void openWindowClick(object sender, EventArgs e) =>
			_application.CreateForm();

		private static void configClick(object sender, EventArgs e) =>
			System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.xml"));

		private void filterPanelsChecked(object sender, EventArgs e) =>
			ShowFilterPanelsChanged?.Invoke();

		private void tooltipsChecked(object sender, EventArgs e)
		{
			HideTooltips = !((CustomCheckBox) sender).Checked;

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain) _tabs.TabIds[i];
				formMain?.ButtonTooltip();
			}
		}

		private static void helpClick(object sender, EventArgs e) =>
			System.Diagnostics.Process.Start(AppDir.Root.AddPath("help\\home.html"));

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
			_buttonLanguage.AutoCheck = false;

			updateLanguage();
			UiModel.LanguageController.LanguageChanged += updateLanguage;

			foreach (var langMenuItem in getLanguageMenuItems())
				langMenuItem.Click += languageMenuClick;
		}

		private void languageMenuClick(object sender, EventArgs e)
		{
			var button = (CustomCheckBox) sender;
			UiModel.LanguageController.Language = button.Text.ToLower(Str.Culture);
		}

		private void updateLanguage()
		{
			var language = UiModel.LanguageController.Language;

			var menuItem = getLanguageMenuItems()
				.Single(_ => Str.Equals(_.Text, language));

			_buttonLanguage.Image = menuItem.Image;
			_buttonLanguage.Text = language.ToUpperInvariant();
			_buttonSubsystem.SetupButton(_buttonLanguage, ButtonImages.ScaleDpi((null, _languageIcons[language])));
		}

		private IEnumerable<CustomCheckBox> getLanguageMenuItems() =>
			_menuLanguage.Controls.OfType<CustomCheckBox>();



		private void setupExternalLinks()
		{
			_buttonVisitForge.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=26");
			_buttonVisitForge.Click += buttonVisitClick;

			_buttonVisitXMage.SetTag(@"http://www.xmage.de");
			_buttonVisitXMage.Click += buttonVisitClick;

			_buttonVisitMagarena.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=82");
			_buttonVisitMagarena.Click += buttonVisitClick;

			_buttonVisitCockatrice.SetTag(@"https://cockatrice.github.io/");
			_buttonVisitCockatrice.Click += buttonVisitClick;

			_buttonVisitDotP2014.SetTag(@"https://www.slightlymagic.net/forum/viewtopic.php?f=99&t=10999&start=270#p213467");
			_buttonVisitDotP2014.Click += buttonVisitClick;

			_buttonMenuDonatePayPal.SetTag(@"http://paypal.me/nidalgo");
			_buttonMenuDonatePayPal.Click += buttonVisitClick;

			_buttonMenuDonateYandexMoney.SetTag(@"https://money.yandex.ru/to/410012387625926?_openstat=template%3Bipulldown%3Btopupme");
			_buttonMenuDonateYandexMoney.Click += buttonVisitClick;

			_buttonVisitMtgo.SetTag(AppDir.Root.AddPath("help\\html\\Import_collection_&_decks_from_Magic_The_Gathering_Online.html"));
			_buttonVisitMtgo.Click += buttonVisitClick;

			_buttonSupport.SetTag(_appSourceConfig.ForumUrl);
			_buttonSupport.Click += buttonVisitClick;

			_buttonVisitMtgArena.SetTag(@"https://magic.wizards.com/en/mtgarena");
			_buttonVisitMtgArena.Click += buttonVisitClick;

			_buttonVisitDeckedBuilder.SetTag(@"http://www.deckedbuilder.com/");
			_buttonVisitDeckedBuilder.Click += buttonVisitClick;
		}

		private static void buttonVisitClick(object sender, EventArgs e)
		{
			var control = (Control) sender;
			var url = control.GetTag<string>();
			System.Diagnostics.Process.Start(url);
		}

		private void setMenuMode(CustomCheckBox sender)
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

			_menuOpen.ResumeLayout(false);
			_menuOpen.PerformLayout();
		}

		private void setupButtons()
		{
			_buttonSubsystem.SetupPopup(new Popup(_menuLanguage, _buttonLanguage));
			_buttonSubsystem.SetupPopup(new Popup(_menuDonate, _buttonDonate, HorizontalAlignment.Center));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen, _buttonOpenDeck,
				beforeShow: () => setMenuMode(_buttonOpenDeck)));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen, _buttonSaveDeck,
				beforeShow: () => setMenuMode(_buttonSaveDeck)));

			_buttonSubsystem.SetupPopup(new Popup(_menuPaste, _buttonPaste));
			_buttonSubsystem.SetupPopup(new Popup(_menuColors, _buttonColorScheme,
				beforeShow: updateMenuColors));

			_buttonSubsystem.SetupPopup(new Popup(_menuConfig, _buttonConfig));

			_buttonSubsystem.SubscribeToEvents();

			ManualMenuPainter.SetupComboBox(_menuUiScale, allowScroll: false);
			ManualMenuPainter.SetupComboBox(_menuUiSmallImageQuality, allowScroll: false);
			ManualMenuPainter.SetupComboBox(_menuUiSuggestDownloadMissingImages, allowScroll: false);
			ManualMenuPainter.SetupComboBox(_menuUiImagesCacheCapacity, allowScroll: false);
			ManualMenuPainter.SetupComboBox(_menuUiUndoDepth, allowScroll: false);
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

		private void unsubscribeButtonEvents() =>
			_buttonSubsystem.UnsubscribeFromEvents();

		private readonly CustomCheckBox[] _deckButtons;
		private readonly ButtonSubsystem _buttonSubsystem;
		private readonly Dictionary<string, Bitmap> _languageIcons;

		private readonly List<SaveLoadMenuMode> _saveLoadMenuModes;

		private SaveLoadMenuMode SaveLoadMenuMode =>
			_saveLoadMenuModes.First(_ => _.IsCurrent);

		public event Action ShowFilterPanelsChanged;
	}
}