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
			Mtgdb.Controls.SearchOptions searchOptions1 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.SortOptions sortOptions1 = new Mtgdb.Controls.SortOptions();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			Mtgdb.Controls.SearchOptions searchOptions2 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.SortOptions sortOptions2 = new Mtgdb.Controls.SortOptions();
			this._tableRoot = new System.Windows.Forms.TableLayoutPanel();
			this._layoutViewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._panelFilters = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterAbility = new Mtgdb.Controls.QuickFilterControl();
			this.FilterType = new Mtgdb.Controls.QuickFilterControl();
			this.FilterRarity = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManager = new Mtgdb.Controls.QuickFilterControl();
			this._panelStatus = new System.Windows.Forms.FlowLayoutPanel();
			this._tabHeadersDeck = new Mtgdb.Controls.TabHeaderControl();
			this._labelStatusScrollDeck = new System.Windows.Forms.Label();
			this._panelIconStatusScrollDeck = new Mtgdb.Controls.CustomPanel();
			this._panelIconStatusSets = new Mtgdb.Controls.CustomPanel();
			this._labelStatusSets = new System.Windows.Forms.Label();
			this._labelStatusScrollCards = new System.Windows.Forms.Label();
			this._panelIconStatusScrollCards = new Mtgdb.Controls.CustomPanel();
			this._panelIconStatusCollection = new Mtgdb.Controls.CustomPanel();
			this._labelStatusCollection = new System.Windows.Forms.Label();
			this._panelIconStatusFilterButtons = new Mtgdb.Controls.CustomPanel();
			this._labelStatusFilterButtons = new System.Windows.Forms.Label();
			this._panelIconStatusSearch = new Mtgdb.Controls.CustomPanel();
			this._labelStatusSearch = new System.Windows.Forms.Label();
			this._panelIconStatusFilterCollection = new Mtgdb.Controls.CustomPanel();
			this._labelStatusFilterCollection = new System.Windows.Forms.Label();
			this._panelIconStatusFilterDeck = new Mtgdb.Controls.CustomPanel();
			this._labelStatusFilterDeck = new System.Windows.Forms.Label();
			this._panelIconStatusFilterLegality = new Mtgdb.Controls.CustomPanel();
			this._labelStatusFilterLegality = new System.Windows.Forms.Label();
			this._panelCost = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterManaAbility = new Mtgdb.Controls.QuickFilterControl();
			this.FilterGeneratedMana = new Mtgdb.Controls.QuickFilterControl();
			this.FilterCmc = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManaCost = new Mtgdb.Controls.QuickFilterControl();
			this._panelCostMode = new System.Windows.Forms.FlowLayoutPanel();
			this._buttonExcludeManaCost = new Mtgdb.Controls.CustomCheckBox();
			this._buttonExcludeManaAbility = new Mtgdb.Controls.CustomCheckBox();
			this._layoutViewCards = new Mtgdb.Controls.LayoutViewControl();
			this._buttonShowProhibit = new Mtgdb.Controls.CustomCheckBox();
			this._panelMenu = new System.Windows.Forms.FlowLayoutPanel();
			this._findCustomPanel = new Mtgdb.Controls.CustomPanel();
			this._findEditor = new Mtgdb.Controls.FixedRichTextBox();
			this._panelIconSearch = new Mtgdb.Controls.CustomPanel();
			this._panelIconLegality = new Mtgdb.Controls.CustomPanel();
			this._menuLegalityFormat = new System.Windows.Forms.ComboBox();
			this._buttonLegalityAllowLegal = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowRestricted = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowBanned = new Mtgdb.Controls.CustomCheckBox();
			this._buttonShowDuplicates = new Mtgdb.Controls.CustomCheckBox();
			this._listBoxSuggest = new System.Windows.Forms.ListBox();
			this._tableRoot.SuspendLayout();
			this._panelFilters.SuspendLayout();
			this._panelStatus.SuspendLayout();
			this._panelCost.SuspendLayout();
			this._panelCostMode.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._findCustomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableRoot
			// 
			this._tableRoot.ColumnCount = 3;
			this._tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableRoot.Controls.Add(this._layoutViewDeck, 0, 4);
			this._tableRoot.Controls.Add(this._panelFilters, 0, 0);
			this._tableRoot.Controls.Add(this._panelStatus, 0, 3);
			this._tableRoot.Controls.Add(this._panelCost, 2, 2);
			this._tableRoot.Controls.Add(this._panelCostMode, 2, 1);
			this._tableRoot.Controls.Add(this._layoutViewCards, 0, 2);
			this._tableRoot.Controls.Add(this._buttonShowProhibit, 1, 1);
			this._tableRoot.Controls.Add(this._panelMenu, 0, 1);
			this._tableRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableRoot.Location = new System.Drawing.Point(0, 0);
			this._tableRoot.Margin = new System.Windows.Forms.Padding(0);
			this._tableRoot.Name = "_tableRoot";
			this._tableRoot.RowCount = 5;
			this._tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 311F));
			this._tableRoot.Size = new System.Drawing.Size(1476, 698);
			this._tableRoot.TabIndex = 8;
			// 
			// _layoutViewDeck
			// 
			this._layoutViewDeck.AllowPartialCards = true;
			this._layoutViewDeck.BackColor = System.Drawing.Color.White;
			this._layoutViewDeck.CardInterval = new System.Drawing.Size(2, 2);
			this._tableRoot.SetColumnSpan(this._layoutViewDeck, 2);
			this._layoutViewDeck.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutViewDeck.HotTrackBackgroundColor = System.Drawing.Color.WhiteSmoke;
			this._layoutViewDeck.HotTrackBorderColor = System.Drawing.Color.Gainsboro;
			this._layoutViewDeck.LayoutControlType = typeof(Mtgdb.Gui.DeckLayout);
			this._layoutViewDeck.Location = new System.Drawing.Point(0, 387);
			this._layoutViewDeck.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewDeck.Name = "_layoutViewDeck";
			this._layoutViewDeck.PartialCardsThreshold = new System.Drawing.Size(161, 225);
			this._layoutViewDeck.SearchOptions = searchOptions1;
			this._layoutViewDeck.Size = new System.Drawing.Size(1372, 311);
			sortOptions1.Allow = true;
			sortOptions1.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc;
			sortOptions1.AscIconHovered = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions1.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc;
			sortOptions1.DescIconHovered = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions1.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none;
			sortOptions1.IconHovered = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewDeck.SortOptions = sortOptions1;
			this._layoutViewDeck.TabIndex = 42;
			this._layoutViewDeck.TabStop = false;
			// 
			// _panelFilters
			// 
			this._panelFilters.AutoSize = true;
			this._panelFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableRoot.SetColumnSpan(this._panelFilters, 3);
			this._panelFilters.Controls.Add(this.FilterAbility);
			this._panelFilters.Controls.Add(this.FilterType);
			this._panelFilters.Controls.Add(this.FilterRarity);
			this._panelFilters.Controls.Add(this.FilterManager);
			this._panelFilters.Location = new System.Drawing.Point(0, 0);
			this._panelFilters.Margin = new System.Windows.Forms.Padding(0);
			this._panelFilters.MinimumSize = new System.Drawing.Size(948, 46);
			this._panelFilters.Name = "_panelFilters";
			this._panelFilters.Size = new System.Drawing.Size(1432, 46);
			this._panelFilters.TabIndex = 0;
			// 
			// FilterAbility
			// 
			this.FilterAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterAbility.CostNeutralValues = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("FilterAbility.CostNeutralValues")));
			this.FilterAbility.EnableRequiringSome = true;
			this.FilterAbility.HideProhibit = true;
			this.FilterAbility.Location = new System.Drawing.Point(0, 0);
			this.FilterAbility.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.FilterAbility.MinimumSize = new System.Drawing.Size(948, 46);
			this.FilterAbility.Name = "FilterAbility";
			this.FilterAbility.Opacity1Enabled = 1F;
			this.FilterAbility.Opacity2ToEnable = 0.75F;
			this.FilterAbility.Opacity4Disabled = 0.2F;
			this.FilterAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterAbility.Properties = null;
			this.FilterAbility.PropertiesCount = 43;
			this.FilterAbility.PropertyImages = null;
			this.FilterAbility.SelectionBorder = 1.75F;
			this.FilterAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterAbility.ShowValueHint = true;
			this.FilterAbility.Size = new System.Drawing.Size(948, 46);
			this.FilterAbility.Spacing = new System.Drawing.Size(2, 2);
			this.FilterAbility.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterAbility.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterAbility.TabIndex = 13;
			this.FilterAbility.TabStop = false;
			// 
			// FilterType
			// 
			this.FilterType.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterType.CostNeutralValues = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("FilterType.CostNeutralValues")));
			this.FilterType.EnableCostBehavior = true;
			this.FilterType.HideProhibit = true;
			this.FilterType.Location = new System.Drawing.Point(968, 0);
			this.FilterType.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.FilterType.MinimumSize = new System.Drawing.Size(178, 46);
			this.FilterType.Name = "FilterType";
			this.FilterType.Opacity1Enabled = 1F;
			this.FilterType.Opacity2ToEnable = 0.75F;
			this.FilterType.Opacity4Disabled = 0.2F;
			this.FilterType.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterType.Properties = null;
			this.FilterType.PropertiesCount = 8;
			this.FilterType.PropertyImages = null;
			this.FilterType.SelectionBorder = 1.75F;
			this.FilterType.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterType.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterType.ShowValueHint = true;
			this.FilterType.Size = new System.Drawing.Size(178, 46);
			this.FilterType.Spacing = new System.Drawing.Size(2, 2);
			this.FilterType.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterType.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterType.TabIndex = 14;
			this.FilterType.TabStop = false;
			// 
			// FilterRarity
			// 
			this.FilterRarity.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterRarity.CostNeutralValues = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("FilterRarity.CostNeutralValues")));
			this.FilterRarity.EnableCostBehavior = true;
			this.FilterRarity.EnableMutuallyExclusive = true;
			this.FilterRarity.HideProhibit = true;
			this.FilterRarity.Location = new System.Drawing.Point(1166, 0);
			this.FilterRarity.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.FilterRarity.MinimumSize = new System.Drawing.Size(134, 46);
			this.FilterRarity.Name = "FilterRarity";
			this.FilterRarity.Opacity1Enabled = 1F;
			this.FilterRarity.Opacity2ToEnable = 0.75F;
			this.FilterRarity.Opacity4Disabled = 0.1F;
			this.FilterRarity.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterRarity.Properties = null;
			this.FilterRarity.PropertiesCount = 6;
			this.FilterRarity.PropertyImages = null;
			this.FilterRarity.SelectionBorder = 1.75F;
			this.FilterRarity.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterRarity.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterRarity.ShowValueHint = true;
			this.FilterRarity.Size = new System.Drawing.Size(134, 46);
			this.FilterRarity.Spacing = new System.Drawing.Size(2, 2);
			this.FilterRarity.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterRarity.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterRarity.TabIndex = 15;
			this.FilterRarity.TabStop = false;
			// 
			// FilterManager
			// 
			this.FilterManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FilterManager.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManager.CostNeutralValues = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("FilterManager.CostNeutralValues")));
			this.FilterManager.EnableRequiringSome = true;
			this.FilterManager.HideProhibit = true;
			this.FilterManager.Location = new System.Drawing.Point(1320, 0);
			this.FilterManager.Margin = new System.Windows.Forms.Padding(0);
			this.FilterManager.MinimumSize = new System.Drawing.Size(112, 46);
			this.FilterManager.Name = "FilterManager";
			this.FilterManager.Opacity1Enabled = 1F;
			this.FilterManager.Opacity2ToEnable = 0.65F;
			this.FilterManager.Opacity4Disabled = 0.2F;
			this.FilterManager.ProhibitedColor = System.Drawing.Color.Tomato;
			this.FilterManager.Properties = null;
			this.FilterManager.PropertyImages = null;
			this.FilterManager.SelectionBorder = 2F;
			this.FilterManager.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManager.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManager.ShowValueHint = true;
			this.FilterManager.Size = new System.Drawing.Size(112, 46);
			this.FilterManager.Spacing = new System.Drawing.Size(2, 2);
			this.FilterManager.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterManager.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterManager.TabIndex = 16;
			this.FilterManager.TabStop = false;
			// 
			// _panelStatus
			// 
			this._tableRoot.SetColumnSpan(this._panelStatus, 2);
			this._panelStatus.Controls.Add(this._tabHeadersDeck);
			this._panelStatus.Controls.Add(this._labelStatusScrollDeck);
			this._panelStatus.Controls.Add(this._panelIconStatusScrollDeck);
			this._panelStatus.Controls.Add(this._panelIconStatusSets);
			this._panelStatus.Controls.Add(this._labelStatusSets);
			this._panelStatus.Controls.Add(this._labelStatusScrollCards);
			this._panelStatus.Controls.Add(this._panelIconStatusScrollCards);
			this._panelStatus.Controls.Add(this._panelIconStatusCollection);
			this._panelStatus.Controls.Add(this._labelStatusCollection);
			this._panelStatus.Controls.Add(this._panelIconStatusFilterButtons);
			this._panelStatus.Controls.Add(this._labelStatusFilterButtons);
			this._panelStatus.Controls.Add(this._panelIconStatusSearch);
			this._panelStatus.Controls.Add(this._labelStatusSearch);
			this._panelStatus.Controls.Add(this._panelIconStatusFilterCollection);
			this._panelStatus.Controls.Add(this._labelStatusFilterCollection);
			this._panelStatus.Controls.Add(this._panelIconStatusFilterDeck);
			this._panelStatus.Controls.Add(this._labelStatusFilterDeck);
			this._panelStatus.Controls.Add(this._panelIconStatusFilterLegality);
			this._panelStatus.Controls.Add(this._labelStatusFilterLegality);
			this._panelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
			this._panelStatus.Location = new System.Drawing.Point(0, 363);
			this._panelStatus.Margin = new System.Windows.Forms.Padding(0);
			this._panelStatus.Name = "_panelStatus";
			this._panelStatus.Size = new System.Drawing.Size(1372, 24);
			this._panelStatus.TabIndex = 14;
			// 
			// _tabHeadersDeck
			// 
			this._tabHeadersDeck.AddButtonWidth = 24;
			this._tabHeadersDeck.AllowAddingTabs = false;
			this._tabHeadersDeck.AllowRemovingTabs = false;
			this._tabHeadersDeck.AllowReorderTabs = false;
			this._tabHeadersDeck.Count = 2;
			this._tabHeadersDeck.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._tabHeadersDeck.Location = new System.Drawing.Point(0, 0);
			this._tabHeadersDeck.Margin = new System.Windows.Forms.Padding(0);
			this._tabHeadersDeck.Name = "_tabHeadersDeck";
			this._tabHeadersDeck.SelectedIndex = 1;
			this._tabHeadersDeck.Size = new System.Drawing.Size(147, 24);
			this._tabHeadersDeck.TabIndex = 29;
			this._tabHeadersDeck.TabStop = false;
			// 
			// _labelStatusScrollDeck
			// 
			this._labelStatusScrollDeck.AutoSize = true;
			this._labelStatusScrollDeck.Location = new System.Drawing.Point(147, 6);
			this._labelStatusScrollDeck.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusScrollDeck.Name = "_labelStatusScrollDeck";
			this._labelStatusScrollDeck.Size = new System.Drawing.Size(36, 13);
			this._labelStatusScrollDeck.TabIndex = 35;
			this._labelStatusScrollDeck.Text = "63/60";
			// 
			// _panelIconStatusScrollDeck
			// 
			this._panelIconStatusScrollDeck.BackgroundImage = global::Mtgdb.Gui.Resx.Resources.Stretch20bw;
			this._panelIconStatusScrollDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusScrollDeck.Location = new System.Drawing.Point(183, 0);
			this._panelIconStatusScrollDeck.Margin = new System.Windows.Forms.Padding(0);
			this._panelIconStatusScrollDeck.Name = "_panelIconStatusScrollDeck";
			this._panelIconStatusScrollDeck.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusScrollDeck.TabIndex = 31;
			this._panelIconStatusScrollDeck.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _panelIconStatusSets
			// 
			this._panelIconStatusSets.BackgroundImage = global::Mtgdb.Gui.Resx.Resources.mtg20;
			this._panelIconStatusSets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusSets.Location = new System.Drawing.Point(231, 0);
			this._panelIconStatusSets.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSets.Name = "_panelIconStatusSets";
			this._panelIconStatusSets.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSets.TabIndex = 34;
			this._panelIconStatusSets.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusSets
			// 
			this._labelStatusSets.AutoSize = true;
			this._labelStatusSets.Location = new System.Drawing.Point(255, 6);
			this._labelStatusSets.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSets.Name = "_labelStatusSets";
			this._labelStatusSets.Size = new System.Drawing.Size(25, 13);
			this._labelStatusSets.TabIndex = 36;
			this._labelStatusSets.Text = "206";
			// 
			// _labelStatusScrollCards
			// 
			this._labelStatusScrollCards.AutoSize = true;
			this._labelStatusScrollCards.Location = new System.Drawing.Point(304, 6);
			this._labelStatusScrollCards.Margin = new System.Windows.Forms.Padding(24, 6, 0, 0);
			this._labelStatusScrollCards.Name = "_labelStatusScrollCards";
			this._labelStatusScrollCards.Size = new System.Drawing.Size(72, 13);
			this._labelStatusScrollCards.TabIndex = 37;
			this._labelStatusScrollCards.Text = "15999/16001";
			// 
			// _panelIconStatusScrollCards
			// 
			this._panelIconStatusScrollCards.BackgroundImage = global::Mtgdb.Gui.Resx.Resources.Stretch20bw;
			this._panelIconStatusScrollCards.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusScrollCards.Location = new System.Drawing.Point(376, 0);
			this._panelIconStatusScrollCards.Margin = new System.Windows.Forms.Padding(0);
			this._panelIconStatusScrollCards.Name = "_panelIconStatusScrollCards";
			this._panelIconStatusScrollCards.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusScrollCards.TabIndex = 30;
			this._panelIconStatusScrollCards.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _panelIconStatusCollection
			// 
			this._panelIconStatusCollection.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.Box20;
			this._panelIconStatusCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusCollection.Location = new System.Drawing.Point(424, 0);
			this._panelIconStatusCollection.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusCollection.Name = "_panelIconStatusCollection";
			this._panelIconStatusCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusCollection.TabIndex = 31;
			this._panelIconStatusCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusCollection
			// 
			this._labelStatusCollection.AutoSize = true;
			this._labelStatusCollection.Location = new System.Drawing.Point(448, 6);
			this._labelStatusCollection.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusCollection.Name = "_labelStatusCollection";
			this._labelStatusCollection.Size = new System.Drawing.Size(25, 13);
			this._labelStatusCollection.TabIndex = 38;
			this._labelStatusCollection.Text = "691";
			// 
			// _panelIconStatusFilterButtons
			// 
			this._panelIconStatusFilterButtons.BackgroundImage = global::Mtgdb.Gui.Resx.ResourcesFilter.Quick_filters;
			this._panelIconStatusFilterButtons.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterButtons.Location = new System.Drawing.Point(497, 0);
			this._panelIconStatusFilterButtons.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterButtons.Name = "_panelIconStatusFilterButtons";
			this._panelIconStatusFilterButtons.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterButtons.TabIndex = 32;
			this._panelIconStatusFilterButtons.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterButtons
			// 
			this._labelStatusFilterButtons.AutoSize = true;
			this._labelStatusFilterButtons.Location = new System.Drawing.Point(521, 6);
			this._labelStatusFilterButtons.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterButtons.Name = "_labelStatusFilterButtons";
			this._labelStatusFilterButtons.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterButtons.TabIndex = 39;
			this._labelStatusFilterButtons.Text = "ignored";
			// 
			// _panelIconStatusSearch
			// 
			this._panelIconStatusSearch.BackgroundImage = global::Mtgdb.Gui.Resx.ResourcesFilter.Search_icon;
			this._panelIconStatusSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusSearch.Location = new System.Drawing.Point(587, 0);
			this._panelIconStatusSearch.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSearch.Name = "_panelIconStatusSearch";
			this._panelIconStatusSearch.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSearch.TabIndex = 33;
			this._panelIconStatusSearch.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusSearch
			// 
			this._labelStatusSearch.AutoSize = true;
			this._labelStatusSearch.Location = new System.Drawing.Point(611, 6);
			this._labelStatusSearch.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSearch.Name = "_labelStatusSearch";
			this._labelStatusSearch.Size = new System.Drawing.Size(145, 13);
			this._labelStatusSearch.TabIndex = 41;
			this._labelStatusSearch.Text = "modified, press Enter to apply";
			// 
			// _panelIconStatusFilterCollection
			// 
			this._panelIconStatusFilterCollection.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.Box20;
			this._panelIconStatusFilterCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterCollection.Location = new System.Drawing.Point(780, 0);
			this._panelIconStatusFilterCollection.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterCollection.Name = "_panelIconStatusFilterCollection";
			this._panelIconStatusFilterCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterCollection.TabIndex = 32;
			this._panelIconStatusFilterCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterCollection
			// 
			this._labelStatusFilterCollection.AutoSize = true;
			this._labelStatusFilterCollection.Location = new System.Drawing.Point(804, 6);
			this._labelStatusFilterCollection.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterCollection.Name = "_labelStatusFilterCollection";
			this._labelStatusFilterCollection.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterCollection.TabIndex = 40;
			this._labelStatusFilterCollection.Text = "ignored";
			// 
			// _panelIconStatusFilterDeck
			// 
			this._panelIconStatusFilterDeck.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.draw_a_card;
			this._panelIconStatusFilterDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterDeck.Location = new System.Drawing.Point(870, 0);
			this._panelIconStatusFilterDeck.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterDeck.Name = "_panelIconStatusFilterDeck";
			this._panelIconStatusFilterDeck.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterDeck.TabIndex = 33;
			this._panelIconStatusFilterDeck.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterDeck
			// 
			this._labelStatusFilterDeck.AutoSize = true;
			this._labelStatusFilterDeck.Location = new System.Drawing.Point(894, 6);
			this._labelStatusFilterDeck.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterDeck.Name = "_labelStatusFilterDeck";
			this._labelStatusFilterDeck.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterDeck.TabIndex = 42;
			this._labelStatusFilterDeck.Text = "ignored";
			// 
			// _panelIconStatusFilterLegality
			// 
			this._panelIconStatusFilterLegality.BackgroundImage = global::Mtgdb.Gui.Resx.Resources.Legality20;
			this._panelIconStatusFilterLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterLegality.Location = new System.Drawing.Point(960, 0);
			this._panelIconStatusFilterLegality.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterLegality.Name = "_panelIconStatusFilterLegality";
			this._panelIconStatusFilterLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterLegality.TabIndex = 34;
			this._panelIconStatusFilterLegality.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterLegality
			// 
			this._labelStatusFilterLegality.AutoSize = true;
			this._labelStatusFilterLegality.Location = new System.Drawing.Point(984, 6);
			this._labelStatusFilterLegality.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterLegality.Name = "_labelStatusFilterLegality";
			this._labelStatusFilterLegality.Size = new System.Drawing.Size(196, 13);
			this._labelStatusFilterLegality.TabIndex = 43;
			this._labelStatusFilterLegality.Text = "and Standard +legal +restricted -banned";
			// 
			// _panelCost
			// 
			this._panelCost.Controls.Add(this.FilterManaAbility);
			this._panelCost.Controls.Add(this.FilterGeneratedMana);
			this._panelCost.Controls.Add(this.FilterCmc);
			this._panelCost.Controls.Add(this.FilterManaCost);
			this._panelCost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelCost.Location = new System.Drawing.Point(1372, 74);
			this._panelCost.Margin = new System.Windows.Forms.Padding(0);
			this._panelCost.MinimumSize = new System.Drawing.Size(104, 618);
			this._panelCost.Name = "_panelCost";
			this._tableRoot.SetRowSpan(this._panelCost, 3);
			this._panelCost.Size = new System.Drawing.Size(104, 618);
			this._panelCost.TabIndex = 15;
			// 
			// FilterManaAbility
			// 
			this.FilterManaAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManaAbility.CostNeutralValues = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("FilterManaAbility.CostNeutralValues")));
			this.FilterManaAbility.EnableRequiringSome = true;
			this.FilterManaAbility.HideProhibit = true;
			this.FilterManaAbility.HintIcon = global::Mtgdb.Gui.Resx.Resources.manatext25i;
			this.FilterManaAbility.HintTextShift = new System.Drawing.Size(-4, -4);
			this.FilterManaAbility.IsFlipped = true;
			this.FilterManaAbility.IsVertical = true;
			this.FilterManaAbility.Location = new System.Drawing.Point(0, 0);
			this.FilterManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
			this.FilterManaAbility.MinimumSize = new System.Drawing.Size(46, 244);
			this.FilterManaAbility.Name = "FilterManaAbility";
			this.FilterManaAbility.Opacity1Enabled = 1F;
			this.FilterManaAbility.Opacity2ToEnable = 0.75F;
			this.FilterManaAbility.Opacity4Disabled = 0.2F;
			this.FilterManaAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaAbility.Properties = null;
			this.FilterManaAbility.PropertiesCount = 11;
			this.FilterManaAbility.PropertyImages = null;
			this.FilterManaAbility.SelectionBorder = 1.75F;
			this.FilterManaAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaAbility.ShowValueHint = true;
			this.FilterManaAbility.Size = new System.Drawing.Size(46, 244);
			this.FilterManaAbility.Spacing = new System.Drawing.Size(2, 2);
			this.FilterManaAbility.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterManaAbility.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterManaAbility.TabIndex = 19;
			this.FilterManaAbility.TabStop = false;
			// 
			// FilterGeneratedMana
			// 
			this.FilterGeneratedMana.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterGeneratedMana.CostNeutralValues = null;
			this.FilterGeneratedMana.EnableRequiringSome = true;
			this.FilterGeneratedMana.HideProhibit = true;
			this.FilterGeneratedMana.IsFlipped = true;
			this.FilterGeneratedMana.IsVertical = true;
			this.FilterGeneratedMana.Location = new System.Drawing.Point(0, 264);
			this.FilterGeneratedMana.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
			this.FilterGeneratedMana.MinimumSize = new System.Drawing.Size(46, 156);
			this.FilterGeneratedMana.Name = "FilterGeneratedMana";
			this.FilterGeneratedMana.Opacity1Enabled = 1F;
			this.FilterGeneratedMana.Opacity2ToEnable = 0.75F;
			this.FilterGeneratedMana.Opacity4Disabled = 0.2F;
			this.FilterGeneratedMana.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterGeneratedMana.Properties = null;
			this.FilterGeneratedMana.PropertiesCount = 7;
			this.FilterGeneratedMana.PropertyImages = null;
			this.FilterGeneratedMana.SelectionBorder = 1.75F;
			this.FilterGeneratedMana.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterGeneratedMana.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterGeneratedMana.ShowValueHint = true;
			this.FilterGeneratedMana.Size = new System.Drawing.Size(46, 156);
			this.FilterGeneratedMana.Spacing = new System.Drawing.Size(2, 2);
			this.FilterGeneratedMana.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterGeneratedMana.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterGeneratedMana.TabIndex = 20;
			this.FilterGeneratedMana.TabStop = false;
			// 
			// FilterCmc
			// 
			this.FilterCmc.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterCmc.CostNeutralValues = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("FilterCmc.CostNeutralValues")));
			this.FilterCmc.EnableCostBehavior = true;
			this.FilterCmc.EnableMutuallyExclusive = true;
			this.FilterCmc.HideProhibit = true;
			this.FilterCmc.IsFlipped = true;
			this.FilterCmc.IsVertical = true;
			this.FilterCmc.Location = new System.Drawing.Point(0, 440);
			this.FilterCmc.Margin = new System.Windows.Forms.Padding(0);
			this.FilterCmc.MinimumSize = new System.Drawing.Size(46, 178);
			this.FilterCmc.Name = "FilterCmc";
			this.FilterCmc.Opacity1Enabled = 1F;
			this.FilterCmc.Opacity2ToEnable = 0.75F;
			this.FilterCmc.Opacity4Disabled = 0.1F;
			this.FilterCmc.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterCmc.Properties = null;
			this.FilterCmc.PropertiesCount = 8;
			this.FilterCmc.PropertyImages = null;
			this.FilterCmc.SelectionBorder = 1.75F;
			this.FilterCmc.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterCmc.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterCmc.Size = new System.Drawing.Size(46, 178);
			this.FilterCmc.Spacing = new System.Drawing.Size(2, 2);
			this.FilterCmc.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterCmc.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterCmc.TabIndex = 21;
			this.FilterCmc.TabStop = false;
			// 
			// FilterManaCost
			// 
			this.FilterManaCost.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManaCost.CostNeutralValues = ((System.Collections.Generic.HashSet<string>)(resources.GetObject("FilterManaCost.CostNeutralValues")));
			this.FilterManaCost.EnableCostBehavior = true;
			this.FilterManaCost.HideProhibit = true;
			this.FilterManaCost.HintIcon = global::Mtgdb.Gui.Resx.Resources.manacost24ctc;
			this.FilterManaCost.HintTextShift = new System.Drawing.Size(4, 10);
			this.FilterManaCost.IsFlipped = true;
			this.FilterManaCost.IsVertical = true;
			this.FilterManaCost.Location = new System.Drawing.Point(58, 0);
			this.FilterManaCost.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this.FilterManaCost.MinimumSize = new System.Drawing.Size(46, 618);
			this.FilterManaCost.Name = "FilterManaCost";
			this.FilterManaCost.Opacity1Enabled = 1F;
			this.FilterManaCost.Opacity2ToEnable = 0.75F;
			this.FilterManaCost.Opacity4Disabled = 0.1F;
			this.FilterManaCost.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaCost.Properties = null;
			this.FilterManaCost.PropertiesCount = 28;
			this.FilterManaCost.PropertyImages = null;
			this.FilterManaCost.SelectionBorder = 1.75F;
			this.FilterManaCost.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaCost.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaCost.ShowValueHint = true;
			this.FilterManaCost.Size = new System.Drawing.Size(46, 618);
			this.FilterManaCost.Spacing = new System.Drawing.Size(2, 2);
			this.FilterManaCost.States = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterManaCost.StatesDefault = new Mtgdb.Controls.FilterValueState[] {
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored,
        Mtgdb.Controls.FilterValueState.Ignored};
			this.FilterManaCost.TabIndex = 22;
			this.FilterManaCost.TabStop = false;
			// 
			// _panelCostMode
			// 
			this._panelCostMode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelCostMode.Controls.Add(this._buttonExcludeManaCost);
			this._panelCostMode.Controls.Add(this._buttonExcludeManaAbility);
			this._panelCostMode.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this._panelCostMode.Location = new System.Drawing.Point(1372, 46);
			this._panelCostMode.Margin = new System.Windows.Forms.Padding(0);
			this._panelCostMode.MinimumSize = new System.Drawing.Size(104, 24);
			this._panelCostMode.Name = "_panelCostMode";
			this._panelCostMode.Size = new System.Drawing.Size(104, 28);
			this._panelCostMode.TabIndex = 18;
			// 
			// _buttonExcludeManaCost
			// 
			this._buttonExcludeManaCost.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonExcludeManaCost.BackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.Checked = true;
			this._buttonExcludeManaCost.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonExcludeManaCost.FlatAppearance.BorderSize = 0;
			this._buttonExcludeManaCost.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExcludeManaCost.Image = ((System.Drawing.Image)(resources.GetObject("_buttonExcludeManaCost.Image")));
			this._buttonExcludeManaCost.Location = new System.Drawing.Point(58, 0);
			this._buttonExcludeManaCost.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaCost.Name = "_buttonExcludeManaCost";
			this._buttonExcludeManaCost.Size = new System.Drawing.Size(24, 24);
			this._buttonExcludeManaCost.TabIndex = 42;
			this._buttonExcludeManaCost.TabStop = false;
			this._buttonExcludeManaCost.UseVisualStyleBackColor = false;
			// 
			// _buttonExcludeManaAbility
			// 
			this._buttonExcludeManaAbility.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonExcludeManaAbility.BackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatAppearance.BorderSize = 0;
			this._buttonExcludeManaAbility.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExcludeManaAbility.Image = ((System.Drawing.Image)(resources.GetObject("_buttonExcludeManaAbility.Image")));
			this._buttonExcludeManaAbility.Location = new System.Drawing.Point(0, 0);
			this._buttonExcludeManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 34, 0);
			this._buttonExcludeManaAbility.Name = "_buttonExcludeManaAbility";
			this._buttonExcludeManaAbility.Size = new System.Drawing.Size(24, 24);
			this._buttonExcludeManaAbility.TabIndex = 41;
			this._buttonExcludeManaAbility.TabStop = false;
			this._buttonExcludeManaAbility.UseVisualStyleBackColor = false;
			// 
			// _layoutViewCards
			// 
			this._layoutViewCards.AllowPartialCards = true;
			this._layoutViewCards.BackColor = System.Drawing.Color.White;
			this._layoutViewCards.CardInterval = new System.Drawing.Size(2, 2);
			this._tableRoot.SetColumnSpan(this._layoutViewCards, 2);
			this._layoutViewCards.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutViewCards.HotTrackBackgroundColor = System.Drawing.Color.WhiteSmoke;
			this._layoutViewCards.HotTrackBorderColor = System.Drawing.Color.Gainsboro;
			this._layoutViewCards.LayoutControlType = typeof(Mtgdb.Gui.CardLayout);
			this._layoutViewCards.Location = new System.Drawing.Point(0, 74);
			this._layoutViewCards.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewCards.Name = "_layoutViewCards";
			this._layoutViewCards.PartialCardsThreshold = new System.Drawing.Size(161, 225);
			searchOptions2.Allow = true;
			searchOptions2.ButtonMargin = new System.Drawing.Size(19, 2);
			searchOptions2.Icon = global::Mtgdb.Gui.Properties.Resources.Search_transp_16;
			searchOptions2.IconHovered = global::Mtgdb.Gui.Properties.Resources.Search_16;
			this._layoutViewCards.SearchOptions = searchOptions2;
			this._layoutViewCards.Size = new System.Drawing.Size(1372, 289);
			sortOptions2.Allow = true;
			sortOptions2.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc;
			sortOptions2.AscIconHovered = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions2.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc;
			sortOptions2.DescIconHovered = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions2.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none;
			sortOptions2.IconHovered = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewCards.SortOptions = sortOptions2;
			this._layoutViewCards.TabIndex = 19;
			this._layoutViewCards.TabStop = false;
			// 
			// _buttonShowProhibit
			// 
			this._buttonShowProhibit.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowProhibit.BackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatAppearance.BorderSize = 0;
			this._buttonShowProhibit.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowProhibit.Image = ((System.Drawing.Image)(resources.GetObject("_buttonShowProhibit.Image")));
			this._buttonShowProhibit.Location = new System.Drawing.Point(1348, 46);
			this._buttonShowProhibit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowProhibit.Name = "_buttonShowProhibit";
			this._buttonShowProhibit.Size = new System.Drawing.Size(24, 24);
			this._buttonShowProhibit.TabIndex = 41;
			this._buttonShowProhibit.TabStop = false;
			this._buttonShowProhibit.UseVisualStyleBackColor = false;
			// 
			// _panelMenu
			// 
			this._panelMenu.AutoSize = true;
			this._panelMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenu.Controls.Add(this._findCustomPanel);
			this._panelMenu.Controls.Add(this._panelIconLegality);
			this._panelMenu.Controls.Add(this._menuLegalityFormat);
			this._panelMenu.Controls.Add(this._buttonLegalityAllowLegal);
			this._panelMenu.Controls.Add(this._buttonLegalityAllowRestricted);
			this._panelMenu.Controls.Add(this._buttonLegalityAllowBanned);
			this._panelMenu.Controls.Add(this._buttonShowDuplicates);
			this._panelMenu.Location = new System.Drawing.Point(0, 46);
			this._panelMenu.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(829, 24);
			this._panelMenu.TabIndex = 10;
			// 
			// _findCustomPanel
			// 
			this._findCustomPanel.BackColor = System.Drawing.Color.White;
			this._findCustomPanel.Controls.Add(this._findEditor);
			this._findCustomPanel.Controls.Add(this._panelIconSearch);
			this._findCustomPanel.Location = new System.Drawing.Point(0, 0);
			this._findCustomPanel.Margin = new System.Windows.Forms.Padding(0);
			this._findCustomPanel.Name = "_findCustomPanel";
			this._findCustomPanel.Size = new System.Drawing.Size(486, 24);
			this._findCustomPanel.TabIndex = 21;
			this._findCustomPanel.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _findEditor
			// 
			this._findEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._findEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._findEditor.DetectUrls = false;
			this._findEditor.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._findEditor.Location = new System.Drawing.Point(24, 5);
			this._findEditor.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this._findEditor.Multiline = false;
			this._findEditor.Name = "_findEditor";
			this._findEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this._findEditor.Size = new System.Drawing.Size(461, 18);
			this._findEditor.TabIndex = 20;
			this._findEditor.TabStop = false;
			this._findEditor.Text = "";
			this._findEditor.WordWrap = false;
			// 
			// _panelIconSearch
			// 
			this._panelIconSearch.BackgroundImage = global::Mtgdb.Gui.Resx.ResourcesFilter.Search_icon;
			this._panelIconSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconSearch.Location = new System.Drawing.Point(1, 1);
			this._panelIconSearch.Margin = new System.Windows.Forms.Padding(1);
			this._panelIconSearch.Name = "_panelIconSearch";
			this._panelIconSearch.Size = new System.Drawing.Size(22, 22);
			this._panelIconSearch.TabIndex = 21;
			this._panelIconSearch.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _panelIconLegality
			// 
			this._panelIconLegality.BackgroundImage = global::Mtgdb.Gui.Resx.Resources.Legality20;
			this._panelIconLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconLegality.Location = new System.Drawing.Point(486, 0);
			this._panelIconLegality.Margin = new System.Windows.Forms.Padding(0);
			this._panelIconLegality.Name = "_panelIconLegality";
			this._panelIconLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconLegality.TabIndex = 35;
			this._panelIconLegality.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _menuLegalityFormat
			// 
			this._menuLegalityFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuLegalityFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuLegalityFormat.Items.AddRange(new object[] {
            "[ any format ]",
            "Commander",
            "Freeform",
            "Legacy",
            "Modern",
            "Prismatic",
            "Singleton 100",
            "Standard",
            "Un-Sets",
            "Tribal Wars Legacy",
            "Vintage",
            "Battle for Zendikar Block",
            "Ice Age Block",
            "Innistrad Block",
            "Invasion Block",
            "Kaladesh Block",
            "Kamigawa Block",
            "Khans of Tarkir Block",
            "Lorwyn-Shadowmoor Block",
            "Masques Block",
            "Mirage Block",
            "Mirrodin Block",
            "Odyssey Block",
            "Onslaught Block",
            "Ravnica Block",
            "Return to Ravnica Block",
            "Scars of Mirrodin Block",
            "Shadows over Innistrad Block",
            "Shards of Alara Block",
            "Tempest Block",
            "Theros Block",
            "Time Spiral Block",
            "Urza Block",
            "Zendikar Block"});
			this._menuLegalityFormat.Location = new System.Drawing.Point(510, 2);
			this._menuLegalityFormat.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuLegalityFormat.MaxDropDownItems = 34;
			this._menuLegalityFormat.Name = "_menuLegalityFormat";
			this._menuLegalityFormat.Size = new System.Drawing.Size(121, 21);
			this._menuLegalityFormat.TabIndex = 39;
			this._menuLegalityFormat.TabStop = false;
			// 
			// _buttonLegalityAllowLegal
			// 
			this._buttonLegalityAllowLegal.AutoSize = true;
			this._buttonLegalityAllowLegal.Checked = true;
			this._buttonLegalityAllowLegal.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonLegalityAllowLegal.Enabled = false;
			this._buttonLegalityAllowLegal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLegalityAllowLegal.Location = new System.Drawing.Point(635, 4);
			this._buttonLegalityAllowLegal.Margin = new System.Windows.Forms.Padding(4, 4, 0, 0);
			this._buttonLegalityAllowLegal.Name = "_buttonLegalityAllowLegal";
			this._buttonLegalityAllowLegal.Size = new System.Drawing.Size(45, 17);
			this._buttonLegalityAllowLegal.TabIndex = 36;
			this._buttonLegalityAllowLegal.TabStop = false;
			this._buttonLegalityAllowLegal.Text = "legal";
			this._buttonLegalityAllowLegal.UseVisualStyleBackColor = true;
			// 
			// _buttonLegalityAllowRestricted
			// 
			this._buttonLegalityAllowRestricted.AutoSize = true;
			this._buttonLegalityAllowRestricted.Checked = true;
			this._buttonLegalityAllowRestricted.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonLegalityAllowRestricted.Enabled = false;
			this._buttonLegalityAllowRestricted.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLegalityAllowRestricted.Location = new System.Drawing.Point(680, 4);
			this._buttonLegalityAllowRestricted.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonLegalityAllowRestricted.Name = "_buttonLegalityAllowRestricted";
			this._buttonLegalityAllowRestricted.Size = new System.Drawing.Size(66, 17);
			this._buttonLegalityAllowRestricted.TabIndex = 37;
			this._buttonLegalityAllowRestricted.TabStop = false;
			this._buttonLegalityAllowRestricted.Text = "restricted";
			this._buttonLegalityAllowRestricted.UseVisualStyleBackColor = true;
			// 
			// _buttonLegalityAllowBanned
			// 
			this._buttonLegalityAllowBanned.AutoSize = true;
			this._buttonLegalityAllowBanned.Enabled = false;
			this._buttonLegalityAllowBanned.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLegalityAllowBanned.Location = new System.Drawing.Point(746, 4);
			this._buttonLegalityAllowBanned.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonLegalityAllowBanned.Name = "_buttonLegalityAllowBanned";
			this._buttonLegalityAllowBanned.Size = new System.Drawing.Size(59, 17);
			this._buttonLegalityAllowBanned.TabIndex = 38;
			this._buttonLegalityAllowBanned.TabStop = false;
			this._buttonLegalityAllowBanned.Text = "banned";
			this._buttonLegalityAllowBanned.UseVisualStyleBackColor = true;
			// 
			// _buttonShowDuplicates
			// 
			this._buttonShowDuplicates.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowDuplicates.BackColor = System.Drawing.Color.Transparent;
			this._buttonShowDuplicates.FlatAppearance.BorderSize = 0;
			this._buttonShowDuplicates.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonShowDuplicates.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonShowDuplicates.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonShowDuplicates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowDuplicates.Image = ((System.Drawing.Image)(resources.GetObject("_buttonShowDuplicates.Image")));
			this._buttonShowDuplicates.Location = new System.Drawing.Point(805, 0);
			this._buttonShowDuplicates.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowDuplicates.Name = "_buttonShowDuplicates";
			this._buttonShowDuplicates.Size = new System.Drawing.Size(24, 24);
			this._buttonShowDuplicates.TabIndex = 40;
			this._buttonShowDuplicates.TabStop = false;
			this._buttonShowDuplicates.UseVisualStyleBackColor = false;
			// 
			// _listBoxSuggest
			// 
			this._listBoxSuggest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._listBoxSuggest.FormattingEnabled = true;
			this._listBoxSuggest.Location = new System.Drawing.Point(3, 72);
			this._listBoxSuggest.Name = "_listBoxSuggest";
			this._listBoxSuggest.Size = new System.Drawing.Size(192, 54);
			this._listBoxSuggest.TabIndex = 9;
			this._listBoxSuggest.TabStop = false;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1476, 698);
			this.Controls.Add(this._listBoxSuggest);
			this.Controls.Add(this._tableRoot);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this._tableRoot.ResumeLayout(false);
			this._tableRoot.PerformLayout();
			this._panelFilters.ResumeLayout(false);
			this._panelStatus.ResumeLayout(false);
			this._panelStatus.PerformLayout();
			this._panelCost.ResumeLayout(false);
			this._panelCostMode.ResumeLayout(false);
			this._panelMenu.ResumeLayout(false);
			this._panelMenu.PerformLayout();
			this._findCustomPanel.ResumeLayout(false);
			this.ResumeLayout(false);

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

		private System.Windows.Forms.TableLayoutPanel _tableRoot;

		private System.Windows.Forms.FlowLayoutPanel _panelFilters;
		private System.Windows.Forms.FlowLayoutPanel _panelStatus;
		private System.Windows.Forms.FlowLayoutPanel _panelCost;
		private System.Windows.Forms.FlowLayoutPanel _panelCostMode;
		
		private Mtgdb.Controls.LayoutViewControl _layoutViewCards;
		private Mtgdb.Controls.TabHeaderControl _tabHeadersDeck;
		
		private System.Windows.Forms.ListBox _listBoxSuggest;
		private Mtgdb.Controls.CustomCheckBox _buttonShowProhibit;
		private System.Windows.Forms.FlowLayoutPanel _panelMenu;
		private Mtgdb.Controls.FixedRichTextBox _findEditor;
		private System.Windows.Forms.ComboBox _menuLegalityFormat;
		private Mtgdb.Controls.CustomCheckBox _buttonLegalityAllowLegal;
		private Mtgdb.Controls.CustomCheckBox _buttonLegalityAllowRestricted;
		private Mtgdb.Controls.CustomCheckBox _buttonLegalityAllowBanned;
		private Mtgdb.Controls.CustomCheckBox _buttonShowDuplicates;
		private Mtgdb.Controls.CustomCheckBox _buttonExcludeManaAbility;
		private Mtgdb.Controls.CustomCheckBox _buttonExcludeManaCost;

		private System.Windows.Forms.Label _labelStatusScrollDeck;
		private System.Windows.Forms.Label _labelStatusSets;
		private System.Windows.Forms.Label _labelStatusScrollCards;
		private System.Windows.Forms.Label _labelStatusCollection;
		private System.Windows.Forms.Label _labelStatusFilterButtons;
		private System.Windows.Forms.Label _labelStatusFilterCollection;
		private System.Windows.Forms.Label _labelStatusSearch;
		private System.Windows.Forms.Label _labelStatusFilterDeck;
		private System.Windows.Forms.Label _labelStatusFilterLegality;

		private Mtgdb.Controls.CustomPanel _panelIconStatusScrollDeck;
		private Mtgdb.Controls.CustomPanel _panelIconStatusScrollCards;
		private Mtgdb.Controls.CustomPanel _panelIconStatusCollection;
		private Mtgdb.Controls.CustomPanel _panelIconStatusFilterButtons;
		private Mtgdb.Controls.CustomPanel _panelIconStatusSearch;
		private Mtgdb.Controls.CustomPanel _panelIconStatusFilterCollection;
		private Mtgdb.Controls.CustomPanel _panelIconStatusSets;
		private Mtgdb.Controls.CustomPanel _panelIconStatusFilterDeck;
		private Mtgdb.Controls.CustomPanel _panelIconStatusFilterLegality;
		private Mtgdb.Controls.CustomPanel _panelIconSearch;
		private Mtgdb.Controls.CustomPanel _panelIconLegality;

		private Mtgdb.Controls.CustomPanel _findCustomPanel;
		private Controls.LayoutViewControl _layoutViewDeck;
	}
}

