using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		public FormMain()
		{
			InitializeComponent();
		}

		public FormMain(
			UndoConfig undoConfig,
			ViewConfig viewConfig,
			CardRepository cardRepo,
			ImageRepository imageRepo,
			ImageCacheConfig imageCacheConfig,
			ImageCache imageCache,
			SuggestModel suggestModel,
			CollectionModel collectionModel,
			LuceneSearcher luceneSearcher,
			KeywordSearcher keywordSearcher,
			TooltipController tooltipController,
			ForgeSetRepository forgeSetRepo,
			UiModel uiModel)
			: this()
		{
			_viewCards = new LayoutView(_layoutViewCards);
			_viewDeck = new LayoutView(_layoutViewDeck);

			_undoConfig = undoConfig;
			_luceneSearcher = luceneSearcher;
			_keywordSearcher = keywordSearcher;
			_quickFilterControls = QuickFilterSetup.GetQuickFilterControls(this);

			_cardRepo = cardRepo;
			_collectionModel = collectionModel;
			_uiModel = uiModel;
			_deckSerializationSubsystem = new DeckSerializationSubsystem(_cardRepo, forgeSetRepo);
			
			beginRestoreSettings();

			_sortSubsystem = new SortSubsystem(_viewCards, _cardRepo);

			QuickFilterImages.SetImages(this);
			QuickFilterSetup.SetQuickFilterProperties(this);

			_quickFilterFacade = new QuickFilterFacade(
				KeywordDefinitions.Patterns,
				KeywordDefinitions.Values,
				KeywordDefinitions.PropertyNames,
				KeywordDefinitions.PropertyNamesDisplay,
				keywordSearcher);

			_evaluators = new Evaluators
			{
				{ 2, c => _legalitySubsystem.IsAllowedInFormat(c) },
				{ 3, c => c.CollectionCount > 0 },
				{ 4, c => c.DeckCount > 0 },
				{ 0, c => _quickFilterFacade.Evaluate(c) },
				{ 1, c => _searchStringSubsystem.SearchResult?.SearchRankById?.ContainsKey(c.Id) != false }
			};

			endRestoreSettings();

			_searchStringSubsystem = new SearchStringSubsystem(
				this,
				suggestModel,
				_findEditor,
				_panelIconSearch,
				_listBoxSuggest,
				_uiModel,
				luceneSearcher,
				_viewCards);

			_toolTipController = tooltipController;
			_tooltipViewCards = new LayoutViewTooltip(_viewCards, _searchStringSubsystem);

			var formZoomCard = new FormZoom(_cardRepo, imageRepo, imageCache);

			_scrollSubsystem = new ScrollSubsystem(_viewDeck, _viewCards);

			_imagePreloadingSubsystem = new ImagePreloadingSubsystem(
				_viewCards,
				_viewDeck,
				_scrollSubsystem,
				imageCacheConfig);

			_deckModel = new DeckModel();

			_draggingSubsystem = new DraggingSubsystem(
				_viewDeck,
				_viewCards,
				_deckModel,
				this,
				imageCache);

			_deckEditingSubsystem = new DeckEditingSubsystem(
				_viewCards,
				_viewDeck,
				_deckModel,
				_collectionModel,
				_scrollSubsystem,
				_draggingSubsystem,
				Cursor,
				formZoomCard);

			_viewDeck.SetDataSource(_deckModel.DataSource);
			_viewCards.SetDataSource(_filteredCards);

			_legalitySubsystem = new LegalitySubsystem(
				_menuLegalityFormat,
				_buttonLegalityAllowLegal,
				_buttonLegalityAllowRestricted,
				_buttonLegalityAllowBanned);

			_drawingSubsystem = new DrawingSubsystem(
				_viewCards,
				_viewDeck,
				Font,
				_draggingSubsystem,
				_searchStringSubsystem,
				_deckModel,
				_quickFilterFacade,
				_legalitySubsystem,
				imageCache);


			_printingSubsystem = new PrintingSubsystem(imageRepo, _cardRepo);

			DoubleBuffered = true;
			KeyPreview = true;

			_buttonSubsystem = new ButtonSubsystem();
			_tabHeadersDeck.SelectedIndex = 0;

			applyDisplayConfig(viewConfig, imageCache);
			scale();

			_keywordsIndexUpToDate = _keywordSearcher.IsUpToDate;
			_luceneSearchIndexUpToDate = _luceneSearcher.IsUpToDate;
			_spellcheckerIndexUpToDate = _luceneSearcher.Spellchecker.IsUpToDate;

			Load += formLoad;
		}

		private void applyDisplayConfig(ViewConfig viewConfig, ImageCache imageCache)
		{
			_viewCards.AllowPartialCards = _viewDeck.AllowPartialCards =
				viewConfig.AllowPartialCards != false;

			var cardSize = imageCache.CardSize;

			if (viewConfig.ShowTextualFields == false)
				_viewCards.HideTextualFields();

			_viewCards.SetImageSize(cardSize);
			_viewDeck.SetImageSize(cardSize);
			_layoutViewDeck.Height = cardSize.Height;

			if (viewConfig.ShowDeck == false)
				_viewDeck.Control.Visible = false;
		}

		private void scale()
		{
			_viewCards.PartialCardSize = _viewCards.PartialCardSize.ByDpi();
			_viewDeck.PartialCardSize = _viewDeck.PartialCardSize.ByDpi();

			_findBorderedPanel.ScaleDpi();
			_menuLegalityFormat.ScaleDpi();

			_panelIconSearch.ScaleDpi();
			_panelIconLegality.ScaleDpi();
			_buttonShowDuplicates.ScaleDpi();
			_panelIconStatusScrollDeck.ScaleDpi();
			_panelIconStatusScrollCards.ScaleDpi();
			_panelIconStatusSets.ScaleDpi();
			_panelIconStatusCollection.ScaleDpi();
			_panelIconStatusFilterButtons.ScaleDpi();
			_panelIconStatusSearch.ScaleDpi();
			_panelIconStatusFilterCollection.ScaleDpi();
			_panelIconStatusFilterDeck.ScaleDpi();
			_panelIconStatusFilterLegality.ScaleDpi();

			_tabHeadersDeck.Height = _tabHeadersDeck.Height.ByDpiHeight();
			_tabHeadersDeck.SlopeSize = _tabHeadersDeck.SlopeSize.ByDpi();
			_tabHeadersDeck.AddButtonSlopeSize = _tabHeadersDeck.AddButtonSlopeSize.ByDpi();
			_tabHeadersDeck.AddButtonWidth = _tabHeadersDeck.AddButtonWidth.ByDpiWidth();

			_listBoxSuggest.Width = _listBoxSuggest.Width.ByDpiWidth();

			foreach (var qf in _quickFilterControls.Concat(Enumerable.Repeat(FilterManager, 1)))
			{
				qf.ImageSize = qf.ImageSize.ByDpi();
				qf.HintTextShift = qf.HintTextShift.ByDpi();
				qf.HintIcon = qf.HintIcon?.ResizeDpi();
			}

			var modeButtonSize = new Size(
				FilterManaCost.ImageSize.Width + FilterManaCost.Spacing.Width * 2,
				FilterManaCost.ImageSize.Height + FilterManaCost.Spacing.Height * 2);

			_buttonExcludeManaAbility.Size = _buttonExcludeManaCost.Size = _buttonShowProhibit.Size =
				modeButtonSize;

			_buttonExcludeManaCost.Margin = _buttonExcludeManaAbility.Margin =
				new Padding(0, 0, modeButtonSize.Width, 0);

			_layout.RowStyles[0].Height = _layout.RowStyles[1].Height = modeButtonSize.Height;

			var searchToSortMargin =
				_layoutViewCards.SearchOptions.ButtonMargin.Width -
				(_layoutViewCards.SortOptions.ButtonMargin.Width + _layoutViewCards.SortOptions.Icon.Width / 2);

			_layoutViewCards.SortOptions.Icon = _layoutViewCards.SortOptions.Icon.HalfResizeDpi();
			_layoutViewCards.SortOptions.AscIcon = _layoutViewCards.SortOptions.AscIcon.HalfResizeDpi();
			_layoutViewCards.SortOptions.DescIcon = _layoutViewCards.SortOptions.DescIcon.HalfResizeDpi();

			_layoutViewCards.SearchOptions.Icon = _layoutViewCards.SearchOptions.Icon.HalfResizeDpi();

			_layoutViewCards.SearchOptions.ButtonMargin = new Size(
				searchToSortMargin + _layoutViewCards.SortOptions.ButtonMargin.Width + _layoutViewCards.SortOptions.Icon.Width,
				_layoutViewCards.SearchOptions.ButtonMargin.Height);

			scalePanelIcon(_panelIconLegality);
			scalePanelIcon(_panelIconSearch);

			scalePanelIcon(_panelIconStatusCollection);
			scalePanelIcon(_panelIconStatusScrollCards);
			scalePanelIcon(_panelIconStatusScrollDeck);
			scalePanelIcon(_panelIconStatusSearch);
			scalePanelIcon(_panelIconStatusSets);

			scalePanelIcon(_panelIconStatusFilterButtons);
			scalePanelIcon(_panelIconStatusFilterCollection);
			scalePanelIcon(_panelIconStatusFilterDeck);
			scalePanelIcon(_panelIconStatusFilterLegality);
		}

		private static void scalePanelIcon(BorderedPanel panel)
		{
			panel.BackgroundImage = ((Bitmap) panel.BackgroundImage)
				.HalfResizeDpi();
		}

		public void SetId(string tabId)
		{
			_historyModel = new HistoryModel(tabId, _uiModel.Language, _undoConfig.MaxDepth);
		}

		private void subscribeToEvents()
		{
			FormClosing += formClosing;
			
			_buttonExcludeManaCost.MouseDown += resetExcludeManaCost;
			_buttonExcludeManaAbility.MouseDown += resetExcludeManaAbility;
			_buttonShowDuplicates.CheckedChanged += showDuplicatesCheckedChanged;

			_draggingSubsystem.SubscribeToEvents();
			
			_drawingSubsystem.SetupDrawingCardEvent();
			_draggingSubsystem.SetupDrawingDraggingMarkEvent();

			// После _deckDraggingSubsystem.SubscribeToEvents(), чтобы тот перезхватил клик при драг-дропе раньше
			_deckEditingSubsystem.SubscribeToEvents();
			// После _deckEditingSubsystem, чтобы показывать зум раньше перемещения образанной карточки
			_scrollSubsystem.SubscribeToEvents();

			_legalitySubsystem.SubscribeToEvents();
			_legalitySubsystem.FilterChanged += legalityFilterChanged;

			_deckModel.DeckChanged += deckChanged;
			_collectionModel.CollectionChanged += collectionChanged;

			_searchStringSubsystem.SubscribeToEvents();
			_searchStringSubsystem.TextApplied += searchStringApplied;
			_searchStringSubsystem.TextChanged += searchStringChanged;


			_sortSubsystem.SubscribeToEvents();
			_sortSubsystem.SortChanged += sortChanged;

			_uiModel.Form.LanguageChanged += languageChanged;
			_buttonExcludeManaAbility.CheckedChanged += excludeManaAbilityChanged;
			_buttonExcludeManaCost.CheckedChanged += excludeManaCostChanged;
			_buttonShowProhibit.CheckedChanged += showProhibitChanged;

			_tabHeadersDeck.SelectedIndexChanged += deckZoneChanged;
			_tabHeadersDeck.MouseMove += deckZoneHover;

			_luceneSearcher.IndexingProgress += luceneSearcherIndexingProgress;
			_luceneSearcher.Spellchecker.IndexingProgress += luceneSearcherIndexingProgress;
			_luceneSearcher.Loaded += luceneSearcherLoaded;
			_luceneSearcher.Disposed += luceneSearcherDisposed;


			_keywordSearcher.Loaded += keywordSearcherLoaded;
			_keywordSearcher.LoadingProgress += keywordSearcherLoadingProgress;

			_scrollSubsystem.Scrolled += gridScrolled;
			_toolTipController.SubscribeToEvents();
			_tooltipViewCards.SubscribeToEvents();

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged += quickFiltersChanged;

			FilterManager.StateChanged += quickFilterManagerChanged;
			_buttonSubsystem.SubscribeToEvents();

			Application.ApplicationExit += applicationExit;

			_layoutViewDeck.AllowDrop = true;
			_layoutViewDeck.DragEnter += deckDragEnter;
			_layoutViewDeck.DragDrop += deckDragDropped;

			_layoutViewCards.AllowDrop = true;
			_layoutViewCards.DragEnter += deckDragEnter;
			_layoutViewCards.DragDrop += deckDragDropped;

			_cardRepo.SetAdded += cardRepoSetAdded;
			_cardRepo.LocalizationLoadingComplete += localizationLoadingComplete;
			if (_cardRepo.IsImageLoadingComplete)
				imageLoadingComplete();
			else
				_cardRepo.ImageLoadingComplete += imageLoadingComplete;

			_eventsSubscribed = true;
		}

		private static void deckDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				if (files.Length < 10)
					e.Effect = DragDropEffects.Copy;
			}
			else if (e.Data.GetFormats().Contains(DataFormats.Text))
			{
				e.Effect = DragDropEffects.Copy;
			}
		}

		public bool IsSearchFocused() => _searchStringSubsystem.IsSearchFocused();

		public void PasteDeck(bool append)
		{
			if (!_cardRepo.IsImageLoadingComplete)
				return;

			var text = Clipboard.GetText();
			if (string.IsNullOrWhiteSpace(text))
				return;

			pasteDeckFromText(text, append);
		}

		private void deckDragDropped(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var files = (string[]) e.Data.GetData(DataFormats.FileDrop);
				
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
					_uiModel.Form.NewTab(form => ((FormMain)form)._requiredDeck = deck);
				}
			}
			else if (e.Data.GetFormats().Contains(DataFormats.Text))
			{
				string text = (string) e.Data.GetData(DataFormats.Text, autoConvert: true);
				pasteDeckFromText(text, append: ModifierKeys == Keys.Shift);
			}
		}

		private void pasteDeckFromText(string text, bool append)
		{
			var deck = _deckSerializationSubsystem.LoadSerialized("*.txt", text);

			if (deck.Error != null)
				MessageBox.Show(deck.Error);
			else
			{
				if (append)
					appendToDeck(deck);
				else
					loadDeck(deck);
			}
		}

		private void unsubscribeFromEvents()
		{
			Load -= formLoad;
			FormClosing -= formClosing;
			
			_buttonExcludeManaCost.MouseDown -= resetExcludeManaCost;
			_buttonExcludeManaAbility.MouseDown -= resetExcludeManaAbility;
			_buttonShowDuplicates.CheckedChanged -= showDuplicatesCheckedChanged;

			_scrollSubsystem.UnsubscribeFromEvents();
			_legalitySubsystem.FilterChanged -= legalityFilterChanged;
			_deckModel.DeckChanged -= deckChanged;
			_collectionModel.CollectionChanged -= collectionChanged;

			_searchStringSubsystem.UnsubscribeFromEvents();
			_searchStringSubsystem.TextApplied -= searchStringApplied;
			_searchStringSubsystem.TextChanged -= searchStringChanged;

			_sortSubsystem.UnsubscribeFromEvents();
			_sortSubsystem.SortChanged -= sortChanged;

			_uiModel.Form.LanguageChanged -= languageChanged;
			_buttonExcludeManaAbility.CheckedChanged -= excludeManaAbilityChanged;
			_buttonExcludeManaCost.CheckedChanged -= excludeManaCostChanged;
			_buttonShowProhibit.CheckedChanged -= showProhibitChanged;

			_tabHeadersDeck.SelectedIndexChanged -= deckZoneChanged;
			_tabHeadersDeck.MouseMove -= deckZoneHover;

			_luceneSearcher.IndexingProgress -= luceneSearcherIndexingProgress;
			_luceneSearcher.Spellchecker.IndexingProgress -= luceneSearcherIndexingProgress;
			_luceneSearcher.Loaded -= luceneSearcherLoaded;
			_luceneSearcher.Disposed -= luceneSearcherDisposed;

			_keywordSearcher.Loaded -= keywordSearcherLoaded;
			_keywordSearcher.LoadingProgress -= keywordSearcherLoadingProgress;

			_scrollSubsystem.Scrolled -= gridScrolled;
			_tooltipViewCards.UnsubscribeFromEvents();

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged -= quickFiltersChanged;

			FilterManager.StateChanged -= quickFilterManagerChanged;
			_buttonSubsystem.UnsubscribeFromEvents();

			Application.ApplicationExit -= applicationExit;

			_layoutViewDeck.DragEnter -= deckDragEnter;
			_layoutViewDeck.DragDrop -= deckDragDropped;

			_layoutViewCards.DragEnter -= deckDragEnter;
			_layoutViewCards.DragDrop -= deckDragDropped;

			_cardRepo.SetAdded -= cardRepoSetAdded;
			_cardRepo.ImageLoadingComplete -= imageLoadingComplete;
			_cardRepo.LocalizationLoadingComplete -= localizationLoadingComplete;
		}

		private Deck _requiredDeck;
		private int? _requiredScroll;

		/// <summary>
		/// Предотвращает реакцию на изменения состояния формы и её контролов.
		/// </summary>
		private int _restoringGuiSettings;

		private readonly CardRepository _cardRepo;
		private readonly QuickFilterFacade _quickFilterFacade;

		private readonly Evaluators _evaluators;

		private const int FilterGroupButtons = 0;
		private const int FilterGroupFind = 1;
		private const int FilterGroupLegality = 2;
		private const int FilterGroupCollection = 3;
		private const int FilterGroupDeck = 4;

		private readonly List<Card> _filteredCards = new List<Card>();
		private bool _breakRefreshing;

		private HistoryModel _historyModel;
		private readonly DeckSerializationSubsystem _deckSerializationSubsystem;
		private readonly SearchStringSubsystem _searchStringSubsystem;
		private readonly DeckModel _deckModel;
		private readonly ImagePreloadingSubsystem _imagePreloadingSubsystem;
		private readonly ScrollSubsystem _scrollSubsystem;
		private readonly CollectionModel _collectionModel;
		private readonly UiModel _uiModel;

		// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
		private readonly DeckEditingSubsystem _deckEditingSubsystem;
		private readonly DrawingSubsystem _drawingSubsystem;
		private readonly DraggingSubsystem _draggingSubsystem;
		private readonly SortSubsystem _sortSubsystem;
		// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

		private readonly QuickFilterControl[] _quickFilterControls;
		private readonly PrintingSubsystem _printingSubsystem;
		private readonly LegalitySubsystem _legalitySubsystem;
		private readonly UndoConfig _undoConfig;
		private readonly LuceneSearcher _luceneSearcher;
		private readonly KeywordSearcher _keywordSearcher;

		private readonly LayoutView _viewCards;
		private readonly LayoutView _viewDeck;
		private readonly TooltipController _toolTipController;
		private readonly LayoutViewTooltip _tooltipViewCards;
		private readonly ButtonSubsystem _buttonSubsystem;

		private readonly bool _luceneSearchIndexUpToDate;
		private readonly bool _spellcheckerIndexUpToDate;

		private bool _threadsRunning;
		private bool _eventsSubscribed;
	}
}
