namespace Mtgdb.Gui
{
	partial class FormChart
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
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChart));
			this._chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this._buttonManaCurveType = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDeckPrice = new Mtgdb.Controls.CustomCheckBox();
			this._tabCols = new Mtgdb.Controls.TabHeaderControl();
			this._buttonApply = new System.Windows.Forms.Button();
			this._menuFields = new System.Windows.Forms.ComboBox();
			this._buttonAddCol = new System.Windows.Forms.Button();
			this._panelFields = new System.Windows.Forms.FlowLayoutPanel();
			this._labelCols = new System.Windows.Forms.Label();
			this._labelRows = new System.Windows.Forms.Label();
			this._tabRows = new Mtgdb.Controls.TabHeaderControl();
			this._labelSum = new System.Windows.Forms.Label();
			this._tabSumm = new Mtgdb.Controls.TabHeaderControl();
			this._labelSummarySort = new System.Windows.Forms.Label();
			this._tabSummSort = new Mtgdb.Controls.TabHeaderControl();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._buttonArgumentTotals = new System.Windows.Forms.CheckBox();
			this._buttonSeriesTotal = new System.Windows.Forms.CheckBox();
			this._buttonExplainTotal = new System.Windows.Forms.CheckBox();
			this._buttonApplyFilter = new System.Windows.Forms.CheckBox();
			this._buttonAddRow = new System.Windows.Forms.Button();
			this._buttonAddSum = new System.Windows.Forms.Button();
			this._panelMenu = new System.Windows.Forms.FlowLayoutPanel();
			this._labelField = new System.Windows.Forms.Label();
			this._labelDataElement = new System.Windows.Forms.Label();
			this._menuLabelDataElement = new System.Windows.Forms.ComboBox();
			this._labelDataSource = new System.Windows.Forms.Label();
			this._menuDataSource = new System.Windows.Forms.ComboBox();
			this._labelChartType = new System.Windows.Forms.Label();
			this._menuChartType = new System.Windows.Forms.ComboBox();
			this._panelTable = new System.Windows.Forms.TableLayoutPanel();
			this._panelFlags = new System.Windows.Forms.FlowLayoutPanel();
			this._menuPrice = new System.Windows.Forms.ComboBox();
			this._menuPriceChartType = new System.Windows.Forms.ComboBox();
			this._buttonArtistsPerYear = new Mtgdb.Controls.CustomCheckBox();
			this._buttonCollectionPrice = new Mtgdb.Controls.CustomCheckBox();
			this._layoutTitle = new Mtgdb.Controls.BorderedTableLayoutPanel();
			this._buttonManaCurveManacost = new Mtgdb.Controls.CustomCheckBox();
			this._buttonCollectionColors = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDeckColors = new Mtgdb.Controls.CustomCheckBox();
			this._panelClient.SuspendLayout();
			this._panelCaption.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._chart)).BeginInit();
			this._panelFields.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._panelTable.SuspendLayout();
			this._panelFlags.SuspendLayout();
			this._layoutTitle.SuspendLayout();
			this.SuspendLayout();
			//
			// _panelClient
			//
			this._panelClient.Controls.Add(this._panelTable);
			this._panelClient.Location = new System.Drawing.Point(4, 57);
			this._panelClient.Size = new System.Drawing.Size(1213, 677);
			//
			// _panelHeader
			//
			this._panelCaption.Controls.Add(this._layoutTitle);
			this._panelCaption.Padding = new System.Windows.Forms.Padding(1);
			this._panelCaption.Size = new System.Drawing.Size(1112, 53);
			//
			// _chart
			//
			this._chart.Dock = System.Windows.Forms.DockStyle.Fill;
			legend1.Name = "Legend1";
			this._chart.Legends.Add(legend1);
			this._chart.Location = new System.Drawing.Point(0, 120);
			this._chart.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._chart.Name = "_chart";
			this._chart.Size = new System.Drawing.Size(1213, 557);
			this._chart.TabIndex = 0;
			//
			// _buttonManaCurveType
			//
			this._buttonManaCurveType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonManaCurveType.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonManaCurveType.BackColor = System.Drawing.Color.Transparent;
			this._buttonManaCurveType.FlatAppearance.BorderSize = 0;
			this._buttonManaCurveType.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveType.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveType.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonManaCurveType.HighlightBackColor = System.Drawing.SystemColors.MenuHighlight;
			this._buttonManaCurveType.Location = new System.Drawing.Point(304, 0);
			this._buttonManaCurveType.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._buttonManaCurveType.Name = "_buttonManaCurveType";
			this._layoutTitle.SetRowSpan(this._buttonManaCurveType, 2);
			this._buttonManaCurveType.Size = new System.Drawing.Size(80, 50);
			this._buttonManaCurveType.TabIndex = 3;
			this._buttonManaCurveType.TabStop = false;
			this._buttonManaCurveType.Text = "Deck\r\nmana curve\r\n/ type";
			this._buttonManaCurveType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonManaCurveType.UseVisualStyleBackColor = true;
			//
			// _buttonDeckPrice
			//
			this._buttonDeckPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonDeckPrice.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDeckPrice.BackColor = System.Drawing.Color.Transparent;
			this._buttonDeckPrice.FlatAppearance.BorderSize = 0;
			this._buttonDeckPrice.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckPrice.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckPrice.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckPrice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDeckPrice.HighlightBackColor = System.Drawing.SystemColors.MenuHighlight;
			this._buttonDeckPrice.Location = new System.Drawing.Point(104, 0);
			this._buttonDeckPrice.Margin = new System.Windows.Forms.Padding(0);
			this._buttonDeckPrice.Name = "_buttonDeckPrice";
			this._layoutTitle.SetRowSpan(this._buttonDeckPrice, 2);
			this._buttonDeckPrice.Size = new System.Drawing.Size(70, 50);
			this._buttonDeckPrice.TabIndex = 4;
			this._buttonDeckPrice.TabStop = false;
			this._buttonDeckPrice.Text = "Deck\r\nprice";
			this._buttonDeckPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonDeckPrice.UseVisualStyleBackColor = true;
			//
			// _tabCols
			//
			this._tabCols.AddButtonWidth = 24;
			this._tabCols.AllowAddingTabs = false;
			this._tabCols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._tabCols.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_32;
			this._tabCols.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_32;
			this._tabCols.Location = new System.Drawing.Point(68, 0);
			this._tabCols.Margin = new System.Windows.Forms.Padding(0);
			this._tabCols.Name = "_tabCols";
			this._tabCols.Size = new System.Drawing.Size(4, 24);
			this._tabCols.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabCols.TabIndex = 1;
			this._tabCols.TextPadding = 4;
			//
			// _buttonApply
			//
			this._buttonApply.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonApply.Location = new System.Drawing.Point(604, 0);
			this._buttonApply.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonApply.Name = "_buttonApply";
			this._buttonApply.Size = new System.Drawing.Size(50, 24);
			this._buttonApply.TabIndex = 28;
			this._buttonApply.Text = "Build!";
			this._buttonApply.UseVisualStyleBackColor = false;
			//
			// _menuFields
			//
			this._menuFields.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuFields.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuFields.IntegralHeight = false;
			this._menuFields.Location = new System.Drawing.Point(49, 1);
			this._menuFields.Margin = new System.Windows.Forms.Padding(0);
			this._menuFields.MaxDropDownItems = 34;
			this._menuFields.Name = "_menuFields";
			this._menuFields.Size = new System.Drawing.Size(132, 21);
			this._menuFields.TabIndex = 40;
			this._menuFields.TabStop = false;
			//
			// _buttonAddCol
			//
			this._buttonAddCol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAddCol.Location = new System.Drawing.Point(197, 0);
			this._buttonAddCol.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonAddCol.Name = "_buttonAddCol";
			this._buttonAddCol.Size = new System.Drawing.Size(80, 24);
			this._buttonAddCol.TabIndex = 41;
			this._buttonAddCol.Text = "+ argument";
			this._buttonAddCol.UseVisualStyleBackColor = false;
			//
			// _panelFields
			//
			this._panelFields.AutoSize = true;
			this._panelFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelFields.Controls.Add(this._labelCols);
			this._panelFields.Controls.Add(this._tabCols);
			this._panelFields.Controls.Add(this._labelRows);
			this._panelFields.Controls.Add(this._tabRows);
			this._panelFields.Controls.Add(this._labelSum);
			this._panelFields.Controls.Add(this._tabSumm);
			this._panelFields.Controls.Add(this._labelSummarySort);
			this._panelFields.Controls.Add(this._tabSummSort);
			this._panelFields.Location = new System.Drawing.Point(0, 48);
			this._panelFields.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._panelFields.Name = "_panelFields";
			this._panelFields.Size = new System.Drawing.Size(310, 24);
			this._panelFields.TabIndex = 42;
			//
			// _labelCols
			//
			this._labelCols.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelCols.AutoSize = true;
			this._labelCols.Location = new System.Drawing.Point(4, 5);
			this._labelCols.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._labelCols.Name = "_labelCols";
			this._labelCols.Size = new System.Drawing.Size(60, 13);
			this._labelCols.TabIndex = 2;
			this._labelCols.Text = "Arguments:";
			//
			// _labelRows
			//
			this._labelRows.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelRows.AutoSize = true;
			this._labelRows.Location = new System.Drawing.Point(76, 5);
			this._labelRows.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._labelRows.Name = "_labelRows";
			this._labelRows.Size = new System.Drawing.Size(39, 13);
			this._labelRows.TabIndex = 0;
			this._labelRows.Text = "Series:";
			//
			// _tabRows
			//
			this._tabRows.AddButtonWidth = 24;
			this._tabRows.AllowAddingTabs = false;
			this._tabRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._tabRows.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_32;
			this._tabRows.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_32;
			this._tabRows.Location = new System.Drawing.Point(119, 0);
			this._tabRows.Margin = new System.Windows.Forms.Padding(0);
			this._tabRows.Name = "_tabRows";
			this._tabRows.Size = new System.Drawing.Size(4, 24);
			this._tabRows.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabRows.TabIndex = 3;
			this._tabRows.TextPadding = 4;
			//
			// _labelSum
			//
			this._labelSum.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelSum.AutoSize = true;
			this._labelSum.Location = new System.Drawing.Point(127, 5);
			this._labelSum.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._labelSum.Name = "_labelSum";
			this._labelSum.Size = new System.Drawing.Size(64, 13);
			this._labelSum.TabIndex = 4;
			this._labelSum.Text = "Aggregates:";
			//
			// _tabSumm
			//
			this._tabSumm.AddButtonWidth = 24;
			this._tabSumm.AllowAddingTabs = false;
			this._tabSumm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._tabSumm.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_32;
			this._tabSumm.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_32;
			this._tabSumm.Location = new System.Drawing.Point(195, 0);
			this._tabSumm.Margin = new System.Windows.Forms.Padding(0);
			this._tabSumm.Name = "_tabSumm";
			this._tabSumm.Size = new System.Drawing.Size(4, 24);
			this._tabSumm.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabSumm.TabIndex = 5;
			this._tabSumm.TextPadding = 4;
			//
			// _labelSummarySort
			//
			this._labelSummarySort.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelSummarySort.AutoSize = true;
			this._labelSummarySort.Location = new System.Drawing.Point(203, 5);
			this._labelSummarySort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this._labelSummarySort.Name = "_labelSummarySort";
			this._labelSummarySort.Size = new System.Drawing.Size(99, 13);
			this._labelSummarySort.TabIndex = 6;
			this._labelSummarySort.Text = "Sort by aggregates:";
			//
			// _tabSummSort
			//
			this._tabSummSort.AddButtonWidth = 24;
			this._tabSummSort.AllowAddingTabs = false;
			this._tabSummSort.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_32;
			this._tabSummSort.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_32;
			this._tabSummSort.Location = new System.Drawing.Point(306, 0);
			this._tabSummSort.Margin = new System.Windows.Forms.Padding(0);
			this._tabSummSort.Name = "_tabSummSort";
			this._tabSummSort.Size = new System.Drawing.Size(4, 24);
			this._tabSummSort.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabSummSort.TabIndex = 7;
			this._tabSummSort.TextPadding = 4;
			//
			// _progressBar
			//
			this._progressBar.Location = new System.Drawing.Point(686, 0);
			this._progressBar.Margin = new System.Windows.Forms.Padding(32, 0, 0, 0);
			this._progressBar.Maximum = 10;
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(300, 24);
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBar.TabIndex = 55;
			this._progressBar.Visible = false;
			//
			// _buttonArgumentTotals
			//
			this._buttonArgumentTotals.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonArgumentTotals.AutoSize = true;
			this._buttonArgumentTotals.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
			this._buttonArgumentTotals.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonArgumentTotals.Location = new System.Drawing.Point(3, 3);
			this._buttonArgumentTotals.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this._buttonArgumentTotals.Name = "_buttonArgumentTotals";
			this._buttonArgumentTotals.Size = new System.Drawing.Size(120, 17);
			this._buttonArgumentTotals.TabIndex = 56;
			this._buttonArgumentTotals.Text = "Show argument total";
			this._buttonArgumentTotals.UseVisualStyleBackColor = true;
			//
			// _buttonSeriesTotal
			//
			this._buttonSeriesTotal.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonSeriesTotal.AutoSize = true;
			this._buttonSeriesTotal.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
			this._buttonSeriesTotal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSeriesTotal.Location = new System.Drawing.Point(123, 3);
			this._buttonSeriesTotal.Margin = new System.Windows.Forms.Padding(0);
			this._buttonSeriesTotal.Name = "_buttonSeriesTotal";
			this._buttonSeriesTotal.Size = new System.Drawing.Size(103, 17);
			this._buttonSeriesTotal.TabIndex = 57;
			this._buttonSeriesTotal.Text = "Show series total";
			this._buttonSeriesTotal.UseVisualStyleBackColor = true;
			//
			// _buttonExplainTotal
			//
			this._buttonExplainTotal.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonExplainTotal.AutoSize = true;
			this._buttonExplainTotal.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
			this._buttonExplainTotal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExplainTotal.Location = new System.Drawing.Point(226, 3);
			this._buttonExplainTotal.Margin = new System.Windows.Forms.Padding(0);
			this._buttonExplainTotal.Name = "_buttonExplainTotal";
			this._buttonExplainTotal.Size = new System.Drawing.Size(80, 17);
			this._buttonExplainTotal.TabIndex = 58;
			this._buttonExplainTotal.Text = "Explain total";
			this._buttonExplainTotal.UseVisualStyleBackColor = true;
			//
			// _buttonApplyFilter
			//
			this._buttonApplyFilter.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonApplyFilter.AutoSize = true;
			this._buttonApplyFilter.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
			this._buttonApplyFilter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonApplyFilter.Location = new System.Drawing.Point(466, 3);
			this._buttonApplyFilter.Margin = new System.Windows.Forms.Padding(160, 0, 0, 0);
			this._buttonApplyFilter.Name = "_buttonApplyFilter";
			this._buttonApplyFilter.Size = new System.Drawing.Size(122, 17);
			this._buttonApplyFilter.TabIndex = 59;
			this._buttonApplyFilter.Text = "Filter by search result";
			this._buttonApplyFilter.UseVisualStyleBackColor = true;
			//
			// _buttonAddRow
			//
			this._buttonAddRow.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
			this._buttonAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAddRow.Location = new System.Drawing.Point(293, 0);
			this._buttonAddRow.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonAddRow.Name = "_buttonAddRow";
			this._buttonAddRow.Size = new System.Drawing.Size(60, 24);
			this._buttonAddRow.TabIndex = 42;
			this._buttonAddRow.Text = "+ series";
			this._buttonAddRow.UseVisualStyleBackColor = false;
			//
			// _buttonAddSum
			//
			this._buttonAddSum.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
			this._buttonAddSum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAddSum.Location = new System.Drawing.Point(369, 0);
			this._buttonAddSum.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonAddSum.Name = "_buttonAddSum";
			this._buttonAddSum.Size = new System.Drawing.Size(80, 24);
			this._buttonAddSum.TabIndex = 43;
			this._buttonAddSum.Text = "+ aggregate";
			this._buttonAddSum.UseVisualStyleBackColor = false;
			//
			// _panelMenu
			//
			this._panelMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelMenu.AutoSize = true;
			this._panelMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelMenu.Controls.Add(this._labelField);
			this._panelMenu.Controls.Add(this._menuFields);
			this._panelMenu.Controls.Add(this._buttonAddCol);
			this._panelMenu.Controls.Add(this._buttonAddRow);
			this._panelMenu.Controls.Add(this._buttonAddSum);
			this._panelMenu.Controls.Add(this._labelDataElement);
			this._panelMenu.Controls.Add(this._menuLabelDataElement);
			this._panelMenu.Controls.Add(this._labelDataSource);
			this._panelMenu.Controls.Add(this._menuDataSource);
			this._panelMenu.Controls.Add(this._labelChartType);
			this._panelMenu.Controls.Add(this._menuChartType);
			this._panelMenu.Location = new System.Drawing.Point(0, 12);
			this._panelMenu.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(1213, 24);
			this._panelMenu.TabIndex = 43;
			//
			// _labelField
			//
			this._labelField.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelField.AutoSize = true;
			this._labelField.Location = new System.Drawing.Point(8, 5);
			this._labelField.Margin = new System.Windows.Forms.Padding(8, 0, 4, 0);
			this._labelField.Name = "_labelField";
			this._labelField.Size = new System.Drawing.Size(37, 13);
			this._labelField.TabIndex = 50;
			this._labelField.Text = "Fields:";
			//
			// _labelDataElement
			//
			this._labelDataElement.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelDataElement.AutoSize = true;
			this._labelDataElement.Location = new System.Drawing.Point(497, 5);
			this._labelDataElement.Margin = new System.Windows.Forms.Padding(48, 0, 4, 0);
			this._labelDataElement.Name = "_labelDataElement";
			this._labelDataElement.Size = new System.Drawing.Size(58, 13);
			this._labelDataElement.TabIndex = 46;
			this._labelDataElement.Text = "Data label:";
			//
			// _menuLabelDataElement
			//
			this._menuLabelDataElement.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuLabelDataElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuLabelDataElement.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuLabelDataElement.IntegralHeight = false;
			this._menuLabelDataElement.Location = new System.Drawing.Point(559, 1);
			this._menuLabelDataElement.Margin = new System.Windows.Forms.Padding(0);
			this._menuLabelDataElement.Name = "_menuLabelDataElement";
			this._menuLabelDataElement.Size = new System.Drawing.Size(132, 21);
			this._menuLabelDataElement.TabIndex = 47;
			this._menuLabelDataElement.TabStop = false;
			//
			// _labelDataSource
			//
			this._labelDataSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelDataSource.AutoSize = true;
			this._labelDataSource.Location = new System.Drawing.Point(707, 5);
			this._labelDataSource.Margin = new System.Windows.Forms.Padding(16, 0, 4, 0);
			this._labelDataSource.Name = "_labelDataSource";
			this._labelDataSource.Size = new System.Drawing.Size(44, 13);
			this._labelDataSource.TabIndex = 45;
			this._labelDataSource.Text = "Source:";
			//
			// _menuDataSource
			//
			this._menuDataSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuDataSource.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuDataSource.IntegralHeight = false;
			this._menuDataSource.Location = new System.Drawing.Point(755, 1);
			this._menuDataSource.Margin = new System.Windows.Forms.Padding(0);
			this._menuDataSource.Name = "_menuDataSource";
			this._menuDataSource.Size = new System.Drawing.Size(132, 21);
			this._menuDataSource.TabIndex = 44;
			this._menuDataSource.TabStop = false;
			//
			// _labelChartType
			//
			this._labelChartType.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelChartType.AutoSize = true;
			this._labelChartType.Location = new System.Drawing.Point(903, 5);
			this._labelChartType.Margin = new System.Windows.Forms.Padding(16, 0, 4, 0);
			this._labelChartType.Name = "_labelChartType";
			this._labelChartType.Size = new System.Drawing.Size(58, 13);
			this._labelChartType.TabIndex = 48;
			this._labelChartType.Text = "Chart type:";
			//
			// _menuChartType
			//
			this._menuChartType.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuChartType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuChartType.IntegralHeight = false;
			this._menuChartType.Location = new System.Drawing.Point(965, 1);
			this._menuChartType.Margin = new System.Windows.Forms.Padding(0);
			this._menuChartType.MaxDropDownItems = 34;
			this._menuChartType.Name = "_menuChartType";
			this._menuChartType.Size = new System.Drawing.Size(132, 21);
			this._menuChartType.TabIndex = 49;
			this._menuChartType.TabStop = false;
			//
			// _panelTable
			//
			this._panelTable.BackColor = System.Drawing.SystemColors.Control;
			this._panelTable.ColumnCount = 1;
			this._panelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelTable.Controls.Add(this._panelMenu, 0, 0);
			this._panelTable.Controls.Add(this._panelFields, 0, 1);
			this._panelTable.Controls.Add(this._chart, 0, 3);
			this._panelTable.Controls.Add(this._panelFlags, 0, 2);
			this._panelTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._panelTable.Location = new System.Drawing.Point(0, 0);
			this._panelTable.Margin = new System.Windows.Forms.Padding(0);
			this._panelTable.Name = "_panelTable";
			this._panelTable.RowCount = 4;
			this._panelTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelTable.Size = new System.Drawing.Size(1213, 677);
			this._panelTable.TabIndex = 44;
			//
			// _panelFlags
			//
			this._panelFlags.AutoSize = true;
			this._panelFlags.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelFlags.Controls.Add(this._buttonArgumentTotals);
			this._panelFlags.Controls.Add(this._buttonSeriesTotal);
			this._panelFlags.Controls.Add(this._buttonExplainTotal);
			this._panelFlags.Controls.Add(this._buttonApplyFilter);
			this._panelFlags.Controls.Add(this._buttonApply);
			this._panelFlags.Controls.Add(this._progressBar);
			this._panelFlags.Location = new System.Drawing.Point(0, 84);
			this._panelFlags.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._panelFlags.Name = "_panelFlags";
			this._panelFlags.Size = new System.Drawing.Size(986, 24);
			this._panelFlags.TabIndex = 44;
			//
			// _menuPrice
			//
			this._menuPrice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuPrice.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuPrice.IntegralHeight = false;
			this._menuPrice.Location = new System.Drawing.Point(4, 4);
			this._menuPrice.Margin = new System.Windows.Forms.Padding(4, 4, 4, 0);
			this._menuPrice.Name = "_menuPrice";
			this._menuPrice.Size = new System.Drawing.Size(96, 21);
			this._menuPrice.TabIndex = 51;
			this._menuPrice.TabStop = false;
			//
			// _menuPriceChartType
			//
			this._menuPriceChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuPriceChartType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuPriceChartType.IntegralHeight = false;
			this._menuPriceChartType.Location = new System.Drawing.Point(4, 27);
			this._menuPriceChartType.Margin = new System.Windows.Forms.Padding(4, 2, 4, 0);
			this._menuPriceChartType.Name = "_menuPriceChartType";
			this._menuPriceChartType.Size = new System.Drawing.Size(96, 21);
			this._menuPriceChartType.TabIndex = 52;
			this._menuPriceChartType.TabStop = false;
			//
			// _buttonArtistsPerYear
			//
			this._buttonArtistsPerYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonArtistsPerYear.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonArtistsPerYear.BackColor = System.Drawing.Color.Transparent;
			this._buttonArtistsPerYear.FlatAppearance.BorderSize = 0;
			this._buttonArtistsPerYear.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonArtistsPerYear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonArtistsPerYear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonArtistsPerYear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonArtistsPerYear.HighlightBackColor = System.Drawing.SystemColors.MenuHighlight;
			this._buttonArtistsPerYear.Location = new System.Drawing.Point(724, 0);
			this._buttonArtistsPerYear.Margin = new System.Windows.Forms.Padding(0);
			this._buttonArtistsPerYear.Name = "_buttonArtistsPerYear";
			this._layoutTitle.SetRowSpan(this._buttonArtistsPerYear, 2);
			this._buttonArtistsPerYear.Size = new System.Drawing.Size(90, 50);
			this._buttonArtistsPerYear.TabIndex = 54;
			this._buttonArtistsPerYear.TabStop = false;
			this._buttonArtistsPerYear.Text = "Customization example: Artists per year";
			this._buttonArtistsPerYear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonArtistsPerYear.UseVisualStyleBackColor = true;
			//
			// _buttonCollectionPrice
			//
			this._buttonCollectionPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonCollectionPrice.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonCollectionPrice.BackColor = System.Drawing.Color.Transparent;
			this._buttonCollectionPrice.FlatAppearance.BorderSize = 0;
			this._buttonCollectionPrice.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionPrice.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionPrice.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionPrice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonCollectionPrice.HighlightBackColor = System.Drawing.SystemColors.MenuHighlight;
			this._buttonCollectionPrice.Location = new System.Drawing.Point(174, 0);
			this._buttonCollectionPrice.Margin = new System.Windows.Forms.Padding(0, 0, 60, 0);
			this._buttonCollectionPrice.Name = "_buttonCollectionPrice";
			this._layoutTitle.SetRowSpan(this._buttonCollectionPrice, 2);
			this._buttonCollectionPrice.Size = new System.Drawing.Size(70, 50);
			this._buttonCollectionPrice.TabIndex = 55;
			this._buttonCollectionPrice.TabStop = false;
			this._buttonCollectionPrice.Text = "Collection\r\nprice";
			this._buttonCollectionPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonCollectionPrice.UseVisualStyleBackColor = true;
			//
			// _layoutTitle
			//
			this._layoutTitle.AutoSize = true;
			this._layoutTitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._layoutTitle.BackColor = System.Drawing.SystemColors.Control;
			this._layoutTitle.ColumnCount = 8;
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._layoutTitle.Controls.Add(this._buttonManaCurveManacost, 4, 0);
			this._layoutTitle.Controls.Add(this._buttonCollectionColors, 6, 0);
			this._layoutTitle.Controls.Add(this._buttonDeckColors, 5, 0);
			this._layoutTitle.Controls.Add(this._buttonManaCurveType, 3, 0);
			this._layoutTitle.Controls.Add(this._menuPrice, 0, 0);
			this._layoutTitle.Controls.Add(this._menuPriceChartType, 0, 1);
			this._layoutTitle.Controls.Add(this._buttonArtistsPerYear, 7, 0);
			this._layoutTitle.Controls.Add(this._buttonDeckPrice, 1, 0);
			this._layoutTitle.Controls.Add(this._buttonCollectionPrice, 2, 0);
			this._layoutTitle.Location = new System.Drawing.Point(1, 1);
			this._layoutTitle.Margin = new System.Windows.Forms.Padding(1, 1, 0, 1);
			this._layoutTitle.Name = "_layoutTitle";
			this._layoutTitle.PaintBackground = false;
			this._layoutTitle.RowCount = 2;
			this._layoutTitle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._layoutTitle.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._layoutTitle.Size = new System.Drawing.Size(814, 50);
			this._layoutTitle.TabIndex = 56;
			this._layoutTitle.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			//
			// _buttonManaCurveManacost
			//
			this._buttonManaCurveManacost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonManaCurveManacost.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonManaCurveManacost.BackColor = System.Drawing.Color.Transparent;
			this._buttonManaCurveManacost.FlatAppearance.BorderSize = 0;
			this._buttonManaCurveManacost.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveManacost.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveManacost.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveManacost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonManaCurveManacost.HighlightBackColor = System.Drawing.SystemColors.MenuHighlight;
			this._buttonManaCurveManacost.Location = new System.Drawing.Point(404, 0);
			this._buttonManaCurveManacost.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._buttonManaCurveManacost.Name = "_buttonManaCurveManacost";
			this._layoutTitle.SetRowSpan(this._buttonManaCurveManacost, 2);
			this._buttonManaCurveManacost.Size = new System.Drawing.Size(80, 50);
			this._buttonManaCurveManacost.TabIndex = 58;
			this._buttonManaCurveManacost.TabStop = false;
			this._buttonManaCurveManacost.Text = "Deck\r\nmana curve\r\n/ color";
			this._buttonManaCurveManacost.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonManaCurveManacost.UseVisualStyleBackColor = true;
			//
			// _buttonCollectionColors
			//
			this._buttonCollectionColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonCollectionColors.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonCollectionColors.BackColor = System.Drawing.Color.Transparent;
			this._buttonCollectionColors.FlatAppearance.BorderSize = 0;
			this._buttonCollectionColors.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionColors.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionColors.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionColors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonCollectionColors.HighlightBackColor = System.Drawing.SystemColors.MenuHighlight;
			this._buttonCollectionColors.Location = new System.Drawing.Point(594, 0);
			this._buttonCollectionColors.Margin = new System.Windows.Forms.Padding(0, 0, 60, 0);
			this._buttonCollectionColors.Name = "_buttonCollectionColors";
			this._layoutTitle.SetRowSpan(this._buttonCollectionColors, 2);
			this._buttonCollectionColors.Size = new System.Drawing.Size(70, 50);
			this._buttonCollectionColors.TabIndex = 57;
			this._buttonCollectionColors.TabStop = false;
			this._buttonCollectionColors.Text = "Collection\r\ncolor\r\n/ type";
			this._buttonCollectionColors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonCollectionColors.UseVisualStyleBackColor = true;
			//
			// _buttonDeckColors
			//
			this._buttonDeckColors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonDeckColors.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDeckColors.BackColor = System.Drawing.Color.Transparent;
			this._buttonDeckColors.FlatAppearance.BorderSize = 0;
			this._buttonDeckColors.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckColors.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckColors.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckColors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDeckColors.HighlightBackColor = System.Drawing.SystemColors.MenuHighlight;
			this._buttonDeckColors.Location = new System.Drawing.Point(504, 0);
			this._buttonDeckColors.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this._buttonDeckColors.Name = "_buttonDeckColors";
			this._layoutTitle.SetRowSpan(this._buttonDeckColors, 2);
			this._buttonDeckColors.Size = new System.Drawing.Size(70, 50);
			this._buttonDeckColors.TabIndex = 56;
			this._buttonDeckColors.TabStop = false;
			this._buttonDeckColors.Text = "Deck\r\ncolor\r\n/ type";
			this._buttonDeckColors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonDeckColors.UseVisualStyleBackColor = true;
			//
			// FormChart
			//
			this.CaptionHeight = 57;
			this.ClientSize = new System.Drawing.Size(1221, 738);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(0, 0);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FormChart";
			this.Text = "Deck statistics";
			this._panelClient.ResumeLayout(false);
			this._panelCaption.ResumeLayout(false);
			this._panelCaption.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._chart)).EndInit();
			this._panelFields.ResumeLayout(false);
			this._panelFields.PerformLayout();
			this._panelMenu.ResumeLayout(false);
			this._panelMenu.PerformLayout();
			this._panelTable.ResumeLayout(false);
			this._panelTable.PerformLayout();
			this._panelFlags.ResumeLayout(false);
			this._panelFlags.PerformLayout();
			this._layoutTitle.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart _chart;
		private Mtgdb.Controls.CustomCheckBox _buttonManaCurveType;
		private Mtgdb.Controls.CustomCheckBox _buttonDeckPrice;
		private Controls.TabHeaderControl _tabCols;
		private System.Windows.Forms.Button _buttonApply;
		private System.Windows.Forms.ComboBox _menuFields;
		private System.Windows.Forms.Button _buttonAddCol;
		private System.Windows.Forms.FlowLayoutPanel _panelFields;
		private System.Windows.Forms.Label _labelRows;
		private System.Windows.Forms.Label _labelCols;
		private Controls.TabHeaderControl _tabRows;
		private System.Windows.Forms.Label _labelSum;
		private Controls.TabHeaderControl _tabSumm;
		private System.Windows.Forms.Button _buttonAddRow;
		private System.Windows.Forms.Button _buttonAddSum;
		private System.Windows.Forms.FlowLayoutPanel _panelMenu;
		private System.Windows.Forms.ComboBox _menuDataSource;
		private System.Windows.Forms.Label _labelDataSource;
		private System.Windows.Forms.Label _labelDataElement;
		private System.Windows.Forms.ComboBox _menuLabelDataElement;
		private System.Windows.Forms.Label _labelChartType;
		private System.Windows.Forms.ComboBox _menuChartType;
		private System.Windows.Forms.Label _labelField;
		private System.Windows.Forms.Label _labelSummarySort;
		private Controls.TabHeaderControl _tabSummSort;
		private System.Windows.Forms.TableLayoutPanel _panelTable;
		private System.Windows.Forms.ComboBox _menuPrice;
		private System.Windows.Forms.ComboBox _menuPriceChartType;
		private Mtgdb.Controls.CustomCheckBox _buttonArtistsPerYear;
		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.CheckBox _buttonArgumentTotals;
		private System.Windows.Forms.CheckBox _buttonSeriesTotal;
		private System.Windows.Forms.CheckBox _buttonExplainTotal;
		private Mtgdb.Controls.CustomCheckBox _buttonCollectionPrice;
		private System.Windows.Forms.CheckBox _buttonApplyFilter;
		private System.Windows.Forms.FlowLayoutPanel _panelFlags;
		private Mtgdb.Controls.BorderedTableLayoutPanel _layoutTitle;
		private Mtgdb.Controls.CustomCheckBox _buttonDeckColors;
		private Mtgdb.Controls.CustomCheckBox _buttonCollectionColors;
		private Mtgdb.Controls.CustomCheckBox _buttonManaCurveManacost;
	}
}