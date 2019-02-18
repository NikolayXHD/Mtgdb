using System.ComponentModel;
using System.Windows.Forms;

namespace Mtgdb.Downloader
{
	sealed partial class FormUpdate
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
			this._textBoxLog = new System.Windows.Forms.RichTextBox();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._labelProgress = new System.Windows.Forms.Label();
			this._tableLayoutRoot = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutButtons = new System.Windows.Forms.TableLayoutPanel();
			this._buttonPrices = new Mtgdb.Controls.ButtonBase();
			this._buttonApp = new Mtgdb.Controls.ButtonBase();
			this._buttonImgMq = new Mtgdb.Controls.ButtonBase();
			this._buttonImgArt = new Mtgdb.Controls.ButtonBase();
			this._buttonImgLq = new Mtgdb.Controls.ButtonBase();
			this._buttonMtgjson = new Mtgdb.Controls.ButtonBase();
			this._buttonEditConfig = new Mtgdb.Controls.ButtonBase();
			this._buttonDesktopShortcut = new Mtgdb.Controls.ButtonBase();
			this._buttonNotifications = new Mtgdb.Controls.ButtonBase();
			this._labelTitle = new System.Windows.Forms.Label();
			this._panelClient.SuspendLayout();
			this._panelCaption.SuspendLayout();
			this._tableLayoutRoot.SuspendLayout();
			this._tableLayoutButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelClient
			// 
			this._panelClient.Controls.Add(this._tableLayoutRoot);
			this._panelClient.Location = new System.Drawing.Point(6, 31);
			this._panelClient.Size = new System.Drawing.Size(827, 418);
			// 
			// _panelCaption
			// 
			this._panelCaption.Controls.Add(this._labelTitle);
			this._panelCaption.Size = new System.Drawing.Size(722, 25);
			// 
			// _textBoxLog
			// 
			this._textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._textBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textBoxLog.Location = new System.Drawing.Point(4, 3);
			this._textBoxLog.Margin = new System.Windows.Forms.Padding(4, 3, 4, 0);
			this._textBoxLog.Name = "_textBoxLog";
			this._textBoxLog.ReadOnly = true;
			this._textBoxLog.Size = new System.Drawing.Size(819, 260);
			this._textBoxLog.TabIndex = 0;
			this._textBoxLog.Text = "";
			// 
			// _progressBar
			// 
			this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._progressBar.Location = new System.Drawing.Point(4, 266);
			this._progressBar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 0);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(819, 25);
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBar.TabIndex = 1;
			this._progressBar.Visible = false;
			// 
			// _labelProgress
			// 
			this._labelProgress.AutoSize = true;
			this._labelProgress.Location = new System.Drawing.Point(4, 294);
			this._labelProgress.Margin = new System.Windows.Forms.Padding(4, 3, 0, 0);
			this._labelProgress.Name = "_labelProgress";
			this._labelProgress.Size = new System.Drawing.Size(145, 13);
			this._labelProgress.TabIndex = 9;
			this._labelProgress.Text = "121 / 10030 files ready";
			this._labelProgress.Visible = false;
			// 
			// _tableLayoutRoot
			// 
			this._tableLayoutRoot.BackColor = System.Drawing.SystemColors.Control;
			this._tableLayoutRoot.ColumnCount = 1;
			this._tableLayoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutRoot.Controls.Add(this._labelProgress, 0, 2);
			this._tableLayoutRoot.Controls.Add(this._progressBar, 0, 1);
			this._tableLayoutRoot.Controls.Add(this._textBoxLog, 0, 0);
			this._tableLayoutRoot.Controls.Add(this._tableLayoutButtons, 0, 3);
			this._tableLayoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutRoot.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutRoot.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutRoot.Name = "_tableLayoutRoot";
			this._tableLayoutRoot.RowCount = 4;
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRoot.Size = new System.Drawing.Size(827, 418);
			this._tableLayoutRoot.TabIndex = 0;
			// 
			// _tableLayoutButtons
			// 
			this._tableLayoutButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutButtons.BackColor = System.Drawing.SystemColors.Control;
			this._tableLayoutButtons.ColumnCount = 7;
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.23077F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.923077F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.23077F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.923077F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.23077F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.23077F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.23077F));
			this._tableLayoutButtons.Controls.Add(this._buttonPrices, 4, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonApp, 6, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonImgMq, 5, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonImgArt, 6, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonImgLq, 4, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonMtgjson, 5, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonEditConfig, 0, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonDesktopShortcut, 0, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonNotifications, 2, 1);
			this._tableLayoutButtons.Location = new System.Drawing.Point(4, 310);
			this._tableLayoutButtons.Margin = new System.Windows.Forms.Padding(4, 3, 0, 0);
			this._tableLayoutButtons.Name = "_tableLayoutButtons";
			this._tableLayoutButtons.RowCount = 2;
			this._tableLayoutButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutButtons.Size = new System.Drawing.Size(823, 108);
			this._tableLayoutButtons.TabIndex = 11;
			// 
			// _buttonPrices
			// 
			this._buttonPrices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonPrices.AutoCheck = false;
			this._buttonPrices.BackColor = System.Drawing.Color.Transparent;
			this._buttonPrices.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonPrices.Image = global::Mtgdb.Downloader.Properties.Resources.price_16;
			this._buttonPrices.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonPrices.Location = new System.Drawing.Point(346, 0);
			this._buttonPrices.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonPrices.Name = "_buttonPrices";
			this._buttonPrices.Padding = new System.Windows.Forms.Padding(4);
			this._buttonPrices.Size = new System.Drawing.Size(154, 51);
			this._buttonPrices.TabIndex = 3;
			this._buttonPrices.Text = "Update\r\ncard prices";
			this._buttonPrices.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonPrices.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonPrices.VisibleAllBorders = true;
			this._buttonPrices.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonApp
			// 
			this._buttonApp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonApp.AutoCheck = false;
			this._buttonApp.BackColor = System.Drawing.Color.Transparent;
			this._buttonApp.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonApp.Image = global::Mtgdb.Downloader.Properties.Resources.update;
			this._buttonApp.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonApp.Location = new System.Drawing.Point(662, 0);
			this._buttonApp.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonApp.Name = "_buttonApp";
			this._buttonApp.Padding = new System.Windows.Forms.Padding(4);
			this._buttonApp.Size = new System.Drawing.Size(157, 51);
			this._buttonApp.TabIndex = 7;
			this._buttonApp.Text = "Update\r\nMtgdb.Gui";
			this._buttonApp.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonApp.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonApp.VisibleAllBorders = true;
			this._buttonApp.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonImgMq
			// 
			this._buttonImgMq.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgMq.AutoCheck = false;
			this._buttonImgMq.BackColor = System.Drawing.Color.Transparent;
			this._buttonImgMq.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonImgMq.Image = global::Mtgdb.Downloader.Properties.Resources.card_img_24;
			this._buttonImgMq.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonImgMq.Location = new System.Drawing.Point(504, 54);
			this._buttonImgMq.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonImgMq.Name = "_buttonImgMq";
			this._buttonImgMq.Padding = new System.Windows.Forms.Padding(4);
			this._buttonImgMq.Size = new System.Drawing.Size(154, 51);
			this._buttonImgMq.TabIndex = 6;
			this._buttonImgMq.Text = "Download\r\nzoom images";
			this._buttonImgMq.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonImgMq.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonImgMq.VisibleAllBorders = true;
			this._buttonImgMq.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonImgArt
			// 
			this._buttonImgArt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgArt.AutoCheck = false;
			this._buttonImgArt.BackColor = System.Drawing.Color.Transparent;
			this._buttonImgArt.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonImgArt.Image = global::Mtgdb.Downloader.Properties.Resources.art_24;
			this._buttonImgArt.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonImgArt.Location = new System.Drawing.Point(662, 54);
			this._buttonImgArt.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonImgArt.Name = "_buttonImgArt";
			this._buttonImgArt.Padding = new System.Windows.Forms.Padding(4);
			this._buttonImgArt.Size = new System.Drawing.Size(157, 51);
			this._buttonImgArt.TabIndex = 8;
			this._buttonImgArt.Text = "Download\r\nartwork images";
			this._buttonImgArt.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonImgArt.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonImgArt.VisibleAllBorders = true;
			this._buttonImgArt.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonImgLq
			// 
			this._buttonImgLq.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgLq.AutoCheck = false;
			this._buttonImgLq.BackColor = System.Drawing.Color.Transparent;
			this._buttonImgLq.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonImgLq.Image = global::Mtgdb.Downloader.Properties.Resources.card_img_16;
			this._buttonImgLq.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonImgLq.Location = new System.Drawing.Point(346, 54);
			this._buttonImgLq.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonImgLq.Name = "_buttonImgLq";
			this._buttonImgLq.Padding = new System.Windows.Forms.Padding(4);
			this._buttonImgLq.Size = new System.Drawing.Size(154, 51);
			this._buttonImgLq.TabIndex = 4;
			this._buttonImgLq.Text = "Download\r\nsmall images";
			this._buttonImgLq.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonImgLq.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonImgLq.VisibleAllBorders = true;
			this._buttonImgLq.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonMtgjson
			// 
			this._buttonMtgjson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonMtgjson.AutoCheck = false;
			this._buttonMtgjson.BackColor = System.Drawing.Color.Transparent;
			this._buttonMtgjson.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonMtgjson.Image = global::Mtgdb.Downloader.Properties.Resources.card_data_20;
			this._buttonMtgjson.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonMtgjson.Location = new System.Drawing.Point(504, 0);
			this._buttonMtgjson.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonMtgjson.Name = "_buttonMtgjson";
			this._buttonMtgjson.Padding = new System.Windows.Forms.Padding(4);
			this._buttonMtgjson.Size = new System.Drawing.Size(154, 51);
			this._buttonMtgjson.TabIndex = 5;
			this._buttonMtgjson.Text = "Update cards\r\nfrom mtgjson.com";
			this._buttonMtgjson.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonMtgjson.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonMtgjson.VisibleAllBorders = true;
			this._buttonMtgjson.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonEditConfig
			// 
			this._buttonEditConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonEditConfig.AutoCheck = false;
			this._buttonEditConfig.BackColor = System.Drawing.Color.Transparent;
			this._buttonEditConfig.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonEditConfig.Image = global::Mtgdb.Downloader.Properties.Resources.properties_16x16;
			this._buttonEditConfig.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonEditConfig.Location = new System.Drawing.Point(0, 0);
			this._buttonEditConfig.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonEditConfig.Name = "_buttonEditConfig";
			this._buttonEditConfig.Padding = new System.Windows.Forms.Padding(4);
			this._buttonEditConfig.Size = new System.Drawing.Size(154, 51);
			this._buttonEditConfig.TabIndex = 0;
			this._buttonEditConfig.Text = "Edit\r\nconfiguration";
			this._buttonEditConfig.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonEditConfig.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonEditConfig.VisibleAllBorders = true;
			this._buttonEditConfig.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _buttonDesktopShortcut
			// 
			this._buttonDesktopShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDesktopShortcut.AutoCheck = false;
			this._buttonDesktopShortcut.BackColor = System.Drawing.Color.Transparent;
			this._buttonDesktopShortcut.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonDesktopShortcut.Image = global::Mtgdb.Downloader.Properties.Resources.mtg_16;
			this._buttonDesktopShortcut.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonDesktopShortcut.Location = new System.Drawing.Point(0, 54);
			this._buttonDesktopShortcut.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonDesktopShortcut.Name = "_buttonDesktopShortcut";
			this._buttonDesktopShortcut.Padding = new System.Windows.Forms.Padding(4);
			this._buttonDesktopShortcut.Size = new System.Drawing.Size(154, 51);
			this._buttonDesktopShortcut.TabIndex = 1;
			this._buttonDesktopShortcut.Text = "Create\r\ndesktop shortcut";
			this._buttonDesktopShortcut.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonDesktopShortcut.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonDesktopShortcut.VisibleAllBorders = true;
			this._buttonDesktopShortcut.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDesktopShortcut.Click += new System.EventHandler(this.buttonDesktopShortcut);
			// 
			// _buttonNotifications
			// 
			this._buttonNotifications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonNotifications.AutoCheck = false;
			this._buttonNotifications.BackColor = System.Drawing.Color.Transparent;
			this._buttonNotifications.ForeColor = System.Drawing.SystemColors.ControlText;
			this._buttonNotifications.Image = global::Mtgdb.Downloader.Properties.Resources.mailbox_26;
			this._buttonNotifications.ImagePosition = System.Drawing.StringAlignment.Near;
			this._buttonNotifications.Location = new System.Drawing.Point(173, 54);
			this._buttonNotifications.Margin = new System.Windows.Forms.Padding(0, 0, 4, 3);
			this._buttonNotifications.Name = "_buttonNotifications";
			this._buttonNotifications.Padding = new System.Windows.Forms.Padding(4);
			this._buttonNotifications.Size = new System.Drawing.Size(154, 51);
			this._buttonNotifications.TabIndex = 2;
			this._buttonNotifications.Text = "Re-show\r\nnotifications";
			this._buttonNotifications.TextAlign = System.Drawing.StringAlignment.Center;
			this._buttonNotifications.TextPosition = System.Drawing.StringAlignment.Center;
			this._buttonNotifications.VisibleAllBorders = true;
			this._buttonNotifications.VisibleBorders = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			// 
			// _labelTitle
			// 
			this._labelTitle.AutoSize = true;
			this._labelTitle.BackColor = System.Drawing.Color.Transparent;
			this._labelTitle.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._labelTitle.Location = new System.Drawing.Point(0, 0);
			this._labelTitle.Margin = new System.Windows.Forms.Padding(0);
			this._labelTitle.Name = "_labelTitle";
			this._labelTitle.Size = new System.Drawing.Size(198, 19);
			this._labelTitle.TabIndex = 0;
			this._labelTitle.Text = "Updates and downloads";
			// 
			// FormUpdate
			// 
			this.ClientSize = new System.Drawing.Size(839, 455);
			this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Location = new System.Drawing.Point(0, 0);
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "FormUpdate";
			this.Text = "Mtgdb.Gui updater";
			this._panelClient.ResumeLayout(false);
			this._panelCaption.ResumeLayout(false);
			this._panelCaption.PerformLayout();
			this._tableLayoutRoot.ResumeLayout(false);
			this._tableLayoutRoot.PerformLayout();
			this._tableLayoutButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private Mtgdb.Controls.ButtonBase _buttonApp;
		private RichTextBox _textBoxLog;
		private Mtgdb.Controls.ButtonBase _buttonImgArt;
		private Mtgdb.Controls.ButtonBase _buttonMtgjson;
		private Mtgdb.Controls.ButtonBase _buttonImgMq;
		private Mtgdb.Controls.ButtonBase _buttonImgLq;
		private Mtgdb.Controls.ButtonBase _buttonDesktopShortcut;
		private Mtgdb.Controls.ButtonBase _buttonEditConfig;
		private ProgressBar _progressBar;
		private Label _labelProgress;
		private TableLayoutPanel _tableLayoutRoot;
		private TableLayoutPanel _tableLayoutButtons;
		private Mtgdb.Controls.ButtonBase _buttonPrices;
		private Mtgdb.Controls.ButtonBase _buttonNotifications;
		private Label _labelTitle;
	}
}