using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Mtgdb.Gui.Properties;
using Mtgdb.Ui;
using Ninject;

namespace Mtgdb.Gui
{
	public partial class FormRoot : CustomBorderForm, IMessageFilter
	{
		public FormRoot()
		{
			InitializeComponent();

			_buttonLanguage.VisibleAllBorders = true;

			getLanguageMenuItems().Append(_buttonLanguage).ForEach(
				b => b.Font = new Font("Consolas", 9f, b.Font.Style));

			new[]
				{
					_buttonMenuCopyDeck,
					_buttonMenuPasteDeck,
					_buttonMenuPasteDeckAppend,
					_buttonMenuCopyCollection,
					_buttonMenuPasteCollection,
					_buttonMenuPasteCollectionAppend,
					_buttonMenuSaveDeck,
					_buttonMenuOpenDeck,
					_buttonMenuSaveCollection,
					_buttonMenuOpenCollection,
					_buttonMenuEditConfig,
					_buttonMenuDonatePayPal,
					_buttonMenuDonateYandexMoney
				}
				.ForEach(b => b.VisibleAllBorders = true);

			_languageIcons = new Dictionary<string, Bitmap>(Str.Comparer)
			{
				{ "cn", Resources.cn },
				{ "jp", Resources.jp },
				{ "kr", Resources.kr },
				{ "tw", Resources.tw },
				{ "es", Resources.es },
				{ "fr", Resources.fr },
				{ "it", Resources.it },
				{ "pt", Resources.pt },
				{ "de", Resources.de },
				{ "en", Resources.en },
				{ "ru", Resources.ru }
			};

			_deckButtons = new[]
			{
				_buttonOpenDeck,
				_buttonSaveDeck,
				_buttonPrint,
				_buttonClear,
				_buttonStat,
				_buttonPaste
			};

			_saveLoadMenuModes = new List<SaveLoadMenuMode>
			{
				new SaveLoadMenuMode
				{
					TitleButton = _buttonOpenDeck,
					MenuButtons = new[] { _buttonMenuOpenDeck, _buttonMenuOpenCollection, _buttonImportMtgArenaCollection },
					MtgArenaButtonText = "Import MTGArena deck",
					IsMtgArenaPaste = true
				},
				new SaveLoadMenuMode
				{
					TitleButton = _buttonSaveDeck,
					MenuButtons = new[] { _buttonMenuSaveDeck, _buttonMenuSaveCollection },
					MtgArenaButtonText = "Export MTGArena deck",
					IsMtgArenaPaste = false
				}
			};

			RegisterDragControl(_layoutTitle);
			RegisterDragControl(_flowTitleLeft);
			RegisterDragControl(_flowTitleRight);
			RegisterDragControl(_tabs);

			_layoutTitle.PaintBackground =
				_flowTitleLeft.PaintBackground =
					_flowTitleRight.PaintBackground =
						_tabs.PaintBackground = false;

			Icon = Icon.ExtractAssociatedIcon(AppDir.Executable);

			_flowTitleLeft.Controls.Cast<Control>()
				.Concat(_flowTitleRight.Controls.Cast<Control>())
				.Append(_panelClient)
				.Where(_ => _.TabStop)
				.ForEach((c, i) => c.TabIndex = i);
		}

