using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	partial class FormRoot
	{
		private void setupButtonClicks()
		{
			_buttonTooltips.Checked = true;
			_buttonDownload.Enabled = false;

			_buttonLoad.MouseEnter += loadMouseEnter;
			_buttonSave.MouseEnter += saveMouseEnter;
			_buttonDownload.Click += downloadClick;
			_buttonLoad.Click += loadDeckClick;
			_buttonSave.Click += saveDeckClick;
			_buttonStat.Click += statClick;
			_buttonPrint.Click += printClick;
			_buttonClear.Click += clearClick;
			_buttonUndo.Click += undoClick;
			_buttonRedo.Click += redoClick;

			var helpButtons = new[]
			{
				_buttonHelpButtons,
				_buttonHelpSearchSyntax,
				_buttonHelpSearchShortcuts,
				_buttonHelpSearchFromGrid,
				_buttonHelpSort,
				_buttonHelpReleaseNotes,
				_buttonHelpReadme,
				_buttonHelpEditor,
				_buttonHelpForum,
				_buttonHelpAbout,
				_buttonHelpWiki
			};

			foreach (var helpButton in helpButtons)
				helpButton.Click += helpClick;

			_buttonGeneralSettings.Click += configClick;
			_buttonDisplaySettings.Click += configClick;
			_buttonTooltips.CheckedChanged += tooltipsChecked;
		}

		private void configClick(object sender, EventArgs e)
		{
			if (_buttonGeneralSettings == sender)
				System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.xml"));
			else if (_buttonDisplaySettings == sender)
				System.Diagnostics.Process.Start(AppDir.Etc.AddPath(@"Mtgdb.Gui.Display.xml"));
		}

		private void tooltipsChecked(object sender, EventArgs e)
		{
			HideTooltips = !((CheckBox)sender).Checked;

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain)_tabs.TabIds[i];
				formMain?.ButtonTooltip();
			}
		}

		private void helpClick(object sender, EventArgs e)
		{
			if (_buttonHelpButtons == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_help_buttons.txt"));
			else if (_buttonHelpEditor == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_help_editor.txt"));

			else if (_buttonHelpSearchSyntax == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_help_search_syntax.txt"));
			else if (_buttonHelpSearchShortcuts == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_help_search_keyboard_shortcuts.txt"));
			else if (_buttonHelpSearchFromGrid == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_help_search_button_over_card_text.txt"));

			else if (_buttonHelpSort == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_help_sort.txt"));
			else if (_buttonHelpReleaseNotes == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_release_notes.txt"));
			else if (_buttonHelpReadme == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_readme.txt"));
			else if (_buttonHelpForum == sender)
				System.Diagnostics.Process.Start("https://www.slightlymagic.net/forum/viewtopic.php?f=62&t=19299");
			else if (_buttonHelpAbout == sender)
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_about.txt"));
			else if (_buttonHelpWiki == sender)
				System.Diagnostics.Process.Start("https://github.com/NikolayXHD/Mtgdb/wiki");
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

		private void loadDeckClick(object sender, EventArgs e)
		{
			getSelectedForm()?.ButtonLoadDeck();
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
			var button = (ButtonBase)sender;
			Language = button.Text.ToLowerInvariant();
		}

		private IEnumerable<CheckBox> getLanguageMenuItems()
		{
			return _menuLanguage.Controls.OfType<CheckBox>();
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
		}

		private static void buttonVisitClick(object sender, EventArgs e)
		{
			var control = (Control)sender;
			var url = control.GetTag<string>();
			System.Diagnostics.Process.Start(url);
		}



		private void loadMouseEnter(object sender, EventArgs e)
		{
			_labelLoad.Visible = true;
			_labelSave.Visible = false;
		}

		private void saveMouseEnter(object sender, EventArgs e)
		{
			_labelLoad.Visible = false;
			_labelSave.Visible = true;
		}



		private void setupButtons()
		{
			setupButton(_buttonLoad);
			setupButton(_buttonSave);
			setupButton(_buttonStat);
			setupButton(_buttonPrint);
			setupButton(_buttonClear);
			setupButton(_buttonDownload);
			setupButton(_buttonConfig);
			setupButton(_buttonHelp);
			setupButton(_buttonLanguage);
			setupButton(_buttonDonate);
			setupButton(_buttonLoad);
			setupButton(_buttonSave);
			setupButton(_buttonTooltips);

			_buttonSubsystem.SetupPopup(new Popup(_menuConfig, _buttonConfig));
			_buttonSubsystem.SetupPopup(new Popup(_menuHelp, _buttonHelp));

			_buttonSubsystem.SetupPopup(new Popup(_menuLanguage, _buttonLanguage,
				closeOnMenuClick: true));

			_buttonSubsystem.SetupPopup(new Popup(_menuDonate, _buttonDonate,
				alignment: HorizontalAlignment.Center,
				openOnHover: false,
				closeOnMenuClick: true,
				borderOnHover: false));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen, _buttonLoad,
				borderOnHover: false));

			_buttonSubsystem.SetupPopup(new Popup(_menuOpen, _buttonSave,
				borderOnHover: false));

			_buttonSubsystem.SubscribeToEvents();
		}

		private void setupButton(CheckBox button)
		{
			var image = (Bitmap) button.Image;
			var hoveredImage = image?.TransformColors(1.1f, 1.05f);

			_buttonSubsystem.SetupButton(button,
				new ButtonImages(
					image,
					image,
					hoveredImage,
					hoveredImage));
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
				if (value == _language)
					return;

				_language = value;

				var menuItem = getLanguageMenuItems()
					.Single(_ => Str.Equals(_.Text, value));

				_buttonLanguage.Image = menuItem.Image;
				_buttonLanguage.Text = value.ToUpperInvariant();
				setupButton(_buttonLanguage);

				LanguageChanged?.Invoke();
			}
		}

		private readonly ButtonBase[] _deckButtons;

		public event Action LanguageChanged;

		private readonly ButtonSubsystem _buttonSubsystem;
	}
}
