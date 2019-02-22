using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Ui;
using CheckBox = Mtgdb.Controls.CheckBox;

namespace Mtgdb.Gui
{
	partial class FormMain
	{
		public FormMain()
		{
			InitializeComponent();
		}

		[UsedImplicitly]
		public FormMain(
			CardRepository cardRepo,
			ImageRepository imageRepo,
			ImageLoader imageLoader,
			UiConfigRepository uiConfigRepository,
			CollectionEditorModel collectionEditor,
			CardSearcher cardSearcher,
			CardDocumentAdapter cardAdapter,
			DeckDocumentAdapter deckAdapter,
			KeywordSearcher keywordSearcher,
			DeckListModel deckListModel,
			DeckSearcher deckSearcher,
			IconRecognizer iconRecognizer,
			DeckSerializationSubsystem serialization,
			MtgArenaIntegration mtgArenaIntegration,
			App app)
			: this()
		{
			DoubleBuffered = true;

			_viewCards = new MtgLayoutView(_layoutViewCards);
			_viewDeck = new MtgLayoutView(_layoutViewDeck);

			_cardSearcher = cardSearcher;
			_keywordSearcher = keywordSearcher;
			_quickFilterControls = QuickFilterSetup.GetQuickFilterControls(this);

			_cardRepo = cardRepo;
			_imageLoader = imageLoader;
			_collectionEditor = collectionEditor;
			_serialization = serialization;
			_mtgArenaIntegration = mtgArenaIntegration;

			beginRestoreSettings();

			_fields = new CardFields();

			QuickFilterSetup.SetQuickFilterProperties(this);
			QuickFilterImages.SetImages(this);

			_quickFilterFacade = new QuickFilterFacade(
				KeywordDefinitions.Patterns,
				KeywordDefinitions.Values.ToKeywordDisplayTexts(),
				KeywordDefinitions.PropertyNames,
				KeywordDefinitions.PropertyNamesDisplay,
				keywordSearcher);

			_cardSearch = new CardSearchSubsystem(
				this,
				_searchBar,
				cardSearcher,
				cardAdapter,
				_layoutViewCards,
				_layoutViewDeck);

			_cardSort = new CardSortSubsystem(_layoutViewCards, _cardRepo, _fields, _cardSearch);

			endRestoreSettings();

			_tooltipViewCards = new LayoutViewTooltip(this, _viewCards, _cardSearch);
			_tooltipViewDeck = new LayoutViewTooltip(this, _viewDeck, _cardSearch);

			_formZoom = new FormZoom(_cardRepo, imageRepo, _imageLoader);

			_scroll = new ScrollSubsystem(_viewDeck, _viewCards);

			_imagePreloading = new ImagePreloadingSubsystem(
				_viewCards,
				_viewDeck,
				_scroll,
				uiConfigRepository);

			_deckEditor = new DeckEditorModel();

			_dragging = new DraggingSubsystem(
				_viewDeck,
				_viewCards,
				_deckEditor,
				this,
				_imageLoader,
				app);

			_deckEditorSubsystem = new DeckEditorSubsystem(
				_viewCards,
				_viewDeck,
				_deckEditor,
				_collectionEditor,
				_dragging,
				Cursor,
				_formZoom,
				this);

			_viewDeck.SetDataSource(_deckEditor.DataSource);
			_viewCards.SetDataSource(_searchResultCards);

			_legality = new LegalitySubsystem(
				_dropdownLegality,
				_buttonLegalityAllowLegal,
				_buttonLegalityAllowRestricted,
				_buttonLegalityAllowBanned,
				_buttonLegalityAllowFuture);

			_drawing = new DrawingSubsystem(
				_viewCards,
				_viewDeck,
				_dragging,
				_cardSearch,
				cardAdapter,
				_deckEditor,
				_quickFilterFacade,
				_legality,
				_imageLoader,
				iconRecognizer);


			_printing = new PrintingSubsystem(imageRepo, _cardRepo);

			DeckZone = Zone.Main;

			scale();
			setRightPanelsWidth();

			_keywordsIndexUpToDate = _keywordSearcher.IsUpToDate;
			_luceneSearchIndexUpToDate = _cardSearcher.IsUpToDate;
			_spellcheckerIndexUpToDate = _cardSearcher.Spellchecker.IsUpToDate;

			_history = new HistorySubsystem(uiConfigRepository);

			_evaluators = new Evaluators
			{
				{ 2, _legality.MatchesLegalityFilter },
				{ 3, evalFilterByCollection },
				{ 4, evalFilterByDeck },
				{ 0, _quickFilterFacade.Evaluate },
				{ 1, evalFilterBySearchText }
			};

			_deckSearcher = deckSearcher;
			_deckListControl.Init(deckListModel,
				iconRecognizer,
				_deckSearcher,
				deckAdapter,
				collectionEditor,
				this);

			_copyPaste = new CopyPasteSubsystem(
				_cardRepo,
				_serialization,
				_collectionEditor,
				_deckEditor,
				this,
				_deckListControl,
				_layoutViewDeck,
				_tabHeadersDeck,
				_layoutViewCards,
				_deckListControl.DeckListView);

			updateExcludeManaAbility();
			updateExcludeManaCost();
			updateShowProhibited();
			updateShowSampleHandButtons();
			updateDeckVisibility();

			subscribeToEvents();

			if (components == null)
				components = new Container();

			components.Add(_deckEditorSubsystem);

			_popupSearchExamples.MenuControl = _menuSearchExamples;
			_popupSearchExamples.MenuAlignment = HorizontalAlignment.Right;
		}

