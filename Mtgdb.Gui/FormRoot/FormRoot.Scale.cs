using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Gui.Properties;
using ButtonBase = Mtgdb.Controls.ButtonBase;

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

			_buttonMenuDonateYandexMoney.ScaleDpi();
			_buttonMenuDonatePayPal.ScaleDpi();
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

			_buttonMenuEditConfig.ScaleDpiHeight();
			_buttonMenuEditConfig.ScaleDpiFont();

			_buttonImportMtgArenaCollection.ScaleDpiFont();
			_buttonImportExportToMtgArena.ScaleDpiFont();
			_menuColors.ScaleDpiFont();

			_labelFormats.ScaleDpiFont();

			new DpiScaler<FormRoot>(form =>
			{
				form._buttonUndo.ButtonImages = ButtonImages.ScaleDpi((Resources.undo_16, Resources.undo_32));
				form._buttonRedo.ButtonImages = ButtonImages.ScaleDpi((Resources.redo_16, Resources.redo_32));
				form._buttonSaveDeck.ButtonImages = ButtonImages.ScaleDpi((Resources.save_16, Resources.save_32));
				form._buttonOpenDeck.ButtonImages = ButtonImages.ScaleDpi((Resources.open_16, Resources.open_32));
				form._buttonStat.ButtonImages = ButtonImages.ScaleDpi((Resources.chart_16, Resources.chart_32));
				form._buttonPrint.ButtonImages = ButtonImages.ScaleDpi((Resources.print_16, Resources.print_32));
				form._buttonClear.ButtonImages = ButtonImages.ScaleDpi((Resources.trash_16, Resources.trash_32));
				form._buttonPaste.ButtonImages = ButtonImages.ScaleDpi((Resources.paste_16, Resources.paste_32));
				form._buttonHelp.ButtonImages = ButtonImages.ScaleDpi((Resources.index_16, Resources.index_32));
				form._buttonConfig.ButtonImages = ButtonImages.ScaleDpi((Resources.properties_16, Resources.properties_32));
				form._buttonTooltips.ButtonImages = ButtonImages.ScaleDpi((Resources.tooltip_16, Resources.tooltip_32));
				form._buttonImportExportToMtgArena.ButtonImages = ButtonImages.ScaleDpi((Resources.paste_16, Resources.paste_32));

				foreach (var langButton in getLanguageMenuItems())
					langButton.ButtonImages = ButtonImages.ScaleDpi((null, form._languageIcons[langButton.Text.Trim()]));

				updateButtonLanguage();

				form._buttonShowFilterPanels.ButtonImages = ButtonImages.ScaleDpi((null, Resources.filters_show_32));
				form._buttonUpdate.ButtonImages = ButtonImages.ScaleDpi((null, Resources.update_40));
				form._buttonMenuOpenDeck.ButtonImages = ButtonImages.ScaleDpi((null, Resources.deck_48));
				form._buttonMenuOpenCollection.ButtonImages = ButtonImages.ScaleDpi((null, Resources.box_48));
				form._buttonMenuSaveDeck.ButtonImages = ButtonImages.ScaleDpi((null, Resources.deck_48));
				form._buttonMenuSaveCollection.ButtonImages = ButtonImages.ScaleDpi((null, Resources.box_48));
				form._buttonOpenWindow.ButtonImages = ButtonImages.ScaleDpi((null, Resources.add_form_32));

				form._buttonColorScheme.ButtonImages = ButtonImages.ScaleDpi((null, Resources.color_swatch_32));
				form._buttonMenuDonateYandexMoney.ButtonImages = ButtonImages.ScaleDpi((Resources.yandex_money_32, null));
				form._buttonMenuDonatePayPal.ButtonImages = ButtonImages.ScaleDpi((Resources.paypal_32, null));

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