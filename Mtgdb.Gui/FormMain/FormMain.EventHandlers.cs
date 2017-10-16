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

			if (_requiredDeck != null)
			{
				_requiredDeck = null;
				historyUpdate();
			}

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
				_sortSubsystem.Invalidate();

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

				RunRefilterTask();
			});
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
			if (restoringSettings())
				return;

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



		private void deckZoneChanged(TabHeaderControl sender, int selected)
		{
			bool isSide = Equals(_tabHeadersDeck.SelectedTabId, 1);

			if (_deckModel.IsSide != isSide)
			{
				beginRestoreSettings();
				_deckModel.SetIsSide(isSide, _cardRepo);
				endRestoreSettings();

				updateViewCards(true, null, FilterGroupDeck, false);
				updateViewDeck(true, null, false);
				updateFormStatus();
			}
		}

		private void appendToDeck(Deck deck)
		{
			beginRestoreSettings();

			foreach (var cardId in deck.MainDeck.Order)
				_deckModel.Add(_cardRepo.CardsById[cardId], deck.MainDeck.Count[cardId]);

			foreach (var cardId in deck.SideDeck.Order)
				_deckModel.Add(_cardRepo.CardsById[cardId], deck.SideDeck.Count[cardId]);

			endRestoreSettings();

			updateViewCards(true, null, FilterGroupDeck, false);
			updateViewDeck(true, null, false);
			updateFormStatus();
		}

		private void loadDeck(Deck deck)
		{
			_historyModel.DeckFile = deck.File;
			_historyModel.DeckName = deck.Name;

			_deckModel.SetDeck(deck);
			_deckModel.LoadDeck(_cardRepo);
		}

		private void deckChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged)
		{
			updateViewCards(listChanged, card, FilterGroupDeck, touchedChanged);
			updateViewDeck(listChanged, card, touchedChanged);
			
			if (restoringSettings())
				return;

			if (countChanged || listChanged)
				historyUpdate();

			updateFormStatus();
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged)
		{
			updateViewCards(listChanged, card, FilterGroupDeck, touchedChanged);
			updateViewDeck(listChanged, card, touchedChanged);
			
			if (restoringSettings())
				return;

			if (countChanged || listChanged)
				historyUpdate();

			updateFormStatus();
		}

		private void updateViewCards(bool listChanged, Card card, int filterGroup, bool touchedChanged)
		{
			if (touchedChanged || listChanged && isFilterGroupEnabled(filterGroup))
			{
				if (!restoringSettings())
					RunRefilterTask();
			}
			else
			{
				if (card == null)
					_viewCards.Invalidate();
				else
					_viewCards.InvalidateCard(card);
			}
		}

		private void updateViewDeck(bool listChanged, Card card, bool touchedChanged)
		{
			if (listChanged)
			{
				_viewDeck.RefreshData();
				_viewDeck.Invalidate();
			}
			else if (touchedChanged)
				_viewDeck.Invalidate();
			else if (card != null)
				_viewDeck.InvalidateCard(card);
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
