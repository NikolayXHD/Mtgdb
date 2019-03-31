using System;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Ui;
using ButtonBase = Mtgdb.Controls.ButtonBase;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void applicationExit(object sender, EventArgs e) =>
			Shutdown();

		private void formLoad(object sender, EventArgs e)
		{
			startThreads();
			subscribeCardRepoEvents();
		}

		private void subscribeCardRepoEvents()
		{
			_cardRepo.SetAdded += cardRepoSetAdded;
			_cardRepo.LocalizationLoadingComplete += localizationLoadingComplete;

			if (_cardRepo.IsLoadingComplete)
				repoLoadingComplete();
			else
				_cardRepo.LoadingComplete += repoLoadingComplete;
		}

		private void unsubscribeCardRepoEvents()
		{
			_cardRepo.SetAdded -= cardRepoSetAdded;
			_cardRepo.LoadingComplete -= repoLoadingComplete;
			_cardRepo.LocalizationLoadingComplete -= localizationLoadingComplete;
		}

		public void Shutdown()
		{
			stopThreads();
			unsubscribeFromEvents();
			unsubscribeCardRepoEvents();
			_deckListControl.UnsubscribeFromEvents();
		}



		private void cardRepoSetAdded()
		{
			if (_cardRepo.SetsByCode.Count % 9 == 0)
				this.Invoke(updateFormStatus);
		}

		private void repoLoadingComplete()
		{
			this.Invoke(delegate
			{
				beginRestoreSettings();

				updateShowSampleHandButtons();
				_cardSearch.Apply();
				_deckEditor.LoadDeck(_cardRepo);
				_cardSort.Invalidate();

				endRestoreSettings();

				RunRefilterTask();
			});
		}

		private void localizationLoadingComplete()
		{
			this.Invoke(delegate
			{
				if (_cardSort.IsLanguageDependent)
				{
					beginRestoreSettings();
					_cardSort.Invalidate();
					endRestoreSettings();
				}

				if (_formRoot.UiModel.LanguageController.Language != CardLocalization.DefaultLanguage &&
					!string.IsNullOrEmpty(_cardSearch.AppliedText))
				{
					beginRestoreSettings();
					_cardSearch.Apply();
					endRestoreSettings();
					RunRefilterTask();
				}
			});
		}

		private void cardSearcherLoaded()
		{
			this.Invoke(delegate
			{
				if (!string.IsNullOrEmpty(_cardSearch.AppliedText))
				{
					beginRestoreSettings();
					_cardSearch.Apply();
					endRestoreSettings();

					RunRefilterTask();
				}
			});
		}

		private void cardSearcherDisposed()
		{
			this.Invoke(updateFormStatus);
		}

		private void indexingProgress() =>
			this.Invoke(updateFormStatus);



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



		private void buttonHideDeckChanged(object sender, EventArgs eventArgs)
		{
			updateFormSettings();

			updateDeckVisibility();

			if (!restoringSettings())
				historyUpdate();
		}

		private void buttonShowScrollChanged(object sender, EventArgs eventArgs)
		{
			if (_updatingButtonHideScroll)
				return;

			_updatingButtonHideScroll = true;

			var value = ((ButtonBase) sender).Checked;

			_layoutViewCards.LayoutOptions.HideScroll =
				_layoutViewDeck.LayoutOptions.HideScroll =
					_deckListControl.HideScroll = !value;

			_buttonShowScrollDeck.Checked =
				_buttonShowScrollCards.Checked = value;

			updateFormSettings();

			if (!restoringSettings())
				historyUpdate();

			_updatingButtonHideScroll = false;
		}

		private void buttonHideTextChanged(object sender, EventArgs e)
		{
			updateFormSettings();

			_viewCards.TextualFieldsVisible =
				_buttonShowText.Checked;

			_viewCards.RefreshData();

			if (!restoringSettings())
				historyUpdate();
		}

		private void buttonPartialCardsChanged(object sender, EventArgs e)
		{
			updateFormSettings();

			_viewCards.AllowPartialCards =
				_viewDeck.AllowPartialCards =
					_deckListControl.AllowPartialCard =
						_buttonShowPartialCards.Checked;

			_viewCards.RefreshData();
			_viewDeck.RefreshData();

			if (!restoringSettings())
				historyUpdate();
		}

		private void rightLayoutChanged(object sender, EventArgs e)
		{
			setRightPanelsWidth();
		}

		private void setRightPanelsWidth()
		{
			setColumnWidth(_panelRightCost);
			setColumnWidth(_panelRightNarrow);
			setColumnWidth(_panelRightManaCost);
		}

		private static void setColumnWidth(Control panel)
		{
			var tableLayout = (TableLayoutPanel) panel.Parent;
			var cell = tableLayout.GetCellPosition(panel);

			var preferredSize = panel.GetPreferredSize(new Size(int.MaxValue, panel.Height));
			int preferredWidth = preferredSize.Width + panel.Margin.Right + panel.Margin.Left;

			tableLayout.ColumnStyles[cell.Column].Width = preferredWidth;
		}

		private static void setRowHeight(Control panel, Size size)
		{
			var tableLayout = (TableLayoutPanel) panel.Parent;
			var cell = tableLayout.GetCellPosition(panel);
			tableLayout.RowStyles[cell.Row].Height = size.Height;
		}


		private void gridScrolled(MtgLayoutView sender)
		{
			if (restoringSettings())
				return;

			if (sender == _viewCards)
			{
				_imagePreloading.Reset();

				if (_cardRepo.IsLoadingComplete)
					_history.Current.SearchResultScroll = _viewCards.VisibleRecordIndex;
			}

			updateFormStatus();
		}

		private void deckListScrolled(object sender) =>
			updateFormStatus();

		private void deckListRefreshed(object sender)
		{
			if (!isFilterGroupEnabled(FilterGroup.Deck))
			{
				updateFormStatus();
				return;
			}

			resetTouchedCard();
			RunRefilterTask();
		}

		private void deckListOpenedDeck(object sender, Deck deck, bool inNewTab)
		{
			if (inNewTab)
				_formRoot.OpenDeckInNewTab(deck);
			else
				LoadDeck(deck);
		}



		private void deckListRenamedDeck(object sender, Deck deck)
		{
			_deckEditor.DeckName = deck.Name;
			updateFormStatus();
		}

		private void deckListAdded(object sender)
		{
			if (_deckListControl.DecksAddedCount % 89 == 0)
				this.Invoke(updateFormStatus);
		}

		private void deckListTransformed(object sender) =>
			this.Invoke(updateFormStatus);

		private void updateFilterByDeckMode()
		{
			if (isFilterGroupEnabled(FilterGroup.Deck))
			{
				if (_deckListControl.FilterByDeckMode == FilterByDeckMode.Ignored)
					_deckListControl.FilterByDeckMode = FilterByDeckMode.CurrentDeck;
			}
			else
				_deckListControl.FilterByDeckMode = FilterByDeckMode.Ignored;
		}


		private void quickFiltersChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroup.Buttons))
				setFilterManagerState(FilterGroup.Buttons, FilterValueState.Required);

			updateTerms();

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}

		private void filterByDeckModeChanged(object sender)
		{
			if (restoringSettings())
				return;

			bool filterGroupEnabled = isFilterGroupEnabled(FilterGroup.Deck);
			bool mustBeEnabled = _deckListControl.FilterByDeckMode != FilterByDeckMode.Ignored;

			if (mustBeEnabled != filterGroupEnabled)
			{
				beginRestoreSettings();

				if (mustBeEnabled)
					setFilterManagerState(FilterGroup.Deck, FilterValueState.Required);
				else
					setFilterManagerState(FilterGroup.Deck, FilterValueState.Ignored);

				endRestoreSettings();
			}

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}

		private void quickFilterManagerChanged(object sender, EventArgs e)
		{
			if (restoringSettings())
				return;

			bool filterGroupEnabled = isFilterGroupEnabled(FilterGroup.Deck);
			bool mustBeEnabled = _deckListControl.FilterByDeckMode != FilterByDeckMode.Ignored;

			if (mustBeEnabled != filterGroupEnabled)
			{
				beginRestoreSettings();

				if (filterGroupEnabled)
					_deckListControl.FilterByDeckMode = FilterByDeckMode.CurrentDeck;
				else
					_deckListControl.FilterByDeckMode = FilterByDeckMode.Ignored;

				endRestoreSettings();
			}

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}



		private void deckZoneChanged(TabHeaderControl sender, int selected)
		{
			updateDeckVisibility();

			var zone = DeckZone;

			if (_deckEditor.CurrentZone != zone)
			{
				beginRestoreSettings();
				_deckEditor.SetZone(zone, _cardRepo);
				endRestoreSettings();
			}

			if (isFilterGroupEnabled(FilterGroup.Deck))
				RunRefilterTask();

			updateFormStatus();
			updateShowSampleHandButtons();
		}

		private void updateDeckVisibility()
		{
			_layoutMain.SuspendLayout();

			_deckListControl.Visible = IsDeckListSelected && !_buttonHideDeck.Checked;
			_layoutViewDeck.Visible = !IsDeckListSelected && !_buttonHideDeck.Checked;

			_layoutMain.ResumeLayout(false);
			_layoutMain.PerformLayout();
		}


		private void deckChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged, Zone? zone, bool changeTerminatesBatch)
		{
			if (zone == _deckEditor.CurrentZone)
			{
				updateViewCards(listChanged, card, FilterGroup.Deck, touchedChanged);
				updateViewDeck(listChanged, countChanged, card, touchedChanged);
			}

			bool isActualDeckChange = zone != Zone.SampleHand &&
				(countChanged || listChanged) && changeTerminatesBatch;

			if (!restoringSettings())
				if (isActualDeckChange)
					historyUpdate();

			if (isActualDeckChange)
				_deckListControl.DeckChanged(_deckEditor.Snapshot());

			if (!restoringSettings())
				updateFormStatus();
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card)
		{
			updateViewCards(listChanged, card, FilterGroup.Collection, touchedChanged: false);

			updateViewDeck(
				listChanged: false,
				countChanged: true,
				card: card,
				touchedChanged: false);

			if (!_isTabSelected)
				return;

			bool isActualCollectionChange = countChanged || listChanged;

			if (!restoringSettings())
				if (isActualCollectionChange)
					historyUpdate();

			if (!restoringSettings())
				updateFormStatus();
		}



		private void updateViewCards(bool listChanged, Card card, FilterGroup changeRelatedFilterGroup, bool touchedChanged)
		{
			if (touchedChanged || listChanged && isFilterGroupEnabled(changeRelatedFilterGroup))
			{
				if (restoringSettings())
					return;

				var touchedCard = _deckEditor.TouchedCard;

				if (touchedChanged && touchedCard != null)
					RunRefilterTask(() => _scroll.EnsureCardVisibility(touchedCard, _viewCards));
				else
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

		private void updateViewDeck(bool listChanged, bool countChanged, Card card, bool touchedChanged)
		{
			if (listChanged)
				_viewDeck.RefreshData();

			if (touchedChanged && _deckEditor.TouchedCard != null)
				_scroll.EnsureCardVisibility(_deckEditor.TouchedCard, _viewDeck);

			if (listChanged)
				_viewDeck.Invalidate();
			else if (touchedChanged || countChanged)
				_viewDeck.Invalidate();
			else if (card != null)
				_viewDeck.InvalidateCard(card);
		}

		private void deckZoneClick(object sender, EventArgs e)
		{
			if (_buttonHideDeck.Checked)
				_buttonHideDeck.Checked = false;
		}

		private void deckZoneHover(object sender, EventArgs e)
		{
			if (!IsDraggingCard)
				return;

			var hoveredIndex = _tabHeadersDeck.HoveredIndex;

			if (hoveredIndex < 0 || hoveredIndex == _tabHeadersDeck.SelectedIndex || hoveredIndex > MaxZoneIndex)
				return;

			_tabHeadersDeck.SelectedIndex = hoveredIndex;
		}

		private void deckZoneDrag(object sender, DragEventArgs e)
		{
			var location = _tabHeadersDeck.PointToClient(new Point(e.X, e.Y));

			_tabHeadersDeck.GetTabIndex(location, out int hoveredIndex, out _);

			if (hoveredIndex < 0 || hoveredIndex == _tabHeadersDeck.SelectedIndex || hoveredIndex >= MaxZoneIndex)
				return;

			_tabHeadersDeck.SelectedIndex = hoveredIndex;
		}



		private void legalityFilterChanged()
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroup.Legality))
				setFilterManagerState(FilterGroup.Legality, FilterValueState.Required);

			RunRefilterTask();
			historyUpdate();
		}

		public void SetPanelVisibility(UiConfig config)
		{
			_layoutRight.Visible = config.ShowRightPanel;

			_panelFilters.Visible = config.ShowTopPanel;

			_panelMenu.Visible =
				_buttonShowScrollCards.Visible =
					_labelStatusScrollCards.Visible = config.ShowSearchBar;
		}

		private void languageChanged()
		{
			if (!_isTabSelected)
				return;

			if (restoringSettings())
				return;

			if (_cardSort.IsLanguageDependent || isFilterGroupEnabled(FilterGroup.Find) && isSearchStringApplied())
			{
				beginRestoreSettings();

				if (_cardSort.IsLanguageDependent)
					_cardSort.Invalidate();

				if (isFilterGroupEnabled(FilterGroup.Find) && isSearchStringApplied())
					_cardSearch.Apply();

				endRestoreSettings();

				RunRefilterTask();
			}
			else
			{
				_viewCards.RefreshData();
				_viewCards.Invalidate();
			}

			historyUpdate();
		}

		private void cardSearchStringApplied()
		{
			beginRestoreSettings();
			_cardSort.Invalidate();
			endRestoreSettings();

			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroup.Find))
				setFilterManagerState(FilterGroup.Find, FilterValueState.Required);

			resetTouchedCard();

			RunRefilterTask();
			historyUpdate();
		}

		private void cardSearchStringChanged()
		{
			updateFormStatus();
		}

		private void cardSortChanged()
		{
			if (restoringSettings())
				return;

			RunRefilterTask();
			historyUpdate();
		}



		public void ScheduleOpeningDeck(Deck deck) =>
			_requiredDeck = deck;

		private void hideSampleHand()
		{
			if (DeckZone == Zone.SampleHand)
				DeckZone = Zone.Main;
		}



		private void sampleHandNew(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			_deckEditor.NewSampleHand(_cardRepo);
		}

		private void sampleHandMulligan(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			_deckEditor.Mulligan(_cardRepo);
		}

		private void sampleHandDraw(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			_deckEditor.Draw(_cardRepo);
		}



		private void historyLoaded() =>
			updateFormStatus();

		private void sizeChanged(object sender, EventArgs e) =>
			_layoutRight.PerformLayout();

		private static void previewKeyDown(object sender, PreviewKeyDownEventArgs e) =>
			e.IsInputKey = true;

		private void beginUpdateDeckIndex() =>
			this.Invoke(updateFormStatus);

		private void zoomSettingsChanged()
		{
			updateFormSettings();

			if (!restoringSettings())
				_history.Current.Zoom = _formZoom.Settings;
		}

		private void resetFiltersClick(object sender, EventArgs e)
		{
			bool modified = false;

			beginRestoreSettings();

			foreach (var filterControl in _quickFilterControls.Append(FilterManager))
				modified |= filterControl.Reset();

			modified |= _legality.Reset();
			modified |= _cardSearch.ResetText();
			modified |= resetShowDuplicates();

			endRestoreSettings();

			if (modified)
			{
				resetTouchedCard();
				RunRefilterTask();
				historyUpdate();
			}
		}

		private bool resetShowDuplicates()
		{
			if (_buttonShowDuplicates.Checked)
				return false;

			_buttonShowDuplicates.Checked = true;
			return true;
		}

		private void searchExampleClicked(string query)
		{
			_cardSearch.AppliedText = query;
			_cardSearch.Apply();
			_popupSearchExamples.ClosePopup();
		}

		private void deckTabsResized(object sender, EventArgs e)
		{
			_panelDeckTabsContainer.Size = new Size(
				Math.Max(_tabHeadersDeck.Width, _panelDeckTabsContainer.Width),
				_tabHeadersDeck.Height);
		}
	}
}