using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Ui;
using ButtonBase = Mtgdb.Controls.ButtonBase;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void subscribeToEvents()
		{
			Load += formLoad;

			_buttonExcludeManaCost.MouseDown += resetExcludeManaCost;
			_buttonExcludeManaAbility.MouseDown += resetExcludeManaAbility;
			_buttonShowDuplicates.CheckedChanged += showDuplicatesCheckedChanged;

			_dragging.SubscribeToEvents();

			_drawing.SetupDrawingCardEvent();
			_dragging.SetupDrawingDraggingMarkEvent();


			// After _deckDraggingSubsystem.SubscribeToEvents(), so that it would recapture the click on drag-n-drop before
			_deckEditorSubsystem.SubscribeToEvents();
			// After _deckEditingSubsystem, to show zoom before moving the card

			_legality.SubscribeToEvents();
			_legality.FilterChanged += legalityFilterChanged;

			_deckEditor.DeckChanged += deckChanged;
			_collectionEditor.CollectionChanged += collectionChanged;

			_cardSort.SubscribeToEvents();
			_cardSort.SortChanged += cardSortChanged;

			_buttonExcludeManaAbility.CheckedChanged += excludeManaAbilityChanged;
			_buttonExcludeManaCost.CheckedChanged += excludeManaCostChanged;
			_buttonShowProhibit.CheckedChanged += showProhibitChanged;

			_tabHeadersDeck.SelectedIndexChanged += deckZoneChanged;
			_tabHeadersDeck.Click += deckZoneClick;

			_deckZones.SubscribeEvents();

			_cardSearcher.IndexingProgress += indexingProgress;
			_cardSearcher.Spellchecker.IndexingProgress += indexingProgress;
			_deckSearcher.IndexingProgress += indexingProgress;
			_deckSearcher.Spellchecker.IndexingProgress += indexingProgress;

			_cardSearcher.Loaded += cardSearcherLoaded;
			_cardSearcher.Disposed += cardSearcherDisposed;

			_keywordSearcher.Loaded += keywordSearcherLoaded;
			_keywordSearcher.LoadingProgress += keywordSearcherLoadingProgress;

			_viewCards.CardIndexChanged += gridScrolled;
			_viewDeck.CardIndexChanged += gridScrolled;

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged += quickFiltersChanged;

			_filterManager.StateChanged += quickFilterManagerChanged;

			Application.ApplicationExit += applicationExit;

			_copyPaste.SubscribeToEvents();

			_buttonSampleHandNew.Pressed += sampleHandNew;
			_buttonSampleHandMulligan.Pressed += sampleHandMulligan;
			_buttonSampleHandDraw.Pressed += sampleHandDraw;

			_buttonHideDeck.CheckedChanged += buttonHideDeckChanged;
			_buttonShowScrollCards.CheckedChanged += buttonShowScrollChanged;
			_buttonShowScrollDeck.CheckedChanged += buttonShowScrollChanged;
			_buttonShowPartialCards.CheckedChanged += buttonPartialCardsChanged;
			_buttonShowText.CheckedChanged += buttonHideTextChanged;
			_buttonResetFilters.Pressed += resetFiltersClick;

			_layoutRight.SizeChanged += rightLayoutChanged;

			_history.Loaded += historyLoaded;

			_searchSubsystem.SubscribeToEvents();

			SizeChanged += sizeChanged;
			PreviewKeyDown += previewKeyDown;

			_deckSearcher.BeginLoad += beginUpdateDeckIndex;

			_deckListControl.Scrolled += deckListScrolled;
			_deckListControl.Refreshed += deckListRefreshed;
			_deckListControl.DeckOpened += deckListOpenedDeck;
			_deckListControl.DeckRenamed += deckListRenamedDeck;
			_deckListControl.FilterByDeckModeChanged += filterByDeckModeChanged;
			_deckListControl.DeckAdded += deckListAdded;
			_deckListControl.DeckTransformed += deckListTransformed;

			_formZoom.SettingsChanged += zoomSettingsChanged;

			_viewCards.CardCreating += cardCreating;
			_viewDeck.CardCreating += cardCreating;

			_menuSearchExamples.QueryClicked += searchExampleClicked;

			Dpi.BeforeChanged += beforeDpiChanged;
			Dpi.AfterChanged += afterDpiChanged;
		}

		private void unsubscribeFromEvents()
		{
			Load -= formLoad;

			_buttonExcludeManaCost.MouseDown -= resetExcludeManaCost;
			_buttonExcludeManaAbility.MouseDown -= resetExcludeManaAbility;
			_buttonShowDuplicates.CheckedChanged -= showDuplicatesCheckedChanged;

			_dragging.UnsubscribeFromEvents();

			_legality.FilterChanged -= legalityFilterChanged;
			_deckEditor.DeckChanged -= deckChanged;
			_collectionEditor.CollectionChanged -= collectionChanged;

			_cardSort.UnsubscribeFromEvents();
			_cardSort.SortChanged -= cardSortChanged;

			_buttonExcludeManaAbility.CheckedChanged -= excludeManaAbilityChanged;
			_buttonExcludeManaCost.CheckedChanged -= excludeManaCostChanged;
			_buttonShowProhibit.CheckedChanged -= showProhibitChanged;

			_tabHeadersDeck.SelectedIndexChanged -= deckZoneChanged;
			_tabHeadersDeck.Click -= deckZoneClick;
			_deckZones.UnsubscribeEvents();

			_cardSearcher.IndexingProgress -= indexingProgress;
			_cardSearcher.Spellchecker.IndexingProgress -= indexingProgress;
			_deckSearcher.IndexingProgress -= indexingProgress;
			_deckSearcher.Spellchecker.IndexingProgress -= indexingProgress;

			_cardSearcher.Loaded -= cardSearcherLoaded;
			_cardSearcher.Disposed -= cardSearcherDisposed;

			_keywordSearcher.Loaded -= keywordSearcherLoaded;
			_keywordSearcher.LoadingProgress -= keywordSearcherLoadingProgress;

			_viewCards.CardIndexChanged -= gridScrolled;
			_viewDeck.CardIndexChanged -= gridScrolled;

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged -= quickFiltersChanged;

			_filterManager.StateChanged -= quickFilterManagerChanged;

			Application.ApplicationExit -= applicationExit;

			_copyPaste.UnsubscribeFromEvents();


			_buttonSampleHandNew.Pressed -= sampleHandNew;
			_buttonSampleHandMulligan.Pressed -= sampleHandMulligan;
			_buttonSampleHandDraw.Pressed -= sampleHandDraw;

			_buttonHideDeck.CheckedChanged -= buttonHideDeckChanged;
			_buttonShowScrollCards.CheckedChanged -= buttonShowScrollChanged;
			_buttonShowScrollDeck.CheckedChanged -= buttonShowScrollChanged;
			_buttonShowPartialCards.CheckedChanged -= buttonPartialCardsChanged;
			_buttonShowText.CheckedChanged -= buttonHideTextChanged;
			_buttonResetFilters.Pressed -= resetFiltersClick;

			_layoutRight.SizeChanged -= rightLayoutChanged;
			_history.Loaded -= historyLoaded;

			_searchSubsystem.UnsubscribeFromEvents();

			SizeChanged -= sizeChanged;
			PreviewKeyDown -= previewKeyDown;

			_deckSearcher.BeginLoad -= beginUpdateDeckIndex;

			_deckListControl.Scrolled -= deckListScrolled;
			_deckListControl.Refreshed -= deckListRefreshed;
			_deckListControl.DeckOpened -= deckListOpenedDeck;
			_deckListControl.DeckRenamed -= deckListRenamedDeck;
			_deckListControl.FilterByDeckModeChanged -= filterByDeckModeChanged;
			_deckListControl.DeckAdded -= deckListAdded;
			_deckListControl.DeckTransformed -= deckListTransformed;

			_formZoom.SettingsChanged -= zoomSettingsChanged;

			_viewCards.CardCreating -= cardCreating;
			_viewDeck.CardCreating -= cardCreating;

			_menuSearchExamples.QueryClicked -= searchExampleClicked;

			Dpi.BeforeChanged -= beforeDpiChanged;
			Dpi.AfterChanged -= afterDpiChanged;
		}

		private void applicationExit(object sender, EventArgs e) =>
			Shutdown();

		private void cardCreating(object view, LayoutControl probeCard) =>
			((CardLayoutControlBase) probeCard).Ui = () => _formRoot.UiModel;

		private void formLoad(object sender, EventArgs e)
		{
			startThreads();
			subscribeCardRepoEvents();
		}

		private void subscribeCardRepoEvents()
		{
			_app.CancellationToken.When(_cardRepo.IsLocalizationLoadingComplete)
				.Run(localizationLoadingComplete);
			_app.CancellationToken.When(_cardRepo.IsLoadingComplete)
				.Run(repoLoadingComplete);
			_cardRepo.SetAdded += cardRepoSetAdded;
		}

		private void unsubscribeCardRepoEvents()
		{
			_cardRepo.SetAdded -= cardRepoSetAdded;
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
				_searchSubsystem.Apply();
				_deckEditor.LoadDeck(_cardRepo);
				_cardSort.Invalidate();

				endRestoreSettings();

				runRefilterTask();
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
					!string.IsNullOrEmpty(_searchSubsystem.AppliedText))
				{
					beginRestoreSettings();
					_searchSubsystem.Apply();
					endRestoreSettings();
					runRefilterTask();
				}
			});
		}

		private void cardSearcherLoaded()
		{
			this.Invoke(delegate
			{
				if (!string.IsNullOrEmpty(_searchSubsystem.AppliedText))
				{
					beginRestoreSettings();
					_searchSubsystem.Apply();
					endRestoreSettings();

					runRefilterTask();
				}
			});
		}

		private void cardSearcherDisposed() =>
			this.Invoke(updateFormStatus);

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
			runRefilterTask();
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

			_viewCards.LayoutOptions.HideScroll =
				_viewDeck.LayoutOptions.HideScroll =
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

			setTextualFieldsVisible(_viewCards, _buttonShowText.Checked);
			_viewCards.RefreshData();

			if (!restoringSettings())
				historyUpdate();
		}

		private void setTextualFieldsVisible(LayoutViewControl view, bool value)
		{
			var layout = value ? typeof(CardLayout) : typeof(DeckLayout);
			view.LayoutControlType = layout;

			var interval = view.LayoutOptions.CardInterval;
			if (value)
				view.LayoutOptions.CardInterval = new Size(interval.Height * 2, interval.Height);
			else
				view.LayoutOptions.CardInterval = new Size(interval.Height, interval.Height);

			var threshold = view.LayoutOptions.PartialCardsThreshold;

			view.LayoutOptions.PartialCardsThreshold = new Size(
				view.CardSize.Width * threshold.Height / view.CardSize.Height,
				threshold.Height);
		}

		private void buttonPartialCardsChanged(object sender, EventArgs e)
		{
			updateFormSettings();

			_viewCards.LayoutOptions.AllowPartialCards =
				_viewDeck.LayoutOptions.AllowPartialCards =
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


		private void gridScrolled(object sender)
		{
			if (restoringSettings())
				return;

			if (sender == _viewCards)
			{
				_imagePreloading.Reset();

				if (_cardRepo.IsLoadingComplete.Signaled)
					_history.Current.SearchResultScroll = _viewCards.CardIndex;
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
			runRefilterTask();
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
			runRefilterTask();
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

				setFilterManagerState(FilterGroup.Deck,
					mustBeEnabled
						? FilterValueState.Required
						: FilterValueState.Ignored);

				endRestoreSettings();
			}

			resetTouchedCard();
			runRefilterTask();
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
			runRefilterTask();
			historyUpdate();
		}



		private void deckZoneChanged(TabHeaderControl sender, int selected)
		{
			updateDeckVisibility();

			var zone = _deckZones.DeckZone;

			if (_deckEditor.CurrentZone != zone)
			{
				beginRestoreSettings();
				_deckEditor.SetZone(zone, _cardRepo);
				var scroll = _deckZones.GetLastScroll(zone);
				if (scroll.HasValue)
					_viewDeck.ScrollTo(scroll.Value);
				endRestoreSettings();
			}

			if (isFilterGroupEnabled(FilterGroup.Deck))
				runRefilterTask();

			updateFormStatus();
			updateShowSampleHandButtons();
		}

		private void updateDeckVisibility()
		{
			_layoutMain.SuspendLayout();

			_deckListControl.Visible = IsDeckListSelected && !_buttonHideDeck.Checked;
			_viewDeck.Visible = !IsDeckListSelected && !_buttonHideDeck.Checked;

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

			if (isActualDeckChange)
			{
				if (!restoringSettings())
					historyUpdate();

				_deckListControl.DeckChanged(_deckEditor.Snapshot());
			}

			if (!restoringSettings())
				updateFormStatus();
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card)
		{
			updateViewCards(listChanged, card, FilterGroup.Collection, touchedChanged: false);

			updateViewDeck(
				listChanged: false,
				countChanged: false,
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



		private void updateViewCards(bool listChanged, Card card, FilterGroup relatedFilterGroup, bool touchedChanged)
		{
			if (touchedChanged || listChanged && isFilterGroupEnabled(relatedFilterGroup))
			{
				if (restoringSettings())
					return;

				var touchedCard = _deckEditor.TouchedCard;

				if (touchedChanged && touchedCard != null)
					runRefilterTask(() => _viewCards.ScrollTo(touchedCard));
				else
					runRefilterTask();

				return;
			}

			if (card != null)
			{
				_deckEditor.Deck.NamesakeIds(card)
					.Select(id => _cardRepo.CardsById[id])
					.Append(card)
					.Distinct()
					.ForEach(_viewCards.InvalidateCard);
			}
			else
				_viewCards.Invalidate();
		}

		private void updateViewDeck(bool listChanged, bool countChanged, Card card, bool touchedChanged)
		{
			if (listChanged)
				_viewDeck.RefreshData();
			else if (card != null)
			{
				_deckEditor.Deck.NamesakeIds(card)
					.Select(id => _cardRepo.CardsById[id])
					.ForEach(_viewDeck.InvalidateCard);
			}
			else
				_viewDeck.Invalidate();

			if ((countChanged || touchedChanged) && _deckEditor.TouchedCard != null)
				_viewDeck.ScrollTo(_deckEditor.TouchedCard);
		}

		private void deckZoneClick(object sender, EventArgs e)
		{
			if (_buttonHideDeck.Checked)
				_buttonHideDeck.Checked = false;
		}

		private void legalityFilterChanged()
		{
			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroup.Legality))
				setFilterManagerState(FilterGroup.Legality, FilterValueState.Required);

			runRefilterTask();
			historyUpdate();
		}

		public void SetPanelVisibility(UiConfig config)
		{
			_panelFilters.Visible = config.ShowTopPanel;
			if (config.ShowTopPanel)
			{
				_searchBar.VisibleBorders |= AnchorStyles.Top;
				_dropdownLegality.VisibleBorders |= AnchorStyles.Top;
			}
			else
			{
				_searchBar.VisibleBorders &= ~AnchorStyles.Top;
				_dropdownLegality.VisibleBorders &= ~AnchorStyles.Top;
			}

			_layoutRight.Visible = config.ShowRightPanel;

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
					_searchSubsystem.Apply();

				endRestoreSettings();

				runRefilterTask();
			}
			else
				_viewCards.RefreshData();

			historyUpdate();
		}

		private void searchSubsystemStringApplied()
		{
			beginRestoreSettings();
			_cardSort.Invalidate();
			endRestoreSettings();

			if (restoringSettings())
				return;

			if (!isFilterGroupEnabled(FilterGroup.Find))
				setFilterManagerState(FilterGroup.Find, FilterValueState.Required);

			resetTouchedCard();

			runRefilterTask();
			historyUpdate();
		}

		private void searchSubsystemStringChanged()
		{
			updateFormStatus();
		}

		private void cardSortChanged()
		{
			if (restoringSettings())
				return;

			runRefilterTask();
			historyUpdate();
		}



		private void sampleHandNew(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete.Signaled)
				return;

			_deckEditor.NewSampleHand(_cardRepo);
		}

		private void sampleHandMulligan(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete.Signaled)
				return;

			_deckEditor.Mulligan(_cardRepo);
		}

		private void sampleHandDraw(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete.Signaled)
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

			foreach (var filterControl in _quickFilterControls.Append(_filterManager))
				modified |= filterControl.Reset();

			modified |= _legality.Reset();
			modified |= _searchSubsystem.ResetText();
			modified |= resetShowDuplicates();

			endRestoreSettings();

			if (modified)
			{
				resetTouchedCard();
				runRefilterTask();
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
			_searchSubsystem.AppliedText = query;
			_searchSubsystem.Apply();
			_popupSearchExamples.ClosePopup();
		}
	}
}
