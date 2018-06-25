using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;

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

			if (_cardRepo.IsPriceLoadingComplete)
				priceLoadingComplete();
			else
				_cardRepo.PriceLoadingComplete += priceLoadingComplete;
		}

		private void unsubscribeCardRepoEvents()
		{
			_cardRepo.SetAdded -= cardRepoSetAdded;
			_cardRepo.LoadingComplete -= repoLoadingComplete;
			_cardRepo.LocalizationLoadingComplete -= localizationLoadingComplete;
			_cardRepo.PriceLoadingComplete -= priceLoadingComplete;
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
			if (_cardRepo.SetsByCode.Count % 3 == 0)
				this.Invoke(delegate
				{
					updateFormStatus();
					Application.DoEvents();
				});
		}

		private void repoLoadingComplete()
		{
			this.Invoke(delegate
			{
				beginRestoreSettings();

				updateShowSampleHandButtons();
				_cardSearchSubsystem.Apply();
				_deckEditorModel.LoadDeck(_cardRepo);
				_sortSubsystem.Invalidate();

				endRestoreSettings();

				RunRefilterTask();
			});
		}

		private void localizationLoadingComplete()
		{
			this.Invoke(delegate
			{
				if (_sortSubsystem.IsLanguageDependent)
				{
					beginRestoreSettings();
					_sortSubsystem.Invalidate();
					endRestoreSettings();
				}

				if (_formRoot.UiModel.LanguageController.Language != CardLocalization.DefaultLanguage &&
					!string.IsNullOrEmpty(_cardSearchSubsystem.AppliedText))
				{
					beginRestoreSettings();
					_cardSearchSubsystem.Apply();
					endRestoreSettings();
					RunRefilterTask();
				}
			});
		}

		private void priceLoadingComplete()
		{
			this.Invoke(delegate
			{
				_deckListControl.PriceLoaded();
			});
		}

		private void cardSearcherLoaded()
		{
			this.Invoke(delegate
			{
				if (!string.IsNullOrEmpty(_cardSearchSubsystem.AppliedText))
				{
					beginRestoreSettings();
					_cardSearchSubsystem.Apply();
					endRestoreSettings();

					RunRefilterTask();
				}
			});
		}

		private void cardSearcherDisposed()
		{
			this.Invoke(updateFormStatus);
		}

		private void cardSearcherIndexingProgress()
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



		private void buttonHideDeckChanged(object sender, EventArgs eventArgs)
		{
			updateFormSettings();

			updateDeckVisibility();

			if (!restoringSettings())
				historyUpdate();
		}

		private void buttonHideScrollChanged(object sender, EventArgs eventArgs)
		{
			updateFormSettings();

			_layoutViewCards.LayoutOptions.HideScroll =
				_layoutViewDeck.LayoutOptions.HideScroll =
					_deckListControl.HideScroll =
					_buttonHideScroll.Checked;

			if (!restoringSettings())
				historyUpdate();
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
				_imagePreloadingSubsystem.Reset();

				if (_cardRepo.IsLoadingComplete)
					_historySubsystem.Current.SearchResultScroll = _viewCards.VisibleRecordIndex;
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

		private void deckListOpenedDeck(object sender, Deck deck) =>
			deckLoaded(deck);

		private void deckListRenamedDeck(object sender, Deck deck)
		{
			_historySubsystem.DeckName = deck.Name;
			updateFormStatus();
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
				if (mustBeEnabled)
					setFilterManagerState(FilterGroup.Deck, FilterValueState.Required);
				else
					setFilterManagerState(FilterGroup.Deck, FilterValueState.Ignored);
			}

			resetTouchedCard();
			RunRefilterTask();
			historyUpdate();
		}

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
			updateDeckVisibility();

			var zone = DeckZone ?? Zone.Main;

			if (_deckEditorModel.Zone != zone)
			{
				beginRestoreSettings();
				_deckEditorModel.SetZone(zone, _cardRepo);
				endRestoreSettings();
			}

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
			if (!zone.HasValue)
				throw new ArgumentNullException(nameof(zone));

			if (zone == _deckEditorModel.Zone)
			{
				updateViewCards(listChanged, card, FilterGroup.Deck, touchedChanged);
				updateViewDeck(listChanged, countChanged, card, touchedChanged);
			}

			bool isActualDeckChange = zone != Zone.SampleHand &&
				(countChanged || listChanged) &&
				changeTerminatesBatch;

			if (!restoringSettings())
				if (isActualDeckChange)
					historyUpdate();

			if (isActualDeckChange)
				_deckListControl.DeckChanged(copyDeck());

			if (!restoringSettings())
				updateFormStatus();
		}

		private void collectionChanged(bool listChanged, bool countChanged, Card card, bool touchedChanged, Zone? zone, bool changeTerminatesBatch)
		{
			updateViewCards(listChanged, card, FilterGroup.Collection, touchedChanged);

			updateViewDeck(
				listChanged: false,
				countChanged: true,
				card: card,
				touchedChanged: touchedChanged);

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

				if (touchedChanged && _deckEditorModel.TouchedCard != null)
					RunRefilterTask(() => _scrollSubsystem.EnsureCardVisibility(_deckEditorModel.TouchedCard, _viewCards));
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

			if (touchedChanged && _deckEditorModel.TouchedCard != null)
				_scrollSubsystem.EnsureCardVisibility(_deckEditorModel.TouchedCard, _viewDeck);

			if (listChanged)
				_viewDeck.Invalidate();
			else if (touchedChanged || countChanged)
				_viewDeck.Invalidate();
			else if (card != null)
				_viewDeck.InvalidateCard(card);
		}



		private void loadDeck(Deck deck)
		{
			_historySubsystem.DeckFile = deck.File;
			_historySubsystem.DeckName = deck.Name;

			_deckEditorModel.SetDeck(deck, _cardRepo);
			_deckEditorModel.Shuffle();
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
					_panelMenu.Visible = _formRoot.ShowFilterPanels;
		}

		private void languageChanged()
		{
			if (!_isTabSelected)
				return;

			if (restoringSettings())
				return;

			if (_sortSubsystem.IsLanguageDependent || isFilterGroupEnabled(FilterGroup.Find) && isSearchStringApplied())
			{
				beginRestoreSettings();

				if (_sortSubsystem.IsLanguageDependent)
					_sortSubsystem.Invalidate();

				if (isFilterGroupEnabled(FilterGroup.Find) && isSearchStringApplied())
					_cardSearchSubsystem.Apply();

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
			_sortSubsystem.Invalidate();
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

			float sampleHandOpacity = 0.6f;

			_buttonSubsystem.SetupButton(_buttonSampleHandNew,
				new ButtonImages(
					null,
					Resources.hand_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.hand_48,
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonSampleHandDraw,
				new ButtonImages(
					null,
					Resources.draw_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.draw_48,
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonSampleHandMulligan,
				new ButtonImages(
					null,
					Resources.mulligan_48.SetOpacity(sampleHandOpacity),
					null,
					Resources.mulligan_48,
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonShowDuplicates,
				new ButtonImages(
					Resources.clone_48.TransformColors(saturation: 0f),
					Resources.clone_48,
					Resources.clone_48.TransformColors(saturation: 0.9f),
					Resources.clone_48.TransformColors(saturation: 2.5f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonShowProhibit,
				new ButtonImages(
					Resources.exclude_hidden_24,
					Resources.exclude_shown_24,
					Resources.exclude_hidden_24.TransformColors(sat, 1.2f),
					Resources.exclude_shown_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonExcludeManaAbility,
				new ButtonImages(
					Resources.include_plus_24,
					Resources.exclude_minus_24,
					Resources.include_plus_24.TransformColors(sat, 1f),
					Resources.exclude_minus_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonExcludeManaCost,
				new ButtonImages(
					Resources.include_plus_24,
					Resources.exclude_minus_24,
					Resources.include_plus_24.TransformColors(sat, 1f),
					Resources.exclude_minus_24.TransformColors(sat, 1.2f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonHideDeck,
				new ButtonImages(
					Resources.shown_40,
					Resources.hidden_40,
					Resources.shown_40.TransformColors(brightness: 0.1f),
					Resources.hidden_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonHideScroll,
				new ButtonImages(
					Resources.scroll_shown_40,
					Resources.scroll_hidden_40,
					Resources.scroll_shown_40.TransformColors(brightness: 1.05f),
					Resources.scroll_hidden_40.TransformColors(brightness: 1.05f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonHidePartialCards,
				new ButtonImages(
					Resources.partial_card_enabled_40,
					Resources.partial_card_disabled_40,
					Resources.partial_card_enabled_40.TransformColors(brightness: 0.1f),
					Resources.partial_card_disabled_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonHideText,
				new ButtonImages(
					Resources.text_enabled_40,
					Resources.text_disabled_40,
					Resources.text_enabled_40.TransformColors(brightness: 0.1f),
					Resources.text_disabled_40.TransformColors(brightness: 0.1f),
					areImagesDoubleSized: true));

			_buttonSubsystem.SetupButton(_buttonSearchExamplesDropDown,
				new ButtonImages(
					null,
					Resources.book_40,
					null,
					Resources.book_40_hovered,
					areImagesDoubleSized: true));
		}



		private static void deckDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
				if (files.Length < 10)
					e.Effect = DragDropEffects.Copy;
			}
			else if (e.Data.GetFormats().Contains(DataFormats.Text))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		public void PasteDeck(bool append)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return;

			pasteDeckFromText(text, append);
		}

		public void PasteCollection(bool append)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return;

			pasteCollectionFromText(text, append);
		}

		public void CopyCollection()
		{
			var deck = Deck.Create(
				_collectionModel.CountById?.ToDictionary(),
				_collectionModel.CountById?.Keys.OrderBy(_ => _cardRepo.CardsById[_].NameEn).ToList(),
				null,
				null);

			var serialized = _deckSerializationSubsystem.SaveSerialized("*.txt", deck);
			Clipboard.SetText(serialized);
		}

		public void CopyDeck()
		{
			Deck deck;

			switch (DeckZone)
			{
				case Zone.SampleHand:
					deck = copyMainDeck();
					break;
				case Zone.Side:
					deck = copuSideDeck();
					break;
				case Zone.Main:
					deck = copyDeck();
					break;
				default:
					return;
			}

			var serialized = _deckSerializationSubsystem.SaveSerialized("*.txt", deck);
			Clipboard.SetText(serialized);
		}

		private Deck copyMainDeck() =>
			Deck.Create(
				_deckEditorModel.SampleHand.CountById.ToDictionary(),
				_deckEditorModel.SampleHand.CardsIds.ToList(),
				null,
				null);

		private Deck copuSideDeck() =>
			Deck.Create(
				_deckEditorModel.SideDeck.CountById.ToDictionary(),
				_deckEditorModel.SideDeck.CardsIds.ToList(),
				null,
				null);

		private Deck copyDeck()
		{
			var result = Deck.Create(
				_deckEditorModel.MainDeck.CountById.ToDictionary(),
				_deckEditorModel.MainDeck.CardsIds.ToList(),
				_deckEditorModel.SideDeck.CountById.ToDictionary(),
				_deckEditorModel.SideDeck.CardsIds.ToList());

			result.Name = _historySubsystem.Current.DeckName;
			result.File = _historySubsystem.Current.DeckFile;

			return result;
		}

		private void pasteDeckFromText(string text, bool append)
		{
			var deck = _deckSerializationSubsystem.LoadSerialized("*.txt", text);

			if (deck.Error != null)
				MessageBox.Show(deck.Error);
			else
				_deckEditorModel.Paste(deck, append, _cardRepo);
		}

		private void hideSampleHand()
		{
			if (DeckZone == Zone.SampleHand)
				DeckZone = Zone.Main;
		}

		private void pasteCollectionFromText(string text, bool append)
		{
			var deck = _deckSerializationSubsystem.LoadSerialized("*.txt", text);

			if (deck.Error != null)
				MessageBox.Show(deck.Error);
			else
				_collectionModel.LoadCollection(deck, append);
		}

		private void deckDragDropped(object sender, DragEventArgs e)
		{
			if (_cardRepo.IsLoadingComplete)
			{
				dragDropped(e.Data);
				return;
			}

			_cardRepo.LoadingComplete += () => { this.Invoke(delegate { dragDropped(e.Data); }); };

			MessageBox.Show(this,
				"Mtgdb.Gui is loading cards.\r\n" +
				"When completed, the deck(s) will be opened.",
				"Opening deck(s) delayed",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		private void dragDropped(IDataObject dragData)
		{
			if (dragData.GetDataPresent(DataFormats.FileDrop))
			{
				var files = (string[]) dragData.GetData(DataFormats.FileDrop);

				var decks = files.Select(f => _deckSerializationSubsystem.LoadFile(f))
					.ToArray();

				var failedDecks = decks.Where(d => d.Error != null).ToArray();
				var loadedDecks = decks.Where(d => d.Error == null).ToArray();

				if (failedDecks.Length > 0)
				{
					var message = string.Join(Str.Endl,
						failedDecks.Select(f => $"{f.File}{Str.Endl}{f.Error}{Str.Endl}"));

					MessageBox.Show(message);
				}

				if (loadedDecks.Length > 0)
					deckLoaded(loadedDecks[0]);

				for (int i = 1; i < loadedDecks.Length; i++)
				{
					var deck = loadedDecks[i];
					_formRoot.AddTab(form => ((FormMain) form)._requiredDeck = deck);
				}
			}
			else if (dragData.GetFormats().Contains(DataFormats.Text))
			{
				string text = (string) dragData.GetData(DataFormats.Text, autoConvert: true);

				if (ModifierKeys == Keys.Alt)
					pasteCollectionFromText(text, append: false);
				else if (ModifierKeys == (Keys.Alt | Keys.Shift))
					pasteCollectionFromText(text, append: true);
				else if (ModifierKeys == Keys.None)
					pasteDeckFromText(text, append: false);
				else if (ModifierKeys == Keys.Shift)
					pasteDeckFromText(text, append: true);
			}
		}



		private void sampleHandNew(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			_deckEditorModel.NewSampleHand(_cardRepo);
		}

		private void sampleHandMulligan(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			_deckEditorModel.Mulligan(_cardRepo);
		}

		private void sampleHandDraw(object sender, EventArgs e)
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			_deckEditorModel.Draw(_cardRepo);
		}



		private void historyLoaded()
		{
			updateFormStatus();
		}

		private void sizeChanged(object sender, EventArgs e)
		{
			_layoutRight.PerformLayout();
		}

		private static void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			e.IsInputKey = true;
		}
	}
}