		private void cardCreating(object view, LayoutControl probeCard) =>
			((CardLayoutControlBase) probeCard).Ui = () => _formRoot.UiModel;

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
			_scroll.SubscribeToEvents();

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
			_tabHeadersDeck.MouseMove += deckZoneHover;
			_tabHeadersDeck.Click += deckZoneClick;

			_cardSearcher.IndexingProgress += cardSearcherIndexingProgress;
			_cardSearcher.Spellchecker.IndexingProgress += cardSearcherIndexingProgress;
			_cardSearcher.Loaded += cardSearcherLoaded;
			_cardSearcher.Disposed += cardSearcherDisposed;


			_keywordSearcher.Loaded += keywordSearcherLoaded;
			_keywordSearcher.LoadingProgress += keywordSearcherLoadingProgress;

			_scroll.Scrolled += gridScrolled;

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged += quickFiltersChanged;

			FilterManager.StateChanged += quickFilterManagerChanged;

			Application.ApplicationExit += applicationExit;

			_copyPaste.SubscribeToEvents();
			_tabHeadersDeck.DragOver += deckZoneDrag;

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

			_cardSearch.SubscribeToEvents();

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

			_layoutViewCards.CardCreating += cardCreating;
			_layoutViewDeck.CardCreating += cardCreating;

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

			_scroll.UnsubscribeFromEvents();

			_legality.FilterChanged -= legalityFilterChanged;
			_deckEditor.DeckChanged -= deckChanged;
			_collectionEditor.CollectionChanged -= collectionChanged;

			_cardSort.UnsubscribeFromEvents();
			_cardSort.SortChanged -= cardSortChanged;

			_buttonExcludeManaAbility.CheckedChanged -= excludeManaAbilityChanged;
			_buttonExcludeManaCost.CheckedChanged -= excludeManaCostChanged;
			_buttonShowProhibit.CheckedChanged -= showProhibitChanged;

			_tabHeadersDeck.SelectedIndexChanged -= deckZoneChanged;
			_tabHeadersDeck.MouseMove -= deckZoneHover;
			_tabHeadersDeck.Click -= deckZoneClick;

			_cardSearcher.IndexingProgress -= cardSearcherIndexingProgress;
			_cardSearcher.Spellchecker.IndexingProgress -= cardSearcherIndexingProgress;
			_cardSearcher.Loaded -= cardSearcherLoaded;
			_cardSearcher.Disposed -= cardSearcherDisposed;

