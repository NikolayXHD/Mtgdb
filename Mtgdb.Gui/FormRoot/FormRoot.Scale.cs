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

			_panelAva.ScaleDpi(bmp => bmp?.HalfResizeDpi());
			_tabs.ScaleDpi(bmp => bmp?.HalfResizeDpi());

			_labelPasteInfo.ScaleDpi();

			_labelFormats.ScaleDpiFont();
			_labelMtgo.ScaleDpiFont();
			_labelMagarena.ScaleDpiFont();
			_labelDotP2.ScaleDpiFont();

			_labelUiScale.ScaleDpi();
			_labelUiUseSmallImages.ScaleDpi();
			_labelUiSuggestDownloadMissingImages.ScaleDpi();
			_labelUiImageCacheCapacity.ScaleDpi();
			_labelUiUndoDepth.ScaleDpi();
			_labelUiUseSmallImagesHint.ScaleDpi();

			_labelDonate.ScaleDpi();

			_buttonUndo.ScaleDpiImages((Resources.undo_16, Resources.undo_32));
			_buttonRedo.ScaleDpiImages((Resources.redo_16, Resources.redo_32));
			_dropdownSaveDeck.ScaleDpiImages((Resources.save_16, Resources.save_32));
			_dropdownOpenDeck.ScaleDpiImages((Resources.open_16, Resources.open_32));
			_buttonStat.ScaleDpiImages((Resources.chart_16, Resources.chart_32));
			_buttonPrint.ScaleDpiImages((Resources.print_16, Resources.print_32));
			_buttonClear.ScaleDpiImages((Resources.trash_16, Resources.trash_32));
			_dropdownPaste.ScaleDpiImages((Resources.paste_16, Resources.paste_32));
			_buttonHelp.ScaleDpiImages((Resources.index_16, Resources.index_32));
			_dropdownConfig.ScaleDpiImages((Resources.properties_16, Resources.properties_32));
			_buttonTooltips.ScaleDpiImages((Resources.tooltip_16, Resources.tooltip_32));
			_buttonImportExportToMtgArena.ScaleDpiImages((Resources.paste_16, Resources.paste_32));

			new[]
				{
					_buttonMenuDonateYandexMoney,
					_buttonMenuDonatePayPal,
					_buttonMenuPasteDeck,
					_buttonMenuPasteDeckAppend,
					_buttonMenuPasteCollection,
					_buttonMenuPasteCollectionAppend,
					_buttonMenuCopyCollection,
					_buttonMenuCopyDeck,
					_buttonMenuOpenDeck,
					_buttonMenuSaveDeck,
					_buttonMenuOpenCollection,
					_buttonMenuSaveCollection,
					_buttonVisitForge,
					_buttonVisitMagarena,
					_buttonVisitXMage,
					_buttonVisitMtgo,
					_buttonVisitDotP2014,
					_buttonVisitCockatrice,
					_buttonVisitMtgArena,
					_buttonVisitDeckedBuilder,
					_buttonImportMtgArenaCollection,
					_buttonImportExportToMtgArena
				}
				.Concat(_flowTitleLeft.Controls.OfType<ButtonBase>())
				.Concat(_flowTitleRight.Controls.OfType<ButtonBase>())
				.Concat(getLanguageMenuItems())
				.ForEach(ButtonBaseScaler.ScaleDpi);

			new[]
			{
				_menuUiScale,
				_menuUiSmallImageQuality,
				_menuUiSuggestDownloadMissingImages,
				_menuUiImagesCacheCapacity,
				_menuUiUndoDepth
			}.ForEach(DropDownBaseScaler.ScaleDpi);

			_buttonMenuEditConfig.ScaleDpiHeight();
			_buttonMenuEditConfig.ScaleDpiFont();

			_menuColors.ScaleDpiFont();
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