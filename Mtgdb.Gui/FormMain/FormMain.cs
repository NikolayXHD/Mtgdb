using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Resx;

namespace Mtgdb.Gui
{
	public sealed partial class FormMain : Form
	{
		public void SaveHistory(string id)
		{
			_historyModel.Id = id;
			_historyModel.Save();
		}

		public void OnSelectedTab(Card draggedCard)
		{
			_uiModel.Deck = _deckModel;
			_searchStringSubsystem.UpdateSuggestInput();
			historyUpdateButtons();

			if (draggedCard == null)
				_findEditor.Focus();
			else
				dragCard(draggedCard);
		}



		private void updateShowProhibited()
		{
			SuspendLayout();

			foreach (var control in _quickFilterControls)
				control.SuspendLayout();

			foreach (var control in _quickFilterControls)
				control.HideProhibit = !_buttonShowProhibit.Checked;

			foreach (var control in _quickFilterControls)
			{
				control.ResumeLayout(false);
				control.PerformLayout();
			}

			_panelCostMode.Width = _panelCost.Width = FilterManaAbility.Width + FilterManaCost.Margin.Left + FilterManaCost.Width;

			int rightMargin = FilterManaCost.ImageSize.Width + FilterManaCost.Spacing.Width;

			_buttonExcludeManaCost.Margin = new Padding(
				FilterManaCost.Width - rightMargin - _buttonExcludeManaCost.Width,
				0,
				rightMargin,
				0);

			_buttonExcludeManaAbility.Margin = new Padding(
				0,
				0,
				rightMargin + FilterManaCost.Margin.Left,
				0);

			ResumeLayout(false);
			PerformLayout();

			//SuspendPaint = false;
			Refresh();
		}

