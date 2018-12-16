using System.Drawing;
using System.Windows.Forms;

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
			Mtgdb.Controls.LayoutOptions layoutOptions3 = new Mtgdb.Controls.LayoutOptions();
			Mtgdb.Controls.SearchOptions searchOptions3 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions3 = new Mtgdb.Controls.ButtonOptions();
			Mtgdb.Controls.SelectionOptions selectionOptions3 = new Mtgdb.Controls.SelectionOptions();
			Mtgdb.Controls.SortOptions sortOptions3 = new Mtgdb.Controls.SortOptions();
			Mtgdb.Controls.LayoutOptions layoutOptions4 = new Mtgdb.Controls.LayoutOptions();
			Mtgdb.Controls.SearchOptions searchOptions4 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions4 = new Mtgdb.Controls.ButtonOptions();
			Mtgdb.Controls.SelectionOptions selectionOptions4 = new Mtgdb.Controls.SelectionOptions();
			Mtgdb.Controls.SortOptions sortOptions4 = new Mtgdb.Controls.SortOptions();
			this._panelFilters = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterAbility = new Mtgdb.Controls.QuickFilterControl();
			this.FilterCastKeyword = new Mtgdb.Controls.QuickFilterControl();
			this.FilterType = new Mtgdb.Controls.QuickFilterControl();
			this.FilterManager = new Mtgdb.Controls.QuickFilterControl();
			this.FilterLayout = new Mtgdb.Controls.QuickFilterControl();
			this.FilterRarity = new Mtgdb.Controls.QuickFilterControl();
			this._panelStatus = new System.Windows.Forms.FlowLayoutPanel();
			this._buttonHideDeck = new Mtgdb.Controls.CustomCheckBox();
			this._buttonShowPartialCards = new Mtgdb.Controls.CustomCheckBox();
			this._buttonShowText = new Mtgdb.Controls.CustomCheckBox();
			this._tabHeadersDeck = new Mtgdb.Controls.TabHeaderControl();
			this._buttonSampleHandNew = new Mtgdb.Controls.CustomCheckBox();
			this._buttonSampleHandMulligan = new Mtgdb.Controls.CustomCheckBox();
			this._buttonSampleHandDraw = new Mtgdb.Controls.CustomCheckBox();
			this._panelIconStatusSets = new Mtgdb.Controls.BorderedPanel();
			this._labelStatusSets = new System.Windows.Forms.Label();
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
			this._labelStatusScrollDeck = new System.Windows.Forms.Label();
			this._buttonShowScrollDeck = new Mtgdb.Controls.CustomCheckBox();
			this._labelStatusScrollCards = new System.Windows.Forms.Label();
			this._panelMenu = new System.Windows.Forms.TableLayoutPanel();
			this._panelSearch = new Mtgdb.Controls.BorderedTableLayoutPanel();
			this._buttonSearchExamplesDropDown = new Mtgdb.Controls.CustomCheckBox();
			this._searchEditor = new Mtgdb.Controls.FixedRichTextBox();
			this._panelIconSearch = new Mtgdb.Controls.BorderedPanel();
			this._panelMenuRightSubpanel = new System.Windows.Forms.FlowLayoutPanel();
			this._panelIconLegality = new Mtgdb.Controls.BorderedPanel();
			this._menuLegalityFormat = new System.Windows.Forms.ComboBox();
			this._buttonLegalityAllowLegal = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowRestricted = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowBanned = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLegalityAllowFuture = new Mtgdb.Controls.CustomCheckBox();
			this._buttonShowDuplicates = new Mtgdb.Controls.CustomCheckBox();
			this._panelRightCost = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterGeneratedMana = new Mtgdb.Controls.QuickFilterControl();
			this._panelManaAbility = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterManaAbility = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaAbility = new Mtgdb.Controls.CustomCheckBox();
			this.FilterCmc = new Mtgdb.Controls.QuickFilterControl();
			this._listBoxSuggest = new System.Windows.Forms.ListBox();
			this._layoutMain = new System.Windows.Forms.TableLayoutPanel();
			this._buttonShowScrollCards = new Mtgdb.Controls.CustomCheckBox();
			this._layoutViewCards = new Mtgdb.Controls.LayoutViewControl();
			this._layoutViewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._deckListControl = new Mtgdb.Controls.DeckListControl();
			this._buttonResetFilters = new Mtgdb.Controls.CustomCheckBox();
			this._layoutRight = new System.Windows.Forms.TableLayoutPanel();
			this._buttonShowProhibit = new Mtgdb.Controls.CustomCheckBox();
			this._panelRightNarrow = new System.Windows.Forms.FlowLayoutPanel();
			this._panelRightManaCost = new System.Windows.Forms.FlowLayoutPanel();
			this.FilterManaCost = new Mtgdb.Controls.QuickFilterControl();
			this._buttonExcludeManaCost = new Mtgdb.Controls.CustomCheckBox();
			this._layoutRoot = new System.Windows.Forms.TableLayoutPanel();
			this._panelSearchExamples = new Mtgdb.Gui.SearchExamplesPanel();
			this._panelFilters.SuspendLayout();
			this._panelStatus.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._panelSearch.SuspendLayout();
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
			this.FilterAbility.ShowValueHint = true;
			this.FilterAbility.Size = new System.Drawing.Size(8, 34);
			this.FilterAbility.Spacing = new System.Drawing.Size(-4, -10);
			this.FilterAbility.TabIndex = 13;
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
			this.FilterCastKeyword.ShowValueHint = true;
			this.FilterCastKeyword.Size = new System.Drawing.Size(8, 34);
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
			this.FilterType.MinimumSize = new System.Drawing.Size(44, 2);
			this.FilterType.Name = "FilterType";
			this.FilterType.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterType.PropertiesCount = 0;
			this.FilterType.PropertyImages = null;
			this.FilterType.SelectionBorder = 1.75F;
			this.FilterType.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterType.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterType.ShowValueHint = true;
			this.FilterType.Size = new System.Drawing.Size(44, 2);
			this.FilterType.Spacing = new System.Drawing.Size(2, 0);
			this.FilterType.TabIndex = 14;
			this.FilterType.TabStop = false;
			//
			// FilterManager
			//
			this.FilterManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FilterManager.BorderShape = Mtgdb.Controls.BorderShape.Ellipse;
			this._layoutRight.SetColumnSpan(this.FilterManager, 3);
			this.FilterManager.EnableRequiringSome = true;
			this.FilterManager.HideProhibit = true;
			this.FilterManager.Location = new System.Drawing.Point(110, 821);
			this.FilterManager.Margin = new System.Windows.Forms.Padding(0);
			this.FilterManager.MinimumSize = new System.Drawing.Size(2, 46);
			this.FilterManager.Name = "FilterManager";
			this.FilterManager.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManager.PropertiesCount = 0;
			this.FilterManager.PropertyImages = null;
			this.FilterManager.SelectionBorder = 2F;
			this.FilterManager.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManager.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManager.ShowValueHint = true;
			this.FilterManager.Size = new System.Drawing.Size(2, 46);
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
			this.FilterLayout.ShowValueHint = true;
			this.FilterLayout.Size = new System.Drawing.Size(24, 2);
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
			this.FilterRarity.MinimumSize = new System.Drawing.Size(24, 2);
			this.FilterRarity.Name = "FilterRarity";
			this.FilterRarity.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterRarity.PropertiesCount = 0;
			this.FilterRarity.PropertyImages = null;
			this.FilterRarity.SelectionBorder = 1.75F;
			this.FilterRarity.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterRarity.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterRarity.ShowValueHint = true;
			this.FilterRarity.Size = new System.Drawing.Size(24, 2);
			this.FilterRarity.Spacing = new System.Drawing.Size(2, 0);
			this.FilterRarity.TabIndex = 15;
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
			this._panelStatus.Controls.Add(this._tabHeadersDeck);
			this._panelStatus.Controls.Add(this._buttonSampleHandNew);
			this._panelStatus.Controls.Add(this._buttonSampleHandMulligan);
			this._panelStatus.Controls.Add(this._buttonSampleHandDraw);
			this._panelStatus.Controls.Add(this._panelIconStatusSets);
			this._panelStatus.Controls.Add(this._labelStatusSets);
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
			this._panelStatus.Controls.Add(this._panelIconStatusSort);
			this._panelStatus.Controls.Add(this._labelStatusSort);
			this._panelStatus.Location = new System.Drawing.Point(0, 221);
			this._panelStatus.Margin = new System.Windows.Forms.Padding(0);
			this._panelStatus.Name = "_panelStatus";
			this._panelStatus.Size = new System.Drawing.Size(1169, 24);
			this._panelStatus.TabIndex = 14;
			//
			// _buttonHideDeck
			//
			this._buttonHideDeck.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHideDeck.BackColor = System.Drawing.Color.Transparent;
			this._buttonHideDeck.FlatAppearance.BorderSize = 0;
			this._buttonHideDeck.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this._buttonHideDeck.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this._buttonHideDeck.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
			this._buttonHideDeck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHideDeck.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonHideDeck.HighlightCheckedOpacity = 0;
			this._buttonHideDeck.Image = global::Mtgdb.Gui.Properties.Resources.shown_40;
			this._buttonHideDeck.Location = new System.Drawing.Point(2, 0);
			this._buttonHideDeck.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonHideDeck.Name = "_buttonHideDeck";
			this._buttonHideDeck.Size = new System.Drawing.Size(24, 24);
			this._buttonHideDeck.TabIndex = 47;
			this._buttonHideDeck.TabStop = false;
			this._buttonHideDeck.UseVisualStyleBackColor = false;
			//
			// _buttonShowPartialCards
			//
			this._buttonShowPartialCards.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowPartialCards.BackColor = System.Drawing.Color.Transparent;
			this._buttonShowPartialCards.Checked = true;
			this._buttonShowPartialCards.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonShowPartialCards.FlatAppearance.BorderSize = 0;
			this._buttonShowPartialCards.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowPartialCards.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonShowPartialCards.Image = global::Mtgdb.Gui.Properties.Resources.partial_card_enabled_40;
			this._buttonShowPartialCards.Location = new System.Drawing.Point(28, 0);
			this._buttonShowPartialCards.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonShowPartialCards.Name = "_buttonShowPartialCards";
			this._buttonShowPartialCards.Size = new System.Drawing.Size(24, 24);
			this._buttonShowPartialCards.TabIndex = 48;
			this._buttonShowPartialCards.TabStop = false;
			this._buttonShowPartialCards.UseVisualStyleBackColor = false;
			//
			// _buttonShowText
			//
			this._buttonShowText.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowText.BackColor = System.Drawing.Color.Transparent;
			this._buttonShowText.Checked = true;
			this._buttonShowText.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonShowText.FlatAppearance.BorderSize = 0;
			this._buttonShowText.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonShowText.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonShowText.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonShowText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowText.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonShowText.Image = global::Mtgdb.Gui.Properties.Resources.text_enabled_40;
			this._buttonShowText.Location = new System.Drawing.Point(54, 0);
			this._buttonShowText.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonShowText.Name = "_buttonShowText";
			this._buttonShowText.Size = new System.Drawing.Size(24, 24);
			this._buttonShowText.TabIndex = 49;
			this._buttonShowText.TabStop = false;
			this._buttonShowText.UseVisualStyleBackColor = false;
			//
			// _tabHeadersDeck
			//
			this._tabHeadersDeck.AllowAddingTabs = false;
			this._tabHeadersDeck.AllowRemovingTabs = false;
			this._tabHeadersDeck.AllowReorderTabs = false;
			this._tabHeadersDeck.ColorSelected = System.Drawing.SystemColors.Window;
			this._tabHeadersDeck.ColorSelectedHovered = System.Drawing.SystemColors.Window;
			this._tabHeadersDeck.ColorUnselected = System.Drawing.SystemColors.Control;
			this._tabHeadersDeck.ColorUnselectedHovered = System.Drawing.SystemColors.Window;
			this._tabHeadersDeck.Count = 4;
			this._tabHeadersDeck.Location = new System.Drawing.Point(78, 0);
			this._tabHeadersDeck.Margin = new System.Windows.Forms.Padding(0);
			this._tabHeadersDeck.Name = "_tabHeadersDeck";
			this._tabHeadersDeck.SelectedIndex = 0;
			this._tabHeadersDeck.Size = new System.Drawing.Size(295, 24);
			this._tabHeadersDeck.TabIndex = 29;
			this._tabHeadersDeck.TabStop = false;
			//
			// _buttonSampleHandNew
			//
			this._buttonSampleHandNew.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSampleHandNew.AutoCheck = false;
			this._buttonSampleHandNew.BackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatAppearance.BorderSize = 0;
			this._buttonSampleHandNew.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSampleHandNew.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonSampleHandNew.Image = global::Mtgdb.Gui.Properties.Resources.hand_48;
			this._buttonSampleHandNew.Location = new System.Drawing.Point(375, 0);
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
			this._buttonSampleHandMulligan.AutoCheck = false;
			this._buttonSampleHandMulligan.BackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatAppearance.BorderSize = 0;
			this._buttonSampleHandMulligan.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandMulligan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSampleHandMulligan.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonSampleHandMulligan.Image = global::Mtgdb.Gui.Properties.Resources.mulligan_48;
			this._buttonSampleHandMulligan.Location = new System.Drawing.Point(437, 0);
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
			this._buttonSampleHandDraw.AutoCheck = false;
			this._buttonSampleHandDraw.BackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatAppearance.BorderSize = 0;
			this._buttonSampleHandDraw.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this._buttonSampleHandDraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSampleHandDraw.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonSampleHandDraw.Image = global::Mtgdb.Gui.Properties.Resources.draw_48;
			this._buttonSampleHandDraw.Location = new System.Drawing.Point(487, 0);
			this._buttonSampleHandDraw.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._buttonSampleHandDraw.Name = "_buttonSampleHandDraw";
			this._buttonSampleHandDraw.Size = new System.Drawing.Size(36, 24);
			this._buttonSampleHandDraw.TabIndex = 46;
			this._buttonSampleHandDraw.TabStop = false;
			this._buttonSampleHandDraw.UseVisualStyleBackColor = false;
			//
			// _panelIconStatusSets
			//
			this._panelIconStatusSets.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.mtg_48;
			this._panelIconStatusSets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusSets.Location = new System.Drawing.Point(547, 0);
			this._panelIconStatusSets.Margin = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this._panelIconStatusSets.Name = "_panelIconStatusSets";
			this._panelIconStatusSets.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSets.TabIndex = 34;
			this._panelIconStatusSets.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusSets
			//
			this._labelStatusSets.AutoSize = true;
			this._labelStatusSets.Location = new System.Drawing.Point(571, 6);
			this._labelStatusSets.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSets.Name = "_labelStatusSets";
			this._labelStatusSets.Size = new System.Drawing.Size(25, 13);
			this._labelStatusSets.TabIndex = 36;
			this._labelStatusSets.Text = "206";
			//
			// _panelIconStatusCollection
			//
			this._panelIconStatusCollection.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.box_48;
			this._panelIconStatusCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusCollection.Location = new System.Drawing.Point(608, 0);
			this._panelIconStatusCollection.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconStatusCollection.Name = "_panelIconStatusCollection";
			this._panelIconStatusCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusCollection.TabIndex = 31;
			this._panelIconStatusCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusCollection
			//
			this._labelStatusCollection.AutoSize = true;
			this._labelStatusCollection.Location = new System.Drawing.Point(632, 6);
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
			this._panelIconStatusFilterButtons.Location = new System.Drawing.Point(669, 0);
			this._panelIconStatusFilterButtons.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconStatusFilterButtons.Name = "_panelIconStatusFilterButtons";
			this._panelIconStatusFilterButtons.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterButtons.TabIndex = 32;
			this._panelIconStatusFilterButtons.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusFilterButtons
			//
			this._labelStatusFilterButtons.AutoSize = true;
			this._labelStatusFilterButtons.Location = new System.Drawing.Point(693, 6);
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
			this._panelIconStatusSearch.Location = new System.Drawing.Point(747, 0);
			this._panelIconStatusSearch.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconStatusSearch.Name = "_panelIconStatusSearch";
			this._panelIconStatusSearch.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSearch.TabIndex = 33;
			this._panelIconStatusSearch.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusSearch
			//
			this._labelStatusSearch.AutoSize = true;
			this._labelStatusSearch.Location = new System.Drawing.Point(771, 6);
			this._labelStatusSearch.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSearch.Name = "_labelStatusSearch";
			this._labelStatusSearch.Size = new System.Drawing.Size(42, 13);
			this._labelStatusSearch.TabIndex = 41;
			this._labelStatusSearch.Text = "ignored";
			//
			// _panelIconStatusFilterCollection
			//
			this._panelIconStatusFilterCollection.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.box_48;
			this._panelIconStatusFilterCollection.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusFilterCollection.Location = new System.Drawing.Point(825, 0);
			this._panelIconStatusFilterCollection.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconStatusFilterCollection.Name = "_panelIconStatusFilterCollection";
			this._panelIconStatusFilterCollection.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterCollection.TabIndex = 32;
			this._panelIconStatusFilterCollection.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusFilterCollection
			//
			this._labelStatusFilterCollection.AutoSize = true;
			this._labelStatusFilterCollection.Location = new System.Drawing.Point(849, 6);
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
			this._panelIconStatusFilterDeck.Location = new System.Drawing.Point(903, 0);
			this._panelIconStatusFilterDeck.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconStatusFilterDeck.Name = "_panelIconStatusFilterDeck";
			this._panelIconStatusFilterDeck.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterDeck.TabIndex = 33;
			this._panelIconStatusFilterDeck.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusFilterDeck
			//
			this._labelStatusFilterDeck.AutoSize = true;
			this._labelStatusFilterDeck.Location = new System.Drawing.Point(927, 6);
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
			this._panelIconStatusFilterLegality.Location = new System.Drawing.Point(981, 0);
			this._panelIconStatusFilterLegality.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconStatusFilterLegality.Name = "_panelIconStatusFilterLegality";
			this._panelIconStatusFilterLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusFilterLegality.TabIndex = 34;
			this._panelIconStatusFilterLegality.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusFilterLegality
			//
			this._labelStatusFilterLegality.AutoSize = true;
			this._labelStatusFilterLegality.Location = new System.Drawing.Point(1005, 6);
			this._labelStatusFilterLegality.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusFilterLegality.Name = "_labelStatusFilterLegality";
			this._labelStatusFilterLegality.Size = new System.Drawing.Size(42, 13);
			this._labelStatusFilterLegality.TabIndex = 43;
			this._labelStatusFilterLegality.Text = "ignored";
			//
			// _panelIconStatusSort
			//
			this._panelIconStatusSort.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.sort_48;
			this._panelIconStatusSort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconStatusSort.Location = new System.Drawing.Point(1059, 0);
			this._panelIconStatusSort.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._panelIconStatusSort.Name = "_panelIconStatusSort";
			this._panelIconStatusSort.Size = new System.Drawing.Size(24, 24);
			this._panelIconStatusSort.TabIndex = 35;
			this._panelIconStatusSort.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _labelStatusSort
			//
			this._labelStatusSort.AutoSize = true;
			this._labelStatusSort.Location = new System.Drawing.Point(1083, 6);
			this._labelStatusSort.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._labelStatusSort.Name = "_labelStatusSort";
			this._labelStatusSort.Size = new System.Drawing.Size(78, 13);
			this._labelStatusSort.TabIndex = 50;
			this._labelStatusSort.Text = "ReleaseDate ˄";
			//
			// _labelStatusScrollDeck
			//
			this._labelStatusScrollDeck.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._labelStatusScrollDeck.AutoSize = true;
			this._labelStatusScrollDeck.Location = new System.Drawing.Point(1169, 221);
			this._labelStatusScrollDeck.Margin = new System.Windows.Forms.Padding(0);
			this._labelStatusScrollDeck.Name = "_labelStatusScrollDeck";
			this._labelStatusScrollDeck.Size = new System.Drawing.Size(48, 24);
			this._labelStatusScrollDeck.TabIndex = 35;
			this._labelStatusScrollDeck.Text = "63/60";
			this._labelStatusScrollDeck.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// _buttonShowScrollDeck
			//
			this._buttonShowScrollDeck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowScrollDeck.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowScrollDeck.BackColor = System.Drawing.SystemColors.Control;
			this._buttonShowScrollDeck.Checked = true;
			this._buttonShowScrollDeck.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonShowScrollDeck.FlatAppearance.BorderSize = 0;
			this._buttonShowScrollDeck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowScrollDeck.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonShowScrollDeck.Image = global::Mtgdb.Gui.Properties.Resources.scroll_shown_40;
			this._buttonShowScrollDeck.Location = new System.Drawing.Point(1217, 221);
			this._buttonShowScrollDeck.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowScrollDeck.Name = "_buttonShowScrollDeck";
			this._buttonShowScrollDeck.Size = new System.Drawing.Size(17, 24);
			this._buttonShowScrollDeck.TabIndex = 31;
			this._buttonShowScrollDeck.TabStop = false;
			this._buttonShowScrollDeck.UseVisualStyleBackColor = false;
			//
			// _labelStatusScrollCards
			//
			this._labelStatusScrollCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._labelStatusScrollCards.AutoSize = true;
			this._labelStatusScrollCards.Location = new System.Drawing.Point(1169, 34);
			this._labelStatusScrollCards.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this._labelStatusScrollCards.Name = "_labelStatusScrollCards";
			this._labelStatusScrollCards.Size = new System.Drawing.Size(48, 21);
			this._labelStatusScrollCards.TabIndex = 37;
			this._labelStatusScrollCards.Text = "0/16001";
			this._labelStatusScrollCards.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			// _panelMenu
			//
			this._panelMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelMenu.AutoSize = true;
			this._panelMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelMenu.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelMenu.Controls.Add(this._panelSearch, 0, 0);
			this._panelMenu.Controls.Add(this._panelMenuRightSubpanel, 1, 0);
			this._panelMenu.Location = new System.Drawing.Point(0, 34);
			this._panelMenu.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelMenu.Size = new System.Drawing.Size(1169, 24);
			this._panelMenu.TabIndex = 10;
			//
			// _panelSearch
			//
			this._panelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelSearch.BackColor = System.Drawing.SystemColors.Window;
			this._panelSearch.ColumnCount = 3;
			this._panelSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelSearch.Controls.Add(this._buttonSearchExamplesDropDown, 2, 0);
			this._panelSearch.Controls.Add(this._searchEditor, 1, 0);
			this._panelSearch.Controls.Add(this._panelIconSearch, 0, 0);
			this._panelSearch.Location = new System.Drawing.Point(0, 0);
			this._panelSearch.Margin = new System.Windows.Forms.Padding(0);
			this._panelSearch.Name = "_panelSearch";
			this._panelSearch.RowCount = 1;
			this._panelSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelSearch.Size = new System.Drawing.Size(818, 24);
			this._panelSearch.TabIndex = 42;
			this._panelSearch.VisibleBorders = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
			//
			// _buttonSearchExamplesDropDown
			//
			this._buttonSearchExamplesDropDown.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSearchExamplesDropDown.BackColor = System.Drawing.Color.Transparent;
			this._buttonSearchExamplesDropDown.FlatAppearance.BorderSize = 0;
			this._buttonSearchExamplesDropDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSearchExamplesDropDown.Image = global::Mtgdb.Gui.Properties.Resources.book_40;
			this._buttonSearchExamplesDropDown.Location = new System.Drawing.Point(795, 1);
			this._buttonSearchExamplesDropDown.Margin = new System.Windows.Forms.Padding(1);
			this._buttonSearchExamplesDropDown.Name = "_buttonSearchExamplesDropDown";
			this._buttonSearchExamplesDropDown.Size = new System.Drawing.Size(22, 22);
			this._buttonSearchExamplesDropDown.TabIndex = 22;
			this._buttonSearchExamplesDropDown.TabStop = false;
			this._buttonSearchExamplesDropDown.UseVisualStyleBackColor = false;
			//
			// _searchEditor
			//
			this._searchEditor.AcceptsTab = true;
			this._searchEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._searchEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._searchEditor.DetectUrls = false;
			this._searchEditor.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._searchEditor.Location = new System.Drawing.Point(24, 5);
			this._searchEditor.Margin = new System.Windows.Forms.Padding(0, 5, 0, 1);
			this._searchEditor.Multiline = false;
			this._searchEditor.Name = "_searchEditor";
			this._searchEditor.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this._searchEditor.Size = new System.Drawing.Size(770, 18);
			this._searchEditor.TabIndex = 20;
			this._searchEditor.TabStop = false;
			this._searchEditor.Text = "";
			this._searchEditor.WordWrap = false;
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
			// _panelMenuRightSubpanel
			//
			this._panelMenuRightSubpanel.AutoSize = true;
			this._panelMenuRightSubpanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenuRightSubpanel.Controls.Add(this._panelIconLegality);
			this._panelMenuRightSubpanel.Controls.Add(this._menuLegalityFormat);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowLegal);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowRestricted);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowBanned);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonLegalityAllowFuture);
			this._panelMenuRightSubpanel.Controls.Add(this._buttonShowDuplicates);
			this._panelMenuRightSubpanel.Location = new System.Drawing.Point(818, 0);
			this._panelMenuRightSubpanel.Margin = new System.Windows.Forms.Padding(0);
			this._panelMenuRightSubpanel.Name = "_panelMenuRightSubpanel";
			this._panelMenuRightSubpanel.Size = new System.Drawing.Size(351, 24);
			this._panelMenuRightSubpanel.TabIndex = 41;
			this._panelMenuRightSubpanel.WrapContents = false;
			//
			// _panelIconLegality
			//
			this._panelIconLegality.BackgroundImage = global::Mtgdb.Gui.Properties.Resources.legality_48;
			this._panelIconLegality.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelIconLegality.Location = new System.Drawing.Point(2, 0);
			this._panelIconLegality.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this._panelIconLegality.Name = "_panelIconLegality";
			this._panelIconLegality.Size = new System.Drawing.Size(24, 24);
			this._panelIconLegality.TabIndex = 35;
			this._panelIconLegality.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _menuLegalityFormat
			//
			this._menuLegalityFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuLegalityFormat.DropDownWidth = 150;
			this._menuLegalityFormat.IntegralHeight = false;
			this._menuLegalityFormat.Location = new System.Drawing.Point(28, 2);
			this._menuLegalityFormat.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuLegalityFormat.MaxDropDownItems = 50;
			this._menuLegalityFormat.Name = "_menuLegalityFormat";
			this._menuLegalityFormat.Size = new System.Drawing.Size(121, 21);
			this._menuLegalityFormat.TabIndex = 39;
			this._menuLegalityFormat.TabStop = false;
			//
			// _buttonLegalityAllowLegal
			//
			this._buttonLegalityAllowLegal.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonLegalityAllowLegal.BackColor = System.Drawing.Color.Transparent;
			this._buttonLegalityAllowLegal.Checked = true;
			this._buttonLegalityAllowLegal.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonLegalityAllowLegal.Enabled = false;
			this._buttonLegalityAllowLegal.FlatAppearance.BorderSize = 0;
			this._buttonLegalityAllowLegal.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowLegal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowLegal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowLegal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLegalityAllowLegal.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonLegalityAllowLegal.Location = new System.Drawing.Point(153, 0);
			this._buttonLegalityAllowLegal.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this._buttonLegalityAllowLegal.Name = "_buttonLegalityAllowLegal";
			this._buttonLegalityAllowLegal.Size = new System.Drawing.Size(40, 24);
			this._buttonLegalityAllowLegal.TabIndex = 36;
			this._buttonLegalityAllowLegal.TabStop = false;
			this._buttonLegalityAllowLegal.Text = "legal";
			this._buttonLegalityAllowLegal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonLegalityAllowLegal.UseVisualStyleBackColor = true;
			//
			// _buttonLegalityAllowRestricted
			//
			this._buttonLegalityAllowRestricted.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonLegalityAllowRestricted.BackColor = System.Drawing.Color.Transparent;
			this._buttonLegalityAllowRestricted.Checked = true;
			this._buttonLegalityAllowRestricted.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonLegalityAllowRestricted.Enabled = false;
			this._buttonLegalityAllowRestricted.FlatAppearance.BorderSize = 0;
			this._buttonLegalityAllowRestricted.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowRestricted.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowRestricted.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowRestricted.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLegalityAllowRestricted.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonLegalityAllowRestricted.Location = new System.Drawing.Point(197, 0);
			this._buttonLegalityAllowRestricted.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this._buttonLegalityAllowRestricted.Name = "_buttonLegalityAllowRestricted";
			this._buttonLegalityAllowRestricted.Size = new System.Drawing.Size(66, 24);
			this._buttonLegalityAllowRestricted.TabIndex = 37;
			this._buttonLegalityAllowRestricted.TabStop = false;
			this._buttonLegalityAllowRestricted.Text = "restricted";
			this._buttonLegalityAllowRestricted.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonLegalityAllowRestricted.UseVisualStyleBackColor = true;
			//
			// _buttonLegalityAllowBanned
			//
			this._buttonLegalityAllowBanned.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonLegalityAllowBanned.BackColor = System.Drawing.Color.Transparent;
			this._buttonLegalityAllowBanned.Enabled = false;
			this._buttonLegalityAllowBanned.FlatAppearance.BorderSize = 0;
			this._buttonLegalityAllowBanned.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowBanned.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowBanned.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowBanned.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLegalityAllowBanned.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonLegalityAllowBanned.Location = new System.Drawing.Point(267, 0);
			this._buttonLegalityAllowBanned.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this._buttonLegalityAllowBanned.Name = "_buttonLegalityAllowBanned";
			this._buttonLegalityAllowBanned.Size = new System.Drawing.Size(56, 24);
			this._buttonLegalityAllowBanned.TabIndex = 38;
			this._buttonLegalityAllowBanned.TabStop = false;
			this._buttonLegalityAllowBanned.Text = "banned";
			this._buttonLegalityAllowBanned.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonLegalityAllowBanned.UseVisualStyleBackColor = true;
			//
			// _buttonLegalityAllowFuture
			//
			this._buttonLegalityAllowFuture.Appearance = Appearance.Button;
			this._buttonLegalityAllowFuture.BackColor = System.Drawing.Color.Transparent;
			this._buttonLegalityAllowFuture.Enabled = false;
			this._buttonLegalityAllowFuture.FlatAppearance.BorderSize = 0;
			this._buttonLegalityAllowFuture.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowFuture.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowFuture.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this._buttonLegalityAllowFuture.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLegalityAllowFuture.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonLegalityAllowFuture.Location = new System.Drawing.Point(264, 0);
			this._buttonLegalityAllowFuture.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
			this._buttonLegalityAllowFuture.Name = "_buttonLegalityAllowBanned";
			this._buttonLegalityAllowFuture.Size = new System.Drawing.Size(56, 24);
			this._buttonLegalityAllowFuture.TabIndex = 38;
			this._buttonLegalityAllowFuture.TabStop = false;
			this._buttonLegalityAllowFuture.Text = "future";
			this._buttonLegalityAllowFuture.TextAlign = ContentAlignment.MiddleCenter;
			this._buttonLegalityAllowFuture.UseVisualStyleBackColor = true;
			//
			// _buttonShowDuplicates
			//
			this._buttonShowDuplicates.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowDuplicates.BackColor = System.Drawing.Color.Transparent;
			this._buttonShowDuplicates.FlatAppearance.BorderSize = 0;
			this._buttonShowDuplicates.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonShowDuplicates.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonShowDuplicates.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonShowDuplicates.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowDuplicates.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonShowDuplicates.Image = global::Mtgdb.Gui.Properties.Resources.clone_48;
			this._buttonShowDuplicates.Location = new System.Drawing.Point(327, 0);
			this._buttonShowDuplicates.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
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
			this._panelRightCost.Controls.Add(this._panelManaAbility);
			this._panelRightCost.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._panelRightCost.Location = new System.Drawing.Point(44, 0);
			this._panelRightCost.Margin = new System.Windows.Forms.Padding(0);
			this._panelRightCost.Name = "_panelRightCost";
			this._layoutRight.SetRowSpan(this._panelRightCost, 2);
			this._panelRightCost.Size = new System.Drawing.Size(44, 50);
			this._panelRightCost.TabIndex = 0;
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
			this.FilterGeneratedMana.ShowValueHint = true;
			this.FilterGeneratedMana.Size = new System.Drawing.Size(44, 2);
			this.FilterGeneratedMana.Spacing = new System.Drawing.Size(2, 0);
			this.FilterGeneratedMana.TabIndex = 20;
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
			this._panelManaAbility.TabIndex = 42;
			this._panelManaAbility.WrapContents = false;
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
			this.FilterManaAbility.ShowValueHint = true;
			this.FilterManaAbility.Size = new System.Drawing.Size(44, 2);
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
			this._buttonExcludeManaAbility.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonExcludeManaAbility.HighlightCheckedOpacity = 0;
			this._buttonExcludeManaAbility.Image = global::Mtgdb.Gui.Properties.Resources.include_plus_24;
			this._buttonExcludeManaAbility.Location = new System.Drawing.Point(0, 2);
			this._buttonExcludeManaAbility.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._buttonExcludeManaAbility.Name = "_buttonExcludeManaAbility";
			this._buttonExcludeManaAbility.Size = new System.Drawing.Size(24, 24);
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
			this.FilterCmc.TabIndex = 21;
			this.FilterCmc.TabStop = false;
			//
			// _listBoxSuggest
			//
			this._listBoxSuggest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
			this._layoutMain.Controls.Add(this._buttonResetFilters, 1, 0);
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
			this._layoutMain.Size = new System.Drawing.Size(1234, 867);
			this._layoutMain.TabIndex = 44;
			//
			// _buttonShowScrollCards
			//
			this._buttonShowScrollCards.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonShowScrollCards.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonShowScrollCards.BackColor = System.Drawing.Color.Transparent;
			this._buttonShowScrollCards.Checked = true;
			this._buttonShowScrollCards.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonShowScrollCards.FlatAppearance.BorderSize = 0;
			this._buttonShowScrollCards.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonShowScrollCards.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonShowScrollCards.Image = global::Mtgdb.Gui.Properties.Resources.scroll_shown_40;
			this._buttonShowScrollCards.Location = new System.Drawing.Point(1217, 34);
			this._buttonShowScrollCards.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowScrollCards.Name = "_buttonShowScrollCards";
			this._buttonShowScrollCards.Size = new System.Drawing.Size(17, 24);
			this._buttonShowScrollCards.TabIndex = 51;
			this._buttonShowScrollCards.TabStop = false;
			this._buttonShowScrollCards.UseVisualStyleBackColor = false;
			//
			// _layoutViewCards
			//
			this._layoutViewCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._layoutViewCards.BackColor = System.Drawing.SystemColors.Window;
			this._layoutMain.SetColumnSpan(this._layoutViewCards, 3);
			this._layoutViewCards.LayoutControlType = typeof(Mtgdb.Gui.CardLayout);
			layoutOptions3.AllowPartialCards = true;
			layoutOptions3.CardInterval = new System.Drawing.Size(4, 2);
			layoutOptions3.PartialCardsThreshold = new System.Drawing.Size(327, 209);
			this._layoutViewCards.LayoutOptions = layoutOptions3;
			this._layoutViewCards.Location = new System.Drawing.Point(0, 58);
			this._layoutViewCards.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewCards.Name = "_layoutViewCards";
			buttonOptions3.Margin = new System.Drawing.Size(0, 0);
			searchOptions3.Button = buttonOptions3;
			this._layoutViewCards.SearchOptions = searchOptions3;
			selectionOptions3.Alpha = ((byte)(192));
			selectionOptions3.ForeColor = System.Drawing.SystemColors.HighlightText;
			selectionOptions3.RectAlpha = ((byte)(0));
			selectionOptions3.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions3.RectFillColor = System.Drawing.Color.Empty;
			this._layoutViewCards.SelectionOptions = selectionOptions3;
			this._layoutViewCards.Size = new System.Drawing.Size(1234, 163);
			sortOptions3.Allow = true;
			this._layoutViewCards.SortOptions = sortOptions3;
			this._layoutViewCards.TabIndex = 19;
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
			layoutOptions4.AllowPartialCards = true;
			layoutOptions4.CardInterval = new System.Drawing.Size(2, 0);
			layoutOptions4.PartialCardsThreshold = new System.Drawing.Size(150, 209);
			this._layoutViewDeck.LayoutOptions = layoutOptions4;
			this._layoutViewDeck.Location = new System.Drawing.Point(0, 245);
			this._layoutViewDeck.Margin = new System.Windows.Forms.Padding(0);
			this._layoutViewDeck.Name = "_layoutViewDeck";
			searchOptions4.Button = buttonOptions4;
			this._layoutViewDeck.SearchOptions = searchOptions4;
			selectionOptions4.Alpha = ((byte)(255));
			selectionOptions4.Enabled = false;
			selectionOptions4.ForeColor = System.Drawing.SystemColors.HighlightText;
			selectionOptions4.RectAlpha = ((byte)(0));
			selectionOptions4.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions4.RectFillColor = System.Drawing.Color.Empty;
			this._layoutViewDeck.SelectionOptions = selectionOptions4;
			this._layoutViewDeck.Size = new System.Drawing.Size(1234, 311);
			sortOptions4.Allow = true;
			this._layoutViewDeck.SortOptions = sortOptions4;
			this._layoutViewDeck.TabIndex = 42;
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
			this._deckListControl.Size = new System.Drawing.Size(1234, 311);
			this._deckListControl.TabIndex = 43;
			this._deckListControl.TabStop = false;
			//
			// _buttonEraseFilters
			// 
			this._buttonResetFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonResetFilters.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonResetFilters.AutoCheck = false;
			this._buttonResetFilters.BackColor = System.Drawing.Color.Transparent;
			this._layoutMain.SetColumnSpan(this._buttonResetFilters, 2);
			this._buttonResetFilters.FlatAppearance.BorderSize = 0;
			this._buttonResetFilters.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonResetFilters.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonResetFilters.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonResetFilters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonResetFilters.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonResetFilters.Image = global::Mtgdb.Gui.Properties.Resources.erase;
			this._buttonResetFilters.Location = new System.Drawing.Point(1210, 0);
			this._buttonResetFilters.Margin = new System.Windows.Forms.Padding(0);
			this._buttonResetFilters.Name = "_buttonResetFilters";
			this._buttonResetFilters.Size = new System.Drawing.Size(24, 24);
			this._buttonResetFilters.TabIndex = 41;
			this._buttonResetFilters.TabStop = false;
			this._buttonResetFilters.UseVisualStyleBackColor = false;
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
			this._layoutRight.Location = new System.Drawing.Point(1234, 0);
			this._layoutRight.Margin = new System.Windows.Forms.Padding(0);
			this._layoutRight.Name = "_layoutRight";
			this._layoutRight.RowCount = 3;
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._layoutRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutRight.Size = new System.Drawing.Size(112, 867);
			this._layoutRight.TabIndex = 45;
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
			this._buttonShowProhibit.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonShowProhibit.Image = global::Mtgdb.Gui.Properties.Resources.exclude_hidden_24;
			this._buttonShowProhibit.Location = new System.Drawing.Point(88, 797);
			this._buttonShowProhibit.Margin = new System.Windows.Forms.Padding(0);
			this._buttonShowProhibit.Name = "_buttonShowProhibit";
			this._buttonShowProhibit.Size = new System.Drawing.Size(24, 24);
			this._buttonShowProhibit.TabIndex = 41;
			this._buttonShowProhibit.TabStop = false;
			this._buttonShowProhibit.UseVisualStyleBackColor = false;
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
			this._layoutRight.SetRowSpan(this._panelRightManaCost, 2);
			this._panelRightManaCost.Size = new System.Drawing.Size(44, 22);
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
			this.FilterManaCost.MinimumSize = new System.Drawing.Size(44, 2);
			this.FilterManaCost.Name = "FilterManaCost";
			this.FilterManaCost.ProhibitedColor = System.Drawing.Color.OrangeRed;
			this.FilterManaCost.PropertiesCount = 0;
			this.FilterManaCost.PropertyImages = null;
			this.FilterManaCost.SelectionBorder = 1.75F;
			this.FilterManaCost.SelectionBorderColor = System.Drawing.SystemColors.ActiveCaption;
			this.FilterManaCost.SelectionColor = System.Drawing.SystemColors.Window;
			this.FilterManaCost.ShowValueHint = true;
			this.FilterManaCost.Size = new System.Drawing.Size(44, 2);
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
			this._buttonExcludeManaCost.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonExcludeManaCost.HighlightCheckedOpacity = 0;
			this._buttonExcludeManaCost.Image = global::Mtgdb.Gui.Properties.Resources.exclude_minus_24;
			this._buttonExcludeManaCost.Location = new System.Drawing.Point(0, 2);
			this._buttonExcludeManaCost.Margin = new System.Windows.Forms.Padding(0, 0, 22, 0);
			this._buttonExcludeManaCost.Name = "_buttonExcludeManaCost";
			this._buttonExcludeManaCost.Size = new System.Drawing.Size(22, 20);
			this._buttonExcludeManaCost.TabIndex = 42;
			this._buttonExcludeManaCost.TabStop = false;
			this._buttonExcludeManaCost.UseVisualStyleBackColor = false;
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
			this._layoutRoot.Size = new System.Drawing.Size(1346, 867);
			this._layoutRoot.TabIndex = 46;
			//
			// _panelSearchExamples
			//
			this._panelSearchExamples.Font = new System.Drawing.Font("Consolas", 9F);
			this._panelSearchExamples.Location = new System.Drawing.Point(238, 84);
			this._panelSearchExamples.Margin = new System.Windows.Forms.Padding(0, 1, 3, 3);
			this._panelSearchExamples.Name = "_panelSearchExamples";
			this._panelSearchExamples.Size = new System.Drawing.Size(730, 852);
			this._panelSearchExamples.TabIndex = 47;
			//
			// FormMain
			//
			this.ClientSize = new System.Drawing.Size(1346, 867);
			this.Controls.Add(this._layoutRoot);
			this.Controls.Add(this._panelSearchExamples);
			this.Controls.Add(this._listBoxSuggest);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "FormMain";
			this._panelFilters.ResumeLayout(false);
			this._panelStatus.ResumeLayout(false);
			this._panelStatus.PerformLayout();
			this._panelMenu.ResumeLayout(false);
			this._panelMenu.PerformLayout();
			this._panelSearch.ResumeLayout(false);
			this._panelMenuRightSubpanel.ResumeLayout(false);
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
		private System.Windows.Forms.TableLayoutPanel _panelMenu;
		private Mtgdb.Controls.FixedRichTextBox _searchEditor;
		private System.Windows.Forms.ComboBox _menuLegalityFormat;
		private Mtgdb.Controls.CustomCheckBox _buttonLegalityAllowLegal;
		private Mtgdb.Controls.CustomCheckBox _buttonLegalityAllowRestricted;
		private Mtgdb.Controls.CustomCheckBox _buttonLegalityAllowBanned;
		private Mtgdb.Controls.CustomCheckBox _buttonLegalityAllowFuture;
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

		private Mtgdb.Controls.CustomCheckBox _buttonShowScrollDeck;
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
		private System.Windows.Forms.FlowLayoutPanel _panelRightCost;
		private System.Windows.Forms.TableLayoutPanel _layoutMain;
		private Controls.CustomCheckBox _buttonSampleHandNew;
		private Controls.CustomCheckBox _buttonSampleHandMulligan;
		private Controls.CustomCheckBox _buttonSampleHandDraw;
		private Controls.CustomCheckBox _buttonHideDeck;
		private Controls.CustomCheckBox _buttonShowPartialCards;
		private Controls.CustomCheckBox _buttonShowText;
		private System.Windows.Forms.TableLayoutPanel _layoutRight;
		private System.Windows.Forms.TableLayoutPanel _layoutRoot;
		private Mtgdb.Controls.CustomCheckBox _buttonSearchExamplesDropDown;
		private SearchExamplesPanel _panelSearchExamples;
		private Controls.BorderedPanel _panelIconStatusSort;
		private System.Windows.Forms.Label _labelStatusSort;
		public Controls.QuickFilterControl FilterLayout;
		public Controls.QuickFilterControl FilterCastKeyword;
		private System.Windows.Forms.FlowLayoutPanel _panelRightNarrow;
		private System.Windows.Forms.FlowLayoutPanel _panelRightManaCost;
		private System.Windows.Forms.FlowLayoutPanel _panelMenuRightSubpanel;
		private Controls.BorderedTableLayoutPanel _panelSearch;
		private System.Windows.Forms.FlowLayoutPanel _panelManaAbility;
		private Controls.CustomCheckBox _buttonShowScrollCards;
		private Mtgdb.Controls.DeckListControl _deckListControl;
		private Controls.CustomCheckBox _buttonResetFilters;
	}
}
