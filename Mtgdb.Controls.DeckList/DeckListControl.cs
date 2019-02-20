using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Dal;
using Mtgdb.Ui;

namespace Mtgdb.Controls
{
	public partial class DeckListControl : UserControl
	{
		public event Action<object> Refreshed;
		public event Action<object> DeckTransformed;
		public event Action<object> Scrolled;
		public event Action<object, Deck, bool> DeckOpened;
		public event Action<object, Deck> DeckRenamed;
		public event Action<object> FilterByDeckModeChanged;
		public event Action<object> DeckAdded;

		public DeckListControl()
		{
			InitializeComponent();
			_viewDeck.LayoutControlType = typeof(DeckListLayout);
		}

		public void Init(
			DeckListModel decks,
			IconRecognizer recognizer,
			DeckSearcher searcher,
			DeckDocumentAdapter adapter,
			CollectionEditorModel collection,
			Control tooltipOwner)
		{
			_searcher = searcher;
			_panelRename.Visible = false;

			_viewDeck.IconRecognizer = recognizer;
			_viewDeck.DataSource = _filteredModels;

			_menuFilterByDeckMode.SetMenuValues(
				"Ignored",
				"Cards in currently open deck",
				"Cards in saved decks matching filter");

			_menuFilterByDeckMode.SelectedIndex = 0;

			_listModel = decks;
			_tooltipOwner = tooltipOwner;
			_collection = collection;

			_searchSubsystem = new DeckSearchSubsystem(this, _searchBar, _searcher, adapter, _viewDeck);
			_deckSort = new DeckSortSubsystem(_viewDeck, new DeckFields(), _searchSubsystem, _listModel);
			_layoutViewTooltip = new ViewDeckListTooltips(_tooltipOwner, _viewDeck);

			_model = _listModel.CreateModel(Deck.Create());
			_model.IsCurrent = true;

			subscribeToEvents();

			updateSortLabel();

			if (components == null)
				components = new Container();

			components.Add(new SearchResultHighlightSubsystem(_viewDeck, _searchSubsystem, adapter));

			_searchSubsystem.Apply();
		}

		private void subscribeToEvents()
		{
			_menuFilterByDeckMode.SelectedIndexChanged += filterByDeckModeChanged;

			_searchSubsystem.SubscribeToEvents();
			_deckSort.SubscribeToEvents();

			_searchSubsystem.TextApplied += searchTextApplied;
			_deckSort.SortChanged += sortChanged;

			_textboxRename.LostFocus += nameLostFocus;
			_textboxRename.KeyDown += nameKeyDown;

			_viewDeck.MouseClicked += viewDeckClicked;
			_viewDeck.RowDataLoaded += viewDeckRowDataLoaded;
			_viewDeck.CardIndexChanged += viewScrolled;
			_viewDeck.MouseMove += deckMouseMove;

			if (_listModel.IsLoaded)
				listModelLoaded();
			else
				_listModel.Loaded += listModelLoaded;

			if (_searcher.IsLoaded)
				searcherLoaded();
			else
				_searcher.Loaded += searcherLoaded;

			ColorSchemeController.SystemColorsChanged += systemColorsChanged;
		}

		public void UnsubscribeFromEvents()
		{
			_menuFilterByDeckMode.SelectedIndexChanged -= filterByDeckModeChanged;

			_searchSubsystem.UnsubscribeFromEvents();
			_deckSort.UnsubscribeFromEvents();

			_searchSubsystem.TextApplied -= searchTextApplied;
			_deckSort.SortChanged -= sortChanged;

			_textboxRename.LostFocus -= nameLostFocus;
			_textboxRename.KeyDown -= nameKeyDown;

			_viewDeck.MouseClicked -= viewDeckClicked;
			_viewDeck.RowDataLoaded -= viewDeckRowDataLoaded;
			_viewDeck.CardIndexChanged -= viewScrolled;
			_viewDeck.MouseMove -= deckMouseMove;

			_listModel.Loaded -= listModelLoaded;
			_searcher.Loaded -= searcherLoaded;

			ColorSchemeController.SystemColorsChanged -= systemColorsChanged;
		}

		private void searcherLoaded()
		{
			runRefreshSearchResultTask(onComplete: null);
			_tooltipOwner.Invoke(_searchSubsystem.Apply);
		}

		private void searchTextApplied() =>
			runRefreshSearchResultTask(onComplete: null);

		private void sortChanged() =>
			runRefreshSearchResultTask(onComplete: () => _tooltipOwner.Invoke(updateSortLabel));

		private void updateSortLabel() =>
			_labelSortStatus.Text = _deckSort.GetTextualStatus();

		private void listModelLoaded() =>
			runRefreshSearchResultTask(onComplete: null);

