using System;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void formLoad(object sender, EventArgs e)
		{
			setupCheckButtonImages();
			updateExcludeManaAbility();
			updateExcludeManaCost();
			updateShowProhibited();

			setupTooltips();
			subscribeToEvents();
			startThreads();

			historyUpdateGlobals();
			historyApply(_historyModel.Current);
			_deckSerializationSubsystem.LastFile = _historyModel.Current.DeckFile;
		}

		private void applicationExit(object sender, EventArgs e)
		{
			new Thread(_ => _historyModel.Save()).Start();
			stopThreads();
			unsubscribeFromEvents();
		}

		private void formClosing(object sender, FormClosingEventArgs e)
		{
			stopThreads();
			unsubscribeFromEvents();
		}



		private void cardRepoSetAdded()
		{
			if (_cardRepo.SetsByCode.Count % 3 == 0)
				this.Invoke(delegate
				{
					updateFormStatus();
					Application.DoEvents();
				});
		}

		private void imageLoadingComplete()
		{
			this.Invoke(delegate
			{
				beginRestoreSettings();

				_deckModel.LoadDeck(_cardRepo);

				foreach (var handler in _onLoadHandlers)
					handler.Invoke(this);

				// to prevent memory leaks
				_onLoadHandlers.Clear();

				_sortSubsystem.Invalidate();

				if (_historyModel.Current.SearchResultScroll.HasValue)
					_viewCards.VisibleRecordIndex = _historyModel.Current.SearchResultScroll.Value;
				
				endRestoreSettings();

				RunRefilterTask();
			});
		}

		private void localizationLoadingComplete()
		{
			this.Invoke(delegate
			{
				beginRestoreSettings();
				_sortSubsystem.Invalidate();
				endRestoreSettings();
			});

			RunRefilterTask();
		}




		private void luceneSearcherLoaded()
		{
			this.Invoke(delegate
			{
				beginRestoreSettings();
				_searchStringSubsystem.ApplyFind();
				endRestoreSettings();

				RunRefilterTask();
			});
		}

		private void luceneSearcherDisposed()
		{
			this.Invoke(updateFormStatus);
		}

		private void luceneSearcherIndexingProgress()
		{
			this.Invoke(updateFormStatus);
		}



		private void formKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Modifiers.Equals(Keys.Alt))
				e.Handled = true;

			if (e.KeyData == Keys.F1)
			{
				System.Diagnostics.Process.Start(AppDir.Root.AddPath(@"_help_search.txt"));
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.F4))
			{
				_uiModel.Form.CloseTab();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.Tab))
			{
				_uiModel.Form.NextTab();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.T))
			{
				_uiModel.Form.NewTab(null);
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.S))
			{
				ButtonSaveDeck();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.O))
			{
				ButtonLoadDeck();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.S))
			{
				ButtonSaveCollection();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.Alt | Keys.O))
			{
				ButtonLoadCollection();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Control | Keys.P))
			{
				ButtonPrint();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Alt | Keys.Left) || e.KeyData == (Keys.Control | Keys.Z))
			{
				historyUndo();
				e.Handled = true;
			}
			else if (e.KeyData == (Keys.Alt | Keys.Right) || e.KeyData == (Keys.Control | Keys.Y))
			{
				e.Handled = true;
				historyRedo();
			}
			else if (e.KeyData == Keys.Escape && IsDraggingCard)
			{
				e.Handled = true;
				StopDragging();
			}
		}



		private void resetExcludeManaAbility(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				_buttonExcludeManaAbility.Checked = false;
		}

		private void resetExcludeManaCost(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle)
				_buttonExcludeManaCost.Checked = true;
		}



		private void showDuplicatesCheckedChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}

		private void showProhibitChanged(object sender, EventArgs e)
		{
			updateShowProhibited();

			if (restoringSettings())
				return;

			historyUpdate();
		}

		private void excludeManaCostChanged(object sender, EventArgs e)
		{
			updateExcludeManaCost();

			if (restoringSettings())
				return;

			historyUpdate();
		}

		private void excludeManaAbilityChanged(object sender, EventArgs e)
		{
			updateExcludeManaAbility();

			if (restoringSettings())
				return;

			historyUpdate();
		}



		private void gridScrolled(LayoutView sender)
		{
			if (sender == _viewCards)
			{
				_imagePreloadingSubsystem.Reset();

				if (_cardRepo.IsLoadingComplete)
					_historyModel.Current.SearchResultScroll = _viewCards.VisibleRecordIndex;
			}

			updateFormStatus();
		}



		private void quickFiltersChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroupButtons))
				setFilterManagerState(FilterGroupButtons, FilterValueState.Required);

			updateTerms();

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}

		private void quickFilterManagerChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}



		private void collectionChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged)
		{
			if (restoringSettings())
				return;

			if (listChanged && isFilterGroupEnabled(FilterGroupCollection))
				RunRefilterTask();
			else
			{
				if (card != null)
				{
					_viewCards.InvalidateCard(card);
					_viewDeck.InvalidateCard(card);
				}
				else
				{
					_viewCards.Invalidate();
					_viewDeck.Invalidate();
				}

				updateFormStatus();
			}

			if (countChanged || listChanged)
				historyUpdate();
		}

		private void deckChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged)
		{
			refilterChangedDeck(listChanged, touchedChanged, card);

			if (restoringSettings())
				return;

			if (countChanged || listChanged)
				historyUpdate();
		}

		private void deckZoneHover(object sender, EventArgs e)
		{
			if (!IsDraggingCard)
				return;

			var hoveredIndex = _tabHeadersDeck.HoveredIndex;

			if (hoveredIndex < 0 || hoveredIndex == _tabHeadersDeck.SelectedIndex || hoveredIndex >= _tabHeadersDeck.Count)
				return;

			_tabHeadersDeck.SelectedIndex = hoveredIndex;

			var card = DraggedCard;
			StopDragging();
			dragCard(card);
		}

		private void deckZoneChanged(TabHeaderControl sender, int selected)
		{
			bool isSide = Equals(_tabHeadersDeck.SelectedTabId, 1);

			if (_deckModel.IsSide != isSide)
			{
				beginRestoreSettings();
				_deckModel.SetIsSide(isSide, _cardRepo);
				endRestoreSettings();

				refilterChangedDeck(listChanged: true, touchedChanged: false, card: null);
			}
		}

		private void legalityFilterChanged()
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroupLegality))
				setFilterManagerState(FilterGroupLegality, FilterValueState.Required);

			RunRefilterTask();
			historyUpdate();
			_viewCards.Focus();
		}

		private void languageChanged()
		{
			if (restoringSettings())
				return;

			if (_sortSubsystem.IsLanguageDependent || (isFilterGroupEnabled(FilterGroupFind) && isSearchStringApplied()))
			{
				if (_sortSubsystem.IsLanguageDependent)
					_sortSubsystem.Invalidate();

				if (isFilterGroupEnabled(FilterGroupFind) && isSearchStringApplied())
					_searchStringSubsystem.ApplyFind();

				RunRefilterTask();
			}
			else
			{
				_viewCards.RefreshData();
				_viewCards.Invalidate();
			}

			historyUpdate();
		}

		private void searchStringApplied()
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroupFind))
				setFilterManagerState(FilterGroupFind, FilterValueState.Required);

			resetTouchedCard();

			RunRefilterTask();
			historyUpdate();
		}

		private void searchStringChanged()
		{
			updateFormStatus();
		}

		private void sortChanged()
		{
			if (restoringSettings())
				return;

			RunRefilterTask();
			historyUpdate();
		}



		private void setupCheckButtonImages()
		{
			const float sat = 2f;

			_buttonSubsystem.SetupButton(_buttonShowDuplicates,
				new ButtonImages(
					Resources.Clone24.TransformColors(saturation: 0f),
					Resources.Clone24,
					Resources.Clone24.TransformColors(saturation: 0.9f),
					Resources.Clone24.TransformColors(saturation: 2.5f)));

			_buttonSubsystem.SetupButton(_buttonShowProhibit,
				new ButtonImages(
					Resources.hidden24,
					Resources.shown24,
					Resources.hidden24.TransformColors(sat, 1.2f),
					Resources.shown24.TransformColors(sat, 1.2f)));

			_buttonSubsystem.SetupButton(_buttonExcludeManaAbility,
				new ButtonImages(
					Resources.include_plus24e,
					Resources.exclude_minus24t,
					Resources.include_plus24e.TransformColors(sat, 1f),
					Resources.exclude_minus24t.TransformColors(sat, 1.2f)));

			_buttonSubsystem.SetupButton(_buttonExcludeManaCost,
				new ButtonImages(
					Resources.include_plus24e,
					Resources.exclude_minus24t,
					Resources.include_plus24e.TransformColors(sat, 1f),
					Resources.exclude_minus24t.TransformColors(sat, 1.2f)));
		}
	}
}
