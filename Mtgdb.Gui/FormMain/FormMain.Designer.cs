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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			Mtgdb.Controls.SearchOptions searchOptions3 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.SortOptions sortOptions3 = new Mtgdb.Controls.SortOptions();
			Mtgdb.Controls.SearchOptions searchOptions4 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.SortOptions sortOptions4 = new Mtgdb.Controls.SortOptions();
			this._panelFilters = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterAbility = new Mtgdb.Controls.QuickFilterControl();
			this.FilterType = new Mtgdb.Controls.QuickFilterControl();
			this.FilterRarity = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManager = new Mtgdb.Controls.QuickFilterControl();
			this._panelStatus = new System.Windows.Forms.FlowLayoutPanel();
			this._tabHeadersDeck = new Mtgdb.Controls.TabHeaderControl();
			this._buttonSampleHandNew = new Mtgdb.Controls.CustomCheckBox();
			this._buttonSampleHandMulligan = new Mtgdb.Controls.CustomCheckBox();
			this._buttonSampleHandDraw = new Mtgdb.Controls.CustomCheckBox();
			this._labelStatusScrollDeck = new System.Windows.Forms.Label();
			this._panelIconStatusScrollDeck = new Mtgdb.Controls.BorderedPanel();
			this._panelIconStatusSets = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusSets = new System.Windows.Forms.Label();
			this._labelStatusScrollCards = new System.Windows.Forms.Label();
			this._panelIconStatusScrollCards = new Mtgdb.Controls.BorderedPanel();
			this._panelIconStatusCollection = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusCollection = new System.Windows.Forms.Label();
			this._panelIconStatusFilterButtons = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusFilterButtons = new System.Windows.Forms.Label();
			this._panelIconStatusSearch = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusSearch = new System.Windows.Forms.Label();
			this._panelIconStatusFilterCollection = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusFilterCollection = new System.Windows.Forms.Label();
			this._panelIconStatusFilterDeck = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusFilterDeck = new System.Windows.Forms.Label();
			this._panelIconStatusFilterLegality = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusFilterLegality = new System.Windows.Forms.Label();
			this._panelMenu = new System.Windows.Forms.FlowLayoutPanel();
			this._findBorderedPanel = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._panelIconSearch = new Mtgdb.Controls.BorderedPanel();
			this._findEditor = new Mtgdb.Controls.FixedRichTextBox();
			this._panelIconLegality = new Mtgdb.Controls.BorderedPanel();
			this._menuLegalityFormat = new System.Windows.Forms.ComboBox();
			this._buttonLegalityAllowLegal = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowRestricted = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowBanned = new Mtgdb.Controls.CustomCheckBox();
			this._buttonShowDuplicates = new Mtgdb.Controls.CustomCheckBox();
			this._panelCostLeft = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterManaAbility = new Mtgdb.Controls.QuickFilterControl();
			this.FilterGeneratedMana = new Mtgdb.Controls.QuickFilterControl();
			this.FilterCmc = new Mtgdb.Controls.QuickFilterControl();
			this._listBoxSuggest = new System.Windows.Forms.ListBox();
			this._layout = new System.Windows.Forms.TableLayoutPanel();
			this._buttonExcludeManaAbility = new Mtgdb.Controls.CustomCheckBox();
			this._buttonExcludeManaCost = new Mtgdb.Controls.CustomCheckBox();
			this.FilterManaCost = new Mtgdb.Controls.QuickFilterControl();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._buttonShowProhibit = new Mtgdb.Controls.CustomCheckBox();
			this._layoutViewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._layoutViewCards = new Mtgdb.Controls.LayoutViewControl();
			this._panelFilters.SuspendLayout();
			this._panelStatus.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._findBorderedPanel.SuspendLayout();
			this._panelCostLeft.SuspendLayout();
			this._layout.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelFilters
			// 
			this._panelFilters.AutoSize = true;
			this._panelFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelFilters.Controls.Add(this.FilterAbility);
			this._panelFilters.Controls.Add(this.FilterType);
			this._panelFilters.Controls.Add(this.FilterRarity);
			this._panelFilters.Controls.Add(this.FilterManager);
			this._panelFilters.Location = new System.Drawing.Point(0, 0);
			this._panelFilters.Margin = new System.Windows.Forms.Padding(0);
			this._panelFilters.Name = "_panelFilters";
			this._layout.SetRowSpan(this._panelFilters, 3);
			this._panelFilters.Size = new System.Drawing.Size(1432, 46);
			this._panelFilters.TabIndex = 0;
			// 
			// FilterAbility
			// 
			this.FilterAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterAbility.EnableRequiringSome = true;
			this.FilterAbility.HideProhibit = true;
			this.FilterAbility.Location = new System.Drawing.Point(0, 0);
			this.FilterAbility.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.FilterAbility.MinimumSize = new System.Drawing.Size(948, 46);
			this.FilterAbility.Name = "FilterAbility";
			this.FilterAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterAbility.PropertiesCount = 43;
			this.FilterAbility.PropertyImages = null;
			this.FilterAbility.SelectionBorder = 1.75F;
			this.FilterAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterAbility.ShowValueHint = true;
			this.FilterAbility.Size = new System.Drawing.Size(948, 46);
			this.FilterAbility.Spacing = new System.Drawing.Size(2, 2);
			this.FilterAbility.TabIndex = 13;
			this.FilterAbility.TabStop = false;
			// 
			// FilterType
			// 
			this.FilterType.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterType.EnableCostBehavior = true;
			this.FilterType.HideProhibit = true;
			this.FilterType.Location = new System.Drawing.Point(968, 0);
			this.FilterType.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.FilterType.MinimumSize = new System.Drawing.Size(178, 46);
			this.FilterType.Name = "FilterType";
			this.FilterType.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterType.PropertiesCount = 8;
			this.FilterType.PropertyImages = null;
			this.FilterType.SelectionBorder = 1.75F;
			this.FilterType.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterType.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterType.ShowValueHint = true;
			this.FilterType.Size = new System.Drawing.Size(178, 46);
			this.FilterType.Spacing = new System.Drawing.Size(2, 2);
			this.FilterType.TabIndex = 14;
			this.FilterType.TabStop = false;
			// 
			// FilterRarity
			// 
			this.FilterRarity.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterRarity.EnableCostBehavior = true;
			this.FilterRarity.EnableMutuallyExclusive = true;
			this.FilterRarity.HideProhibit = true;
			this.FilterRarity.Location = new System.Drawing.Point(1166, 0);
			this.FilterRarity.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.FilterRarity.MinimumSize = new System.Drawing.Size(134, 46);
			this.FilterRarity.Name = "FilterRarity";
			this.FilterRarity.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterRarity.PropertiesCount = 6;
			this.FilterRarity.PropertyImages = null;
			this.FilterRarity.SelectionBorder = 1.75F;
			this.FilterRarity.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterRarity.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterRarity.ShowValueHint = true;
			this.FilterRarity.Size = new System.Drawing.Size(134, 46);
			this.FilterRarity.Spacing = new System.Drawing.Size(2, 2);
			this.FilterRarity.TabIndex = 15;
			this.FilterRarity.TabStop = false;
			// 
			// FilterManager
			// 
			this.FilterManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FilterManager.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManager.EnableRequiringSome = true;
			this.FilterManager.HideProhibit = true;
			this.FilterManager.Location = new System.Drawing.Point(1320, 0);
			this.FilterManager.Margin = new System.Windows.Forms.Padding(0);
			this.FilterManager.MinimumSize = new System.Drawing.Size(112, 46);
			this.FilterManager.Name = "FilterManager";
			this.FilterManager.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManager.PropertyImages = null;
			this.FilterManager.SelectionBorder = 2F;
			this.FilterManager.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManager.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManager.ShowValueHint = true;
			this.FilterManager.Size = new System.Drawing.Size(112, 46);
			this.FilterManager.Spacing = new System.Drawing.Size(2, 2);
			this.FilterManager.TabIndex = 16;
			this.FilterManager.TabStop = false;
			// 
			// _panelStatus
			// 
			this._panelStatus.AutoSize = true;
			this._panelStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelStatus.Controls.Add(this._tabHeadersDeck);
			this._panelStatus.Controls.Add(this._buttonSampleHandNew);
			this._panelStatus.Controls.Add(this._buttonSampleHandMulligan);
			this._panelStatus.Controls.Add(this._buttonSampleHandDraw);
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
			this._panelStatus.Location = new System.Drawing.Point(0, 365);
			this._panelStatus.Margin = new System.Windows.Forms.Padding(0);
			this._panelStatus.Name = "_panelStatus";
			this._panelStatus.Size = new System.Drawing.Size(1432, 24);
			this._panelStatus.TabIndex = 14;
			// 
			// _tabHeadersDeck
			// 
			this._tabHeadersDeck.AddButtonWidth = 24;
			this._tabHeadersDeck.AllowAddingTabs = false;
			this._tabHeadersDeck.AllowRemovingTabs = false;
			this._tabHeadersDeck.AllowReorderTabs = false;
			this._tabHeadersDeck.Count = 3;
			this._tabHeadersDeck.Location = new System.Drawing.Point(0, 0);
			this._tabHeadersDeck.Margin = new System.Windows.Forms.Padding(0);
			this._tabHeadersDeck.Name = "_tabHeadersDeck";
			this._tabHeadersDeck.SelectedIndex = 0;
			this._tabHeadersDeck.Size = new System.Drawing.Size(225, 24);
			this._tabHeadersDeck.TabIndex = 29;
			this._tabHeadersDeck.TabStop = false;
			// 
			// _buttonSampleHandNew
			// 
			this._buttonSampleHandNew.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSampleHandNew.BackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatAppearance.BorderSize = 0;
			this._buttonSampleHandNew.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSampleHandNew.Image = global::Mtgdb.Gui.Properties.Resources.hand_48;
			this._buttonSampleHandNew.Location = new System.Drawing.Point(227, 0);
			this._buttonSampleHandNew.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonSampleHandNew.Name = "_buttonSampleHandNew";
			this._buttonSampleHandNew.Size = new System.Drawing.Size(60, 24);
			this._buttonSampleHandNew.TabIndex = 44;
			this._buttonSampleHandNew.TabStop = false;
			this._buttonSampleHandNew.UseVisualStyleBackColor = false;
			// 
			// _buttonSampleHandMulligan
			// 
			this._buttonSampleHandMulligan.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSampleHandMulligan.BackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatAppearance.BorderSize = 0;
			this._buttonSampleHandMulligan.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSampleHandMulligan.Image = global::Mtgdb.Gui.Properties.Resources.mulligan_48;
			this._buttonSampleHandMulligan.Location = new System.Drawing.Point(289, 0);
			this._buttonSampleHandMulligan.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonSampleHandMulligan.Name = "_buttonSampleHandMulligan";
			this._buttonSampleHandMulligan.Size = new System.Drawing.Size(48, 24);
			this._buttonSampleHandMulligan.TabIndex = 45;
			this._buttonSampleHandMulligan.TabStop = false;
			this._buttonSampleHandMulligan.UseVisualStyleBackColor = false;
			// 
			// _buttonSampleHandDraw
			// 
			this._buttonSampleHandDraw.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSampleHandDraw.BackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatAppearance.BorderSize = 0;
			this._buttonSampleHandDraw.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSampleHandDraw.Image = global::Mtgdb.Gui.Properties.Resources.draw_48;
			this._buttonSampleHandDraw.Location = new System.Drawing.Point(339, 0);
			this._buttonSampleHandDraw.Margin = new System.Windows.Forms.Padding(2, 0, 24, 0);
			this._buttonSampleHandDraw.Name = "_buttonSampleHandDraw";
			this._buttonSampleHandDraw.Size = new System.Drawing.Size(36, 24);
			this._buttonSampleHandDraw.TabIndex = 46;
			this._buttonSampleHandDraw.TabStop = false;
			this._buttonSampleHandDraw.UseVisualStyleBackColor = false;
			// 
			// _labelStatusScrollDeck
			// 
			this._labelStatusScrollDeck.AutoSize = true;
			this._labelStatusScrollDeck.Location = new System.Drawing.Point(399, 6);
			this._labelStatusScrollDeck.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusScrollDeck.Name = "_labelStatusScrollDeck";
			this._labelStatusScrollDeck.Size = new System.Drawing.Size(36, 13);
			this._labelStatusScrollDeck.TabIndex = 35;
			this._labelStatusScrollDeck.Text = "63/60";
			// 
			// _panelIconStatusScrollDeck
			// 
			this._panelIconStatusScrollDeck.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.scroll_48;
			this._panelIconStatusScrollDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusScrollDeck.Location = new System.Drawing.Point(435, 0);
			this._panelIconStatusScrollDeck.Margin = new System.Windows.Forms.Padding(0);
			this._panelIconStatusScrollDeck.Name = "_panelIconStatusScrollDeck";
			this._panelIconStatusScrollDeck.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusScrollDeck.TabIndex = 31;
			this._panelIconStatusScrollDeck.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _panelIconStatusSets
			// 
			this._panelIconStatusSets.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.mtg_48;
			this._panelIconStatusSets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusSets.Location = new System.Drawing.Point(483, 0);
			this._panelIconStatusSets.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSets.Name = "_panelIconStatusSets";
			this._panelIconStatusSets.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSets.TabIndex = 34;
			this._panelIconStatusSets.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusSets
			// 
			this._labelStatusSets.AutoSize = true;
			this._labelStatusSets.Location = new System.Drawing.Point(507, 6);
			this._labelStatusSets.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSets.Name = "_labelStatusSets";
			this._labelStatusSets.Size = new System.Drawing.Size(25, 13);
			this._labelStatusSets.TabIndex = 36;
			this._labelStatusSets.Text = "206";
			// 
			// _labelStatusScrollCards
			// 
			this._labelStatusScrollCards.AutoSize = true;
			this._labelStatusScrollCards.Location = new System.Drawing.Point(556, 6);
			this._labelStatusScrollCards.Margin = new System.Windows.Forms.Padding(24, 6, 0, 0);
			this._labelStatusScrollCards.Name = "_labelStatusScrollCards";
			this._labelStatusScrollCards.Size = new System.Drawing.Size(72, 13);
			this._labelStatusScrollCards.TabIndex = 37;
			this._labelStatusScrollCards.Text = "15999/16001";
			// 
			// _panelIconStatusScrollCards
			// 
			this._panelIconStatusScrollCards.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.scroll_48;
			this._panelIconStatusScrollCards.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusScrollCards.Location = new System.Drawing.Point(628, 0);
			this._panelIconStatusScrollCards.Margin = new System.Windows.Forms.Padding(0);
			this._panelIconStatusScrollCards.Name = "_panelIconStatusScrollCards";
			this._panelIconStatusScrollCards.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusScrollCards.TabIndex = 30;
			this._panelIconStatusScrollCards.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _panelIconStatusCollection
			// 
			this._panelIconStatusCollection.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.box_48;
			this._panelIconStatusCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusCollection.Location = new System.Drawing.Point(676, 0);
			this._panelIconStatusCollection.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusCollection.Name = "_panelIconStatusCollection";
			this._panelIconStatusCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusCollection.TabIndex = 31;
			this._panelIconStatusCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusCollection
			// 
			this._labelStatusCollection.AutoSize = true;
			this._labelStatusCollection.Location = new System.Drawing.Point(700, 6);
			this._labelStatusCollection.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusCollection.Name = "_labelStatusCollection";
			this._labelStatusCollection.Size = new System.Drawing.Size(25, 13);
			this._labelStatusCollection.TabIndex = 38;
			this._labelStatusCollection.Text = "691";
			// 
			// _panelIconStatusFilterButtons
			// 
			this._panelIconStatusFilterButtons.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.quick_filters_48;
			this._panelIconStatusFilterButtons.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterButtons.Location = new System.Drawing.Point(749, 0);
			this._panelIconStatusFilterButtons.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterButtons.Name = "_panelIconStatusFilterButtons";
			this._panelIconStatusFilterButtons.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterButtons.TabIndex = 32;
			this._panelIconStatusFilterButtons.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterButtons
			// 
			this._labelStatusFilterButtons.AutoSize = true;
			this._labelStatusFilterButtons.Location = new System.Drawing.Point(773, 6);
			this._labelStatusFilterButtons.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterButtons.Name = "_labelStatusFilterButtons";
			this._labelStatusFilterButtons.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterButtons.TabIndex = 39;
			this._labelStatusFilterButtons.Text = "ignored";
			// 
			// _panelIconStatusSearch
			// 
			this._panelIconStatusSearch.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.search_48;
			this._panelIconStatusSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusSearch.Location = new System.Drawing.Point(839, 0);
			this._panelIconStatusSearch.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSearch.Name = "_panelIconStatusSearch";
			this._panelIconStatusSearch.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSearch.TabIndex = 33;
			this._panelIconStatusSearch.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusSearch
			// 
			this._labelStatusSearch.AutoSize = true;
			this._labelStatusSearch.Location = new System.Drawing.Point(863, 6);
			this._labelStatusSearch.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSearch.Name = "_labelStatusSearch";
			this._labelStatusSearch.Size = new System.Drawing.Size(145, 13);
			this._labelStatusSearch.TabIndex = 41;
			this._labelStatusSearch.Text = "modified, press Enter to apply";
			// 
			// _panelIconStatusFilterCollection
			// 
			this._panelIconStatusFilterCollection.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.box_48;
			this._panelIconStatusFilterCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterCollection.Location = new System.Drawing.Point(1032, 0);
			this._panelIconStatusFilterCollection.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterCollection.Name = "_panelIconStatusFilterCollection";
			this._panelIconStatusFilterCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterCollection.TabIndex = 32;
			this._panelIconStatusFilterCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterCollection
			// 
			this._labelStatusFilterCollection.AutoSize = true;
			this._labelStatusFilterCollection.Location = new System.Drawing.Point(1056, 6);
			this._labelStatusFilterCollection.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterCollection.Name = "_labelStatusFilterCollection";
			this._labelStatusFilterCollection.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterCollection.TabIndex = 40;
			this._labelStatusFilterCollection.Text = "ignored";
			// 
			// _panelIconStatusFilterDeck
			// 
			this._panelIconStatusFilterDeck.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.draw_a_card_48;
			this._panelIconStatusFilterDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterDeck.Location = new System.Drawing.Point(1122, 0);
			this._panelIconStatusFilterDeck.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterDeck.Name = "_panelIconStatusFilterDeck";
			this._panelIconStatusFilterDeck.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterDeck.TabIndex = 33;
			this._panelIconStatusFilterDeck.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterDeck
			// 
			this._labelStatusFilterDeck.AutoSize = true;
			this._labelStatusFilterDeck.Location = new System.Drawing.Point(1146, 6);
			this._labelStatusFilterDeck.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterDeck.Name = "_labelStatusFilterDeck";
			this._labelStatusFilterDeck.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterDeck.TabIndex = 42;
			this._labelStatusFilterDeck.Text = "ignored";
			// 
			// _panelIconStatusFilterLegality
			// 
			this._panelIconStatusFilterLegality.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.legality_48;
			this._panelIconStatusFilterLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterLegality.Location = new System.Drawing.Point(1212, 0);
			this._panelIconStatusFilterLegality.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterLegality.Name = "_panelIconStatusFilterLegality";
			this._panelIconStatusFilterLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterLegality.TabIndex = 34;
			this._panelIconStatusFilterLegality.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterLegality
			// 
			this._labelStatusFilterLegality.AutoSize = true;
			this._labelStatusFilterLegality.Location = new System.Drawing.Point(1236, 6);
			this._labelStatusFilterLegality.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterLegality.Name = "_labelStatusFilterLegality";
			this._labelStatusFilterLegality.Size = new System.Drawing.Size(196, 13);
			this._labelStatusFilterLegality.TabIndex = 43;
			this._labelStatusFilterLegality.Text = "and Standard +legal +restricted -banned";
			// 
			// _panelMenu
			// 
			this._panelMenu.AutoSize = true;
			this._panelMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenu.Controls.Add(this._findBorderedPanel);
			this._panelMenu.Controls.Add(this._panelIconLegality);
			this._panelMenu.Controls.Add(this._menuLegalityFormat);
			this._panelMenu.Controls.Add(this._buttonLegalityAllowLegal);
			this._panelMenu.Controls.Add(this._buttonLegalityAllowRestricted);
			this._panelMenu.Controls.Add(this._buttonLegalityAllowBanned);
			this._panelMenu.Controls.Add(this._buttonShowDuplicates);
			this._panelMenu.Location = new System.Drawing.Point(0, 48);
			this._panelMenu.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(835, 24);
			this._panelMenu.TabIndex = 10;
			// 
			// _findBorderedPanel
			// 
			this._findBorderedPanel.BackColor = System.Drawing.Color.White;
			this._findBorderedPanel.Controls.Add(this._panelIconSearch);
			this._findBorderedPanel.Controls.Add(this._findEditor);
			this._findBorderedPanel.Location = new System.Drawing.Point(0, 0);
			this._findBorderedPanel.Margin = new System.Windows.Forms.Padding(0);
			this._findBorderedPanel.Name = "_findBorderedPanel";
			this._findBorderedPanel.Size = new System.Drawing.Size(486, 24);
			this._findBorderedPanel.TabIndex = 22;
			this._findBorderedPanel.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _panelIconSearch
			// 
			this._panelIconSearch.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.search_48;
			this._panelIconSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconSearch.Location = new System.Drawing.Point(1, 1);
			this._panelIconSearch.Margin = new System.Windows.Forms.Padding(1);
			this._panelIconSearch.Name = "_panelIconSearch";
			this._panelIconSearch.Size = new System.Drawing.Size(22, 22);
			this._panelIconSearch.TabIndex = 21;
			this._panelIconSearch.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _findEditor
			// 
			this._findEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._findEditor.DetectUrls = false;
			this._findEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._findEditor.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._findEditor.Location = new System.Drawing.Point(24, 5);
			this._findEditor.Margin = new System.Windows.Forms.Padding(0, 5, 1, 1);
			this._findEditor.Multiline = false;
			this._findEditor.Name = "_findEditor";
			this._findEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this._findEditor.Size = new System.Drawing.Size(461, 18);
			this._findEditor.TabIndex = 20;
			this._findEditor.TabStop = false;
			this._findEditor.Text = "";
			this._findEditor.WordWrap = false;
			// 
			// _panelIconLegality
			// 
			this._panelIconLegality.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.legality_48;
			this._panelIconLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconLegality.Location = new System.Drawing.Point(488, 0);
			this._panelIconLegality.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this._panelIconLegality.Name = "_panelIconLegality";
			this._panelIconLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconLegality.TabIndex = 35;
			this._panelIconLegality.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _menuLegalityFormat
			// 
			this._menuLegalityFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuLegalityFormat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuLegalityFormat.IntegralHeight = false;
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
            "Amonkhet Block",
            "Battle for Zendikar Block",
            "Ice Age Block",
            "Innistrad Block",
            "Ixalan Block",
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
			this._menuLegalityFormat.Location = new System.Drawing.Point(514, 2);
			this._menuLegalityFormat.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuLegalityFormat.MaxDropDownItems = 35;
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
			this._buttonLegalityAllowLegal.Location = new System.Drawing.Point(639, 4);
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
			this._buttonLegalityAllowRestricted.Location = new System.Drawing.Point(684, 4);
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
			this._buttonLegalityAllowBanned.Location = new System.Drawing.Point(750, 4);
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
			this._buttonShowDuplicates.Image = global::Mtgdb.Gui.Properties.Resources.clone_48;
			this._buttonShowDuplicates.Location = new System.Drawing.Point(811, 0);
			this._buttonShowDuplicates.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonShowDuplicates.Name = "_buttonShowDuplicates";
			this._buttonShowDuplicates.Size = new System.Drawing.Size(24, 24);
			this._buttonShowDuplicates.TabIndex = 40;
			this._buttonShowDuplicates.TabStop = false;
			this._buttonShowDuplicates.UseVisualStyleBackColor = false;
			// 
			// _panelCostLeft
			// 
			this._panelCostLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._panelCostLeft.AutoSize = true;
			this._panelCostLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelCostLeft.Controls.Add(this.FilterManaAbility);
			this._panelCostLeft.Controls.Add(this.FilterGeneratedMana);
			this._panelCostLeft.Controls.Add(this.FilterCmc);
			this._panelCostLeft.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelCostLeft.Location = new System.Drawing.Point(1446, 48);
			this._panelCostLeft.Margin = new System.Windows.Forms.Padding(0);
			this._panelCostLeft.Name = "_panelCostLeft";
			this._layout.SetRowSpan(this._panelCostLeft, 5);
			this._panelCostLeft.Size = new System.Drawing.Size(46, 618);
			this._panelCostLeft.TabIndex = 0;
			this._panelCostLeft.WrapContents = false;
			// 
			// FilterManaAbility
			// 
			this.FilterManaAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManaAbility.EnableRequiringSome = true;
			this.FilterManaAbility.HideProhibit = true;
			this.FilterManaAbility.HintIcon = global::Mtgdb.Gui.Properties.Resources.manatext_25;
			this.FilterManaAbility.HintTextShift = new System.Drawing.Size(-4, -4);
			this.FilterManaAbility.IsFlipped = true;
			this.FilterManaAbility.IsVertical = true;
			this.FilterManaAbility.Location = new System.Drawing.Point(0, 0);
			this.FilterManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
			this.FilterManaAbility.MaximumSize = new System.Drawing.Size(50, 200);
			this.FilterManaAbility.MinimumSize = new System.Drawing.Size(46, 244);
			this.FilterManaAbility.Name = "FilterManaAbility";
			this.FilterManaAbility.Opacity3ToDisable = 0.2F;
			this.FilterManaAbility.Opacity4Disabled = 0.1F;
			this.FilterManaAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaAbility.PropertiesCount = 11;
			this.FilterManaAbility.PropertyImages = null;
			this.FilterManaAbility.SelectionBorder = 1.75F;
			this.FilterManaAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaAbility.ShowValueHint = true;
			this.FilterManaAbility.Size = new System.Drawing.Size(46, 244);
			this.FilterManaAbility.Spacing = new System.Drawing.Size(2, 2);
			this.FilterManaAbility.TabIndex = 19;
			this.FilterManaAbility.TabStop = false;
			// 
			// FilterGeneratedMana
			// 
			this.FilterGeneratedMana.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterGeneratedMana.EnableRequiringSome = true;
			this.FilterGeneratedMana.HideProhibit = true;
			this.FilterGeneratedMana.IsFlipped = true;
			this.FilterGeneratedMana.IsVertical = true;
			this.FilterGeneratedMana.Location = new System.Drawing.Point(0, 264);
			this.FilterGeneratedMana.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
			this.FilterGeneratedMana.MinimumSize = new System.Drawing.Size(46, 156);
			this.FilterGeneratedMana.Name = "FilterGeneratedMana";
			this.FilterGeneratedMana.Opacity3ToDisable = 0.2F;
			this.FilterGeneratedMana.Opacity4Disabled = 0.1F;
			this.FilterGeneratedMana.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterGeneratedMana.PropertiesCount = 7;
			this.FilterGeneratedMana.PropertyImages = null;
			this.FilterGeneratedMana.SelectionBorder = 1.75F;
			this.FilterGeneratedMana.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterGeneratedMana.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterGeneratedMana.ShowValueHint = true;
			this.FilterGeneratedMana.Size = new System.Drawing.Size(46, 156);
			this.FilterGeneratedMana.Spacing = new System.Drawing.Size(2, 2);
			this.FilterGeneratedMana.TabIndex = 20;
			this.FilterGeneratedMana.TabStop = false;
			// 
			// FilterCmc
			// 
			this.FilterCmc.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterCmc.EnableCostBehavior = true;
			this.FilterCmc.EnableMutuallyExclusive = true;
			this.FilterCmc.HideProhibit = true;
			this.FilterCmc.IsFlipped = true;
			this.FilterCmc.IsVertical = true;
			this.FilterCmc.Location = new System.Drawing.Point(0, 440);
			this.FilterCmc.Margin = new System.Windows.Forms.Padding(0);
			this.FilterCmc.MinimumSize = new System.Drawing.Size(46, 178);
			this.FilterCmc.Name = "FilterCmc";
			this.FilterCmc.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterCmc.PropertiesCount = 8;
			this.FilterCmc.PropertyImages = null;
			this.FilterCmc.SelectionBorder = 1.75F;
			this.FilterCmc.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterCmc.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterCmc.Size = new System.Drawing.Size(46, 178);
			this.FilterCmc.Spacing = new System.Drawing.Size(2, 2);
			this.FilterCmc.TabIndex = 21;
			this.FilterCmc.TabStop = false;
			// 
			// _listBoxSuggest
			// 
			this._listBoxSuggest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._listBoxSuggest.FormattingEnabled = true;
			this._listBoxSuggest.Location = new System.Drawing.Point(12, 94);
			this._listBoxSuggest.Name = "_listBoxSuggest";
			this._listBoxSuggest.Size = new System.Drawing.Size(192, 119);
			this._listBoxSuggest.TabIndex = 9;
			this._listBoxSuggest.TabStop = false;
			// 
			// _layout
			// 
			this._layout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layout.ColumnCount = 3;
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layout.Controls.Add(this._buttonExcludeManaAbility, 1, 1);
			this._layout.Controls.Add(this._layoutViewDeck, 0, 6);
			this._layout.Controls.Add(this._buttonExcludeManaCost, 2, 1);
			this._layout.Controls.Add(this._panelFilters, 0, 0);
			this._layout.Controls.Add(this._panelMenu, 0, 3);
			this._layout.Controls.Add(this._layoutViewCards, 0, 4);
			this._layout.Controls.Add(this._panelStatus, 0, 5);
			this._layout.Controls.Add(this.FilterManaCost, 2, 2);
			this._layout.Controls.Add(this._panelCostLeft, 1, 2);
			this._layout.Controls.Add(this.flowLayoutPanel1, 1, 0);
			this._layout.Controls.Add(this._buttonShowProhibit, 2, 0);
			this._layout.Location = new System.Drawing.Point(0, 0);
			this._layout.Margin = new System.Windows.Forms.Padding(0);
			this._layout.Name = "_layout";
			this._layout.RowCount = 7;
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.Size = new System.Drawing.Size(1550, 700);
			this._layout.TabIndex = 44;
			// 
			// _buttonExcludeManaAbility
			// 
			this._buttonExcludeManaAbility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonExcludeManaAbility.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonExcludeManaAbility.BackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatAppearance.BorderSize = 0;
			this._buttonExcludeManaAbility.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaAbility.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExcludeManaAbility.Image = global::Mtgdb.Gui.Properties.Resources.include_plus_24;
			this._buttonExcludeManaAbility.Location = new System.Drawing.Point(1447, 24);
			this._buttonExcludeManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaAbility.Name = "_buttonExcludeManaAbility";
			this._buttonExcludeManaAbility.Size = new System.Drawing.Size(23, 24);
			this._buttonExcludeManaAbility.TabIndex = 41;
			this._buttonExcludeManaAbility.TabStop = false;
			this._buttonExcludeManaAbility.UseVisualStyleBackColor = false;
			// 
			// _buttonExcludeManaCost
			// 
			this._buttonExcludeManaCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonExcludeManaCost.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonExcludeManaCost.BackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.Checked = true;
			this._buttonExcludeManaCost.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonExcludeManaCost.FlatAppearance.BorderSize = 0;
			this._buttonExcludeManaCost.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonExcludeManaCost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExcludeManaCost.Image = global::Mtgdb.Gui.Properties.Resources.exclude_minus_24;
			this._buttonExcludeManaCost.Location = new System.Drawing.Point(1504, 24);
			this._buttonExcludeManaCost.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaCost.Name = "_buttonExcludeManaCost";
			this._buttonExcludeManaCost.Size = new System.Drawing.Size(24, 24);
			this._buttonExcludeManaCost.TabIndex = 42;
			this._buttonExcludeManaCost.TabStop = false;
			this._buttonExcludeManaCost.UseVisualStyleBackColor = false;
			// 
			// FilterManaCost
			// 
			this.FilterManaCost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FilterManaCost.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManaCost.EnableCostBehavior = true;
			this.FilterManaCost.HideProhibit = true;
			this.FilterManaCost.HintIcon = global::Mtgdb.Gui.Properties.Resources.manacost_25;
			this.FilterManaCost.HintTextShift = new System.Drawing.Size(4, 10);
			this.FilterManaCost.IsFlipped = true;
			this.FilterManaCost.IsVertical = true;
			this.FilterManaCost.Location = new System.Drawing.Point(1504, 48);
			this.FilterManaCost.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this.FilterManaCost.MinimumSize = new System.Drawing.Size(46, 618);
			this.FilterManaCost.Name = "FilterManaCost";
			this.FilterManaCost.Opacity3ToDisable = 0.2F;
			this.FilterManaCost.Opacity4Disabled = 0.1F;
			this.FilterManaCost.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaCost.PropertiesCount = 28;
			this.FilterManaCost.PropertyImages = null;
			this._layout.SetRowSpan(this.FilterManaCost, 5);
			this.FilterManaCost.SelectionBorder = 1.75F;
			this.FilterManaCost.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaCost.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaCost.ShowValueHint = true;
			this.FilterManaCost.Size = new System.Drawing.Size(46, 618);
			this.FilterManaCost.Spacing = new System.Drawing.Size(2, 2);
			this.FilterManaCost.TabIndex = 22;
			this.FilterManaCost.TabStop = false;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(1492, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
			this.flowLayoutPanel1.TabIndex = 22;
			// 
			// _buttonShowProhibit
			// 
			this._buttonShowProhibit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowProhibit.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowProhibit.BackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatAppearance.BorderSize = 0;
			this._buttonShowProhibit.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonShowProhibit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowProhibit.Image = ((System.Drawing.Image)(resources.GetObject("_buttonShowProhibit.Image")));
			this._buttonShowProhibit.Location = new System.Drawing.Point(1526, 0);
			this._buttonShowProhibit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowProhibit.Name = "_buttonShowProhibit";
			this._buttonShowProhibit.Size = new System.Drawing.Size(24, 24);
			this._buttonShowProhibit.TabIndex = 41;
			this._buttonShowProhibit.TabStop = false;
			this._buttonShowProhibit.UseVisualStyleBackColor = false;
			// 
			// _layoutViewDeck
			// 
			this._layoutViewDeck.AllowPartialCards = true;
			this._layoutViewDeck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutViewDeck.BackColor = System.Drawing.Color.White;
			this._layoutViewDeck.CardInterval = new System.Drawing.Size(2, 2);
			this._layoutViewDeck.HotTrackBackgroundColor = System.Drawing.Color.WhiteSmoke;
			this._layoutViewDeck.HotTrackBorderColor = System.Drawing.Color.Gainsboro;
			this._layoutViewDeck.LayoutControlType = typeof(Mtgdb.Gui.DeckLayout);
			this._layoutViewDeck.Location = new System.Drawing.Point(0, 389);
			this._layoutViewDeck.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewDeck.Name = "_layoutViewDeck";
			this._layoutViewDeck.PartialCardsThreshold = new System.Drawing.Size(161, 225);
			this._layoutViewDeck.SearchOptions = searchOptions3;
			this._layoutViewDeck.Size = new System.Drawing.Size(1446, 311);
			sortOptions3.Allow = true;
			sortOptions3.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions3.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions3.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewDeck.SortOptions = sortOptions3;
			this._layoutViewDeck.TabIndex = 42;
			this._layoutViewDeck.TabStop = false;
			// 
			// _layoutViewCards
			// 
			this._layoutViewCards.AllowPartialCards = true;
			this._layoutViewCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutViewCards.BackColor = System.Drawing.Color.White;
			this._layoutViewCards.CardInterval = new System.Drawing.Size(4, 2);
			this._layoutViewCards.HotTrackBackgroundColor = System.Drawing.Color.WhiteSmoke;
			this._layoutViewCards.HotTrackBorderColor = System.Drawing.Color.Gainsboro;
			this._layoutViewCards.LayoutControlType = typeof(Mtgdb.Gui.CardLayout);
			this._layoutViewCards.Location = new System.Drawing.Point(0, 72);
			this._layoutViewCards.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewCards.Name = "_layoutViewCards";
			this._layoutViewCards.PartialCardsThreshold = new System.Drawing.Size(161, 225);
			searchOptions4.Allow = true;
			searchOptions4.ButtonMargin = new System.Drawing.Size(19, 2);
			searchOptions4.Icon = global::Mtgdb.Gui.Properties.Resources.search_hovered;
			this._layoutViewCards.SearchOptions = searchOptions4;
			this._layoutViewCards.Size = new System.Drawing.Size(1446, 293);
			sortOptions4.Allow = true;
			sortOptions4.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions4.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions4.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewCards.SortOptions = sortOptions4;
			this._layoutViewCards.TabIndex = 19;
			this._layoutViewCards.TabStop = false;
			// 
			// FormMain
			// 
			this.ClientSize = new System.Drawing.Size(1550, 700);
			this.Controls.Add(this._listBoxSuggest);
			this.Controls.Add(this._layout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this._panelFilters.ResumeLayout(false);
			this._panelStatus.ResumeLayout(false);
			this._panelStatus.PerformLayout();
			this._panelMenu.ResumeLayout(false);
			this._panelMenu.PerformLayout();
			this._findBorderedPanel.ResumeLayout(false);
			this._panelCostLeft.ResumeLayout(false);
			this._layout.ResumeLayout(false);
			this._layout.PerformLayout();
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

		private System.Windows.Forms.FlowLayoutPanel _panelFilters;
		private System.Windows.Forms.FlowLayoutPanel _panelStatus;
		
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

		private Mtgdb.Controls.BorderedPanel _panelIconStatusScrollDeck;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusScrollCards;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusCollection;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusFilterButtons;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusSearch;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusFilterCollection;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusSets;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusFilterDeck;
		private Mtgdb.Controls.BorderedPanel _panelIconStatusFilterLegality;
		private Mtgdb.Controls.BorderedPanel _panelIconSearch;
		private Mtgdb.Controls.BorderedPanel _panelIconLegality;
		private Controls.LayoutViewControl _layoutViewDeck;
		private Mtgdb.Controls.BorderedFlowLayoutPanel _findBorderedPanel;
		private System.Windows.Forms.FlowLayoutPanel _panelCostLeft;
		private System.Windows.Forms.TableLayoutPanel _layout;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private Controls.CustomCheckBox _buttonSampleHandNew;
		private Controls.CustomCheckBox _buttonSampleHandMulligan;
		private Controls.CustomCheckBox _buttonSampleHandDraw;
	}
}