		public void ButtonTooltip()
		{
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




		private void refilterChangedDeck(bool listChanged, bool touchedChanged, Card card)
		{
			if (touchedChanged || listChanged && isFilterGroupEnabled(FilterGroupDeck))
				RunRefilterTask();
			else
			{
				if (card == null)
					_viewCards.Invalidate();
				else
					_viewCards.InvalidateCard(card);

				updateFormStatus();
			}

			updateViewDeck(listChanged, touchedChanged, card);
		}

		private void updateViewDeck(bool listChanged, bool touchedChanged, Card card)
		{
			if (listChanged)
			{
				_viewDeck.RefreshData();
				_viewDeck.Invalidate();
			}
			else if (touchedChanged)
				_viewDeck.Invalidate();
			else
				_viewDeck.InvalidateCard(card);
		}



		private void resetTouchedCard()
		{
			_deckModel.TouchedCard = null;
		}

		public void RunRefilterTask()
		{
			ThreadPool.QueueUserWorkItem(_ => refilter());
		}

		private void refilter()
		{
			var touchedCard = _deckModel.TouchedCard;
			bool showDuplicates = _buttonShowDuplicates.Checked;
			var filterManagerStates = FilterManager.States;

			_breakRefreshing = true;

			lock (_filteredCards)
			{
				_breakRefreshing = false;

				var filteredCards = new List<Card>();

				var allCards = _sortSubsystem.SortedCards;

				if (showDuplicates)
				{
					for (int i = 0; i < allCards.Count; i++)
					{
						if (_breakRefreshing)
							return;

						var card = allCards[i];

						card.IsSearchResult = fit(card, filterManagerStates);

						if (card.IsSearchResult || card == touchedCard)
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
							Card otherCard;
							bool isCurrentCardMoreRecent;

							if (!cardsByName.TryGetValue(card.NameNormalized, out otherCard))
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
						card.IsSearchResult = cardsByName.TryGet(card.NameNormalized) == card;

						if (card.IsSearchResult || card == touchedCard)
							filteredCards.Add(card);
					}
				}

				_filteredCards.Clear();
				_filteredCards.AddRange(filteredCards);
			}

			this.Invoke(delegate
			{
				_imagePreloadingSubsystem.Reset();
				refreshData();
			});
		}

		private void refreshData()
		{
			if (_viewCards.VisibleRecordIndex >= _filteredCards.Count)
				_viewCards.VisibleRecordIndex = 0;

			_viewCards.RefreshData();
			_viewCards.Invalidate();

			_viewDeck.Invalidate();

			updateFormStatus();
		}



		private void updateFormStatus()
		{
			_labelStatusSets.Text = _cardRepo.SetsByCode.Count.ToString();
			_labelStatusScrollCards.Text = $"{_viewCards.VisibleRecordIndex}/{_filteredCards.Count}";

			_tabHeadersDeck.SetTabSettings(new Dictionary<object, TabSettings>
			{
				{ 0, new TabSettings($"main deck: {_deckModel.MainDeckSize}/60") },
				{ 1, new TabSettings($"sideboard: {_deckModel.SideDeckSize}/15") }
			});

			_labelStatusScrollDeck.Text = $"{_viewDeck.VisibleRecordIndex}/{_viewDeck.RowCount}";
			_labelStatusCollection.Text = _collectionModel.CollectionSize.ToString();

			var filterManagerStates = FilterManager.States;

			_labelStatusFilterButtons.Text = getStatusFilterButtons(filterManagerStates);
			_labelStatusSearch.Text = getStatusSearch(filterManagerStates);
			_labelStatusFilterCollection.Text = getStatusCollectionOnly(filterManagerStates);
			_labelStatusFilterDeck.Text = getStatusDeckOnly(filterManagerStates);

			_labelStatusFilterLegality.Text = getStatusLegalityFilter(filterManagerStates);
		}

		private string getStatusFilterButtons(FilterValueState[] filterManagerStates)
		{
			if (!_keywordSearcher.IsLoading && !_keywordSearcher.IsLoaded)
				return Resources.Title_building_keywords_pending;

			if (_keywordSearcher.IsUpToDate && _keywordSearcher.IsLoading)
				return Resources.Title_loading_keywords;

			if (_keywordSearcher.IsLoading)
			{
				return string.Format(
					Resources.Title_building_index,
					Resources.Title_building_index_keywords,
					_keywordSearcher.SetsCount,
					_cardRepo.SetsByCode.Count);
			}

			string filterManagerModeDisplayText = getFilterManagerModeDisplayText(
				filterManagerStates,
				FilterGroupButtons,
				isQuickFilteringActive(),
				Resources.Title_mode_no_input);

			return filterManagerModeDisplayText;
		}

		private static string getStatusDeckOnly(FilterValueState[] filterManagerStates)
		{
			var gridFilterModeText = getFilterManagerModeDisplayText(filterManagerStates, FilterGroupDeck, true, Resources.Title_mode_no_input);

			return gridFilterModeText;
		}

		private static string getStatusCollectionOnly(FilterValueState[] filterManagerStates)
		{
			var gridFilterModeText = getFilterManagerModeDisplayText(filterManagerStates, FilterGroupCollection, true, Resources.Title_mode_no_input);

			return gridFilterModeText;
		}

		private string getStatusSearch(FilterValueState[] filterManagerStates)
		{
			if (isSearchStringModified())
				return Resources.Title_mode_search_string_is_modified;

			string noInputText;

			if (!_luceneSearcher.IsLoading && !_luceneSearcher.IsLoaded)
			{
				noInputText = Resources.Title_building_index_pending;
			}
			else if (_luceneSearcher.IsLoading)
			{
				noInputText = string.Format(
					Resources.Title_building_index,
					Resources.Title_building_index_cards,
					_luceneSearcher.SetsAddedToIndex,
					_cardRepo.SetsByCode.Count);
			}
			else if (_luceneSearcher.Spellchecker.IsLoading)
			{
				noInputText = string.Format(
					Resources.Title_building_index,
					Resources.Title_building_index_intellisense,
					_luceneSearcher.Spellchecker.IndexedFields,
					_luceneSearcher.Spellchecker.TotalFields);
			}
			else if (_searchStringSubsystem.SearchResult?.ParseErrorMessage != null)
				noInputText = Resources.Title_syntax_error;
			else
				noInputText = Resources.Title_mode_no_input;

			var searchStringText = getFilterManagerModeDisplayText(
				filterManagerStates,
				FilterGroupFind,
				isSearchStringApplied(),
				noInputText);

			return searchStringText;
		}

		private bool isSearchStringApplied()
		{
			return
				_searchStringSubsystem.SearchResult?.SearchRankById != null &&
				_luceneSearcher.Spellchecker.IsLoaded;
		}

		private string getStatusLegalityFilter(FilterValueState[] filterManagerStates)
		{
			var result = new StringBuilder();

			var gridFilterModeText = getFilterManagerModeDisplayText(filterManagerStates,
				FilterGroupLegality,
				!string.IsNullOrEmpty(_legalitySubsystem.FilterFormat),
				_legalitySubsystem.AnyFormat);

			result.Append(gridFilterModeText);

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
				result.Append(Resources.Title_Legal);

				if (_legalitySubsystem.AllowRestricted)
					result.Append(allowed);
				else
					result.Append(notAllowed);
				result.Append(Resources.Title_Restricted);

				if (_legalitySubsystem.AllowBanned)
					result.Append(allowed);
				else
					result.Append(notAllowed);
				result.Append(Resources.Title_Banned);
			}

			return result.ToString();
		}


