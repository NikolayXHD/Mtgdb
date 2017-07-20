﻿namespace Mtgdb.Gui
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
			this._buttonManaCurve = new System.Windows.Forms.CheckBox();
			this._buttonDeckPrice = new System.Windows.Forms.CheckBox();
			this._tabCols = new Mtgdb.Controls.TabHeaderControl();
			this._buttonApply = new System.Windows.Forms.Button();
			this._menuFields = new System.Windows.Forms.ComboBox();
			this._buttonAddCol = new System.Windows.Forms.Button();
			this._panelFields = new System.Windows.Forms.FlowLayoutPanel();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._labelCols = new System.Windows.Forms.Label();
			this._labelRows = new System.Windows.Forms.Label();
			this._tabRows = new Mtgdb.Controls.TabHeaderControl();
			this._labelSum = new System.Windows.Forms.Label();
			this._tabSumm = new Mtgdb.Controls.TabHeaderControl();
			this._labelSummarySort = new System.Windows.Forms.Label();
			this._tabSummSort = new Mtgdb.Controls.TabHeaderControl();
			this._buttonArgumentTotals = new System.Windows.Forms.CheckBox();
			this._buttonSeriesTotal = new System.Windows.Forms.CheckBox();
			this._buttonExplainTotal = new System.Windows.Forms.CheckBox();
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
			this._menuPrice = new System.Windows.Forms.ComboBox();
			this._menuPriceChartType = new System.Windows.Forms.ComboBox();
			this._buttonArtistsPerYear = new System.Windows.Forms.CheckBox();
			this._panelClient.SuspendLayout();
			this._panelHeader.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._chart)).BeginInit();
			this._panelFields.SuspendLayout();
			this._panelMenu.SuspendLayout();
			this._panelTable.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelClient
			// 
			this._panelClient.Controls.Add(this._panelTable);
			this._panelClient.Location = new System.Drawing.Point(4, 57);
			this._panelClient.Size = new System.Drawing.Size(975, 539);
			// 
			// _panelHeader
			// 
			this._panelHeader.Controls.Add(this._menuPriceChartType);
			this._panelHeader.Controls.Add(this._buttonArtistsPerYear);
			this._panelHeader.Controls.Add(this._menuPrice);
			this._panelHeader.Controls.Add(this._buttonManaCurve);
			this._panelHeader.Controls.Add(this._buttonDeckPrice);
			this._panelHeader.Padding = new System.Windows.Forms.Padding(1);
			this._panelHeader.Size = new System.Drawing.Size(882, 53);
			// 
			// _chart
			// 
			this._chart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			legend1.Name = "Legend1";
			this._chart.Legends.Add(legend1);
			this._chart.Location = new System.Drawing.Point(0, 72);
			this._chart.Margin = new System.Windows.Forms.Padding(0);
			this._chart.Name = "_chart";
			this._chart.Size = new System.Drawing.Size(975, 467);
			this._chart.TabIndex = 0;
			// 
			// _buttonManaCurve
			// 
			this._buttonManaCurve.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonManaCurve.FlatAppearance.BorderSize = 0;
			this._buttonManaCurve.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
			this._buttonManaCurve.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonManaCurve.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonManaCurve.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonManaCurve.Location = new System.Drawing.Point(1, 1);
			this._buttonManaCurve.Margin = new System.Windows.Forms.Padding(0);
			this._buttonManaCurve.Name = "_buttonManaCurve";
			this._buttonManaCurve.Size = new System.Drawing.Size(79, 51);
			this._buttonManaCurve.TabIndex = 3;
			this._buttonManaCurve.TabStop = false;
			this._buttonManaCurve.Text = "Deck mana curve";
			this._buttonManaCurve.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonManaCurve.UseVisualStyleBackColor = true;
			// 
			// _buttonDeckPrice
			// 
			this._buttonDeckPrice.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDeckPrice.FlatAppearance.BorderSize = 0;
			this._buttonDeckPrice.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
			this._buttonDeckPrice.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonDeckPrice.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonDeckPrice.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDeckPrice.Location = new System.Drawing.Point(80, 1);
			this._buttonDeckPrice.Margin = new System.Windows.Forms.Padding(0);
			this._buttonDeckPrice.Name = "_buttonDeckPrice";
			this._buttonDeckPrice.Size = new System.Drawing.Size(126, 51);
			this._buttonDeckPrice.TabIndex = 4;
			this._buttonDeckPrice.TabStop = false;
			this._buttonDeckPrice.Text = "Deck\r\nprice";
			this._buttonDeckPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonDeckPrice.UseVisualStyleBackColor = true;
			// 
			// _tabCols
			// 
			this._tabCols.AddButtonWidth = 24;
			this._tabCols.AllowAddingTabs = false;
			this._tabCols.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_16;
			this._tabCols.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_16;
			this._tabCols.ColorTabBorder = System.Drawing.Color.LightGray;
			this._tabCols.ColorUnselected = System.Drawing.Color.WhiteSmoke;
			this._tabCols.ColorUnselectedHovered = System.Drawing.Color.White;
			this._tabCols.Location = new System.Drawing.Point(166, 0);
			this._tabCols.Margin = new System.Windows.Forms.Padding(0);
			this._tabCols.Name = "_tabCols";
			this._tabCols.Size = new System.Drawing.Size(4, 24);
			this._tabCols.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabCols.TabIndex = 1;
			this._tabCols.TextPadding = 4;
			// 
			// _buttonApply
			// 
			this._buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonApply.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._buttonApply.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonApply.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			this._buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonApply.Location = new System.Drawing.Point(741, 0);
			this._buttonApply.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._buttonApply.Name = "_buttonApply";
			this._buttonApply.Size = new System.Drawing.Size(43, 24);
			this._buttonApply.TabIndex = 28;
			this._buttonApply.Text = "Build!";
			this._buttonApply.UseVisualStyleBackColor = false;
			// 
			// _menuFields
			// 
			this._menuFields.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuFields.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuFields.Location = new System.Drawing.Point(46, 2);
			this._menuFields.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuFields.MaxDropDownItems = 34;
			this._menuFields.Name = "_menuFields";
			this._menuFields.Size = new System.Drawing.Size(100, 21);
			this._menuFields.TabIndex = 40;
			this._menuFields.TabStop = false;
			// 
			// _buttonAddCol
			// 
			this._buttonAddCol.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._buttonAddCol.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonAddCol.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			this._buttonAddCol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAddCol.Location = new System.Drawing.Point(158, 0);
			this._buttonAddCol.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._buttonAddCol.Name = "_buttonAddCol";
			this._buttonAddCol.Size = new System.Drawing.Size(70, 24);
			this._buttonAddCol.TabIndex = 41;
			this._buttonAddCol.Text = "+ argument";
			this._buttonAddCol.UseVisualStyleBackColor = false;
			// 
			// _panelFields
			// 
			this._panelFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelFields.Controls.Add(this._progressBar);
			this._panelFields.Controls.Add(this._labelCols);
			this._panelFields.Controls.Add(this._tabCols);
			this._panelFields.Controls.Add(this._labelRows);
			this._panelFields.Controls.Add(this._tabRows);
			this._panelFields.Controls.Add(this._labelSum);
			this._panelFields.Controls.Add(this._tabSumm);
			this._panelFields.Controls.Add(this._labelSummarySort);
			this._panelFields.Controls.Add(this._tabSummSort);
			this._panelFields.Controls.Add(this._buttonArgumentTotals);
			this._panelFields.Controls.Add(this._buttonSeriesTotal);
			this._panelFields.Controls.Add(this._buttonExplainTotal);
			this._panelFields.Controls.Add(this._buttonApply);
			this._panelFields.Location = new System.Drawing.Point(0, 48);
			this._panelFields.Margin = new System.Windows.Forms.Padding(0);
			this._panelFields.Name = "_panelFields";
			this._panelFields.Size = new System.Drawing.Size(975, 24);
			this._panelFields.TabIndex = 42;
			// 
			// _progressBar
			// 
			this._progressBar.Location = new System.Drawing.Point(0, 0);
			this._progressBar.Margin = new System.Windows.Forms.Padding(0);
			this._progressBar.Maximum = 10;
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(100, 24);
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBar.TabIndex = 55;
			this._progressBar.Visible = false;
			// 
			// _labelCols
			// 
			this._labelCols.AutoSize = true;
			this._labelCols.Location = new System.Drawing.Point(103, 6);
			this._labelCols.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this._labelCols.Name = "_labelCols";
			this._labelCols.Size = new System.Drawing.Size(60, 13);
			this._labelCols.TabIndex = 2;
			this._labelCols.Text = "Arguments:";
			// 
			// _labelRows
			// 
			this._labelRows.AutoSize = true;
			this._labelRows.Location = new System.Drawing.Point(173, 6);
			this._labelRows.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this._labelRows.Name = "_labelRows";
			this._labelRows.Size = new System.Drawing.Size(39, 13);
			this._labelRows.TabIndex = 0;
			this._labelRows.Text = "Series:";
			// 
			// _tabRows
			// 
			this._tabRows.AddButtonWidth = 24;
			this._tabRows.AllowAddingTabs = false;
			this._tabRows.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_16;
			this._tabRows.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_16;
			this._tabRows.ColorTabBorder = System.Drawing.Color.LightGray;
			this._tabRows.ColorUnselected = System.Drawing.Color.WhiteSmoke;
			this._tabRows.ColorUnselectedHovered = System.Drawing.Color.White;
			this._tabRows.Location = new System.Drawing.Point(215, 0);
			this._tabRows.Margin = new System.Windows.Forms.Padding(0);
			this._tabRows.Name = "_tabRows";
			this._tabRows.Size = new System.Drawing.Size(4, 24);
			this._tabRows.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabRows.TabIndex = 3;
			this._tabRows.TextPadding = 4;
			// 
			// _labelSum
			// 
			this._labelSum.AutoSize = true;
			this._labelSum.Location = new System.Drawing.Point(222, 6);
			this._labelSum.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this._labelSum.Name = "_labelSum";
			this._labelSum.Size = new System.Drawing.Size(64, 13);
			this._labelSum.TabIndex = 4;
			this._labelSum.Text = "Aggregates:";
			// 
			// _tabSumm
			// 
			this._tabSumm.AddButtonWidth = 24;
			this._tabSumm.AllowAddingTabs = false;
			this._tabSumm.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_16;
			this._tabSumm.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_16;
			this._tabSumm.ColorTabBorder = System.Drawing.Color.LightGray;
			this._tabSumm.ColorUnselected = System.Drawing.Color.WhiteSmoke;
			this._tabSumm.ColorUnselectedHovered = System.Drawing.Color.White;
			this._tabSumm.Location = new System.Drawing.Point(289, 0);
			this._tabSumm.Margin = new System.Windows.Forms.Padding(0);
			this._tabSumm.Name = "_tabSumm";
			this._tabSumm.Size = new System.Drawing.Size(4, 24);
			this._tabSumm.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabSumm.TabIndex = 5;
			this._tabSumm.TextPadding = 4;
			// 
			// _labelSummarySort
			// 
			this._labelSummarySort.AutoSize = true;
			this._labelSummarySort.Location = new System.Drawing.Point(296, 6);
			this._labelSummarySort.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
			this._labelSummarySort.Name = "_labelSummarySort";
			this._labelSummarySort.Size = new System.Drawing.Size(99, 13);
			this._labelSummarySort.TabIndex = 6;
			this._labelSummarySort.Text = "Sort by aggregates:";
			// 
			// _tabSummSort
			// 
			this._tabSummSort.AddButtonWidth = 24;
			this._tabSummSort.AllowAddingTabs = false;
			this._tabSummSort.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_16;
			this._tabSummSort.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_16;
			this._tabSummSort.ColorTabBorder = System.Drawing.Color.LightGray;
			this._tabSummSort.ColorUnselected = System.Drawing.Color.WhiteSmoke;
			this._tabSummSort.ColorUnselectedHovered = System.Drawing.Color.White;
			this._tabSummSort.Location = new System.Drawing.Point(398, 0);
			this._tabSummSort.Margin = new System.Windows.Forms.Padding(0);
			this._tabSummSort.Name = "_tabSummSort";
			this._tabSummSort.Size = new System.Drawing.Size(4, 24);
			this._tabSummSort.SlopeSize = new System.Drawing.Size(4, 24);
			this._tabSummSort.TabIndex = 7;
			this._tabSummSort.TextPadding = 4;
			// 
			// _buttonArgumentTotals
			// 
			this._buttonArgumentTotals.AutoSize = true;
			this._buttonArgumentTotals.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._buttonArgumentTotals.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonArgumentTotals.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonArgumentTotals.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			this._buttonArgumentTotals.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonArgumentTotals.Location = new System.Drawing.Point(426, 4);
			this._buttonArgumentTotals.Margin = new System.Windows.Forms.Padding(24, 4, 0, 0);
			this._buttonArgumentTotals.Name = "_buttonArgumentTotals";
			this._buttonArgumentTotals.Size = new System.Drawing.Size(120, 17);
			this._buttonArgumentTotals.TabIndex = 56;
			this._buttonArgumentTotals.Text = "Show argument total";
			this._buttonArgumentTotals.UseVisualStyleBackColor = true;
			// 
			// _buttonSeriesTotal
			// 
			this._buttonSeriesTotal.AutoSize = true;
			this._buttonSeriesTotal.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._buttonSeriesTotal.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonSeriesTotal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonSeriesTotal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			this._buttonSeriesTotal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSeriesTotal.Location = new System.Drawing.Point(546, 4);
			this._buttonSeriesTotal.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonSeriesTotal.Name = "_buttonSeriesTotal";
			this._buttonSeriesTotal.Size = new System.Drawing.Size(103, 17);
			this._buttonSeriesTotal.TabIndex = 57;
			this._buttonSeriesTotal.Text = "Show series total";
			this._buttonSeriesTotal.UseVisualStyleBackColor = true;
			// 
			// _buttonExplainTotal
			// 
			this._buttonExplainTotal.AutoSize = true;
			this._buttonExplainTotal.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._buttonExplainTotal.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonExplainTotal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonExplainTotal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			this._buttonExplainTotal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonExplainTotal.Location = new System.Drawing.Point(649, 4);
			this._buttonExplainTotal.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonExplainTotal.Name = "_buttonExplainTotal";
			this._buttonExplainTotal.Size = new System.Drawing.Size(80, 17);
			this._buttonExplainTotal.TabIndex = 58;
			this._buttonExplainTotal.Text = "Explain total";
			this._buttonExplainTotal.UseVisualStyleBackColor = true;
			// 
			// _buttonAddRow
			// 
			this._buttonAddRow.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._buttonAddRow.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonAddRow.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			this._buttonAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAddRow.Location = new System.Drawing.Point(240, 0);
			this._buttonAddRow.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._buttonAddRow.Name = "_buttonAddRow";
			this._buttonAddRow.Size = new System.Drawing.Size(53, 24);
			this._buttonAddRow.TabIndex = 42;
			this._buttonAddRow.Text = "+ series";
			this._buttonAddRow.UseVisualStyleBackColor = false;
			// 
			// _buttonAddSum
			// 
			this._buttonAddSum.FlatAppearance.BorderColor = System.Drawing.Color.DarkGray;
			this._buttonAddSum.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonAddSum.FlatAppearance.MouseOverBackColor = System.Drawing.Color.WhiteSmoke;
			this._buttonAddSum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonAddSum.Location = new System.Drawing.Point(305, 0);
			this._buttonAddSum.Margin = new System.Windows.Forms.Padding(12, 0, 0, 0);
			this._buttonAddSum.Name = "_buttonAddSum";
			this._buttonAddSum.Size = new System.Drawing.Size(74, 24);
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
			this._panelMenu.Margin = new System.Windows.Forms.Padding(0, 12, 0, 12);
			this._panelMenu.Name = "_panelMenu";
			this._panelMenu.Size = new System.Drawing.Size(975, 24);
			this._panelMenu.TabIndex = 43;
			// 
			// _labelField
			// 
			this._labelField.AutoSize = true;
			this._labelField.Location = new System.Drawing.Point(6, 6);
			this._labelField.Margin = new System.Windows.Forms.Padding(6, 6, 3, 0);
			this._labelField.Name = "_labelField";
			this._labelField.Size = new System.Drawing.Size(37, 13);
			this._labelField.TabIndex = 50;
			this._labelField.Text = "Fields:";
			// 
			// _labelDataElement
			// 
			this._labelDataElement.AutoSize = true;
			this._labelDataElement.Location = new System.Drawing.Point(415, 6);
			this._labelDataElement.Margin = new System.Windows.Forms.Padding(36, 6, 3, 0);
			this._labelDataElement.Name = "_labelDataElement";
			this._labelDataElement.Size = new System.Drawing.Size(58, 13);
			this._labelDataElement.TabIndex = 46;
			this._labelDataElement.Text = "Data label:";
			// 
			// _menuLabelDataElement
			// 
			this._menuLabelDataElement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuLabelDataElement.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuLabelDataElement.Location = new System.Drawing.Point(476, 2);
			this._menuLabelDataElement.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuLabelDataElement.MaxDropDownItems = 34;
			this._menuLabelDataElement.Name = "_menuLabelDataElement";
			this._menuLabelDataElement.Size = new System.Drawing.Size(100, 21);
			this._menuLabelDataElement.TabIndex = 47;
			this._menuLabelDataElement.TabStop = false;
			// 
			// _labelDataSource
			// 
			this._labelDataSource.AutoSize = true;
			this._labelDataSource.Location = new System.Drawing.Point(588, 6);
			this._labelDataSource.Margin = new System.Windows.Forms.Padding(12, 6, 3, 0);
			this._labelDataSource.Name = "_labelDataSource";
			this._labelDataSource.Size = new System.Drawing.Size(44, 13);
			this._labelDataSource.TabIndex = 45;
			this._labelDataSource.Text = "Source:";
			// 
			// _menuDataSource
			// 
			this._menuDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuDataSource.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuDataSource.Location = new System.Drawing.Point(635, 2);
			this._menuDataSource.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuDataSource.MaxDropDownItems = 34;
			this._menuDataSource.Name = "_menuDataSource";
			this._menuDataSource.Size = new System.Drawing.Size(100, 21);
			this._menuDataSource.TabIndex = 44;
			this._menuDataSource.TabStop = false;
			// 
			// _labelChartType
			// 
			this._labelChartType.AutoSize = true;
			this._labelChartType.Location = new System.Drawing.Point(747, 6);
			this._labelChartType.Margin = new System.Windows.Forms.Padding(12, 6, 3, 0);
			this._labelChartType.Name = "_labelChartType";
			this._labelChartType.Size = new System.Drawing.Size(58, 13);
			this._labelChartType.TabIndex = 48;
			this._labelChartType.Text = "Chart type:";
			// 
			// _menuChartType
			// 
			this._menuChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuChartType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuChartType.Location = new System.Drawing.Point(808, 2);
			this._menuChartType.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._menuChartType.MaxDropDownItems = 34;
			this._menuChartType.Name = "_menuChartType";
			this._menuChartType.Size = new System.Drawing.Size(100, 21);
			this._menuChartType.TabIndex = 49;
			this._menuChartType.TabStop = false;
			// 
			// _panelTable
			// 
			this._panelTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelTable.ColumnCount = 1;
			this._panelTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelTable.Controls.Add(this._chart, 0, 2);
			this._panelTable.Controls.Add(this._panelMenu, 0, 0);
			this._panelTable.Controls.Add(this._panelFields, 0, 1);
			this._panelTable.Location = new System.Drawing.Point(0, 0);
			this._panelTable.Margin = new System.Windows.Forms.Padding(0);
			this._panelTable.Name = "_panelTable";
			this._panelTable.RowCount = 3;
			this._panelTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._panelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
			this._panelTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._panelTable.Size = new System.Drawing.Size(975, 539);
			this._panelTable.TabIndex = 44;
			// 
			// _menuPrice
			// 
			this._menuPrice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuPrice.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuPrice.Location = new System.Drawing.Point(83, 4);
			this._menuPrice.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this._menuPrice.MaxDropDownItems = 34;
			this._menuPrice.Name = "_menuPrice";
			this._menuPrice.Size = new System.Drawing.Size(73, 21);
			this._menuPrice.TabIndex = 51;
			this._menuPrice.TabStop = false;
			// 
			// _menuPriceChartType
			// 
			this._menuPriceChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._menuPriceChartType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this._menuPriceChartType.Location = new System.Drawing.Point(83, 28);
			this._menuPriceChartType.MaxDropDownItems = 34;
			this._menuPriceChartType.Name = "_menuPriceChartType";
			this._menuPriceChartType.Size = new System.Drawing.Size(73, 21);
			this._menuPriceChartType.TabIndex = 52;
			this._menuPriceChartType.TabStop = false;
			// 
			// _buttonArtistsPerYear
			// 
			this._buttonArtistsPerYear.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonArtistsPerYear.FlatAppearance.BorderSize = 0;
			this._buttonArtistsPerYear.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
			this._buttonArtistsPerYear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonArtistsPerYear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonArtistsPerYear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonArtistsPerYear.Location = new System.Drawing.Point(206, 1);
			this._buttonArtistsPerYear.Margin = new System.Windows.Forms.Padding(0);
			this._buttonArtistsPerYear.Name = "_buttonArtistsPerYear";
			this._buttonArtistsPerYear.Size = new System.Drawing.Size(126, 51);
			this._buttonArtistsPerYear.TabIndex = 54;
			this._buttonArtistsPerYear.TabStop = false;
			this._buttonArtistsPerYear.Text = "Customization example: Artists per year";
			this._buttonArtistsPerYear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this._buttonArtistsPerYear.UseVisualStyleBackColor = true;
			// 
			// FormChart
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(983, 600);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImageClose = global::Mtgdb.Gui.Properties.Resources.close;
			this.ImageMaximize = global::Mtgdb.Gui.Properties.Resources.maximize;
			this.ImageMinimize = global::Mtgdb.Gui.Properties.Resources.minimize;
			this.ImageNormalize = global::Mtgdb.Gui.Properties.Resources.normalize;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "FormChart";
			this.Text = "Deck statistics";
			this.TitleHeight = 57;
			this._panelClient.ResumeLayout(false);
			this._panelHeader.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._chart)).EndInit();
			this._panelFields.ResumeLayout(false);
			this._panelFields.PerformLayout();
			this._panelMenu.ResumeLayout(false);
			this._panelMenu.PerformLayout();
			this._panelTable.ResumeLayout(false);
			this._panelTable.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart _chart;
		private System.Windows.Forms.CheckBox _buttonManaCurve;
		private System.Windows.Forms.CheckBox _buttonDeckPrice;
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
		private System.Windows.Forms.CheckBox _buttonArtistsPerYear;
		private System.Windows.Forms.ProgressBar _progressBar;
		private System.Windows.Forms.CheckBox _buttonArgumentTotals;
		private System.Windows.Forms.CheckBox _buttonSeriesTotal;
		private System.Windows.Forms.CheckBox _buttonExplainTotal;
	}
}