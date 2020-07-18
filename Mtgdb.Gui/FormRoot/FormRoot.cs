using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Data.Index;
using Mtgdb.Downloader;
using Mtgdb.Ui;
using Ninject;

namespace Mtgdb.Gui
{
	public partial class FormRoot : CustomBorderForm
	{
		public FormRoot()
		{
			InitializeComponent();

			_deckButtons = new[]
			{
				_dropdownOpenDeck,
				_dropdownSaveDeck,
				_buttonPrint,
				_buttonClear,
				_buttonStat,
				_dropdownPaste
			};

			_saveLoadMenuModes = new List<SaveLoadMenuMode>
			{
				new SaveLoadMenuMode
				{
					TitleButton = _dropdownOpenDeck,
					MenuButtons = new[] { _buttonMenuOpenDeck, _buttonMenuOpenCollection, _buttonImportMtgArenaCollection, _buttonRestoreCollection },
					MtgArenaButtonText = "Import deck from MTGArena",
					IsMtgArenaPaste = true
				},
				new SaveLoadMenuMode
				{
					TitleButton = _dropdownSaveDeck,
					MenuButtons = new[] { _buttonMenuSaveDeck, _buttonMenuSaveCollection },
					MtgArenaButtonText = "Export deck to MTGArena",
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

			Icon = Icon.ExtractAssociatedIcon(AppDir.Executable.Value);

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
			App app,
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

			_app = app;
			_appSourceConfig = appSourceConfig;
			_uiConfigRepository = uiConfigRepository;
			_repo = repo;
			_serialization = serialization;
			_colorSchemeEditor = colorSchemeEditor;

			_formMainFactory = formMainFactory;
			_downloaderSubsystem = downloaderSubsystem;
			_newsService = newsService;

			KeyPreview = true;
			PreviewKeyDown += previewKeyDown;
			KeyDown += formKeyDown;

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

			MessageFilter.Instance.GlobalMouseMove += globalMouseMove;
			FormClosing += formClosing;

			_newsService.NewsFetched += newsFetched;
			_newsService.NewsDisplayed += newsDisplayed;
			_downloaderSubsystem.ProgressCalculated += downloaderProgressCalculated;

			setupExternalLinks();
			setupButtonClicks();
			setupLanguageMenu();

			setupTooltips();

			foreach (var button in _deckButtons)
				button.Enabled = false;

			Text = $"Mtgdb.Gui v{AppDir.GetVersion()}";

			scale();
			updateFormBorderColor();
			ColorSchemeController.SystemColorsChanging += updateFormBorderColor;

			_menuColors.BackColor = SystemColors.Control;
			_menuColors.ForeColor = SystemColors.ControlText;

			if (components == null)
				components = new Container();

			components.Add(new UiConfigMenuSubsystem(
				_menuUiScale,
				_menuUiSmallImageQuality,
				_menuUiSuggestDownloadMissingImages,
				_menuUiImagesCacheCapacity,
				_menuUiUndoDepth,
				_menuUiApplySearchBar,
				_checkboxAllPanels,
				_checkboxTopPanel, _checkboxRightPanel, _checkboxSearchBar, _repo, uiConfigRepository));

			_dropdownOpenDeck.BeforeShow = () => setMenuMode(_dropdownOpenDeck);
			_dropdownSaveDeck.BeforeShow = () => setMenuMode(_dropdownSaveDeck);
			_dropdownColorScheme.BeforeShow = updateMenuColors;

			_ctsLifetime = new CancellationTokenSource();
			_ctsLifetime.Token.When(_repo.IsLoadingComplete).Run(repositoryLoaded);
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
				button.Enabled = _repo.IsLoadingComplete.Signaled;
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
			var form = GetTab(i);

			bool creatingNewForm = form == null;

			if (creatingNewForm)
				form = createFormMain();

			form.SetFormRoot(this);

			if (creatingNewForm)
			{
				FsPath historyFile = App.GetHistoryFile(Id, i);
				form.LoadHistory(historyFile);
				form.SetPanelVisibility(_uiConfigRepository.Config);

				_tabs.TabIds[i] = form;
			}
		}

		public void UpdateTabText(FormMain formMain)
		{
			int formIndex = _tabs.TabIds.IndexOf(formMain);

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
			var formMain = GetTab(i);

			// implicit connection: move_tab_between_forms
			if (formMain == null)
				return;

			var lastTabId = _tabs.Count - 1;

			formMain.SaveHistory(App.GetHistoryFile(Id, lastTabId));
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
			_ctsLifetime.Cancel();

			_app.MoveFormHistoryToEnd(this);

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = GetTab(i);
				formMain.SaveHistory(App.GetHistoryFile(Id, i));
			}

			Enumerable.Range(0, _tabs.Count)
				.Select(GetTab)
				.ToArray()
				.ForEach(dispose);

			_app.StopForm(this);

			MessageFilter.Instance.GlobalMouseMove -= globalMouseMove;
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

			return form;
		}

		private void tabMouseMove(object sender, MouseEventArgs e)
		{
			var draggingFromTab = _app.FindCardDraggingForm();

			if (draggingFromTab == null)
				return;

			var hoveredForm = HoveredTab;

			if (hoveredForm != null && draggingFromTab != hoveredForm)
				_tabs.SelectedTabId = hoveredForm;
			else if (_tabs.HoveredIndex == _tabs.AddButtonIndex)
				AddTab();
		}

		private void globalMouseMove(object s, EventArgs e)
		{
			if (!_tabs.IsUnderMouse())
				return;

			var tabDraggingForm = _app.Forms.FirstOrDefault(_ => _._tabs.IsDragging());

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

			var formMain = tabDraggingForm.GetTab(draggedIndex);

			// implicit connection: move_tab_between_forms
			dragSourceTabs.TabIds[draggedIndex] = null;
			tabDraggingForm._panelClient.Controls.Remove(formMain);

			dragSourceTabs.RemoveTab(draggedIndex);

			_app.MoveTabHistory(Id, TabsCount, tabDraggingForm.Id, draggedIndex);

			addTab(formMain);

			_tabs.Capture = true;
			_tabs.BeginDrag(TabsCount - 1);
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
				var form = GetTab(i);
				form.ScheduleOpeningDeck(deck);
			}

			_tabs.TabAdded += onTabCreated;
			_tabs.AddTab(select: false, tabText: _serialization.GetShortDisplayName(deck.Name));
			_tabs.TabAdded -= onTabCreated;
		}