		private void deckMouseMove(object sender, MouseEventArgs e)
		{
			var hitInfo = _viewDeck.CalcHitInfo(e.Location);
			var model = (DeckModel) _viewDeck.FindRow(hitInfo.RowHandle);

			if (model != null)
			{
				updateCursor(_viewDeck,
					overText: hitInfo.FieldName != null,
					overButton: hitInfo.IsSomeButton);
			}
			else
			{
				updateCursor(_viewDeck);
			}
		}

		private void updateCursor(Control control, bool overText = false, bool overButton = false, bool outside = false)
		{
			if (outside)
				control.Cursor = Cursor;
			else if (overButton)
				control.Cursor = Cursors.Default;
			else if (overText)
				control.Cursor = _textSelectionCursor;
			else
				control.Cursor = Cursors.Default;
		}


		public void SetUi(TooltipController controller, DeckSuggestModel suggestModel)
		{
			if (_searchSubsystem.SuggestModel != null)
				_searchSubsystem.UnsubscribeSuggestModelEvents();

			_searchSubsystem.SuggestModel = suggestModel;
			_searchSubsystem.SubscribeSuggestModelEvents();

			setupTooltips(controller);

			_searchSubsystem.UpdateSuggestInput();
		}

		public void StartThread() =>
			_searchSubsystem.StartThread();

		public void AbortThread() =>
			_searchSubsystem.AbortThread();

		public void DeckChanged(Deck deck)
		{
			_model.OriginalDeck = deck;
			_model.UpdateTransformedDeck();
			refreshData();
		}

		public void CollectionChanged() =>
			_model.UpdateCollection(_collection.Snapshot(), affectedNames: null);

		private void viewDeckClicked(object view, HitInfo hitInfo, MouseEventArgs mouseArgs)
		{
			switch (mouseArgs.Button)
			{
				case MouseButtons.Left:
					if (hitInfo.IsAddButton())
						saveCurrentDeck();
					else if (hitInfo.IsRemoveButton())
						removeDeck((DeckModel) hitInfo.RowDataSource);
					else if (hitInfo.IsOpenButton())
						openDeck((DeckModel) hitInfo.RowDataSource, inNewTab: false, transformed: false);
					else if (hitInfo.IsOpenTransformedButton())
						openDeck((DeckModel) hitInfo.RowDataSource, inNewTab: false, transformed: true);
					else if (hitInfo.IsRenameButton())
						beginRenaming((DeckModel) hitInfo.RowDataSource, hitInfo.FieldBounds.Value);

					break;

				case MouseButtons.Middle:
					if (hitInfo.IsOpenButton())
						openDeck((DeckModel) hitInfo.RowDataSource, inNewTab: true, transformed: false);
					else if (hitInfo.IsOpenTransformedButton())
						openDeck((DeckModel) hitInfo.RowDataSource, inNewTab: true, transformed: true);

					break;
			}
		}

		private void viewDeckRowDataLoaded(object view, int rowHandle) =>
			endRenaming(commit: false);

		private void viewScrolled(object obj) =>
			Scrolled?.Invoke(this);

		private void runRefreshSearchResultTask(Action onComplete)
		{
			ThreadPool.QueueUserWorkItem(_ =>
			{
				_aborted = true;
				lock (_sync)
				{
					_aborted = false;

					IsTransformingDecks = true;
					DeckTransformed?.Invoke(this);

					_listModel.TransformDecks(() => _aborted);

					IsTransformingDecks = false;
					DeckTransformed?.Invoke(this);

					if (_aborted)
						return;

					var searchResult = _searchSubsystem?.SearchResult?.RelevanceById;

					var models = _deckSort.SortedRecords
						.Where(m => searchResult == null || searchResult.ContainsKey(m.Id))
						.ToList();

					_filteredModels.Clear();
					_filteredModels.Add(_model);

					_cardIdsInFilteredDecks.Clear();

					foreach (var model in models)
					{
						_filteredModels.Add(model);

						_cardIdsInFilteredDecks.UnionWith(model.Deck.MainDeck.Order);
						_cardIdsInFilteredDecks.UnionWith(model.Deck.Sideboard.Order);
					}
				}

				_tooltipOwner.Invoke(delegate
				{
					refreshData();
					onComplete?.Invoke();
				});

			});
		}

		private void filterByDeckModeChanged(object sender, EventArgs e)
		{
			FilterByDeckMode = (FilterByDeckMode) _menuFilterByDeckMode.SelectedIndex;
			FilterByDeckModeChanged?.Invoke(this);
		}


		private void saveCurrentDeck()
		{
			var copy = _model.OriginalDeck.Copy();

			if (string.IsNullOrEmpty(copy.Name))
				copy.Name = "[no name]";

			BeginLoadingDecks(count: 1);
			AddDeck(copy);

			EndLoadingDecks();
		}

		public void BeginLoadingDecks(int count)
		{
			if (count <= 0)
				throw new ArgumentException($"{nameof(count)}: {count}", nameof(count));

			DecksToAddCount = count;
			_saved = DateTime.Now;
		}

		public void ContinueLoadingDecks(int count) =>
			DecksToAddCount = count;

