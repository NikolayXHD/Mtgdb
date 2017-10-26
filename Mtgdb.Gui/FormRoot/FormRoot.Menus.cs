using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
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

			_buttonMenuGeneralSettings.Click += configClick;
			_buttonMenuDisplaySettings.Click += configClick;
			_buttonTooltips.CheckedChanged += tooltipsChecked;

			_buttonMenuPaste.Click += pasteClick;
			_buttonPaste.Click += pasteClick;
			_buttonMenuPasteAppend.Click += pasteClick;
		}

		private void pasteClick(object sender, EventArgs e)
		{
			getSelectedForm()?.PasteDeck(append: sender == _buttonMenuPasteAppend);
		}

		private void configClick(object sender, EventArgs e)
		{
			if (_buttonMenuGeneralSettings == sender)
				System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.xml"));
			else if (_buttonMenuDisplaySettings == sender)
				System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.Display.xml"));
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
			getSelectedForm()?.ButtonRedo();
			_undoingOrRedoing = false;
		}

		private void undoClick(object sender, EventArgs e)
		{
			if (_undoingOrRedoing)
				return;

			_undoingOrRedoing = true;
			getSelectedForm()?.ButtonUndo();
			_undoingOrRedoing = false;
		}

		private void clearClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonClearDeck();
		}

		private void printClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonPrint();
		}

		private void statClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonPivot();
		}

		private void saveDeckClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonSaveDeck();
		}

		private void openDeckClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonLoadDeck();
		}

		private void saveCollectionClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonSaveCollection();
		}

		private void openCollectionClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonLoadCollection();
		}

		private void downloadClick(object sender, EventArgs e)
		{
			_downloaderSubsystem.ShowDownloader(this, auto: false);

			ThreadPool.QueueUserWorkItem(_ =>
			{
				Thread.Sleep(2000);
				this.Invoke(updateDownloadButton);
			});
		}

		private void setupLanguageMenu()
		{
			foreach (var langMenuItem in getLanguageMenuItems())
				langMenuItem.Click += languageMenuClick;
		}

		private void languageMenuClick(object sender, EventArgs e)
		{
			var button = (ButtonBase) sender;
			Language = button.Text.ToLowerInvariant();
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

			setupButton(_buttonDownload, Resources.update_40, true);
			setupButton(_buttonMenuOpenDeck, Resources.draw_a_card_48, true);
			setupButton(_buttonMenuOpenCollection, Resources.box_48, true);
			setupButton(_buttonMenuSaveDeck, Resources.draw_a_card_48, true);
			setupButton(_buttonMenuSaveCollection, Resources.box_48, true);

			setupButton(_buttonLanguage, Resources.en, true);
			foreach (var langButton in getLanguageMenuItems())
				setupButton(langButton, _languageIcons[langButton.Text.Trim()], true);

			setupButton(_buttonDonate, null, false);
			setupButton(_buttonDonateYandexMoney, Resources.yandex_money_32, false);
			setupButton(_buttonDonatePayPal, Resources.paypal_32, false);

			_buttonSubsystem.SetupPopup(new Popup(_menuConfig, _buttonConfig, container: _layoutConfig));

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
			var form = getSelectedForm();
			if (form == null)
				return;

			bool handled = true;

			if (e.KeyData == Keys.F1)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"help\\html\\Search_input_keyboard_shortcuts.html"));
			else if (e.KeyData == (Keys.Control | Keys.F4))
				CloseTab();
			else if (e.KeyData == (Keys.Control | Keys.Tab))
				NextTab();
			else if (e.KeyData == (Keys.Control | Keys.T))
				NewTab(onCreated: null);
			else if (e.KeyData == (Keys.Alt | Keys.Left) || e.KeyData == (Keys.Control | Keys.Z))
				form.ButtonUndo();
			else if (e.KeyData == (Keys.Alt | Keys.Right) || e.KeyData == (Keys.Control | Keys.Y))
				form.ButtonRedo();
			else if (e.KeyData == (Keys.Control | Keys.F))
				form.FocusSearch();
			else if (e.KeyData == Keys.Escape)
				form.StopDragging();
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



		public string Language
		{
			get { return _language; }
			set
			{
				value = value.Trim();

				if (value == _language)
					return;

				_language = value;

				var menuItem = getLanguageMenuItems()
					.Single(_ => Str.Equals(_.Text.Trim(), value));

				_buttonLanguage.Image = menuItem.Image;
				_buttonLanguage.Text = value.ToUpperInvariant();
				setupButton(_buttonLanguage, _languageIcons[value], true);

				LanguageChanged?.Invoke();
			}
		}

		private readonly ButtonBase[] _deckButtons;
		public event Action LanguageChanged;

		private readonly ButtonSubsystem _buttonSubsystem;
		private readonly CheckBox[] _saveLoadButtons;
		private readonly CheckBox[][] _saveLoadMenuButtons;
		private readonly Dictionary<string, Bitmap> _languageIcons;
	}
}