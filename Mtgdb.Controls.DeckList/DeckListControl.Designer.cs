namespace Mtgdb.Controls
{
	partial class DeckListControl
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

		#region Component Designer generated code

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
			this._panelLayout = new System.Windows.Forms.TableLayoutPanel();
			this._panelSearchIcon = new Mtgdb.Controls.BorderedPanel();
			this._viewDeck = new Mtgdb.Controls.LayoutViewControl();
			this._menuFilterByDeckMode = new System.Windows.Forms.ComboBox();
			this._labelSortStatus = new System.Windows.Forms.Label();
			this._labelFilterByDeckMode = new System.Windows.Forms.Label();
			this._panelSortIcon = new Mtgdb.Controls.BorderedPanel();
			this._searchBar = new Mtgdb.Controls.SearchBar();
			this._textBoxName = new System.Windows.Forms.TextBox();
			this._panelLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelLayout
			// 
			this._panelLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelLayout.ColumnCount = 6;
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._panelLayout.Controls.Add(this._panelSearchIcon, 0, 0);
			this._panelLayout.Controls.Add(this._viewDeck, 0, 1);
			this._panelLayout.Controls.Add(this._menuFilterByDeckMode, 5, 0);
			this._panelLayout.Controls.Add(this._labelSortStatus, 3, 0);
			this._panelLayout.Controls.Add(this._labelFilterByDeckMode, 4, 0);
			this._panelLayout.Controls.Add(this._panelSortIcon, 2, 0);
			this._panelLayout.Controls.Add(this._searchBar, 1, 0);
			this._panelLayout.Location = new System.Drawing.Point(0, 0);
			this._panelLayout.Margin = new System.Windows.Forms.Padding(0);
			this._panelLayout.Name = "_panelLayout";
			this._panelLayout.RowCount = 2;
			this._panelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelLayout.Size = new System.Drawing.Size(732, 311);
			this._panelLayout.TabIndex = 2;
			// 
			// _panelSearchIcon
			// 
			this._panelSearchIcon.BackgroundImage = global::Mtgdb.Controls.Properties.Resources.search_48;
			this._panelSearchIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelSearchIcon.Location = new System.Drawing.Point(0, 0);
			this._panelSearchIcon.Margin = new System.Windows.Forms.Padding(0);
			this._panelSearchIcon.Name = "_panelSearchIcon";
			this._panelSearchIcon.Size = new System.Drawing.Size(24, 24);
			this._panelSearchIcon.TabIndex = 0;
			this._panelSearchIcon.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _viewDeck
			// 
			this._viewDeck.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._viewDeck.BackColor = System.Drawing.SystemColors.Window;
			this._panelLayout.SetColumnSpan(this._viewDeck, 6);
			layoutOptions3.AllowPartialCards = true;
			layoutOptions3.CardInterval = new System.Drawing.Size(2, 0);
			layoutOptions3.PartialCardsThreshold = new System.Drawing.Size(150, 0);
			this._viewDeck.LayoutOptions = layoutOptions3;
			this._viewDeck.Location = new System.Drawing.Point(0, 24);
			this._viewDeck.Margin = new System.Windows.Forms.Padding(0);
			this._viewDeck.Name = "_viewDeck";
			searchOptions3.Allow = false;
			searchOptions3.Button = buttonOptions3;
			this._viewDeck.SearchOptions = searchOptions3;
			selectionOptions3.Alpha = ((byte)(255));
			selectionOptions3.ForeColor = System.Drawing.SystemColors.HighlightText;
			selectionOptions3.RectAlpha = ((byte)(0));
			selectionOptions3.RectBorderColor = System.Drawing.Color.Empty;
			selectionOptions3.RectFillColor = System.Drawing.Color.Empty;
			this._viewDeck.SelectionOptions = selectionOptions3;
			this._viewDeck.Size = new System.Drawing.Size(732, 287);
			sortOptions3.Allow = true;
			sortOptions3.ButtonMargin = new System.Drawing.Size(0, 0);
			this._viewDeck.SortOptions = sortOptions3;
			this._viewDeck.TabIndex = 1;
			// 
			// _menuFilterByDeckMode
			// 
			this._menuFilterByDeckMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._menuFilterByDeckMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuFilterByDeckMode.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuFilterByDeckMode.FormattingEnabled = true;
			this._menuFilterByDeckMode.Items.AddRange(new object[] {
            "Ignored",
            "Cards in currently open deck",
            "Cards in saved decks matching filter"});
			this._menuFilterByDeckMode.Location = new System.Drawing.Point(542, 1);
			this._menuFilterByDeckMode.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this._menuFilterByDeckMode.Name = "_menuFilterByDeckMode";
			this._menuFilterByDeckMode.Size = new System.Drawing.Size(190, 21);
			this._menuFilterByDeckMode.TabIndex = 2;
			// 
			// _labelSortStatus
			// 
			this._labelSortStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelSortStatus.AutoSize = true;
			this._labelSortStatus.Location = new System.Drawing.Point(387, 5);
			this._labelSortStatus.Name = "_labelSortStatus";
			this._labelSortStatus.Size = new System.Drawing.Size(44, 13);
			this._labelSortStatus.TabIndex = 3;
			this._labelSortStatus.Text = "Name ^";
			this._labelSortStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _labelFilterByDeckMode
			// 
			this._labelFilterByDeckMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelFilterByDeckMode.AutoSize = true;
			this._labelFilterByDeckMode.Location = new System.Drawing.Point(437, 5);
			this._labelFilterByDeckMode.Name = "_labelFilterByDeckMode";
			this._labelFilterByDeckMode.Size = new System.Drawing.Size(102, 13);
			this._labelFilterByDeckMode.TabIndex = 4;
			this._labelFilterByDeckMode.Text = "Filter cards by deck:";
			this._labelFilterByDeckMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _panelSortIcon
			// 
			this._panelSortIcon.BackgroundImage = global::Mtgdb.Controls.Properties.Resources.sort_48;
			this._panelSortIcon.Location = new System.Drawing.Point(364, 0);
			this._panelSortIcon.Margin = new System.Windows.Forms.Padding(0);
			this._panelSortIcon.Name = "_panelSortIcon";
			this._panelSortIcon.Size = new System.Drawing.Size(20, 24);
			this._panelSortIcon.TabIndex = 5;
			this._panelSortIcon.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			// 
			// _searchBar
			// 
			this._searchBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._searchBar.Font = new System.Drawing.Font("Source Code Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._searchBar.Location = new System.Drawing.Point(24, 0);
			this._searchBar.Margin = new System.Windows.Forms.Padding(0);
			this._searchBar.Name = "_searchBar";
			this._searchBar.SelectedIndex = -1;
			this._searchBar.Size = new System.Drawing.Size(340, 24);
			this._searchBar.TabIndex = 6;
			// 
			// _textBoxName
			// 
			this._textBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._textBoxName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._textBoxName.Location = new System.Drawing.Point(24, 51);
			this._textBoxName.Margin = new System.Windows.Forms.Padding(10, 2, 0, 0);
			this._textBoxName.Multiline = true;
			this._textBoxName.Name = "_textBoxName";
			this._textBoxName.Size = new System.Drawing.Size(177, 20);
			this._textBoxName.TabIndex = 1;
			this._textBoxName.TabStop = false;
			this._textBoxName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// DeckListControl
			// 
			this.Controls.Add(this._textBoxName);
			this.Controls.Add(this._panelLayout);
			this.Name = "DeckListControl";
			this.Size = new System.Drawing.Size(732, 311);
			this._panelLayout.ResumeLayout(false);
			this._panelLayout.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _panelLayout;
		private LayoutViewControl _viewDeck;
		private BorderedPanel _panelSearchIcon;
		private System.Windows.Forms.TextBox _textBoxName;
		private System.Windows.Forms.Label _labelFilterByDeckMode;
		private System.Windows.Forms.ComboBox _menuFilterByDeckMode;
		private System.Windows.Forms.Label _labelSortStatus;
		private BorderedPanel _panelSortIcon;
		private SearchBar _searchBar;
	}
}
