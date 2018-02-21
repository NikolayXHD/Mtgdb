using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Mtgdb.Controls;
using Mtgdb.Dal;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public partial class FormRoot : CustomBorderForm, IUiForm
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

			_buttonMenuGeneralSettings.ScaleDpi();

			
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
			SuggestModel suggestModel,
			Loader loader,
			TooltipController tooltipController,
			CardRepository repository)
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
			_tabs.AllowDrop = true;
			_tabs.DragOver += tabsDragOver;

			Application.ApplicationExit += applicationExit;

			KeyDown += formKeyDown;

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

			Language = CardLocalization.DefaultLanguage;
		}

		private void load(object sender, EventArgs e)
		{
			_loader.Add(() =>
			{
				_downloaderSubsystem.FetchNews(repeatViewed: false);
				this.Invoke(updateDownloadButton);
			});

			_loader.Add(() =>
			{
				_downloaderSubsystem.CalculateProgress();

				while (!_downloaderSubsystem.NewsLoaded)
					Thread.Sleep(100);

				this.Invoke(delegate
				{
					_buttonDownload.Enabled = true;
				});

				if (_downloaderSubsystem.NeedToSuggestDownloader)
					_downloaderSubsystem.ShowDownloader(this, auto: true);
			});

			_loader.Run();

			_suggestModel.StartSuggestThread();
		}

		private void repositoryLoaded()
		{
			this.Invoke(delegate
			{
				foreach (var button in _deckButtons)
					button.Enabled = true;
			});
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

		private void pageCreated(TabHeaderControl sender, int i)
		{
			var form = createFormMain();
			form.LoadHistory(i.ToString());

			_tabs.TabIds[i] = form;
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

			form?.OnTabUnselected();
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
			selectedForm.OnTabSelected(_draggedCard);

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

		private void pageClosed(TabHeaderControl sender)
		{
			if (_tabs.Count == 0)
				Close();
		}

		private void applicationExit(object sender, EventArgs e)
		{
			for (int i = 0; i < _tabs.Count; i++)
			{
				var formMain = (FormMain) _tabs.TabIds[i];
				formMain.SaveHistory(i.ToString());
			}
		}

		private void updateDownloadButton()
		{
			var image = _downloaderSubsystem.HasUnreadNews
				? Resources.update_notification_40
				: Resources.update_40;

			setupButton(_buttonDownload, image, true);
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
				NewTab(null);
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

		public void PrevTab()
		{
			int nextPageIndex = _tabs.SelectedIndex - 1;

			if (nextPageIndex < 0)
				nextPageIndex = _tabs.Count - 1;

			_tabs.SelectedIndex = nextPageIndex;
		}

		public void NewTab(Action<object> onCreated)
		{
			if (onCreated != null)
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
			else
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
