using System;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void scale()
		{
			new[]
				{
					_buttonSampleHandNew,
					_buttonSampleHandDraw,
					_buttonSampleHandMulligan,
					_buttonShowDuplicates,
					_buttonShowScrollCards,
					_buttonShowScrollDeck,
					_buttonShowPartialCards,
					_buttonShowText,
					_buttonResetFilters,
					_buttonExcludeManaAbility,
					_buttonExcludeManaCost,
					_buttonShowProhibit,
					_buttonHideDeck
				}
				.ForEach(ButtonBaseScaler.ScaleDpiAuto);

			new[]
			{
				_buttonLegalityAllowLegal,
				_buttonLegalityAllowRestricted,
				_buttonLegalityAllowBanned,
				_buttonLegalityAllowFuture
			}.ForEach(ButtonBaseScaler.ScaleDpiAuto);

			_deckEditorSubsystem.Scale();
			_menuSearchExamples.Scale();

			_labelStatusScrollCards.ScaleDpi();
			_labelStatusScrollDeck.ScaleDpi();

			_labelStatusSets.ScaleDpiAuto();
			_labelStatusCollection.ScaleDpiAuto();
			_labelStatusFilterButtons.ScaleDpiAuto();
			_labelStatusSearch.ScaleDpiAuto();
			_labelStatusFilterCollection.ScaleDpiAuto();
			_labelStatusFilterDeck.ScaleDpiAuto();
			_labelStatusFilterLegality.ScaleDpiAuto();
			_labelStatusSort.ScaleDpiAuto();

			Bitmap transformIcon(Bitmap bmp) =>
				bmp?.HalfResizeDpi();

			_tabHeadersDeck.ScaleDpi(transformIcon);

			scaleLayoutView(_layoutViewCards);
			scaleLayoutView(_layoutViewDeck);

			foreach (var qf in _quickFilterControls.Append(FilterManager))
				qf.ScaleDpi();

			_deckListControl.Scale();

			_popupSearchExamples.ScaleDpiAuto();

			_dropdownLegality.ScaleDpi();
			_searchBar.ScaleDpi();

			new DpiScaler<FormMain>(
				form =>
				{
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