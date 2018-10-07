using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupButtonClicks()
		{
			_buttonTooltips.Checked = true;
			_buttonShowFilterPanels.Checked = true;
			_buttonDownload.Enabled = false;

			foreach (var state in _saveLoadMenuModes)
				state.TitleButton.MouseEnter += saveLoadMouseEnter;

			_buttonDownload.Click += downloadClick;

			_buttonOpenDeck.Click += openDeckClick;
			_buttonMenuOpenDeck.Click += openDeckClick;
			_buttonSaveDeck.Click += saveDeckClick;
			_buttonMenuSaveDeck.Click += saveDeckClick;

			_buttonMenuOpenCollection.Click += openCollectionClick;
			_buttonMenuSaveCollection.Click += saveCollectionClick;

			_buttonStat.Click += statClick;
			_buttonPrint.Click += printClick;
			_buttonClear.Click += clearClick;
			_buttonUndo.Click += undoClick;
			_buttonRedo.Click += redoClick;

			_buttonHelp.Click += helpClick;

			_buttonConfig.Click += configClick;
			_buttonTooltips.CheckedChanged += tooltipsChecked;
			_buttonShowFilterPanels.CheckedChanged += filterPanelsChecked;

			_buttonPaste.Click += pasteClick;
			_buttonOpenWindow.Click += openWindowClick;
			_buttonMenuPasteDeck.Click += pasteClick;
			_buttonMenuPasteDeckAppend.Click += pasteClick;
			_buttonMenuPasteCollection.Click += pasteClick;
			_buttonMenuPasteCollectionAppend.Click += pasteClick;
			_buttonMenuCopyCollection.Click += pasteClick;
			_buttonMenuCopyDeck.Click += pasteClick;
			_buttonImportExportToMtgArena.Click += buttonImportExportToMtgArenaClick;
			_buttonColorScheme.Click += buttonColorSchemeClick;
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

		private void openWindowClick(object sender, EventArgs e)
		{
			_application.CreateForm();
		}

		private static void configClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.xml"));
		}

		private void filterPanelsChecked(object sender, EventArgs e)
		{
			ShowFilterPanelsChanged?.Invoke();
		}

		private void tooltipsChecked(object sender, EventArgs e)
		{
			HideTooltips = !((CheckBox) sender).Checked;

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain) _tabs.TabIds[i];
				formMain?.ButtonTooltip();
			}
		}

		private static void helpClick(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(AppDir.Root.AddPath("help\\home.html"));
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

		private void clearClick(object sender, EventArgs e)
		{
			SelectedTab?.ButtonClearDeck();
		}

		private void printClick(object sender, EventArgs e)
		{
			SelectedTab?.ButtonPrint();
		}

		private void statClick(object sender, EventArgs e)
		{
			SelectedTab?.ButtonPivot();
		}

		private void saveDeckClick(object sender, EventArgs e)
		{
			SelectedTab?.ButtonSaveDeck();
		}

		private void openDeckClick(object sender, EventArgs e)
		{
			SelectedTab?.ButtonLoadDeck();
		}

		private void saveCollectionClick(object sender, EventArgs e)
		{
			SelectedTab?.ButtonSaveCollection();
		}

		private void openCollectionClick(object sender, EventArgs e)
		{
			SelectedTab?.ButtonLoadCollection();
		}

		private void downloadClick(object sender, EventArgs e)
		{
			_downloaderSubsystem.ShowDownloader(this, auto: false);
		}

		private void setupLanguageMenu()
		{
			setup(_buttonLanguage);

			updateLanguage();
			UiModel.LanguageController.LanguageChanged += updateLanguage;

			foreach (var langMenuItem in getLanguageMenuItems())
			{
				setup(langMenuItem);
				langMenuItem.Click += languageMenuClick;
			}

			void setup(CustomCheckBox button)
			{
				button.AutoCheck = false;
				button.BackColor = SystemColors.Window;
				button.HighlightBackColor = SystemColors.Control;

				button.HighlightMouseOverOpacity = 64;
				button.HighlightMouseDownOpacity = 128;
			}
		}

		private void languageMenuClick(object sender, EventArgs e)
		{
			var button = (ButtonBase) sender;
			UiModel.LanguageController.Language = button.Text.ToLower(Str.Culture).Trim();
		}

		private void updateLanguage()
		{
			var language = UiModel.LanguageController.Language;

			var menuItem = getLanguageMenuItems()
				.Single(_ => Str.Equals(_.Text.Trim(), language));

			_buttonLanguage.Image = menuItem.Image;
			_buttonLanguage.Text = language.ToUpperInvariant();
			_buttonSubsystem.SetupButton(_buttonLanguage, new ButtonImages(_languageIcons[language], true));
		}

		private IEnumerable<CustomCheckBox> getLanguageMenuItems() =>
			_layoutLanguage.Controls.OfType<CustomCheckBox>();



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

			_buttonDonatePayPal.SetTag(@"http://paypal.me/nidalgo");
			_buttonDonatePayPal.Click += buttonVisitClick;

			_buttonDonateYandexMoney.SetTag(@"https://money.yandex.ru/to/410012387625926?_openstat=template%3Bipulldown%3Btopupme");
			_buttonDonateYandexMoney.Click += buttonVisitClick;

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



		private void saveLoadMouseEnter(object sender, EventArgs e)
		{
			_layoutOpen.SuspendLayout();

			foreach (var state in _saveLoadMenuModes)
			{
				state.IsCurrent = state.TitleButton == sender;

				if (state.IsCurrent)
					_buttonImportExportToMtgArena.Text = state.MtgArenaButtonText;

				foreach (var menuButton in state.MenuButtons)
					menuButton.Visible = state.IsCurrent;
			}

			_layoutOpen.ResumeLayout(false);
			_layoutOpen.PerformLayout();
		}

		private void setupButtons()
		{
			setupButton(_buttonUndo,
				Resources.undo_16,
				Resources.undo_32);

			setupButton(_buttonRedo,
				Resources.redo_16,
				Resources.redo_32);

			setupButton(_buttonSaveDeck,
				Resources.save_16,
				Resources.save_32);

			setupButton(_buttonOpenDeck,
				Resources.open_16,
				Resources.open_32);

			setupButton(_buttonStat,
				Resources.chart_16,
				Resources.chart_32);

			setupButton(_buttonPrint,
				Resources.print_16,
				Resources.print_32);

			setupButton(_buttonClear,
				Resources.trash_16,
				Resources.trash_32);

			setupButton(_buttonPaste,
				Resources.paste_16,
				Resources.paste_32);

			setupButton(_buttonHelp,
				Resources.index_16,
				Resources.index_32);

			setupButton(_buttonConfig,
				Resources.properties_16,
				Resources.properties_32);

			setupButton(_buttonTooltips,
				Resources.tooltip_16,
				Resources.tooltip_32);

			_buttonSubsystem.SetupButton(_buttonShowFilterPanels,
				new ButtonImages(Resources.filters_show_32, x2: true));

			_buttonSubsystem.SetupButton(_buttonDownload,
				new ButtonImages(Resources.update_40, x2: true));

			_buttonSubsystem.SetupButton(_buttonMenuOpenDeck,
				new ButtonImages(Resources.deck_48, x2: true));

			_buttonSubsystem.SetupButton(_buttonMenuOpenCollection,
				new ButtonImages(Resources.box_48, x2: true));

			_buttonSubsystem.SetupButton(_buttonMenuSaveDeck,
				new ButtonImages(Resources.deck_48, x2: true));

			_buttonSubsystem.SetupButton(_buttonMenuSaveCollection,
				new ButtonImages(Resources.box_48, x2: true));

			_buttonSubsystem.SetupButton(_buttonOpenWindow,
				new ButtonImages(Resources.add_form_32, x2: true));

			_buttonSubsystem.SetupButton(_buttonLanguage,
				new ButtonImages(Resources.en, x2: true));

			_buttonSubsystem.SetupButton(_buttonColorScheme,
				new ButtonImages(Resources.color_swatch_32, x2: true));

			foreach (var langButton in getLanguageMenuItems())
				_buttonSubsystem.SetupButton(langButton,
					new ButtonImages(_languageIcons[langButton.Text.Trim()], x2: true));

			_buttonSubsystem.SetupButton(_buttonDonate, new ButtonImages(null, x2: false));
			_buttonSubsystem.SetupButton(_buttonSupport, new ButtonImages(null, x2: false));
			_buttonSubsystem.SetupButton(_buttonDonateYandexMoney, new ButtonImages(Resources.yandex_money_32, x2: false));
			_buttonSubsystem.SetupButton(_buttonDonatePayPal, new ButtonImages(Resources.paypal_32, x2: false));

			_buttonSubsystem.SetupButton(_buttonImportExportToMtgArena, new ButtonImages(Resources.paste_32, x2: false));

			_buttonSubsystem.SetupPopup(new Popup(_menuLanguage,
				_buttonLanguage,
				container: _layoutLanguage,
				openOnHover: false,
				closeMenuOnClick: true));

			_buttonSubsystem.SetupPopup(new Popup(_menuDonate,
				_buttonDonate,
				container: _layoutDonate,
				alignment: HorizontalAlignment.Center,
				openOnHover: false,
				closeMenuOnClick: true,
				borderOnHover: false));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen,
				_buttonOpenDeck,
				container: _layoutOpen,
				borderOnHover: false,
				closeMenuOnClick: true));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen,
				_buttonSaveDeck,
				container: _layoutOpen,
				borderOnHover: false,
				closeMenuOnClick: true));

			_buttonSubsystem.SetupPopup(new Popup(_menuPaste,
				_buttonPaste,
				container: _layoutPaste,
				borderOnHover: false,
				closeMenuOnClick: true));

			_buttonSubsystem.SubscribeToEvents();
		}

		private void setupButton(CustomCheckBox button, Bitmap image, Bitmap imageDouble)
		{
			bool x2 = Dpi.ScalePercent > 100;
			var bmp = x2 ? imageDouble : image;
			_buttonSubsystem.SetupButton(button, new ButtonImages(bmp, x2));
		}

		private static void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			e.IsInputKey = true;
		}

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

		private void unsubscribeButtonEvents()
		{
			_buttonSubsystem.UnsubscribeFromEvents();
		}

		private readonly ButtonBase[] _deckButtons;
		private readonly ButtonSubsystem _buttonSubsystem;
		private readonly Dictionary<string, Bitmap> _languageIcons;

		private readonly List<SaveLoadMenuMode> _saveLoadMenuModes;

		private SaveLoadMenuMode SaveLoadMenuMode =>
			_saveLoadMenuModes.First(_ => _.IsCurrent);
	}
}