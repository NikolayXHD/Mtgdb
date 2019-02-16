using System;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void scale()
		{
			_deckEditorSubsystem.Scale();
			_searchBar.ScaleDpi();
			_menuSearchExamples.ScaleDpi();

			_buttonShowDuplicates.ScaleDpi();

			_buttonSampleHandNew.ScaleDpi();
			_buttonSampleHandMulligan.ScaleDpi();
			_buttonSampleHandDraw.ScaleDpi();

			_buttonHideDeck.ScaleDpi();
			_buttonShowScrollCards.ScaleDpi();
			_buttonShowScrollDeck.ScaleDpi();
			_buttonShowPartialCards.ScaleDpi();
			_buttonShowText.ScaleDpi();
			_dropdownSearchExamples.ScaleDpi();
			_buttonResetFilters.ScaleDpi();

			_labelStatusScrollCards.ScaleDpi();
			_labelStatusScrollDeck.ScaleDpi();

			_labelStatusSets.ScaleDpi();
			_labelStatusCollection.ScaleDpi();
			_labelStatusFilterButtons.ScaleDpi();
			_labelStatusSearch.ScaleDpi();
			_labelStatusFilterCollection.ScaleDpi();
			_labelStatusFilterDeck.ScaleDpi();
			_labelStatusFilterLegality.ScaleDpi();
			_labelStatusSort.ScaleDpi();

			Bitmap transformIcon(Bitmap bmp) =>
				bmp?.HalfResizeDpi();

			_tabHeadersDeck.ScaleDpi(transformIcon);

			scaleLayoutView(_layoutViewCards);
			scaleLayoutView(_layoutViewDeck);

			foreach (var qf in _quickFilterControls.Append(FilterManager))
				qf.ScaleDpi();

			_panelIconSearch.ScaleDpi(transformIcon);
			_panelIconLegality.ScaleDpi(transformIcon);
			_panelIconStatusSets.ScaleDpi(transformIcon);
			_panelIconStatusCollection.ScaleDpi(transformIcon);
			_panelIconStatusFilterButtons.ScaleDpi(transformIcon);
			_panelIconStatusSearch.ScaleDpi(transformIcon);
			_panelIconStatusFilterCollection.ScaleDpi(transformIcon);
			_panelIconStatusFilterDeck.ScaleDpi(transformIcon);
			_panelIconStatusFilterLegality.ScaleDpi(transformIcon);
			_panelIconStatusSort.ScaleDpi(transformIcon);

			_deckListControl.Scale();

			_searchBar.ScaleDpi();

			new DpiScaler<FormMain>(
				form =>
				{
					form._buttonSampleHandNew.ButtonImages = ButtonImages.ScaleDpi((null, Resources.hand_48));
					form._buttonSampleHandDraw.ButtonImages = ButtonImages.ScaleDpi((null, Resources.draw_48));
					form._buttonSampleHandMulligan.ButtonImages = ButtonImages.ScaleDpi((null, Resources.mulligan_48));
					form._buttonShowDuplicates.ButtonImages = ButtonImages.ScaleDpi((null, Resources.clone_48));
					form._buttonShowProhibit.ButtonImages = ButtonImages.ScaleDpi((null, Resources.exclude_minus_24));

					form._buttonExcludeManaAbility.ButtonImages = ButtonImages.ScaleDpi(
						(null, Resources.include_plus_24),
						(null, Resources.exclude_minus_24));

					form._buttonExcludeManaCost.ButtonImages = ButtonImages.ScaleDpi(
						(null, Resources.include_plus_24),
						(null, Resources.exclude_minus_24));

					form._buttonHideDeck.ButtonImages = ButtonImages.ScaleDpi(
						(null, Resources.shown_40),
						(null, Resources.hidden_40));

					var scrollImages = ButtonImages.ScaleDpi((null, Resources.scroll_shown_40));
					form._buttonShowScrollCards.ButtonImages = scrollImages;
					form._buttonShowScrollDeck.ButtonImages = scrollImages;

					form._buttonShowPartialCards.ButtonImages = ButtonImages.ScaleDpi((null, Resources.partial_card_enabled_40));
					form._buttonShowText.ButtonImages = ButtonImages.ScaleDpi((null, Resources.text_enabled_40));
					form._dropdownSearchExamples.ButtonImages = ButtonImages.ScaleDpi((null, Resources.book_40));
					form._buttonResetFilters.ButtonImages = ButtonImages.ScaleDpi((null, Resources.erase));

					int border = FilterManaCost.Border;
					var modeButtonSize = FilterManaCost.ImageSize.Plus(new Size(border, border).MultiplyBy(2));
					int rightMargin = FilterManaCost.Width - modeButtonSize.Width;

					new[]
					{
						_buttonExcludeManaAbility,
						_buttonExcludeManaCost,
						_buttonShowProhibit
					}.ForEach(button => button.Size = modeButtonSize);

					setRowHeight(_buttonShowProhibit, modeButtonSize);

					_buttonExcludeManaCost.Margin =
						_buttonExcludeManaAbility.Margin =
							new Padding(0, 0, rightMargin, 0);

					_buttonShowProhibit.Margin = new Padding(0);

					int deckHeight = _imageLoader.CardSize.Height + _layoutViewDeck.LayoutOptions.CardInterval.Height;

					_layoutViewDeck.Height = deckHeight;
					_deckListControl.Height = deckHeight;
				}).Setup(this);

			_menuLegality.ScaleDpi();
			_menuLegalityCheckBoxes.ForEach(CheckBoxScaler.ScaleDpi);
		}

		private static void scaleLayoutView(LayoutViewControl view)
		{
			view.ScaleDpi(
				bmp => bmp.HalfResizeDpi(),
				transformSearchIcon: bmp => bmp.HalfResizeDpi(), transformCustomButtonIcon: (bmp, field, i) =>
				{
					int delta = DeckEditorButtons.GetCountDelta(i);
					bool isDeck = DeckEditorButtons.IsDeck(i);
					return bmp.HalfResizeDpi(preventMoire: isDeck && Math.Abs(delta) == 1);
				});
		}

		private void beforeDpiChanged()
		{
			_panelFilters.SuspendLayout();
			_panelStatus.SuspendLayout();
			_panelMenu.SuspendLayout();
			_searchBar.SuspendLayout();
			_panelMenuRightSubpanel.SuspendLayout();
			_panelRightCost.SuspendLayout();
			_panelManaAbility.SuspendLayout();
			_layoutMain.SuspendLayout();
			_layoutRight.SuspendLayout();
			_panelRightNarrow.SuspendLayout();
			_panelRightManaCost.SuspendLayout();
			_layoutRoot.SuspendLayout();
			SuspendLayout();
		}

		private void afterDpiChanged()
		{
			_panelFilters.ResumeLayout(false);
			_panelFilters.PerformLayout();

			_panelStatus.ResumeLayout(false);
			_panelStatus.PerformLayout();

			_panelMenu.ResumeLayout(false);
			_panelMenu.PerformLayout();

			_searchBar.ResumeLayout(false);
			_searchBar.PerformLayout();

			_panelMenuRightSubpanel.ResumeLayout(false);
			_panelMenuRightSubpanel.PerformLayout();

			_panelRightCost.ResumeLayout(false);
			_panelRightCost.PerformLayout();

			_panelManaAbility.ResumeLayout(false);
			_panelManaAbility.PerformLayout();

			_layoutMain.ResumeLayout(false);
			_layoutMain.PerformLayout();

			_layoutRight.ResumeLayout(false);
			_layoutRight.PerformLayout();

			_panelRightNarrow.ResumeLayout(false);
			_panelRightNarrow.PerformLayout();

			_panelRightManaCost.ResumeLayout(false);
			_panelRightManaCost.PerformLayout();

			_layoutRoot.ResumeLayout(false);
			_layoutRoot.PerformLayout();

			ResumeLayout(false);
			PerformLayout();

			_layoutRight.PerformLayout();
		}
	}
}