		public void AddDeck(Deck deck)
		{
			if (_saved.HasValue)
				deck.Saved = _saved.Value;

			if (_listModel.Add(deck))
				_decksAdded.Add(deck);

			DeckAdded?.Invoke(this);
		}

		public void EndLoadingDecks()
		{
			bool changed = DecksAddedCount > 0;

			DecksToAddCount = 0;
			_saved = null;

			if (changed)
				_listModel.Save();

			_decksAdded.Clear();

			runRefreshSearchResultTask(onComplete: null);
		}

		private void removeDeck(DeckModel deckModel)
		{
			_listModel.Remove(deckModel);
			_listModel.Save();
		}

		private void openDeck(DeckModel deckModel, bool inNewTab, bool transformed)
		{
			var deck = transformed
				? deckModel.Deck
				: deckModel.OriginalDeck;

			DeckOpened?.Invoke(this, deck, inNewTab);
		}



		private void nameLostFocus(object sender, EventArgs e) =>
			endRenaming(commit: true);

		private void nameKeyDown(object sender, KeyEventArgs e)
		{
			bool handled = true;

			if (e.KeyCode == Keys.Enter)
				endRenaming(commit: true);
			else if (e.KeyCode == Keys.Escape)
				endRenaming(commit: false);
			else
				handled = false;

			if (handled)
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
		}

		private void beginRenaming(DeckModel model, Rectangle fieldBounds)
		{
			_renamedModel = model;

			fieldBounds.Offset(_viewDeck.Location);
			_panelRename.Bounds = fieldBounds;

			_textboxRename.Text = model.Name;
			_textboxRename.SelectAll();
			_textboxRename.SelectionAlignment = HorizontalAlignment.Center;
			
			_panelRename.Visible = true;
			_textboxRename.Focus();
		}

		private void endRenaming(bool commit)
		{
			if (_renamedModel == null)
				return;

			var renamedModel = _renamedModel;

			if (commit)
			{
				if (renamedModel.IsCurrent)
				{
					renamedModel.Name = _textboxRename.Text;
				}
				else
				{
					_listModel.Rename(renamedModel, _textboxRename.Text);
					_listModel.Save();
				}
			}

			_renamedModel = null;
			_panelRename.Visible = false;
			_textboxRename.Text = string.Empty;

			if (commit)
			{
				if (renamedModel.IsCurrent)
					DeckRenamed?.Invoke(this, renamedModel.OriginalDeck);

				refreshData();
			}
		}



		public bool AnyFilteredDeckContains(Card c) =>
			_cardIdsInFilteredDecks.Contains(c.Id);

		public bool IsSearchFocused() =>
			_searchSubsystem.IsSearchFocused();

		private void refreshData()
		{
			_viewDeck.RefreshData();
			Refreshed?.Invoke(this);
		}

		public bool HideScroll
		{
			get => _viewDeck.LayoutOptions.HideScroll;
			set => _viewDeck.LayoutOptions.HideScroll = value;
		}

		public bool AllowPartialCard
		{
			get => _viewDeck.LayoutOptions.AllowPartialCards;
			set => _viewDeck.LayoutOptions.AllowPartialCards = value;
		}

		public FilterByDeckMode FilterByDeckMode
		{
			get => _filterByDeckMode;
			set
			{
				if (_filterByDeckMode == value)
					return;

				_filterByDeckMode = value;
				_menuFilterByDeckMode.SelectedIndex = (int) value;
			}
		}

		public LayoutViewControl DeckListView =>
			_viewDeck;

		public int ScrollPosition =>
			_viewDeck.CardIndex;

		public int MaxScroll =>
			_viewDeck.Count;

		public int FilteredDecksCount =>
			Math.Max(_filteredModels.Count - 1, 0);

		public bool IsAddingDecks =>
			_saved.HasValue;

		public bool IsSearcherLoaded =>
			_searchSubsystem.IsLoaded;

		public bool IsSearcherUpdating =>
			_searchSubsystem.IsUpdating;

		public int DecksAddedCount => _decksAdded.Count;
		public int DecksToAddCount { get; private set; }

		public bool IsTransformingDecks { get; private set; }

		private DeckModel _model;
		private DeckListModel _listModel;
		private DeckModel _renamedModel;
		private readonly HashSet<Deck> _decksAdded = new HashSet<Deck>();
		private readonly HashSet<string> _cardIdsInFilteredDecks = new HashSet<string>(Str.Comparer);
		private readonly List<DeckModel> _filteredModels = new List<DeckModel>();

		private DateTime? _saved;
		private Control _tooltipOwner;
		private CollectionEditorModel _collection;

		private ViewDeckListTooltips _layoutViewTooltip;
		private DeckSearchSubsystem _searchSubsystem;

		private FilterByDeckMode _filterByDeckMode;

		private Cursor _textSelectionCursor;
		private DeckSortSubsystem _deckSort;

		private bool _aborted;
		private readonly object _sync = new object();
		private DeckSearcher _searcher;
	}
}