using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls.Properties;
using Mtgdb.Dal;

namespace Mtgdb.Controls
{
	public partial class DeckListControl : UserControl
	{
		public event Action<object> Refreshed;
		public event Action<object> Scrolled;
		public event Action<object, Deck, bool> DeckOpened;
		public event Action<object, Deck> DeckRenamed;
		public event Action<object> FilterByDeckModeChanged;
		public event Action<object> DeckAdded;

		public DeckListControl()
		{
			InitializeComponent();
		}

		public void Init(
			DeckListModel decks,
			IconRecognizer recognizer,
			DeckSearcher searcher,
			DeckDocumentAdapter adapter,
			UiModelSnapshotFactory uiFactory,
			Control tooltipOwner)
		{
			_listModel = decks;
			_tooltipOwner = tooltipOwner;
			_uiFactory = uiFactory;

			_model = new DeckModel(Deck.Create(), _uiFactory.Snapshot())
			{
				IsCurrent = true
			};

			_viewDeck.IconRecognizer = recognizer;
			_viewDeck.LayoutControlType = typeof(DeckListLayout);
			_viewDeck.DataSource = _filteredModels;

			var iBeamIcon = Resources.text_selection_24.ResizeDpi();
			var iBeamHotSpot = new Size(iBeamIcon.Width / 2, iBeamIcon.Height / 2);
			_textSelectionCursor = CursorHelper.CreateCursor(iBeamIcon, iBeamHotSpot);

			_textBoxName.Visible = false;

			_customTooltip = new ViewDeckListTooltips(_tooltipOwner, _viewDeck);

			_searchSubsystem = new DeckSearchSubsystem(
				this,
				_textBoxSearch,
				_panelSearchIcon,
				_listBoxSuggest,
				searcher,
				adapter,
				_viewDeck);

			_menuFilterByDeckMode.SelectedIndex = 0;

			_viewDeck.MouseClicked += viewDeckClicked;
			_viewDeck.RowDataLoaded += viewDeckRowDataLoaded;
			_viewDeck.CardIndexChanged += viewScrolled;

			_textBoxName.LostFocus += nameLostFocus;
			_textBoxName.KeyDown += nameKeyDown;

			_menuFilterByDeckMode.SelectedIndexChanged += filterByDeckModeChanged;

			_searchSubsystem.TextApplied += updateSearchResult;

			_searchSubsystem.SubscribeToEvents();
			_customTooltip.SubscribeToEvents();

			_higlightSubsystem = new SearchResultHiglightSubsystem(_viewDeck, _searchSubsystem, adapter);
			_higlightSubsystem.SubscribeToEvents();

			_viewDeck.MouseMove += deckMouseMove;

			if (_listModel.IsLoaded)
				listModelLoaded();
			else
				_listModel.Loaded += listModelLoaded;

			searcher.Loaded += searcherLoaded;

			if (searcher.IsLoaded)
				_searchSubsystem.Apply();
		}

		private void searcherLoaded() =>
			_tooltipOwner.Invoke(_searchSubsystem.Apply);

		private void listModelLoaded() =>
			_searchSubsystem.ModelChanged();

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
			_model.Deck = deck;

			_viewDeck.RefreshData();
			Refreshed?.Invoke(this);
		}

		public void CollectionChanged()
		{
			_model.Ui = _uiFactory.Snapshot();
			_searchSubsystem.ModelChanged();
		}



