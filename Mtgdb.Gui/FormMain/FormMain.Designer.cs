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
			this._filterAbility = new Mtgdb.Controls.QuickFilterControl();
			this._filterCastKeyword = new Mtgdb.Controls.QuickFilterControl();
			this._filterType = new Mtgdb.Controls.QuickFilterControl();
			this._filterManager = new Mtgdb.Controls.QuickFilterControl();
			this._filterLayout = new Mtgdb.Controls.QuickFilterControl();
			this._filterRarity = new Mtgdb.Controls.QuickFilterControl();
			this._panelStatus = new System.Windows.Forms.FlowLayoutPanel();
			this._buttonHideDeck = new Mtgdb.Controls.ButtonBase();
			this._buttonShowPartialCards = new Mtgdb.Controls.ButtonBase();
			this._buttonShowText = new Mtgdb.Controls.ButtonBase();
			this._tabHeadersDeck = new Mtgdb.Controls.TabHeaderControl();
			this._buttonSampleHandNew = new Mtgdb.Controls.ButtonBase();
			this._buttonSampleHandMulligan = new Mtgdb.Controls.ButtonBase();
			this._buttonSampleHandDraw = new Mtgdb.Controls.ButtonBase();
			this._labelStatusSets = new Mtgdb.Controls.ControlBase();
			this._labelStatusCollection = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterButtons = new Mtgdb.Controls.ControlBase();
			this._labelStatusSearch = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterCollection = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterDeck = new Mtgdb.Controls.ControlBase();
			this._labelStatusFilterLegality = new Mtgdb.Controls.ControlBase();
			this._labelStatusSort = new Mtgdb.Controls.ControlBase();
			this._labelStatusScrollDeck = new System.Windows.Forms.Label();
			this._labelStatusScrollCards = new System.Windows.Forms.Label();
			this._panelMenu = new System.Windows.Forms.TableLayoutPanel();
			this._searchBar = new Mtgdb.Controls.SearchBar();
			this._panelMenuRightSubpanel = new System.Windows.Forms.FlowLayoutPanel();
			this._popupSearchExamples = new Mtgdb.Controls.Popup();
			this._panelIconLegality = new Mtgdb.Controls.ControlBase();
			this._dropdownLegality = new Mtgdb.Controls.DropDown();
			this._buttonLegalityAllowLegal = new Mtgdb.Controls.CheckBox();
			this._buttonLegalityAllowRestricted = new Mtgdb.Controls.CheckBox();
			this._buttonLegalityAllowBanned = new Mtgdb.Controls.CheckBox();
			this._buttonLegalityAllowFuture = new Mtgdb.Controls.CheckBox();
			this._buttonShowDuplicates = new Mtgdb.Controls.ButtonBase();
			this._panelRightCost = new System.Windows.Forms.FlowLayoutPanel();
			this._filterGeneratedMana = new Mtgdb.Controls.QuickFilterControl();
			this._panelManaAbility = new System.Windows.Forms.FlowLayoutPanel();
			this._filterManaAbility = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaAbility = new Mtgdb.Controls.ButtonBase();
			this._filterCmc = new Mtgdb.Controls.QuickFilterControl();
			this._layoutMain = new System.Windows.Forms.TableLayoutPanel();
			this._buttonShowScrollDeck = new Mtgdb.Controls.ButtonBase();
			this._buttonShowScrollCards = new Mtgdb.Controls.ButtonBase();
			this._viewCards = new Mtgdb.Controls.LayoutViewControl();
			this._viewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._deckListControl = new Mtgdb.Controls.DeckListControl();
			this._layoutRight = new System.Windows.Forms.TableLayoutPanel();
			this._buttonShowProhibit = new Mtgdb.Controls.ButtonBase();
			this._panelRightNarrow = new System.Windows.Forms.FlowLayoutPanel();
			this._panelRightManaCost = new System.Windows.Forms.FlowLayoutPanel();
			this._filterManaCost = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaCost = new Mtgdb.Controls.ButtonBase();
			this._buttonResetFilters = new Mtgdb.Controls.ButtonBase();
			this._layoutRoot = new System.Windows.Forms.TableLayoutPanel();
			this._menuSearchExamples = new Mtgdb.Gui.SearchExamplesMenu();
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
			this.SuspendLayout();
			//
			// _panelFilters
			//
			this._panelFilters.AutoSize = true;
			this._panelFilters.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layoutMain.SetColumnSpan(this._panelFilters, 3);
			this._panelFilters.Controls.Add(this._filterAbility);
			this._panelFilters.Controls.Add(this._filterCastKeyword);
			this._panelFilters.Location = new System.Drawing.Point(0, 0);
			this._panelFilters.Margin = new System.Windows.Forms.Padding(0);
			this._panelFilters.Name = "_panelFilters";
			this._panelFilters.Size = new System.Drawing.Size(26, 34);
			this._panelFilters.TabIndex = 0;
			//
			// _filterAbility
			//
			this._filterAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterAbility.EnableRequiringSome = true;
			this._filterAbility.HideProhibit = true;
			this._filterAbility.Location = new System.Drawing.Point(0, 0);
			this._filterAbility.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this._filterAbility.MinimumSize = new System.Drawing.Size(8, 34);
			this._filterAbility.Name = "_filterAbility";
			this._filterAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterAbility.PropertiesCount = 0;
			this._filterAbility.PropertyImages = null;
			this._filterAbility.SelectionBorder = 1.75F;
			this._filterAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterAbility.Size = new System.Drawing.Size(8, 34);
			this._filterAbility.Spacing = new System.Drawing.Size(-4, -10);
			this._filterAbility.TabIndex = 0;
			this._filterAbility.TabStop = false;
			//
			// _filterCastKeyword
			//
			this._filterCastKeyword.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterCastKeyword.EnableRequiringSome = true;
			this._filterCastKeyword.HideProhibit = true;
			this._filterCastKeyword.Location = new System.Drawing.Point(18, 0);
			this._filterCastKeyword.Margin = new System.Windows.Forms.Padding(0);
			this._filterCastKeyword.MinimumSize = new System.Drawing.Size(8, 34);
			this._filterCastKeyword.Name = "_filterCastKeyword";
			this._filterCastKeyword.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterCastKeyword.PropertiesCount = 0;
			this._filterCastKeyword.PropertyImages = null;
			this._filterCastKeyword.SelectionBorder = 1.75F;
			this._filterCastKeyword.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterCastKeyword.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterCastKeyword.Size = new System.Drawing.Size(8, 34);
			this._filterCastKeyword.Spacing = new System.Drawing.Size(-4, -10);
			this._filterCastKeyword.TabIndex = 1;
			this._filterCastKeyword.TabStop = false;
			//
			// _filterType
			//
			this._filterType.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterType.EnableCostBehavior = true;
			this._filterType.HideProhibit = true;
			this._filterType.IsFlipped = true;
			this._filterType.IsVertical = true;
			this._filterType.Location = new System.Drawing.Point(0, 0);
			this._filterType.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this._filterType.MinimumSize = new System.Drawing.Size(44, 2);
			this._filterType.Name = "_filterType";
			this._filterType.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterType.PropertiesCount = 0;
			this._filterType.PropertyImages = null;
			this._filterType.SelectionBorder = 1.75F;
			this._filterType.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterType.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterType.Size = new System.Drawing.Size(44, 2);
			this._filterType.Spacing = new System.Drawing.Size(2, 0);
			this._filterType.TabIndex = 0;
			this._filterType.TabStop = false;
			//
			// _filterManager
			//
			this._filterManager.Anchor =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Right)));
			this._filterManager.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._layoutRight.SetColumnSpan(this._filterManager, 3);
			this._filterManager.EnableRequiringSome = true;
			this._filterManager.HideProhibit = true;
			this._filterManager.Location = new System.Drawing.Point(110, 833);
			this._filterManager.Margin = new System.Windows.Forms.Padding(0);
			this._filterManager.MinimumSize = new System.Drawing.Size(2, 34);
			this._filterManager.Name = "_filterManager";
			this._filterManager.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterManager.PropertiesCount = 0;
			this._filterManager.PropertyImages = null;
			this._filterManager.SelectionBorder = 2F;
			this._filterManager.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterManager.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterManager.Size = new System.Drawing.Size(2, 34);
			this._filterManager.Spacing = new System.Drawing.Size(2, -10);
			this._filterManager.TabIndex = 6;
			this._filterManager.TabStop = false;
			//
			// _filterLayout
			//
			this._filterLayout.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterLayout.EnableCostBehavior = true;
			this._filterLayout.EnableMutuallyExclusive = true;
			this._filterLayout.HideProhibit = true;
			this._filterLayout.IsFlipped = true;
			this._filterLayout.IsVertical = true;
			this._filterLayout.Location = new System.Drawing.Point(0, 12);
			this._filterLayout.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this._filterLayout.MinimumSize = new System.Drawing.Size(24, 2);
			this._filterLayout.Name = "_filterLayout";
			this._filterLayout.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterLayout.PropertiesCount = 0;
			this._filterLayout.PropertyImages = null;
			this._filterLayout.SelectionBorder = 1.75F;
			this._filterLayout.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterLayout.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterLayout.Size = new System.Drawing.Size(24, 2);
			this._filterLayout.Spacing = new System.Drawing.Size(2, 0);
			this._filterLayout.TabIndex = 1;
			this._filterLayout.TabStop = false;
			//
			// _filterRarity
			//
			this._filterRarity.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterRarity.EnableCostBehavior = true;
			this._filterRarity.EnableMutuallyExclusive = true;
			this._filterRarity.HideProhibit = true;
			this._filterRarity.IsFlipped = true;
			this._filterRarity.IsVertical = true;
			this._filterRarity.Location = new System.Drawing.Point(0, 0);
			this._filterRarity.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this._filterRarity.MinimumSize = new System.Drawing.Size(24, 2);
			this._filterRarity.Name = "_filterRarity";
			this._filterRarity.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterRarity.PropertiesCount = 0;
			this._filterRarity.PropertyImages = null;
			this._filterRarity.SelectionBorder = 1.75F;
			this._filterRarity.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterRarity.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterRarity.Size = new System.Drawing.Size(24, 2);
			this._filterRarity.Spacing = new System.Drawing.Size(2, 0);
			this._filterRarity.TabIndex = 0;
			this._filterRarity.TabStop = false;
			//
			// _panelStatus
			//
			this._panelStatus.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
			this._panelStatus.AutoSize = true;
			this._panelStatus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelStatus.Controls.Add(this._buttonHideDeck);
			this._panelStatus.Controls.Add(this._buttonShowPartialCards);
			this._panelStatus.Controls.Add(this._buttonShowText);
			this._panelStatus.Controls.Add(this._tabHeadersDeck);
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
			this._buttonShowPartialCards.Image =
				global::Mtgdb.Gui.Properties.Resources.partial_card_enabled_40;
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
			this._tabHeadersDeck.Location = new System.Drawing.Point(72, 0);
			this._tabHeadersDeck.Margin = new System.Windows.Forms.Padding(0);
			this._tabHeadersDeck.Name = "_tabHeadersDeck";
			this._tabHeadersDeck.SelectedIndex = 4;
			this._tabHeadersDeck.Size = new System.Drawing.Size(144, 24);
			this._tabHeadersDeck.SlopeSize = new System.Drawing.Size(9, 21);
			this._tabHeadersDeck.TabIndex = 12;
			this._tabHeadersDeck.TabStop = false;
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
			this._buttonSampleHandNew.Size = new System.Drawing.Size(90, 24);
			this._buttonSampleHandNew.TabIndex = 13;
			this._buttonSampleHandNew.Text = "new hand";
			//
			// _buttonSampleHandMulligan
			//
			this._buttonSampleHandMulligan.AutoCheck = false;
			this._buttonSampleHandMulligan.ImageScale = 0.5F;
			this._buttonSampleHandMulligan.Location = new System.Drawing.Point(306, 0);
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
			this._buttonSampleHandDraw.Location = new System.Drawing.Point(359, 0);
			this._buttonSampleHandDraw.Margin = new System.Windows.Forms.Padding(0);
			this._buttonSampleHandDraw.Name = "_buttonSampleHandDraw";
			this._buttonSampleHandDraw.Size = new System.Drawing.Size(38, 24);
			this._buttonSampleHandDraw.TabIndex = 15;
			this._buttonSampleHandDraw.Text = "draw";
			//
			// _labelStatusSets
			//
			this._labelStatusSets.AutoSize = true;
			this._labelStatusSets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusSets.Image = global::Mtgdb.Gui.Properties.Resources.mtg_48;
			this._labelStatusSets.ImageScale = 0.5F;
			this._labelStatusSets.Location = new System.Drawing.Point(397, 0);
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
			this._labelStatusCollection.Location = new System.Drawing.Point(450, 0);
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
			this._labelStatusFilterButtons.BackgroundImageLayout =
				System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterButtons.Image =
				global::Mtgdb.Gui.Properties.Resources.quick_filters_48;
			this._labelStatusFilterButtons.ImageScale = 0.5F;
			this._labelStatusFilterButtons.Location = new System.Drawing.Point(509, 0);
			this._labelStatusFilterButtons.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterButtons.Name = "_labelStatusFilterButtons";
			this._labelStatusFilterButtons.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterButtons.Size = new System.Drawing.Size(55, 24);
			this._labelStatusFilterButtons.TabIndex = 11;
			this._labelStatusFilterButtons.Text = "and";
			//
			// _labelStatusSearch
			//
			this._labelStatusSearch.AutoSize = true;
			this._labelStatusSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusSearch.Image = global::Mtgdb.Gui.Properties.Resources.search_48;
			this._labelStatusSearch.ImageScale = 0.5F;
			this._labelStatusSearch.Location = new System.Drawing.Point(564, 0);
			this._labelStatusSearch.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusSearch.Name = "_labelStatusSearch";
			this._labelStatusSearch.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusSearch.Size = new System.Drawing.Size(55, 24);
			this._labelStatusSearch.TabIndex = 13;
			this._labelStatusSearch.Text = "and";
			//
			// _labelStatusFilterCollection
			//
			this._labelStatusFilterCollection.AutoSize = true;
			this._labelStatusFilterCollection.BackgroundImageLayout =
				System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterCollection.Image = global::Mtgdb.Gui.Properties.Resources.box_48;
			this._labelStatusFilterCollection.ImageScale = 0.5F;
			this._labelStatusFilterCollection.Location = new System.Drawing.Point(619, 0);
			this._labelStatusFilterCollection.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterCollection.Name = "_labelStatusFilterCollection";
			this._labelStatusFilterCollection.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterCollection.Size = new System.Drawing.Size(54, 24);
			this._labelStatusFilterCollection.TabIndex = 15;
			this._labelStatusFilterCollection.Text = "any";
			//
			// _labelStatusFilterDeck
			//
			this._labelStatusFilterDeck.AutoSize = true;
			this._labelStatusFilterDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterDeck.Image = global::Mtgdb.Gui.Properties.Resources.deck_48;
			this._labelStatusFilterDeck.ImageScale = 0.5F;
			this._labelStatusFilterDeck.Location = new System.Drawing.Point(673, 0);
			this._labelStatusFilterDeck.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterDeck.Name = "_labelStatusFilterDeck";
			this._labelStatusFilterDeck.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterDeck.Size = new System.Drawing.Size(54, 24);
			this._labelStatusFilterDeck.TabIndex = 17;
			this._labelStatusFilterDeck.Text = "any";
			//
			// _labelStatusFilterLegality
			//
			this._labelStatusFilterLegality.AutoSize = true;
			this._labelStatusFilterLegality.BackgroundImageLayout =
				System.Windows.Forms.ImageLayout.Center;
			this._labelStatusFilterLegality.Image = global::Mtgdb.Gui.Properties.Resources.legality_48;
			this._labelStatusFilterLegality.ImageScale = 0.5F;
			this._labelStatusFilterLegality.Location = new System.Drawing.Point(727, 0);
			this._labelStatusFilterLegality.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusFilterLegality.Name = "_labelStatusFilterLegality";
			this._labelStatusFilterLegality.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusFilterLegality.Size = new System.Drawing.Size(146, 24);
			this._labelStatusFilterLegality.TabIndex = 19;
			this._labelStatusFilterLegality.Text = "Standard +L +R -B -F";
			//
			// _labelStatusSort
			//
			this._labelStatusSort.AutoSize = true;
			this._labelStatusSort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._labelStatusSort.Image = global::Mtgdb.Gui.Properties.Resources.sort_48;
			this._labelStatusSort.ImageScale = 0.5F;
			this._labelStatusSort.Location = new System.Drawing.Point(873, 0);
			this._labelStatusSort.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusSort.Name = "_labelStatusSort";
			this._labelStatusSort.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
			this._labelStatusSort.Size = new System.Drawing.Size(78, 24);
			this._labelStatusSort.TabIndex = 21;
			this._labelStatusSort.Text = "Name ^";
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
			this._panelMenu.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
			this._panelMenu.AutoSize = true;
			this._panelMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenu.ColumnStyles.Add(
				new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
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
			// _searchBar
			//
			this._searchBar.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
			this._searchBar.Font = new System.Drawing.Font("Source Code Pro", 9.749999F,
				System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (204)));
			this._searchBar.Image = global::Mtgdb.Gui.Properties.Resources.search_48;
			this._searchBar.ImageScale = 0.5F;
			this._searchBar.Location = new System.Drawing.Point(0, 0);
			this._searchBar.Margin = new System.Windows.Forms.Padding(0);
			this._searchBar.Name = "_searchBar";
			this._searchBar.SelectedIndex = -1;
			this._searchBar.Size = new System.Drawing.Size(715, 24);
			this._searchBar.TabIndex = 1;
			this._searchBar.VisibleAllBorders = null;
			this._searchBar.VisibleBorders =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
					System.Windows.Forms.AnchorStyles.Right)));
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
			this._panelMenuRightSubpanel.Location = new System.Drawing.Point(715, 0);
			this._panelMenuRightSubpanel.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenuRightSubpanel.Name = "_panelMenuRightSubpanel";
			this._panelMenuRightSubpanel.Size = new System.Drawing.Size(456, 24);
			this._panelMenuRightSubpanel.TabIndex = 2;
			this._panelMenuRightSubpanel.WrapContents = false;
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
			this._buttonLegalityAllowLegal.Size = new System.Drawing.Size(56, 24);
			this._buttonLegalityAllowLegal.TabIndex = 2;
			this._buttonLegalityAllowLegal.Text = "legal";
			//
			// _buttonLegalityAllowRestricted
			//
			this._buttonLegalityAllowRestricted.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLegalityAllowRestricted.Checked = true;
			this._buttonLegalityAllowRestricted.Enabled = false;
			this._buttonLegalityAllowRestricted.ImageScale = 0.5F;
			this._buttonLegalityAllowRestricted.Location = new System.Drawing.Point(214, 0);
			this._buttonLegalityAllowRestricted.Margin = new System.Windows.Forms.Padding(0);
			this._buttonLegalityAllowRestricted.Name = "_buttonLegalityAllowRestricted";
			this._buttonLegalityAllowRestricted.Size = new System.Drawing.Size(80, 24);
			this._buttonLegalityAllowRestricted.TabIndex = 3;
			this._buttonLegalityAllowRestricted.Text = "restricted";
			//
			// _buttonLegalityAllowBanned
			//
			this._buttonLegalityAllowBanned.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLegalityAllowBanned.Enabled = false;
			this._buttonLegalityAllowBanned.ImageScale = 0.5F;
			this._buttonLegalityAllowBanned.Location = new System.Drawing.Point(294, 0);
			this._buttonLegalityAllowBanned.Margin = new System.Windows.Forms.Padding(0);
			this._buttonLegalityAllowBanned.Name = "_buttonLegalityAllowBanned";
			this._buttonLegalityAllowBanned.Size = new System.Drawing.Size(71, 24);
			this._buttonLegalityAllowBanned.TabIndex = 4;
			this._buttonLegalityAllowBanned.Text = "banned";
			//
			// _buttonLegalityAllowFuture
			//
			this._buttonLegalityAllowFuture.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLegalityAllowFuture.Checked = true;
			this._buttonLegalityAllowFuture.Enabled = false;
			this._buttonLegalityAllowFuture.ImageScale = 0.5F;
			this._buttonLegalityAllowFuture.Location = new System.Drawing.Point(365, 0);
			this._buttonLegalityAllowFuture.Margin = new System.Windows.Forms.Padding(0);
			this._buttonLegalityAllowFuture.Name = "_buttonLegalityAllowFuture";
			this._buttonLegalityAllowFuture.Size = new System.Drawing.Size(63, 24);
			this._buttonLegalityAllowFuture.TabIndex = 5;
			this._buttonLegalityAllowFuture.Text = "future";
			//
			// _buttonShowDuplicates
			//
			this._buttonShowDuplicates.Image = global::Mtgdb.Gui.Properties.Resources.clone_48;
			this._buttonShowDuplicates.ImageScale = 0.5F;
			this._buttonShowDuplicates.Location = new System.Drawing.Point(432, 0);
			this._buttonShowDuplicates.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this._buttonShowDuplicates.Name = "_buttonShowDuplicates";
			this._buttonShowDuplicates.Size = new System.Drawing.Size(24, 24);
			this._buttonShowDuplicates.TabIndex = 7;
			//
			// _panelRightCost
			//
			this._panelRightCost.AutoSize = true;
			this._panelRightCost.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelRightCost.Controls.Add(this._filterType);
			this._panelRightCost.Controls.Add(this._filterGeneratedMana);
			this._panelRightCost.Controls.Add(this._panelManaAbility);
			this._panelRightCost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelRightCost.Location = new System.Drawing.Point(44, 0);
			this._panelRightCost.Margin = new System.Windows.Forms.Padding(0);
			this._panelRightCost.Name = "_panelRightCost";
			this._layoutRight.SetRowSpan(this._panelRightCost, 2);
			this._panelRightCost.Size = new System.Drawing.Size(44, 50);
			this._panelRightCost.TabIndex = 1;
			//
			// _filterGeneratedMana
			//
			this._filterGeneratedMana.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterGeneratedMana.EnableRequiringSome = true;
			this._filterGeneratedMana.HideProhibit = true;
			this._filterGeneratedMana.IsFlipped = true;
			this._filterGeneratedMana.IsVertical = true;
			this._filterGeneratedMana.Location = new System.Drawing.Point(0, 12);
			this._filterGeneratedMana.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
			this._filterGeneratedMana.MinimumSize = new System.Drawing.Size(44, 2);
			this._filterGeneratedMana.Name = "_filterGeneratedMana";
			this._filterGeneratedMana.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterGeneratedMana.PropertiesCount = 0;
			this._filterGeneratedMana.PropertyImages = null;
			this._filterGeneratedMana.SelectionBorder = 1.75F;
			this._filterGeneratedMana.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterGeneratedMana.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterGeneratedMana.Size = new System.Drawing.Size(44, 2);
			this._filterGeneratedMana.Spacing = new System.Drawing.Size(2, 0);
			this._filterGeneratedMana.TabIndex = 1;
			this._filterGeneratedMana.TabStop = false;
			//
			// _panelManaAbility
			//
			this._panelManaAbility.AutoSize = true;
			this._panelManaAbility.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelManaAbility.Controls.Add(this._filterManaAbility);
			this._panelManaAbility.Controls.Add(this._buttonExcludeManaAbility);
			this._panelManaAbility.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelManaAbility.Location = new System.Drawing.Point(0, 24);
			this._panelManaAbility.Margin = new System.Windows.Forms.Padding(0);
			this._panelManaAbility.Name = "_panelManaAbility";
			this._panelManaAbility.Size = new System.Drawing.Size(44, 26);
			this._panelManaAbility.TabIndex = 2;
			this._panelManaAbility.WrapContents = false;
			//
			// _filterManaAbility
			//
			this._filterManaAbility.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterManaAbility.EnableRequiringSome = true;
			this._filterManaAbility.HideProhibit = true;
			this._filterManaAbility.HintIcon = global::Mtgdb.Gui.Properties.Resources.manatext_25;
			this._filterManaAbility.HintTextShift = new System.Drawing.Size(2, -6);
			this._filterManaAbility.IsFlipped = true;
			this._filterManaAbility.IsVertical = true;
			this._filterManaAbility.Location = new System.Drawing.Point(0, 0);
			this._filterManaAbility.Margin = new System.Windows.Forms.Padding(0);
			this._filterManaAbility.MaximumSize = new System.Drawing.Size(50, 200);
			this._filterManaAbility.MinimumSize = new System.Drawing.Size(44, 2);
			this._filterManaAbility.Name = "_filterManaAbility";
			this._filterManaAbility.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterManaAbility.PropertiesCount = 0;
			this._filterManaAbility.PropertyImages = null;
			this._filterManaAbility.SelectionBorder = 1.75F;
			this._filterManaAbility.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterManaAbility.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterManaAbility.Size = new System.Drawing.Size(44, 2);
			this._filterManaAbility.Spacing = new System.Drawing.Size(2, 0);
			this._filterManaAbility.TabIndex = 0;
			this._filterManaAbility.TabStop = false;
			//
			// _buttonExcludeManaAbility
			//
			this._buttonExcludeManaAbility.Anchor =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Right)));
			this._buttonExcludeManaAbility.HighlightCheckedOpacity = 0;
			this._buttonExcludeManaAbility.ImageChecked =
				global::Mtgdb.Gui.Properties.Resources.exclude_minus_24;
			this._buttonExcludeManaAbility.ImageScale = 0.5F;
			this._buttonExcludeManaAbility.ImageUnchecked =
				global::Mtgdb.Gui.Properties.Resources.include_plus_24;
			this._buttonExcludeManaAbility.Location = new System.Drawing.Point(0, 2);
			this._buttonExcludeManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._buttonExcludeManaAbility.Name = "_buttonExcludeManaAbility";
			this._buttonExcludeManaAbility.Size = new System.Drawing.Size(24, 24);
			this._buttonExcludeManaAbility.TabIndex = 18;
			//
			// _filterCmc
			//
			this._filterCmc.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterCmc.EnableCostBehavior = true;
			this._filterCmc.EnableMutuallyExclusive = true;
			this._filterCmc.HideProhibit = true;
			this._filterCmc.IsFlipped = true;
			this._filterCmc.IsVertical = true;
			this._filterCmc.Location = new System.Drawing.Point(0, 24);
			this._filterCmc.Margin = new System.Windows.Forms.Padding(0);
			this._filterCmc.MinimumSize = new System.Drawing.Size(24, 2);
			this._filterCmc.Name = "_filterCmc";
			this._filterCmc.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterCmc.PropertiesCount = 0;
			this._filterCmc.PropertyImages = null;
			this._filterCmc.SelectionBorder = 1.75F;
			this._filterCmc.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterCmc.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterCmc.Size = new System.Drawing.Size(24, 2);
			this._filterCmc.Spacing = new System.Drawing.Size(2, 0);
			this._filterCmc.TabIndex = 2;
			this._filterCmc.TabStop = false;
			//
			// _layoutMain
			//
			this._layoutMain.Anchor =
				((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
						System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) |
					System.Windows.Forms.AnchorStyles.Right)));
			this._layoutMain.ColumnCount = 3;
			this._layoutMain.ColumnStyles.Add(
				new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutMain.Controls.Add(this._labelStatusScrollCards, 1, 1);
			this._layoutMain.Controls.Add(this._buttonShowScrollDeck, 2, 3);
			this._layoutMain.Controls.Add(this._labelStatusScrollDeck, 1, 3);
			this._layoutMain.Controls.Add(this._buttonShowScrollCards, 2, 1);
			this._layoutMain.Controls.Add(this._viewCards, 0, 2);
			this._layoutMain.Controls.Add(this._panelFilters, 0, 0);
			this._layoutMain.Controls.Add(this._panelMenu, 0, 1);
			this._layoutMain.Controls.Add(this._panelStatus, 0, 3);
			this._layoutMain.Controls.Add(this._viewDeck, 0, 4);
			this._layoutMain.Controls.Add(this._deckListControl, 0, 5);
			this._layoutMain.Location = new System.Drawing.Point(0, 0);
			this._layoutMain.Margin = new System.Windows.Forms.Padding(0);
			this._layoutMain.Name = "_layoutMain";
			this._layoutMain.RowCount = 6;
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(
				new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutMain.Size = new System.Drawing.Size(1267, 867);
			this._layoutMain.TabIndex = 0;
			//
			// _buttonShowScrollDeck
			//
			this._buttonShowScrollDeck.Anchor =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
					System.Windows.Forms.AnchorStyles.Right)));
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
			this._buttonShowScrollCards.Anchor =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom |
					System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowScrollCards.Checked = true;
			this._buttonShowScrollCards.Image = global::Mtgdb.Gui.Properties.Resources.scroll_shown_40;
			this._buttonShowScrollCards.ImageScale = 0.5F;
			this._buttonShowScrollCards.Location = new System.Drawing.Point(1250, 34);
			this._buttonShowScrollCards.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowScrollCards.Name = "_buttonShowScrollCards";
			this._buttonShowScrollCards.Size = new System.Drawing.Size(17, 24);
			this._buttonShowScrollCards.TabIndex = 3;
			//
			// _viewCards
			//
			this._viewCards.Anchor =
				((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
						System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) |
					System.Windows.Forms.AnchorStyles.Right)));
			this._viewCards.BackColor = System.Drawing.SystemColors.Window;
			this._layoutMain.SetColumnSpan(this._viewCards, 3);
			this._viewCards.LayoutControlType = typeof(Mtgdb.Gui.CardLayout);
			layoutOptions1.AllowPartialCards = true;
			layoutOptions1.CardInterval = new System.Drawing.Size(4, 2);
			layoutOptions1.PartialCardsThreshold = new System.Drawing.Size(327, 209);
			this._viewCards.LayoutOptions = layoutOptions1;
			this._viewCards.Location = new System.Drawing.Point(0, 58);
			this._viewCards.Margin = new System.Windows.Forms.Padding(0);
			this._viewCards.Name = "_viewCards";
			buttonOptions1.Margin = new System.Drawing.Size(0, 0);
			searchOptions1.Button = buttonOptions1;
			this._viewCards.SearchOptions = searchOptions1;
			selectionOptions1.Alpha = ((byte) (192));
			selectionOptions1.ForeColor = System.Drawing.SystemColors.HighlightText;
			selectionOptions1.RectAlpha = ((byte) (0));
			selectionOptions1.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions1.RectFillColor = System.Drawing.Color.Empty;
			this._viewCards.SelectionOptions = selectionOptions1;
			this._viewCards.Size = new System.Drawing.Size(1267, 163);
			sortOptions1.Allow = true;
			this._viewCards.SortOptions = sortOptions1;
			this._viewCards.TabIndex = 4;
			this._viewCards.TabStop = false;
			//
			// _viewDeck
			//
			this._viewDeck.Anchor =
				((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
						System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) |
					System.Windows.Forms.AnchorStyles.Right)));
			this._viewDeck.BackColor = System.Drawing.SystemColors.Window;
			this._layoutMain.SetColumnSpan(this._viewDeck, 3);
			this._viewDeck.LayoutControlType = typeof(Mtgdb.Gui.DeckLayout);
			layoutOptions2.AllowPartialCards = true;
			layoutOptions2.CardInterval = new System.Drawing.Size(2, 0);
			layoutOptions2.PartialCardsThreshold = new System.Drawing.Size(150, 209);
			this._viewDeck.LayoutOptions = layoutOptions2;
			this._viewDeck.Location = new System.Drawing.Point(0, 245);
			this._viewDeck.Margin = new System.Windows.Forms.Padding(0);
			this._viewDeck.Name = "_viewDeck";
			searchOptions2.Button = buttonOptions2;
			this._viewDeck.SearchOptions = searchOptions2;
			selectionOptions2.Alpha = ((byte) (255));
			selectionOptions2.Enabled = false;
			selectionOptions2.ForeColor = System.Drawing.SystemColors.HighlightText;
			selectionOptions2.RectAlpha = ((byte) (0));
			selectionOptions2.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions2.RectFillColor = System.Drawing.Color.Empty;
			this._viewDeck.SelectionOptions = selectionOptions2;
			this._viewDeck.Size = new System.Drawing.Size(1267, 311);
			sortOptions2.Allow = true;
			this._viewDeck.SortOptions = sortOptions2;
			this._viewDeck.TabIndex = 7;
			this._viewDeck.TabStop = false;
			//
			// _deckListControl
			//
			this._deckListControl.AllowPartialCard = true;
			this._deckListControl.Anchor =
				((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top |
						System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) |
					System.Windows.Forms.AnchorStyles.Right)));
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
			// _layoutRight
			//
			this._layoutRight.Anchor =
				((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutRight.AutoSize = true;
			this._layoutRight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layoutRight.ColumnCount = 3;
			this._layoutRight.ColumnStyles.Add(
				new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
			this._layoutRight.ColumnStyles.Add(
				new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
			this._layoutRight.ColumnStyles.Add(
				new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layoutRight.Controls.Add(this._buttonShowProhibit, 2, 1);
			this._layoutRight.Controls.Add(this._panelRightNarrow, 2, 0);
			this._layoutRight.Controls.Add(this._panelRightManaCost, 0, 0);
			this._layoutRight.Controls.Add(this._panelRightCost, 1, 0);
			this._layoutRight.Controls.Add(this._filterManager, 0, 2);
			this._layoutRight.Controls.Add(this._buttonResetFilters, 0, 1);
			this._layoutRight.Location = new System.Drawing.Point(1267, 0);
			this._layoutRight.Margin = new System.Windows.Forms.Padding(0);
			this._layoutRight.Name = "_layoutRight";
			this._layoutRight.RowCount = 3;
			this._layoutRight.RowStyles.Add(
				new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRight.RowStyles.Add(
				new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutRight.Size = new System.Drawing.Size(112, 867);
			this._layoutRight.TabIndex = 1;
			//
			// _buttonShowProhibit
			//
			this._buttonShowProhibit.Anchor =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowProhibit.ImageChecked =
				global::Mtgdb.Gui.Properties.Resources.exclude_shown_24;
			this._buttonShowProhibit.ImageScale = 0.5F;
			this._buttonShowProhibit.ImageUnchecked =
				global::Mtgdb.Gui.Properties.Resources.exclude_hidden_24;
			this._buttonShowProhibit.Location = new System.Drawing.Point(88, 809);
			this._buttonShowProhibit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowProhibit.Name = "_buttonShowProhibit";
			this._buttonShowProhibit.Size = new System.Drawing.Size(24, 24);
			this._buttonShowProhibit.TabIndex = 4;
			//
			// _panelRightNarrow
			//
			this._panelRightNarrow.AutoSize = true;
			this._panelRightNarrow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelRightNarrow.Controls.Add(this._filterRarity);
			this._panelRightNarrow.Controls.Add(this._filterLayout);
			this._panelRightNarrow.Controls.Add(this._filterCmc);
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
			this._panelRightManaCost.Controls.Add(this._filterManaCost);
			this._panelRightManaCost.Controls.Add(this._buttonExcludeManaCost);
			this._panelRightManaCost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelRightManaCost.Location = new System.Drawing.Point(0, 0);
			this._panelRightManaCost.Margin = new System.Windows.Forms.Padding(0);
			this._panelRightManaCost.Name = "_panelRightManaCost";
			this._panelRightManaCost.Size = new System.Drawing.Size(44, 26);
			this._panelRightManaCost.TabIndex = 0;
			this._panelRightManaCost.WrapContents = false;
			//
			// _filterManaCost
			//
			this._filterManaCost.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._filterManaCost.EnableCostBehavior = true;
			this._filterManaCost.HideProhibit = true;
			this._filterManaCost.HintIcon = global::Mtgdb.Gui.Properties.Resources.manacost_25;
			this._filterManaCost.HintIconShift = new System.Drawing.Size(0, -6);
			this._filterManaCost.HintTextShift = new System.Drawing.Size(0, 4);
			this._filterManaCost.IsFlipped = true;
			this._filterManaCost.IsVertical = true;
			this._filterManaCost.Location = new System.Drawing.Point(0, 0);
			this._filterManaCost.Margin = new System.Windows.Forms.Padding(0);
			this._filterManaCost.MinimumSize = new System.Drawing.Size(44, 2);
			this._filterManaCost.Name = "_filterManaCost";
			this._filterManaCost.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this._filterManaCost.PropertiesCount = 0;
			this._filterManaCost.PropertyImages = null;
			this._filterManaCost.SelectionBorder = 1.75F;
			this._filterManaCost.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this._filterManaCost.SelectionColor = System.Drawing.SystemColors.Window;
			this._filterManaCost.Size = new System.Drawing.Size(44, 2);
			this._filterManaCost.Spacing = new System.Drawing.Size(2, 0);
			this._filterManaCost.TabIndex = 0;
			this._filterManaCost.TabStop = false;
			//
			// _buttonExcludeManaCost
			//
			this._buttonExcludeManaCost.Anchor =
				((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top |
					System.Windows.Forms.AnchorStyles.Right)));
			this._buttonExcludeManaCost.Checked = true;
			this._buttonExcludeManaCost.HighlightCheckedOpacity = 0;
			this._buttonExcludeManaCost.ImageChecked =
				global::Mtgdb.Gui.Properties.Resources.exclude_minus_24;
			this._buttonExcludeManaCost.ImageScale = 0.5F;
			this._buttonExcludeManaCost.ImageUnchecked =
				global::Mtgdb.Gui.Properties.Resources.include_plus_24;
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
			// _layoutRoot
			//
			this._layoutRoot.ColumnCount = 2;
			this._layoutRoot.ColumnStyles.Add(
				new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutRoot.Controls.Add(this._layoutMain, 0, 0);
			this._layoutRoot.Controls.Add(this._layoutRight, 1, 0);
			this._layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutRoot.Location = new System.Drawing.Point(0, 0);
			this._layoutRoot.Name = "_layoutRoot";
			this._layoutRoot.RowCount = 1;
			this._layoutRoot.RowStyles.Add(
				new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRoot.Size = new System.Drawing.Size(1379, 867);
			this._layoutRoot.TabIndex = 0;
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
			this._menuSearchExamples.Size = new System.Drawing.Size(615, 879);
			this._menuSearchExamples.TabIndex = 3;
			this._menuSearchExamples.TabStop = false;
			this._menuSearchExamples.Visible = false;
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
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private Mtgdb.Controls.QuickFilterControl _filterType;
		private Mtgdb.Controls.QuickFilterControl _filterAbility;
		private Mtgdb.Controls.QuickFilterControl _filterRarity;
		private Mtgdb.Controls.QuickFilterControl _filterManager;
		private Mtgdb.Controls.QuickFilterControl _filterManaCost;
		private Mtgdb.Controls.QuickFilterControl _filterManaAbility;
		private Mtgdb.Controls.QuickFilterControl _filterCmc;
		private Mtgdb.Controls.QuickFilterControl _filterGeneratedMana;
		private System.Windows.Forms.FlowLayoutPanel _panelFilters;
		private System.Windows.Forms.FlowLayoutPanel _panelStatus;
		private Mtgdb.Controls.LayoutViewControl _viewCards;
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
		private Mtgdb.Controls.LayoutViewControl _viewDeck;
		private System.Windows.Forms.FlowLayoutPanel _panelRightCost;
		private System.Windows.Forms.TableLayoutPanel _layoutMain;
		private Mtgdb.Controls.ButtonBase _buttonSampleHandNew;
		private Mtgdb.Controls.ButtonBase _buttonSampleHandMulligan;
		private Mtgdb.Controls.ButtonBase _buttonSampleHandDraw;
		private Mtgdb.Controls.ButtonBase _buttonHideDeck;
		private Mtgdb.Controls.ButtonBase _buttonShowPartialCards;
		private Mtgdb.Controls.ButtonBase _buttonShowText;
		private System.Windows.Forms.TableLayoutPanel _layoutRight;
		private System.Windows.Forms.TableLayoutPanel _layoutRoot;
		private Mtgdb.Controls.Popup _popupSearchExamples;
		private Mtgdb.Gui.SearchExamplesMenu _menuSearchExamples;
		private Mtgdb.Controls.ControlBase _labelStatusSort;
		private Mtgdb.Controls.QuickFilterControl _filterLayout;
		private Mtgdb.Controls.QuickFilterControl _filterCastKeyword;
		private System.Windows.Forms.FlowLayoutPanel _panelRightNarrow;
		private System.Windows.Forms.FlowLayoutPanel _panelRightManaCost;
		private System.Windows.Forms.FlowLayoutPanel _panelMenuRightSubpanel;
		private System.Windows.Forms.FlowLayoutPanel _panelManaAbility;
		private Mtgdb.Controls.ButtonBase _buttonShowScrollCards;
		private Mtgdb.Controls.DeckListControl _deckListControl;
		private Mtgdb.Controls.ButtonBase _buttonResetFilters;
		private Mtgdb.Controls.SearchBar _searchBar;
	}
}
