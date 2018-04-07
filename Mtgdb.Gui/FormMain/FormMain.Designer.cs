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
			Mtgdb.Controls.LayoutOptions layoutOptions1 = new Mtgdb.Controls.LayoutOptions();
			Mtgdb.Controls.SearchOptions searchOptions1 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions1 = new Mtgdb.Controls.ButtonOptions();
			Mtgdb.Controls.SelectionOptions selectionOptions1 = new Mtgdb.Controls.SelectionOptions();
			Mtgdb.Controls.SortOptions sortOptions1 = new Mtgdb.Controls.SortOptions();
			Mtgdb.Controls.LayoutOptions layoutOptions2 = new Mtgdb.Controls.LayoutOptions();
			Mtgdb.Controls.SearchOptions searchOptions2 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions2 = new Mtgdb.Controls.ButtonOptions();
			Mtgdb.Controls.SelectionOptions selectionOptions2 = new Mtgdb.Controls.SelectionOptions();
			Mtgdb.Controls.SortOptions sortOptions2 = new Mtgdb.Controls.SortOptions();
			this._panelFilters = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterAbility = new Mtgdb.Controls.QuickFilterControl();
			this.FilterType = new Mtgdb.Controls.QuickFilterControl();
			this.FilterRarity = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManager = new Mtgdb.Controls.QuickFilterControl();
			this._panelStatus = new System.Windows.Forms.FlowLayoutPanel();
			this._buttonHideDeck = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHidePartialCards = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHideText = new Mtgdb.Controls.CustomCheckBox();
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
			this._layoutViewCards = new Mtgdb.Controls.LayoutViewControl();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._layoutViewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._layoutRight = new System.Windows.Forms.TableLayoutPanel();
			this._buttonExcludeManaAbility = new Mtgdb.Controls.CustomCheckBox();
			this._buttonExcludeManaCost = new Mtgdb.Controls.CustomCheckBox();
			this._buttonShowProhibit = new Mtgdb.Controls.CustomCheckBox();
			this.FilterManaCost = new Mtgdb.Controls.QuickFilterControl();
			this._layoutRoot = new System.Windows.Forms.TableLayoutPanel();
			this._panelFilters.SuspendLayout();
			this._panelStatus.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._findBorderedPanel.SuspendLayout();
			this._panelCostLeft.SuspendLayout();
			this._layout.SuspendLayout();
			this._layoutRight.SuspendLayout();
			this._layoutRoot.SuspendLayout();
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
			this._panelStatus.Controls.Add(this._buttonHideDeck);
			this._panelStatus.Controls.Add(this._buttonHidePartialCards);
			this._panelStatus.Controls.Add(this._buttonHideText);
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
			this._panelStatus.Location = new System.Drawing.Point(0, 489);
			this._panelStatus.Margin = new System.Windows.Forms.Padding(0);
			this._panelStatus.Name = "_panelStatus";
			this._panelStatus.Size = new System.Drawing.Size(1510, 24);
			this._panelStatus.TabIndex = 14;
			// 
			// _buttonHideDeck
			// 
			this._buttonHideDeck.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHideDeck.BackColor = System.Drawing.Color.Transparent;
			this._buttonHideDeck.FlatAppearance.BorderSize = 0;
			this._buttonHideDeck.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonHideDeck.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonHideDeck.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonHideDeck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHideDeck.Image = global::Mtgdb.Gui.Properties.Resources.shown_40;
			this._buttonHideDeck.Location = new System.Drawing.Point(2, 0);
			this._buttonHideDeck.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonHideDeck.Name = "_buttonHideDeck";
			this._buttonHideDeck.Size = new System.Drawing.Size(24, 24);
			this._buttonHideDeck.TabIndex = 47;
			this._buttonHideDeck.TabStop = false;
			this._buttonHideDeck.UseVisualStyleBackColor = false;
			// 
			// _buttonHidePartialCards
			// 
			this._buttonHidePartialCards.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHidePartialCards.BackColor = System.Drawing.Color.Transparent;
			this._buttonHidePartialCards.FlatAppearance.BorderSize = 0;
			this._buttonHidePartialCards.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonHidePartialCards.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonHidePartialCards.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonHidePartialCards.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHidePartialCards.Image = global::Mtgdb.Gui.Properties.Resources.partial_card_enabled_40;
			this._buttonHidePartialCards.Location = new System.Drawing.Point(28, 0);
			this._buttonHidePartialCards.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonHidePartialCards.Name = "_buttonHidePartialCards";
			this._buttonHidePartialCards.Size = new System.Drawing.Size(24, 24);
			this._buttonHidePartialCards.TabIndex = 48;
			this._buttonHidePartialCards.TabStop = false;
			this._buttonHidePartialCards.UseVisualStyleBackColor = false;
			// 
			// _buttonHideText
			// 
			this._buttonHideText.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHideText.BackColor = System.Drawing.Color.Transparent;
			this._buttonHideText.FlatAppearance.BorderSize = 0;
			this._buttonHideText.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonHideText.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonHideText.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonHideText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHideText.Image = global::Mtgdb.Gui.Properties.Resources.text_enabled_40;
			this._buttonHideText.Location = new System.Drawing.Point(54, 0);
			this._buttonHideText.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonHideText.Name = "_buttonHideText";
			this._buttonHideText.Size = new System.Drawing.Size(24, 24);
			this._buttonHideText.TabIndex = 49;
			this._buttonHideText.TabStop = false;
			this._buttonHideText.UseVisualStyleBackColor = false;
			// 
			// _tabHeadersDeck
			// 
			this._tabHeadersDeck.AddButtonWidth = 24;
			this._tabHeadersDeck.AllowAddingTabs = false;
			this._tabHeadersDeck.AllowRemovingTabs = false;
			this._tabHeadersDeck.AllowReorderTabs = false;
			this._tabHeadersDeck.Count = 3;
			this._tabHeadersDeck.Location = new System.Drawing.Point(78, 0);
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
			this._buttonSampleHandNew.Location = new System.Drawing.Point(305, 0);
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
			this._buttonSampleHandMulligan.Location = new System.Drawing.Point(367, 0);
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
			this._buttonSampleHandDraw.Location = new System.Drawing.Point(417, 0);
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
			this._labelStatusScrollDeck.Location = new System.Drawing.Point(477, 6);
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
			this._panelIconStatusScrollDeck.Location = new System.Drawing.Point(513, 0);
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
			this._panelIconStatusSets.Location = new System.Drawing.Point(561, 0);
			this._panelIconStatusSets.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSets.Name = "_panelIconStatusSets";
			this._panelIconStatusSets.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSets.TabIndex = 34;
			this._panelIconStatusSets.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusSets
			// 
			this._labelStatusSets.AutoSize = true;
			this._labelStatusSets.Location = new System.Drawing.Point(585, 6);
			this._labelStatusSets.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSets.Name = "_labelStatusSets";
			this._labelStatusSets.Size = new System.Drawing.Size(25, 13);
			this._labelStatusSets.TabIndex = 36;
			this._labelStatusSets.Text = "206";
			// 
			// _labelStatusScrollCards
			// 
			this._labelStatusScrollCards.AutoSize = true;
			this._labelStatusScrollCards.Location = new System.Drawing.Point(634, 6);
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
			this._panelIconStatusScrollCards.Location = new System.Drawing.Point(706, 0);
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
			this._panelIconStatusCollection.Location = new System.Drawing.Point(754, 0);
			this._panelIconStatusCollection.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusCollection.Name = "_panelIconStatusCollection";
			this._panelIconStatusCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusCollection.TabIndex = 31;
			this._panelIconStatusCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusCollection
			// 
			this._labelStatusCollection.AutoSize = true;
			this._labelStatusCollection.Location = new System.Drawing.Point(778, 6);
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
			this._panelIconStatusFilterButtons.Location = new System.Drawing.Point(827, 0);
			this._panelIconStatusFilterButtons.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterButtons.Name = "_panelIconStatusFilterButtons";
			this._panelIconStatusFilterButtons.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterButtons.TabIndex = 32;
			this._panelIconStatusFilterButtons.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterButtons
			// 
			this._labelStatusFilterButtons.AutoSize = true;
			this._labelStatusFilterButtons.Location = new System.Drawing.Point(851, 6);
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
			this._panelIconStatusSearch.Location = new System.Drawing.Point(917, 0);
			this._panelIconStatusSearch.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSearch.Name = "_panelIconStatusSearch";
			this._panelIconStatusSearch.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSearch.TabIndex = 33;
			this._panelIconStatusSearch.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusSearch
			// 
			this._labelStatusSearch.AutoSize = true;
			this._labelStatusSearch.Location = new System.Drawing.Point(941, 6);
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
			this._panelIconStatusFilterCollection.Location = new System.Drawing.Point(1110, 0);
			this._panelIconStatusFilterCollection.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterCollection.Name = "_panelIconStatusFilterCollection";
			this._panelIconStatusFilterCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterCollection.TabIndex = 32;
			this._panelIconStatusFilterCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterCollection
			// 
			this._labelStatusFilterCollection.AutoSize = true;
			this._labelStatusFilterCollection.Location = new System.Drawing.Point(1134, 6);
			this._labelStatusFilterCollection.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterCollection.Name = "_labelStatusFilterCollection";
			this._labelStatusFilterCollection.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterCollection.TabIndex = 40;
			this._labelStatusFilterCollection.Text = "ignored";
			// 
			// _panelIconStatusFilterDeck
			// 
			this._panelIconStatusFilterDeck.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.deck_48;
			this._panelIconStatusFilterDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterDeck.Location = new System.Drawing.Point(1200, 0);
			this._panelIconStatusFilterDeck.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterDeck.Name = "_panelIconStatusFilterDeck";
			this._panelIconStatusFilterDeck.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterDeck.TabIndex = 33;
			this._panelIconStatusFilterDeck.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterDeck
			// 
			this._labelStatusFilterDeck.AutoSize = true;
			this._labelStatusFilterDeck.Location = new System.Drawing.Point(1224, 6);
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
			this._panelIconStatusFilterLegality.Location = new System.Drawing.Point(1290, 0);
			this._panelIconStatusFilterLegality.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusFilterLegality.Name = "_panelIconStatusFilterLegality";
			this._panelIconStatusFilterLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterLegality.TabIndex = 34;
			this._panelIconStatusFilterLegality.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusFilterLegality
			// 
			this._labelStatusFilterLegality.AutoSize = true;
			this._labelStatusFilterLegality.Location = new System.Drawing.Point(1314, 6);
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
			this._panelMenu.Location = new System.Drawing.Point(0, 46);
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
			this._panelCostLeft.AutoSize = true;
			this._panelCostLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelCostLeft.Controls.Add(this.FilterManaAbility);
			this._panelCostLeft.Controls.Add(this.FilterGeneratedMana);
			this._panelCostLeft.Controls.Add(this.FilterCmc);
			this._panelCostLeft.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelCostLeft.Location = new System.Drawing.Point(0, 44);
			this._panelCostLeft.Margin = new System.Windows.Forms.Padding(0);
			this._panelCostLeft.Name = "_panelCostLeft";
			this._panelCostLeft.Size = new System.Drawing.Size(46, 706);
			this._panelCostLeft.TabIndex = 0;
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
			this.FilterManaAbility.MinimumSize = new System.Drawing.Size(46, 266);
			this.FilterManaAbility.Name = "FilterManaAbility";
			this.FilterManaAbility.Opacity3ToDisable = 0.2F;
			this.FilterManaAbility.Opacity4Disabled = 0.1F;
			this.FilterManaAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaAbility.PropertiesCount = 12;
			this.FilterManaAbility.PropertyImages = null;
			this.FilterManaAbility.SelectionBorder = 1.75F;
			this.FilterManaAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaAbility.ShowValueHint = true;
			this.FilterManaAbility.Size = new System.Drawing.Size(46, 266);
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
			this.FilterGeneratedMana.Location = new System.Drawing.Point(0, 286);
			this.FilterGeneratedMana.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
			this.FilterGeneratedMana.MinimumSize = new System.Drawing.Size(46, 222);
			this.FilterGeneratedMana.Name = "FilterGeneratedMana";
			this.FilterGeneratedMana.Opacity3ToDisable = 0.2F;
			this.FilterGeneratedMana.Opacity4Disabled = 0.1F;
			this.FilterGeneratedMana.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterGeneratedMana.PropertiesCount = 10;
			this.FilterGeneratedMana.PropertyImages = null;
			this.FilterGeneratedMana.SelectionBorder = 1.75F;
			this.FilterGeneratedMana.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterGeneratedMana.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterGeneratedMana.ShowValueHint = true;
			this.FilterGeneratedMana.Size = new System.Drawing.Size(46, 222);
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
			this.FilterCmc.Location = new System.Drawing.Point(0, 528);
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
			this._listBoxSuggest.Location = new System.Drawing.Point(12, 84);
			this._listBoxSuggest.Name = "_listBoxSuggest";
			this._listBoxSuggest.Size = new System.Drawing.Size(204, 119);
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
			this._layout.Controls.Add(this._layoutViewCards, 0, 2);
			this._layout.Controls.Add(this._panelFilters, 0, 0);
			this._layout.Controls.Add(this._panelMenu, 0, 1);
			this._layout.Controls.Add(this._panelStatus, 0, 3);
			this._layout.Controls.Add(this.flowLayoutPanel1, 1, 0);
			this._layout.Controls.Add(this._layoutViewDeck, 0, 4);
			this._layout.Location = new System.Drawing.Point(0, 0);
			this._layout.Margin = new System.Windows.Forms.Padding(0);
			this._layout.Name = "_layout";
			this._layout.RowCount = 5;
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layout.Size = new System.Drawing.Size(1537, 826);
			this._layout.TabIndex = 44;
			// 
			// _layoutViewCards
			// 
			this._layoutViewCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutViewCards.BackColor = System.Drawing.Color.White;
			this._layoutViewCards.LayoutControlType = typeof(Mtgdb.Gui.CardLayout);
			layoutOptions1.AlignTopLeftHoveredIcon = global::Mtgdb.Gui.Properties.Resources.corner_hovered_32;
			layoutOptions1.AlignTopLeftIcon = global::Mtgdb.Gui.Properties.Resources.corner_32;
			layoutOptions1.AllowPartialCards = true;
			layoutOptions1.CardInterval = new System.Drawing.Size(4, 2);
			layoutOptions1.PartialCardsThreshold = new System.Drawing.Size(327, 209);
			this._layoutViewCards.LayoutOptions = layoutOptions1;
			this._layoutViewCards.Location = new System.Drawing.Point(0, 70);
			this._layoutViewCards.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewCards.Name = "_layoutViewCards";
			buttonOptions1.HotTrackOpacityDelta = 0.4F;
			buttonOptions1.Icon = global::Mtgdb.Gui.Properties.Resources.search_hovered;
			buttonOptions1.Margin = new System.Drawing.Size(0, 0);
			searchOptions1.Button = buttonOptions1;
			this._layoutViewCards.SearchOptions = searchOptions1;
			selectionOptions1.Alpha = ((byte)(192));
			selectionOptions1.BackColor = System.Drawing.Color.MediumBlue;
			selectionOptions1.ForeColor = System.Drawing.Color.White;
			selectionOptions1.HotTrackBackColor = System.Drawing.Color.WhiteSmoke;
			selectionOptions1.HotTrackBorderColor = System.Drawing.Color.Gainsboro;
			selectionOptions1.RectAlpha = ((byte)(16));
			selectionOptions1.RectBorderColor = System.Drawing.Color.MediumBlue;
			selectionOptions1.RectFillColor = System.Drawing.Color.RoyalBlue;
			this._layoutViewCards.SelectionOptions = selectionOptions1;
			this._layoutViewCards.Size = new System.Drawing.Size(1537, 419);
			sortOptions1.Allow = true;
			sortOptions1.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions1.ButtonMargin = new System.Drawing.Size(0, 0);
			sortOptions1.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions1.HotTrackOpacityDelta = 0.4F;
			sortOptions1.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewCards.SortOptions = sortOptions1;
			this._layoutViewCards.TabIndex = 19;
			this._layoutViewCards.TabStop = false;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(1538, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
			this.flowLayoutPanel1.TabIndex = 22;
			// 
			// _layoutViewDeck
			// 
			this._layoutViewDeck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutViewDeck.BackColor = System.Drawing.Color.White;
			this._layoutViewDeck.LayoutControlType = typeof(Mtgdb.Gui.DeckLayout);
			layoutOptions2.AlignTopLeftHoveredIcon = global::Mtgdb.Gui.Properties.Resources.corner_hovered_32;
			layoutOptions2.AlignTopLeftIcon = global::Mtgdb.Gui.Properties.Resources.corner_32;
			layoutOptions2.AllowPartialCards = true;
			layoutOptions2.CardInterval = new System.Drawing.Size(2, 2);
			layoutOptions2.PartialCardsThreshold = new System.Drawing.Size(150, 209);
			this._layoutViewDeck.LayoutOptions = layoutOptions2;
			this._layoutViewDeck.Location = new System.Drawing.Point(0, 513);
			this._layoutViewDeck.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewDeck.Name = "_layoutViewDeck";
			buttonOptions2.HotTrackOpacityDelta = 0.4F;
			searchOptions2.Button = buttonOptions2;
			this._layoutViewDeck.SearchOptions = searchOptions2;
			selectionOptions2.Alpha = ((byte)(255));
			selectionOptions2.BackColor = System.Drawing.Color.Empty;
			selectionOptions2.Enabled = false;
			selectionOptions2.ForeColor = System.Drawing.Color.Empty;
			selectionOptions2.HotTrackBackColor = System.Drawing.Color.WhiteSmoke;
			selectionOptions2.HotTrackBorderColor = System.Drawing.Color.Gainsboro;
			selectionOptions2.RectAlpha = ((byte)(255));
			selectionOptions2.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions2.RectFillColor = System.Drawing.Color.Empty;
			this._layoutViewDeck.SelectionOptions = selectionOptions2;
			this._layoutViewDeck.Size = new System.Drawing.Size(1537, 313);
			sortOptions2.Allow = true;
			sortOptions2.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions2.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions2.HotTrackOpacityDelta = 0.4F;
			sortOptions2.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewDeck.SortOptions = sortOptions2;
			this._layoutViewDeck.TabIndex = 42;
			this._layoutViewDeck.TabStop = false;
			// 
			// _layoutRight
			// 
			this._layoutRight.AutoSize = true;
			this._layoutRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layoutRight.ColumnCount = 2;
			this._layoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
			this._layoutRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutRight.Controls.Add(this._buttonExcludeManaAbility, 0, 1);
			this._layoutRight.Controls.Add(this._buttonExcludeManaCost, 1, 1);
			this._layoutRight.Controls.Add(this._buttonShowProhibit, 1, 0);
			this._layoutRight.Controls.Add(this._panelCostLeft, 0, 2);
			this._layoutRight.Controls.Add(this.FilterManaCost, 1, 2);
			this._layoutRight.Location = new System.Drawing.Point(1537, 0);
			this._layoutRight.Margin = new System.Windows.Forms.Padding(0);
			this._layoutRight.Name = "_layoutRight";
			this._layoutRight.RowCount = 3;
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRight.Size = new System.Drawing.Size(104, 750);
			this._layoutRight.TabIndex = 45;
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
			this._buttonExcludeManaAbility.Location = new System.Drawing.Point(1, 24);
			this._buttonExcludeManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaAbility.Name = "_buttonExcludeManaAbility";
			this._buttonExcludeManaAbility.Size = new System.Drawing.Size(23, 20);
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
			this._buttonExcludeManaCost.Location = new System.Drawing.Point(58, 24);
			this._buttonExcludeManaCost.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaCost.Name = "_buttonExcludeManaCost";
			this._buttonExcludeManaCost.Size = new System.Drawing.Size(24, 20);
			this._buttonExcludeManaCost.TabIndex = 42;
			this._buttonExcludeManaCost.TabStop = false;
			this._buttonExcludeManaCost.UseVisualStyleBackColor = false;
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
			this._buttonShowProhibit.Image = global::Mtgdb.Gui.Properties.Resources.exclude_hidden_24;
			this._buttonShowProhibit.Location = new System.Drawing.Point(80, 0);
			this._buttonShowProhibit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowProhibit.Name = "_buttonShowProhibit";
			this._buttonShowProhibit.Size = new System.Drawing.Size(24, 24);
			this._buttonShowProhibit.TabIndex = 41;
			this._buttonShowProhibit.TabStop = false;
			this._buttonShowProhibit.UseVisualStyleBackColor = false;
			// 
			// FilterManaCost
			// 
			this.FilterManaCost.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManaCost.EnableCostBehavior = true;
			this.FilterManaCost.HideProhibit = true;
			this.FilterManaCost.HintIcon = global::Mtgdb.Gui.Properties.Resources.manacost_25;
			this.FilterManaCost.HintTextShift = new System.Drawing.Size(4, 10);
			this.FilterManaCost.IsFlipped = true;
			this.FilterManaCost.IsVertical = true;
			this.FilterManaCost.Location = new System.Drawing.Point(58, 44);
			this.FilterManaCost.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this.FilterManaCost.MinimumSize = new System.Drawing.Size(46, 618);
			this.FilterManaCost.Name = "FilterManaCost";
			this.FilterManaCost.Opacity3ToDisable = 0.2F;
			this.FilterManaCost.Opacity4Disabled = 0.1F;
			this.FilterManaCost.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaCost.PropertiesCount = 28;
			this.FilterManaCost.PropertyImages = null;
			this.FilterManaCost.SelectionBorder = 1.75F;
			this.FilterManaCost.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaCost.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaCost.ShowValueHint = true;
			this.FilterManaCost.Size = new System.Drawing.Size(46, 618);
			this.FilterManaCost.Spacing = new System.Drawing.Size(2, 2);
			this.FilterManaCost.TabIndex = 22;
			this.FilterManaCost.TabStop = false;
			// 
			// _layoutRoot
			// 
			this._layoutRoot.ColumnCount = 2;
			this._layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutRoot.Controls.Add(this._layout, 0, 0);
			this._layoutRoot.Controls.Add(this._layoutRight, 1, 0);
			this._layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutRoot.Location = new System.Drawing.Point(0, 0);
			this._layoutRoot.Name = "_layoutRoot";
			this._layoutRoot.RowCount = 1;
			this._layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRoot.Size = new System.Drawing.Size(1641, 826);
			this._layoutRoot.TabIndex = 46;
			// 
			// FormMain
			// 
			this.ClientSize = new System.Drawing.Size(1641, 826);
			this.Controls.Add(this._listBoxSuggest);
			this.Controls.Add(this._layoutRoot);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
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
			this._layoutRight.ResumeLayout(false);
			this._layoutRight.PerformLayout();
			this._layoutRoot.ResumeLayout(false);
			this._layoutRoot.PerformLayout();
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
		private Controls.CustomCheckBox _buttonHideDeck;
		private Controls.CustomCheckBox _buttonHidePartialCards;
		private Controls.CustomCheckBox _buttonHideText;
		private System.Windows.Forms.TableLayoutPanel _layoutRight;
		private System.Windows.Forms.TableLayoutPanel _layoutRoot;
	}
}