		private void setupTooltips(TooltipController controller)
		{
			controller.SetTooltip(_tooltipOwner, "Deck name", "Type deck name.\r\n" +
				"press Enter to apply\r\n" +
				"press Esc to cancel",
				_textBoxName);

			controller.SetTooltip(_tooltipOwner,
				() => _searchSubsystem.SearchResult?.ParseErrorMessage != null
					? "Syntax error"
					: "Search decks",
				() => _searchSubsystem.SearchResult?.ParseErrorMessage ??
					"Type some query to narrow down the list of decks below\r\n" +
					"Example queries:\r\n" +
					"name: affin*\r\n" +
					"mana: \\{w\\} AND \\{u\\}\r\n\r\n" +
					"Ctrl+SPACE to get intellisense\r\n" +
					"Enter to apply\r\n" +
					"Ctrl+Backspace to delete one word\r\n" +
					"F1 to learn searh string syntax\r\n\r\n",
				_panelSearchIcon,
				_panelSearch,
				_textBoxSearch);

			string filterMode(FilterByDeckMode mode) =>
				_menuFilterByDeckMode.Items[(int) mode].ToString();

			controller.SetTooltip(_tooltipOwner,
				"Filter by deck mode",
				"Controls how search result of cards is affected by decks.\r\n\r\n" +
				$"- {filterMode(FilterByDeckMode.Ignored)}\r\n" +
				"    decks do not affect search result of cards\r\n" +
				$"- {filterMode(FilterByDeckMode.CurrentDeck)}\r\n" +
				"    show cards present in currently opened deck\r\n" +
				$"- {filterMode(FilterByDeckMode.FilteredSavedDecks)}\r\n" +
				"    show cards present in any saved deck from list below matching search criteria for " +
				"saved decks on the left",
				_menuFilterByDeckMode);

			controller.SetCustomTooltip(_customTooltip);
		}

		private void viewDeckClicked(object view, HitInfo hitInfo, MouseEventArgs mouseArgs)
		{
			switch (mouseArgs.Button)
			{
				case MouseButtons.Left:
					if (hitInfo.CustomButtonIndex == DeckListLayout.CustomButtonAdd)
						saveCurrentDeck();
					else if (hitInfo.CustomButtonIndex == DeckListLayout.CustomButtonRemove)
						removeDeck((DeckModel) hitInfo.RowDataSource);
					else if (hitInfo.CustomButtonIndex == DeckListLayout.CustomButtonOpen)
						openDeck((DeckModel) hitInfo.RowDataSource, inNewTab: false);
					else if (hitInfo.CustomButtonIndex == DeckListLayout.CustomButtonRename)
						beginRenaming((DeckModel) hitInfo.RowDataSource, hitInfo.FieldBounds.Value);

					break;

				case MouseButtons.Middle:
					if (hitInfo.CustomButtonIndex == DeckListLayout.CustomButtonOpen)
						openDeck((DeckModel) hitInfo.RowDataSource, inNewTab: true);
					break;
			}
		}

		private void viewDeckRowDataLoaded(object view, int rowHandle) =>
			endRenaming(commit: false);

		private void viewScrolled(object obj) =>
			Scrolled?.Invoke(this);

		private void updateSearchResult()
		{
			if (_uiFactory == null)
				return;

			var searchResult = _searchSubsystem?.SearchResult?.RelevanceById;

			_filteredModels.Clear();
			_filteredModels.Add(_model);

			var ui = _uiFactory.Snapshot();

			var models = _listModel.GetModels(ui)
				.Reverse()
				.Where(m => searchResult == null || searchResult.ContainsKey(m.Id));

			_cardIdsInFilteredDecks.Clear();

			foreach (var model in models)
			{
				_filteredModels.Add(model);

				_cardIdsInFilteredDecks.UnionWith(model.Deck.MainDeck.Order);
				_cardIdsInFilteredDecks.UnionWith(model.Deck.Sideboard.Order);
			}

			_viewDeck.RefreshData();
			Refreshed?.Invoke(this);
		}

		private void filterByDeckModeChanged(object sender, EventArgs e)
		{
			FilterByDeckMode = (FilterByDeckMode) _menuFilterByDeckMode.SelectedIndex;
			FilterByDeckModeChanged?.Invoke(this);
		}


		private void saveCurrentDeck()
		{
			BeginLoadingDecks(count: 1);
			AddDeck(_model.Deck.Copy());
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

			_listModel.Add(deck);

			DecksAddedCount++;
			DeckAdded?.Invoke(this);
		}

		public void EndLoadingDecks()
		{
			bool changed = DecksAddedCount > 0;

			DecksToAddCount = 0;
			DecksAddedCount = 0;
			_saved = null;

			if (!changed)
				return;

			_searchSubsystem.ModelChanged();
			_listModel.Save();
		}

		private void removeDeck(DeckModel deckModel)
		{
			_listModel.Remove(deckModel.Deck);
			_listModel.Save();
			_searchSubsystem.ModelChanged();
		}

