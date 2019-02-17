
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
			this._buttonManaCurveType = new Mtgdb.Controls.ButtonBase();
			this._buttonDeckPrice = new Mtgdb.Controls.ButtonBase();
			this._buttonApply = new Mtgdb.Controls.ButtonBase();
			this._menuFields = new Mtgdb.Controls.DropDown();
			this._buttonAddCol = new Mtgdb.Controls.ButtonBase();
			this._panelFields = new System.Windows.Forms.FlowLayoutPanel();
			this._labelCols = new System.Windows.Forms.Label();
			this._tabCols = new Mtgdb.Controls.TabHeaderControl();
			this._labelRows = new System.Windows.Forms.Label();
			this._tabRows = new Mtgdb.Controls.TabHeaderControl();
			this._labelSum = new System.Windows.Forms.Label();
			this._tabSumm = new Mtgdb.Controls.TabHeaderControl();
			this._labelSummarySort = new System.Windows.Forms.Label();
			this._tabSummSort = new Mtgdb.Controls.TabHeaderControl();
			this._buttonArgumentTotal = new Mtgdb.Controls.CheckBox();
			this._buttonSeriesTotal = new Mtgdb.Controls.CheckBox();
			this._buttonExplainTotal = new Mtgdb.Controls.CheckBox();
			this._buttonFilterBySearchResult = new Mtgdb.Controls.CheckBox();
			this._buttonAddRow = new Mtgdb.Controls.ButtonBase();
			this._buttonAddSum = new Mtgdb.Controls.ButtonBase();
			this._panelMenu = new System.Windows.Forms.FlowLayoutPanel();
			this._labelField = new System.Windows.Forms.Label();
			this._labelDataElement = new System.Windows.Forms.Label();
			this._menuLabelDataElement = new Mtgdb.Controls.DropDown();
			this._labelDataSource = new System.Windows.Forms.Label();
			this._menuDataSource = new Mtgdb.Controls.DropDown();
			this._labelChartType = new System.Windows.Forms.Label();
			this._menuChartType = new Mtgdb.Controls.DropDown();
			this._panelTable = new System.Windows.Forms.TableLayoutPanel();
			this._panelFlags = new System.Windows.Forms.FlowLayoutPanel();
			this._menuPrice = new Mtgdb.Controls.DropDown();
			this._menuPriceChartType = new Mtgdb.Controls.DropDown();
			this._buttonCollectionPrice = new Mtgdb.Controls.ButtonBase();
			this._buttonManaCurveManacost = new Mtgdb.Controls.ButtonBase();
			this._buttonCollectionColors = new Mtgdb.Controls.ButtonBase();
			this._buttonDeckColors = new Mtgdb.Controls.ButtonBase();
			this._labelTitle = new System.Windows.Forms.Label();
			this._flowTitle = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._flowFileMenu = new Mtgdb.Controls.BorderedFlowLayoutPanel();
			this._dropdownMruFiles = new Mtgdb.Controls.Popup();
			this._buttonLoad = new Mtgdb.Controls.ButtonBase();
			this._buttonSave = new Mtgdb.Controls.ButtonBase();
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
			this._panelClient.Location = new System.Drawing.Point(6, 68);
			this._panelClient.Size = new System.Drawing.Size(1209, 664);
			// 
			// _panelCaption
			// 
			this._panelCaption.Controls.Add(this._flowTitle);
			this._panelCaption.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
			this._panelCaption.Size = new System.Drawing.Size(1104, 62);
			// 
			// _chart
			// 
			this._chart.Dock = System.Windows.Forms.DockStyle.Fill;
			this._chart.Location = new System.Drawing.Point(0, 120);
			this._chart.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._chart.Name = "_chart";
			this._chart.Size = new System.Drawing.Size(1209, 544);
			this._chart.TabIndex = 0;
			// 
			// _buttonManaCurveType
			// 
			this._buttonManaCurveType.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonManaCurveType.BackColor = System.Drawing.Color.Transparent;
			this._buttonManaCurveType.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonManaCurveType.Location = new System.Drawing.Point(356, 11);
			this._buttonManaCurveType.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
			this._buttonManaCurveType.Name = "_buttonManaCurveType";
			this._buttonManaCurveType.Padding = new System.Windows.Forms.Padding(4);
			this._buttonManaCurveType.Size = new System.Drawing.Size(76, 40);
			this._buttonManaCurveType.TabIndex = 3;
			this._buttonManaCurveType.Text = "mana curve \r\n/ type";
			// 
			// _buttonDeckPrice
			// 
			this._buttonDeckPrice.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonDeckPrice.BackColor = System.Drawing.Color.Transparent;
			this._buttonDeckPrice.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonDeckPrice.Location = new System.Drawing.Point(6, 8);
			this._buttonDeckPrice.Margin = new System.Windows.Forms.Padding(6, 3, 0, 3);
			this._buttonDeckPrice.Name = "_buttonDeckPrice";
			this._buttonDeckPrice.Padding = new System.Windows.Forms.Padding(4);
			this._buttonDeckPrice.Size = new System.Drawing.Size(50, 40);
			this._buttonDeckPrice.TabIndex = 4;
			this._buttonDeckPrice.Text = "deck\r\nprice";
			// 
			// _buttonApply
			// 
			this._buttonApply.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonApply.AutoCheck = false;
			this._buttonApply.BackColor = System.Drawing.Color.Transparent;
			this._buttonApply.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonApply.Location = new System.Drawing.Point(435, 0);
			this._buttonApply.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonApply.Name = "_buttonApply";
			this._buttonApply.Padding = new System.Windows.Forms.Padding(4);
			this._buttonApply.Size = new System.Drawing.Size(50, 24);
			this._buttonApply.TabIndex = 28;
			this._buttonApply.Text = "Build!";
			this._buttonApply.VisibleAllBorders = true;
			this._buttonApply.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _menuFields
			// 
			this._menuFields.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuFields.EmptySelectionText = "";
			this._menuFields.Location = new System.Drawing.Point(49, 0);
			this._menuFields.Margin = new System.Windows.Forms.Padding(0);
			this._menuFields.Name = "_menuFields";
			this._menuFields.SelectedIndex = -1;
			this._menuFields.Size = new System.Drawing.Size(132, 24);
			this._menuFields.TabIndex = 40;
			// 
			// _buttonAddCol
			// 
			this._buttonAddCol.AutoCheck = false;
			this._buttonAddCol.BackColor = System.Drawing.Color.Transparent;
			this._buttonAddCol.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonAddCol.Location = new System.Drawing.Point(197, 0);
			this._buttonAddCol.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonAddCol.Name = "_buttonAddCol";
			this._buttonAddCol.Padding = new System.Windows.Forms.Padding(4);
			this._buttonAddCol.Size = new System.Drawing.Size(64, 24);
			this._buttonAddCol.TabIndex = 41;
			this._buttonAddCol.Text = "+ argument";
			this._buttonAddCol.VisibleAllBorders = true;
			this._buttonAddCol.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
			// _buttonArgumentTotal
			// 
			this._buttonArgumentTotal.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonArgumentTotal.BackColor = System.Drawing.Color.Transparent;
			this._buttonArgumentTotal.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonArgumentTotal.Location = new System.Drawing.Point(3, 0);
			this._buttonArgumentTotal.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this._buttonArgumentTotal.Name = "_buttonArgumentTotal";
			this._buttonArgumentTotal.Padding = new System.Windows.Forms.Padding(4);
			this._buttonArgumentTotal.Size = new System.Drawing.Size(99, 24);
			this._buttonArgumentTotal.TabIndex = 56;
			this._buttonArgumentTotal.Text = "Argument total";
			// 
			// _buttonSeriesTotal
			// 
			this._buttonSeriesTotal.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonSeriesTotal.BackColor = System.Drawing.Color.Transparent;
			this._buttonSeriesTotal.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonSeriesTotal.Location = new System.Drawing.Point(102, 0);
			this._buttonSeriesTotal.Margin = new System.Windows.Forms.Padding(0);
			this._buttonSeriesTotal.Name = "_buttonSeriesTotal";
			this._buttonSeriesTotal.Padding = new System.Windows.Forms.Padding(4);
			this._buttonSeriesTotal.Size = new System.Drawing.Size(83, 24);
			this._buttonSeriesTotal.TabIndex = 57;
			this._buttonSeriesTotal.Text = "Series total";
			// 
			// _buttonExplainTotal
			// 
			this._buttonExplainTotal.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonExplainTotal.BackColor = System.Drawing.Color.Transparent;
			this._buttonExplainTotal.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonExplainTotal.Location = new System.Drawing.Point(185, 0);
			this._buttonExplainTotal.Margin = new System.Windows.Forms.Padding(0);
			this._buttonExplainTotal.Name = "_buttonExplainTotal";
			this._buttonExplainTotal.Padding = new System.Windows.Forms.Padding(4);
			this._buttonExplainTotal.Size = new System.Drawing.Size(88, 24);
			this._buttonExplainTotal.TabIndex = 58;
			this._buttonExplainTotal.Text = "Explain total";
			// 
			// _buttonFilterBySearchResult
			// 
			this._buttonFilterBySearchResult.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonFilterBySearchResult.BackColor = System.Drawing.Color.Transparent;
			this._buttonFilterBySearchResult.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonFilterBySearchResult.Location = new System.Drawing.Point(289, 0);
			this._buttonFilterBySearchResult.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonFilterBySearchResult.Name = "_buttonFilterBySearchResult";
			this._buttonFilterBySearchResult.Padding = new System.Windows.Forms.Padding(4);
			this._buttonFilterBySearchResult.Size = new System.Drawing.Size(130, 24);
			this._buttonFilterBySearchResult.TabIndex = 59;
			this._buttonFilterBySearchResult.Text = "Filter by search result";
			// 
			// _buttonAddRow
			// 
			this._buttonAddRow.AutoCheck = false;
			this._buttonAddRow.BackColor = System.Drawing.Color.Transparent;
			this._buttonAddRow.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonAddRow.Location = new System.Drawing.Point(277, 0);
			this._buttonAddRow.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonAddRow.Name = "_buttonAddRow";
			this._buttonAddRow.Padding = new System.Windows.Forms.Padding(4);
			this._buttonAddRow.Size = new System.Drawing.Size(48, 24);
			this._buttonAddRow.TabIndex = 42;
			this._buttonAddRow.Text = "+ series";
			this._buttonAddRow.VisibleAllBorders = true;
			this._buttonAddRow.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonAddSum
			// 
			this._buttonAddSum.AutoCheck = false;
			this._buttonAddSum.BackColor = System.Drawing.Color.Transparent;
			this._buttonAddSum.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonAddSum.Location = new System.Drawing.Point(341, 0);
			this._buttonAddSum.Margin = new System.Windows.Forms.Padding(16, 0, 0, 0);
			this._buttonAddSum.Name = "_buttonAddSum";
			this._buttonAddSum.Padding = new System.Windows.Forms.Padding(4);
			this._buttonAddSum.Size = new System.Drawing.Size(72, 24);
			this._buttonAddSum.TabIndex = 43;
			this._buttonAddSum.Text = "+ aggregate";
			this._buttonAddSum.VisibleAllBorders = true;
			this._buttonAddSum.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
			this._labelDataElement.Location = new System.Drawing.Point(461, 5);
			this._labelDataElement.Margin = new System.Windows.Forms.Padding(48, 0, 4, 0);
			this._labelDataElement.Name = "_labelDataElement";
			this._labelDataElement.Size = new System.Drawing.Size(58, 13);
			this._labelDataElement.TabIndex = 46;
			this._labelDataElement.Text = "Data label:";
			// 
			// _menuLabelDataElement
			// 
			this._menuLabelDataElement.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuLabelDataElement.EmptySelectionText = "";
			this._menuLabelDataElement.Location = new System.Drawing.Point(523, 0);
			this._menuLabelDataElement.Margin = new System.Windows.Forms.Padding(0);
			this._menuLabelDataElement.Name = "_menuLabelDataElement";
			this._menuLabelDataElement.SelectedIndex = -1;
			this._menuLabelDataElement.Size = new System.Drawing.Size(132, 24);
			this._menuLabelDataElement.TabIndex = 47;
			this._menuLabelDataElement.TabStop = false;
			// 
			// _labelDataSource
			// 
			this._labelDataSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelDataSource.AutoSize = true;
			this._labelDataSource.Location = new System.Drawing.Point(671, 5);
			this._labelDataSource.Margin = new System.Windows.Forms.Padding(16, 0, 4, 0);
			this._labelDataSource.Name = "_labelDataSource";
			this._labelDataSource.Size = new System.Drawing.Size(44, 13);
			this._labelDataSource.TabIndex = 45;
			this._labelDataSource.Text = "Source:";
			// 
			// _menuDataSource
			// 
			this._menuDataSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuDataSource.EmptySelectionText = "";
			this._menuDataSource.Location = new System.Drawing.Point(719, 0);
			this._menuDataSource.Margin = new System.Windows.Forms.Padding(0);
			this._menuDataSource.Name = "_menuDataSource";
			this._menuDataSource.SelectedIndex = -1;
			this._menuDataSource.Size = new System.Drawing.Size(132, 24);
			this._menuDataSource.TabIndex = 44;
			this._menuDataSource.TabStop = false;
			// 
			// _labelChartType
			// 
			this._labelChartType.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._labelChartType.AutoSize = true;
			this._labelChartType.Location = new System.Drawing.Point(867, 5);
			this._labelChartType.Margin = new System.Windows.Forms.Padding(16, 0, 4, 0);
			this._labelChartType.Name = "_labelChartType";
			this._labelChartType.Size = new System.Drawing.Size(58, 13);
			this._labelChartType.TabIndex = 48;
			this._labelChartType.Text = "Chart type:";
			// 
			// _menuChartType
			// 
			this._menuChartType.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._menuChartType.EmptySelectionText = "";
			this._menuChartType.Location = new System.Drawing.Point(929, 0);
			this._menuChartType.Margin = new System.Windows.Forms.Padding(0);
			this._menuChartType.Name = "_menuChartType";
			this._menuChartType.SelectedIndex = -1;
			this._menuChartType.Size = new System.Drawing.Size(132, 24);
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
			this._panelTable.Size = new System.Drawing.Size(1209, 664);
			this._panelTable.TabIndex = 44;
			// 
			// _panelFlags
			// 
			this._panelFlags.AutoSize = true;
			this._panelFlags.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._panelFlags.Controls.Add(this._buttonArgumentTotal);
			this._panelFlags.Controls.Add(this._buttonSeriesTotal);
			this._panelFlags.Controls.Add(this._buttonExplainTotal);
			this._panelFlags.Controls.Add(this._buttonFilterBySearchResult);
			this._panelFlags.Controls.Add(this._buttonApply);
			this._panelFlags.Location = new System.Drawing.Point(0, 84);
			this._panelFlags.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
			this._panelFlags.Name = "_panelFlags";
			this._panelFlags.Size = new System.Drawing.Size(485, 24);
			this._panelFlags.TabIndex = 44;
			// 
			// _menuPrice
			// 
			this._menuPrice.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this._menuPrice.EmptySelectionText = "";
			this._menuPrice.Location = new System.Drawing.Point(0, 0);
			this._menuPrice.Margin = new System.Windows.Forms.Padding(0);
			this._menuPrice.Name = "_menuPrice";
			this._menuPrice.SelectedIndex = -1;
			this._menuPrice.Size = new System.Drawing.Size(60, 24);
			this._menuPrice.TabIndex = 51;
			this._menuPrice.TabStop = false;
			// 
			// _menuPriceChartType
			// 
			this._menuPriceChartType.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this._menuPriceChartType.EmptySelectionText = "";
			this._menuPriceChartType.Location = new System.Drawing.Point(0, 26);
			this._menuPriceChartType.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuPriceChartType.Name = "_menuPriceChartType";
			this._menuPriceChartType.SelectedIndex = -1;
			this._menuPriceChartType.Size = new System.Drawing.Size(60, 24);
			this._menuPriceChartType.TabIndex = 52;
			this._menuPriceChartType.TabStop = false;
			// 
			// _buttonCollectionPrice
			// 
			this._buttonCollectionPrice.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonCollectionPrice.BackColor = System.Drawing.Color.Transparent;
			this._buttonCollectionPrice.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonCollectionPrice.Location = new System.Drawing.Point(56, 8);
			this._buttonCollectionPrice.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this._buttonCollectionPrice.Name = "_buttonCollectionPrice";
			this._buttonCollectionPrice.Padding = new System.Windows.Forms.Padding(4);
			this._buttonCollectionPrice.Size = new System.Drawing.Size(60, 40);
			this._buttonCollectionPrice.TabIndex = 55;
			this._buttonCollectionPrice.Text = "collection\r\nprice";
			// 
			// _buttonManaCurveManacost
			// 
			this._buttonManaCurveManacost.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonManaCurveManacost.BackColor = System.Drawing.Color.Transparent;
			this._buttonManaCurveManacost.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonManaCurveManacost.Location = new System.Drawing.Point(432, 11);
			this._buttonManaCurveManacost.Margin = new System.Windows.Forms.Padding(0);
			this._buttonManaCurveManacost.Name = "_buttonManaCurveManacost";
			this._buttonManaCurveManacost.Padding = new System.Windows.Forms.Padding(4);
			this._buttonManaCurveManacost.Size = new System.Drawing.Size(76, 40);
			this._buttonManaCurveManacost.TabIndex = 58;
			this._buttonManaCurveManacost.Text = "mana curve\r\n/ color";
			// 
			// _buttonCollectionColors
			// 
			this._buttonCollectionColors.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonCollectionColors.BackColor = System.Drawing.Color.Transparent;
			this._buttonCollectionColors.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonCollectionColors.Location = new System.Drawing.Point(584, 11);
			this._buttonCollectionColors.Margin = new System.Windows.Forms.Padding(0);
			this._buttonCollectionColors.Name = "_buttonCollectionColors";
			this._buttonCollectionColors.Padding = new System.Windows.Forms.Padding(4);
			this._buttonCollectionColors.Size = new System.Drawing.Size(90, 40);
			this._buttonCollectionColors.TabIndex = 57;
			this._buttonCollectionColors.Text = "collection color\r\n/ type";
			// 
			// _buttonDeckColors
			// 
			this._buttonDeckColors.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonDeckColors.BackColor = System.Drawing.Color.Transparent;
			this._buttonDeckColors.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonDeckColors.Location = new System.Drawing.Point(508, 11);
			this._buttonDeckColors.Margin = new System.Windows.Forms.Padding(0);
			this._buttonDeckColors.Name = "_buttonDeckColors";
			this._buttonDeckColors.Padding = new System.Windows.Forms.Padding(4);
			this._buttonDeckColors.Size = new System.Drawing.Size(76, 40);
			this._buttonDeckColors.TabIndex = 56;
			this._buttonDeckColors.Text = "deck color\r\n/ type";
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
			this._flowTitle.Size = new System.Drawing.Size(674, 62);
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
			this._flowFileMenu.Controls.Add(this._dropdownMruFiles);
			this._flowFileMenu.Controls.Add(this._buttonLoad);
			this._flowFileMenu.Controls.Add(this._buttonSave);
			this._flowFileMenu.Location = new System.Drawing.Point(3, 3);
			this._flowFileMenu.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this._flowFileMenu.Name = "_flowFileMenu";
			this._flowFileMenu.Size = new System.Drawing.Size(153, 56);
			this._flowFileMenu.TabIndex = 63;
			// 
			// _dropdownMruFiles
			// 
			this._dropdownMruFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._dropdownMruFiles.BackColor = System.Drawing.Color.Transparent;
			this._dropdownMruFiles.ForeColor = System.Drawing.SystemColors.ControlText;
			this._dropdownMruFiles.Image = global::Mtgdb.Gui.Properties.Resources.down_32;
			this._dropdownMruFiles.Location = new System.Drawing.Point(51, 13);
			this._dropdownMruFiles.Margin = new System.Windows.Forms.Padding(0, 13, 0, 13);
			this._dropdownMruFiles.MenuControl = null;
			this._dropdownMruFiles.Name = "_dropdownMruFiles";
			this._dropdownMruFiles.Padding = new System.Windows.Forms.Padding(4);
			this._dropdownMruFiles.Size = new System.Drawing.Size(24, 24);
			this._dropdownMruFiles.TabIndex = 62;
			// 
			// _buttonLoad
			// 
			this._buttonLoad.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonLoad.AutoCheck = false;
			this._buttonLoad.BackColor = System.Drawing.Color.Transparent;
			this._buttonLoad.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonLoad.Image = global::Mtgdb.Gui.Properties.Resources.open_16;
			this._buttonLoad.Location = new System.Drawing.Point(99, 13);
			this._buttonLoad.Margin = new System.Windows.Forms.Padding(24, 13, 0, 13);
			this._buttonLoad.Name = "_buttonLoad";
			this._buttonLoad.Padding = new System.Windows.Forms.Padding(4);
			this._buttonLoad.Size = new System.Drawing.Size(24, 24);
			this._buttonLoad.TabIndex = 58;
			// 
			// _buttonSave
			// 
			this._buttonSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this._buttonSave.AutoCheck = false;
			this._buttonSave.BackColor = System.Drawing.Color.Transparent;
			this._buttonSave.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonSave.Image = global::Mtgdb.Gui.Properties.Resources.save_16;
			this._buttonSave.Location = new System.Drawing.Point(123, 13);
			this._buttonSave.Margin = new System.Windows.Forms.Padding(0, 13, 6, 13);
			this._buttonSave.Name = "_buttonSave";
			this._buttonSave.Padding = new System.Windows.Forms.Padding(4);
			this._buttonSave.Size = new System.Drawing.Size(24, 24);
			this._buttonSave.TabIndex = 57;
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
			this._flowPriceReports.Size = new System.Drawing.Size(188, 56);
			this._flowPriceReports.TabIndex = 61;
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
			this._flowDropdowns.Size = new System.Drawing.Size(60, 50);
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
			this.CaptionHeight = 68;
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
		private Mtgdb.Controls.ButtonBase _buttonManaCurveType;
		private Mtgdb.Controls.ButtonBase _buttonDeckPrice;
		private Controls.TabHeaderControl _tabCols;
		private Mtgdb.Controls.ButtonBase _buttonApply;
		private Mtgdb.Controls.DropDown _menuFields;
		private Mtgdb.Controls.ButtonBase _buttonAddCol;
		private System.Windows.Forms.FlowLayoutPanel _panelFields;
		private System.Windows.Forms.Label _labelRows;
		private System.Windows.Forms.Label _labelCols;
		private Controls.TabHeaderControl _tabRows;
		private System.Windows.Forms.Label _labelSum;
		private Controls.TabHeaderControl _tabSumm;
		private Mtgdb.Controls.ButtonBase _buttonAddRow;
		private Mtgdb.Controls.ButtonBase _buttonAddSum;
		private System.Windows.Forms.FlowLayoutPanel _panelMenu;
		private Mtgdb.Controls.DropDown _menuDataSource;
		private System.Windows.Forms.Label _labelDataSource;
		private System.Windows.Forms.Label _labelDataElement;
		private Mtgdb.Controls.DropDown _menuLabelDataElement;
		private System.Windows.Forms.Label _labelChartType;
		private Mtgdb.Controls.DropDown _menuChartType;
		private System.Windows.Forms.Label _labelField;
		private System.Windows.Forms.Label _labelSummarySort;
		private Controls.TabHeaderControl _tabSummSort;
		private System.Windows.Forms.TableLayoutPanel _panelTable;
		private Mtgdb.Controls.DropDown _menuPrice;
		private Mtgdb.Controls.DropDown _menuPriceChartType;
		private Mtgdb.Controls.CheckBox _buttonArgumentTotal;
		private Mtgdb.Controls.CheckBox _buttonSeriesTotal;
		private Mtgdb.Controls.CheckBox _buttonExplainTotal;
		private Mtgdb.Controls.ButtonBase _buttonCollectionPrice;
		private Mtgdb.Controls.CheckBox _buttonFilterBySearchResult;
		private System.Windows.Forms.FlowLayoutPanel _panelFlags;
		private Mtgdb.Controls.ButtonBase _buttonDeckColors;
		private Mtgdb.Controls.ButtonBase _buttonCollectionColors;
		private Mtgdb.Controls.ButtonBase _buttonManaCurveManacost;
		private Mtgdb.Controls.ButtonBase _buttonSave;
		private Mtgdb.Controls.ButtonBase _buttonLoad;
		private System.Windows.Forms.Label _labelTitle;
		private Controls.BorderedFlowLayoutPanel _flowTitle;
		private Controls.BorderedFlowLayoutPanel _flowDropdowns;
		private Controls.BorderedFlowLayoutPanel _flowPriceReports;
		private Mtgdb.Controls.Popup _dropdownMruFiles;
		private System.Windows.Forms.ContextMenuStrip _menuMruFiles;
		private Controls.BorderedFlowLayoutPanel _flowFileMenu;
	}
}