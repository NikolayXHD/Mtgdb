using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
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
			UndoConfig undoConfig,
			CardRepository cardRepo,
			ImageRepository imageRepo,
			ImageCacheConfig imageCacheConfig,
			ImageLoader imageLoader,
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
			Application application)
			: this()
		{
			DoubleBuffered = true;
			KeyPreview = true;

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

			_buttons = new ButtonSubsystem();

			_cardSearch = new CardSearchSubsystem(
				this,
				_searchEditor,
				_panelIconSearch,
				_listBoxSuggest,
				cardSearcher,
				cardAdapter,
				_layoutViewCards,
				_layoutViewDeck);

			_panelSearchExamples.Setup(_cardSearch, _buttons, _buttonSearchExamplesDropDown);

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
				imageCacheConfig);

			_deckEditor = new DeckEditorModel();

			_dragging = new DraggingSubsystem(
				_viewDeck,
				_viewCards,
				_deckEditor,
				this,
				_imageLoader,
				application);

			_deckEditorUi = new DeckEditorUi(
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
				_menuLegalityFormat,
				_buttonLegalityAllowLegal,
				_buttonLegalityAllowRestricted,
				_buttonLegalityAllowBanned);

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

			_searchTextSelection = new RichTextBoxSelectionSubsystem(_searchEditor);

			_history = new HistorySubsystem(undoConfig);

			_evaluators = new Evaluators
			{
				{ 2, _legality.IsAllowedInFormat },
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

			setupCheckButtonImages();

			updateExcludeManaAbility();
			updateExcludeManaCost();
			updateShowProhibited();
			updateShowSampleHandButtons();
			updateDeckVisibility();

			subscribeToEvents();
		}

		private void scale()
		{
			_panelSearch.ScaleDpi();
			_panelSearchExamples.ScaleDpi();

			_menuLegalityFormat.ScaleDpi();
			_menuLegalityFormat.DropDownWidth = _menuLegalityFormat.DropDownWidth.ByDpiWidth();

			_buttonLegalityAllowLegal.ScaleDpi();
			_buttonLegalityAllowRestricted.ScaleDpi();
			_buttonLegalityAllowBanned.ScaleDpi();


			_buttonShowDuplicates.ScaleDpi();

			_buttonSampleHandNew.ScaleDpi();
			_buttonSampleHandMulligan.ScaleDpi();
			_buttonSampleHandDraw.ScaleDpi();

			_buttonHideDeck.ScaleDpi();
			_buttonShowScrollCards.ScaleDpi();
			_buttonShowScrollDeck.ScaleDpi();
			_buttonShowPartialCards.ScaleDpi();
			_buttonShowText.ScaleDpi();
			_buttonSearchExamplesDropDown.ScaleDpi();
			_buttonResetFilters.ScaleDpi();

			_tabHeadersDeck.Height = _tabHeadersDeck.Height.ByDpiHeight();
			_tabHeadersDeck.SlopeSize = _tabHeadersDeck.SlopeSize.ByDpi();
			_tabHeadersDeck.AddButtonSlopeSize = _tabHeadersDeck.AddButtonSlopeSize.ByDpi();
			_tabHeadersDeck.AddButtonWidth = _tabHeadersDeck.AddButtonWidth.ByDpiWidth();

			_listBoxSuggest.Width = _listBoxSuggest.Width.ByDpiWidth();

			foreach (var qf in _quickFilterControls.Append(FilterManager))
			{
				qf.ImageSize = qf.ImageSize.ByDpi();
				qf.HintTextShift = qf.HintTextShift.ByDpi();
				qf.HintIcon = qf.HintIcon?.ResizeDpi();
			}

			int border = FilterManaCost.Border;
			var modeButtonSize = FilterManaCost.ImageSize.Plus(new Size(border, border).MultiplyBy(2));
			int rightMargin = FilterManaCost.Width - modeButtonSize.Width;

			var modeButtons = new[]
			{
				_buttonExcludeManaAbility,
				_buttonExcludeManaCost,
				_buttonShowProhibit
			};

			foreach (var button in modeButtons)
				button.Size = modeButtonSize;

			setRowHeight(_buttonShowProhibit, modeButtonSize);

			_buttonExcludeManaCost.Margin =
				_buttonExcludeManaAbility.Margin =
					new Padding(0, 0, rightMargin, 0);

			_buttonShowProhibit.Margin = new Padding(0);

			scaleLayoutView(_layoutViewCards);
			scaleLayoutView(_layoutViewDeck);

			int deckHeight = _imageLoader.CardSize.Height + _layoutViewDeck.LayoutOptions.CardInterval.Height;

			_layoutViewDeck.Height = deckHeight;
			_deckListControl.Height = deckHeight;

			scalePanelIcon(_panelIconSearch);
			scalePanelIcon(_panelIconLegality);
			scalePanelIcon(_panelIconStatusSets);
			scalePanelIcon(_panelIconStatusCollection);
			scalePanelIcon(_panelIconStatusFilterButtons);
			scalePanelIcon(_panelIconStatusSearch);
			scalePanelIcon(_panelIconStatusFilterCollection);
			scalePanelIcon(_panelIconStatusFilterDeck);
			scalePanelIcon(_panelIconStatusFilterLegality);
			scalePanelIcon(_panelIconStatusSort);

			_deckListControl.Scale();
		}

		private void scaleLayoutView(LayoutViewControl view)
		{
			view.TransformIcons(bmp => bmp.HalfResizeDpi());
			view.LayoutOptions.PartialCardsThreshold = view.LayoutOptions.PartialCardsThreshold.ByDpi();
			view.ProbeCardCreating += probeCardCreating;
		}

		private void probeCardCreating(object view, LayoutControl probeCard)
		{
			probeCard.ScaleDpi();

			foreach (var field in probeCard.Fields)
			{
				field.SearchOptions.Button.Icon = field.SearchOptions.Button.Icon?.HalfResizeDpi();

				for (int i = 0; i < field.CustomButtons.Count; i++)
				{
					var button = field.CustomButtons[i];

					int delta = DeckEditorButtons.GetCountDelta(i);
					bool isDeck = DeckEditorButtons.IsDeck(i);

					button.Icon = button.Icon?.HalfResizeDpi(preventMoire: isDeck && Math.Abs(delta) == 1);
					button.IconTransp = button.IconTransp?.HalfResizeDpi();
				}
			}

			((CardLayoutControlBase) probeCard).Ui = _formRoot.UiModel;
		}

		private static void scalePanelIcon(BorderedPanel panel)
		{
			panel.ScaleDpi();
			panel.BackgroundImage = ((Bitmap) panel.BackgroundImage).HalfResizeDpi();
		}

		private void subscribeToEvents()
		{
			Load += formLoad;
			FormClosing += formClosing;

			_buttonExcludeManaCost.MouseDown += resetExcludeManaCost;
			_buttonExcludeManaAbility.MouseDown += resetExcludeManaAbility;
			_buttonShowDuplicates.CheckedChanged += showDuplicatesCheckedChanged;

			_dragging.SubscribeToEvents();

			_drawing.SetupDrawingCardEvent();
			_dragging.SetupDrawingDraggingMarkEvent();

			// After _deckDraggingSubsystem.SubscribeToEvents(), so that it would recapture the click on drag-n-drop before
			_deckEditorUi.SubscribeToEvents();
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

			_tooltipViewCards.SubscribeToEvents();
			_tooltipViewDeck.SubscribeToEvents();

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged += quickFiltersChanged;

			FilterManager.StateChanged += quickFilterManagerChanged;
			_buttons.SubscribeToEvents();

			System.Windows.Forms.Application.ApplicationExit += applicationExit;

			_copyPaste.SubscribeToEvents();
			_tabHeadersDeck.DragOver += deckZoneDrag;

			_buttonSampleHandNew.Click += sampleHandNew;
			_buttonSampleHandMulligan.Click += sampleHandMulligan;
			_buttonSampleHandDraw.Click += sampleHandDraw;

			_searchTextSelection.SubscribeToEvents();

			_buttonHideDeck.CheckedChanged += buttonHideDeckChanged;
			_buttonShowScrollCards.CheckedChanged += buttonShowScrollChanged;
			_buttonShowScrollDeck.CheckedChanged += buttonShowScrollChanged;
			_buttonShowPartialCards.CheckedChanged += buttonPartialCardsChanged;
			_buttonShowText.CheckedChanged += buttonHideTextChanged;
			_buttonResetFilters.Click += resetFiltersClick;

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

			ColorSchemeController.SystemColorsChanging += systemColorsChanging;
		}

		private void unsubscribeFromEvents()
		{
			Load -= formLoad;
			FormClosing -= formClosing;

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
			_tooltipViewCards.UnsubscribeFromEvents();
			_tooltipViewDeck.UnsubscribeFromEvents();

			foreach (var filterControl in _quickFilterControls)
				filterControl.StateChanged -= quickFiltersChanged;

			FilterManager.StateChanged -= quickFilterManagerChanged;
			_buttons.UnsubscribeFromEvents();

			System.Windows.Forms.Application.ApplicationExit -= applicationExit;

			_copyPaste.UnsubscribeFromEvents();
			_tabHeadersDeck.DragOver -= deckZoneDrag;

			_buttonSampleHandNew.Click -= sampleHandNew;
			_buttonSampleHandMulligan.Click -= sampleHandMulligan;
			_buttonSampleHandDraw.Click -= sampleHandDraw;

			_searchTextSelection.UnsubscribeFromEvents();

			_buttonHideDeck.CheckedChanged -= buttonHideDeckChanged;
			_buttonShowScrollCards.CheckedChanged -= buttonShowScrollChanged;
			_buttonShowScrollDeck.CheckedChanged -= buttonShowScrollChanged;
			_buttonShowPartialCards.CheckedChanged -= buttonPartialCardsChanged;
			_buttonShowText.CheckedChanged -= buttonHideTextChanged;
			_buttonResetFilters.Click -= resetFiltersClick;

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

			ColorSchemeController.SystemColorsChanging += systemColorsChanging;
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

		private IFormRoot _formRoot;

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
		private readonly DeckEditorUi _deckEditorUi;
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
		private readonly ButtonSubsystem _buttons;

		private readonly bool _luceneSearchIndexUpToDate;
		private readonly bool _spellcheckerIndexUpToDate;
		private readonly RichTextBoxSelectionSubsystem _searchTextSelection;
		private readonly CopyPasteSubsystem _copyPaste;
		private readonly FormZoom _formZoom;

		private string _deckName;
		private UiModel _uiSnapshot;
		private readonly DeckSearcher _deckSearcher;

		private const int MaxZoneIndex = (int) Zone.SampleHand;
		private const int DeckListTabIndex = MaxZoneIndex + 1;
	}
}