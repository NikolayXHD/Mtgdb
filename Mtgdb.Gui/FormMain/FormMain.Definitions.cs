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

			_luceneSearcher = luceneSearcher;
			_keywordSearcher = keywordSearcher;
			_quickFilterControls = QuickFilterSetup.GetQuickFilterControls(this);

			_cardRepo = cardRepo;
			_imageCache = imageCache;
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
				{ 1, c => _searchStringSubsystem.SearchResult?.SearchRankById?.ContainsKey(c.IndexInFile) != false }
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

			var formZoomCard = new FormZoom(_cardRepo, imageRepo, _imageCache);

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
				_imageCache);

			_deckEditingSubsystem = new DeckEditingSubsystem(
				_viewCards,
				_viewDeck,
				_deckModel,
				_collectionModel,
				_draggingSubsystem,
				Cursor,
				formZoomCard);

			_viewDeck.SetDataSource(_deckModel.DataSource);
			_viewCards.SetDataSource(_searchResultCards);

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
				_imageCache);


			_printingSubsystem = new PrintingSubsystem(imageRepo, _cardRepo);

			DoubleBuffered = true;
			KeyPreview = true;

			_buttonSubsystem = new ButtonSubsystem();
			DeckZone = Zone.Main;

			scale();
			setPanelCostWidth();

			_keywordsIndexUpToDate = _keywordSearcher.IsUpToDate;
			_luceneSearchIndexUpToDate = _luceneSearcher.IsUpToDate;
			_spellcheckerIndexUpToDate = _luceneSearcher.Spellchecker.IsUpToDate;

			_findEditorSelectionSubsystem = new RichTextBoxSelectionSubsystem(_findEditor);

			Load += formLoad;

			_historyModel = new HistoryModel(undoConfig);
		}

		private void scale()
		{
			_viewCards.PartialCardSize = _viewCards.PartialCardSize.ByDpi();
			_viewDeck.PartialCardSize = _viewDeck.PartialCardSize.ByDpi();

			_findBorderedPanel.ScaleDpi();
			_menuLegalityFormat.ScaleDpi();

			_panelIconSearch.ScaleDpi();
			_panelIconLegality.ScaleDpi();
			_panelIconStatusScrollDeck.ScaleDpi();
			_panelIconStatusScrollCards.ScaleDpi();
			_panelIconStatusSets.ScaleDpi();
			_panelIconStatusCollection.ScaleDpi();
			_panelIconStatusFilterButtons.ScaleDpi();
			_panelIconStatusSearch.ScaleDpi();
			_panelIconStatusFilterCollection.ScaleDpi();
			_panelIconStatusFilterDeck.ScaleDpi();
			_panelIconStatusFilterLegality.ScaleDpi();

			_buttonShowDuplicates.ScaleDpi();
			_buttonSampleHandNew.ScaleDpi();
			_buttonSampleHandDraw.ScaleDpi();
			_buttonSampleHandMulligan.ScaleDpi();
			_buttonHideDeck.ScaleDpi();
			_buttonHidePartialCards.ScaleDpi();
			_buttonHideText.ScaleDpi();

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

			_layoutRight.RowStyles[0].Height = _layoutRight.RowStyles[1].Height = modeButtonSize.Height;

			scaleLayoutView(_layoutViewCards);
			scaleLayoutView(_layoutViewDeck);

			_layoutViewDeck.Height = _imageCache.CardSize
				.Plus(_layoutViewDeck.LayoutOptions.CardInterval)
				.Height;

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

		private static void scaleLayoutView(LayoutViewControl view)
		{
			var searchToSortMargin =
				view.SearchOptions.ButtonMargin.Width -
				(view.SortOptions.ButtonMargin.Width + view.SortOptions.Icon.Width / 2);

			view.SortOptions.Icon = view.SortOptions.Icon?.HalfResizeDpi();
			view.SortOptions.AscIcon = view.SortOptions.AscIcon?.HalfResizeDpi();
			view.SortOptions.DescIcon = view.SortOptions.DescIcon?.HalfResizeDpi();
			view.SearchOptions.Icon = view.SearchOptions.Icon?.HalfResizeDpi();
			view.LayoutOptions.AlignTopLeftIcon = view.LayoutOptions.AlignTopLeftIcon?.HalfResizeDpi();
			view.LayoutOptions.AlignTopLeftHoveredIcon = view.LayoutOptions.AlignTopLeftHoveredIcon?.HalfResizeDpi();

			view.SearchOptions.ButtonMargin = new Size(
				searchToSortMargin + view.SortOptions.ButtonMargin.Width + view.SortOptions.Icon.Width,
				view.SearchOptions.ButtonMargin.Height);
		}

		private static void probeCardCreating(object view, LayoutControl probeCard)
		{
			probeCard.ScaleDpi();
		}

		private static void scalePanelIcon(BorderedPanel panel)
		{
			panel.BackgroundImage = ((Bitmap) panel.BackgroundImage).HalfResizeDpi();
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
			_tabHeadersDeck.AllowDrop = true;
			_layoutViewCards.AllowDrop = true;

			_layoutViewDeck.DragEnter += deckDragEnter;
			_layoutViewCards.DragEnter += deckDragEnter;
			_tabHeadersDeck.DragEnter += deckDragEnter;

			_layoutViewDeck.DragDrop += deckDragDropped;
			_layoutViewCards.DragDrop += deckDragDropped;
			_tabHeadersDeck.DragDrop += deckDragDropped;

			_tabHeadersDeck.DragOver += deckZoneDrag;

			_cardRepo.SetAdded += cardRepoSetAdded;
			_cardRepo.LocalizationLoadingComplete += localizationLoadingComplete;
			if (_cardRepo.IsImageLoadingComplete)
				imageLoadingComplete();
			else
				_cardRepo.ImageLoadingComplete += imageLoadingComplete;

			_buttonSampleHandNew.Click += sampleHandNew;
			_buttonSampleHandMulligan.Click += sampleHandMulligan;
			_buttonSampleHandDraw.Click += sampleHandDraw;

			_findEditorSelectionSubsystem.SubsribeToEvents();

			_buttonHideDeck.CheckedChanged += buttonDeckHideChanged;
			_buttonHidePartialCards.CheckedChanged += buttonPartialCardsChanged;
			_buttonHideText.CheckedChanged += buttonHideTextChanged;

			_layoutRight.SizeChanged += rightLayoutChanged;

			_layoutViewCards.ProbeCardCreating += probeCardCreating;
			_layoutViewDeck.ProbeCardCreating += probeCardCreating;
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
			_layoutViewCards.DragEnter -= deckDragEnter;
			_tabHeadersDeck.DragEnter -= deckDragEnter;

			_layoutViewDeck.DragDrop -= deckDragDropped;
			_layoutViewCards.DragDrop -= deckDragDropped;
			_tabHeadersDeck.DragDrop -= deckDragDropped;

			_tabHeadersDeck.DragOver -= deckZoneDrag;

			_cardRepo.SetAdded -= cardRepoSetAdded;
			_cardRepo.ImageLoadingComplete -= imageLoadingComplete;
			_cardRepo.LocalizationLoadingComplete -= localizationLoadingComplete;

			_buttonSampleHandNew.Click -= sampleHandNew;
			_buttonSampleHandMulligan.Click -= sampleHandMulligan;
			_buttonSampleHandDraw.Click -= sampleHandDraw;

			_findEditorSelectionSubsystem.UnsubsribeFromEvents();

			_buttonHideDeck.CheckedChanged -= buttonDeckHideChanged;
			_buttonHidePartialCards.CheckedChanged -= buttonPartialCardsChanged;
			_buttonHideText.CheckedChanged -= buttonHideTextChanged;

			_layoutRight.SizeChanged -= rightLayoutChanged;
			_layoutViewCards.ProbeCardCreating -= probeCardCreating;
			_layoutViewDeck.ProbeCardCreating -= probeCardCreating;
		}

		private Deck _requiredDeck;
		private int? _requiredScroll;

		/// <summary>
		/// Предотвращает реакцию на изменения состояния формы и её контролов.
		/// </summary>
		private int _restoringGuiSettings;

		private readonly bool _keywordsIndexUpToDate;

		private readonly CardRepository _cardRepo;
		private readonly ImageCache _imageCache;
		private readonly QuickFilterFacade _quickFilterFacade;

		private readonly Evaluators _evaluators;

		private readonly List<Card> _searchResultCards = new List<Card>();
		private readonly HashSet<Card> _filteredCards = new HashSet<Card>();

		private bool _breakRefreshing;

		private readonly HistoryModel _historyModel;
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
		private readonly LuceneSearcher _luceneSearcher;
		private readonly KeywordSearcher _keywordSearcher;

		private readonly LayoutView _viewCards;
		private readonly LayoutView _viewDeck;
		private readonly TooltipController _toolTipController;
		private readonly LayoutViewTooltip _tooltipViewCards;
		private readonly ButtonSubsystem _buttonSubsystem;

		private readonly bool _luceneSearchIndexUpToDate;
		private readonly bool _spellcheckerIndexUpToDate;
		private readonly RichTextBoxSelectionSubsystem _findEditorSelectionSubsystem;

		private bool _threadsRunning;
		private bool _isLoaded;
		private bool _isTabSelected;

		public Zone DeckZone
		{
			get { return (Zone)_tabHeadersDeck.SelectedIndex; }
			set { _tabHeadersDeck.SelectedIndex = (int) value; }
		}
	}
}
