using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public partial class FormRoot : CustomBorderForm, IUiForm
	{
		public FormRoot()
		{
			InitializeComponent();
		}

		// ReSharper disable once UnusedMember.Global
		public FormRoot(Func<FormMain> formMainFactory,
			DownloaderSubsystem downloaderSubsystem,
			SuggestModel suggestModel,
			Loader loader,
			TooltipController tooltipController,
			CardRepository repository,
			UiModel uiModel)
			:this()
		{
			_tooltipController = tooltipController;

			repository.LoadingComplete += repositoryLoaded;

			_buttonSubsystem = new ButtonSubsystem();
			RegisterDragControl(_tabs);
			QueryHandleDrag += queryHandleDrag;
			Load += load;
			Closed += closed;

			_formMainFactory = formMainFactory;
			_downloaderSubsystem = downloaderSubsystem;
			_suggestModel = suggestModel;
			_loader = loader;

			_tabs.TabAdded += pageCreated;
			_tabs.TabRemoving += pageClosing;
			_tabs.TabRemoved += pageClosed;
			_tabs.SelectedIndexChanging += selectedPageChanging;
			_tabs.SelectedIndexChanged += selectedPageChanged;
			_tabs.TabReordered += pageReordered;

			setupButtons();

			setupExternalLinks();
			setupButtonClicks();
			setupLanguageMenu();
			setupTooltips();

			if (!DesignMode)
				SnapTo(Direction.North, default(Point));

			_deckButtons = new ButtonBase[]
			{
				_buttonLoad,
				_buttonSave,
				_buttonPrint,
				_buttonClear,
				_buttonStat
			};

			foreach (var button in _deckButtons)
				button.Enabled = false;

			Text = $"Mtgdb.Gui v{AppDir.GetVersion()}";

			uiModel.Form = this;
		}

		private void repositoryLoaded()
		{
			this.Invoke(delegate
			{
				foreach (var button in _deckButtons)
					button.Enabled = true;
			});
		}

		private FormMain getSelectedForm()
		{
			return (FormMain)_tabs.SelectedTabId;
		}

		private void suggestDownloader()
		{
			_downloaderSubsystem.CalculateProgress();

			this.Invoke(delegate
			{
				_buttonDownload.Enabled = true;
			});

			if (_downloaderSubsystem.NeedToSuggestDownloader)
				_downloaderSubsystem.ShowDownloader(this, auto: true);
		}

		private void pageCreated(TabHeaderControl sender, int i)
		{
			var form = createFormMain(i);
			_tabs.TabIds[i] = form;
		}



		private void formTextChanged(object sender, EventArgs e)
		{
			var formMain = (FormMain) sender;
			var formIndex = _tabs.TabIds.IndexOf(formMain);

			if (formIndex < 0)
				return;

			string text = formMain.Text.NonEmpty() ?? _tabs.GetDefaultText(formIndex);
			_tabs.SetTabSetting(formMain, new TabSettings(text));
		}

		private void selectedPageChanging(TabHeaderControl sender, int selected)
		{
			pageUnselecting();
		}

		private void selectedPageChanged(TabHeaderControl sender, int selected)
		{
			pageSelected();
		}

		private void pageUnselecting()
		{
			var form = getSelectedForm();

			if (_draggingForm != null && _draggingForm == form)
				_draggingForm.StopDragging();
		}

		private void pageSelected()
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
			selectedForm.OnSelectedTab(_draggedCard);

			_draggingForm = null;
			_draggedCard = null;
		}

		private void pageClosing(object sender, int i)
		{
			var formMain = (FormMain) _tabs.TabIds[i];
			var lastTabId = (_tabs.Count - 1).ToString();
			formMain.SaveHistory(lastTabId);
			formMain.Close();
		}

		private void pageReordered(TabHeaderControl sender)
		{
			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain)_tabs.TabIds[i];
				formMain.SaveHistory(i.ToString());
			}
		}

		private void pageClosed(TabHeaderControl sender)
		{
			pageReordered(sender);

			if (_tabs.Count == 0)
				Close();
		}

		private void load(object sender, EventArgs e)
		{
			_loader.Add(suggestDownloader);
			_loader.Run();

			_suggestModel.StartSuggestThread();
		}

		private void closed(object sender, EventArgs e)
		{
			unsubsribeButtonEvents();
			_loader.Abort();
			_suggestModel.AbortSuggestThread();
		}

		private void queryHandleDrag(object sender, MouseEventArgs e, CancelEventArgs cancelArgs)
		{
			if (sender == _tabs)
				cancelArgs.Cancel = _tabs.HoveredIndex >= 0;
		}



		private FormMain createFormMain(int tabsCount)
		{
			string tabId = tabsCount.ToString();
			var form = _formMainFactory();
			form.SetId(tabId);

			form.TopLevel = false;
			form.ControlBox = false;
			form.FormBorderStyle = FormBorderStyle.None;
			form.Size = _panelClient.Size;
			form.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			form.Location = new Point(0, 0);

			form.TextChanged += formTextChanged;
			_tabs.MouseMove += tabMouseMove;
			return form;
		}

		private void tabMouseMove(object sender, MouseEventArgs e)
		{
			_draggedCard = null;

			var selectedForm = getSelectedForm();

			if (selectedForm == null)
				return;

			if (!selectedForm.IsDraggingCard)
				return;

			_draggedCard = selectedForm.DraggedCard;
			_draggingForm = selectedForm;

			var hoveredForm = (FormMain)_tabs.HoveredTabId;

			if (hoveredForm != null && selectedForm != hoveredForm)
				_tabs.SelectedTabId = hoveredForm;
			else if (_tabs.HoveredIndex == _tabs.AddButtonIndex)
				NewTab();
		}


		public bool HideTooltips
		{
			get
			{
				return !_tooltipController.Active;
			}
			set
			{
				bool showTooltips = !value;
				if (_tooltipController.Active != showTooltips)
					_tooltipController.Active = showTooltips;

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

		public void NextTab()
		{
			int nextPageIndex = 1 + _tabs.SelectedIndex;

			if (nextPageIndex == _tabs.Count)
				nextPageIndex = 0;

			_tabs.SelectedIndex = nextPageIndex;
		}

		public void NewTab()
		{
			_tabs.AddTab();
		}

		public void CloseTab()
		{
			_tabs.RemoveTab(_tabs.SelectedIndex);
		}

		private readonly Func<FormMain> _formMainFactory;
		private readonly DownloaderSubsystem _downloaderSubsystem;
		private readonly SuggestModel _suggestModel;
		private readonly Loader _loader;
		private string _language;

		private bool _undoingOrRedoing;
		private Card _draggedCard;
		private FormMain _draggingForm;
		private readonly TooltipController _tooltipController;
		
		public sealed override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}
	}
}
