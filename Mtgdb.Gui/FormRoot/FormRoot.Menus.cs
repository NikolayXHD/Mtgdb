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
			_buttonFilterPanels.Checked = true;
			_buttonDownload.Enabled = false;

			foreach (var button in _saveLoadButtons)
				button.MouseEnter += saveLoadMouseEnter;

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
			_buttonFilterPanels.CheckedChanged += filterPanelsChecked;
			
			_buttonPaste.Click += pasteClick;
			_buttonOpenWindow.Click += openWindowClick;
			_buttonMenuPasteDeck.Click += pasteClick;
			_buttonMenuPasteDeckAppend.Click += pasteClick;
			_buttonMenuPasteCollection.Click += pasteClick;
			_buttonMenuPasteCollectionAppend.Click += pasteClick;
			_buttonMenuCopyCollection.Click += pasteClick;
			_buttonMenuCopyDeck.Click += pasteClick;
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

		private void openWindowClick(object sender, EventArgs e)
		{
			_formManager.CreateForm();
		}

		private void configClick(object sender, EventArgs e)
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

		private void helpClick(object sender, EventArgs e)
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
			updateLanguage();
			UiModel.LanguageController.LanguageChanged += updateLanguage;

			foreach (var langMenuItem in getLanguageMenuItems())
				langMenuItem.Click += languageMenuClick;
		}

		private void languageMenuClick(object sender, EventArgs e)
		{
			var button = (ButtonBase) sender;
			UiModel.LanguageController.Language = button.Text.ToLowerInvariant().Trim();
		}

		private void updateLanguage()
		{
			var language = UiModel.LanguageController.Language;

			var menuItem = getLanguageMenuItems()
				.Single(_ => Str.Equals(_.Text.Trim(), language));

			_buttonLanguage.Image = menuItem.Image;
			_buttonLanguage.Text = language.ToUpperInvariant();
			setupButton(_buttonLanguage, _languageIcons[language], true);
		}

		private IEnumerable<ButtonBase> getLanguageMenuItems()
		{
			return _layoutLanguage.Controls.OfType<ButtonBase>();
		}



		private void setupExternalLinks()
		{
			_buttonVisitForge.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=26");
			_buttonVisitForge.Click += buttonVisitClick;

			_buttonVisitXMage.SetTag(@"http://www.xmage.de");
			_buttonVisitXMage.LinkClicked += buttonVisitClick;

			_buttonVisitMagarena.SetTag(@"https://www.slightlymagic.net/forum/viewforum.php?f=82");
			_buttonVisitMagarena.LinkClicked += buttonVisitClick;

			_buttonVisitCockatrice.SetTag(@"https://cockatrice.github.io/");
			_buttonVisitCockatrice.LinkClicked += buttonVisitClick;

			_buttonVisitDotP2014.SetTag(@"https://www.slightlymagic.net/forum/viewtopic.php?f=99&t=10999&start=270#p213467");
			_buttonVisitDotP2014.LinkClicked += buttonVisitClick;

			_buttonDonatePayPal.SetTag(@"http://paypal.me/nidalgo");
			_buttonDonatePayPal.Click += buttonVisitClick;

			_buttonDonateYandexMoney.SetTag(@"https://money.yandex.ru/to/410012387625926?_openstat=template%3Bipulldown%3Btopupme");
			_buttonDonateYandexMoney.Click += buttonVisitClick;

			_buttonVisitMtgo.SetTag(AppDir.Root.AddPath("help\\html\\Import_collection_&_decks_from_Magic_The_Gathering_Online.html"));
			_buttonVisitMtgo.Click += buttonVisitClick;
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
			for (int i = 0; i < _saveLoadButtons.Length; i++)
			{
				bool visible = sender == _saveLoadButtons[i];
				var menuButtons = _saveLoadMenuButtons[i];

				for (int j = 0; j < menuButtons.Length; j++)
					menuButtons[j].Visible = visible;
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

			_buttonSubsystem.SetupButton(_buttonFilterPanels,
				new ButtonImages(
					Resources.filters_hide_32,
					Resources.filters_show_32,
					Resources.filters_hide_hovered_32,
					Resources.filters_show_hovered_32,
					areImagesDoubleSized: true));

			setupButton(_buttonDownload, Resources.update_40, true);
			setupButton(_buttonMenuOpenDeck, Resources.deck_48, true);
			setupButton(_buttonMenuOpenCollection, Resources.box_48, true);
			setupButton(_buttonMenuSaveDeck, Resources.deck_48, true);
			setupButton(_buttonMenuSaveCollection, Resources.box_48, true);
			setupButton(_buttonOpenWindow, Resources.add_form_32, true);

			setupButton(_buttonLanguage, Resources.en, true);
			foreach (var langButton in getLanguageMenuItems())
				setupButton(langButton, _languageIcons[langButton.Text.Trim()], true);

			setupButton(_buttonDonate, null, false);
			setupButton(_buttonDonateYandexMoney, Resources.yandex_money_32, false);
			setupButton(_buttonDonatePayPal, Resources.paypal_32, false);

			_buttonSubsystem.SetupPopup(new Popup(_menuLanguage,
				_buttonLanguage,
				container: _layoutLanguage,
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

		private void setupButton(ButtonBase button, Bitmap image, bool areImagesDoublesized)
		{
			var hoveredImage = image?.TransformColors(1.1f, 1.05f);

			_buttonSubsystem.SetupButton(button,
				new ButtonImages(
					image,
					image,
					hoveredImage,
					hoveredImage,
					areImagesDoublesized));
		}

		private void setupButton(ButtonBase button, Bitmap image, Bitmap imageDouble)
		{
			bool useDoubleSizedImage = Dpi.ScalePercent > 100;

			Bitmap normal;
			Bitmap hovered;

			if (useDoubleSizedImage)
			{
				hovered = imageDouble?.TransformColors(1.1f, 1.05f);
				normal = imageDouble;
			}
			else
			{
				hovered = image?.TransformColors(1.1f, 1.05f);
				normal = image;
			}

			_buttonSubsystem.SetupButton(button,
				new ButtonImages(
					normal,
					normal,
					hovered,
					hovered,
					useDoubleSizedImage));
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
				else if (!form.IsSearchFocused() && form.GetSelectedText() != null)
					form.ResetSelectedText();
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
				if (form.IsSearchFocused())
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
				string selectedText;

				if (form.IsSearchFocused())
					handled = false;
				else if (!string.IsNullOrEmpty(selectedText = form.GetSelectedText()))
					Clipboard.SetText(selectedText);
				else
					form.CopyDeck();
			}
			else if (e.KeyData == (Keys.Alt | Keys.C))
			{
				if (form.IsSearchFocused())
					handled = false;
				else
					form.CopyCollection();
			}
			else if (e.KeyData == (Keys.Control | Keys.A))
			{
				if (form.IsSearchFocused())
					handled = false;
				else
					form.SelectAllText();
			}
			else
			{
				if (!e.Modifiers.Equals(Keys.Alt))
					handled = false;
			}

			e.Handled = handled;
			e.SuppressKeyPress = handled;
		}

		private void unsubsribeButtonEvents()
		{
			_buttonSubsystem.UnsubscribeFromEvents();
		}

		private readonly ButtonBase[] _deckButtons;

		private readonly ButtonSubsystem _buttonSubsystem;
		private readonly CheckBox[] _saveLoadButtons;
		private readonly CheckBox[][] _saveLoadMenuButtons;
		private readonly Dictionary<string, Bitmap> _languageIcons;
	}
}