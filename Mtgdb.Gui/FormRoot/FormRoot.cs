using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Downloader;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public partial class FormRoot : CustomBorderForm, IFormRoot, IMessageFilter
	{
		public FormRoot()
		{
			InitializeComponent();

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

			_deckButtons = new ButtonBase[]
			{
				_buttonOpenDeck,
				_buttonSaveDeck,
				_buttonPrint,
				_buttonClear,
				_buttonStat,
				_buttonPaste
			};

			_saveLoadButtons = new CheckBox[]
			{
				_buttonOpenDeck,
				_buttonSaveDeck
			};

			_saveLoadMenuButtons = new CheckBox[][]
			{
				new [] { _buttonMenuOpenDeck, _buttonMenuOpenCollection },
				new [] { _buttonMenuSaveDeck, _buttonMenuSaveCollection }
			};

			scale();

			RegisterDragControl(_layoutTitle);
			RegisterDragControl(_flowTitleLeft);
			RegisterDragControl(_flowTitleRight);
		}

		private void scale()
		{
			TitleHeight = TitleHeight.ByDpiHeight();

			ImageMinimize = ImageMinimize.HalfResizeDpi();
			ImageMaximize = ImageMaximize.HalfResizeDpi();
			ImageNormalize = ImageNormalize.HalfResizeDpi();
			ImageClose = ImageClose.HalfResizeDpi();

			_buttonDonateYandexMoney.ScaleDpi();
			_buttonDonatePayPal.ScaleDpi();
			_panelAva.BackgroundImage = ((Bitmap) _panelAva.BackgroundImage).HalfResizeDpi();
			_panelAva.ScaleDpi();
			_labelDonate.ScaleDpi();

			_buttonMenuPasteDeck.ScaleDpi();
			_buttonMenuPasteDeckAppend.ScaleDpi();
			_buttonMenuPasteCollection.ScaleDpi();
			_buttonMenuPasteCollectionAppend.ScaleDpi();
			_buttonMenuCopyCollection.ScaleDpi();
			_buttonMenuCopyDeck.ScaleDpi();

			_labelPasteInfo.ScaleDpi();

			_buttonMenuOpenDeck.ScaleDpi();
			_buttonMenuSaveDeck.ScaleDpi();
			_buttonMenuOpenCollection.ScaleDpi();
			_buttonMenuSaveCollection.ScaleDpi();
			_buttonVisitForge.ScaleDpi();
			_buttonVisitMagarena.ScaleDpi();
			_buttonVisitXMage.ScaleDpi();
			_buttonVisitMtgo.ScaleDpi();
			_buttonVisitDotP2014.ScaleDpi();
			_buttonVisitCockatrice.ScaleDpi();
			_labelMtgo.ScaleDpi();
			_labelMagarena.ScaleDpi();
			_labelDotP2.ScaleDpi();

			_tabs.Height = _tabs.Height.ByDpiHeight();
			_tabs.SlopeSize = _tabs.SlopeSize.ByDpi();
			_tabs.AddButtonSlopeSize = _tabs.AddButtonSlopeSize.ByDpi();
			_tabs.AddButtonWidth = _tabs.AddButtonWidth.ByDpiWidth();

			_tabs.CloseIcon = _tabs.CloseIcon.HalfResizeDpi();
			_tabs.CloseIconHovered = _tabs.CloseIconHovered.HalfResizeDpi();
			_tabs.AddIcon = _tabs.AddIcon.HalfResizeDpi();

			foreach (var langButton in getLanguageMenuItems())
				langButton.ScaleDpi();

			foreach (var titleButton in _flowTitleLeft.Controls.OfType<ButtonBase>())
				titleButton.ScaleDpi();

			foreach (var titleButton in _flowTitleRight.Controls.OfType<ButtonBase>())
				titleButton.ScaleDpi();
		}

		// ReSharper disable once UnusedMember.Global
		public FormRoot(Func<FormMain> formMainFactory,
			DownloaderSubsystem downloaderSubsystem,
			NewsService newsService,
			SuggestModel suggestModel,
			TooltipController tooltipController,
			CardRepository repo,
			UiModel uiModel,
			FormManager formManager)
			:this()
		{
			TooltipController = tooltipController;
			UiModel = uiModel;
			SuggestModel = suggestModel;

			SuggestModel.Ui = UiModel;

			_formManager = formManager;

			_repo = repo;
			_repo.LoadingComplete += repositoryLoaded;

			_buttonSubsystem = new ButtonSubsystem();
			RegisterDragControl(_tabs);
			QueryHandleDrag += queryHandleDrag;
			Load += load;
			Closed += closed;

			_formMainFactory = formMainFactory;
			_downloaderSubsystem = downloaderSubsystem;
			_newsService = newsService;

			_tabs.TabAdded += tabCreated;
			_tabs.TabRemoving += tabClosing;
			_tabs.TabRemoved += tabClosed;
			_tabs.SelectedIndexChanging += selectedTabChanging;
			_tabs.SelectedIndexChanged += selectedTabChanged;
			_tabs.AllowDrop = true;
			_tabs.DragOver += tabsDragOver;
			_tabs.MouseMove += tabMouseMove;

			FormClosing += formClosing;

			KeyDown += formKeyDown;

			_newsService.NewsFetched += newsFetched;
			_newsService.NewsDisplayed += newsDisplayed;
			_downloaderSubsystem.ProgressCalculated += downloaderProgressCalculated;

			setupButtons();

			setupExternalLinks();
			setupButtonClicks();
			setupLanguageMenu();
			setupTooltips();

			if (!DesignMode)
				SnapTo(Direction.North, default(Point));

			foreach (var button in _deckButtons)
				button.Enabled = false;

			Text = $"Mtgdb.Gui v{AppDir.GetVersion()}";

			_formManager.Add(this);
		}

		private void load(object sender, EventArgs e)
		{
			updateDownloadButton();
			updateDeckButtons();

			Application.AddMessageFilter(this);
			SuggestModel.StartSuggestThread();
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

			int hoveredIndex;
			bool hoveredClose;
			_tabs.GetTabIndex(location, out hoveredIndex, out hoveredClose);

			if (hoveredIndex == _tabs.AddButtonIndex)
			{
				_tabs.AddTab();
				return;
			}

			if (hoveredIndex < 0 || hoveredIndex == _tabs.SelectedIndex || hoveredIndex >= _tabs.Count)
				return;
			
			_tabs.SelectedIndex = hoveredIndex;
		}

		private FormMain getSelectedForm()
		{
			return (FormMain)_tabs.SelectedTabId;
		}

		private void tabCreated(TabHeaderControl sender, int i)
		{
			var form = (FormMain) _tabs.TabIds[i];

			bool creatingNewForm = form == null;

			if (creatingNewForm)
				form = createFormMain();

			form.SetFormRoot(this);

			if (creatingNewForm)
			{
				form.LoadHistory(AppDir.History.AddPath(Id.ToString()), i.ToString());
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
			var form = getSelectedForm();
			form?.OnTabUnselected();
		}

		private void selectedTabChanged(TabHeaderControl sender, int selected)
		{
			tabSelected();
		}

		private void tabSelected()
		{
			var selectedForm = getSelectedForm();

			if (selectedForm == null)
				return;

			selectedForm.Location = new Point(0, 0);
			selectedForm.Size = _panelClient.Size;

			if (!_panelClient.Controls.Contains(selectedForm))
			{
				_panelClient.Controls.Add(selectedForm);
				selectedForm.Show();
			}

			selectedForm.BringToFront();
			selectedForm.OnTabSelected();
		}

		private void tabClosing(object sender, int i)
		{
			var formMain = (FormMain) _tabs.TabIds[i];

			// implicit connection: move_tab_between_forms
			if (formMain == null)
				return;

			var lastTabId = (_tabs.Count - 1).ToString();

			formMain.SaveHistory(AppDir.History.AddPath(Id.ToString()), lastTabId);
			formMain.Close();
		}

		private void tabClosed(TabHeaderControl sender)
		{
			if (_tabs.Count == 0)
				Close();
		}

		private void formClosing(object sender, EventArgs e)
		{
			_formManager.MoveToEnd(this);

			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain) _tabs.TabIds[i];
				formMain.SaveHistory(AppDir.History.AddPath(Id.ToString()), i.ToString());
			}

			_formManager.Remove(this);

			Application.RemoveMessageFilter(this);
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
			var image = _newsService.HasUnreadNews
				? Resources.update_notification_40
				: Resources.update_40;

			bool enabled = _downloaderSubsystem.IsProgressCalculated && _newsService.NewsLoaded;

			this.Invoke(delegate
			{
				_buttonDownload.Enabled = enabled;
				setupButton(_buttonDownload, image, true);

				if (enabled && _downloaderSubsystem.NeedToSuggestDownloader)
					_downloaderSubsystem.ShowDownloader(this, auto: true);
			});
		}



		private void closed(object sender, EventArgs e)
		{
			unsubsribeButtonEvents();
			SuggestModel.AbortSuggestThread();
		}

		private void queryHandleDrag(object sender, MouseEventArgs e, CancelEventArgs cancelArgs)
		{
			if (sender == _tabs)
				cancelArgs.Cancel = _tabs.HoveredIndex >= 0;
		}



		private FormMain createFormMain()
		{
			var form = _formMainFactory();

			form.TopLevel = false;
			form.ControlBox = false;
			form.FormBorderStyle = FormBorderStyle.None;
			form.Size = _panelClient.Size;
			form.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			form.Location = new Point(0, 0);

			form.TextChanged += formTextChanged;
			return form;
		}

		private void tabMouseMove(object sender, MouseEventArgs e)
		{
			var draggingFromTab = _formManager.FindCardDraggingForm();

			if (draggingFromTab == null)
				return;

			var hoveredForm = (FormMain) _tabs.HoveredTabId;

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

			var tabDraggingForm = _formManager.Forms.FirstOrDefault(_ => _._tabs.IsDragging());

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

			var formMain = (FormMain) dragSourceTabs.TabIds[draggedIndex];

			// implicit connection: move_tab_between_forms
			dragSourceTabs.TabIds[draggedIndex] = null;

			dragSourceTabs.RemoveTab(draggedIndex);

			_formManager.SwapTabs(Id, TabsCount, tabDraggingForm.Id, tabDraggingForm.TabsCount);

			AddTab(formMain);

			_tabs.Capture = true;
			_tabs.BeginDrag(TabsCount - 1);
		}

		public bool ShowFilterPanels
		{
			get { return _buttonFilterPanels.Checked; }
			set { _buttonFilterPanels.Checked = value; }
		}

		public event Action ShowFilterPanelsChanged;

		public bool HideTooltips
		{
			get
			{
				return !TooltipController.Active;
			}
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
			set
			{
				_buttonUndo.Enabled = value;
			}
		}

		public bool CanRedo
		{
			set
			{
				_buttonRedo.Enabled = value;
			}
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

		public void AddTab(Action<object> onCreated)
		{
			Action<TabHeaderControl, int> onTabCreated = (c, i) =>
			{
				var form = (FormMain) c.TabIds[i];
				onCreated(form);
			};

			_tabs.TabAdded += onTabCreated;
			_tabs.AddTab();
			_tabs.TabAdded -= onTabCreated;
		}

		public void AddTab(FormMain form)
		{
			_tabs.AddTab(form);
		}

		public void CloseTab()
		{
			_tabs.RemoveTab(_tabs.SelectedIndex);
		}



		public int TabsCount => _tabs.Count;

		public IEnumerable<FormMain> Tabs => _tabs.TabIds.Cast<FormMain>();

		public SuggestModel SuggestModel { get; }
		public UiModel UiModel { get; }
		public TooltipController TooltipController { get; }

		public bool ShowTextualFields { get; set; }
		public bool ShowDeck { get; set; }
		public bool ShowPartialCards { get; set; }

		private int Id => _formManager.GetId(this);

		public sealed override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		private bool _undoingOrRedoing;



		private readonly Func<FormMain> _formMainFactory;
		private readonly DownloaderSubsystem _downloaderSubsystem;
		private readonly NewsService _newsService;
		private readonly FormManager _formManager;
		private readonly CardRepository _repo;
	}
}
