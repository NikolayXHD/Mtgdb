using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public sealed partial class FormMain : Form
	{
		public void SetFormRoot(IFormRoot formRoot)
		{
			if (formRoot == _formRoot)
				return;

			if (_formRoot != null)
			{
				_cardSearchSubsystem.UnsubscribeSuggestModelEvents();

				_cardSearchSubsystem.TextApplied -= cardSearchStringApplied;
				_cardSearchSubsystem.TextChanged -= cardSearchStringChanged;

				_formRoot.UiModel.LanguageController.LanguageChanged -= languageChanged;
				_formRoot.ShowFilterPanelsChanged -= showFilterPanelsChanged;
				_formRoot.TooltipController.UnsetTooltips(this);
			}

			_formRoot = formRoot;

			if (formRoot != null)
			{
				_cardSearchSubsystem.Ui =
					_deckEditingSubsystem.Ui =
						_imagePreloadingSubsystem.Ui =
							_printingSubsystem.Ui =
								_draggingSubsystem.Ui =
									_drawingSubsystem.Ui =
										_fields.Ui = _formRoot.UiModel;

				_cardSearchSubsystem.SuggestModel = _formRoot.CardSuggestModel;

				_deckListControl.SetUi(_formRoot.UiModel, _formRoot.TooltipController, _formRoot.DeckSuggestModel);

				_cardSearchSubsystem.SubscribeSuggestModelEvents();

				_cardSearchSubsystem.TextApplied += cardSearchStringApplied;
				_cardSearchSubsystem.TextChanged += cardSearchStringChanged;

				_formRoot.UiModel.LanguageController.LanguageChanged += languageChanged;
				_formRoot.ShowFilterPanelsChanged += showFilterPanelsChanged;
				
				setupTooltips();

				_formRoot.TooltipController.SubscribeToEvents();

				// calls probeCardCreating handler
				resetLayouts();
			}
		}

		private bool evalFilterBySearchText(Card c) =>
			_cardSearchSubsystem.SearchResult?.RelevanceById?.ContainsKey(c.IndexInFile) != false;

		private bool evalFilterByDeck(Card c)
		{
			switch (_deckListControl.FilterByDeckMode)
			{
				case FilterByDeckMode.CurrentDeck:
					return c.DeckCount(_formRoot.UiModel) > 0;
				case FilterByDeckMode.FilteredSavedDecks:
					return _deckListControl.AnyFilteredDeckContains(c);
				default:
					throw new NotSupportedException();
			}
		}

		private bool evalFilterByCollection(Card c) =>
			c.CollectionCount(_formRoot.UiModel) > 0;

		public void LoadHistory(string historyFile)
		{
			_historySubsystem.LoadHistory(historyFile);
			_deckSerializationSubsystem.State.LastFile = _historySubsystem.Current.DeckFile;
		}

		public void SaveHistory(string historyFile)
		{
			historyUpdateFormPosition(_historySubsystem.Current);
			_historySubsystem.Save(historyFile);
		}

		public void OnTabSelected()
		{
			if (!_historySubsystem.IsLoaded)
				throw new InvalidOperationException("History must be loaded first");

			_isTabSelected = true;

			lock (_searchResultCards)
				updateIsSearchResult();

			_formRoot.UiModel.Deck = _deckEditorModel;

			_cardSearchSubsystem.UpdateSuggestInput();

			bool isFirstTime = !_formRoot.LoadedGuiSettings;

			if (isFirstTime)
			{
				updateFormSettings();
				updateFormPosition();
			}
			else
			{
				readFormSettingsTo(_historySubsystem.Current);
			}

			if (_collectionModel.IsLoaded)
				_historySubsystem.Current.Collection = _collectionModel.CountById.ToDictionary();

			// loads collection
			historyApply(_historySubsystem.Current);

			if (_requiredDeck != null)
			{
				_requiredDeck = null;
				historyUpdate();
			}

			historyUpdateButtons();

			if (IsHandleCreated)
			{
				_searchEditor.Focus();
				startThreads();
			}
		}



		private void updateFormPosition()
		{
			if (_historySubsystem.Current.WindowArea.HasValue)
			{
				_formRoot.WindowArea = _historySubsystem.Current.WindowArea.Value;

				if (_historySubsystem.Current.WindowSnapDirection.HasValue)
					_formRoot.SnapDirection = _historySubsystem.Current.WindowSnapDirection.Value;
			}
			else
			{
				_formRoot.SnapDirection = Direction.North;
			}
		}

		private void historyUpdateFormPosition(GuiSettings settings)
		{
			settings.WindowArea = _formRoot.WindowArea;
			settings.WindowSnapDirection = _formRoot.SnapDirection;
		}

		public void OnTabUnselected()
		{
			_isTabSelected = false;
			stopThreads();
		}

		private void updateShowSampleHandButtons()
		{
			bool isSampleHand = _deckEditorModel.Zone == Zone.SampleHand;
			bool enabled = _cardRepo.IsLoadingComplete;

			_buttonSampleHandNew.Visible = isSampleHand;
			_buttonSampleHandMulligan.Visible = isSampleHand;
			_buttonSampleHandDraw.Visible = isSampleHand;

			_buttonSampleHandNew.Enabled = enabled;
			_buttonSampleHandMulligan.Enabled = enabled;
			_buttonSampleHandDraw.Enabled = enabled;
		}

		private void updateShowProhibited()
		{
			_layoutRoot.SuspendLayout();
			_layoutRight.SuspendLayout();
			_layoutMain.SuspendLayout();

			var controls = _quickFilterControls.Append(FilterManager);

			// ReSharper disable PossibleMultipleEnumeration
			foreach (var control in controls)
				control.SuspendLayout();

			foreach (var control in controls)
				control.HideProhibit = !_buttonShowProhibit.Checked;

			foreach (var control in controls)
			{
				control.ResumeLayout(false);
				control.PerformLayout();
			}
			// ReSharper restore PossibleMultipleEnumeration

			setRightPanelsWidth();

			_layoutRight.ResumeLayout(false);
			_layoutRight.PerformLayout();

			_layoutMain.ResumeLayout(false);
			_layoutMain.PerformLayout();

			_layoutRoot.ResumeLayout(false);
			_layoutRoot.PerformLayout();
		}

		public void ButtonTooltip()
		{
			if (!_isTabSelected)
				return;

			if (restoringSettings())
				return;

			historyUpdate();
		}



		private void updateExcludeManaCost()
		{
			FilterManaCost.EnableRequiringSome = !_buttonExcludeManaCost.Checked;
			FilterManaCost.EnableCostBehavior = _buttonExcludeManaCost.Checked;
		}

		private void updateExcludeManaAbility()
		{
			FilterManaAbility.EnableRequiringSome = !_buttonExcludeManaAbility.Checked;
			FilterManaAbility.EnableCostBehavior = _buttonExcludeManaAbility.Checked;
		}



		private void resetTouchedCard()
		{
			_deckEditorModel.TouchedCard = null;
		}

		public void RunRefilterTask(Action onFinished = null)
		{
			ThreadPool.QueueUserWorkItem(_ => refilter(onFinished));
		}

		private void refilter(Action onFinished)
		{
			var touchedCard = _deckEditorModel.TouchedCard;
			bool showDuplicates = _buttonShowDuplicates.Checked;
			var filterManagerStates = FilterManager.States;

			_breakRefreshing = true;

			lock (_searchResultCards)
			{
				_breakRefreshing = false;

				var searchResultCards = new List<Card>();
				var filteredCards = new List<Card>();

				var allCards = _sortSubsystem.SortedCards;

				if (showDuplicates)
				{
					for (int i = 0; i < allCards.Count; i++)
					{
						if (_breakRefreshing)
							return;

						var card = allCards[i];

						bool isFiltered = fit(card, filterManagerStates);

						if (isFiltered || card == touchedCard)
							searchResultCards.Add(card);

						if (isFiltered)
							filteredCards.Add(card);
					}
				}
				else
				{
					var cardsByName = new Dictionary<string, Card>();

					for (int i = 0; i < allCards.Count; i++)
					{
						if (_breakRefreshing)
							return;

						var card = allCards[i];

						if (fit(card, filterManagerStates))
						{
							bool isCurrentCardMoreRecent;

							if (!cardsByName.TryGetValue(card.NameNormalized, out var otherCard))
								isCurrentCardMoreRecent = true;
							else
							{
								var dateCompare = Str.Compare(card.ReleaseDate, otherCard.ReleaseDate);
								if (dateCompare > 0)
									isCurrentCardMoreRecent = true;
								else if (dateCompare == 0)
									isCurrentCardMoreRecent = card.IndexInFile < otherCard.IndexInFile;
								else
									isCurrentCardMoreRecent = false;
							}

							if (isCurrentCardMoreRecent)
								cardsByName[card.NameNormalized] = card;
						}
					}

					for (int i = 0; i < allCards.Count; i++)
					{
						if (_breakRefreshing)
							return;

						var card = allCards[i];

						bool isFiltered = cardsByName.TryGet(card.NameNormalized) == card;

						if (isFiltered || card == touchedCard)
							searchResultCards.Add(card);

						if (isFiltered)
							filteredCards.Add(card);
					}
				}

				// implicit connection: data_source_sync
				lock (_searchResultCards)
				{
					_searchResultCards.Clear();
					_searchResultCards.AddRange(searchResultCards);
				}

				_filteredCards.Clear();
				_filteredCards.UnionWith(filteredCards);

				updateIsSearchResult();
			}

			this.Invoke(delegate
			{
				_imagePreloadingSubsystem.Reset();
				refreshData();
				onFinished?.Invoke();
			});
		}

		private void updateIsSearchResult()
		{
			foreach (var card in _sortSubsystem.SortedCards)
				card.IsSearchResult = _filteredCards.Contains(card);
		}

		private void refreshData()
		{
			int visibleRecordIndex;

			if (_requiredScroll.HasValue && _cardRepo.IsLoadingComplete)
			{
				visibleRecordIndex = _requiredScroll.Value;
				_requiredScroll = null;
			}
			else
				visibleRecordIndex = _viewCards.VisibleRecordIndex;

			if (visibleRecordIndex >= _searchResultCards.Count)
				visibleRecordIndex = 0;

			_viewCards.VisibleRecordIndex = visibleRecordIndex;

			_viewCards.RefreshData();
			_viewCards.Invalidate();

			_viewDeck.Invalidate();

			updateFormStatus();
		}



		private void updateFormStatus()
		{
			_panelStatus.SuspendLayout();

			_labelStatusSets.Text = _cardRepo.SetsByCode.Count.ToString();
			_labelStatusScrollCards.Text = $"{_viewCards.VisibleRecordIndex}/{_searchResultCards.Count}";

			_tabHeadersDeck.SetTabSettings(new Dictionary<object, TabSettings>
			{
				{ (int) Zone.Main, new TabSettings($"main deck: {_deckEditorModel.MainDeckSize}/60") },
				{ (int) Zone.Side, new TabSettings($"sideboard: {_deckEditorModel.SideDeckSize}/15") },
				{ (int) Zone.SampleHand, new TabSettings($"sample hand: {_deckEditorModel.SampleHandSize}") },
				{ DeckListTabIndex, new TabSettings($"deck list: {_deckListControl.FilteredDecksCount}") }
			});

			if (IsDeckListSelected)
				_labelStatusScrollDeck.Text = $"{_deckListControl.ScrollPosition}/{_deckListControl.MaxScroll}";
			else
				_labelStatusScrollDeck.Text = $"{_viewDeck.VisibleRecordIndex}/{_viewDeck.RowCount}";

			_labelStatusCollection.Text = _collectionModel.CollectionSize.ToString();

			var filterManagerStates = FilterManager.States;

			_labelStatusFilterButtons.Text = getStatusFilterButtons(filterManagerStates);
			_labelStatusSearch.Text = getStatusSearch(filterManagerStates);
			_labelStatusFilterCollection.Text = getStatusCollectionOnly(filterManagerStates);
			_labelStatusFilterDeck.Text = getStatusDeckOnly(filterManagerStates);

			_labelStatusFilterLegality.Text = getStatusLegalityFilter(filterManagerStates);

			_labelStatusSort.Text = getStatusSort();

			setTitle(_historySubsystem.DeckName);

			_panelStatus.ResumeLayout(false);
			_panelStatus.PerformLayout();
		}

		private string getStatusFilterButtons(FilterValueState[] filterManagerStates)
		{
			if (!_keywordSearcher.IsLoading && !_keywordSearcher.IsLoaded)
				return "pending keywod search…";

			if (_keywordsIndexUpToDate && _keywordSearcher.IsLoading)
				return "loading keywords…";

			if (_keywordSearcher.IsLoading)
				return $"indexing keywords {_keywordSearcher.SetsCount} / {_cardRepo.SetsByCode.Count} sets…";

			string status = getFilterStatusText(
				filterManagerStates,
				FilterGroup.Buttons,
				isQuickFilteringActive(),
				"empty");

			return status;
		}

		private string getStatusDeckOnly(FilterValueState[] filterManagerStates)
		{
			var status = getFilterStatusText(filterManagerStates, FilterGroup.Deck, true, null);

			switch (_deckListControl.FilterByDeckMode)
			{
				case FilterByDeckMode.CurrentDeck:
					status += ", current deck";
					break;
				case FilterByDeckMode.FilteredSavedDecks:
					status += ", filtered saved decks";
					break;
			}

			return status;
		}

		private static string getStatusCollectionOnly(FilterValueState[] filterManagerStates)
		{
			var status = getFilterStatusText(filterManagerStates, FilterGroup.Collection, true, "empty");
			return status;
		}

		private string getStatusSearch(FilterValueState[] filterManagerStates)
		{
			if (isSearchStringModified())
				return "receving user input";

			string noInputText;

			if (!_cardSearcher.IsLoading && !_cardSearcher.IsLoaded && !_cardSearcher.Spellchecker.IsLoading && !_cardSearcher.Spellchecker.IsLoaded)
				noInputText = "pending index load…";
			else if (_cardSearcher.IsLoading)
			{
				if (_luceneSearchIndexUpToDate)
					noInputText = "loading search index…";
				else
					noInputText = $"indexing search {_cardSearcher.SetsAddedToIndex} / {_cardRepo.SetsByCode.Count} sets…";
			}
			else if (_cardSearcher.Spellchecker.IsLoading)
			{
				if (_spellcheckerIndexUpToDate)
					noInputText = "loading intellisense…";
				else
					noInputText = $"indexing intellisense {_cardSearcher.Spellchecker.IndexedFields} / {_cardSearcher.Spellchecker.TotalFields} fields…";
			}
			else if (_cardSearchSubsystem.SearchResult?.ParseErrorMessage != null)
				noInputText = "syntax error";
			else
				noInputText = "empty";

			var status = getFilterStatusText(
				filterManagerStates,
				FilterGroup.Find,
				isSearchStringApplied(),
				noInputText);

			return status;
		}

		private bool isSearchStringApplied()
		{
			return
				_cardSearchSubsystem.SearchResult?.RelevanceById != null &&
				_cardSearcher.Spellchecker.IsLoaded;
		}

		private string getStatusSort()
		{
			var infos = _sortSubsystem.SortInfo?.ToList() ?? new List<FieldSortInfo>();
			
			if (_cardSearchSubsystem.SearchResult?.RelevanceById != null)
				infos.Add(new FieldSortInfo("Relevance", SortOrder.Descending));

			if (infos.Count == 0)
				return "none";

			return string.Join(" ", infos);
		}

		private string getStatusLegalityFilter(FilterValueState[] filterManagerStates)
		{
			var result = new StringBuilder();

			var status = getFilterStatusText(filterManagerStates,
				FilterGroup.Legality,
				!string.IsNullOrEmpty(_legalitySubsystem.FilterFormat),
				Legality.AnyFormat);

			result.Append(status);

			if (!string.IsNullOrEmpty(_legalitySubsystem.FilterFormat))
			{
				result.Append(' ');
				result.Append(_legalitySubsystem.FilterFormat);

				const string allowed = @" +";
				const string notAllowed = @" -";

				if (_legalitySubsystem.AllowLegal)
					result.Append(allowed);
				else
					result.Append(notAllowed);
				result.Append(Legality.Legal);

				if (_legalitySubsystem.AllowRestricted)
					result.Append(allowed);
				else
					result.Append(notAllowed);
				result.Append(Legality.Restricted);

				if (_legalitySubsystem.AllowBanned)
					result.Append(allowed);
				else
					result.Append(notAllowed);

				result.Append(Legality.Banned);
			}

			return result.ToString();
		}


		private bool isSearchStringModified() => 
			_searchEditor.Text != (_historySubsystem?.Current?.Find ?? string.Empty);

		private static string getFilterStatusText(
			FilterValueState[] filterManagerStates,
			FilterGroup filterGroup,
			bool hasInput,
			string noInputText)
		{
			if (!hasInput)
				return noInputText;

			switch (filterManagerStates[filterGroup.Index()])
			{
				case FilterValueState.RequiredSome:
					return "OR mode";
				case FilterValueState.Required:
					return "AND mode";
				default:
					return "ignored";
			}
		}

		private bool isFilterGroupEnabled(FilterGroup filterGroup)
		{
			var state = FilterManager.States[filterGroup.Index()];

			switch (state)
			{
				case FilterValueState.Required:
				case FilterValueState.RequiredSome:
				case FilterValueState.Prohibited:
					return true;
				case FilterValueState.Ignored:
					return false;
				default:
					throw new NotSupportedException();
			}
		}

		private bool isQuickFilteringActive()
		{
			var buttonStates = QuickFilterSetup.GetButtonStates(this);
			var result = buttonStates.Any(arr => arr.Any(_ => _ != FilterValueState.Ignored));
			return result;
		}

		private void setFilterManagerState(FilterGroup filterGroup, FilterValueState value)
		{
			var states = FilterManager.States;

			if (states[filterGroup.Index()] == value)
				return;

			states[filterGroup.Index()] = value;

			beginRestoreSettings();
			FilterManager.States = states;
			endRestoreSettings();
		}


		private void updateFormSettings()
		{
			_formRoot.ShowDeck = !_buttonHideDeck.Checked;
			_formRoot.ShowScroll = !_buttonHideScroll.Checked;
			_formRoot.ShowPartialCards = !_buttonHidePartialCards.Checked;
			_formRoot.ShowTextualFields = !_buttonHideText.Checked;

			_formRoot.LoadedGuiSettings = true;
		}

		private void readFormSettingsTo(GuiSettings settings)
		{
			settings.ShowDeck = _formRoot.ShowDeck;
			settings.ShowPartialCards = _formRoot.ShowPartialCards;
			settings.ShowTextualFields = _formRoot.ShowTextualFields;
			settings.ShowScroll = _formRoot.ShowScroll;

			settings.ShowFilterPanels = _formRoot.ShowFilterPanels;
			settings.HideTooltips = _formRoot.HideTooltips;
			settings.Language = _formRoot.UiModel.LanguageController.Language;
		}

		private void historyUpdate()
		{
			if (_historySubsystem == null)
				return;

			var settings = new GuiSettings
			{
				Find = _cardSearchSubsystem.AppliedText,
				FilterAbility = FilterAbility.States,
				FilterCastKeyword = FilterCastKeyword.States,
				FilterMana = FilterManaCost.States,
				FilterManaAbility = FilterManaAbility.States,
				FilterManaGenerated = FilterGeneratedMana.States,
				FilterRarity = FilterRarity.States,
				FilterType = FilterType.States,
				FilterCmc = FilterCmc.States,
				FilterLayout = FilterLayout.States,
				FilterGrid = FilterManager.States,
				Language = _formRoot.UiModel.LanguageController.Language,
				MainDeckCount = _deckEditorModel.MainDeck.CountById.ToDictionary(),
				MainDeckOrder = _deckEditorModel.MainDeck.CardsIds.ToList(),
				SideDeckCount = _deckEditorModel.SideDeck.CountById.ToDictionary(),
				SideDeckOrder = _deckEditorModel.SideDeck.CardsIds.ToList(),
				Collection = _collectionModel.CountById.ToDictionary(),
				ShowDuplicates = _buttonShowDuplicates.Checked,
				HideTooltips = !_formRoot.TooltipController.Active,
				ExcludeManaAbilities = _buttonExcludeManaAbility.Checked,
				ExcludeManaCost = _buttonExcludeManaCost.Checked,
				ShowProhibit = _buttonShowProhibit.Checked,
				Sort = _sortSubsystem.SortString,
				LegalityFilterFormat = _legalitySubsystem.FilterFormat,
				LegalityAllowLegal = _legalitySubsystem.AllowLegal,
				LegalityAllowRestricted = _legalitySubsystem.AllowRestricted,
				LegalityAllowBanned = _legalitySubsystem.AllowBanned,
				DeckFile = _historySubsystem.DeckFile,
				DeckName = _historySubsystem.DeckName,
				SearchResultScroll = _viewCards.VisibleRecordIndex,
				ShowDeck = !_buttonHideDeck.Checked,
				ShowScroll = !_buttonHideScroll.Checked,
				ShowPartialCards = !_buttonHidePartialCards.Checked,
				ShowTextualFields = !_buttonHideText.Checked,
				ShowFilterPanels = _formRoot.ShowFilterPanels,
				FilterByDeckMode = _deckListControl.FilterByDeckMode
			};

			historyUpdateFormPosition(settings);

			_historySubsystem.Add(settings);
			historyUpdateButtons();
		}

		private void historyApply(GuiSettings settings)
		{
			beginRestoreSettings();

			if (settings.FilterAbility == null || settings.FilterAbility.Length == FilterAbility.PropertiesCount)
				FilterAbility.States = settings.FilterAbility;

			if (settings.FilterCastKeyword == null || settings.FilterCastKeyword.Length == FilterCastKeyword.PropertiesCount)
				FilterCastKeyword.States = settings.FilterCastKeyword;

			if (settings.FilterMana == null || settings.FilterMana.Length == FilterManaCost.PropertiesCount)
				FilterManaCost.States = settings.FilterMana;

			if (settings.FilterManaAbility == null || settings.FilterManaAbility.Length == FilterManaAbility.PropertiesCount)
				FilterManaAbility.States = settings.FilterManaAbility;

			if (settings.FilterManaGenerated == null || settings.FilterManaGenerated.Length == FilterGeneratedMana.PropertiesCount)
				FilterGeneratedMana.States = settings.FilterManaGenerated;

			if (settings.FilterRarity == null || settings.FilterRarity.Length == FilterRarity.PropertiesCount)
				FilterRarity.States = settings.FilterRarity;

			if (settings.FilterType == null || settings.FilterType.Length == FilterType.PropertiesCount)
				FilterType.States = settings.FilterType;

			if (settings.FilterCmc == null || settings.FilterCmc.Length == FilterCmc.PropertiesCount)
				FilterCmc.States = settings.FilterCmc;

			if (settings.FilterLayout == null || settings.FilterLayout.Length == FilterLayout.PropertiesCount)
				FilterLayout.States = settings.FilterLayout;

			if (settings.FilterGrid == null || settings.FilterGrid.Length == FilterManager.PropertiesCount)
				FilterManager.States = settings.FilterGrid;

			updateFilterByDeckMode();

			updateTerms();

			_cardSearchSubsystem.AppliedText = settings.Find ?? string.Empty;

			_formRoot.UiModel.LanguageController.Language = settings.Language ?? CardLocalization.DefaultLanguage;

			_cardSearchSubsystem.Apply();
			_buttonShowDuplicates.Checked = settings.ShowDuplicates;

			_formRoot.HideTooltips = settings.HideTooltips;

			_buttonExcludeManaAbility.Checked = settings.ExcludeManaAbilities;
			_buttonExcludeManaCost.Checked = settings.ExcludeManaCost != false;
			_buttonShowProhibit.Checked = settings.ShowProhibit;
			_sortSubsystem.ApplySort(settings.Sort);

			hideSampleHand();

			_collectionModel.LoadCollection(settings.CollectionModel, append: false);
			loadDeck(_requiredDeck ?? settings.Deck);

			_legalitySubsystem.SetFilterFormat(settings.LegalityFilterFormat);
			_legalitySubsystem.SetAllowLegal(settings.LegalityAllowLegal != false);
			_legalitySubsystem.SetAllowRestricted(settings.LegalityAllowRestricted != false);
			_legalitySubsystem.SetAllowBanned(settings.LegalityAllowBanned == true);

			_requiredScroll = settings.SearchResultScroll;

			_buttonHideDeck.Checked = settings.ShowDeck == false;
			_buttonHideScroll.Checked = settings.ShowScroll == false;
			_buttonHidePartialCards.Checked = settings.ShowPartialCards == false;
			_buttonHideText.Checked = settings.ShowTextualFields == false;
			_formRoot.ShowFilterPanels = settings.ShowFilterPanels != false;

			applyShowFilterPanels();

			_deckListControl.FilterByDeckMode = settings.FilterByDeckMode ??
			(isFilterGroupEnabled(FilterGroup.Deck)
				? FilterByDeckMode.CurrentDeck
				: FilterByDeckMode.Ignored);

			endRestoreSettings();

			resetTouchedCard();
			RunRefilterTask();
			historyUpdateButtons();

			setTitle(settings.DeckName);
		}

		private void historyRedo()
		{
			if (_historySubsystem.Redo())
				historyApply(_historySubsystem.Current);
		}

		private void historyUndo()
		{
			if (_historySubsystem.Undo())
				historyApply(_historySubsystem.Current);
		}

		private void historyUpdateButtons()
		{
			_formRoot.CanUndo = _historySubsystem.CanUndo;
			_formRoot.CanRedo = _historySubsystem.CanRedo;
		}

		private void updateTerms()
		{
			var buttonStates = QuickFilterSetup.GetButtonStates(this);
			_quickFilterFacade.ApplyValueStates(buttonStates);
		}



		private bool fit(Card card, FilterValueState[] filterManagerStates)
		{
			foreach (var ev in _evaluators)
			{
				if (ev.Key >= filterManagerStates.Length)
					continue;

				var state = filterManagerStates[ev.Key];

				if (state != FilterValueState.Required && state != FilterValueState.Prohibited)
					continue;

				bool requiredResult = state == FilterValueState.Required;

				if (ev.Value(card) != requiredResult)
					return false;
			}

			bool existsRequiredSome = false;
			foreach (var ev in _evaluators)
			{
				if (filterManagerStates[ev.Key] != FilterValueState.RequiredSome)
					continue;

				if (ev.Value(card))
					return true;

				existsRequiredSome = true;
			}

			return !existsRequiredSome;
		}



		private void startThreads()
		{
			lock (this)
			{
				if (_threadsRunning)
					return;

				_threadsRunning = true;
			}

			_imagePreloadingSubsystem.StartThread();
			_cardSearchSubsystem.StartThread();
			_deckListControl.StartThread();
		}

		private void stopThreads()
		{
			lock (this)
			{
				if (!_threadsRunning)
					return;

				_threadsRunning = false;
			}

			_imagePreloadingSubsystem.AbortThread();
			_cardSearchSubsystem.AbortThread();
			_deckListControl.AbortThread();
		}


		public void ButtonClearDeck()
		{
			resetTouchedCard();

			if (_deckEditorModel.Zone == Zone.Main)
			{
				_historySubsystem.DeckFile = null;
				_historySubsystem.DeckName = null;
			}

			_deckEditorModel.Clear();
		}

		public void ButtonSaveDeck()
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var saved = _deckSerializationSubsystem.SaveDeck(_historySubsystem.Current.Deck);

			if (saved == null)
				return;

			setTitle(saved.Name);
			_historySubsystem.DeckFile = saved.File;
			_historySubsystem.DeckName = saved.Name;
		}

		public void ButtonLoadDeck()
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var loaded = _deckSerializationSubsystem.LoadDeck();

			if (loaded == null)
				return;

			deckLoaded(loaded);
		}

		public void ButtonSaveCollection()
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var saved = _deckSerializationSubsystem.SaveCollection(_historySubsystem.Current.CollectionModel);

			if (saved?.Error != null)
				MessageBox.Show(this, saved.Error);
		}

		public void ButtonLoadCollection()
		{
			if (!_cardRepo.IsLoadingComplete)
				return;

			var loaded = _deckSerializationSubsystem.LoadCollection();

			if (loaded == null)
				return;

			if (loaded.Error != null)
			{
				MessageBox.Show(this, loaded.Error);
				return;
			}

			_collectionModel.LoadCollection(loaded, append: false);
		}

		private void deckLoaded(Deck deck)
		{
			if (deck.Error != null)
			{
				MessageBox.Show(deck.Error);
				return;
			}

			hideSampleHand();
			loadDeck(deck);
		}

		private void setTitle(string deckName)
		{
			if (deckName == null)
			{
				Text = DeckSerializationSubsystem.NoDeck;
				return;
			}

			const int maxLength = 22;

			if (deckName.Length > maxLength)
				deckName = $"…{deckName.Substring(deckName.Length - maxLength)}";

			Text = deckName;
		}



		public void ButtonUndo()
		{
			_cardSearchSubsystem.ApplyDirtyText();
			historyUndo();
		}

		public void ButtonRedo() => historyRedo();

		public void ButtonPivot() => new FormChart(_cardRepo, _formRoot.UiModel, _fields).Show();

		public void ButtonPrint()
		{
			if (_cardRepo.IsLoadingComplete)
				_printingSubsystem.ShowPrintingDialog(_deckEditorModel, _historySubsystem.DeckName);
		}

		public void FocusSearch() => _cardSearchSubsystem.FocusSearch();

		public void ShowFindExamples() => _panelSearchExamples.ShowFindExamples();



		private void keywordSearcherLoaded()
		{
			updateTerms();
			RunRefilterTask();
		}

		private void keywordSearcherLoadingProgress()
		{
			this.Invoke(updateFormStatus);
		}

		public bool IsDraggingCard => _draggingSubsystem.IsDragging();

		public Card DraggedCard => _deckEditorModel.DraggedCard;

		public void StopDragging() => _draggingSubsystem.DragAbort();

		public bool IsSearchResultFocused() => !_cardSearchSubsystem.IsSearchFocused();

		private void beginRestoreSettings()
		{
			Interlocked.Increment(ref _restoringGuiSettings);
		}

		private void endRestoreSettings()
		{
			Interlocked.Decrement(ref _restoringGuiSettings);
		}

		private bool restoringSettings()
		{
			return _restoringGuiSettings > 0;
		}
	}
}