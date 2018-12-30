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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChart));
			this._chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this._buttonManaCurveType = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDeckPrice = new Mtgdb.Controls.CustomCheckBox();
			this._buttonApply = new System.Windows.Forms.Button();
			this._menuFields = new System.Windows.Forms.ComboBox();
			this._buttonAddCol = new System.Windows.Forms.Button();
			this._panelFields = new System.Windows.Forms.FlowLayoutPanel();
			this._labelCols = new System.Windows.Forms.Label();
			this._tabCols = new Mtgdb.Controls.TabHeaderControl();
			this._labelRows = new System.Windows.Forms.Label();
			this._tabRows = new Mtgdb.Controls.TabHeaderControl();
			this._labelSum = new System.Windows.Forms.Label();
			this._tabSumm = new Mtgdb.Controls.TabHeaderControl();
			this._labelSummarySort = new System.Windows.Forms.Label();
			this._tabSummSort = new Mtgdb.Controls.TabHeaderControl();
			this._buttonArgumentTotals = new System.Windows.Forms.CheckBox();
			this._buttonSeriesTotal = new System.Windows.Forms.CheckBox();
			this._buttonExplainTotal = new System.Windows.Forms.CheckBox();
			this._buttonFilterBySearchResult = new System.Windows.Forms.CheckBox();
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
			this._buttonCollectionPrice = new Mtgdb.Controls.CustomCheckBox();
			this._buttonManaCurveManacost = new Mtgdb.Controls.CustomCheckBox();
			this._buttonCollectionColors = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDeckColors = new Mtgdb.Controls.CustomCheckBox();
			this._labelTitle = new System.Windows.Forms.Label();
			this._flowTitle = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._flowFileMenu = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._buttonMruFiles = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLoad = new Mtgdb.Controls.CustomCheckBox();
			this._buttonSave = new Mtgdb.Controls.CustomCheckBox();
			this._flowPriceReports = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._flowDropdowns = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._menuMruFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._panelClient.SuspendLayout();
			this._panelCaption.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._chart)).BeginInit();
			this._panelFields.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._panelTable.SuspendLayout();
			this._panelFlags.SuspendLayout();
			this._flowTitle.SuspendLayout();
			this._flowFileMenu.SuspendLayout();
			this._flowPriceReports.SuspendLayout();
			this._flowDropdowns.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelClient
			// 
			this._panelClient.Controls.Add(this._panelTable);
			this._panelClient.Location = new System.Drawing.Point(6, 63);
			this._panelClient.Size = new System.Drawing.Size(1209, 669);
			// 
			// _panelCaption
			// 
			this._panelCaption.Controls.Add(this._flowTitle);
			this._panelCaption.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this._panelCaption.Size = new System.Drawing.Size(1104, 57);
			// 
			// _chart
			// 
			this._chart.Dock = System.Windows.Forms.DockStyle.Fill;
			this._chart.Location = new System.Drawing.Point(0, 120);
			this._chart.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._chart.Name = "_chart";
			this._chart.Size = new System.Drawing.Size(1209, 549);
			this._chart.TabIndex = 0;
			// 
			// _buttonManaCurveType
			// 
			this._buttonManaCurveType.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonManaCurveType.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonManaCurveType.BackColor = System.Drawing.Color.Transparent;
			this._buttonManaCurveType.FlatAppearance.BorderSize = 0;
			this._buttonManaCurveType.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveType.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveType.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonManaCurveType.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonManaCurveType.Location = new System.Drawing.Point(356, 8);
			this._buttonManaCurveType.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
			this._buttonManaCurveType.Name = "_buttonManaCurveType";
			this._buttonManaCurveType.Size = new System.Drawing.Size(76, 40);
			this._buttonManaCurveType.TabIndex = 3;
			this._buttonManaCurveType.TabStop = false;
			this._buttonManaCurveType.Text = "mana curve \r\n/ type";
			this._buttonManaCurveType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonManaCurveType.UseVisualStyleBackColor = true;
			// 
			// _buttonDeckPrice
			// 
			this._buttonDeckPrice.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonDeckPrice.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDeckPrice.BackColor = System.Drawing.Color.Transparent;
			this._buttonDeckPrice.FlatAppearance.BorderSize = 0;
			this._buttonDeckPrice.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckPrice.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckPrice.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckPrice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDeckPrice.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonDeckPrice.Location = new System.Drawing.Point(6, 5);
			this._buttonDeckPrice.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
			this._buttonDeckPrice.Name = "_buttonDeckPrice";
			this._buttonDeckPrice.Size = new System.Drawing.Size(50, 40);
			this._buttonDeckPrice.TabIndex = 4;
			this._buttonDeckPrice.TabStop = false;
			this._buttonDeckPrice.Text = "deck\r\nprice";
			this._buttonDeckPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonDeckPrice.UseVisualStyleBackColor = true;
			// 
			// _buttonApply
			// 
			this._buttonApply.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonApply.Location = new System.Drawing.Point(504, 0);
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
			// _tabCols
			// 
			this._tabCols.AllowAddingTabs = false;
			this._tabCols.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._tabCols.Location = new System.Drawing.Point(68, 0);
			this._tabCols.Margin = new System.Windows.Forms.Padding(0);
			this._tabCols.Name = "_tabCols";
			this._tabCols.Size = new System.Drawing.Size(4, 24);
			this._tabCols.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabCols.TabIndex = 1;
			this._tabCols.TextPadding = 4;
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
			this._tabRows.AllowAddingTabs = false;
			this._tabRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
			this._tabSumm.AllowAddingTabs = false;
			this._tabSumm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
			this._tabSummSort.AllowAddingTabs = false;
			this._tabSummSort.Location = new System.Drawing.Point(306, 0);
			this._tabSummSort.Margin = new System.Windows.Forms.Padding(0);
			this._tabSummSort.Name = "_tabSummSort";
			this._tabSummSort.Size = new System.Drawing.Size(4, 24);
			this._tabSummSort.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabSummSort.TabIndex = 7;
			this._tabSummSort.TextPadding = 4;
			// 
			// _buttonArgumentTotals
			// 
			this._buttonArgumentTotals.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonArgumentTotals.AutoSize = true;
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
			this._buttonExplainTotal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExplainTotal.Location = new System.Drawing.Point(226, 3);
			this._buttonExplainTotal.Margin = new System.Windows.Forms.Padding(0);
			this._buttonExplainTotal.Name = "_buttonExplainTotal";
			this._buttonExplainTotal.Size = new System.Drawing.Size(80, 17);
			this._buttonExplainTotal.TabIndex = 58;
			this._buttonExplainTotal.Text = "Explain total";
			this._buttonExplainTotal.UseVisualStyleBackColor = true;
			// 
			// _buttonFilterBySearchResult
			// 
			this._buttonFilterBySearchResult.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonFilterBySearchResult.AutoSize = true;
			this._buttonFilterBySearchResult.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder;
			this._buttonFilterBySearchResult.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonFilterBySearchResult.Location = new System.Drawing.Point(366, 3);
			this._buttonFilterBySearchResult.Margin = new System.Windows.Forms.Padding(60, 0, 0, 0);
			this._buttonFilterBySearchResult.Name = "_buttonFilterBySearchResult";
			this._buttonFilterBySearchResult.Size = new System.Drawing.Size(122, 17);
			this._buttonFilterBySearchResult.TabIndex = 59;
			this._buttonFilterBySearchResult.Text = "Filter by search result";
			this._buttonFilterBySearchResult.UseVisualStyleBackColor = true;
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
			this._panelMenu.Size = new System.Drawing.Size(1209, 24);
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
			this._panelTable.Size = new System.Drawing.Size(1209, 669);
			this._panelTable.TabIndex = 44;
			// 
			// _panelFlags
			// 
			this._panelFlags.AutoSize = true;
			this._panelFlags.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelFlags.Controls.Add(this._buttonArgumentTotals);
			this._panelFlags.Controls.Add(this._buttonSeriesTotal);
			this._panelFlags.Controls.Add(this._buttonExplainTotal);
			this._panelFlags.Controls.Add(this._buttonFilterBySearchResult);
			this._panelFlags.Controls.Add(this._buttonApply);
			this._panelFlags.Location = new System.Drawing.Point(0, 84);
			this._panelFlags.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._panelFlags.Name = "_panelFlags";
			this._panelFlags.Size = new System.Drawing.Size(554, 24);
			this._panelFlags.TabIndex = 44;
			// 
			// _menuPrice
			// 
			this._menuPrice.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this._menuPrice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuPrice.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuPrice.Location = new System.Drawing.Point(0, 0);
			this._menuPrice.Margin = new System.Windows.Forms.Padding(0);
			this._menuPrice.Name = "_menuPrice";
			this._menuPrice.Size = new System.Drawing.Size(60, 21);
			this._menuPrice.TabIndex = 51;
			this._menuPrice.TabStop = false;
			// 
			// _menuPriceChartType
			// 
			this._menuPriceChartType.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this._menuPriceChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuPriceChartType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuPriceChartType.Location = new System.Drawing.Point(0, 23);
			this._menuPriceChartType.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuPriceChartType.Name = "_menuPriceChartType";
			this._menuPriceChartType.Size = new System.Drawing.Size(60, 21);
			this._menuPriceChartType.TabIndex = 52;
			this._menuPriceChartType.TabStop = false;
			// 
			// _buttonCollectionPrice
			// 
			this._buttonCollectionPrice.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonCollectionPrice.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonCollectionPrice.BackColor = System.Drawing.Color.Transparent;
			this._buttonCollectionPrice.FlatAppearance.BorderSize = 0;
			this._buttonCollectionPrice.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionPrice.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionPrice.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionPrice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonCollectionPrice.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonCollectionPrice.Location = new System.Drawing.Point(56, 5);
			this._buttonCollectionPrice.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this._buttonCollectionPrice.Name = "_buttonCollectionPrice";
			this._buttonCollectionPrice.Size = new System.Drawing.Size(60, 40);
			this._buttonCollectionPrice.TabIndex = 55;
			this._buttonCollectionPrice.TabStop = false;
			this._buttonCollectionPrice.Text = "collection\r\nprice";
			this._buttonCollectionPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonCollectionPrice.UseVisualStyleBackColor = true;
			// 
			// _buttonManaCurveManacost
			// 
			this._buttonManaCurveManacost.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonManaCurveManacost.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonManaCurveManacost.BackColor = System.Drawing.Color.Transparent;
			this._buttonManaCurveManacost.FlatAppearance.BorderSize = 0;
			this._buttonManaCurveManacost.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveManacost.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveManacost.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonManaCurveManacost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonManaCurveManacost.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonManaCurveManacost.Location = new System.Drawing.Point(432, 8);
			this._buttonManaCurveManacost.Margin = new System.Windows.Forms.Padding(0);
			this._buttonManaCurveManacost.Name = "_buttonManaCurveManacost";
			this._buttonManaCurveManacost.Size = new System.Drawing.Size(76, 40);
			this._buttonManaCurveManacost.TabIndex = 58;
			this._buttonManaCurveManacost.TabStop = false;
			this._buttonManaCurveManacost.Text = "mana curve\r\n/ color";
			this._buttonManaCurveManacost.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonManaCurveManacost.UseVisualStyleBackColor = true;
			// 
			// _buttonCollectionColors
			// 
			this._buttonCollectionColors.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonCollectionColors.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonCollectionColors.BackColor = System.Drawing.Color.Transparent;
			this._buttonCollectionColors.FlatAppearance.BorderSize = 0;
			this._buttonCollectionColors.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionColors.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionColors.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonCollectionColors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonCollectionColors.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonCollectionColors.Location = new System.Drawing.Point(584, 8);
			this._buttonCollectionColors.Margin = new System.Windows.Forms.Padding(0);
			this._buttonCollectionColors.Name = "_buttonCollectionColors";
			this._buttonCollectionColors.Size = new System.Drawing.Size(90, 40);
			this._buttonCollectionColors.TabIndex = 57;
			this._buttonCollectionColors.TabStop = false;
			this._buttonCollectionColors.Text = "collection color\r\n/ type";
			this._buttonCollectionColors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonCollectionColors.UseVisualStyleBackColor = true;
			// 
			// _buttonDeckColors
			// 
			this._buttonDeckColors.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonDeckColors.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDeckColors.BackColor = System.Drawing.Color.Transparent;
			this._buttonDeckColors.FlatAppearance.BorderSize = 0;
			this._buttonDeckColors.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckColors.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckColors.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonDeckColors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDeckColors.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonDeckColors.Location = new System.Drawing.Point(508, 8);
			this._buttonDeckColors.Margin = new System.Windows.Forms.Padding(0);
			this._buttonDeckColors.Name = "_buttonDeckColors";
			this._buttonDeckColors.Size = new System.Drawing.Size(76, 40);
			this._buttonDeckColors.TabIndex = 56;
			this._buttonDeckColors.TabStop = false;
			this._buttonDeckColors.Text = "deck color\r\n/ type";
			this._buttonDeckColors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonDeckColors.UseVisualStyleBackColor = true;
			// 
			// _labelTitle
			// 
			this._labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._labelTitle.AutoSize = true;
			this._labelTitle.BackColor = System.Drawing.Color.Transparent;
			this._labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelTitle.Location = new System.Drawing.Point(6, 0);
			this._labelTitle.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
			this._labelTitle.Name = "_labelTitle";
			this._labelTitle.Size = new System.Drawing.Size(45, 50);
			this._labelTitle.TabIndex = 59;
			this._labelTitle.Text = "Name";
			this._labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _flowTitle
			// 
			this._flowTitle.AutoSize = true;
			this._flowTitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._flowTitle.BackColor = System.Drawing.Color.Transparent;
			this._flowTitle.Controls.Add(this._flowFileMenu);
			this._flowTitle.Controls.Add(this._flowPriceReports);
			this._flowTitle.Controls.Add(this._buttonManaCurveType);
			this._flowTitle.Controls.Add(this._buttonManaCurveManacost);
			this._flowTitle.Controls.Add(this._buttonDeckColors);
			this._flowTitle.Controls.Add(this._buttonCollectionColors);
			this._flowTitle.Location = new System.Drawing.Point(0, 0);
			this._flowTitle.Margin = new System.Windows.Forms.Padding(0);
			this._flowTitle.Name = "_flowTitle";
			this._flowTitle.Size = new System.Drawing.Size(674, 56);
			this._flowTitle.TabIndex = 57;
			this._flowTitle.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			this._flowTitle.WrapContents = false;
			// 
			// _flowFileMenu
			// 
			this._flowFileMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this._flowFileMenu.AutoSize = true;
			this._flowFileMenu.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._flowFileMenu.BorderColor = System.Drawing.SystemColors.ControlText;
			this._flowFileMenu.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			this._flowFileMenu.Controls.Add(this._labelTitle);
			this._flowFileMenu.Controls.Add(this._buttonMruFiles);
			this._flowFileMenu.Controls.Add(this._buttonLoad);
			this._flowFileMenu.Controls.Add(this._buttonSave);
			this._flowFileMenu.Location = new System.Drawing.Point(3, 3);
			this._flowFileMenu.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this._flowFileMenu.Name = "_flowFileMenu";
			this._flowFileMenu.Size = new System.Drawing.Size(153, 50);
			this._flowFileMenu.TabIndex = 63;
			this._flowFileMenu.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonMruFiles
			// 
			this._buttonMruFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonMruFiles.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonMruFiles.AutoCheck = false;
			this._buttonMruFiles.BackColor = System.Drawing.Color.Transparent;
			this._buttonMruFiles.FlatAppearance.BorderSize = 0;
			this._buttonMruFiles.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonMruFiles.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonMruFiles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonMruFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonMruFiles.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonMruFiles.Image = global::Mtgdb.Gui.Properties.Resources.down_32;
			this._buttonMruFiles.Location = new System.Drawing.Point(51, 13);
			this._buttonMruFiles.Margin = new System.Windows.Forms.Padding(0, 13, 0, 13);
			this._buttonMruFiles.Name = "_buttonMruFiles";
			this._buttonMruFiles.Size = new System.Drawing.Size(24, 24);
			this._buttonMruFiles.TabIndex = 62;
			this._buttonMruFiles.TabStop = false;
			this._buttonMruFiles.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonMruFiles.UseVisualStyleBackColor = true;
			// 
			// _buttonLoad
			// 
			this._buttonLoad.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLoad.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonLoad.AutoCheck = false;
			this._buttonLoad.BackColor = System.Drawing.Color.Transparent;
			this._buttonLoad.FlatAppearance.BorderSize = 0;
			this._buttonLoad.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonLoad.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonLoad.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLoad.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonLoad.Image = global::Mtgdb.Gui.Properties.Resources.open_16;
			this._buttonLoad.Location = new System.Drawing.Point(99, 13);
			this._buttonLoad.Margin = new System.Windows.Forms.Padding(24, 13, 0, 13);
			this._buttonLoad.Name = "_buttonLoad";
			this._buttonLoad.Size = new System.Drawing.Size(24, 24);
			this._buttonLoad.TabIndex = 58;
			this._buttonLoad.TabStop = false;
			this._buttonLoad.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonLoad.UseVisualStyleBackColor = true;
			// 
			// _buttonSave
			// 
			this._buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonSave.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSave.AutoCheck = false;
			this._buttonSave.BackColor = System.Drawing.Color.Transparent;
			this._buttonSave.FlatAppearance.BorderSize = 0;
			this._buttonSave.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
			this._buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSave.HighlightBackColor = System.Drawing.SystemColors.HotTrack;
			this._buttonSave.Image = global::Mtgdb.Gui.Properties.Resources.save_16;
			this._buttonSave.Location = new System.Drawing.Point(123, 13);
			this._buttonSave.Margin = new System.Windows.Forms.Padding(0, 13, 6, 13);
			this._buttonSave.Name = "_buttonSave";
			this._buttonSave.Size = new System.Drawing.Size(24, 24);
			this._buttonSave.TabIndex = 57;
			this._buttonSave.TabStop = false;
			this._buttonSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonSave.UseVisualStyleBackColor = true;
			// 
			// _flowPriceReports
			// 
			this._flowPriceReports.AutoSize = true;
			this._flowPriceReports.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._flowPriceReports.BorderColor = System.Drawing.SystemColors.ControlText;
			this._flowPriceReports.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			this._flowPriceReports.Controls.Add(this._buttonDeckPrice);
			this._flowPriceReports.Controls.Add(this._buttonCollectionPrice);
			this._flowPriceReports.Controls.Add(this._flowDropdowns);
			this._flowPriceReports.Location = new System.Drawing.Point(162, 3);
			this._flowPriceReports.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
			this._flowPriceReports.Name = "_flowPriceReports";
			this._flowPriceReports.Size = new System.Drawing.Size(188, 50);
			this._flowPriceReports.TabIndex = 61;
			this._flowPriceReports.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _flowDropdowns
			// 
			this._flowDropdowns.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._flowDropdowns.AutoSize = true;
			this._flowDropdowns.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._flowDropdowns.Controls.Add(this._menuPrice);
			this._flowDropdowns.Controls.Add(this._menuPriceChartType);
			this._flowDropdowns.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this._flowDropdowns.Location = new System.Drawing.Point(122, 3);
			this._flowDropdowns.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
			this._flowDropdowns.Name = "_flowDropdowns";
			this._flowDropdowns.Size = new System.Drawing.Size(60, 44);
			this._flowDropdowns.TabIndex = 60;
			this._flowDropdowns.VisibleBorders = System.Windows.Forms.AnchorStyles.None;
			this._flowDropdowns.WrapContents = false;
			// 
			// _menuMruFiles
			// 
			this._menuMruFiles.Name = "_menuMruFiles";
			this._menuMruFiles.Size = new System.Drawing.Size(61, 4);
			// 
			// FormChart
			// 
			this.CaptionHeight = 63;
			this.ClientSize = new System.Drawing.Size(1221, 738);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Location = new System.Drawing.Point(0, 0);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FormChart";
			this.Text = "Deck statistics";
			this.Controls.SetChildIndex(this._panelClient, 0);
			this.Controls.SetChildIndex(this._panelCaption, 0);
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
			this._flowTitle.ResumeLayout(false);
			this._flowTitle.PerformLayout();
			this._flowFileMenu.ResumeLayout(false);
			this._flowFileMenu.PerformLayout();
			this._flowPriceReports.ResumeLayout(false);
			this._flowPriceReports.PerformLayout();
			this._flowDropdowns.ResumeLayout(false);
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
		private System.Windows.Forms.CheckBox _buttonArgumentTotals;
		private System.Windows.Forms.CheckBox _buttonSeriesTotal;
		private System.Windows.Forms.CheckBox _buttonExplainTotal;
		private Mtgdb.Controls.CustomCheckBox _buttonCollectionPrice;
		private System.Windows.Forms.CheckBox _buttonFilterBySearchResult;
		private System.Windows.Forms.FlowLayoutPanel _panelFlags;
		private Mtgdb.Controls.CustomCheckBox _buttonDeckColors;
		private Mtgdb.Controls.CustomCheckBox _buttonCollectionColors;
		private Mtgdb.Controls.CustomCheckBox _buttonManaCurveManacost;
		private Controls.CustomCheckBox _buttonSave;
		private Controls.CustomCheckBox _buttonLoad;
		private System.Windows.Forms.Label _labelTitle;
		private Controls.BorderedFlowLayoutPanel _flowTitle;
		private Controls.BorderedFlowLayoutPanel _flowDropdowns;
		private Controls.BorderedFlowLayoutPanel _flowPriceReports;
		private Controls.CustomCheckBox _buttonMruFiles;
		private System.Windows.Forms.ContextMenuStrip _menuMruFiles;
		private Controls.BorderedFlowLayoutPanel _flowFileMenu;
	}
}