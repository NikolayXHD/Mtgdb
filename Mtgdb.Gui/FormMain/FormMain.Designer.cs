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
			this.FilterCastKeyword = new Mtgdb.Controls.QuickFilterControl();
			this.FilterType = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManager = new Mtgdb.Controls.QuickFilterControl();
			this.FilterLayout = new Mtgdb.Controls.QuickFilterControl();
			this.FilterRarity = new Mtgdb.Controls.QuickFilterControl();
			this._panelFilterButtons = new System.Windows.Forms.FlowLayoutPanel();
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
			this._panelIconStatusSort = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusSort = new System.Windows.Forms.Label();
			this._panelMenu = new System.Windows.Forms.FlowLayoutPanel();
			this._findBorderedPanel = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._panelIconSearch = new Mtgdb.Controls.BorderedPanel();
			this._findEditor = new Mtgdb.Controls.FixedRichTextBox();
			this._buttonFindExamplesDropDown = new System.Windows.Forms.CheckBox();
			this._panelIconLegality = new Mtgdb.Controls.BorderedPanel();
			this._menuLegalityFormat = new System.Windows.Forms.ComboBox();
			this._buttonLegalityAllowLegal = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowRestricted = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowBanned = new Mtgdb.Controls.CustomCheckBox();
			this._buttonShowDuplicates = new Mtgdb.Controls.CustomCheckBox();
			this._panelRightCost = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterGeneratedMana = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManaAbility = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaAbility = new Mtgdb.Controls.CustomCheckBox();
			this.FilterCmc = new Mtgdb.Controls.QuickFilterControl();
			this._listBoxSuggest = new System.Windows.Forms.ListBox();
			this._layoutMain = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._layoutRight = new System.Windows.Forms.TableLayoutPanel();
			this._panelRightNarrow = new System.Windows.Forms.FlowLayoutPanel();
			this._panelRightManaCost = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterManaCost = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaCost = new Mtgdb.Controls.CustomCheckBox();
			this._panelFilterManager = new System.Windows.Forms.FlowLayoutPanel();
			this._buttonShowProhibit = new Mtgdb.Controls.CustomCheckBox();
			this._layoutRoot = new System.Windows.Forms.TableLayoutPanel();
			this._panelFindExamples = new Mtgdb.Controls.BorderedTableLayoutPanel();
			this.label32 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.label28 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.label30 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.label37 = new System.Windows.Forms.Label();
			this.label38 = new System.Windows.Forms.Label();
			this.label39 = new System.Windows.Forms.Label();
			this.label40 = new System.Windows.Forms.Label();
			this.label41 = new System.Windows.Forms.Label();
			this.label42 = new System.Windows.Forms.Label();
			this.label43 = new System.Windows.Forms.Label();
			this.label44 = new System.Windows.Forms.Label();
			this.label45 = new System.Windows.Forms.Label();
			this.label46 = new System.Windows.Forms.Label();
			this.label47 = new System.Windows.Forms.Label();
			this.label48 = new System.Windows.Forms.Label();
			this.label49 = new System.Windows.Forms.Label();
			this.label50 = new System.Windows.Forms.Label();
			this.label51 = new System.Windows.Forms.Label();
			this.label52 = new System.Windows.Forms.Label();
			this.label53 = new System.Windows.Forms.Label();
			this.label54 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.label34 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label36 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this._layoutViewCards = new Mtgdb.Controls.LayoutViewControl();
			this._layoutViewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._panelFilters.SuspendLayout();
			this._panelFilterButtons.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._findBorderedPanel.SuspendLayout();
			this._panelRightCost.SuspendLayout();
			this._layoutMain.SuspendLayout();
			this._layoutRight.SuspendLayout();
			this._panelRightNarrow.SuspendLayout();
			this._panelRightManaCost.SuspendLayout();
			this._panelFilterManager.SuspendLayout();
			this._layoutRoot.SuspendLayout();
			this._panelFindExamples.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelFilters
			// 
			this._panelFilters.AutoSize = true;
			this._panelFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelFilters.Controls.Add(this.FilterAbility);
			this._panelFilters.Controls.Add(this.FilterCastKeyword);
			this._panelFilters.Location = new System.Drawing.Point(0, 0);
			this._panelFilters.Margin = new System.Windows.Forms.Padding(0);
			this._panelFilters.Name = "_panelFilters";
			this._panelFilters.Size = new System.Drawing.Size(1620, 34);
			this._panelFilters.TabIndex = 0;
			// 
			// FilterAbility
			// 
			this.FilterAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterAbility.EnableRequiringSome = true;
			this.FilterAbility.HideProhibit = true;
			this.FilterAbility.Location = new System.Drawing.Point(0, 0);
			this.FilterAbility.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this.FilterAbility.MinimumSize = new System.Drawing.Size(1032, 34);
			this.FilterAbility.Name = "FilterAbility";
			this.FilterAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterAbility.PropertiesCount = 64;
			this.FilterAbility.PropertyImages = null;
			this.FilterAbility.SelectionBorder = 1.75F;
			this.FilterAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterAbility.ShowValueHint = true;
			this.FilterAbility.Size = new System.Drawing.Size(1032, 34);
			this.FilterAbility.Spacing = new System.Drawing.Size(-4, -10);
			this.FilterAbility.TabIndex = 13;
			this.FilterAbility.TabStop = false;
			// 
			// FilterCastKeyword
			// 
			this.FilterCastKeyword.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterCastKeyword.EnableRequiringSome = true;
			this.FilterCastKeyword.HideProhibit = true;
			this.FilterCastKeyword.Location = new System.Drawing.Point(1042, 0);
			this.FilterCastKeyword.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this.FilterCastKeyword.MinimumSize = new System.Drawing.Size(568, 34);
			this.FilterCastKeyword.Name = "FilterCastKeyword";
			this.FilterCastKeyword.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterCastKeyword.PropertiesCount = 35;
			this.FilterCastKeyword.PropertyImages = null;
			this.FilterCastKeyword.SelectionBorder = 1.75F;
			this.FilterCastKeyword.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterCastKeyword.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterCastKeyword.ShowValueHint = true;
			this.FilterCastKeyword.Size = new System.Drawing.Size(568, 34);
			this.FilterCastKeyword.Spacing = new System.Drawing.Size(-4, -10);
			this.FilterCastKeyword.TabIndex = 18;
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
			this.FilterType.MinimumSize = new System.Drawing.Size(44, 178);
			this.FilterType.Name = "FilterType";
			this.FilterType.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterType.PropertiesCount = 8;
			this.FilterType.PropertyImages = null;
			this.FilterType.SelectionBorder = 1.75F;
			this.FilterType.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterType.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterType.ShowValueHint = true;
			this.FilterType.Size = new System.Drawing.Size(44, 178);
			this.FilterType.Spacing = new System.Drawing.Size(2, 0);
			this.FilterType.TabIndex = 14;
			this.FilterType.TabStop = false;
			// 
			// FilterManager
			// 
			this.FilterManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FilterManager.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterManager.EnableRequiringSome = true;
			this.FilterManager.HideProhibit = true;
			this.FilterManager.Location = new System.Drawing.Point(0, 24);
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
			// FilterLayout
			// 
			this.FilterLayout.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterLayout.EnableCostBehavior = true;
			this.FilterLayout.EnableMutuallyExclusive = true;
			this.FilterLayout.HideProhibit = true;
			this.FilterLayout.IsFlipped = true;
			this.FilterLayout.IsVertical = true;
			this.FilterLayout.Location = new System.Drawing.Point(0, 144);
			this.FilterLayout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.FilterLayout.MinimumSize = new System.Drawing.Size(24, 266);
			this.FilterLayout.Name = "FilterLayout";
			this.FilterLayout.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterLayout.PropertiesCount = 12;
			this.FilterLayout.PropertyImages = null;
			this.FilterLayout.SelectionBorder = 1.75F;
			this.FilterLayout.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterLayout.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterLayout.ShowValueHint = true;
			this.FilterLayout.Size = new System.Drawing.Size(24, 266);
			this.FilterLayout.Spacing = new System.Drawing.Size(2, 0);
			this.FilterLayout.TabIndex = 17;
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
			this.FilterRarity.MinimumSize = new System.Drawing.Size(24, 134);
			this.FilterRarity.Name = "FilterRarity";
			this.FilterRarity.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterRarity.PropertiesCount = 6;
			this.FilterRarity.PropertyImages = null;
			this.FilterRarity.SelectionBorder = 1.75F;
			this.FilterRarity.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterRarity.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterRarity.ShowValueHint = true;
			this.FilterRarity.Size = new System.Drawing.Size(24, 134);
			this.FilterRarity.Spacing = new System.Drawing.Size(2, 0);
			this.FilterRarity.TabIndex = 15;
			this.FilterRarity.TabStop = false;
			// 
			// _panelFilterButtons
			// 
			this._panelFilterButtons.AutoSize = true;
			this._panelFilterButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelFilterButtons.Controls.Add(this._buttonHideDeck);
			this._panelFilterButtons.Controls.Add(this._buttonHidePartialCards);
			this._panelFilterButtons.Controls.Add(this._buttonHideText);
			this._panelFilterButtons.Controls.Add(this._tabHeadersDeck);
			this._panelFilterButtons.Controls.Add(this._buttonSampleHandNew);
			this._panelFilterButtons.Controls.Add(this._buttonSampleHandMulligan);
			this._panelFilterButtons.Controls.Add(this._buttonSampleHandDraw);
			this._panelFilterButtons.Controls.Add(this._labelStatusScrollDeck);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusScrollDeck);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusSets);
			this._panelFilterButtons.Controls.Add(this._labelStatusSets);
			this._panelFilterButtons.Controls.Add(this._labelStatusScrollCards);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusScrollCards);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusCollection);
			this._panelFilterButtons.Controls.Add(this._labelStatusCollection);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusFilterButtons);
			this._panelFilterButtons.Controls.Add(this._labelStatusFilterButtons);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusSearch);
			this._panelFilterButtons.Controls.Add(this._labelStatusSearch);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusFilterCollection);
			this._panelFilterButtons.Controls.Add(this._labelStatusFilterCollection);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusFilterDeck);
			this._panelFilterButtons.Controls.Add(this._labelStatusFilterDeck);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusFilterLegality);
			this._panelFilterButtons.Controls.Add(this._labelStatusFilterLegality);
			this._panelFilterButtons.Controls.Add(this._panelIconStatusSort);
			this._panelFilterButtons.Controls.Add(this._labelStatusSort);
			this._panelFilterButtons.Location = new System.Drawing.Point(0, 470);
			this._panelFilterButtons.Margin = new System.Windows.Forms.Padding(0);
			this._panelFilterButtons.Name = "_panelFilterButtons";
			this._panelFilterButtons.Size = new System.Drawing.Size(1558, 43);
			this._panelFilterButtons.TabIndex = 14;
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
			// _panelIconStatusSort
			// 
			this._panelIconStatusSort.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.sort_48;
			this._panelIconStatusSort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusSort.Location = new System.Drawing.Point(1534, 0);
			this._panelIconStatusSort.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSort.Name = "_panelIconStatusSort";
			this._panelIconStatusSort.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSort.TabIndex = 35;
			this._panelIconStatusSort.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _labelStatusSort
			// 
			this._labelStatusSort.AutoSize = true;
			this._labelStatusSort.Location = new System.Drawing.Point(0, 30);
			this._labelStatusSort.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSort.Name = "_labelStatusSort";
			this._labelStatusSort.Size = new System.Drawing.Size(78, 13);
			this._labelStatusSort.TabIndex = 50;
			this._labelStatusSort.Text = "ReleaseDate ˄";
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
			this._panelMenu.Location = new System.Drawing.Point(0, 34);
			this._panelMenu.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(1028, 24);
			this._panelMenu.TabIndex = 10;
			// 
			// _findBorderedPanel
			// 
			this._findBorderedPanel.BackColor = System.Drawing.Color.White;
			this._findBorderedPanel.Controls.Add(this._panelIconSearch);
			this._findBorderedPanel.Controls.Add(this._findEditor);
			this._findBorderedPanel.Controls.Add(this._buttonFindExamplesDropDown);
			this._findBorderedPanel.Location = new System.Drawing.Point(0, 0);
			this._findBorderedPanel.Margin = new System.Windows.Forms.Padding(0);
			this._findBorderedPanel.Name = "_findBorderedPanel";
			this._findBorderedPanel.Size = new System.Drawing.Size(679, 24);
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
			this._findEditor.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._findEditor.Location = new System.Drawing.Point(24, 5);
			this._findEditor.Margin = new System.Windows.Forms.Padding(0, 5, 0, 1);
			this._findEditor.Multiline = false;
			this._findEditor.Name = "_findEditor";
			this._findEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this._findEditor.Size = new System.Drawing.Size(630, 18);
			this._findEditor.TabIndex = 20;
			this._findEditor.TabStop = false;
			this._findEditor.Text = "";
			this._findEditor.WordWrap = false;
			// 
			// _buttonFindExamplesDropDown
			// 
			this._buttonFindExamplesDropDown.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonFindExamplesDropDown.BackColor = System.Drawing.Color.Transparent;
			this._buttonFindExamplesDropDown.FlatAppearance.BorderSize = 0;
			this._buttonFindExamplesDropDown.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonFindExamplesDropDown.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonFindExamplesDropDown.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonFindExamplesDropDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonFindExamplesDropDown.Image = global::Mtgdb.Gui.Properties.Resources.book_40;
			this._buttonFindExamplesDropDown.Location = new System.Drawing.Point(655, 1);
			this._buttonFindExamplesDropDown.Margin = new System.Windows.Forms.Padding(1);
			this._buttonFindExamplesDropDown.Name = "_buttonFindExamplesDropDown";
			this._buttonFindExamplesDropDown.Size = new System.Drawing.Size(22, 22);
			this._buttonFindExamplesDropDown.TabIndex = 22;
			this._buttonFindExamplesDropDown.TabStop = false;
			this._buttonFindExamplesDropDown.UseVisualStyleBackColor = false;
			// 
			// _panelIconLegality
			// 
			this._panelIconLegality.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.legality_48;
			this._panelIconLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconLegality.Location = new System.Drawing.Point(681, 0);
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
			this._menuLegalityFormat.Location = new System.Drawing.Point(707, 2);
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
			this._buttonLegalityAllowLegal.Location = new System.Drawing.Point(832, 4);
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
			this._buttonLegalityAllowRestricted.Location = new System.Drawing.Point(877, 4);
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
			this._buttonLegalityAllowBanned.Location = new System.Drawing.Point(943, 4);
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
			this._buttonShowDuplicates.Location = new System.Drawing.Point(1004, 0);
			this._buttonShowDuplicates.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonShowDuplicates.Name = "_buttonShowDuplicates";
			this._buttonShowDuplicates.Size = new System.Drawing.Size(24, 24);
			this._buttonShowDuplicates.TabIndex = 40;
			this._buttonShowDuplicates.TabStop = false;
			this._buttonShowDuplicates.UseVisualStyleBackColor = false;
			// 
			// _panelRightCost
			// 
			this._panelRightCost.AutoSize = true;
			this._panelRightCost.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelRightCost.Controls.Add(this.FilterType);
			this._panelRightCost.Controls.Add(this.FilterGeneratedMana);
			this._panelRightCost.Controls.Add(this.FilterManaAbility);
			this._panelRightCost.Controls.Add(this._buttonExcludeManaAbility);
			this._panelRightCost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelRightCost.Location = new System.Drawing.Point(44, 0);
			this._panelRightCost.Margin = new System.Windows.Forms.Padding(0);
			this._panelRightCost.Name = "_panelRightCost";
			this._panelRightCost.Size = new System.Drawing.Size(44, 706);
			this._panelRightCost.TabIndex = 0;
			// 
			// FilterGeneratedMana
			// 
			this.FilterGeneratedMana.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterGeneratedMana.EnableRequiringSome = true;
			this.FilterGeneratedMana.HideProhibit = true;
			this.FilterGeneratedMana.IsFlipped = true;
			this.FilterGeneratedMana.IsVertical = true;
			this.FilterGeneratedMana.Location = new System.Drawing.Point(0, 188);
			this.FilterGeneratedMana.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this.FilterGeneratedMana.MinimumSize = new System.Drawing.Size(44, 222);
			this.FilterGeneratedMana.Name = "FilterGeneratedMana";
			this.FilterGeneratedMana.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterGeneratedMana.PropertiesCount = 10;
			this.FilterGeneratedMana.PropertyImages = null;
			this.FilterGeneratedMana.SelectionBorder = 1.75F;
			this.FilterGeneratedMana.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterGeneratedMana.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterGeneratedMana.ShowValueHint = true;
			this.FilterGeneratedMana.Size = new System.Drawing.Size(44, 222);
			this.FilterGeneratedMana.Spacing = new System.Drawing.Size(2, 0);
			this.FilterGeneratedMana.TabIndex = 20;
			this.FilterGeneratedMana.TabStop = false;
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
			this.FilterManaAbility.Location = new System.Drawing.Point(0, 420);
			this.FilterManaAbility.Margin = new System.Windows.Forms.Padding(0);
			this.FilterManaAbility.MaximumSize = new System.Drawing.Size(50, 200);
			this.FilterManaAbility.MinimumSize = new System.Drawing.Size(44, 266);
			this.FilterManaAbility.Name = "FilterManaAbility";
			this.FilterManaAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaAbility.PropertiesCount = 12;
			this.FilterManaAbility.PropertyImages = null;
			this.FilterManaAbility.SelectionBorder = 1.75F;
			this.FilterManaAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaAbility.ShowValueHint = true;
			this.FilterManaAbility.Size = new System.Drawing.Size(44, 266);
			this.FilterManaAbility.Spacing = new System.Drawing.Size(2, 0);
			this.FilterManaAbility.TabIndex = 19;
			this.FilterManaAbility.TabStop = false;
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
			this._buttonExcludeManaAbility.Location = new System.Drawing.Point(0, 686);
			this._buttonExcludeManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaAbility.Name = "_buttonExcludeManaAbility";
			this._buttonExcludeManaAbility.Size = new System.Drawing.Size(22, 20);
			this._buttonExcludeManaAbility.TabIndex = 41;
			this._buttonExcludeManaAbility.TabStop = false;
			this._buttonExcludeManaAbility.UseVisualStyleBackColor = false;
			// 
			// FilterCmc
			// 
			this.FilterCmc.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this.FilterCmc.EnableCostBehavior = true;
			this.FilterCmc.EnableMutuallyExclusive = true;
			this.FilterCmc.HideProhibit = true;
			this.FilterCmc.IsFlipped = true;
			this.FilterCmc.IsVertical = true;
			this.FilterCmc.Location = new System.Drawing.Point(0, 420);
			this.FilterCmc.Margin = new System.Windows.Forms.Padding(0);
			this.FilterCmc.MinimumSize = new System.Drawing.Size(24, 178);
			this.FilterCmc.Name = "FilterCmc";
			this.FilterCmc.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterCmc.PropertiesCount = 8;
			this.FilterCmc.PropertyImages = null;
			this.FilterCmc.SelectionBorder = 1.75F;
			this.FilterCmc.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterCmc.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterCmc.Size = new System.Drawing.Size(24, 178);
			this.FilterCmc.Spacing = new System.Drawing.Size(2, 0);
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
			// _layoutMain
			// 
			this._layoutMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutMain.ColumnCount = 3;
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutMain.Controls.Add(this._layoutViewCards, 0, 2);
			this._layoutMain.Controls.Add(this._panelFilters, 0, 0);
			this._layoutMain.Controls.Add(this._panelMenu, 0, 1);
			this._layoutMain.Controls.Add(this._panelFilterButtons, 0, 3);
			this._layoutMain.Controls.Add(this.flowLayoutPanel1, 1, 0);
			this._layoutMain.Controls.Add(this._layoutViewDeck, 0, 4);
			this._layoutMain.Location = new System.Drawing.Point(0, 0);
			this._layoutMain.Margin = new System.Windows.Forms.Padding(0);
			this._layoutMain.Name = "_layoutMain";
			this._layoutMain.RowCount = 5;
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.Size = new System.Drawing.Size(1626, 826);
			this._layoutMain.TabIndex = 44;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(1627, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
			this.flowLayoutPanel1.TabIndex = 22;
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
			this._layoutRight.Controls.Add(this._panelRightNarrow, 2, 0);
			this._layoutRight.Controls.Add(this._panelRightManaCost, 0, 0);
			this._layoutRight.Controls.Add(this._panelRightCost, 1, 0);
			this._layoutRight.Controls.Add(this._panelFilterManager, 0, 1);
			this._layoutRight.Location = new System.Drawing.Point(1626, 0);
			this._layoutRight.Margin = new System.Windows.Forms.Padding(0);
			this._layoutRight.Name = "_layoutRight";
			this._layoutRight.RowCount = 2;
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutRight.Size = new System.Drawing.Size(112, 826);
			this._layoutRight.TabIndex = 45;
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
			this._panelRightNarrow.Size = new System.Drawing.Size(24, 598);
			this._panelRightNarrow.TabIndex = 43;
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
			this._panelRightManaCost.Size = new System.Drawing.Size(44, 638);
			this._panelRightManaCost.TabIndex = 44;
			this._panelRightManaCost.WrapContents = false;
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
			this.FilterManaCost.MinimumSize = new System.Drawing.Size(44, 618);
			this.FilterManaCost.Name = "FilterManaCost";
			this.FilterManaCost.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaCost.PropertiesCount = 28;
			this.FilterManaCost.PropertyImages = null;
			this.FilterManaCost.SelectionBorder = 1.75F;
			this.FilterManaCost.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaCost.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaCost.ShowValueHint = true;
			this.FilterManaCost.Size = new System.Drawing.Size(44, 618);
			this.FilterManaCost.Spacing = new System.Drawing.Size(2, 0);
			this.FilterManaCost.TabIndex = 22;
			this.FilterManaCost.TabStop = false;
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
			this._buttonExcludeManaCost.Location = new System.Drawing.Point(0, 618);
			this._buttonExcludeManaCost.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaCost.Name = "_buttonExcludeManaCost";
			this._buttonExcludeManaCost.Size = new System.Drawing.Size(22, 20);
			this._buttonExcludeManaCost.TabIndex = 42;
			this._buttonExcludeManaCost.TabStop = false;
			this._buttonExcludeManaCost.UseVisualStyleBackColor = false;
			// 
			// _panelFilterManager
			// 
			this._panelFilterManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._panelFilterManager.AutoSize = true;
			this._panelFilterManager.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layoutRight.SetColumnSpan(this._panelFilterManager, 3);
			this._panelFilterManager.Controls.Add(this._buttonShowProhibit);
			this._panelFilterManager.Controls.Add(this.FilterManager);
			this._panelFilterManager.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelFilterManager.Location = new System.Drawing.Point(0, 756);
			this._panelFilterManager.Margin = new System.Windows.Forms.Padding(0);
			this._panelFilterManager.Name = "_panelFilterManager";
			this._panelFilterManager.Size = new System.Drawing.Size(112, 70);
			this._panelFilterManager.TabIndex = 45;
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
			this._buttonShowProhibit.Location = new System.Drawing.Point(88, 0);
			this._buttonShowProhibit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowProhibit.Name = "_buttonShowProhibit";
			this._buttonShowProhibit.Size = new System.Drawing.Size(24, 24);
			this._buttonShowProhibit.TabIndex = 41;
			this._buttonShowProhibit.TabStop = false;
			this._buttonShowProhibit.UseVisualStyleBackColor = false;
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
			this._layoutRoot.Size = new System.Drawing.Size(1738, 826);
			this._layoutRoot.TabIndex = 46;
			// 
			// _panelFindExamples
			// 
			this._panelFindExamples.BackColor = System.Drawing.Color.White;
			this._panelFindExamples.ColumnCount = 2;
			this._panelFindExamples.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
			this._panelFindExamples.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64F));
			this._panelFindExamples.Controls.Add(this.label32, 0, 27);
			this._panelFindExamples.Controls.Add(this.label9, 1, 4);
			this._panelFindExamples.Controls.Add(this.label8, 1, 3);
			this._panelFindExamples.Controls.Add(this.label7, 1, 2);
			this._panelFindExamples.Controls.Add(this.label3, 0, 2);
			this._panelFindExamples.Controls.Add(this.label2, 1, 1);
			this._panelFindExamples.Controls.Add(this.label1, 0, 1);
			this._panelFindExamples.Controls.Add(this.label4, 0, 3);
			this._panelFindExamples.Controls.Add(this.label5, 0, 4);
			this._panelFindExamples.Controls.Add(this.label11, 0, 0);
			this._panelFindExamples.Controls.Add(this.label12, 0, 5);
			this._panelFindExamples.Controls.Add(this.label15, 0, 9);
			this._panelFindExamples.Controls.Add(this.label17, 0, 8);
			this._panelFindExamples.Controls.Add(this.label18, 0, 10);
			this._panelFindExamples.Controls.Add(this.label19, 0, 12);
			this._panelFindExamples.Controls.Add(this.label20, 0, 13);
			this._panelFindExamples.Controls.Add(this.label21, 0, 14);
			this._panelFindExamples.Controls.Add(this.label23, 0, 15);
			this._panelFindExamples.Controls.Add(this.label22, 0, 16);
			this._panelFindExamples.Controls.Add(this.label24, 0, 17);
			this._panelFindExamples.Controls.Add(this.label25, 0, 18);
			this._panelFindExamples.Controls.Add(this.label26, 0, 19);
			this._panelFindExamples.Controls.Add(this.label27, 0, 20);
			this._panelFindExamples.Controls.Add(this.label28, 0, 21);
			this._panelFindExamples.Controls.Add(this.label6, 0, 22);
			this._panelFindExamples.Controls.Add(this.label10, 0, 23);
			this._panelFindExamples.Controls.Add(this.label29, 0, 24);
			this._panelFindExamples.Controls.Add(this.label30, 0, 25);
			this._panelFindExamples.Controls.Add(this.label31, 0, 26);
			this._panelFindExamples.Controls.Add(this.label37, 1, 9);
			this._panelFindExamples.Controls.Add(this.label38, 1, 10);
			this._panelFindExamples.Controls.Add(this.label39, 1, 11);
			this._panelFindExamples.Controls.Add(this.label40, 1, 13);
			this._panelFindExamples.Controls.Add(this.label41, 1, 14);
			this._panelFindExamples.Controls.Add(this.label42, 1, 15);
			this._panelFindExamples.Controls.Add(this.label43, 1, 16);
			this._panelFindExamples.Controls.Add(this.label44, 1, 17);
			this._panelFindExamples.Controls.Add(this.label45, 1, 18);
			this._panelFindExamples.Controls.Add(this.label46, 1, 19);
			this._panelFindExamples.Controls.Add(this.label47, 1, 20);
			this._panelFindExamples.Controls.Add(this.label48, 1, 21);
			this._panelFindExamples.Controls.Add(this.label49, 1, 22);
			this._panelFindExamples.Controls.Add(this.label50, 1, 23);
			this._panelFindExamples.Controls.Add(this.label51, 1, 24);
			this._panelFindExamples.Controls.Add(this.label52, 1, 25);
			this._panelFindExamples.Controls.Add(this.label53, 1, 26);
			this._panelFindExamples.Controls.Add(this.label54, 1, 27);
			this._panelFindExamples.Controls.Add(this.label13, 0, 7);
			this._panelFindExamples.Controls.Add(this.label14, 0, 6);
			this._panelFindExamples.Controls.Add(this.label33, 1, 7);
			this._panelFindExamples.Controls.Add(this.label34, 1, 6);
			this._panelFindExamples.Controls.Add(this.label16, 0, 11);
			this._panelFindExamples.Controls.Add(this.label36, 1, 8);
			this._panelFindExamples.Controls.Add(this.label35, 1, 5);
			this._panelFindExamples.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._panelFindExamples.Location = new System.Drawing.Point(238, 84);
			this._panelFindExamples.Margin = new System.Windows.Forms.Padding(0, 1, 3, 3);
			this._panelFindExamples.Name = "_panelFindExamples";
			this._panelFindExamples.RowCount = 28;
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.265906F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631509F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631509F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631508F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631508F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.265906F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.628961F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 18.41849F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.265906F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631212F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.262424F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631483F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631481F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631481F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631481F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631481F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.631481F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._panelFindExamples.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._panelFindExamples.Size = new System.Drawing.Size(673, 726);
			this._panelFindExamples.TabIndex = 47;
			this._panelFindExamples.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// label32
			// 
			this.label32.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label32.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label32.Location = new System.Drawing.Point(1, 703);
			this.label32.Margin = new System.Windows.Forms.Padding(1, 0, 0, 1);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(241, 22);
			this.label32.TabIndex = 32;
			this.label32.Text = "angel^3 OR demon";
			// 
			// label9
			// 
			this.label9.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label9.Location = new System.Drawing.Point(242, 95);
			this.label9.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(430, 19);
			this.label9.TabIndex = 8;
			this.label9.Text = "Both words must be present, each in any field in any order";
			// 
			// label8
			// 
			this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label8.Location = new System.Drawing.Point(242, 76);
			this.label8.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(430, 19);
			this.label8.TabIndex = 7;
			this.label8.Text = "Restricts the search to Name field only";
			// 
			// label7
			// 
			this.label7.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label7.Location = new System.Drawing.Point(242, 57);
			this.label7.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(430, 19);
			this.label7.TabIndex = 6;
			this.label7.Text = "Contains both words in exactly same order in some field";
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(1, 57);
			this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(241, 19);
			this.label3.TabIndex = 2;
			this.label3.Text = "\"llanowar elves\"";
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label2.Location = new System.Drawing.Point(242, 38);
			this.label2.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(430, 19);
			this.label2.TabIndex = 1;
			this.label2.Text = "Has either llanowar OR elves in any field";
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(1, 38);
			this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(241, 19);
			this.label1.TabIndex = 0;
			this.label1.Text = "llanowar elves\r\n";
			// 
			// label4
			// 
			this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label4.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label4.Location = new System.Drawing.Point(1, 76);
			this.label4.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(241, 19);
			this.label4.TabIndex = 3;
			this.label4.Text = "name:\"llanowar elves\"";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label5.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label5.Location = new System.Drawing.Point(1, 95);
			this.label5.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(241, 19);
			this.label5.TabIndex = 4;
			this.label5.Text = "llanowar AND elves";
			// 
			// label11
			// 
			this._panelFindExamples.SetColumnSpan(this.label11, 2);
			this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label11.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label11.Location = new System.Drawing.Point(1, 1);
			this.label11.Margin = new System.Windows.Forms.Padding(1, 1, 1, 0);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(671, 37);
			this.label11.TabIndex = 10;
			this.label11.Text = "Basic examples";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label12
			// 
			this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label12.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label12.Location = new System.Drawing.Point(1, 114);
			this.label12.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(240, 38);
			this.label12.TabIndex = 11;
			this.label12.Text = "Caveats";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label15
			// 
			this.label15.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label15.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label15.Location = new System.Drawing.Point(1, 342);
			this.label15.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(241, 19);
			this.label15.TabIndex = 14;
			this.label15.Text = "like:displace";
			// 
			// label17
			// 
			this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label17.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label17.Location = new System.Drawing.Point(1, 304);
			this.label17.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(240, 38);
			this.label17.TabIndex = 16;
			this.label17.Text = "Search similar cards";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label18
			// 
			this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label18.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label18.Location = new System.Drawing.Point(1, 361);
			this.label18.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(241, 19);
			this.label18.TabIndex = 17;
			this.label18.Text = "like:\"thalia\'s lieutenant\"";
			// 
			// label19
			// 
			this._panelFindExamples.SetColumnSpan(this.label19, 2);
			this.label19.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label19.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label19.Location = new System.Drawing.Point(1, 399);
			this.label19.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(671, 38);
			this.label19.TabIndex = 18;
			this.label19.Text = "More syntax details";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label20
			// 
			this.label20.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label20.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label20.Location = new System.Drawing.Point(1, 437);
			this.label20.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(241, 19);
			this.label20.TabIndex = 19;
			this.label20.Text = "((angel OR demon) AND legendary)";
			// 
			// label21
			// 
			this.label21.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label21.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label21.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label21.Location = new System.Drawing.Point(1, 456);
			this.label21.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(241, 19);
			this.label21.TabIndex = 20;
			this.label21.Text = "name:ooze";
			// 
			// label23
			// 
			this.label23.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label23.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label23.Location = new System.Drawing.Point(1, 475);
			this.label23.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(241, 19);
			this.label23.TabIndex = 22;
			this.label23.Text = "disk";
			// 
			// label22
			// 
			this.label22.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label22.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label22.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label22.Location = new System.Drawing.Point(1, 494);
			this.label22.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(241, 19);
			this.label22.TabIndex = 21;
			this.label22.Text = "\"discard your hand\"";
			// 
			// label24
			// 
			this.label24.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label24.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label24.Location = new System.Drawing.Point(1, 513);
			this.label24.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(241, 19);
			this.label24.TabIndex = 23;
			this.label24.Text = "type:(rogue rat)";
			// 
			// label25
			// 
			this.label25.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label25.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label25.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label25.Location = new System.Drawing.Point(1, 532);
			this.label25.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(241, 19);
			this.label25.TabIndex = 24;
			this.label25.Text = "subtypes:(*ngel OR dem* OR human)";
			// 
			// label26
			// 
			this.label26.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label26.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label26.Location = new System.Drawing.Point(1, 551);
			this.label26.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(241, 19);
			this.label26.TabIndex = 25;
			this.label26.Text = "su????*";
			// 
			// label27
			// 
			this.label27.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label27.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label27.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label27.Location = new System.Drawing.Point(1, 570);
			this.label27.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(241, 19);
			this.label27.TabIndex = 26;
			this.label27.Text = "nameen:/[ab]nge.{1,2}|demon/";
			// 
			// label28
			// 
			this.label28.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label28.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label28.Location = new System.Drawing.Point(1, 589);
			this.label28.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(241, 19);
			this.label28.TabIndex = 27;
			this.label28.Text = "neviniral~";
			// 
			// label6
			// 
			this.label6.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label6.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label6.Location = new System.Drawing.Point(1, 608);
			this.label6.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(241, 19);
			this.label6.TabIndex = 5;
			this.label6.Text = "neviniral~0.2";
			// 
			// label10
			// 
			this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label10.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label10.Location = new System.Drawing.Point(1, 627);
			this.label10.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(241, 19);
			this.label10.TabIndex = 28;
			this.label10.Text = "\"mana color\"~2";
			// 
			// label29
			// 
			this.label29.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label29.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label29.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label29.Location = new System.Drawing.Point(1, 646);
			this.label29.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(241, 19);
			this.label29.TabIndex = 29;
			this.label29.Text = "name:[a TO ced]";
			// 
			// label30
			// 
			this.label30.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label30.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label30.Location = new System.Drawing.Point(1, 665);
			this.label30.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(241, 19);
			this.label30.TabIndex = 30;
			this.label30.Text = "pricingmid:{100 TO *}";
			// 
			// label31
			// 
			this.label31.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label31.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label31.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label31.Location = new System.Drawing.Point(1, 684);
			this.label31.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(241, 19);
			this.label31.TabIndex = 31;
			this.label31.Text = "cmc:{0 TO 2]";
			// 
			// label37
			// 
			this.label37.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label37.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label37.Location = new System.Drawing.Point(242, 342);
			this.label37.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(430, 19);
			this.label37.TabIndex = 37;
			this.label37.Text = "Find cards with similar Text or GeneratedMana";
			// 
			// label38
			// 
			this.label38.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label38.Location = new System.Drawing.Point(242, 361);
			this.label38.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(430, 19);
			this.label38.TabIndex = 38;
			this.label38.Text = "Name containing whitespace must be quoted";
			// 
			// label39
			// 
			this.label39.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label39.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label39.Location = new System.Drawing.Point(242, 380);
			this.label39.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label39.Name = "label39";
			this.label39.Size = new System.Drawing.Size(430, 19);
			this.label39.TabIndex = 39;
			this.label39.Text = "Set min similarity, default is 0.6";
			// 
			// label40
			// 
			this.label40.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label40.Location = new System.Drawing.Point(242, 437);
			this.label40.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(430, 19);
			this.label40.TabIndex = 40;
			this.label40.Text = "Boolean operators can be nested";
			// 
			// label41
			// 
			this.label41.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label41.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label41.Location = new System.Drawing.Point(242, 456);
			this.label41.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(430, 19);
			this.label41.TabIndex = 41;
			this.label41.Text = "Search in a specific field";
			// 
			// label42
			// 
			this.label42.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label42.Location = new System.Drawing.Point(242, 475);
			this.label42.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(430, 19);
			this.label42.TabIndex = 42;
			this.label42.Text = "Searh in any field";
			// 
			// label43
			// 
			this.label43.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label43.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label43.Location = new System.Drawing.Point(242, 494);
			this.label43.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(430, 19);
			this.label43.TabIndex = 43;
			this.label43.Text = "Search whole phrase";
			// 
			// label44
			// 
			this.label44.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label44.Location = new System.Drawing.Point(242, 513);
			this.label44.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(430, 19);
			this.label44.TabIndex = 44;
			this.label44.Text = "Cards of types Rogue OR Rat because default operator is OR";
			// 
			// label45
			// 
			this.label45.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label45.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label45.Location = new System.Drawing.Point(242, 532);
			this.label45.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(430, 19);
			this.label45.TabIndex = 45;
			this.label45.Text = "* means 0 or more characters";
			// 
			// label46
			// 
			this.label46.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label46.Location = new System.Drawing.Point(242, 551);
			this.label46.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label46.Name = "label46";
			this.label46.Size = new System.Drawing.Size(430, 19);
			this.label46.TabIndex = 46;
			this.label46.Text = "? means any one character, can be used to set minimum length";
			// 
			// label47
			// 
			this.label47.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label47.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label47.Location = new System.Drawing.Point(242, 570);
			this.label47.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label47.Name = "label47";
			this.label47.Size = new System.Drawing.Size(430, 19);
			this.label47.TabIndex = 47;
			this.label47.Text = "Regular expressions (lucene dialect) are delimited by /";
			// 
			// label48
			// 
			this.label48.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label48.Location = new System.Drawing.Point(242, 589);
			this.label48.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label48.Name = "label48";
			this.label48.Size = new System.Drawing.Size(430, 19);
			this.label48.TabIndex = 48;
			this.label48.Text = "Approximate spelling, searches nevinYrral and so on";
			// 
			// label49
			// 
			this.label49.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label49.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label49.Location = new System.Drawing.Point(242, 608);
			this.label49.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label49.Name = "label49";
			this.label49.Size = new System.Drawing.Size(430, 19);
			this.label49.TabIndex = 49;
			this.label49.Text = "Set min similarity, default is 0.5, valid is between 0 and 1";
			// 
			// label50
			// 
			this.label50.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label50.Location = new System.Drawing.Point(242, 627);
			this.label50.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label50.Name = "label50";
			this.label50.Size = new System.Drawing.Size(430, 19);
			this.label50.TabIndex = 50;
			this.label50.Text = "Words mana and color have 2 or less words between them";
			// 
			// label51
			// 
			this.label51.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label51.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label51.Location = new System.Drawing.Point(242, 646);
			this.label51.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label51.Name = "label51";
			this.label51.Size = new System.Drawing.Size(430, 19);
			this.label51.TabIndex = 51;
			this.label51.Text = "A word between \'a\' and \'ced\' in alphabet order";
			// 
			// label52
			// 
			this.label52.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label52.Location = new System.Drawing.Point(242, 665);
			this.label52.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label52.Name = "label52";
			this.label52.Size = new System.Drawing.Size(430, 19);
			this.label52.TabIndex = 52;
			this.label52.Text = "Cards with price strictly > than 100$";
			// 
			// label53
			// 
			this.label53.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label53.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label53.Location = new System.Drawing.Point(242, 684);
			this.label53.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label53.Name = "label53";
			this.label53.Size = new System.Drawing.Size(430, 19);
			this.label53.TabIndex = 53;
			this.label53.Text = "Boundary types are {} non-inclusive, [] inclusive";
			// 
			// label54
			// 
			this.label54.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label54.Location = new System.Drawing.Point(242, 703);
			this.label54.Margin = new System.Windows.Forms.Padding(0, 0, 1, 1);
			this.label54.Name = "label54";
			this.label54.Size = new System.Drawing.Size(430, 22);
			this.label54.TabIndex = 54;
			this.label54.Text = "Booster affects result order favoring angel by a factor of 3";
			// 
			// label13
			// 
			this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label13.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label13.Location = new System.Drawing.Point(1, 171);
			this.label13.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(241, 133);
			this.label13.TabIndex = 12;
			this.label13.Text = "name: shivan dragon";
			// 
			// label14
			// 
			this.label14.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label14.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label14.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label14.Location = new System.Drawing.Point(1, 152);
			this.label14.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(241, 19);
			this.label14.TabIndex = 13;
			this.label14.Text = "angel";
			// 
			// label33
			// 
			this.label33.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label33.Location = new System.Drawing.Point(242, 171);
			this.label33.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(430, 133);
			this.label33.TabIndex = 33;
			this.label33.Text = resources.GetString("label33.Text");
			// 
			// label34
			// 
			this.label34.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label34.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label34.Location = new System.Drawing.Point(242, 152);
			this.label34.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(430, 19);
			this.label34.TabIndex = 34;
			this.label34.Text = "Will not match angelic. Use wildcards * and ? e.g. angel*";
			// 
			// label16
			// 
			this.label16.BackColor = System.Drawing.Color.WhiteSmoke;
			this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label16.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label16.Location = new System.Drawing.Point(1, 380);
			this.label16.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(241, 19);
			this.label16.TabIndex = 15;
			this.label16.Text = "like:\"predator ooze\"~0.75";
			// 
			// label36
			// 
			this.label36.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label36.Location = new System.Drawing.Point(242, 304);
			this.label36.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(430, 38);
			this.label36.TabIndex = 36;
			this.label36.Text = "(the button on top-right corner of card image does the same)";
			this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label35
			// 
			this.label35.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label35.Location = new System.Drawing.Point(242, 114);
			this.label35.Margin = new System.Windows.Forms.Padding(0, 0, 1, 0);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(430, 38);
			this.label35.TabIndex = 35;
			this.label35.Text = "Search is case-INsensitive, AND OR NOT must be uppercase";
			this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
			this._layoutViewCards.Location = new System.Drawing.Point(0, 58);
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
			this._layoutViewCards.Size = new System.Drawing.Size(1626, 412);
			sortOptions1.Allow = true;
			sortOptions1.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions1.ButtonMargin = new System.Drawing.Size(0, 0);
			sortOptions1.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions1.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewCards.SortOptions = sortOptions1;
			this._layoutViewCards.TabIndex = 19;
			this._layoutViewCards.TabStop = false;
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
			this._layoutViewDeck.Size = new System.Drawing.Size(1626, 313);
			sortOptions2.Allow = true;
			sortOptions2.AscIcon = global::Mtgdb.Gui.Properties.Resources.sort_asc_hovered;
			sortOptions2.DescIcon = global::Mtgdb.Gui.Properties.Resources.sort_desc_hovered;
			sortOptions2.Icon = global::Mtgdb.Gui.Properties.Resources.sort_none_hovered;
			this._layoutViewDeck.SortOptions = sortOptions2;
			this._layoutViewDeck.TabIndex = 42;
			this._layoutViewDeck.TabStop = false;
			// 
			// FormMain
			// 
			this.ClientSize = new System.Drawing.Size(1738, 826);
			this.Controls.Add(this._panelFindExamples);
			this.Controls.Add(this._listBoxSuggest);
			this.Controls.Add(this._layoutRoot);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormMain";
			this._panelFilters.ResumeLayout(false);
			this._panelFilterButtons.ResumeLayout(false);
			this._panelFilterButtons.PerformLayout();
			this._panelMenu.ResumeLayout(false);
			this._panelMenu.PerformLayout();
			this._findBorderedPanel.ResumeLayout(false);
			this._panelRightCost.ResumeLayout(false);
			this._layoutMain.ResumeLayout(false);
			this._layoutMain.PerformLayout();
			this._layoutRight.ResumeLayout(false);
			this._layoutRight.PerformLayout();
			this._panelRightNarrow.ResumeLayout(false);
			this._panelRightManaCost.ResumeLayout(false);
			this._panelFilterManager.ResumeLayout(false);
			this._layoutRoot.ResumeLayout(false);
			this._layoutRoot.PerformLayout();
			this._panelFindExamples.ResumeLayout(false);
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
		private System.Windows.Forms.FlowLayoutPanel _panelFilterButtons;
		
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
		private System.Windows.Forms.FlowLayoutPanel _panelRightCost;
		private System.Windows.Forms.TableLayoutPanel _layoutMain;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private Controls.CustomCheckBox _buttonSampleHandNew;
		private Controls.CustomCheckBox _buttonSampleHandMulligan;
		private Controls.CustomCheckBox _buttonSampleHandDraw;
		private Controls.CustomCheckBox _buttonHideDeck;
		private Controls.CustomCheckBox _buttonHidePartialCards;
		private Controls.CustomCheckBox _buttonHideText;
		private System.Windows.Forms.TableLayoutPanel _layoutRight;
		private System.Windows.Forms.TableLayoutPanel _layoutRoot;
		private System.Windows.Forms.CheckBox _buttonFindExamplesDropDown;
		private Controls.BorderedTableLayoutPanel _panelFindExamples;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.Label label42;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.Label label44;
		private System.Windows.Forms.Label label45;
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.Label label47;
		private System.Windows.Forms.Label label48;
		private System.Windows.Forms.Label label49;
		private System.Windows.Forms.Label label50;
		private System.Windows.Forms.Label label51;
		private System.Windows.Forms.Label label52;
		private System.Windows.Forms.Label label53;
		private System.Windows.Forms.Label label54;
		private System.Windows.Forms.Label label29;
		private Controls.BorderedPanel _panelIconStatusSort;
		private System.Windows.Forms.Label _labelStatusSort;
		public Controls.QuickFilterControl FilterLayout;
		public Controls.QuickFilterControl FilterCastKeyword;
		private System.Windows.Forms.FlowLayoutPanel _panelRightNarrow;
		private System.Windows.Forms.FlowLayoutPanel _panelRightManaCost;
		private System.Windows.Forms.FlowLayoutPanel _panelFilterManager;
	}
}

