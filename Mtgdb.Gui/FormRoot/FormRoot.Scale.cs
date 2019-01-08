using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public partial class FormRoot
	{
		private void scale()
		{
			Dpi.BeforeChanged += beforeDpiChanged;
			Dpi.AfterChanged += afterDpiChanged;
			FormClosing += formClosing;

			void formClosing(object s, FormClosingEventArgs e)
			{
				Dpi.BeforeChanged -= beforeDpiChanged;
				Dpi.AfterChanged -= afterDpiChanged;
				FormClosing -= formClosing;
			}

			this.ScaleDpi();

			_buttonDonateYandexMoney.ScaleDpi();
			_buttonDonatePayPal.ScaleDpi();
			_panelAva.ScaleDpi(bmp => bmp?.HalfResizeDpi());
			_labelDonate.ScaleDpi();

			_buttonMenuPasteDeck.ScaleDpi();
			_buttonMenuPasteDeckAppend.ScaleDpi();
			_buttonMenuPasteCollection.ScaleDpi();
			_buttonMenuPasteCollectionAppend.ScaleDpi();
			_buttonMenuCopyCollection.ScaleDpi();
			_buttonMenuCopyDeck.ScaleDpi();

			_labelPasteInfo.ScaleDpi();

			_buttonMenuOpenDeck.ScaleDpi();
			_buttonMenuSaveDeck.ScaleDpi();
			_buttonMenuOpenCollection.ScaleDpi();
			_buttonMenuSaveCollection.ScaleDpi();
			_buttonVisitForge.ScaleDpi();
			_buttonVisitMagarena.ScaleDpi();
			_buttonVisitXMage.ScaleDpi();
			_buttonVisitMtgo.ScaleDpi();
			_buttonVisitDotP2014.ScaleDpi();
			_buttonVisitCockatrice.ScaleDpi();
			_buttonVisitMtgArena.ScaleDpi();
			_buttonVisitDeckedBuilder.ScaleDpi();
			_labelMtgo.ScaleDpi();
			_labelMagarena.ScaleDpi();
			_labelDotP2.ScaleDpi();

			_buttonImportMtgArenaCollection.ScaleDpiHeight();
			_buttonImportExportToMtgArena.ScaleDpiHeight();

			_tabs.ScaleDpi(bmp => bmp?.HalfResizeDpi());

			foreach (var langButton in getLanguageMenuItems())
				langButton.ScaleDpi();

			foreach (var titleButton in _flowTitleLeft.Controls.OfType<ButtonBase>())
				titleButton.ScaleDpi();

			foreach (var titleButton in _flowTitleRight.Controls.OfType<ButtonBase>())
				titleButton.ScaleDpi();

			_menuUiScale.ScaleDpi();
			_menuUiSmallImageQuality.ScaleDpi();
			_menuUiSuggestDownloadMissingImages.ScaleDpi();
			_menuUiImagesCacheCapacity.ScaleDpi();
			_menuUiUndoDepth.ScaleDpi();
			_labelUiScale.ScaleDpi();
			_labelUiUseSmallImages.ScaleDpi();
			_labelUiSuggestDownloadMissingImages.ScaleDpi();
			_labelUiImageCacheCapacity.ScaleDpi();
			_labelUiUndoDepth.ScaleDpi();
			_labelUiUseSmallImagesHint.ScaleDpi();
			_labelUiAppliedAfterRestartHint.ScaleDpi();

			_buttonEditConfig.ScaleDpiHeight();
			_buttonEditConfig.ScaleDpiFont();

			_buttonImportMtgArenaCollection.ScaleDpiFont();
			_buttonImportExportToMtgArena.ScaleDpiFont();
			_menuColors.ScaleDpiFont();

			_labelFormats.ScaleDpiFont();

			new DpiScaler<FormRoot>(form =>
			{
				form._buttonSubsystem.SetupButton(form._buttonUndo, ButtonImages.ScaleDpi((Resources.undo_16, Resources.undo_32)));
				form._buttonSubsystem.SetupButton(form._buttonRedo, ButtonImages.ScaleDpi((Resources.redo_16, Resources.redo_32)));
				form._buttonSubsystem.SetupButton(form._buttonSaveDeck, ButtonImages.ScaleDpi((Resources.save_16, Resources.save_32)));
				form._buttonSubsystem.SetupButton(form._buttonOpenDeck, ButtonImages.ScaleDpi((Resources.open_16, Resources.open_32)));
				form._buttonSubsystem.SetupButton(form._buttonStat, ButtonImages.ScaleDpi((Resources.chart_16, Resources.chart_32)));
				form._buttonSubsystem.SetupButton(form._buttonPrint, ButtonImages.ScaleDpi((Resources.print_16, Resources.print_32)));
				form._buttonSubsystem.SetupButton(form._buttonClear, ButtonImages.ScaleDpi((Resources.trash_16, Resources.trash_32)));
				form._buttonSubsystem.SetupButton(form._buttonPaste, ButtonImages.ScaleDpi((Resources.paste_16, Resources.paste_32)));
				form._buttonSubsystem.SetupButton(form._buttonHelp, ButtonImages.ScaleDpi((Resources.index_16, Resources.index_32)));
				form._buttonSubsystem.SetupButton(form._buttonConfig, ButtonImages.ScaleDpi((Resources.properties_16, Resources.properties_32)));
				form._buttonSubsystem.SetupButton(form._buttonTooltips, ButtonImages.ScaleDpi((Resources.tooltip_16, Resources.tooltip_32)));
				form._buttonSubsystem.SetupButton(form._buttonImportExportToMtgArena, ButtonImages.ScaleDpi((Resources.paste_16, Resources.paste_32)));

				foreach (var langButton in getLanguageMenuItems())
					form._buttonSubsystem.SetupButton(langButton,
						ButtonImages.ScaleDpi((null, form._languageIcons[langButton.Text.Trim()])));

				form._buttonSubsystem.SetupButton(form._buttonShowFilterPanels, ButtonImages.ScaleDpi((null, Resources.filters_show_32)));
				form._buttonSubsystem.SetupButton(form._buttonUpdate, ButtonImages.ScaleDpi((null, Resources.update_40)));
				form._buttonSubsystem.SetupButton(form._buttonMenuOpenDeck, ButtonImages.ScaleDpi((null, Resources.deck_48)));
				form._buttonSubsystem.SetupButton(form._buttonMenuOpenCollection, ButtonImages.ScaleDpi((null, Resources.box_48)));
				form._buttonSubsystem.SetupButton(form._buttonMenuSaveDeck, ButtonImages.ScaleDpi((null, Resources.deck_48)));
				form._buttonSubsystem.SetupButton(form._buttonMenuSaveCollection, ButtonImages.ScaleDpi((null, Resources.box_48)));
				form._buttonSubsystem.SetupButton(form._buttonOpenWindow, ButtonImages.ScaleDpi((null, Resources.add_form_32)));
				form._buttonSubsystem.SetupButton(form._buttonLanguage, ButtonImages.ScaleDpi((null, Resources.en)));
				form._buttonSubsystem.SetupButton(form._buttonColorScheme, ButtonImages.ScaleDpi((null, Resources.color_swatch_32)));
				form._buttonSubsystem.SetupButton(form._buttonDonateYandexMoney, ButtonImages.ScaleDpi((Resources.yandex_money_32, null)));
				form._buttonSubsystem.SetupButton(form._buttonDonatePayPal, ButtonImages.ScaleDpi((Resources.paypal_32, null)));
			}).Setup(this);
		}

		private void beforeDpiChanged()
		{
			_panelCaption.SuspendLayout();
			_menuOpen.SuspendLayout();
			_menuLanguage.SuspendLayout();
			_menuDonate.SuspendLayout();
			_menuPaste.SuspendLayout();
			_layoutTitle.SuspendLayout();
			_flowTitleRight.SuspendLayout();
			_flowTitleLeft.SuspendLayout();
			_menuColors.SuspendLayout();
			_menuConfig.SuspendLayout();
			SuspendLayout();
		}

		private void afterDpiChanged()
		{
			_panelCaption.ResumeLayout(false);
			_panelCaption.PerformLayout();

			_menuOpen.ResumeLayout(false);
			_menuOpen.PerformLayout();

			_menuLanguage.ResumeLayout(false);
			_menuLanguage.PerformLayout();

			_menuDonate.ResumeLayout(false);
			_menuDonate.PerformLayout();

			_menuPaste.ResumeLayout(false);
			_menuPaste.PerformLayout();

			_layoutTitle.ResumeLayout(false);
			_layoutTitle.PerformLayout();

			_flowTitleRight.ResumeLayout(false);
			_flowTitleRight.PerformLayout();

			_flowTitleLeft.ResumeLayout(false);
			_flowTitleLeft.PerformLayout();

			_menuColors.ResumeLayout(false);
			_menuColors.PerformLayout();

			_menuConfig.ResumeLayout(false);
			_menuConfig.PerformLayout();

			ResumeLayout(false);
			PerformLayout();
		}
	}
}