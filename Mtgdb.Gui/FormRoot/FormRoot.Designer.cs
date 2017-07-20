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
			this._buttonSave = new Mtgdb.Controls.CustomCheckBox();
			this._buttonLoad = new Mtgdb.Controls.CustomCheckBox();
			this._buttonTooltips = new Mtgdb.Controls.CustomCheckBox();
			this._menuOpen = new Mtgdb.Controls.CustomPanel();
			this._labelDotP2 = new System.Windows.Forms.Label();
			this._labelDotP = new System.Windows.Forms.Label();
			this._labelMagarena = new System.Windows.Forms.Label();
			this._labelMagarena2 = new System.Windows.Forms.Label();
			this._labelSave = new System.Windows.Forms.Label();
			this._labelLoad = new System.Windows.Forms.Label();
			this._buttonVisitDotP2014 = new System.Windows.Forms.LinkLabel();
			this._buttonVisitCockatrice = new System.Windows.Forms.LinkLabel();
			this._buttonVisitMagarena = new System.Windows.Forms.LinkLabel();
			this._buttonVisitXMage = new System.Windows.Forms.LinkLabel();
			this._buttonVisitForge = new System.Windows.Forms.LinkLabel();
			this._labelFormats = new System.Windows.Forms.Label();
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
			this._menuHelp = new Mtgdb.Controls.CustomPanel();
			this._buttonHelpReadme = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHelpReleaseNotes = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHelpSort = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHelpSearch = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHelpButtons = new Mtgdb.Controls.CustomCheckBox();
			this._buttonHelpEditor = new Mtgdb.Controls.CustomCheckBox();
			this._menuConfig = new Mtgdb.Controls.CustomPanel();
			this._buttonDisplaySettings = new Mtgdb.Controls.CustomCheckBox();
			this._buttonGeneralSettings = new Mtgdb.Controls.CustomCheckBox();
			this._panelHeader.SuspendLayout();
			this._menuOpen.SuspendLayout();
			this._menuLanguage.SuspendLayout();
			this._menuDonate.SuspendLayout();
			this._menuHelp.SuspendLayout();
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
			this._panelHeader.Controls.Add(this._buttonLoad);
			this._panelHeader.Controls.Add(this._buttonSave);
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
			// _buttonSave
			// 
			this._buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonSave.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonSave.FlatAppearance.BorderSize = 0;
			this._buttonSave.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonSave.Image = global::Mtgdb.Gui.Properties.Resources.save_16x16;
			this._buttonSave.Location = new System.Drawing.Point(455, 3);
			this._buttonSave.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonSave.Name = "_buttonSave";
			this._buttonSave.Size = new System.Drawing.Size(30, 24);
			this._buttonSave.TabIndex = 16;
			this._buttonSave.TabStop = false;
			this._buttonSave.UseVisualStyleBackColor = true;
			// 
			// _buttonLoad
			// 
			this._buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonLoad.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonLoad.FlatAppearance.BorderSize = 0;
			this._buttonLoad.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(220)))));
			this._buttonLoad.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
			this._buttonLoad.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(230)))));
			this._buttonLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonLoad.Image = global::Mtgdb.Gui.Properties.Resources.open2_16x16;
			this._buttonLoad.Location = new System.Drawing.Point(425, 3);
			this._buttonLoad.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
			this._buttonLoad.Name = "_buttonLoad";
			this._buttonLoad.Size = new System.Drawing.Size(30, 24);
			this._buttonLoad.TabIndex = 17;
			this._buttonLoad.TabStop = false;
			this._buttonLoad.UseVisualStyleBackColor = true;
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
			this._menuOpen.Controls.Add(this._labelDotP2);
			this._menuOpen.Controls.Add(this._labelDotP);
			this._menuOpen.Controls.Add(this._labelMagarena);
			this._menuOpen.Controls.Add(this._labelMagarena2);
			this._menuOpen.Controls.Add(this._labelSave);
			this._menuOpen.Controls.Add(this._labelLoad);
			this._menuOpen.Controls.Add(this._buttonVisitDotP2014);
			this._menuOpen.Controls.Add(this._buttonVisitCockatrice);
			this._menuOpen.Controls.Add(this._buttonVisitMagarena);
			this._menuOpen.Controls.Add(this._buttonVisitXMage);
			this._menuOpen.Controls.Add(this._buttonVisitForge);
			this._menuOpen.Controls.Add(this._labelFormats);
			this._menuOpen.Location = new System.Drawing.Point(0, 50);
			this._menuOpen.Name = "_menuOpen";
			this._menuOpen.Size = new System.Drawing.Size(288, 144);
			this._menuOpen.TabIndex = 0;
			this._menuOpen.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _labelDotP2
			// 
			this._labelDotP2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._labelDotP2.AutoSize = true;
			this._labelDotP2.BackColor = System.Drawing.Color.Transparent;
			this._labelDotP2.Location = new System.Drawing.Point(56, 128);
			this._labelDotP2.Margin = new System.Windows.Forms.Padding(3);
			this._labelDotP2.Name = "_labelDotP2";
			this._labelDotP2.Size = new System.Drawing.Size(229, 13);
			this._labelDotP2.TabIndex = 12;
			this._labelDotP2.Text = "** a modified version supporting Forge format";
			// 
			// _labelDotP
			// 
			this._labelDotP.AutoSize = true;
			this._labelDotP.BackColor = System.Drawing.Color.Transparent;
			this._labelDotP.Location = new System.Drawing.Point(256, 46);
			this._labelDotP.Margin = new System.Windows.Forms.Padding(3);
			this._labelDotP.Name = "_labelDotP";
			this._labelDotP.Size = new System.Drawing.Size(19, 13);
			this._labelDotP.TabIndex = 11;
			this._labelDotP.Text = "**";
			// 
			// _labelMagarena
			// 
			this._labelMagarena.AutoSize = true;
			this._labelMagarena.BackColor = System.Drawing.Color.Transparent;
			this._labelMagarena.Location = new System.Drawing.Point(180, 46);
			this._labelMagarena.Margin = new System.Windows.Forms.Padding(3);
			this._labelMagarena.Name = "_labelMagarena";
			this._labelMagarena.Size = new System.Drawing.Size(13, 13);
			this._labelMagarena.TabIndex = 10;
			this._labelMagarena.Text = "*";
			// 
			// _labelMagarena2
			// 
			this._labelMagarena2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._labelMagarena2.AutoSize = true;
			this._labelMagarena2.BackColor = System.Drawing.Color.Transparent;
			this._labelMagarena2.Location = new System.Drawing.Point(63, 115);
			this._labelMagarena2.Margin = new System.Windows.Forms.Padding(3);
			this._labelMagarena2.Name = "_labelMagarena2";
			this._labelMagarena2.Size = new System.Drawing.Size(204, 13);
			this._labelMagarena2.TabIndex = 9;
			this._labelMagarena2.Text = "* supports the format used by Magarena";
			// 
			// _labelSave
			// 
			this._labelSave.AutoSize = true;
			this._labelSave.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelSave.Location = new System.Drawing.Point(3, 3);
			this._labelSave.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._labelSave.Name = "_labelSave";
			this._labelSave.Size = new System.Drawing.Size(97, 13);
			this._labelSave.TabIndex = 8;
			this._labelSave.Text = "Save file: Ctrl+S";
			// 
			// _labelLoad
			// 
			this._labelLoad.AutoSize = true;
			this._labelLoad.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelLoad.Location = new System.Drawing.Point(3, 3);
			this._labelLoad.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._labelLoad.Name = "_labelLoad";
			this._labelLoad.Size = new System.Drawing.Size(99, 13);
			this._labelLoad.TabIndex = 7;
			this._labelLoad.Text = "Open file: Ctrl+O";
			// 
			// _buttonVisitDotP2014
			// 
			this._buttonVisitDotP2014.ActiveLinkColor = System.Drawing.Color.Blue;
			this._buttonVisitDotP2014.BackColor = System.Drawing.Color.Transparent;
			this._buttonVisitDotP2014.Image = global::Mtgdb.Gui.Properties.Resources.DotP2014_32;
			this._buttonVisitDotP2014.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this._buttonVisitDotP2014.LinkColor = System.Drawing.Color.Blue;
			this._buttonVisitDotP2014.Location = new System.Drawing.Point(193, 46);
			this._buttonVisitDotP2014.Margin = new System.Windows.Forms.Padding(0);
			this._buttonVisitDotP2014.Name = "_buttonVisitDotP2014";
			this._buttonVisitDotP2014.Size = new System.Drawing.Size(89, 57);
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
			this._buttonVisitCockatrice.Location = new System.Drawing.Point(136, 45);
			this._buttonVisitCockatrice.Margin = new System.Windows.Forms.Padding(0);
			this._buttonVisitCockatrice.Name = "_buttonVisitCockatrice";
			this._buttonVisitCockatrice.Size = new System.Drawing.Size(57, 45);
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
			this._buttonVisitMagarena.Location = new System.Drawing.Point(81, 45);
			this._buttonVisitMagarena.Margin = new System.Windows.Forms.Padding(0);
			this._buttonVisitMagarena.Name = "_buttonVisitMagarena";
			this._buttonVisitMagarena.Size = new System.Drawing.Size(55, 45);
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
			this._buttonVisitXMage.Location = new System.Drawing.Point(42, 45);
			this._buttonVisitXMage.Margin = new System.Windows.Forms.Padding(0);
			this._buttonVisitXMage.Name = "_buttonVisitXMage";
			this._buttonVisitXMage.Size = new System.Drawing.Size(39, 45);
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
			this._buttonVisitForge.Location = new System.Drawing.Point(4, 45);
			this._buttonVisitForge.Margin = new System.Windows.Forms.Padding(0);
			this._buttonVisitForge.Name = "_buttonVisitForge";
			this._buttonVisitForge.Size = new System.Drawing.Size(35, 45);
			this._buttonVisitForge.TabIndex = 0;
			this._buttonVisitForge.TabStop = true;
			this._buttonVisitForge.Text = "Forge";
			this._buttonVisitForge.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// _labelFormats
			// 
			this._labelFormats.AutoSize = true;
			this._labelFormats.BackColor = System.Drawing.Color.Transparent;
			this._labelFormats.Location = new System.Drawing.Point(3, 28);
			this._labelFormats.Margin = new System.Windows.Forms.Padding(3);
			this._labelFormats.Name = "_labelFormats";
			this._labelFormats.Size = new System.Drawing.Size(126, 13);
			this._labelFormats.TabIndex = 6;
			this._labelFormats.Text = "Supported deck formats:";
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
			this._menuDonate.Location = new System.Drawing.Point(0, 202);
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
			this._labelDonate.Text = "This application is free.\r\n\r\nIt shows no advertising or other scrap\r\nbecause it w" +
    "as made not for money but because of inspiration.\r\n\r\nHovewer I won\'t blame you i" +
    "f you reward \r\nmy efforts :) Thank you!";
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
			// _menuHelp
			// 
			this._menuHelp.BackColor = System.Drawing.Color.White;
			this._menuHelp.Controls.Add(this._buttonHelpReadme);
			this._menuHelp.Controls.Add(this._buttonHelpReleaseNotes);
			this._menuHelp.Controls.Add(this._buttonHelpSort);
			this._menuHelp.Controls.Add(this._buttonHelpSearch);
			this._menuHelp.Controls.Add(this._buttonHelpButtons);
			this._menuHelp.Controls.Add(this._buttonHelpEditor);
			this._menuHelp.Location = new System.Drawing.Point(300, 50);
			this._menuHelp.Name = "_menuHelp";
			this._menuHelp.Size = new System.Drawing.Size(189, 146);
			this._menuHelp.TabIndex = 29;
			this._menuHelp.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonHelpReadme
			// 
			this._buttonHelpReadme.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHelpReadme.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonHelpReadme.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonHelpReadme.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonHelpReadme.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpReadme.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpReadme.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHelpReadme.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonHelpReadme.Location = new System.Drawing.Point(1, 121);
			this._buttonHelpReadme.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonHelpReadme.Name = "_buttonHelpReadme";
			this._buttonHelpReadme.Size = new System.Drawing.Size(187, 24);
			this._buttonHelpReadme.TabIndex = 24;
			this._buttonHelpReadme.TabStop = false;
			this._buttonHelpReadme.Text = "        Readme";
			this._buttonHelpReadme.UseVisualStyleBackColor = true;
			// 
			// _buttonHelpReleaseNotes
			// 
			this._buttonHelpReleaseNotes.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHelpReleaseNotes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonHelpReleaseNotes.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonHelpReleaseNotes.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonHelpReleaseNotes.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpReleaseNotes.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpReleaseNotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHelpReleaseNotes.Image = global::Mtgdb.Gui.Properties.Resources.update_bw;
			this._buttonHelpReleaseNotes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonHelpReleaseNotes.Location = new System.Drawing.Point(1, 97);
			this._buttonHelpReleaseNotes.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonHelpReleaseNotes.Name = "_buttonHelpReleaseNotes";
			this._buttonHelpReleaseNotes.Size = new System.Drawing.Size(187, 24);
			this._buttonHelpReleaseNotes.TabIndex = 23;
			this._buttonHelpReleaseNotes.TabStop = false;
			this._buttonHelpReleaseNotes.Text = "        Release notes";
			this._buttonHelpReleaseNotes.UseVisualStyleBackColor = true;
			// 
			// _buttonHelpSort
			// 
			this._buttonHelpSort.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHelpSort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonHelpSort.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonHelpSort.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonHelpSort.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpSort.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpSort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHelpSort.Image = global::Mtgdb.Gui.Properties.Resources.sort_20;
			this._buttonHelpSort.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonHelpSort.Location = new System.Drawing.Point(1, 73);
			this._buttonHelpSort.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonHelpSort.Name = "_buttonHelpSort";
			this._buttonHelpSort.Size = new System.Drawing.Size(187, 24);
			this._buttonHelpSort.TabIndex = 22;
			this._buttonHelpSort.TabStop = false;
			this._buttonHelpSort.Text = "        Help on Sort";
			this._buttonHelpSort.UseVisualStyleBackColor = true;
			// 
			// _buttonHelpSearch
			// 
			this._buttonHelpSearch.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHelpSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonHelpSearch.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonHelpSearch.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonHelpSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHelpSearch.Image = global::Mtgdb.Gui.Properties.Resources.Search_icon;
			this._buttonHelpSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonHelpSearch.Location = new System.Drawing.Point(1, 49);
			this._buttonHelpSearch.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonHelpSearch.Name = "_buttonHelpSearch";
			this._buttonHelpSearch.Size = new System.Drawing.Size(187, 24);
			this._buttonHelpSearch.TabIndex = 21;
			this._buttonHelpSearch.TabStop = false;
			this._buttonHelpSearch.Text = "        Help on Search string";
			this._buttonHelpSearch.UseVisualStyleBackColor = true;
			// 
			// _buttonHelpButtons
			// 
			this._buttonHelpButtons.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHelpButtons.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonHelpButtons.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonHelpButtons.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonHelpButtons.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpButtons.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpButtons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHelpButtons.Image = global::Mtgdb.Gui.Properties.Resources.Quick_filters;
			this._buttonHelpButtons.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonHelpButtons.Location = new System.Drawing.Point(1, 25);
			this._buttonHelpButtons.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonHelpButtons.Name = "_buttonHelpButtons";
			this._buttonHelpButtons.Size = new System.Drawing.Size(187, 24);
			this._buttonHelpButtons.TabIndex = 20;
			this._buttonHelpButtons.TabStop = false;
			this._buttonHelpButtons.Text = "        Help on Filter buttons";
			this._buttonHelpButtons.UseVisualStyleBackColor = true;
			// 
			// _buttonHelpEditor
			// 
			this._buttonHelpEditor.Appearance = System.Windows.Forms.Appearance.Button;
			this._buttonHelpEditor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this._buttonHelpEditor.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
			this._buttonHelpEditor.FlatAppearance.CheckedBackColor = System.Drawing.Color.White;
			this._buttonHelpEditor.FlatAppearance.MouseDownBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpEditor.FlatAppearance.MouseOverBackColor = System.Drawing.Color.GhostWhite;
			this._buttonHelpEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this._buttonHelpEditor.Image = global::Mtgdb.Gui.Properties.Resources.Box20;
			this._buttonHelpEditor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this._buttonHelpEditor.Location = new System.Drawing.Point(1, 1);
			this._buttonHelpEditor.Margin = new System.Windows.Forms.Padding(1, 1, 0, 0);
			this._buttonHelpEditor.Name = "_buttonHelpEditor";
			this._buttonHelpEditor.Size = new System.Drawing.Size(187, 24);
			this._buttonHelpEditor.TabIndex = 19;
			this._buttonHelpEditor.TabStop = false;
			this._buttonHelpEditor.Text = "        Help on deck / collection editor";
			this._buttonHelpEditor.UseVisualStyleBackColor = true;
			// 
			// _menuConfig
			// 
			this._menuConfig.BackColor = System.Drawing.Color.White;
			this._menuConfig.Controls.Add(this._buttonDisplaySettings);
			this._menuConfig.Controls.Add(this._buttonGeneralSettings);
			this._menuConfig.Location = new System.Drawing.Point(495, 50);
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
			// FormRoot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1000, 800);
			this.Controls.Add(this._menuConfig);
			this.Controls.Add(this._menuHelp);
			this.Controls.Add(this._menuDonate);
			this.Controls.Add(this._menuLanguage);
			this.Controls.Add(this._menuOpen);
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
			this.Controls.SetChildIndex(this._menuOpen, 0);
			this.Controls.SetChildIndex(this._menuLanguage, 0);
			this.Controls.SetChildIndex(this._menuDonate, 0);
			this.Controls.SetChildIndex(this._menuHelp, 0);
			this.Controls.SetChildIndex(this._menuConfig, 0);
			this._panelHeader.ResumeLayout(false);
			this._menuOpen.ResumeLayout(false);
			this._menuOpen.PerformLayout();
			this._menuLanguage.ResumeLayout(false);
			this._menuDonate.ResumeLayout(false);
			this._menuHelp.ResumeLayout(false);
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
		private Mtgdb.Controls.CustomCheckBox _buttonLoad;
		private Mtgdb.Controls.CustomCheckBox _buttonSave;
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
		private System.Windows.Forms.Label _labelLoad;
		private System.Windows.Forms.Label _labelSave;
		private System.Windows.Forms.Label _labelMagarena2;
		private System.Windows.Forms.Label _labelDotP2;
		private System.Windows.Forms.Label _labelDotP;
		private System.Windows.Forms.Label _labelMagarena;
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
		private Controls.CustomPanel _menuHelp;
		private Mtgdb.Controls.CustomCheckBox _buttonHelpEditor;
		private Mtgdb.Controls.CustomCheckBox _buttonHelpButtons;
		private Mtgdb.Controls.CustomCheckBox _buttonHelpSearch;
		private Mtgdb.Controls.CustomCheckBox _buttonHelpSort;
		private Mtgdb.Controls.CustomCheckBox _buttonHelpReleaseNotes;
		private Mtgdb.Controls.CustomCheckBox _buttonHelpReadme;
		private Controls.CustomPanel _menuConfig;
		private Mtgdb.Controls.CustomCheckBox _buttonDisplaySettings;
		private Mtgdb.Controls.CustomCheckBox _buttonGeneralSettings;
	}
}