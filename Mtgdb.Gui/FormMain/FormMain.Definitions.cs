using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using Mtgdb.Data.Model;
using Mtgdb.Ui;

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

			_cardSearcher = cardSearcher;
			_keywordSearcher = keywordSearcher;
			_quickFilterControls = QuickFilterSetup.GetQuickFilterControls(this);

			_cardRepo = cardRepo;
			_imageLoader = imageLoader;
			_uiConfigRepository = uiConfigRepository;
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

			_searchSubsystem = new CardSearchSubsystem(
				this,
				_searchBar,
				uiConfigRepository,
				cardSearcher,
				cardAdapter,
				_viewCards,
				_viewDeck);

			_cardSort = new CardSortSubsystem(_viewCards, _cardRepo, _fields, _searchSubsystem);

			endRestoreSettings();

			_countInputSubsystem = new CountInputSubsystem();
			_tooltipViewCards = new LayoutViewTooltip(this, _viewCards, _searchSubsystem, _countInputSubsystem);
			_tooltipViewDeck = new LayoutViewTooltip(this, _viewDeck, _searchSubsystem, _countInputSubsystem);

			_formZoom = new FormZoom(_cardRepo, imageRepo, _imageLoader);

			_imagePreloading = new ImagePreloadingSubsystem(
				_viewCards,
				_viewDeck,
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
				_countInputSubsystem,
				Cursor,
				_formZoom,
				this);

			_viewDeck.DataSource = _deckEditor.DataSource;
			_viewCards.DataSource = _searchResultCards;

			_legality = new LegalitySubsystem(
				_dropdownLegality,
				_buttonLegalityAllowLegal,
				_buttonLegalityAllowRestricted,
				_buttonLegalityAllowBanned,
				_buttonLegalityAllowFuture);

			_drawing = new DrawingSubsystem(
				_viewCards,
				_viewDeck,
				_searchSubsystem,
				cardAdapter,
				_deckEditor,
				_countInputSubsystem,
				_quickFilterFacade,
				_legality,
				iconRecognizer);


			_printing = new PrintingSubsystem(imageRepo, _cardRepo);

			_deckZones = new DeckZoneSubsystem(
				_tabHeadersDeck,
				_dragging,
				_viewDeck)
			{
				DeckZone = Zone.Main
			};

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
			_deckListControl.Init(
				deckListModel,
				iconRecognizer,
				_deckSearcher,
				deckAdapter,
				collectionEditor,
				uiConfigRepository,
				this);

			_copyPaste = new CopyPasteSubsystem(
				_cardRepo,
				_serialization,
				_collectionEditor,
				_deckEditor,
				this,
				_deckListControl,
				_viewDeck,
				_tabHeadersDeck,
				_viewCards,
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
			components.Add(_countInputSubsystem);

			_popupSearchExamples.MenuControl = _menuSearchExamples;
			_popupSearchExamples.MenuAlignment = HorizontalAlignment.Right;
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
			_deckZones.IsDeckListSelected;

		public Zone? DeckZone =>
			_deckZones.DeckZone;

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
		private readonly UiConfigRepository _uiConfigRepository;
		private readonly QuickFilterFacade _quickFilterFacade;

		private readonly List<Card> _searchResultCards = new List<Card>();
		private readonly HashSet<Card> _filteredCards = new HashSet<Card>();

		private readonly HistorySubsystem _history;
		private readonly DeckSerializationSubsystem _serialization;
		private readonly MtgArenaIntegration _mtgArenaIntegration;
		private readonly DeckEditorModel _deckEditor;
		private readonly ImagePreloadingSubsystem _imagePreloading;
		private readonly CollectionEditorModel _collectionEditor;
		private readonly CardSearchSubsystem _searchSubsystem;

		// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
		private readonly DeckEditorSubsystem _deckEditorSubsystem;
		private readonly CountInputSubsystem _countInputSubsystem;
		private readonly DrawingSubsystem _drawing;
		private readonly DraggingSubsystem _dragging;

		private readonly CardSortSubsystem _cardSort;
		// ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

		private readonly QuickFilterControl[] _quickFilterControls;
		private readonly PrintingSubsystem _printing;
		private readonly LegalitySubsystem _legality;
		private readonly CardSearcher _cardSearcher;
		private readonly KeywordSearcher _keywordSearcher;

		private readonly LayoutViewTooltip _tooltipViewCards;
		private readonly LayoutViewTooltip _tooltipViewDeck;

		private readonly bool _luceneSearchIndexUpToDate;
		private readonly bool _spellcheckerIndexUpToDate;
		private readonly CopyPasteSubsystem _copyPaste;
		private readonly FormZoom _formZoom;

		private string _deckName;
		private UiModel _uiSnapshot;
		private readonly DeckSearcher _deckSearcher;
		private readonly DeckZoneSubsystem _deckZones;

		// ReSharper disable ConvertToAutoProperty
		public QuickFilterControl FilterManaCost => _filterManaCost;
		public QuickFilterControl FilterAbility => _filterAbility;
		public QuickFilterControl FilterRarity => _filterRarity;
		public QuickFilterControl FilterType => _filterType;
		public QuickFilterControl FilterGeneratedMana => _filterGeneratedMana;
		public QuickFilterControl FilterLayout => _filterLayout;
		public QuickFilterControl FilterCastKeyword => _filterCastKeyword;
		public QuickFilterControl FilterCmc => _filterCmc;
		public QuickFilterControl FilterManaAbility => _filterManaAbility;
		public QuickFilterControl FilterManager => _filterManager;
		// ReSharper restore ConvertToAutoProperty
	}
}
