namespace Mtgdb.Gui
{
	partial class FormRoot
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRoot));
			this._tabs = new Mtgdb.Controls.TabHeaderControl();
			this._buttonUndo = new Mtgdb.Controls.CustomCheckBox();
			this._buttonRedo = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDonate = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDownload = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLanguage = new Mtgdb.Controls.CustomCheckBox();
			this._buttonConfig = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHelp = new Mtgdb.Controls.CustomCheckBox();
			this._buttonClear = new Mtgdb.Controls.CustomCheckBox();
			this._buttonPrint = new Mtgdb.Controls.CustomCheckBox();
			this._buttonStat = new Mtgdb.Controls.CustomCheckBox();
			this._buttonSaveDeck = new Mtgdb.Controls.CustomCheckBox();
			this._buttonOpenDeck = new Mtgdb.Controls.CustomCheckBox();
			this._buttonTooltips = new Mtgdb.Controls.CustomCheckBox();
			this._menuOpen = new Mtgdb.Controls.CustomPanel();
			this._buttonMenuSaveCollection = new Mtgdb.Controls.CustomCheckBox();
			this._buttonVisitMtgo = new System.Windows.Forms.LinkLabel();
			this._labelDotP2 = new System.Windows.Forms.Label();
			this._labelMagarena2 = new System.Windows.Forms.Label();
			this._buttonVisitDotP2014 = new System.Windows.Forms.LinkLabel();
			this._buttonVisitCockatrice = new System.Windows.Forms.LinkLabel();
			this._buttonVisitMagarena = new System.Windows.Forms.LinkLabel();
			this._buttonVisitXMage = new System.Windows.Forms.LinkLabel();
			this._buttonVisitForge = new System.Windows.Forms.LinkLabel();
			this._labelFormats = new System.Windows.Forms.Label();
			this._buttonMenuOpenCollection = new Mtgdb.Controls.CustomCheckBox();
			this._buttonMenuSaveDeck = new Mtgdb.Controls.CustomCheckBox();
			this._buttonMenuOpenDeck = new Mtgdb.Controls.CustomCheckBox();
			this._menuLanguage = new Mtgdb.Controls.CustomPanel();
			this._buttonPT = new Mtgdb.Controls.CustomCheckBox();
			this._buttonTW = new Mtgdb.Controls.CustomCheckBox();
			this._buttonRU = new Mtgdb.Controls.CustomCheckBox();
			this._buttonIT = new Mtgdb.Controls.CustomCheckBox();
			this._buttonKR = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDE = new Mtgdb.Controls.CustomCheckBox();
			this._buttonFR = new Mtgdb.Controls.CustomCheckBox();
			this._buttonJP = new Mtgdb.Controls.CustomCheckBox();
			this._buttonEN = new Mtgdb.Controls.CustomCheckBox();
			this._buttonES = new Mtgdb.Controls.CustomCheckBox();
			this._buttonCN = new Mtgdb.Controls.CustomCheckBox();
			this._menuDonate = new Mtgdb.Controls.CustomPanel();
			this._labelDonate = new System.Windows.Forms.Label();
			this._buttonDonatePayPal = new Mtgdb.Controls.CustomCheckBox();
			this._buttonDonateYandexMoney = new Mtgdb.Controls.CustomCheckBox();
			this._panelAva = new System.Windows.Forms.Panel();
			this._menuConfig = new Mtgdb.Controls.CustomPanel();
			this._buttonDisplaySettings = new Mtgdb.Controls.CustomCheckBox();
			this._buttonGeneralSettings = new Mtgdb.Controls.CustomCheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this._panelHeader.SuspendLayout();
			this._menuOpen.SuspendLayout();
			this._menuLanguage.SuspendLayout();
			this._menuDonate.SuspendLayout();
			this._menuConfig.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelClient
			// 
			this._panelClient.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._panelClient.Location = new System.Drawing.Point(4, 33);
			this._panelClient.Size = new System.Drawing.Size(992, 763);
			// 
			// _panelHeader
			// 
			this._panelHeader.Controls.Add(this._buttonTooltips);
			this._panelHeader.Controls.Add(this._buttonOpenDeck);
			this._panelHeader.Controls.Add(this._buttonSaveDeck);
			this._panelHeader.Controls.Add(this._buttonStat);
			this._panelHeader.Controls.Add(this._buttonPrint);
			this._panelHeader.Controls.Add(this._buttonClear);
			this._panelHeader.Controls.Add(this._buttonHelp);
			this._panelHeader.Controls.Add(this._buttonConfig);
			this._panelHeader.Controls.Add(this._buttonLanguage);
			this._panelHeader.Controls.Add(this._buttonDownload);
			this._panelHeader.Controls.Add(this._buttonDonate);
			this._panelHeader.Controls.Add(this._buttonRedo);
			this._panelHeader.Controls.Add(this._buttonUndo);
			this._panelHeader.Controls.Add(this._tabs);
			this._panelHeader.Size = new System.Drawing.Size(899, 29);
			// 
			// _tabs
			// 
			this._tabs.AddButtonSlopeSize = new System.Drawing.Size(9, 17);
			this._tabs.AddButtonWidth = 24;
			this._tabs.AddIcon = global::Mtgdb.Gui.Properties.Resources.add_tab_16;
			this._tabs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._tabs.CloseIcon = global::Mtgdb.Gui.Properties.Resources.close_tab_16;
			this._tabs.CloseIconHovered = global::Mtgdb.Gui.Properties.Resources.close_tab_hovered_16;
			this._tabs.ColorSelected = System.Drawing.SystemColors.Control;
			this._tabs.ColorSelectedHovered = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
			this._tabs.ColorTabBorder = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(166)))), ((int)(((byte)(166)))));
			this._tabs.ColorUnselected = System.Drawing.Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(196)))), ((int)(((byte)(196)))));
			this._tabs.ColorUnselectedHovered = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
			this._tabs.DrawBottomBorder = true;
			this._tabs.Location = new System.Drawing.Point(61, 0);
			this._tabs.Margin = new System.Windows.Forms.Padding(0);
			this._tabs.Name = "_tabs";
			this._tabs.Size = new System.Drawing.Size(42, 29);
			this._tabs.SlopeSize = new System.Drawing.Size(15, 29);
			this._tabs.TabIndex = 4;
			this._tabs.TabStop = false;
			// 
			// _buttonUndo
			// 
			this._buttonUndo.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonUndo.FlatAppearance.BorderSize = 0;
			this._buttonUndo.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonUndo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonUndo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonUndo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonUndo.Image = global::Mtgdb.Gui.Properties.Resources.undo_16x16;
			this._buttonUndo.Location = new System.Drawing.Point(1, 4);
			this._buttonUndo.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonUndo.Name = "_buttonUndo";
			this._buttonUndo.Size = new System.Drawing.Size(30, 24);
			this._buttonUndo.TabIndex = 5;
			this._buttonUndo.TabStop = false;
			this._buttonUndo.UseVisualStyleBackColor = true;
			// 
			// _buttonRedo
			// 
			this._buttonRedo.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonRedo.FlatAppearance.BorderSize = 0;
			this._buttonRedo.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonRedo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonRedo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonRedo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonRedo.Image = global::Mtgdb.Gui.Properties.Resources.redo_16x16;
			this._buttonRedo.Location = new System.Drawing.Point(31, 4);
			this._buttonRedo.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonRedo.Name = "_buttonRedo";
			this._buttonRedo.Size = new System.Drawing.Size(30, 24);
			this._buttonRedo.TabIndex = 6;
			this._buttonRedo.TabStop = false;
			this._buttonRedo.UseVisualStyleBackColor = true;
			// 
			// _buttonDonate
			// 
			this._buttonDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDonate.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDonate.FlatAppearance.BorderSize = 0;
			this._buttonDonate.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonDonate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonDonate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonDonate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDonate.Location = new System.Drawing.Point(809, 4);
			this._buttonDonate.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonDonate.Name = "_buttonDonate";
			this._buttonDonate.Size = new System.Drawing.Size(49, 24);
			this._buttonDonate.TabIndex = 7;
			this._buttonDonate.TabStop = false;
			this._buttonDonate.Text = "donate";
			this._buttonDonate.UseVisualStyleBackColor = true;
			// 
			// _buttonDownload
			// 
			this._buttonDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDownload.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDownload.FlatAppearance.BorderSize = 0;
			this._buttonDownload.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonDownload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonDownload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDownload.Image = global::Mtgdb.Gui.Properties.Resources.update;
			this._buttonDownload.Location = new System.Drawing.Point(749, 4);
			this._buttonDownload.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonDownload.Name = "_buttonDownload";
			this._buttonDownload.Size = new System.Drawing.Size(30, 24);
			this._buttonDownload.TabIndex = 8;
			this._buttonDownload.TabStop = false;
			this._buttonDownload.UseVisualStyleBackColor = true;
			// 
			// _buttonLanguage
			// 
			this._buttonLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonLanguage.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonLanguage.BackColor = System.Drawing.Color.White;
			this._buttonLanguage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonLanguage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(170)))));
			this._buttonLanguage.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonLanguage.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this._buttonLanguage.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
			this._buttonLanguage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLanguage.Image = global::Mtgdb.Gui.Properties.Resources.gb;
			this._buttonLanguage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonLanguage.Location = new System.Drawing.Point(689, 4);
			this._buttonLanguage.Margin = new System.Windows.Forms.Padding(0, 4, 0, 1);
			this._buttonLanguage.Name = "_buttonLanguage";
			this._buttonLanguage.Size = new System.Drawing.Size(60, 23);
			this._buttonLanguage.TabIndex = 9;
			this._buttonLanguage.TabStop = false;
			this._buttonLanguage.Text = "EN";
			this._buttonLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonLanguage.UseVisualStyleBackColor = false;
			// 
			// _buttonConfig
			// 
			this._buttonConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonConfig.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonConfig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonConfig.FlatAppearance.BorderSize = 0;
			this._buttonConfig.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonConfig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonConfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonConfig.Image = global::Mtgdb.Gui.Properties.Resources.properties_16x16;
			this._buttonConfig.Location = new System.Drawing.Point(659, 3);
			this._buttonConfig.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonConfig.Name = "_buttonConfig";
			this._buttonConfig.Size = new System.Drawing.Size(30, 24);
			this._buttonConfig.TabIndex = 10;
			this._buttonConfig.TabStop = false;
			this._buttonConfig.UseVisualStyleBackColor = true;
			// 
			// _buttonHelp
			// 
			this._buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonHelp.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHelp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonHelp.FlatAppearance.BorderSize = 0;
			this._buttonHelp.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonHelp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonHelp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHelp.Image = global::Mtgdb.Gui.Properties.Resources.index_16x16;
			this._buttonHelp.Location = new System.Drawing.Point(599, 3);
			this._buttonHelp.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonHelp.Name = "_buttonHelp";
			this._buttonHelp.Size = new System.Drawing.Size(30, 24);
			this._buttonHelp.TabIndex = 12;
			this._buttonHelp.TabStop = false;
			this._buttonHelp.UseVisualStyleBackColor = true;
			// 
			// _buttonClear
			// 
			this._buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonClear.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonClear.FlatAppearance.BorderSize = 0;
			this._buttonClear.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonClear.Image = global::Mtgdb.Gui.Properties.Resources.trash_16x16;
			this._buttonClear.Location = new System.Drawing.Point(545, 3);
			this._buttonClear.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonClear.Name = "_buttonClear";
			this._buttonClear.Size = new System.Drawing.Size(30, 24);
			this._buttonClear.TabIndex = 13;
			this._buttonClear.TabStop = false;
			this._buttonClear.UseVisualStyleBackColor = true;
			// 
			// _buttonPrint
			// 
			this._buttonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonPrint.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonPrint.FlatAppearance.BorderSize = 0;
			this._buttonPrint.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonPrint.Image = global::Mtgdb.Gui.Properties.Resources.print_16x16;
			this._buttonPrint.Location = new System.Drawing.Point(515, 3);
			this._buttonPrint.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonPrint.Name = "_buttonPrint";
			this._buttonPrint.Size = new System.Drawing.Size(30, 24);
			this._buttonPrint.TabIndex = 14;
			this._buttonPrint.TabStop = false;
			this._buttonPrint.UseVisualStyleBackColor = true;
			// 
			// _buttonStat
			// 
			this._buttonStat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonStat.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonStat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonStat.FlatAppearance.BorderSize = 0;
			this._buttonStat.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonStat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonStat.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonStat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonStat.Image = global::Mtgdb.Gui.Properties.Resources.chart2_16x16;
			this._buttonStat.Location = new System.Drawing.Point(485, 3);
			this._buttonStat.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonStat.Name = "_buttonStat";
			this._buttonStat.Size = new System.Drawing.Size(30, 24);
			this._buttonStat.TabIndex = 15;
			this._buttonStat.TabStop = false;
			this._buttonStat.UseVisualStyleBackColor = true;
			// 
			// _buttonSaveDeck
			// 
			this._buttonSaveDeck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonSaveDeck.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSaveDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonSaveDeck.FlatAppearance.BorderSize = 0;
			this._buttonSaveDeck.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonSaveDeck.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonSaveDeck.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonSaveDeck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSaveDeck.Image = global::Mtgdb.Gui.Properties.Resources.save_16x16;
			this._buttonSaveDeck.Location = new System.Drawing.Point(455, 3);
			this._buttonSaveDeck.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonSaveDeck.Name = "_buttonSaveDeck";
			this._buttonSaveDeck.Size = new System.Drawing.Size(30, 24);
			this._buttonSaveDeck.TabIndex = 16;
			this._buttonSaveDeck.TabStop = false;
			this._buttonSaveDeck.UseVisualStyleBackColor = true;
			// 
			// _buttonOpenDeck
			// 
			this._buttonOpenDeck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonOpenDeck.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonOpenDeck.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonOpenDeck.FlatAppearance.BorderSize = 0;
			this._buttonOpenDeck.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonOpenDeck.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonOpenDeck.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonOpenDeck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonOpenDeck.Image = global::Mtgdb.Gui.Properties.Resources.open2_16x16;
			this._buttonOpenDeck.Location = new System.Drawing.Point(425, 3);
			this._buttonOpenDeck.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonOpenDeck.Name = "_buttonOpenDeck";
			this._buttonOpenDeck.Size = new System.Drawing.Size(30, 24);
			this._buttonOpenDeck.TabIndex = 17;
			this._buttonOpenDeck.TabStop = false;
			this._buttonOpenDeck.UseVisualStyleBackColor = true;
			// 
			// _buttonTooltips
			// 
			this._buttonTooltips.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonTooltips.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonTooltips.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonTooltips.Checked = true;
			this._buttonTooltips.CheckState = System.Windows.Forms.CheckState.Checked;
			this._buttonTooltips.FlatAppearance.BorderSize = 0;
			this._buttonTooltips.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(240)))));
			this._buttonTooltips.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonTooltips.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonTooltips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonTooltips.Image = global::Mtgdb.Gui.Properties.Resources.showhidecomment_16x16;
			this._buttonTooltips.Location = new System.Drawing.Point(629, 3);
			this._buttonTooltips.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonTooltips.Name = "_buttonTooltips";
			this._buttonTooltips.Size = new System.Drawing.Size(30, 24);
			this._buttonTooltips.TabIndex = 18;
			this._buttonTooltips.TabStop = false;
			this._buttonTooltips.UseVisualStyleBackColor = true;
			// 
			// _menuOpen
			// 
			this._menuOpen.BackColor = System.Drawing.Color.White;
			this._menuOpen.Controls.Add(this.label2);
			this._menuOpen.Controls.Add(this._buttonMenuSaveCollection);
			this._menuOpen.Controls.Add(this._buttonVisitMtgo);
			this._menuOpen.Controls.Add(this._labelDotP2);
			this._menuOpen.Controls.Add(this._labelMagarena2);
			this._menuOpen.Controls.Add(this._buttonVisitDotP2014);
			this._menuOpen.Controls.Add(this._buttonVisitCockatrice);
			this._menuOpen.Controls.Add(this._buttonVisitMagarena);
			this._menuOpen.Controls.Add(this._buttonVisitXMage);
			this._menuOpen.Controls.Add(this._buttonVisitForge);
			this._menuOpen.Controls.Add(this._labelFormats);
			this._menuOpen.Controls.Add(this._buttonMenuOpenCollection);
			this._menuOpen.Controls.Add(this._buttonMenuSaveDeck);
			this._menuOpen.Controls.Add(this._buttonMenuOpenDeck);
			this._menuOpen.Location = new System.Drawing.Point(29, 59);
			this._menuOpen.Name = "_menuOpen";
			this._menuOpen.Size = new System.Drawing.Size(299, 353);
			this._menuOpen.TabIndex = 0;
			this._menuOpen.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonMenuSaveCollection
			// 
			this._buttonMenuSaveCollection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonMenuSaveCollection.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonMenuSaveCollection.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonMenuSaveCollection.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonMenuSaveCollection.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuSaveCollection.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuSaveCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonMenuSaveCollection.Image = global::Mtgdb.Gui.Properties.Resources.Box_32;
			this._buttonMenuSaveCollection.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonMenuSaveCollection.Location = new System.Drawing.Point(3, 42);
			this._buttonMenuSaveCollection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
			this._buttonMenuSaveCollection.Name = "_buttonMenuSaveCollection";
			this._buttonMenuSaveCollection.Size = new System.Drawing.Size(293, 37);
			this._buttonMenuSaveCollection.TabIndex = 34;
			this._buttonMenuSaveCollection.TabStop = false;
			this._buttonMenuSaveCollection.Text = "            Save collection to file: Ctrl+Alt+S";
			this._buttonMenuSaveCollection.UseVisualStyleBackColor = true;
			this._buttonMenuSaveCollection.Visible = false;
			// 
			// _buttonVisitMtgo
			// 
			this._buttonVisitMtgo.ActiveLinkColor = System.Drawing.Color.Blue;
			this._buttonVisitMtgo.BackColor = System.Drawing.Color.Transparent;
			this._buttonVisitMtgo.Image = global::Mtgdb.Gui.Properties.Resources.mtgo_32;
			this._buttonVisitMtgo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this._buttonVisitMtgo.LinkArea = new System.Windows.Forms.LinkArea(0, 26);
			this._buttonVisitMtgo.LinkColor = System.Drawing.Color.Blue;
			this._buttonVisitMtgo.Location = new System.Drawing.Point(0, 175);
			this._buttonVisitMtgo.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._buttonVisitMtgo.Name = "_buttonVisitMtgo";
			this._buttonVisitMtgo.Size = new System.Drawing.Size(90, 54);
			this._buttonVisitMtgo.TabIndex = 32;
			this._buttonVisitMtgo.TabStop = true;
			this._buttonVisitMtgo.Text = "Magic The Gathering Online";
			this._buttonVisitMtgo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// _labelDotP2
			// 
			this._labelDotP2.AutoSize = true;
			this._labelDotP2.BackColor = System.Drawing.Color.Transparent;
			this._labelDotP2.Location = new System.Drawing.Point(76, 241);
			this._labelDotP2.Margin = new System.Windows.Forms.Padding(0);
			this._labelDotP2.Name = "_labelDotP2";
			this._labelDotP2.Size = new System.Drawing.Size(205, 13);
			this._labelDotP2.TabIndex = 12;
			this._labelDotP2.Text = "* Modified version supports Forge format";
			// 
			// _labelMagarena2
			// 
			this._labelMagarena2.AutoSize = true;
			this._labelMagarena2.BackColor = System.Drawing.Color.Transparent;
			this._labelMagarena2.Location = new System.Drawing.Point(76, 304);
			this._labelMagarena2.Margin = new System.Windows.Forms.Padding(0);
			this._labelMagarena2.Name = "_labelMagarena2";
			this._labelMagarena2.Size = new System.Drawing.Size(145, 13);
			this._labelMagarena2.TabIndex = 9;
			this._labelMagarena2.Text = "* Supports Magarena format";
			// 
			// _buttonVisitDotP2014
			// 
			this._buttonVisitDotP2014.ActiveLinkColor = System.Drawing.Color.Blue;
			this._buttonVisitDotP2014.BackColor = System.Drawing.Color.Transparent;
			this._buttonVisitDotP2014.Image = global::Mtgdb.Gui.Properties.Resources.DotP2014_32;
			this._buttonVisitDotP2014.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this._buttonVisitDotP2014.LinkArea = new System.Windows.Forms.LinkArea(0, 30);
			this._buttonVisitDotP2014.LinkColor = System.Drawing.Color.Blue;
			this._buttonVisitDotP2014.Location = new System.Drawing.Point(0, 238);
			this._buttonVisitDotP2014.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._buttonVisitDotP2014.Name = "_buttonVisitDotP2014";
			this._buttonVisitDotP2014.Size = new System.Drawing.Size(90, 54);
			this._buttonVisitDotP2014.TabIndex = 4;
			this._buttonVisitDotP2014.TabStop = true;
			this._buttonVisitDotP2014.Text = "Riiak\'s DotP 2014 Deck Builder";
			this._buttonVisitDotP2014.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// _buttonVisitCockatrice
			// 
			this._buttonVisitCockatrice.ActiveLinkColor = System.Drawing.Color.Blue;
			this._buttonVisitCockatrice.BackColor = System.Drawing.Color.Transparent;
			this._buttonVisitCockatrice.Image = global::Mtgdb.Gui.Properties.Resources.Cockatrice_32;
			this._buttonVisitCockatrice.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this._buttonVisitCockatrice.LinkArea = new System.Windows.Forms.LinkArea(0, 10);
			this._buttonVisitCockatrice.LinkColor = System.Drawing.Color.Blue;
			this._buttonVisitCockatrice.Location = new System.Drawing.Point(0, 301);
			this._buttonVisitCockatrice.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._buttonVisitCockatrice.Name = "_buttonVisitCockatrice";
			this._buttonVisitCockatrice.Size = new System.Drawing.Size(90, 46);
			this._buttonVisitCockatrice.TabIndex = 3;
			this._buttonVisitCockatrice.TabStop = true;
			this._buttonVisitCockatrice.Text = "Cockatrice";
			this._buttonVisitCockatrice.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// _buttonVisitMagarena
			// 
			this._buttonVisitMagarena.ActiveLinkColor = System.Drawing.Color.Blue;
			this._buttonVisitMagarena.BackColor = System.Drawing.Color.Transparent;
			this._buttonVisitMagarena.Image = global::Mtgdb.Gui.Properties.Resources.Magarena_32;
			this._buttonVisitMagarena.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this._buttonVisitMagarena.LinkArea = new System.Windows.Forms.LinkArea(0, 8);
			this._buttonVisitMagarena.LinkColor = System.Drawing.Color.Blue;
			this._buttonVisitMagarena.Location = new System.Drawing.Point(160, 118);
			this._buttonVisitMagarena.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._buttonVisitMagarena.Name = "_buttonVisitMagarena";
			this._buttonVisitMagarena.Size = new System.Drawing.Size(90, 48);
			this._buttonVisitMagarena.TabIndex = 2;
			this._buttonVisitMagarena.TabStop = true;
			this._buttonVisitMagarena.Text = "Magarena";
			this._buttonVisitMagarena.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// _buttonVisitXMage
			// 
			this._buttonVisitXMage.ActiveLinkColor = System.Drawing.Color.Blue;
			this._buttonVisitXMage.BackColor = System.Drawing.Color.Transparent;
			this._buttonVisitXMage.Image = global::Mtgdb.Gui.Properties.Resources.xmage_32;
			this._buttonVisitXMage.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this._buttonVisitXMage.LinkArea = new System.Windows.Forms.LinkArea(0, 5);
			this._buttonVisitXMage.LinkColor = System.Drawing.Color.Blue;
			this._buttonVisitXMage.Location = new System.Drawing.Point(80, 118);
			this._buttonVisitXMage.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._buttonVisitXMage.Name = "_buttonVisitXMage";
			this._buttonVisitXMage.Size = new System.Drawing.Size(90, 48);
			this._buttonVisitXMage.TabIndex = 1;
			this._buttonVisitXMage.TabStop = true;
			this._buttonVisitXMage.Text = "XMage";
			this._buttonVisitXMage.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// _buttonVisitForge
			// 
			this._buttonVisitForge.ActiveLinkColor = System.Drawing.Color.Blue;
			this._buttonVisitForge.BackColor = System.Drawing.Color.Transparent;
			this._buttonVisitForge.Image = global::Mtgdb.Gui.Properties.Resources.forge_32;
			this._buttonVisitForge.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this._buttonVisitForge.LinkArea = new System.Windows.Forms.LinkArea(0, 5);
			this._buttonVisitForge.LinkColor = System.Drawing.Color.Blue;
			this._buttonVisitForge.Location = new System.Drawing.Point(0, 118);
			this._buttonVisitForge.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
			this._buttonVisitForge.Name = "_buttonVisitForge";
			this._buttonVisitForge.Size = new System.Drawing.Size(90, 48);
			this._buttonVisitForge.TabIndex = 0;
			this._buttonVisitForge.TabStop = true;
			this._buttonVisitForge.Text = "Forge";
			this._buttonVisitForge.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// _labelFormats
			// 
			this._labelFormats.AutoSize = true;
			this._labelFormats.BackColor = System.Drawing.Color.Transparent;
			this._labelFormats.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelFormats.Location = new System.Drawing.Point(19, 93);
			this._labelFormats.Margin = new System.Windows.Forms.Padding(3);
			this._labelFormats.Name = "_labelFormats";
			this._labelFormats.Size = new System.Drawing.Size(114, 14);
			this._labelFormats.TabIndex = 6;
			this._labelFormats.Text = "Supported formats:";
			// 
			// _buttonMenuOpenCollection
			// 
			this._buttonMenuOpenCollection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonMenuOpenCollection.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonMenuOpenCollection.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonMenuOpenCollection.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonMenuOpenCollection.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuOpenCollection.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuOpenCollection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonMenuOpenCollection.Image = global::Mtgdb.Gui.Properties.Resources.Box_32;
			this._buttonMenuOpenCollection.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonMenuOpenCollection.Location = new System.Drawing.Point(3, 42);
			this._buttonMenuOpenCollection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
			this._buttonMenuOpenCollection.Name = "_buttonMenuOpenCollection";
			this._buttonMenuOpenCollection.Size = new System.Drawing.Size(293, 37);
			this._buttonMenuOpenCollection.TabIndex = 31;
			this._buttonMenuOpenCollection.TabStop = false;
			this._buttonMenuOpenCollection.Text = "            Load collection from file: Ctrl+Alt+O";
			this._buttonMenuOpenCollection.UseVisualStyleBackColor = true;
			this._buttonMenuOpenCollection.Visible = false;
			// 
			// _buttonMenuSaveDeck
			// 
			this._buttonMenuSaveDeck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonMenuSaveDeck.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonMenuSaveDeck.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonMenuSaveDeck.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonMenuSaveDeck.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuSaveDeck.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuSaveDeck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonMenuSaveDeck.Image = global::Mtgdb.Gui.Properties.Resources.draw_a_card_32;
			this._buttonMenuSaveDeck.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonMenuSaveDeck.Location = new System.Drawing.Point(3, 3);
			this._buttonMenuSaveDeck.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._buttonMenuSaveDeck.Name = "_buttonMenuSaveDeck";
			this._buttonMenuSaveDeck.Size = new System.Drawing.Size(293, 37);
			this._buttonMenuSaveDeck.TabIndex = 33;
			this._buttonMenuSaveDeck.TabStop = false;
			this._buttonMenuSaveDeck.Text = "            Save deck to file: Ctrl+S";
			this._buttonMenuSaveDeck.UseVisualStyleBackColor = true;
			this._buttonMenuSaveDeck.Visible = false;
			// 
			// _buttonMenuOpenDeck
			// 
			this._buttonMenuOpenDeck.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonMenuOpenDeck.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonMenuOpenDeck.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonMenuOpenDeck.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonMenuOpenDeck.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuOpenDeck.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonMenuOpenDeck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonMenuOpenDeck.Image = global::Mtgdb.Gui.Properties.Resources.draw_a_card_32;
			this._buttonMenuOpenDeck.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonMenuOpenDeck.Location = new System.Drawing.Point(3, 3);
			this._buttonMenuOpenDeck.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._buttonMenuOpenDeck.Name = "_buttonMenuOpenDeck";
			this._buttonMenuOpenDeck.Size = new System.Drawing.Size(293, 37);
			this._buttonMenuOpenDeck.TabIndex = 30;
			this._buttonMenuOpenDeck.TabStop = false;
			this._buttonMenuOpenDeck.Text = "            Load deck from file: Ctrl+O";
			this._buttonMenuOpenDeck.UseVisualStyleBackColor = true;
			this._buttonMenuOpenDeck.Visible = false;
			// 
			// _menuLanguage
			// 
			this._menuLanguage.BackColor = System.Drawing.Color.White;
			this._menuLanguage.Controls.Add(this._buttonPT);
			this._menuLanguage.Controls.Add(this._buttonTW);
			this._menuLanguage.Controls.Add(this._buttonRU);
			this._menuLanguage.Controls.Add(this._buttonIT);
			this._menuLanguage.Controls.Add(this._buttonKR);
			this._menuLanguage.Controls.Add(this._buttonDE);
			this._menuLanguage.Controls.Add(this._buttonFR);
			this._menuLanguage.Controls.Add(this._buttonJP);
			this._menuLanguage.Controls.Add(this._buttonEN);
			this._menuLanguage.Controls.Add(this._buttonES);
			this._menuLanguage.Controls.Add(this._buttonCN);
			this._menuLanguage.Location = new System.Drawing.Point(854, 33);
			this._menuLanguage.Name = "_menuLanguage";
			this._menuLanguage.Size = new System.Drawing.Size(146, 98);
			this._menuLanguage.TabIndex = 13;
			this._menuLanguage.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonPT
			// 
			this._buttonPT.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonPT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonPT.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonPT.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonPT.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonPT.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonPT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonPT.Image = global::Mtgdb.Gui.Properties.Resources.pt;
			this._buttonPT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonPT.Location = new System.Drawing.Point(49, 73);
			this._buttonPT.Margin = new System.Windows.Forms.Padding(0);
			this._buttonPT.Name = "_buttonPT";
			this._buttonPT.Size = new System.Drawing.Size(48, 24);
			this._buttonPT.TabIndex = 28;
			this._buttonPT.TabStop = false;
			this._buttonPT.Text = "PT";
			this._buttonPT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonPT.UseVisualStyleBackColor = true;
			// 
			// _buttonTW
			// 
			this._buttonTW.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonTW.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonTW.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonTW.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonTW.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonTW.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonTW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonTW.Image = global::Mtgdb.Gui.Properties.Resources.tw;
			this._buttonTW.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonTW.Location = new System.Drawing.Point(1, 73);
			this._buttonTW.Margin = new System.Windows.Forms.Padding(0);
			this._buttonTW.Name = "_buttonTW";
			this._buttonTW.Size = new System.Drawing.Size(48, 24);
			this._buttonTW.TabIndex = 27;
			this._buttonTW.TabStop = false;
			this._buttonTW.Text = "TW";
			this._buttonTW.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonTW.UseVisualStyleBackColor = true;
			// 
			// _buttonRU
			// 
			this._buttonRU.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonRU.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonRU.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonRU.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonRU.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonRU.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonRU.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonRU.Image = global::Mtgdb.Gui.Properties.Resources.ru;
			this._buttonRU.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonRU.Location = new System.Drawing.Point(97, 49);
			this._buttonRU.Margin = new System.Windows.Forms.Padding(0);
			this._buttonRU.Name = "_buttonRU";
			this._buttonRU.Size = new System.Drawing.Size(48, 24);
			this._buttonRU.TabIndex = 26;
			this._buttonRU.TabStop = false;
			this._buttonRU.Text = "RU";
			this._buttonRU.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonRU.UseVisualStyleBackColor = true;
			// 
			// _buttonIT
			// 
			this._buttonIT.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonIT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonIT.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonIT.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonIT.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonIT.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonIT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonIT.Image = global::Mtgdb.Gui.Properties.Resources.it;
			this._buttonIT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonIT.Location = new System.Drawing.Point(49, 49);
			this._buttonIT.Margin = new System.Windows.Forms.Padding(0);
			this._buttonIT.Name = "_buttonIT";
			this._buttonIT.Size = new System.Drawing.Size(48, 24);
			this._buttonIT.TabIndex = 25;
			this._buttonIT.TabStop = false;
			this._buttonIT.Text = "IT";
			this._buttonIT.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonIT.UseVisualStyleBackColor = true;
			// 
			// _buttonKR
			// 
			this._buttonKR.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonKR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonKR.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonKR.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonKR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonKR.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonKR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonKR.Image = global::Mtgdb.Gui.Properties.Resources.kr;
			this._buttonKR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonKR.Location = new System.Drawing.Point(1, 49);
			this._buttonKR.Margin = new System.Windows.Forms.Padding(0);
			this._buttonKR.Name = "_buttonKR";
			this._buttonKR.Size = new System.Drawing.Size(48, 24);
			this._buttonKR.TabIndex = 24;
			this._buttonKR.TabStop = false;
			this._buttonKR.Text = "KR";
			this._buttonKR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonKR.UseVisualStyleBackColor = true;
			// 
			// _buttonDE
			// 
			this._buttonDE.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDE.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonDE.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonDE.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonDE.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDE.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDE.Image = global::Mtgdb.Gui.Properties.Resources.de;
			this._buttonDE.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonDE.Location = new System.Drawing.Point(97, 25);
			this._buttonDE.Margin = new System.Windows.Forms.Padding(0);
			this._buttonDE.Name = "_buttonDE";
			this._buttonDE.Size = new System.Drawing.Size(48, 24);
			this._buttonDE.TabIndex = 23;
			this._buttonDE.TabStop = false;
			this._buttonDE.Text = "DE";
			this._buttonDE.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonDE.UseVisualStyleBackColor = true;
			// 
			// _buttonFR
			// 
			this._buttonFR.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonFR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonFR.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonFR.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonFR.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonFR.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonFR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonFR.Image = global::Mtgdb.Gui.Properties.Resources.fr;
			this._buttonFR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonFR.Location = new System.Drawing.Point(49, 25);
			this._buttonFR.Margin = new System.Windows.Forms.Padding(0);
			this._buttonFR.Name = "_buttonFR";
			this._buttonFR.Size = new System.Drawing.Size(48, 24);
			this._buttonFR.TabIndex = 22;
			this._buttonFR.TabStop = false;
			this._buttonFR.Text = "FR";
			this._buttonFR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonFR.UseVisualStyleBackColor = true;
			// 
			// _buttonJP
			// 
			this._buttonJP.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonJP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonJP.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonJP.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonJP.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonJP.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonJP.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonJP.Image = global::Mtgdb.Gui.Properties.Resources.jp;
			this._buttonJP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonJP.Location = new System.Drawing.Point(1, 25);
			this._buttonJP.Margin = new System.Windows.Forms.Padding(0);
			this._buttonJP.Name = "_buttonJP";
			this._buttonJP.Size = new System.Drawing.Size(48, 24);
			this._buttonJP.TabIndex = 21;
			this._buttonJP.TabStop = false;
			this._buttonJP.Text = "JP";
			this._buttonJP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonJP.UseVisualStyleBackColor = true;
			// 
			// _buttonEN
			// 
			this._buttonEN.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonEN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonEN.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonEN.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonEN.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonEN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonEN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonEN.Image = global::Mtgdb.Gui.Properties.Resources.gb;
			this._buttonEN.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonEN.Location = new System.Drawing.Point(97, 1);
			this._buttonEN.Margin = new System.Windows.Forms.Padding(0);
			this._buttonEN.Name = "_buttonEN";
			this._buttonEN.Size = new System.Drawing.Size(48, 24);
			this._buttonEN.TabIndex = 20;
			this._buttonEN.TabStop = false;
			this._buttonEN.Text = "EN";
			this._buttonEN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonEN.UseVisualStyleBackColor = true;
			// 
			// _buttonES
			// 
			this._buttonES.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonES.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonES.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonES.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonES.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonES.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonES.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonES.Image = global::Mtgdb.Gui.Properties.Resources.es;
			this._buttonES.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonES.Location = new System.Drawing.Point(49, 1);
			this._buttonES.Margin = new System.Windows.Forms.Padding(0);
			this._buttonES.Name = "_buttonES";
			this._buttonES.Size = new System.Drawing.Size(48, 24);
			this._buttonES.TabIndex = 19;
			this._buttonES.TabStop = false;
			this._buttonES.Text = "ES";
			this._buttonES.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonES.UseVisualStyleBackColor = true;
			// 
			// _buttonCN
			// 
			this._buttonCN.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonCN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonCN.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonCN.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonCN.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonCN.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonCN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonCN.Image = global::Mtgdb.Gui.Properties.Resources.cn;
			this._buttonCN.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonCN.Location = new System.Drawing.Point(1, 1);
			this._buttonCN.Margin = new System.Windows.Forms.Padding(0);
			this._buttonCN.Name = "_buttonCN";
			this._buttonCN.Size = new System.Drawing.Size(48, 24);
			this._buttonCN.TabIndex = 18;
			this._buttonCN.TabStop = false;
			this._buttonCN.Text = "CN";
			this._buttonCN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonCN.UseVisualStyleBackColor = true;
			// 
			// _menuDonate
			// 
			this._menuDonate.BackColor = System.Drawing.Color.White;
			this._menuDonate.Controls.Add(this._labelDonate);
			this._menuDonate.Controls.Add(this._buttonDonatePayPal);
			this._menuDonate.Controls.Add(this._buttonDonateYandexMoney);
			this._menuDonate.Controls.Add(this._panelAva);
			this._menuDonate.Location = new System.Drawing.Point(458, 117);
			this._menuDonate.Name = "_menuDonate";
			this._menuDonate.Size = new System.Drawing.Size(325, 167);
			this._menuDonate.TabIndex = 29;
			this._menuDonate.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _labelDonate
			// 
			this._labelDonate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._labelDonate.Location = new System.Drawing.Point(99, 6);
			this._labelDonate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 0);
			this._labelDonate.Name = "_labelDonate";
			this._labelDonate.Size = new System.Drawing.Size(220, 117);
			this._labelDonate.TabIndex = 31;
			this._labelDonate.Text = "This application is free.\r\n\r\nIf you like it, consider donating to support its mai" +
    "ntenance and further development.\r\n\r\nThank you!";
			// 
			// _buttonDonatePayPal
			// 
			this._buttonDonatePayPal.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDonatePayPal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonDonatePayPal.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonDonatePayPal.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonDonatePayPal.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDonatePayPal.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDonatePayPal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDonatePayPal.Image = global::Mtgdb.Gui.Properties.Resources.paypal_32;
			this._buttonDonatePayPal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonDonatePayPal.Location = new System.Drawing.Point(183, 129);
			this._buttonDonatePayPal.Margin = new System.Windows.Forms.Padding(6);
			this._buttonDonatePayPal.Name = "_buttonDonatePayPal";
			this._buttonDonatePayPal.Size = new System.Drawing.Size(136, 32);
			this._buttonDonatePayPal.TabIndex = 30;
			this._buttonDonatePayPal.TabStop = false;
			this._buttonDonatePayPal.Text = "Donate via PayPal";
			this._buttonDonatePayPal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonDonatePayPal.UseVisualStyleBackColor = true;
			// 
			// _buttonDonateYandexMoney
			// 
			this._buttonDonateYandexMoney.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDonateYandexMoney.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonDonateYandexMoney.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonDonateYandexMoney.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonDonateYandexMoney.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDonateYandexMoney.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDonateYandexMoney.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDonateYandexMoney.Image = global::Mtgdb.Gui.Properties.Resources.YandexMoney_32;
			this._buttonDonateYandexMoney.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonDonateYandexMoney.Location = new System.Drawing.Point(6, 129);
			this._buttonDonateYandexMoney.Margin = new System.Windows.Forms.Padding(6, 6, 0, 6);
			this._buttonDonateYandexMoney.Name = "_buttonDonateYandexMoney";
			this._buttonDonateYandexMoney.Size = new System.Drawing.Size(171, 32);
			this._buttonDonateYandexMoney.TabIndex = 29;
			this._buttonDonateYandexMoney.TabStop = false;
			this._buttonDonateYandexMoney.Text = "Donate via YandexMoney";
			this._buttonDonateYandexMoney.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this._buttonDonateYandexMoney.UseVisualStyleBackColor = true;
			// 
			// _panelAva
			// 
			this._panelAva.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("_panelAva.BackgroundImage")));
			this._panelAva.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._panelAva.Location = new System.Drawing.Point(6, 6);
			this._panelAva.Margin = new System.Windows.Forms.Padding(6, 6, 0, 0);
			this._panelAva.Name = "_panelAva";
			this._panelAva.Size = new System.Drawing.Size(87, 117);
			this._panelAva.TabIndex = 0;
			// 
			// _menuConfig
			// 
			this._menuConfig.BackColor = System.Drawing.Color.White;
			this._menuConfig.Controls.Add(this._buttonDisplaySettings);
			this._menuConfig.Controls.Add(this._buttonGeneralSettings);
			this._menuConfig.Location = new System.Drawing.Point(524, 50);
			this._menuConfig.Name = "_menuConfig";
			this._menuConfig.Size = new System.Drawing.Size(189, 50);
			this._menuConfig.TabIndex = 30;
			this._menuConfig.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonDisplaySettings
			// 
			this._buttonDisplaySettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDisplaySettings.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonDisplaySettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonDisplaySettings.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonDisplaySettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonDisplaySettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDisplaySettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonDisplaySettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonDisplaySettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonDisplaySettings.Location = new System.Drawing.Point(1, 25);
			this._buttonDisplaySettings.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonDisplaySettings.Name = "_buttonDisplaySettings";
			this._buttonDisplaySettings.Size = new System.Drawing.Size(187, 24);
			this._buttonDisplaySettings.TabIndex = 20;
			this._buttonDisplaySettings.TabStop = false;
			this._buttonDisplaySettings.Text = "Display settings";
			this._buttonDisplaySettings.UseVisualStyleBackColor = true;
			// 
			// _buttonGeneralSettings
			// 
			this._buttonGeneralSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonGeneralSettings.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonGeneralSettings.BackColor = System.Drawing.Color.White;
			this._buttonGeneralSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonGeneralSettings.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonGeneralSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonGeneralSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonGeneralSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonGeneralSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonGeneralSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonGeneralSettings.Location = new System.Drawing.Point(1, 1);
			this._buttonGeneralSettings.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonGeneralSettings.Name = "_buttonGeneralSettings";
			this._buttonGeneralSettings.Size = new System.Drawing.Size(187, 24);
			this._buttonGeneralSettings.TabIndex = 19;
			this._buttonGeneralSettings.TabStop = false;
			this._buttonGeneralSettings.Text = "General settings";
			this._buttonGeneralSettings.UseVisualStyleBackColor = false;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(76, 178);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(205, 41);
			this.label2.TabIndex = 36;
			this.label2.Text = "* MTGO .txt format is supported in many websites including magic.wizards.com, www" +
    ".mtggoldfish.com";
			// 
			// FormRoot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1000, 800);
			this.Controls.Add(this._menuOpen);
			this.Controls.Add(this._menuConfig);
			this.Controls.Add(this._menuDonate);
			this.Controls.Add(this._menuLanguage);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImageClose = global::Mtgdb.Gui.Properties.Resources.close;
			this.ImageMaximize = global::Mtgdb.Gui.Properties.Resources.maximize;
			this.ImageMinimize = global::Mtgdb.Gui.Properties.Resources.minimize;
			this.ImageNormalize = global::Mtgdb.Gui.Properties.Resources.normalize;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "FormRoot";
			this.Text = "FormTabbed";
			this.TitleHeight = 33;
			this.Controls.SetChildIndex(this._panelClient, 0);
			this.Controls.SetChildIndex(this._panelHeader, 0);
			this.Controls.SetChildIndex(this._menuLanguage, 0);
			this.Controls.SetChildIndex(this._menuDonate, 0);
			this.Controls.SetChildIndex(this._menuConfig, 0);
			this.Controls.SetChildIndex(this._menuOpen, 0);
			this._panelHeader.ResumeLayout(false);
			this._menuOpen.ResumeLayout(false);
			this._menuOpen.PerformLayout();
			this._menuLanguage.ResumeLayout(false);
			this._menuDonate.ResumeLayout(false);
			this._menuConfig.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private Mtgdb.Controls.CustomCheckBox _buttonUndo;
		private Mtgdb.Controls.CustomCheckBox _buttonRedo;
		private Mtgdb.Controls.CustomCheckBox _buttonDonate;
		private Mtgdb.Controls.CustomCheckBox _buttonDownload;
		private Mtgdb.Controls.CustomCheckBox _buttonLanguage;
		private Mtgdb.Controls.CustomCheckBox _buttonConfig;
		private Mtgdb.Controls.CustomCheckBox _buttonHelp;
		private Mtgdb.Controls.CustomCheckBox _buttonOpenDeck;
		private Mtgdb.Controls.CustomCheckBox _buttonSaveDeck;
		private Mtgdb.Controls.CustomCheckBox _buttonStat;
		private Mtgdb.Controls.CustomCheckBox _buttonPrint;
		private Mtgdb.Controls.CustomCheckBox _buttonClear;
		private Mtgdb.Controls.CustomCheckBox _buttonTooltips;
		private Controls.TabHeaderControl _tabs;
		Mtgdb.Controls.CustomPanel _menuOpen;
		private System.Windows.Forms.LinkLabel _buttonVisitForge;
		private System.Windows.Forms.LinkLabel _buttonVisitXMage;
		private System.Windows.Forms.LinkLabel _buttonVisitMagarena;
		private System.Windows.Forms.LinkLabel _buttonVisitCockatrice;
		private System.Windows.Forms.LinkLabel _buttonVisitDotP2014;
		private System.Windows.Forms.Label _labelFormats;
		private System.Windows.Forms.Label _labelMagarena2;
		private System.Windows.Forms.Label _labelDotP2;
		private Controls.CustomPanel _menuLanguage;
		private Mtgdb.Controls.CustomCheckBox _buttonPT;
		private Mtgdb.Controls.CustomCheckBox _buttonTW;
		private Mtgdb.Controls.CustomCheckBox _buttonRU;
		private Mtgdb.Controls.CustomCheckBox _buttonIT;
		private Mtgdb.Controls.CustomCheckBox _buttonKR;
		private Mtgdb.Controls.CustomCheckBox _buttonDE;
		private Mtgdb.Controls.CustomCheckBox _buttonFR;
		private Mtgdb.Controls.CustomCheckBox _buttonJP;
		private Mtgdb.Controls.CustomCheckBox _buttonEN;
		private Mtgdb.Controls.CustomCheckBox _buttonES;
		private Mtgdb.Controls.CustomCheckBox _buttonCN;
		private Controls.CustomPanel _menuDonate;
		private System.Windows.Forms.Panel _panelAva;
		private Mtgdb.Controls.CustomCheckBox _buttonDonatePayPal;
		private Mtgdb.Controls.CustomCheckBox _buttonDonateYandexMoney;
		private System.Windows.Forms.Label _labelDonate;
		private Controls.CustomPanel _menuConfig;
		private Mtgdb.Controls.CustomCheckBox _buttonDisplaySettings;
		private Mtgdb.Controls.CustomCheckBox _buttonGeneralSettings;
		private Controls.CustomCheckBox _buttonMenuOpenDeck;
		private Controls.CustomCheckBox _buttonMenuOpenCollection;
		private System.Windows.Forms.LinkLabel _buttonVisitMtgo;
		private Controls.CustomCheckBox _buttonMenuSaveDeck;
		private Controls.CustomCheckBox _buttonMenuSaveCollection;
		private System.Windows.Forms.Label label2;
	}
}