		private FormMain SelectedTab => (FormMain)_tabs.SelectedTabId;

		private FormMain HoveredTab => (FormMain)_tabs.HoveredTabId;

		public FormMain GetTab(int i) => (FormMain) _tabs.TabIds[i];

		public int TabsCount => _tabs.Count;

		public IEnumerable<FormMain> Tabs => _tabs.TabIds.Cast<FormMain>();

		public CardSuggestModel CardSuggestModel { get; }
		public DeckSuggestModel DeckSuggestModel { get; }
		public UiModel UiModel { get; }

		public Direction? SnapDirection =>
			Enum.GetValues(typeof(Direction))
				.Cast<Direction>()
				.FirstOrDefault(IsSnappedTo);

		public TooltipController TooltipController { get; }
		public TooltipController QuickFilterTooltipController { get; }

		public bool ShowTextualFields { get; set; }
		public bool ShowDeck { get; set; }
		public bool ShowPartialCards { get; set; }
		public bool ShowScroll { get; set; }
		public GuiSettings.ZoomSettings ZoomSettings { get; set; }
		public bool LoadedGuiSettings { get; set; }


		private int Id => _app.GetId(this);

		public sealed override string Text
		{
			get => base.Text;
			set => base.Text = value;
		}

		private bool _undoingOrRedoing;



		private readonly Func<FormMain> _formMainFactory;
		private readonly DownloaderSubsystem _downloaderSubsystem;
		private readonly NewsService _newsService;
		private readonly App _app;
		private readonly AppSourceConfig _appSourceConfig;
		private readonly UiConfigRepository _uiConfigRepository;
		private readonly CardRepository _repo;
		private readonly DeckSerializationSubsystem _serialization;
		private readonly ColorSchemeEditor _colorSchemeEditor;
		private readonly CancellationTokenSource _ctsLifetime;
	}
}
