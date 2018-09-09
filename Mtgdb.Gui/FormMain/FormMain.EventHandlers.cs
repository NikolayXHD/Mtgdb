using System;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;
using Mtgdb.Ui;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		private void resetLayouts()
		{
			_layoutViewCards.LayoutControlType = _layoutViewCards.LayoutControlType;
			_layoutViewDeck.LayoutControlType = _layoutViewDeck.LayoutControlType;
		}

		private void applicationExit(object sender, EventArgs e)
		{
			shutdown();
		}

		private void formLoad(object sender, EventArgs e)
		{
			_searchEditor.Focus();

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

		private void formClosing(object sender, FormClosingEventArgs e)
		{
			shutdown();
		}

		private void shutdown()
		{
			stopThreads();
			unsubscribeFromEvents();
			unsubscribeCardRepoEvents();
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

		private void cardSearcherIndexingProgress() =>
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

		private void buttonHideScrollChanged(object sender, EventArgs eventArgs)
		{
			if (_updatingButtonHideScroll)
				return;

			_updatingButtonHideScroll = true;

			var value = ((CheckBox) sender).Checked;

			_layoutViewCards.LayoutOptions.HideScroll =
				_layoutViewDeck.LayoutOptions.HideScroll =
					_deckListControl.HideScroll =
						_buttonHideScrollDeck.Checked =
							_buttonHideScrollCards.Checked = value;

			updateFormSettings();

			if (!restoringSettings())
				historyUpdate();

			_updatingButtonHideScroll = false;
		}

		private void buttonHideTextChanged(object sender, EventArgs e)
		{
			updateFormSettings();

			_viewCards.TextualFieldsVisible =
				!_buttonHideText.Checked;

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
				!_buttonHidePartialCards.Checked;



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
			_viewCards.Focus();

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

			if (isActualCollectionChange)
				_deckListControl.CollectionChanged();

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
			_viewCards.Focus();
		}

		private void showFilterPanelsChanged()
		{
			if (!_isTabSelected)
				return;

			if (restoringSettings())
				return;

			applyShowFilterPanels();

			historyUpdate();
		}

		private void applyShowFilterPanels()
		{
			_layoutRight.Visible =
				_panelFilters.Visible =
					_panelMenu.Visible =
						_buttonHideScrollCards.Visible =
							_labelStatusScrollCards.Visible =
						_formRoot.ShowFilterPanels;
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



		private void setupCheckButtonImages()
		{
			const float sat = 2f;

			float sampleHandOpacity = 0.6f;

			_buttons.SetupButton(_buttonSampleHandNew,
				new ButtonImages(
					null,
					Resources.hand_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.hand_48,
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonSampleHandDraw,
				new ButtonImages(
					null,
					Resources.draw_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.draw_48,
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonSampleHandMulligan,
				new ButtonImages(
					null,
					Resources.mulligan_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.mulligan_48,
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonShowDuplicates,
				new ButtonImages(
					Resources.clone_48.TransformColors(saturation: 0f),
					Resources.clone_48,
					Resources.clone_48.TransformColors(saturation: 0.9f),
					Resources.clone_48.TransformColors(saturation: 2.5f),
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonShowProhibit,
				new ButtonImages(
					Resources.exclude_hidden_24,
					Resources.exclude_shown_24,
					Resources.exclude_hidden_24.TransformColors(sat, 1.2f),
					Resources.exclude_shown_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonExcludeManaAbility,
				new ButtonImages(
					Resources.include_plus_24,
					Resources.exclude_minus_24,
					Resources.include_plus_24.TransformColors(sat, 1f),
					Resources.exclude_minus_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonExcludeManaCost,
				new ButtonImages(
					Resources.include_plus_24,
					Resources.exclude_minus_24,
					Resources.include_plus_24.TransformColors(sat, 1f),
					Resources.exclude_minus_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonHideDeck,
				new ButtonImages(
					Resources.shown_40,
					Resources.hidden_40,
					Resources.shown_40.TransformColors(brightness: 0.1f),
					Resources.hidden_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			var scrollImages = new ButtonImages(
				Resources.scroll_shown_40,
				Resources.scroll_hidden_40,
				Resources.scroll_hidden_40.TransformColors(brightness: 1.05f),
				Resources.scroll_shown_40.TransformColors(brightness: 1.05f),
				areImagesDoubleSized: true);

			_buttons.SetupButton(_buttonHideScrollCards, scrollImages);
			_buttons.SetupButton(_buttonHideScrollDeck, scrollImages);

			_buttons.SetupButton(_buttonHidePartialCards,
				new ButtonImages(
					Resources.partial_card_enabled_40,
					Resources.partial_card_disabled_40,
					Resources.partial_card_enabled_40.TransformColors(brightness: 0.1f),
					Resources.partial_card_disabled_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonHideText,
				new ButtonImages(
					Resources.text_enabled_40,
					Resources.text_disabled_40,
					Resources.text_enabled_40.TransformColors(brightness: 0.1f),
					Resources.text_disabled_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			_buttons.SetupButton(_buttonSearchExamplesDropDown,
				new ButtonImages(
					null,
					Resources.book_40,
					null,
					Resources.book_40_hovered,
					areImagesDoubleSized: true));
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
	}
}