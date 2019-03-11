namespace Mtgdb.Gui
{
	sealed partial class FormMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			Mtgdb.Controls.LayoutOptions layoutOptions5 = new Mtgdb.Controls.LayoutOptions();
			Mtgdb.Controls.SearchOptions searchOptions5 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions5 = new Mtgdb.Controls.ButtonOptions();
			Mtgdb.Controls.SelectionOptions selectionOptions5 = new Mtgdb.Controls.SelectionOptions();
			Mtgdb.Controls.SortOptions sortOptions5 = new Mtgdb.Controls.SortOptions();
			Mtgdb.Controls.LayoutOptions layoutOptions6 = new Mtgdb.Controls.LayoutOptions();
			Mtgdb.Controls.SearchOptions searchOptions6 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions6 = new Mtgdb.Controls.ButtonOptions();
			Mtgdb.Controls.SelectionOptions selectionOptions6 = new Mtgdb.Controls.SelectionOptions();
			Mtgdb.Controls.SortOptions sortOptions6 = new Mtgdb.Controls.SortOptions();
			this._panelFilters = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterAbility = new Mtgdb.Controls.QuickFilterControl();
			this.FilterCastKeyword = new Mtgdb.Controls.QuickFilterControl();
			this.FilterType = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManager = new Mtgdb.Controls.QuickFilterControl();
			this.FilterLayout = new Mtgdb.Controls.QuickFilterControl();
			this.FilterRarity = new Mtgdb.Controls.QuickFilterControl();
			this._panelStatus = new System.Windows.Forms.FlowLayoutPanel();
			this._tabHeadersDeck = new Mtgdb.Controls.TabHeaderControl();
			this._buttonSampleHandMulligan = new Mtgdb.Controls.ButtonBase();
			this._buttonSampleHandDraw = new Mtgdb.Controls.ButtonBase();
			this._labelStatusScrollDeck = new System.Windows.Forms.Label();
			this._labelStatusScrollCards = new System.Windows.Forms.Label();
			this._panelMenu = new System.Windows.Forms.TableLayoutPanel();
			this._panelMenuRightSubpanel = new System.Windows.Forms.FlowLayoutPanel();
			this._panelRightCost = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterGeneratedMana = new Mtgdb.Controls.QuickFilterControl();
			this._panelManaAbility = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterCmc = new Mtgdb.Controls.QuickFilterControl();
			this._layoutMain = new System.Windows.Forms.TableLayoutPanel();
			this._layoutRight = new System.Windows.Forms.TableLayoutPanel();
			this._panelRightNarrow = new System.Windows.Forms.FlowLayoutPanel();
			this._panelRightManaCost = new System.Windows.Forms.FlowLayoutPanel();
			this._layoutRoot = new System.Windows.Forms.TableLayoutPanel();
			this._buttonShowScrollDeck = new Mtgdb.Controls.ButtonBase();
			this._buttonShowScrollCards = new Mtgdb.Controls.ButtonBase();
			this._searchBar = new Mtgdb.Controls.SearchBar();
			this._popupSearchExamples = new Mtgdb.Controls.Popup();
			this._panelIconLegality = new Mtgdb.Controls.ControlBase();
			this._dropdownLegality = new Mtgdb.Controls.DropDown();
			this._buttonLegalityAllowLegal = new Mtgdb.Controls.CheckBox();
			this._buttonLegalityAllowRestricted = new Mtgdb.Controls.CheckBox();
			this._buttonLegalityAllowBanned = new Mtgdb.Controls.CheckBox();
			this._buttonLegalityAllowFuture = new Mtgdb.Controls.CheckBox();
			this._buttonShowDuplicates = new Mtgdb.Controls.ButtonBase();
			this._buttonHideDeck = new Mtgdb.Controls.ButtonBase();
			this._buttonShowPartialCards = new Mtgdb.Controls.ButtonBase();
			this._buttonShowText = new Mtgdb.Controls.ButtonBase();
			this._buttonSampleHandNew = new Mtgdb.Controls.ButtonBase();
			this._labelStatusSets = new Mtgdb.Controls.ControlBase();
			this._labelStatusCollection = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterButtons = new Mtgdb.Controls.ControlBase();
			this._labelStatusSearch = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterCollection = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterDeck = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterLegality = new Mtgdb.Controls.ControlBase();
			this._labelStatusSort = new Mtgdb.Controls.ControlBase();
			this.FilterManaAbility = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaAbility = new Mtgdb.Controls.ButtonBase();
			this._buttonShowProhibit = new Mtgdb.Controls.ButtonBase();
			this.FilterManaCost = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaCost = new Mtgdb.Controls.ButtonBase();
			this._buttonResetFilters = new Mtgdb.Controls.ButtonBase();
			this._layoutViewCards = new Mtgdb.Controls.LayoutViewControl();
			this._layoutViewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._deckListControl = new Mtgdb.Controls.DeckListControl();
			this._menuSearchExamples = new Mtgdb.Gui.SearchExamplesMenu();
			this._panelDeckTabsContainer = new System.Windows.Forms.Panel();
			this._panelFilters.SuspendLayout();
			this._panelStatus.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._panelMenuRightSubpanel.SuspendLayout();
			this._panelRightCost.SuspendLayout();
			this._panelManaAbility.SuspendLayout();
			this._layoutMain.SuspendLayout();
			this._layoutRight.SuspendLayout();
			this._panelRightNarrow.SuspendLayout();
			this._panelRightManaCost.SuspendLayout();
			this._layoutRoot.SuspendLayout();
			this._panelDeckTabsContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelFilters
			// 
			this._panelFilters.AutoSize = true;
			this._panelFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layoutMain.SetColumnSpan(this._panelFilters, 3);
			this._panelFilters.Controls.Add(this.FilterAbility);
			this._panelFilters.Controls.Add(this.FilterCastKeyword);
			this._panelFilters.Location = new System.Drawing.Point(0, 0);
			this._panelFilters.Margin = new System.Windows.Forms.Padding(0);
			this._panelFilters.Name = "_panelFilters";
			this._panelFilters.Size = new System.Drawing.Size(26, 34);
			this._panelFilters.TabIndex = 0;
			// 
			// FilterAbility
			// 
			this.FilterAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterAbility.EnableRequiringSome = true;
			this.FilterAbility.HideProhibit = true;
			this.FilterAbility.Location = new System.Drawing.Point(0, 0);
			this.FilterAbility.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this.FilterAbility.MinimumSize = new System.Drawing.Size(8, 34);
			this.FilterAbility.Name = "FilterAbility";
			this.FilterAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterAbility.PropertiesCount = 0;
			this.FilterAbility.PropertyImages = null;
			this.FilterAbility.SelectionBorder = 1.75F;
			this.FilterAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterAbility.Size = new System.Drawing.Size(8, 34);
			this.FilterAbility.Spacing = new System.Drawing.Size(-4, -10);
			this.FilterAbility.TabIndex = 0;
			this.FilterAbility.TabStop = false;
			// 
			// FilterCastKeyword
			// 
			this.FilterCastKeyword.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterCastKeyword.EnableRequiringSome = true;
			this.FilterCastKeyword.HideProhibit = true;
			this.FilterCastKeyword.Location = new System.Drawing.Point(18, 0);
			this.FilterCastKeyword.Margin = new System.Windows.Forms.Padding(0);
			this.FilterCastKeyword.MinimumSize = new System.Drawing.Size(8, 34);
			this.FilterCastKeyword.Name = "FilterCastKeyword";
			this.FilterCastKeyword.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterCastKeyword.PropertiesCount = 0;
			this.FilterCastKeyword.PropertyImages = null;
			this.FilterCastKeyword.SelectionBorder = 1.75F;
			this.FilterCastKeyword.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterCastKeyword.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterCastKeyword.Size = new System.Drawing.Size(8, 34);
			this.FilterCastKeyword.Spacing = new System.Drawing.Size(-4, -10);
			this.FilterCastKeyword.TabIndex = 1;
			this.FilterCastKeyword.TabStop = false;
			// 
			// FilterType
			// 
			this.FilterType.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterType.EnableCostBehavior = true;
			this.FilterType.HideProhibit = true;
			this.FilterType.IsFlipped = true;
			this.FilterType.IsVertical = true;
			this.FilterType.Location = new System.Drawing.Point(0, 0);
			this.FilterType.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.FilterType.MinimumSize = new System.Drawing.Size(44, 2);
			this.FilterType.Name = "FilterType";
			this.FilterType.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterType.PropertiesCount = 0;
			this.FilterType.PropertyImages = null;
			this.FilterType.SelectionBorder = 1.75F;
			this.FilterType.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterType.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterType.Size = new System.Drawing.Size(44, 2);
			this.FilterType.Spacing = new System.Drawing.Size(2, 0);
			this.FilterType.TabIndex = 0;
			this.FilterType.TabStop = false;
			// 
			// FilterManager
			// 
			this.FilterManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FilterManager.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._layoutRight.SetColumnSpan(this.FilterManager, 3);
			this.FilterManager.EnableRequiringSome = true;
			this.FilterManager.HideProhibit = true;
			this.FilterManager.Location = new System.Drawing.Point(110, 833);
			this.FilterManager.Margin = new System.Windows.Forms.Padding(0);
			this.FilterManager.MinimumSize = new System.Drawing.Size(2, 34);
			this.FilterManager.Name = "FilterManager";
			this.FilterManager.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManager.PropertiesCount = 0;
			this.FilterManager.PropertyImages = null;
			this.FilterManager.SelectionBorder = 2F;
			this.FilterManager.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManager.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManager.Size = new System.Drawing.Size(2, 34);
			this.FilterManager.Spacing = new System.Drawing.Size(2, -10);
			this.FilterManager.TabIndex = 6;
			this.FilterManager.TabStop = false;
			// 
			// FilterLayout
			// 
			this.FilterLayout.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterLayout.EnableCostBehavior = true;
			this.FilterLayout.EnableMutuallyExclusive = true;
			this.FilterLayout.HideProhibit = true;
			this.FilterLayout.IsFlipped = true;
			this.FilterLayout.IsVertical = true;
			this.FilterLayout.Location = new System.Drawing.Point(0, 12);
			this.FilterLayout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.FilterLayout.MinimumSize = new System.Drawing.Size(24, 2);
			this.FilterLayout.Name = "FilterLayout";
			this.FilterLayout.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterLayout.PropertiesCount = 0;
			this.FilterLayout.PropertyImages = null;
			this.FilterLayout.SelectionBorder = 1.75F;
			this.FilterLayout.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterLayout.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterLayout.Size = new System.Drawing.Size(24, 2);
			this.FilterLayout.Spacing = new System.Drawing.Size(2, 0);
			this.FilterLayout.TabIndex = 1;
			this.FilterLayout.TabStop = false;
			// 
			// FilterRarity
			// 
			this.FilterRarity.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterRarity.EnableCostBehavior = true;
			this.FilterRarity.EnableMutuallyExclusive = true;
			this.FilterRarity.HideProhibit = true;
			this.FilterRarity.IsFlipped = true;
			this.FilterRarity.IsVertical = true;
			this.FilterRarity.Location = new System.Drawing.Point(0, 0);
			this.FilterRarity.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.FilterRarity.MinimumSize = new System.Drawing.Size(24, 2);
			this.FilterRarity.Name = "FilterRarity";
			this.FilterRarity.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterRarity.PropertiesCount = 0;
			this.FilterRarity.PropertyImages = null;
			this.FilterRarity.SelectionBorder = 1.75F;
			this.FilterRarity.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterRarity.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterRarity.Size = new System.Drawing.Size(24, 2);
			this.FilterRarity.Spacing = new System.Drawing.Size(2, 0);
			this.FilterRarity.TabIndex = 0;
			this.FilterRarity.TabStop = false;
			// 
			// _panelStatus
			// 
			this._panelStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelStatus.AutoSize = true;
			this._panelStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelStatus.Controls.Add(this._buttonHideDeck);
			this._panelStatus.Controls.Add(this._buttonShowPartialCards);
			this._panelStatus.Controls.Add(this._buttonShowText);
			this._panelStatus.Controls.Add(this._panelDeckTabsContainer);
			this._panelStatus.Controls.Add(this._buttonSampleHandNew);
			this._panelStatus.Controls.Add(this._buttonSampleHandMulligan);
			this._panelStatus.Controls.Add(this._buttonSampleHandDraw);
			this._panelStatus.Controls.Add(this._labelStatusSets);
			this._panelStatus.Controls.Add(this._labelStatusCollection);
			this._panelStatus.Controls.Add(this._labelStatusFilterButtons);
			this._panelStatus.Controls.Add(this._labelStatusSearch);
			this._panelStatus.Controls.Add(this._labelStatusFilterCollection);
			this._panelStatus.Controls.Add(this._labelStatusFilterDeck);
			this._panelStatus.Controls.Add(this._labelStatusFilterLegality);
			this._panelStatus.Controls.Add(this._labelStatusSort);
			this._panelStatus.Location = new System.Drawing.Point(0, 221);
			this._panelStatus.Margin = new System.Windows.Forms.Padding(0);
			this._panelStatus.Name = "_panelStatus";
			this._panelStatus.Size = new System.Drawing.Size(1171, 24);
			this._panelStatus.TabIndex = 5;
			// 
			// _tabHeadersDeck
			// 
			this._tabHeadersDeck.AddButtonSlopeSize = new System.Drawing.Size(6, 14);
			this._tabHeadersDeck.AllowAddingTabs = false;
			this._tabHeadersDeck.AllowRemovingTabs = false;
			this._tabHeadersDeck.AllowReorderTabs = false;
			this._tabHeadersDeck.ColorSelected = System.Drawing.SystemColors.Window;
			this._tabHeadersDeck.ColorSelectedHovered = System.Drawing.SystemColors.Window;
			this._tabHeadersDeck.ColorUnselected = System.Drawing.SystemColors.Control;
			this._tabHeadersDeck.ColorUnselectedHovered = System.Drawing.SystemColors.Window;
			this._tabHeadersDeck.Count = 5;
			this._tabHeadersDeck.Location = new System.Drawing.Point(0, 0);
			this._tabHeadersDeck.Margin = new System.Windows.Forms.Padding(0);
			this._tabHeadersDeck.Name = "_tabHeadersDeck";
			this._tabHeadersDeck.SelectedIndex = 4;
			this._tabHeadersDeck.Size = new System.Drawing.Size(144, 24);
			this._tabHeadersDeck.SlopeSize = new System.Drawing.Size(9, 21);
			this._tabHeadersDeck.TabIndex = 12;
			this._tabHeadersDeck.TabStop = false;
			// 
			// _buttonSampleHandMulligan
			// 
			this._buttonSampleHandMulligan.AutoCheck = false;
			this._buttonSampleHandMulligan.ImageScale = 0.5F;
			this._buttonSampleHandMulligan.Location = new System.Drawing.Point(301, 0);
			this._buttonSampleHandMulligan.Margin = new System.Windows.Forms.Padding(0);
			this._buttonSampleHandMulligan.Name = "_buttonSampleHandMulligan";
			this._buttonSampleHandMulligan.Size = new System.Drawing.Size(53, 24);
			this._buttonSampleHandMulligan.TabIndex = 14;
			this._buttonSampleHandMulligan.Text = "mulligan";
			// 
			// _buttonSampleHandDraw
			// 
			this._buttonSampleHandDraw.AutoCheck = false;
			this._buttonSampleHandDraw.ImageScale = 0.5F;
			this._buttonSampleHandDraw.Location = new System.Drawing.Point(354, 0);
			this._buttonSampleHandDraw.Margin = new System.Windows.Forms.Padding(0);
			this._buttonSampleHandDraw.Name = "_buttonSampleHandDraw";
			this._buttonSampleHandDraw.Size = new System.Drawing.Size(38, 24);
			this._buttonSampleHandDraw.TabIndex = 15;
			this._buttonSampleHandDraw.Text = "draw";
			// 
			// _labelStatusScrollDeck
			// 
			this._labelStatusScrollDeck.Location = new System.Drawing.Point(1171, 221);
			this._labelStatusScrollDeck.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusScrollDeck.Name = "_labelStatusScrollDeck";
			this._labelStatusScrollDeck.Size = new System.Drawing.Size(79, 24);
			this._labelStatusScrollDeck.TabIndex = 5;
			this._labelStatusScrollDeck.Text = "63/60";
			this._labelStatusScrollDeck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _labelStatusScrollCards
			// 
			this._labelStatusScrollCards.Location = new System.Drawing.Point(1171, 34);
			this._labelStatusScrollCards.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this._labelStatusScrollCards.Name = "_labelStatusScrollCards";
			this._labelStatusScrollCards.Size = new System.Drawing.Size(79, 21);
			this._labelStatusScrollCards.TabIndex = 2;
			this._labelStatusScrollCards.Text = "35999/36008";
			this._labelStatusScrollCards.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _panelMenu
			// 
			this._panelMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelMenu.AutoSize = true;
			this._panelMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelMenu.Controls.Add(this._searchBar, 0, 0);
			this._panelMenu.Controls.Add(this._panelMenuRightSubpanel, 1, 0);
			this._panelMenu.Location = new System.Drawing.Point(0, 34);
			this._panelMenu.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelMenu.Size = new System.Drawing.Size(1171, 24);
			this._panelMenu.TabIndex = 1;
			// 
			// _panelMenuRightSubpanel
			// 
			this._panelMenuRightSubpanel.AutoSize = true;
			this._panelMenuRightSubpanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenuRightSubpanel.Controls.Add(this._popupSearchExamples);
			this._panelMenuRightSubpanel.Controls.Add(this._panelIconLegality);
			this._panelMenuRightSubpanel.Controls.Add(this._dropdownLegality);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowLegal);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowRestricted);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowBanned);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowFuture);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonShowDuplicates);
			this._panelMenuRightSubpanel.Location = new System.Drawing.Point(733, 0);
			this._panelMenuRightSubpanel.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenuRightSubpanel.Name = "_panelMenuRightSubpanel";
			this._panelMenuRightSubpanel.Size = new System.Drawing.Size(438, 24);
			this._panelMenuRightSubpanel.TabIndex = 2;
			this._panelMenuRightSubpanel.WrapContents = false;
			// 
			// _panelRightCost
			// 
			this._panelRightCost.AutoSize = true;
			this._panelRightCost.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelRightCost.Controls.Add(this.FilterType);
			this._panelRightCost.Controls.Add(this.FilterGeneratedMana);
			this._panelRightCost.Controls.Add(this._panelManaAbility);
			this._panelRightCost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelRightCost.Location = new System.Drawing.Point(44, 0);
			this._panelRightCost.Margin = new System.Windows.Forms.Padding(0);
			this._panelRightCost.Name = "_panelRightCost";
			this._layoutRight.SetRowSpan(this._panelRightCost, 2);
			this._panelRightCost.Size = new System.Drawing.Size(44, 50);
			this._panelRightCost.TabIndex = 1;
			// 
			// FilterGeneratedMana
			// 
			this.FilterGeneratedMana.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterGeneratedMana.EnableRequiringSome = true;
			this.FilterGeneratedMana.HideProhibit = true;
			this.FilterGeneratedMana.IsFlipped = true;
			this.FilterGeneratedMana.IsVertical = true;
			this.FilterGeneratedMana.Location = new System.Drawing.Point(0, 12);
			this.FilterGeneratedMana.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.FilterGeneratedMana.MinimumSize = new System.Drawing.Size(44, 2);
			this.FilterGeneratedMana.Name = "FilterGeneratedMana";
			this.FilterGeneratedMana.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterGeneratedMana.PropertiesCount = 0;
			this.FilterGeneratedMana.PropertyImages = null;
			this.FilterGeneratedMana.SelectionBorder = 1.75F;
			this.FilterGeneratedMana.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterGeneratedMana.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterGeneratedMana.Size = new System.Drawing.Size(44, 2);
			this.FilterGeneratedMana.Spacing = new System.Drawing.Size(2, 0);
			this.FilterGeneratedMana.TabIndex = 1;
			this.FilterGeneratedMana.TabStop = false;
			// 
			// _panelManaAbility
			// 
			this._panelManaAbility.AutoSize = true;
			this._panelManaAbility.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelManaAbility.Controls.Add(this.FilterManaAbility);
			this._panelManaAbility.Controls.Add(this._buttonExcludeManaAbility);
			this._panelManaAbility.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelManaAbility.Location = new System.Drawing.Point(0, 24);
			this._panelManaAbility.Margin = new System.Windows.Forms.Padding(0);
			this._panelManaAbility.Name = "_panelManaAbility";
			this._panelManaAbility.Size = new System.Drawing.Size(44, 26);
			this._panelManaAbility.TabIndex = 2;
			this._panelManaAbility.WrapContents = false;
			// 
			// FilterCmc
			// 
			this.FilterCmc.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterCmc.EnableCostBehavior = true;
			this.FilterCmc.EnableMutuallyExclusive = true;
			this.FilterCmc.HideProhibit = true;
			this.FilterCmc.IsFlipped = true;
			this.FilterCmc.IsVertical = true;
			this.FilterCmc.Location = new System.Drawing.Point(0, 24);
			this.FilterCmc.Margin = new System.Windows.Forms.Padding(0);
			this.FilterCmc.MinimumSize = new System.Drawing.Size(24, 2);
			this.FilterCmc.Name = "FilterCmc";
			this.FilterCmc.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterCmc.PropertiesCount = 0;
			this.FilterCmc.PropertyImages = null;
			this.FilterCmc.SelectionBorder = 1.75F;
			this.FilterCmc.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterCmc.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterCmc.Size = new System.Drawing.Size(24, 2);
			this.FilterCmc.Spacing = new System.Drawing.Size(2, 0);
			this.FilterCmc.TabIndex = 2;
			this.FilterCmc.TabStop = false;
			// 
			// _layoutMain
			// 
			this._layoutMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutMain.ColumnCount = 3;
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutMain.Controls.Add(this._labelStatusScrollCards, 1, 1);
			this._layoutMain.Controls.Add(this._buttonShowScrollDeck, 2, 3);
			this._layoutMain.Controls.Add(this._labelStatusScrollDeck, 1, 3);
			this._layoutMain.Controls.Add(this._buttonShowScrollCards, 2, 1);
			this._layoutMain.Controls.Add(this._layoutViewCards, 0, 2);
			this._layoutMain.Controls.Add(this._panelFilters, 0, 0);
			this._layoutMain.Controls.Add(this._panelMenu, 0, 1);
			this._layoutMain.Controls.Add(this._panelStatus, 0, 3);
			this._layoutMain.Controls.Add(this._layoutViewDeck, 0, 4);
			this._layoutMain.Controls.Add(this._deckListControl, 0, 5);
			this._layoutMain.Location = new System.Drawing.Point(0, 0);
			this._layoutMain.Margin = new System.Windows.Forms.Padding(0);
			this._layoutMain.Name = "_layoutMain";
			this._layoutMain.RowCount = 6;
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.Size = new System.Drawing.Size(1267, 867);
			this._layoutMain.TabIndex = 0;
			// 
			// _layoutRight
			// 
			this._layoutRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutRight.AutoSize = true;
			this._layoutRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layoutRight.ColumnCount = 3;
			this._layoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
			this._layoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
			this._layoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layoutRight.Controls.Add(this._buttonShowProhibit, 2, 1);
			this._layoutRight.Controls.Add(this._panelRightNarrow, 2, 0);
			this._layoutRight.Controls.Add(this._panelRightManaCost, 0, 0);
			this._layoutRight.Controls.Add(this._panelRightCost, 1, 0);
			this._layoutRight.Controls.Add(this.FilterManager, 0, 2);
			this._layoutRight.Controls.Add(this._buttonResetFilters, 0, 1);
			this._layoutRight.Location = new System.Drawing.Point(1267, 0);
			this._layoutRight.Margin = new System.Windows.Forms.Padding(0);
			this._layoutRight.Name = "_layoutRight";
			this._layoutRight.RowCount = 3;
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutRight.Size = new System.Drawing.Size(112, 867);
			this._layoutRight.TabIndex = 1;
			// 
			// _panelRightNarrow
			// 
			this._panelRightNarrow.AutoSize = true;
			this._panelRightNarrow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelRightNarrow.Controls.Add(this.FilterRarity);
			this._panelRightNarrow.Controls.Add(this.FilterLayout);
			this._panelRightNarrow.Controls.Add(this.FilterCmc);
			this._panelRightNarrow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelRightNarrow.Location = new System.Drawing.Point(88, 0);
			this._panelRightNarrow.Margin = new System.Windows.Forms.Padding(0);
			this._panelRightNarrow.Name = "_panelRightNarrow";
			this._panelRightNarrow.Size = new System.Drawing.Size(24, 26);
			this._panelRightNarrow.TabIndex = 2;
			// 
			// _panelRightManaCost
			// 
			this._panelRightManaCost.AutoSize = true;
			this._panelRightManaCost.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelRightManaCost.Controls.Add(this.FilterManaCost);
			this._panelRightManaCost.Controls.Add(this._buttonExcludeManaCost);
			this._panelRightManaCost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelRightManaCost.Location = new System.Drawing.Point(0, 0);
			this._panelRightManaCost.Margin = new System.Windows.Forms.Padding(0);
			this._panelRightManaCost.Name = "_panelRightManaCost";
			this._panelRightManaCost.Size = new System.Drawing.Size(44, 26);
			this._panelRightManaCost.TabIndex = 0;
			this._panelRightManaCost.WrapContents = false;
			// 
			// _layoutRoot
			// 
			this._layoutRoot.ColumnCount = 2;
			this._layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutRoot.Controls.Add(this._layoutMain, 0, 0);
			this._layoutRoot.Controls.Add(this._layoutRight, 1, 0);
			this._layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutRoot.Location = new System.Drawing.Point(0, 0);
			this._layoutRoot.Name = "_layoutRoot";
			this._layoutRoot.RowCount = 1;
			this._layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRoot.Size = new System.Drawing.Size(1379, 867);
			this._layoutRoot.TabIndex = 0;
			// 
			// _buttonShowScrollDeck
			// 
			this._buttonShowScrollDeck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowScrollDeck.Checked = true;
			this._buttonShowScrollDeck.Image = global::Mtgdb.Gui.Properties.Resources.scroll_shown_40;
			this._buttonShowScrollDeck.ImageScale = 0.5F;
			this._buttonShowScrollDeck.Location = new System.Drawing.Point(1250, 221);
			this._buttonShowScrollDeck.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowScrollDeck.Name = "_buttonShowScrollDeck";
			this._buttonShowScrollDeck.Size = new System.Drawing.Size(17, 24);
			this._buttonShowScrollDeck.TabIndex = 6;
			// 
			// _buttonShowScrollCards
			// 
			this._buttonShowScrollCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowScrollCards.Checked = true;
			this._buttonShowScrollCards.Image = global::Mtgdb.Gui.Properties.Resources.scroll_shown_40;
			this._buttonShowScrollCards.ImageScale = 0.5F;
			this._buttonShowScrollCards.Location = new System.Drawing.Point(1250, 34);
			this._buttonShowScrollCards.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowScrollCards.Name = "_buttonShowScrollCards";
			this._buttonShowScrollCards.Size = new System.Drawing.Size(17, 24);
			this._buttonShowScrollCards.TabIndex = 3;
			// 
			// _searchBar
			// 
			this._searchBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._searchBar.Font = new System.Drawing.Font("Source Code Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._searchBar.Image = global::Mtgdb.Gui.Properties.Resources.search_48;
			this._searchBar.ImageScale = 0.5F;
			this._searchBar.Location = new System.Drawing.Point(0, 0);
			this._searchBar.Margin = new System.Windows.Forms.Padding(0);
			this._searchBar.Name = "_searchBar";
			this._searchBar.SelectedIndex = -1;
			this._searchBar.Size = new System.Drawing.Size(733, 24);
			this._searchBar.TabIndex = 1;
			this._searchBar.VisibleAllBorders = null;
			this._searchBar.VisibleBorders = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _popupSearchExamples
			// 
			this._popupSearchExamples.Image = global::Mtgdb.Gui.Properties.Resources.book_40;
			this._popupSearchExamples.ImageScale = 0.5F;
			this._popupSearchExamples.Location = new System.Drawing.Point(0, 0);
			this._popupSearchExamples.Margin = new System.Windows.Forms.Padding(0);
			this._popupSearchExamples.Name = "_popupSearchExamples";
			this._popupSearchExamples.Size = new System.Drawing.Size(24, 24);
			this._popupSearchExamples.TabIndex = 0;
			// 
			// _panelIconLegality
			// 
			this._panelIconLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconLegality.Image = global::Mtgdb.Gui.Properties.Resources.legality_48;
			this._panelIconLegality.ImageScale = 0.5F;
			this._panelIconLegality.Location = new System.Drawing.Point(36, 0);
			this._panelIconLegality.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconLegality.Name = "_panelIconLegality";
			this._panelIconLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconLegality.TabIndex = 0;
			// 
			// _dropdownLegality
			// 
			this._dropdownLegality.EmptySelectionText = "";
			this._dropdownLegality.ImageScale = 0.5F;
			this._dropdownLegality.Location = new System.Drawing.Point(62, 0);
			this._dropdownLegality.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._dropdownLegality.Name = "_dropdownLegality";
			this._dropdownLegality.SelectedIndex = -1;
			this._dropdownLegality.Size = new System.Drawing.Size(96, 24);
			this._dropdownLegality.TabIndex = 1;
			this._dropdownLegality.Text = "[ Any format ]";
			// 
			// _buttonLegalityAllowLegal
			// 
			this._buttonLegalityAllowLegal.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLegalityAllowLegal.Checked = true;
			this._buttonLegalityAllowLegal.Enabled = false;
			this._buttonLegalityAllowLegal.ImageScale = 0.5F;
			this._buttonLegalityAllowLegal.Location = new System.Drawing.Point(158, 0);
			this._buttonLegalityAllowLegal.Margin = new System.Windows.Forms.Padding(0);
			this._buttonLegalityAllowLegal.Name = "_buttonLegalityAllowLegal";
			this._buttonLegalityAllowLegal.Size = new System.Drawing.Size(53, 24);
			this._buttonLegalityAllowLegal.TabIndex = 2;
			this._buttonLegalityAllowLegal.Text = "legal";
			// 
			// _buttonLegalityAllowRestricted
			// 
			this._buttonLegalityAllowRestricted.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLegalityAllowRestricted.Checked = true;
			this._buttonLegalityAllowRestricted.Enabled = false;
			this._buttonLegalityAllowRestricted.ImageScale = 0.5F;
			this._buttonLegalityAllowRestricted.Location = new System.Drawing.Point(211, 0);
			this._buttonLegalityAllowRestricted.Margin = new System.Windows.Forms.Padding(0);
			this._buttonLegalityAllowRestricted.Name = "_buttonLegalityAllowRestricted";
			this._buttonLegalityAllowRestricted.Size = new System.Drawing.Size(74, 24);
			this._buttonLegalityAllowRestricted.TabIndex = 3;
			this._buttonLegalityAllowRestricted.Text = "restricted";
			// 
			// _buttonLegalityAllowBanned
			// 
			this._buttonLegalityAllowBanned.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLegalityAllowBanned.Enabled = false;
			this._buttonLegalityAllowBanned.ImageScale = 0.5F;
			this._buttonLegalityAllowBanned.Location = new System.Drawing.Point(285, 0);
			this._buttonLegalityAllowBanned.Margin = new System.Windows.Forms.Padding(0);
			this._buttonLegalityAllowBanned.Name = "_buttonLegalityAllowBanned";
			this._buttonLegalityAllowBanned.Size = new System.Drawing.Size(67, 24);
			this._buttonLegalityAllowBanned.TabIndex = 4;
			this._buttonLegalityAllowBanned.Text = "banned";
			// 
			// _buttonLegalityAllowFuture
			// 
			this._buttonLegalityAllowFuture.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLegalityAllowFuture.Checked = true;
			this._buttonLegalityAllowFuture.Enabled = false;
			this._buttonLegalityAllowFuture.ImageScale = 0.5F;
			this._buttonLegalityAllowFuture.Location = new System.Drawing.Point(352, 0);
			this._buttonLegalityAllowFuture.Margin = new System.Windows.Forms.Padding(0);
			this._buttonLegalityAllowFuture.Name = "_buttonLegalityAllowFuture";
			this._buttonLegalityAllowFuture.Size = new System.Drawing.Size(58, 24);
			this._buttonLegalityAllowFuture.TabIndex = 5;
			this._buttonLegalityAllowFuture.Text = "future";
			// 
			// _buttonShowDuplicates
			// 
			this._buttonShowDuplicates.Image = global::Mtgdb.Gui.Properties.Resources.clone_48;
			this._buttonShowDuplicates.ImageScale = 0.5F;
			this._buttonShowDuplicates.Location = new System.Drawing.Point(414, 0);
			this._buttonShowDuplicates.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this._buttonShowDuplicates.Name = "_buttonShowDuplicates";
			this._buttonShowDuplicates.Size = new System.Drawing.Size(24, 24);
			this._buttonShowDuplicates.TabIndex = 7;
			// 
			// _buttonHideDeck
			// 
			this._buttonHideDeck.HighlightCheckedOpacity = 0;
			this._buttonHideDeck.ImageChecked = global::Mtgdb.Gui.Properties.Resources.hidden_40;
			this._buttonHideDeck.ImageScale = 0.5F;
			this._buttonHideDeck.ImageUnchecked = global::Mtgdb.Gui.Properties.Resources.shown_40;
			this._buttonHideDeck.Location = new System.Drawing.Point(0, 0);
			this._buttonHideDeck.Margin = new System.Windows.Forms.Padding(0);
			this._buttonHideDeck.Name = "_buttonHideDeck";
			this._buttonHideDeck.Size = new System.Drawing.Size(24, 24);
			this._buttonHideDeck.TabIndex = 3;
			// 
			// _buttonShowPartialCards
			// 
			this._buttonShowPartialCards.Checked = true;
			this._buttonShowPartialCards.Image = global::Mtgdb.Gui.Properties.Resources.partial_card_enabled_40;
			this._buttonShowPartialCards.ImageScale = 0.5F;
			this._buttonShowPartialCards.Location = new System.Drawing.Point(24, 0);
			this._buttonShowPartialCards.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowPartialCards.Name = "_buttonShowPartialCards";
			this._buttonShowPartialCards.Size = new System.Drawing.Size(24, 24);
			this._buttonShowPartialCards.TabIndex = 10;
			// 
			// _buttonShowText
			// 
			this._buttonShowText.Checked = true;
			this._buttonShowText.Image = global::Mtgdb.Gui.Properties.Resources.text_enabled_40;
			this._buttonShowText.ImageScale = 0.5F;
			this._buttonShowText.Location = new System.Drawing.Point(48, 0);
			this._buttonShowText.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowText.Name = "_buttonShowText";
			this._buttonShowText.Size = new System.Drawing.Size(24, 24);
			this._buttonShowText.TabIndex = 11;
			// 
			// _buttonSampleHandNew
			// 
			this._buttonSampleHandNew.AutoCheck = false;
			this._buttonSampleHandNew.AutoSize = true;
			this._buttonSampleHandNew.Image = global::Mtgdb.Gui.Properties.Resources.hand_48;
			this._buttonSampleHandNew.ImageScale = 0.5F;
			this._buttonSampleHandNew.Location = new System.Drawing.Point(216, 0);
			this._buttonSampleHandNew.Margin = new System.Windows.Forms.Padding(0);
			this._buttonSampleHandNew.Name = "_buttonSampleHandNew";
			this._buttonSampleHandNew.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._buttonSampleHandNew.Size = new System.Drawing.Size(85, 24);
			this._buttonSampleHandNew.TabIndex = 13;
			this._buttonSampleHandNew.Text = "new hand";
			// 
			// _labelStatusSets
			// 
			this._labelStatusSets.AutoSize = true;
			this._labelStatusSets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusSets.Image = global::Mtgdb.Gui.Properties.Resources.mtg_48;
			this._labelStatusSets.ImageScale = 0.5F;
			this._labelStatusSets.Location = new System.Drawing.Point(392, 0);
			this._labelStatusSets.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusSets.Name = "_labelStatusSets";
			this._labelStatusSets.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusSets.Size = new System.Drawing.Size(53, 24);
			this._labelStatusSets.TabIndex = 7;
			this._labelStatusSets.Text = "432";
			// 
			// _labelStatusCollection
			// 
			this._labelStatusCollection.AutoSize = true;
			this._labelStatusCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusCollection.Image = global::Mtgdb.Gui.Properties.Resources.box_48;
			this._labelStatusCollection.ImageScale = 0.5F;
			this._labelStatusCollection.Location = new System.Drawing.Point(445, 0);
			this._labelStatusCollection.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusCollection.Name = "_labelStatusCollection";
			this._labelStatusCollection.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusCollection.Size = new System.Drawing.Size(59, 24);
			this._labelStatusCollection.TabIndex = 9;
			this._labelStatusCollection.Text = "1231";
			// 
			// _labelStatusFilterButtons
			// 
			this._labelStatusFilterButtons.AutoSize = true;
			this._labelStatusFilterButtons.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterButtons.Image = global::Mtgdb.Gui.Properties.Resources.quick_filters_48;
			this._labelStatusFilterButtons.ImageScale = 0.5F;
			this._labelStatusFilterButtons.Location = new System.Drawing.Point(504, 0);
			this._labelStatusFilterButtons.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterButtons.Name = "_labelStatusFilterButtons";
			this._labelStatusFilterButtons.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterButtons.Size = new System.Drawing.Size(53, 24);
			this._labelStatusFilterButtons.TabIndex = 11;
			this._labelStatusFilterButtons.Text = "and";
			// 
			// _labelStatusSearch
			// 
			this._labelStatusSearch.AutoSize = true;
			this._labelStatusSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusSearch.Image = global::Mtgdb.Gui.Properties.Resources.search_48;
			this._labelStatusSearch.ImageScale = 0.5F;
			this._labelStatusSearch.Location = new System.Drawing.Point(557, 0);
			this._labelStatusSearch.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusSearch.Name = "_labelStatusSearch";
			this._labelStatusSearch.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusSearch.Size = new System.Drawing.Size(53, 24);
			this._labelStatusSearch.TabIndex = 13;
			this._labelStatusSearch.Text = "and";
			// 
			// _labelStatusFilterCollection
			// 
			this._labelStatusFilterCollection.AutoSize = true;
			this._labelStatusFilterCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterCollection.Image = global::Mtgdb.Gui.Properties.Resources.box_48;
			this._labelStatusFilterCollection.ImageScale = 0.5F;
			this._labelStatusFilterCollection.Location = new System.Drawing.Point(610, 0);
			this._labelStatusFilterCollection.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterCollection.Name = "_labelStatusFilterCollection";
			this._labelStatusFilterCollection.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterCollection.Size = new System.Drawing.Size(52, 24);
			this._labelStatusFilterCollection.TabIndex = 15;
			this._labelStatusFilterCollection.Text = "any";
			// 
			// _labelStatusFilterDeck
			// 
			this._labelStatusFilterDeck.AutoSize = true;
			this._labelStatusFilterDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterDeck.Image = global::Mtgdb.Gui.Properties.Resources.deck_48;
			this._labelStatusFilterDeck.ImageScale = 0.5F;
			this._labelStatusFilterDeck.Location = new System.Drawing.Point(662, 0);
			this._labelStatusFilterDeck.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterDeck.Name = "_labelStatusFilterDeck";
			this._labelStatusFilterDeck.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterDeck.Size = new System.Drawing.Size(52, 24);
			this._labelStatusFilterDeck.TabIndex = 17;
			this._labelStatusFilterDeck.Text = "any";
			// 
			// _labelStatusFilterLegality
			// 
			this._labelStatusFilterLegality.AutoSize = true;
			this._labelStatusFilterLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterLegality.Image = global::Mtgdb.Gui.Properties.Resources.legality_48;
			this._labelStatusFilterLegality.ImageScale = 0.5F;
			this._labelStatusFilterLegality.Location = new System.Drawing.Point(714, 0);
			this._labelStatusFilterLegality.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterLegality.Name = "_labelStatusFilterLegality";
			this._labelStatusFilterLegality.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterLegality.Size = new System.Drawing.Size(135, 24);
			this._labelStatusFilterLegality.TabIndex = 19;
			this._labelStatusFilterLegality.Text = "Standard +L +R -B -F";
			// 
			// _labelStatusSort
			// 
			this._labelStatusSort.AutoSize = true;
			this._labelStatusSort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusSort.Image = global::Mtgdb.Gui.Properties.Resources.sort_48;
			this._labelStatusSort.ImageScale = 0.5F;
			this._labelStatusSort.Location = new System.Drawing.Point(849, 0);
			this._labelStatusSort.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusSort.Name = "_labelStatusSort";
			this._labelStatusSort.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusSort.Size = new System.Drawing.Size(72, 24);
			this._labelStatusSort.TabIndex = 21;
			this._labelStatusSort.Text = "Name ^";
			// 
			// FilterManaAbility
			// 
			this.FilterManaAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManaAbility.EnableRequiringSome = true;
			this.FilterManaAbility.HideProhibit = true;
			this.FilterManaAbility.HintIcon = global::Mtgdb.Gui.Properties.Resources.manatext_25;
			this.FilterManaAbility.HintTextShift = new System.Drawing.Size(2, -6);
			this.FilterManaAbility.IsFlipped = true;
			this.FilterManaAbility.IsVertical = true;
			this.FilterManaAbility.Location = new System.Drawing.Point(0, 0);
			this.FilterManaAbility.Margin = new System.Windows.Forms.Padding(0);
			this.FilterManaAbility.MaximumSize = new System.Drawing.Size(50, 200);
			this.FilterManaAbility.MinimumSize = new System.Drawing.Size(44, 2);
			this.FilterManaAbility.Name = "FilterManaAbility";
			this.FilterManaAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaAbility.PropertiesCount = 0;
			this.FilterManaAbility.PropertyImages = null;
			this.FilterManaAbility.SelectionBorder = 1.75F;
			this.FilterManaAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaAbility.Size = new System.Drawing.Size(44, 2);
			this.FilterManaAbility.Spacing = new System.Drawing.Size(2, 0);
			this.FilterManaAbility.TabIndex = 0;
			this.FilterManaAbility.TabStop = false;
			// 
			// _buttonExcludeManaAbility
			// 
			this._buttonExcludeManaAbility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonExcludeManaAbility.HighlightCheckedOpacity = 0;
			this._buttonExcludeManaAbility.ImageChecked = global::Mtgdb.Gui.Properties.Resources.exclude_minus_24;
			this._buttonExcludeManaAbility.ImageScale = 0.5F;
			this._buttonExcludeManaAbility.ImageUnchecked = global::Mtgdb.Gui.Properties.Resources.include_plus_24;
			this._buttonExcludeManaAbility.Location = new System.Drawing.Point(0, 2);
			this._buttonExcludeManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._buttonExcludeManaAbility.Name = "_buttonExcludeManaAbility";
			this._buttonExcludeManaAbility.Size = new System.Drawing.Size(24, 24);
			this._buttonExcludeManaAbility.TabIndex = 18;
			// 
			// _buttonShowProhibit
			// 
			this._buttonShowProhibit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowProhibit.ImageChecked = global::Mtgdb.Gui.Properties.Resources.exclude_shown_24;
			this._buttonShowProhibit.ImageScale = 0.5F;
			this._buttonShowProhibit.ImageUnchecked = global::Mtgdb.Gui.Properties.Resources.exclude_hidden_24;
			this._buttonShowProhibit.Location = new System.Drawing.Point(88, 809);
			this._buttonShowProhibit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowProhibit.Name = "_buttonShowProhibit";
			this._buttonShowProhibit.Size = new System.Drawing.Size(24, 24);
			this._buttonShowProhibit.TabIndex = 4;
			// 
			// FilterManaCost
			// 
			this.FilterManaCost.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManaCost.EnableCostBehavior = true;
			this.FilterManaCost.HideProhibit = true;
			this.FilterManaCost.HintIcon = global::Mtgdb.Gui.Properties.Resources.manacost_25;
			this.FilterManaCost.HintIconShift = new System.Drawing.Size(0, -6);
			this.FilterManaCost.HintTextShift = new System.Drawing.Size(0, 4);
			this.FilterManaCost.IsFlipped = true;
			this.FilterManaCost.IsVertical = true;
			this.FilterManaCost.Location = new System.Drawing.Point(0, 0);
			this.FilterManaCost.Margin = new System.Windows.Forms.Padding(0);
			this.FilterManaCost.MinimumSize = new System.Drawing.Size(44, 2);
			this.FilterManaCost.Name = "FilterManaCost";
			this.FilterManaCost.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaCost.PropertiesCount = 0;
			this.FilterManaCost.PropertyImages = null;
			this.FilterManaCost.SelectionBorder = 1.75F;
			this.FilterManaCost.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaCost.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaCost.Size = new System.Drawing.Size(44, 2);
			this.FilterManaCost.Spacing = new System.Drawing.Size(2, 0);
			this.FilterManaCost.TabIndex = 0;
			this.FilterManaCost.TabStop = false;
			// 
			// _buttonExcludeManaCost
			// 
			this._buttonExcludeManaCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonExcludeManaCost.Checked = true;
			this._buttonExcludeManaCost.HighlightCheckedOpacity = 0;
			this._buttonExcludeManaCost.ImageChecked = global::Mtgdb.Gui.Properties.Resources.exclude_minus_24;
			this._buttonExcludeManaCost.ImageScale = 0.5F;
			this._buttonExcludeManaCost.ImageUnchecked = global::Mtgdb.Gui.Properties.Resources.include_plus_24;
			this._buttonExcludeManaCost.Location = new System.Drawing.Point(0, 2);
			this._buttonExcludeManaCost.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaCost.Name = "_buttonExcludeManaCost";
			this._buttonExcludeManaCost.Size = new System.Drawing.Size(24, 24);
			this._buttonExcludeManaCost.TabIndex = 9;
			// 
			// _buttonResetFilters
			// 
			this._buttonResetFilters.AutoCheck = false;
			this._buttonResetFilters.Image = global::Mtgdb.Gui.Properties.Resources.erase;
			this._buttonResetFilters.ImageScale = 0.5F;
			this._buttonResetFilters.Location = new System.Drawing.Point(0, 809);
			this._buttonResetFilters.Margin = new System.Windows.Forms.Padding(0);
			this._buttonResetFilters.Name = "_buttonResetFilters";
			this._buttonResetFilters.Size = new System.Drawing.Size(24, 24);
			this._buttonResetFilters.TabIndex = 3;
			// 
			// _layoutViewCards
			// 
			this._layoutViewCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutViewCards.BackColor = System.Drawing.SystemColors.Window;
			this._layoutMain.SetColumnSpan(this._layoutViewCards, 3);
			this._layoutViewCards.LayoutControlType = typeof(Mtgdb.Gui.CardLayout);
			layoutOptions5.AllowPartialCards = true;
			layoutOptions5.CardInterval = new System.Drawing.Size(4, 2);
			layoutOptions5.PartialCardsThreshold = new System.Drawing.Size(327, 209);
			this._layoutViewCards.LayoutOptions = layoutOptions5;
			this._layoutViewCards.Location = new System.Drawing.Point(0, 58);
			this._layoutViewCards.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewCards.Name = "_layoutViewCards";
			buttonOptions5.Margin = new System.Drawing.Size(0, 0);
			searchOptions5.Button = buttonOptions5;
			this._layoutViewCards.SearchOptions = searchOptions5;
			selectionOptions5.Alpha = ((byte)(192));
			selectionOptions5.ForeColor = System.Drawing.SystemColors.HighlightText;
			selectionOptions5.RectAlpha = ((byte)(0));
			selectionOptions5.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions5.RectFillColor = System.Drawing.Color.Empty;
			this._layoutViewCards.SelectionOptions = selectionOptions5;
			this._layoutViewCards.Size = new System.Drawing.Size(1267, 163);
			sortOptions5.Allow = true;
			this._layoutViewCards.SortOptions = sortOptions5;
			this._layoutViewCards.TabIndex = 4;
			this._layoutViewCards.TabStop = false;
			// 
			// _layoutViewDeck
			// 
			this._layoutViewDeck.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutViewDeck.BackColor = System.Drawing.SystemColors.Window;
			this._layoutMain.SetColumnSpan(this._layoutViewDeck, 3);
			this._layoutViewDeck.LayoutControlType = typeof(Mtgdb.Gui.DeckLayout);
			layoutOptions6.AllowPartialCards = true;
			layoutOptions6.CardInterval = new System.Drawing.Size(2, 0);
			layoutOptions6.PartialCardsThreshold = new System.Drawing.Size(150, 209);
			this._layoutViewDeck.LayoutOptions = layoutOptions6;
			this._layoutViewDeck.Location = new System.Drawing.Point(0, 245);
			this._layoutViewDeck.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewDeck.Name = "_layoutViewDeck";
			searchOptions6.Button = buttonOptions6;
			this._layoutViewDeck.SearchOptions = searchOptions6;
			selectionOptions6.Alpha = ((byte)(255));
			selectionOptions6.Enabled = false;
			selectionOptions6.ForeColor = System.Drawing.SystemColors.HighlightText;
			selectionOptions6.RectAlpha = ((byte)(0));
			selectionOptions6.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions6.RectFillColor = System.Drawing.Color.Empty;
			this._layoutViewDeck.SelectionOptions = selectionOptions6;
			this._layoutViewDeck.Size = new System.Drawing.Size(1267, 311);
			sortOptions6.Allow = true;
			this._layoutViewDeck.SortOptions = sortOptions6;
			this._layoutViewDeck.TabIndex = 7;
			this._layoutViewDeck.TabStop = false;
			// 
			// _deckListControl
			// 
			this._deckListControl.AllowPartialCard = true;
			this._deckListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutMain.SetColumnSpan(this._deckListControl, 3);
			this._deckListControl.FilterByDeckMode = Mtgdb.Controls.FilterByDeckMode.Ignored;
			this._deckListControl.HideScroll = false;
			this._deckListControl.Location = new System.Drawing.Point(0, 556);
			this._deckListControl.Margin = new System.Windows.Forms.Padding(0);
			this._deckListControl.Name = "_deckListControl";
			this._deckListControl.Size = new System.Drawing.Size(1267, 311);
			this._deckListControl.TabIndex = 8;
			this._deckListControl.TabStop = false;
			this._deckListControl.Visible = false;
			// 
			// _menuSearchExamples
			// 
			this._menuSearchExamples.AutoSize = true;
			this._menuSearchExamples.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._menuSearchExamples.BackColor = System.Drawing.SystemColors.Window;
			this._menuSearchExamples.Font = new System.Drawing.Font("Consolas", 9F);
			this._menuSearchExamples.ForeColor = System.Drawing.SystemColors.WindowText;
			this._menuSearchExamples.Location = new System.Drawing.Point(238, 84);
			this._menuSearchExamples.Margin = new System.Windows.Forms.Padding(0, 1, 3, 3);
			this._menuSearchExamples.Name = "_menuSearchExamples";
			this._menuSearchExamples.Padding = new System.Windows.Forms.Padding(1);
			this._menuSearchExamples.Size = new System.Drawing.Size(773, 774);
			this._menuSearchExamples.TabIndex = 3;
			this._menuSearchExamples.TabStop = false;
			this._menuSearchExamples.Visible = false;
			// 
			// _panelTabsContainer
			// 
			this._panelDeckTabsContainer.Controls.Add(this._tabHeadersDeck);
			this._panelDeckTabsContainer.Location = new System.Drawing.Point(72, 0);
			this._panelDeckTabsContainer.Margin = new System.Windows.Forms.Padding(0);
			this._panelDeckTabsContainer.Name = "_panelDeckTabsContainer";
			this._panelDeckTabsContainer.Size = new System.Drawing.Size(144, 24);
			this._panelDeckTabsContainer.TabIndex = 22;
			// 
			// FormMain
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this._layoutRoot);
			this.Controls.Add(this._menuSearchExamples);
			this.Name = "FormMain";
			this.Size = new System.Drawing.Size(1379, 867);
			this._panelFilters.ResumeLayout(false);
			this._panelStatus.ResumeLayout(false);
			this._panelStatus.PerformLayout();
			this._panelMenu.ResumeLayout(false);
			this._panelMenu.PerformLayout();
			this._panelMenuRightSubpanel.ResumeLayout(false);
			this._panelMenuRightSubpanel.PerformLayout();
			this._panelRightCost.ResumeLayout(false);
			this._panelRightCost.PerformLayout();
			this._panelManaAbility.ResumeLayout(false);
			this._layoutMain.ResumeLayout(false);
			this._layoutMain.PerformLayout();
			this._layoutRight.ResumeLayout(false);
			this._layoutRight.PerformLayout();
			this._panelRightNarrow.ResumeLayout(false);
			this._panelRightManaCost.ResumeLayout(false);
			this._layoutRoot.ResumeLayout(false);
			this._layoutRoot.PerformLayout();
			this._panelDeckTabsContainer.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public Mtgdb.Controls.QuickFilterControl FilterType;
		public Mtgdb.Controls.QuickFilterControl FilterAbility;
		public Mtgdb.Controls.QuickFilterControl FilterRarity;
		public Mtgdb.Controls.QuickFilterControl FilterManager;
		public Mtgdb.Controls.QuickFilterControl FilterManaCost;
		public Mtgdb.Controls.QuickFilterControl FilterManaAbility;
		public Mtgdb.Controls.QuickFilterControl FilterCmc;
		public Mtgdb.Controls.QuickFilterControl FilterGeneratedMana;

		private System.Windows.Forms.FlowLayoutPanel _panelFilters;
		private System.Windows.Forms.FlowLayoutPanel _panelStatus;

		private Mtgdb.Controls.LayoutViewControl _layoutViewCards;
		private Mtgdb.Controls.TabHeaderControl _tabHeadersDeck;
		private Mtgdb.Controls.ButtonBase _buttonShowProhibit;
		private System.Windows.Forms.TableLayoutPanel _panelMenu;
		private Mtgdb.Controls.DropDown _dropdownLegality;
		private Mtgdb.Controls.CheckBox _buttonLegalityAllowLegal;
		private Mtgdb.Controls.CheckBox _buttonLegalityAllowRestricted;
		private Mtgdb.Controls.CheckBox _buttonLegalityAllowBanned;
		private Mtgdb.Controls.CheckBox _buttonLegalityAllowFuture;
		private Mtgdb.Controls.ButtonBase _buttonShowDuplicates;
		private Mtgdb.Controls.ButtonBase _buttonExcludeManaAbility;
		private Mtgdb.Controls.ButtonBase _buttonExcludeManaCost;

		private System.Windows.Forms.Label _labelStatusScrollDeck;
		private System.Windows.Forms.Label _labelStatusScrollCards;

		private Mtgdb.Controls.ButtonBase _buttonShowScrollDeck;
		private Mtgdb.Controls.ControlBase _labelStatusCollection;
		private Mtgdb.Controls.ControlBase _labelStatusFilterButtons;
		private Mtgdb.Controls.ControlBase _labelStatusSearch;
		private Mtgdb.Controls.ControlBase _labelStatusFilterCollection;
		private Mtgdb.Controls.ControlBase _labelStatusSets;
		private Mtgdb.Controls.ControlBase _labelStatusFilterDeck;
		private Mtgdb.Controls.ControlBase _labelStatusFilterLegality;
		private Mtgdb.Controls.ControlBase _panelIconLegality;
		private Controls.LayoutViewControl _layoutViewDeck;
		private System.Windows.Forms.FlowLayoutPanel _panelRightCost;
		private System.Windows.Forms.TableLayoutPanel _layoutMain;
		private Controls.ButtonBase _buttonSampleHandNew;
		private Controls.ButtonBase _buttonSampleHandMulligan;
		private Controls.ButtonBase _buttonSampleHandDraw;
		private Controls.ButtonBase _buttonHideDeck;
		private Controls.ButtonBase _buttonShowPartialCards;
		private Controls.ButtonBase _buttonShowText;
		private System.Windows.Forms.TableLayoutPanel _layoutRight;
		private System.Windows.Forms.TableLayoutPanel _layoutRoot;
		private Mtgdb.Controls.Popup _popupSearchExamples;
		private SearchExamplesMenu _menuSearchExamples;
		private Controls.ControlBase _labelStatusSort;
		public Controls.QuickFilterControl FilterLayout;
		public Controls.QuickFilterControl FilterCastKeyword;
		private System.Windows.Forms.FlowLayoutPanel _panelRightNarrow;
		private System.Windows.Forms.FlowLayoutPanel _panelRightManaCost;
		private System.Windows.Forms.FlowLayoutPanel _panelMenuRightSubpanel;
		private System.Windows.Forms.FlowLayoutPanel _panelManaAbility;
		private Controls.ButtonBase _buttonShowScrollCards;
		private Mtgdb.Controls.DeckListControl _deckListControl;
		private Controls.ButtonBase _buttonResetFilters;
		private Controls.SearchBar _searchBar;
		private System.Windows.Forms.Panel _panelDeckTabsContainer;
	}
}
