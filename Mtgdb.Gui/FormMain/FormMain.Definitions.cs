using System.Collections.Generic;
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

		// ReSharper disable once UnusedMember.Global
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
			_deckSerializationSubsystem = new DeckSerializationSubsystem(_cardRepo);
			
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

			var formZoomCard = new FormZoomImage(_cardRepo, imageRepo, imageCache);

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

			_viewCards.AllowPartialCards = _viewDeck.AllowPartialCards =
				viewConfig.AllowPartialCards != false;

			var cardSize = imageCache.CardSize;

			int partialCardHorizontal = _viewCards.PartialCardHorizontal;
			int partialCardVertical = _viewCards.PartialCardVertical;

			_viewCards.PartialCardHorizontal = _viewDeck.PartialCardHorizontal =
				partialCardHorizontal * cardSize.Height / imageCache.CardSizeDefault.Height;

			_viewCards.PartialCardVertical = _viewDeck.PartialCardVertical =
				partialCardVertical * cardSize.Height / imageCache.CardSizeDefault.Height;


			if (viewConfig.ShowTextualFields == false)
				_viewCards.HideTextualFields();

			_viewCards.SetImageSize(cardSize);
			_viewDeck.SetImageSize(cardSize);

			int deckRowIndex = _tableRoot.GetRow(_viewDeck.Control);
			if (viewConfig.ShowDeck == false)
			{
				_viewDeck.Control.Visible = false;
				_tableRoot.RowStyles[deckRowIndex].Height = 0;
			}
			else
			{
				_tableRoot.RowStyles[deckRowIndex].Height = cardSize.Height;
			}

			_buttonSubsystem = new ButtonSubsystem();
			_tabHeadersDeck.SelectedIndex = 0;

			Load += formLoad;
		}

		public void SetId(string tabId)
		{
			_historyModel = new HistoryModel(tabId, _uiModel.Language, _undoConfig.MaxDepth);
		}

		private void subscribeToEvents()
		{
			FormClosing += formClosing;
			KeyDown += formKeyDown;

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

			_tabHeadersDeck.SelectedIndexChanged += deckAreaChanged;
			_tabHeadersDeck.MouseMove += deckAreaHover;

			_cardRepo.SetAdded += cardRepoSetAdded;
			
			_cardRepo.LocalizationLoadingComplete += localizationLoadingComplete;
			if (_cardRepo.IsImageLoadingComplete)
				imageLoadingComplete();
			else
				_cardRepo.ImageLoadingComplete += imageLoadingComplete;

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

			_deckSerializationSubsystem.DeckLoaded += deckLoaded;
			_deckSerializationSubsystem.DeckSaved += deckSaved;

			Application.ApplicationExit += applicationExit;
		}

		private void unsubscribeFromEvents()
		{
			Load -= formLoad;
			FormClosing -= formClosing;
			KeyDown -= formKeyDown;

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

			_tabHeadersDeck.SelectedIndexChanged -= deckAreaChanged;
			_tabHeadersDeck.MouseMove -= deckAreaHover;

			_cardRepo.SetAdded -= cardRepoSetAdded;
			_cardRepo.ImageLoadingComplete -= imageLoadingComplete;
			_cardRepo.LocalizationLoadingComplete -= localizationLoadingComplete;

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

			_deckSerializationSubsystem.DeckLoaded -= deckLoaded;
			_deckSerializationSubsystem.DeckSaved -= deckSaved;

			Application.ApplicationExit -= applicationExit;
		}

		private readonly CardRepository _cardRepo;
		private readonly QuickFilterFacade _quickFilterFacade;

		/// <summary>
		/// Предотвращает реакцию на изменения состояния формы и её контролов.
		/// </summary>
		private int _restoringGuiSettings;

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
	}
}