		private void openDeck(DeckModel deckModel, bool inNewTab) =>
			DeckOpened?.Invoke(this, deckModel.Deck, inNewTab);



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
			_textBoxName.Bounds = fieldBounds;
			_textBoxName.Text = model.Deck.Name;
			_textBoxName.SelectAll();

			_textBoxName.Visible = true;
			_textBoxName.Focus();
		}

		private void endRenaming(bool commit)
		{
			if (_renamedModel == null)
				return;

			var renamedModel = _renamedModel;

			if (commit)
			{
				_listModel.Rename(renamedModel.Deck, _textBoxName.Text);
				_listModel.Save();
			}

			_renamedModel = null;
			_textBoxName.Visible = false;
			_textBoxName.Text = string.Empty;

			if (commit)
			{
				_searchSubsystem.ModelChanged();

				if (renamedModel.IsCurrent)
					DeckRenamed?.Invoke(this, renamedModel.Deck);
			}
		}



		public void Scale()
		{
			scalePanelIcon(_panelSearchIcon);

			_panelSearch.Height = _panelSearch.Height.ByDpiHeight();
			_menuFilterByDeckMode.ScaleDpi();

			scaleLayoutView(_viewDeck);
		}

		private static void scalePanelIcon(BorderedPanel panel)
		{
			panel.ScaleDpi();
			panel.BackgroundImage = ((Bitmap) panel.BackgroundImage).HalfResizeDpi();
		}

		private static void scaleLayoutView(LayoutViewControl view)
		{
			var sortIcon = view.SortOptions.Icon;

			view.SortOptions.Icon = sortIcon?.HalfResizeDpi();
			view.SortOptions.AscIcon = view.SortOptions.AscIcon?.HalfResizeDpi();
			view.SortOptions.DescIcon = view.SortOptions.DescIcon?.HalfResizeDpi();
			view.SearchOptions.Button.Icon = view.SearchOptions.Button.Icon?.HalfResizeDpi();

			view.LayoutOptions.AlignTopLeftIcon = view.LayoutOptions.AlignTopLeftIcon?.HalfResizeDpi();
			view.LayoutOptions.AlignTopLeftHoveredIcon = view.LayoutOptions.AlignTopLeftHoveredIcon?.HalfResizeDpi();

			view.ProbeCardCreating += probeCardCreating;
		}

		private static void probeCardCreating(object view, LayoutControl probeCard)
		{
			var card = (DeckListLayout) probeCard;

			card.ScaleDpi();

			foreach (var field in card.Fields)
			{
				field.Image = ((Bitmap) field.Image)?.HalfResizeDpi();

				for (int i = 0; i < field.CustomButtons.Count; i++)
				{
					var button = field.CustomButtons[i];
					button.Icon = button.Icon?.ResizeDpi();
				}
			}

			card.ImageDeckBox = Resources.deckbox.ResizeDpi();
			card.ImageDeckBoxOpened = Resources.deckbox_opened.ResizeDpi();
		}

		public bool AnyFilteredDeckContains(Card c) =>
			_cardIdsInFilteredDecks.Contains(c.Id);

		public bool IsSearchFocused() =>
			_searchSubsystem.IsSearchFocused();



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
			_viewDeck.Count - 1;

		public bool IsAddingDecks =>
			_saved.HasValue;

		public bool IsSearcherLoaded =>
			_searchSubsystem.IsLoaded;

		public bool IsSearcherUpdating =>
			_searchSubsystem.IsUpdating;

		public int DecksAddedCount { get; private set; }
		public int DecksToAddCount { get; private set; }

		private DateTime? _saved;

		private DeckModel _model;

		private Control _tooltipOwner;

		private UiModelSnapshotFactory _uiFactory;
		private DeckListModel _listModel;

		private DeckModel _renamedModel;
		private ViewDeckListTooltips _customTooltip;
		private DeckSearchSubsystem _searchSubsystem;

		private FilterByDeckMode _filterByDeckMode;
		private SearchResultHiglightSubsystem _higlightSubsystem;

		private readonly HashSet<string> _cardIdsInFilteredDecks = new HashSet<string>(Str.Comparer);
		private readonly List<DeckModel> _filteredModels = new List<DeckModel>();
		private Cursor _textSelectionCursor;
	}
}