			_keywordSearcher.Loaded -= keywordSearcherLoaded;
			_keywordSearcher.LoadingProgress -= keywordSearcherLoadingProgress;

			_scroll.Scrolled -= gridScrolled;

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged -= quickFiltersChanged;

			FilterManager.StateChanged -= quickFilterManagerChanged;

			Application.ApplicationExit -= applicationExit;

			_copyPaste.UnsubscribeFromEvents();
			_tabHeadersDeck.DragOver -= deckZoneDrag;

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

			_cardSearch.UnsubscribeFromEvents();

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

			_layoutViewCards.CardCreating -= cardCreating;
			_layoutViewDeck.CardCreating -= cardCreating;

			_menuSearchExamples.QueryClicked -= searchExampleClicked;

			Dpi.BeforeChanged -= beforeDpiChanged;
			Dpi.AfterChanged -= afterDpiChanged;
		}

		public Zone? DeckZone
		{
			get
			{
				int zoneIndex = _tabHeadersDeck.SelectedIndex;

				if (zoneIndex > MaxZoneIndex)
					return null;

				return (Zone) zoneIndex;
			}

			set => _tabHeadersDeck.SelectedIndex = (int) value;
		}

		public string DeckName
		{
			get => _deckName;
			private set
			{
				_deckName = value;
				Text = _serialization.GetShortDisplayName(value);
			}
		}

		public bool IsDeckListSelected =>
			_tabHeadersDeck.SelectedIndex == DeckListTabIndex;

		private FormRoot _formRoot;

		private bool _threadsRunning;
		private bool _isTabSelected;
		private bool _breakRefreshing;

		private Deck _requiredDeck;
		private int? _requiredScroll;

		private bool _updatingButtonHideScroll;

		/// <summary>
		/// Prevents handling the changes in the state of form or its controls
		/// </summary>
		private int _restoringGuiSettings;

		private readonly Evaluators _evaluators;
		private readonly CardFields _fields;
		private readonly bool _keywordsIndexUpToDate;

		private readonly CardRepository _cardRepo;
		private readonly ImageLoader _imageLoader;
		private readonly QuickFilterFacade _quickFilterFacade;

		private readonly List<Card> _searchResultCards = new List<Card>();
		private readonly HashSet<Card> _filteredCards = new HashSet<Card>();

		private readonly HistorySubsystem _history;
		private readonly DeckSerializationSubsystem _serialization;
		private readonly MtgArenaIntegration _mtgArenaIntegration;
		private readonly DeckEditorModel _deckEditor;
		private readonly ImagePreloadingSubsystem _imagePreloading;
		private readonly ScrollSubsystem _scroll;
		private readonly CollectionEditorModel _collectionEditor;
		private readonly CardSearchSubsystem _cardSearch;

		// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
		private readonly DeckEditorSubsystem _deckEditorSubsystem;
		private readonly DrawingSubsystem _drawing;
		private readonly DraggingSubsystem _dragging;

		private readonly CardSortSubsystem _cardSort;
		// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

		private readonly QuickFilterControl[] _quickFilterControls;
		private readonly PrintingSubsystem _printing;
		private readonly LegalitySubsystem _legality;
		private readonly CardSearcher _cardSearcher;
		private readonly KeywordSearcher _keywordSearcher;

		private readonly MtgLayoutView _viewCards;
		private readonly MtgLayoutView _viewDeck;
		private readonly LayoutViewTooltip _tooltipViewCards;
		private readonly LayoutViewTooltip _tooltipViewDeck;

		private readonly bool _luceneSearchIndexUpToDate;
		private readonly bool _spellcheckerIndexUpToDate;
		private readonly CopyPasteSubsystem _copyPaste;
		private readonly FormZoom _formZoom;

		private string _deckName;
		private UiModel _uiSnapshot;
		private readonly DeckSearcher _deckSearcher;

		private const int MaxZoneIndex = (int) Zone.SampleHand;
		private const int DeckListTabIndex = MaxZoneIndex + 1;
	}
}