		private bool isSearchStringModified()
		{
			return _historyModel != null && _findEditor.Text != (_historyModel.Current.Find ?? string.Empty);
		}

		private static string getFilterManagerModeDisplayText(
			FilterValueState[] filterManagerStates,
			int filterVariantIndex,
			bool hasInput,
			string noInputText)
		{
			if (hasInput)
			{
				switch (filterManagerStates[filterVariantIndex])
				{
					case FilterValueState.RequiredSome:
						return Resources.Title_mode_or;
					case FilterValueState.Required:
						return Resources.Title_mode_and;
					default:
						return Resources.Title_mode_ignored;
				}
			}

			return noInputText;
		}

		private bool isFilterGroupEnabled(int index)
		{
			var state = FilterManager.States[index];

			switch (state)
			{
				case FilterValueState.Required:
				case FilterValueState.RequiredSome:
					return true;
				default:
					return false;
			}
		}

		private bool isQuickFilteringActive()
		{
			var buttonStates = QuickFilterSetup.GetButtonStates(this);
			var result = buttonStates.Any(arr => arr.Any(_ => _ != FilterValueState.Ignored));
			return result;
		}

		private void setFilterManagerState(int i, FilterValueState value)
		{
			var states = FilterManager.States;

			if (states[i] == value)
				return;

			states[i] = value;

			beginRestoreSettings();
			FilterManager.States = states;
			endRestoreSettings();
		}



		private void historyUpdateGlobals()
		{
			var settings = _historyModel.Current;
			
			if (_uiModel.HasLanguage && _uiModel.Language != settings.Language)
				settings.Language = _uiModel.Language;

			if (_collectionModel.IsInitialized && !_collectionModel.CountById.IsEqualTo(settings.Collection))
				settings.Collection = _collectionModel.CountById.ToDictionary();

			if (_uiModel.Form.HideTooltips != settings.HideTooltips)
				settings.HideTooltips = _uiModel.Form.HideTooltips;
		}

		private void historyUpdate()
		{
			if (_historyModel == null)
				return;

			var settings = new GuiSettings
			{
				Find = _searchStringSubsystem.AppliedText,
				FilterAbility = FilterAbility.States,
				FilterMana = FilterManaCost.States,
				FilterManaAbility = FilterManaAbility.States,
				FilterManaGenerated = FilterGeneratedMana.States,
				FilterRarity = FilterRarity.States,
				FilterType = FilterType.States,
				FilterCmc = FilterCmc.States,
				FilterGrid = FilterManager.States,
				Language = _uiModel.Language,
				Deck = _deckModel.MainDeck.CountById.ToDictionary(),
				DeckOrder = _deckModel.MainDeck.CardsIds.ToList(),
				SideDeck = _deckModel.SideDeck.CountById.ToDictionary(),
				SideDeckOrder = _deckModel.SideDeck.CardsIds.ToList(),
				Collection = _collectionModel.CountById.ToDictionary(),
				ShowDuplicates = _buttonShowDuplicates.Checked,
				HideTooltips = !_toolTipController.Active,
				ExcludeManaAbilities = _buttonExcludeManaAbility.Checked,
				ExcludeManaCost = _buttonExcludeManaCost.Checked,
				ShowProhibit = _buttonShowProhibit.Checked,
				Sort = _sortSubsystem.SortString,
				LegalityFilterFormat = _legalitySubsystem.FilterFormat,
				LegalityAllowLegal = _legalitySubsystem.AllowLegal,
				LegalityAllowRestricted = _legalitySubsystem.AllowRestricted,
				LegalityAllowBanned = _legalitySubsystem.AllowBanned,
				DeckFile = _deckSerializationSubsystem.LastFile,
				SearchResultScroll = _viewCards.VisibleRecordIndex
			};

			_historyModel.Add(settings);
			historyUpdateButtons();
		}