		[UsedImplicitly]
		public FormRoot(Func<FormMain> formMainFactory,
			DownloaderSubsystem downloaderSubsystem,
			NewsService newsService,
			CardSuggestModel cardSuggestModel,
			DeckSuggestModel deckSuggestModel,
			[Named(GuiModule.DefaultTooltipScope)] TooltipController tooltipController,
			[Named(GuiModule.QuickFilterTooltipScope)] TooltipController quickFilterTooltipController,
			CardRepository repo,
			DeckSerializationSubsystem serialization,
			UiModel uiModel,
			ColorSchemeEditor colorSchemeEditor,
			Application application,
			AppSourceConfig appSourceConfig,
			UiConfigRepository uiConfigRepository)
			:this()
		{
			TooltipController = tooltipController;
			QuickFilterTooltipController = quickFilterTooltipController;

			UiModel = uiModel;

			DeckSuggestModel = deckSuggestModel;

			CardSuggestModel = cardSuggestModel;
			CardSuggestModel.Ui = UiModel;

			_application = application;
			_appSourceConfig = appSourceConfig;
			_repo = repo;
			_serialization = serialization;
			_colorSchemeEditor = colorSchemeEditor;

			_popupSubsystem = new PopupSubsystem();
			_formMainFactory = formMainFactory;
			_downloaderSubsystem = downloaderSubsystem;
			_newsService = newsService;

			KeyPreview = true;
			PreviewKeyDown += previewKeyDown;
			KeyDown += formKeyDown;

			_repo.LoadingComplete += repositoryLoaded;

			QueryHandleDrag += queryHandleDrag;
			Load += load;
			Closed += closed;

			_tabs.AllowDrop = true;
			_tabs.TabAdded += tabCreated;
			_tabs.TabRemoving += tabClosing;
			_tabs.TabRemoved += tabClosed;
			_tabs.SelectedIndexChanging += selectedTabChanging;
			_tabs.SelectedIndexChanged += selectedTabChanged;
			_tabs.DragOver += tabsDragOver;
			_tabs.MouseMove += tabMouseMove;

			FormClosing += formClosing;

			_newsService.NewsFetched += newsFetched;
			_newsService.NewsDisplayed += newsDisplayed;
			_downloaderSubsystem.ProgressCalculated += downloaderProgressCalculated;

			setupButtons();

			setupExternalLinks();
			setupButtonClicks();
			setupLanguageMenu();

			_uiConfigMenuSubsystem = new UiConfigMenuSubsystem(
				_menuUiScale,
				_menuUiSmallImageQuality,
				_menuUiSuggestDownloadMissingImages,
				_menuUiImagesCacheCapacity,
				_menuUiUndoDepth,
				uiConfigRepository);

			_uiConfigMenuSubsystem.SetupMenu();

			setupTooltips();

			foreach (var button in _deckButtons)
				button.Enabled = false;

			Text = $"Mtgdb.Gui v{AppDir.GetVersion()}";

			scale();
			updateFormBorderColor();
			ColorSchemeController.SystemColorsChanging += updateFormBorderColor;

			_menuColors.BackColor = SystemColors.Control;
			_menuColors.ForeColor = SystemColors.ControlText;
		}

		private void updateFormBorderColor()
		{
			_tabs.ColorTabBorder = _panelCaption.BorderColor;
			_layoutTitle.BorderColor = _panelCaption.BorderColor;
		}

		private void load(object sender, EventArgs e)
		{
			updateDownloadButton();
			updateDeckButtons();

			System.Windows.Forms.Application.AddMessageFilter(this);
			CardSuggestModel.StartSuggestThread();
			DeckSuggestModel.StartSuggestThread();

			if (SelectedTab != null)
			{
				SelectedTab.Focus();
				SelectedTab.FocusSearch();
			}
		}

		private void repositoryLoaded()
		{
			this.Invoke(updateDeckButtons);
		}

		private void updateDeckButtons()
		{
			foreach (var button in _deckButtons)
				button.Enabled = _repo.IsLoadingComplete;
		}



		private void tabsDragOver(object sender, DragEventArgs e)
		{
			var location = _tabs.PointToClient(new Point(e.X, e.Y));

			_tabs.GetTabIndex(location, out int hoveredIndex, out _);

			if (hoveredIndex == _tabs.AddButtonIndex)
			{
				_tabs.AddTab(insertToTheLeft: false);
				return;
			}

			if (hoveredIndex < 0 || hoveredIndex == _tabs.SelectedIndex || hoveredIndex >= _tabs.Count)
				return;

			_tabs.SelectedIndex = hoveredIndex;
		}

		private void tabCreated(TabHeaderControl sender, int i)
		{
			var form = getTab(i);

			bool creatingNewForm = form == null;

			if (creatingNewForm)
				form = createFormMain();

			form.SetFormRoot(this);

			if (creatingNewForm)
			{
				string historyFile = Application.GetHistoryFile(Id, i);
				form.LoadHistory(historyFile);
				_tabs.TabIds[i] = form;
			}
		}

		private void formTextChanged(object sender, EventArgs e)
		{
			var formMain = (FormMain) sender;
			var formIndex = _tabs.TabIds.IndexOf(formMain);

			if (formIndex < 0)
				return;

			string text =
				formMain.Text.Non(DeckSerializationSubsystem.NoDeck) ??
				_tabs.GetDefaultText(formIndex);

			_tabs.SetTabSetting(formMain, new TabSettings(text));
		}

		private void selectedTabChanging(TabHeaderControl sender, int selected)
		{
			tabUnselecting();
		}

		private void tabUnselecting()
		{
			var form = SelectedTab;
			form?.OnTabUnselected();
		}

		private void selectedTabChanged(TabHeaderControl sender, int selected)
		{
			tabSelected();
		}

