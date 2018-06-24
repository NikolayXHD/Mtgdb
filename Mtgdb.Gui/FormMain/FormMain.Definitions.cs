using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using JetBrains.Annotations;
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

		[UsedImplicitly]
		public FormMain(
			UndoConfig undoConfig,
			CardRepository cardRepo,
			ImageRepository imageRepo,
			ImageCacheConfig imageCacheConfig,
			ImageLoader imageLoader,
			CollectionModel collectionModel,
			LuceneSearcher luceneSearcher,
			KeywordSearcher keywordSearcher,
			ForgeSetRepository forgeSetRepo,
			FormManager formManager)
			: this()
		{
			DoubleBuffered = true;
			KeyPreview = true;

			_viewCards = new MtgLayoutView(_layoutViewCards);
			_viewDeck = new MtgLayoutView(_layoutViewDeck);

			_luceneSearcher = luceneSearcher;
			_keywordSearcher = keywordSearcher;
			_quickFilterControls = QuickFilterSetup.GetQuickFilterControls(this);

			_cardRepo = cardRepo;
			_imageLoader = imageLoader;
			_collectionModel = collectionModel;
			_deckSerializationSubsystem = new DeckSerializationSubsystem(_cardRepo, forgeSetRepo);

			beginRestoreSettings();

			_fields = new Fields();

			QuickFilterSetup.SetQuickFilterProperties(this);
			QuickFilterImages.SetImages(this);

			_quickFilterFacade = new QuickFilterFacade(
				KeywordDefinitions.Patterns,
				KeywordDefinitions.Values.ToKeywordDisplayTexts(),
				KeywordDefinitions.PropertyNames,
				KeywordDefinitions.PropertyNamesDisplay,
				keywordSearcher);

			_buttonSubsystem = new ButtonSubsystem();

			_searchSubsystem = new SearchStringSubsystem(
				this,
				_searchEditor,
				_panelIconSearch,
				_listBoxSuggest,
				luceneSearcher,
				_viewCards,
				_viewDeck);

			_panelSearchExamples.Setup(_searchSubsystem, _buttonSubsystem, _buttonSearchExamplesDropDown);

			_sortSubsystem = new SortSubsystem(_viewCards, _cardRepo, _fields, _searchSubsystem);

			endRestoreSettings();

			_tooltipViewCards = new LayoutViewTooltip(this, _viewCards, _searchSubsystem);
			_tooltipViewDeck = new LayoutViewTooltip(this, _viewDeck, _searchSubsystem);

			var formZoomCard = new FormZoom(_cardRepo, imageRepo, _imageLoader);

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
				_imageLoader,
				formManager);

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
				_draggingSubsystem,
				_searchSubsystem,
				_deckModel,
				_quickFilterFacade,
				_legalitySubsystem,
				_imageLoader);


			_printingSubsystem = new PrintingSubsystem(imageRepo, _cardRepo);

			DeckZone = Zone.Main;

			scale();
			setRightPanelsWidth();

			_keywordsIndexUpToDate = _keywordSearcher.IsUpToDate;
			_luceneSearchIndexUpToDate = _luceneSearcher.IsUpToDate;
			_spellcheckerIndexUpToDate = _luceneSearcher.Spellchecker.IsUpToDate;

			_searchEditorSelectionSubsystem = new RichTextBoxSelectionSubsystem(_searchEditor);

			_historySubsystem = new HistorySubsystem(undoConfig);

			_evaluators = new Evaluators
			{
				{ 2, _legalitySubsystem.IsAllowedInFormat },
				{ 3, evalFilterByCollection },
				{ 4, evalFilterByDeck },
				{ 0, _quickFilterFacade.Evaluate },
				{ 1, evalFilterBySearchText }
			};

			setupCheckButtonImages();
			
			updateExcludeManaAbility();
			updateExcludeManaCost();
			updateShowProhibited();
			updateShowSampleHandButtons();
			updateDeckListVisibility();
			subscribeToEvents();
		}

		private void scale()
		{
			_viewCards.PartialCardSize = _viewCards.PartialCardSize.ByDpi();
			_viewDeck.PartialCardSize = _viewDeck.PartialCardSize.ByDpi();

			_panelSearch.ScaleDpi();
			_panelSearchExamples.ScaleDpi();

			_menuLegalityFormat.ScaleDpi();

			_buttonShowDuplicates.ScaleDpi();
			
			_buttonSampleHandNew.ScaleDpi();
			_buttonSampleHandMulligan.ScaleDpi();
			_buttonSampleHandDraw.ScaleDpi();
			
			_buttonHideDeck.ScaleDpi();
			_buttonHideScroll.ScaleDpi();
			_buttonHidePartialCards.ScaleDpi();
			_buttonHideText.ScaleDpi();
			_buttonSearchExamplesDropDown.ScaleDpi();

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

			var modeButtonSize = new Size(
				FilterManaCost.ImageSize.Width + border * 2,
				FilterManaCost.ImageSize.Height + border * 2);

			_buttonExcludeManaAbility.Size =
				_buttonExcludeManaCost.Size =
					_buttonShowProhibit.Size = modeButtonSize;

			setRowHeight(_buttonShowProhibit, modeButtonSize);

			int rightMargin = FilterManaCost.Margin.Right + FilterManaCost.Width - modeButtonSize.Width;

			_buttonExcludeManaCost.Margin =
				_buttonExcludeManaAbility.Margin =
					new Padding(0, 0, rightMargin, 0);

			scaleLayoutView(_layoutViewCards);
			scaleLayoutView(_layoutViewDeck);

			_panelDeck.Height = _imageLoader.CardSize.Height + _layoutViewDeck.LayoutOptions.CardInterval.Height;

			scalePanelIcon(_panelIconSearch);
			scalePanelIcon(_panelIconLegality);
			scalePanelIcon(_panelIconStatusScrollDeck);
			scalePanelIcon(_panelIconStatusScrollCards);
			scalePanelIcon(_panelIconStatusSets);
			scalePanelIcon(_panelIconStatusCollection);
			scalePanelIcon(_panelIconStatusFilterButtons);
			scalePanelIcon(_panelIconStatusSearch);
			scalePanelIcon(_panelIconStatusFilterCollection);
			scalePanelIcon(_panelIconStatusFilterDeck);
			scalePanelIcon(_panelIconStatusFilterLegality);
			scalePanelIcon(_panelIconStatusSort);
		}

		private static void scaleLayoutView(LayoutViewControl view)
		{
			var sortIcon = view.SortOptions.Icon;

			view.SortOptions.Icon = sortIcon.HalfResizeDpi();
			view.SortOptions.AscIcon = view.SortOptions.AscIcon?.HalfResizeDpi();
			view.SortOptions.DescIcon = view.SortOptions.DescIcon?.HalfResizeDpi();
			view.SearchOptions.Button.Icon = view.SearchOptions.Button.Icon?.HalfResizeDpi();

			view.LayoutOptions.AlignTopLeftIcon = view.LayoutOptions.AlignTopLeftIcon?.HalfResizeDpi();
			view.LayoutOptions.AlignTopLeftHoveredIcon = view.LayoutOptions.AlignTopLeftHoveredIcon?.HalfResizeDpi();
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

			_sortSubsystem.SubscribeToEvents();
			_sortSubsystem.SortChanged += sortChanged;

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

			_tooltipViewCards.SubscribeToEvents();
			_tooltipViewDeck.SubscribeToEvents();

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

			_buttonSampleHandNew.Click += sampleHandNew;
			_buttonSampleHandMulligan.Click += sampleHandMulligan;
			_buttonSampleHandDraw.Click += sampleHandDraw;

			_searchEditorSelectionSubsystem.SubsribeToEvents();

			_buttonHideDeck.CheckedChanged += buttonHideDeckChanged;
			_buttonHideScroll.CheckedChanged += buttonHideScrollChanged;
			_buttonHidePartialCards.CheckedChanged += buttonPartialCardsChanged;
			_buttonHideText.CheckedChanged += buttonHideTextChanged;

			_layoutRight.SizeChanged += rightLayoutChanged;

			_layoutViewCards.ProbeCardCreating += probeCardCreating;
			_layoutViewDeck.ProbeCardCreating += probeCardCreating;

			_historySubsystem.Loaded += historyLoaded;

			_searchSubsystem.SubscribeToEvents();

			SizeChanged += sizeChanged;
			PreviewKeyDown += previewKeyDown;
		}

		private void unsubscribeFromEvents()
		{
			Load -= formLoad;
			FormClosing -= formClosing;

			_buttonExcludeManaCost.MouseDown -= resetExcludeManaCost;
			_buttonExcludeManaAbility.MouseDown -= resetExcludeManaAbility;
			_buttonShowDuplicates.CheckedChanged -= showDuplicatesCheckedChanged;

			_draggingSubsystem.UnsubscribeFromEvents();

			_scrollSubsystem.UnsubscribeFromEvents();

			_legalitySubsystem.FilterChanged -= legalityFilterChanged;
			_deckModel.DeckChanged -= deckChanged;
			_collectionModel.CollectionChanged -= collectionChanged;

			_sortSubsystem.UnsubscribeFromEvents();
			_sortSubsystem.SortChanged -= sortChanged;

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
			_tooltipViewDeck.UnsubscribeFromEvents();

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

			_buttonSampleHandNew.Click -= sampleHandNew;
			_buttonSampleHandMulligan.Click -= sampleHandMulligan;
			_buttonSampleHandDraw.Click -= sampleHandDraw;

			_searchEditorSelectionSubsystem.UnsubsribeFromEvents();

			_buttonHideDeck.CheckedChanged -= buttonHideDeckChanged;
			_buttonHideScroll.CheckedChanged -= buttonHideScrollChanged;
			_buttonHidePartialCards.CheckedChanged -= buttonPartialCardsChanged;
			_buttonHideText.CheckedChanged -= buttonHideTextChanged;

			_layoutRight.SizeChanged -= rightLayoutChanged;
			_layoutViewCards.ProbeCardCreating -= probeCardCreating;
			_layoutViewDeck.ProbeCardCreating -= probeCardCreating;
			_historySubsystem.Loaded -= historyLoaded;

			_searchSubsystem.UnsubscribeFromEvents();

			SizeChanged -= sizeChanged;
			PreviewKeyDown -= previewKeyDown;
		}

		private Zone DeckZone
		{
			get
			{
				int zoneIndex = _tabHeadersDeck.SelectedIndex;

				if (zoneIndex > MaxZoneIndex)
					return default(Zone);

				return (Zone) zoneIndex;
			}

			set => _tabHeadersDeck.SelectedIndex = (int) value;
		}

		private IFormRoot _formRoot;

		private bool _threadsRunning;
		private bool _isTabSelected;
		private bool _breakRefreshing;

		private Deck _requiredDeck;
		private int? _requiredScroll;

		/// <summary>
		/// Предотвращает реакцию на изменения состояния формы и её контролов.
		/// </summary>
		private int _restoringGuiSettings;

		private readonly Evaluators _evaluators;
		private readonly Fields _fields;
		private readonly bool _keywordsIndexUpToDate;

		private readonly CardRepository _cardRepo;
		private readonly ImageLoader _imageLoader;
		private readonly QuickFilterFacade _quickFilterFacade;

		private readonly List<Card> _searchResultCards = new List<Card>();
		private readonly HashSet<Card> _filteredCards = new HashSet<Card>();

		private readonly HistorySubsystem _historySubsystem;
		private readonly DeckSerializationSubsystem _deckSerializationSubsystem;
		private readonly DeckModel _deckModel;
		private readonly ImagePreloadingSubsystem _imagePreloadingSubsystem;
		private readonly ScrollSubsystem _scrollSubsystem;
		private readonly CollectionModel _collectionModel;
		private readonly SearchStringSubsystem _searchSubsystem;

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

		private readonly MtgLayoutView _viewCards;
		private readonly MtgLayoutView _viewDeck;
		private readonly LayoutViewTooltip _tooltipViewCards;
		private readonly LayoutViewTooltip _tooltipViewDeck;
		private readonly ButtonSubsystem _buttonSubsystem;

		private readonly bool _luceneSearchIndexUpToDate;
		private readonly bool _spellcheckerIndexUpToDate;
		private readonly RichTextBoxSelectionSubsystem _searchEditorSelectionSubsystem;

		private const int MaxZoneIndex = (int) Zone.SampleHand;
		private const int DeckListTabIndex = MaxZoneIndex + 1;
	}
}