		private void historyApply(GuiSettings settings)
		{
			beginRestoreSettings();

			if (settings.FilterAbility == null || settings.FilterAbility.Length == FilterAbility.PropertiesCount)
				FilterAbility.States = settings.FilterAbility;

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

			if (settings.FilterGrid == null || settings.FilterGrid.Length == FilterManager.PropertiesCount)
				FilterManager.States = settings.FilterGrid;

			updateTerms();

			_searchStringSubsystem.AppliedText = settings.Find ?? string.Empty;

			if (settings.Language != null)
				_uiModel.Language = settings.Language;

			_searchStringSubsystem.ApplyFind();
			_buttonShowDuplicates.Checked = settings.ShowDuplicates;

			_toolTipController.Active = !settings.HideTooltips;
			_buttonExcludeManaAbility.Checked = settings.ExcludeManaAbilities;
			_buttonExcludeManaCost.Checked = settings.ExcludeManaCost != false;
			_buttonShowProhibit.Checked = settings.ShowProhibit;
			_sortSubsystem.ApplySort(settings.Sort);

			_collectionModel.SetCollection(settings.Collection);
			loadDeck(settings);

			_legalitySubsystem.SetFilterFormat(settings.LegalityFilterFormat);
			_legalitySubsystem.SetAllowLegal(settings.LegalityAllowLegal != false);
			_legalitySubsystem.SetAllowRestricted(settings.LegalityAllowRestricted != false);
			_legalitySubsystem.SetAllowBanned(settings.LegalityAllowBanned == true);

			if (settings.SearchResultScroll.HasValue && _cardRepo.IsImageLoadingComplete)
				_viewCards.VisibleRecordIndex = settings.SearchResultScroll.Value;

			endRestoreSettings();
			
			resetTouchedCard();
			RunRefilterTask();
			historyUpdateButtons();
			setTitle(settings.DeckFile);
		}

		private void loadDeck(GuiSettings settings)
		{
			_deckModel.SetDeck(
				settings.Deck,
				settings.DeckOrder,
				settings.SideDeck,
				settings.SideDeckOrder);

			_deckModel.LoadDeck(_cardRepo);
		}

		private void historyRedo()
		{
			if (_historyModel.Redo())
				historyApply(_historyModel.Current);
		}

		private void historyUndo()
		{
			if (_historyModel.Undo())
				historyApply(_historyModel.Current);
		}

		private void historyUpdateButtons()
		{
			_uiModel.Form.CanUndo = _historyModel.CanUndo;
			_uiModel.Form.CanRedo = _historyModel.CanRedo;
		}

		private void updateTerms()
		{
			var buttonStates = QuickFilterSetup.GetButtonStates(this);
			_quickFilterFacade.ApplyValueStates(buttonStates);
		}



		private bool fit(Card card, FilterValueState[] filterManagerStates)
		{
			foreach (var ev in _evaluators)
				if ((ev.Key >= filterManagerStates.Length || filterManagerStates[ev.Key] == FilterValueState.Required) && !ev.Value(card))
					return false;

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
			_imagePreloadingSubsystem.StartThread();
			_searchStringSubsystem.StartThread();
		}

		private void stopThreads()
		{
			_imagePreloadingSubsystem.AbortThread();
			_searchStringSubsystem.AbortThread();
		}


		public void ButtonClearDeck()
		{
			_deckModel.Clear(loadingDeck: false);
			resetTouchedCard();
		}

		public void ButtonSaveDeck()
		{
			_deckSerializationSubsystem.SaveDeck(_historyModel.Current);
		}

		public void ButtonLoadDeck()
		{
			_deckSerializationSubsystem.LoadDeck();
		}

		private void deckLoaded(GuiSettings loadedDeck)
		{
			this.Invoke(delegate
			{
				loadDeck(loadedDeck);
				setTitle(_deckSerializationSubsystem.LastFile);

				historyUpdate();
			});
		}

		private void deckSaved()
		{
			this.Invoke(delegate
			{
				setTitle(_deckSerializationSubsystem.LastFile);
				_historyModel.Current.DeckFile = _deckSerializationSubsystem.LastFile;
			});
		}

		private void setTitle(string file)
		{
			if (file == null)
			{
				Text = null;
				return;
			}

			string fileName = Path.GetFileName(file);
			const int maxLength = 20;

			if (fileName.Length > maxLength)
				fileName = $"...{fileName.Substring(fileName.Length - maxLength)}";

			Text = fileName;
		}

		public void ButtonUndo()
		{
			historyUndo();
		}

		public void ButtonRedo()
		{
			historyRedo();
		}

		public void ButtonPivot()
		{
			var formPivot = new FormChart(_cardRepo);
			formPivot.Show();
		}

		public void ButtonPrint()
		{
			_printingSubsystem.ShowPrintingDialog(_deckModel);
		}





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

		public Card DraggedCard => _deckModel.CardDragged;

		private void dragCard(Card card)
		{
			if (_draggingSubsystem.IsDragging())
				_draggingSubsystem. DragAbort();

			_viewCards.Focus();
			_draggingSubsystem.DragBegin(card, _viewCards);
		}

		public void StopDragging()
		{
			_draggingSubsystem.DragAbort();
		}



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