		private void tabSelected()
		{
			var selectedForm = SelectedTab;

			if (selectedForm == null)
				return;

			selectedForm.Location = new Point(0, 0);
			selectedForm.Size = _panelClient.Size;

			if (!_panelClient.Controls.Contains(selectedForm))
				_panelClient.Controls.Add(selectedForm);

			selectedForm.BringToFront();
			selectedForm.Show();
			selectedForm.Focus();
			selectedForm.FocusSearch();

			selectedForm.OnTabSelected();

			foreach (Control control in _panelClient.Controls)
				if (control != selectedForm && control.Visible)
					control.Hide();
		}

		private void tabClosing(object sender, int i)
		{
			var formMain = getTab(i);

			// implicit connection: move_tab_between_forms
			if (formMain == null)
				return;

			var lastTabId = _tabs.Count - 1;

			formMain.SaveHistory(Application.GetHistoryFile(Id, lastTabId));
			formMain.Hide();

			dispose(formMain);
		}

		private void dispose(FormMain formMain)
		{
			formMain.Shutdown();
			_panelClient.Controls.Remove(formMain);
			formMain.Dispose();
		}

		private void tabClosed(TabHeaderControl sender)
		{
			if (_tabs.Count == 0)
				Close();
		}

		private void formClosing(object sender, EventArgs e)
		{
			_application.MoveFormHistoryToEnd(this);

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = getTab(i);
				formMain.SaveHistory(Application.GetHistoryFile(Id, i));
			}

			Enumerable.Range(0, _tabs.Count)
				.Select(getTab)
				.ToArray()
				.ForEach(dispose);

			_application.RemoveForm(this);

			System.Windows.Forms.Application.RemoveMessageFilter(this);
		}



		private void newsFetched()
		{
			updateDownloadButton();
		}

		private void newsDisplayed()
		{
			updateDownloadButton();
		}

		private void downloaderProgressCalculated()
		{
			updateDownloadButton();
		}

		private void updateDownloadButton()
		{
			_buttonUpdate.Checked = _newsService.HasUnreadNews;
			this.Invoke(delegate
			{
				bool enabled = _downloaderSubsystem.IsProgressCalculated && _newsService.NewsLoaded;

				_buttonUpdate.Enabled = enabled;

				if (enabled && _downloaderSubsystem.NeedToSuggestDownloader)
					_downloaderSubsystem.ShowDownloader(this, auto: true);
			});
		}



		private void closed(object sender, EventArgs e)
		{
			unsubscribeButtonEvents();
			CardSuggestModel.AbortSuggestThread();
			DeckSuggestModel.AbortSuggestThread();
		}

		private void queryHandleDrag(object sender, MouseEventArgs e, CancelEventArgs cancelArgs)
		{
			if (sender == _tabs)
				cancelArgs.Cancel = _tabs.HoveredIndex >= 0;
		}



		private FormMain createFormMain()
		{
			var form = _formMainFactory();

			form.Size = _panelClient.Size;
			form.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			form.Location = new Point(0, 0);

			form.TextChanged += formTextChanged;
			return form;
		}

		private void tabMouseMove(object sender, MouseEventArgs e)
		{
			var draggingFromTab = _application.FindCardDraggingForm();

			if (draggingFromTab == null)
				return;

			var hoveredForm = HoveredTab;

			if (hoveredForm != null && draggingFromTab != hoveredForm)
				_tabs.SelectedTabId = hoveredForm;
			else if (_tabs.HoveredIndex == _tabs.AddButtonIndex)
				AddTab();
		}

		public bool PreFilterMessage(ref Message m)
		{
			if (Disposing || IsDisposed)
				return false;

			// WM_MOUSEMOVE
			if (m.Msg == 0x0200)
				mouseMoved();

			return false;
		}

		private void mouseMoved()
		{
			if (!_tabs.IsUnderMouse())
				return;

			var tabDraggingForm = _application.Forms.FirstOrDefault(_ => _._tabs.IsDragging());

			if (tabDraggingForm == this)
				return;

			if (tabDraggingForm == null)
				return;

			var draggedIndex = tabDraggingForm._tabs.DraggingIndex.Value;

			takeTabFromAnotherForm(tabDraggingForm, draggedIndex);
		}

		private void takeTabFromAnotherForm(FormRoot tabDraggingForm, int draggedIndex)
		{
			var dragSourceTabs = tabDraggingForm._tabs;

			dragSourceTabs.AbortDrag();
			dragSourceTabs.Capture = false;

			var formMain = tabDraggingForm.getTab(draggedIndex);

			// implicit connection: move_tab_between_forms
			dragSourceTabs.TabIds[draggedIndex] = null;
			tabDraggingForm._panelClient.Controls.Remove(formMain);

			dragSourceTabs.RemoveTab(draggedIndex);

			_application.MoveTabHistory(Id, TabsCount, tabDraggingForm.Id, draggedIndex);

			addTab(formMain);

			_tabs.Capture = true;
			_tabs.BeginDrag(TabsCount - 1);
		}

		public bool ShowFilterPanels
		{
			get => _buttonShowFilterPanels.Checked;
			set => _buttonShowFilterPanels.Checked = value;
		}



		public bool HideTooltips
		{
			get => !TooltipController.Active;
			set
			{
				bool showTooltips = !value;
				if (TooltipController.Active != showTooltips)
					TooltipController.Active = showTooltips;

				if (_buttonTooltips.Checked != showTooltips)
					_buttonTooltips.Checked = showTooltips;
			}
		}

		public bool CanUndo
		{
			set => _buttonUndo.Enabled = value;
		}

		public bool CanRedo
		{
			set => _buttonRedo.Enabled = value;
		}

		public void SelectNextTab()
		{
			int nextPageIndex = 1 + _tabs.SelectedIndex;

			if (nextPageIndex == _tabs.Count)
				nextPageIndex = 0;

			_tabs.SelectedIndex = nextPageIndex;
		}

		public void SelectPreviousTab()
		{
			int nextPageIndex = _tabs.SelectedIndex - 1;

			if (nextPageIndex < 0)
				nextPageIndex = _tabs.Count - 1;

			_tabs.SelectedIndex = nextPageIndex;
		}

		public void AddTab()
		{
			_tabs.AddTab();
		}

		private void addTab(FormMain form) => _tabs.AddTab(form);

		public void CloseTab()
		{
			_tabs.RemoveTab(_tabs.SelectedIndex);
		}

		public void OpenDeckInNewTab(Deck deck)
		{
			void onTabCreated(TabHeaderControl c, int i)
			{
				var form = getTab(i);
				form.ScheduleOpeningDeck(deck);
			}

			_tabs.TabAdded += onTabCreated;
			_tabs.AddTab(select: false, tabText: _serialization.GetShortDisplayName(deck.Name));
			_tabs.TabAdded -= onTabCreated;
		}



		private FormMain SelectedTab => (FormMain)_tabs.SelectedTabId;

		private FormMain HoveredTab => (FormMain)_tabs.HoveredTabId;

		private FormMain getTab(int i) => (FormMain) _tabs.TabIds[i];

		public int TabsCount => _tabs.Count;

		public IEnumerable<FormMain> Tabs => _tabs.TabIds.Cast<FormMain>();

		public CardSuggestModel CardSuggestModel { get; }
		public DeckSuggestModel DeckSuggestModel { get; }
		public UiModel UiModel { get; }

		public Direction? SnapDirection
		{
			get => Enum.GetValues(typeof(Direction))
				.Cast<Direction>()
				.FirstOrDefault(IsSnappedTo);

			set
			{
				if (!value.HasValue)
					return;

				if (!IsSnappedTo(value.Value))
					SnapTo(value.Value);
			}
		}

		public Rectangle WindowArea
		{
			get => DesktopBounds;
			set
			{
				SnapTo(Direction.MiddleCenter);

				var nearestScreenArea = Screen.GetWorkingArea(value);
				if (nearestScreenArea.IntersectsWith(value))
					DesktopBounds = value;
			}
		}

		public TooltipController TooltipController { get; }
		public TooltipController QuickFilterTooltipController { get; }

		public bool ShowTextualFields { get; set; }
		public bool ShowDeck { get; set; }
		public bool ShowPartialCards { get; set; }
		public bool ShowScroll { get; set; }
		public GuiSettings.ZoomSettings ZoomSettings { get; set; }
		public bool LoadedGuiSettings { get; set; }


		private int Id => _application.GetId(this);

		public sealed override string Text
		{
			get => base.Text;
			set => base.Text = value;
		}

		private bool _undoingOrRedoing;



		private readonly Func<FormMain> _formMainFactory;
		private readonly DownloaderSubsystem _downloaderSubsystem;
		private readonly NewsService _newsService;
		private readonly Application _application;
		private readonly AppSourceConfig _appSourceConfig;
		private readonly CardRepository _repo;
		private readonly DeckSerializationSubsystem _serialization;
		private readonly ColorSchemeEditor _colorSchemeEditor;
		private readonly UiConfigMenuSubsystem _uiConfigMenuSubsystem;
	}
}
