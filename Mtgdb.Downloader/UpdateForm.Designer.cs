﻿using System.ComponentModel;
using System.Windows.Forms;

namespace Mtgdb.Downloader
{
	sealed partial class UpdateForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateForm));
			this._buttonApp = new System.Windows.Forms.Button();
			this._textBoxLog = new System.Windows.Forms.RichTextBox();
			this._buttonImgArt = new System.Windows.Forms.Button();
			this._buttonsMtgjson = new System.Windows.Forms.Button();
			this._buttonImgMq = new System.Windows.Forms.Button();
			this._buttonImgLq = new System.Windows.Forms.Button();
			this._buttonDesktopShortcut = new System.Windows.Forms.Button();
			this._buttonEditConfig = new System.Windows.Forms.Button();
			this._progressBar = new System.Windows.Forms.ProgressBar();
			this._labelProgress = new System.Windows.Forms.Label();
			this._tableLayoutRoot = new System.Windows.Forms.TableLayoutPanel();
			this._tableLayoutButtons = new System.Windows.Forms.TableLayoutPanel();
			this._buttonPrices = new System.Windows.Forms.Button();
			this._buttonNotifications = new System.Windows.Forms.Button();
			this._panelClient.SuspendLayout();
			this._tableLayoutRoot.SuspendLayout();
			this._tableLayoutButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelClient
			// 
			this._panelClient.Controls.Add(this._tableLayoutRoot);
			this._panelClient.Size = new System.Drawing.Size(691, 392);
			// 
			// _panelHeader
			// 
			this._panelHeader.Size = new System.Drawing.Size(598, 20);
			// 
			// _buttonApp
			// 
			this._buttonApp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonApp.AutoSize = true;
			this._buttonApp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonApp.Image = global::Mtgdb.Downloader.Properties.Resources.update;
			this._buttonApp.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonApp.Location = new System.Drawing.Point(554, 0);
			this._buttonApp.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonApp.Name = "_buttonApp";
			this._buttonApp.Size = new System.Drawing.Size(131, 47);
			this._buttonApp.TabIndex = 1;
			this._buttonApp.Text = "Update\r\nMtgdb.Gui";
			this._buttonApp.UseVisualStyleBackColor = true;
			// 
			// _textBoxLog
			// 
			this._textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._textBoxLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._textBoxLog.Location = new System.Drawing.Point(3, 3);
			this._textBoxLog.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._textBoxLog.Name = "_textBoxLog";
			this._textBoxLog.ReadOnly = true;
			this._textBoxLog.Size = new System.Drawing.Size(685, 244);
			this._textBoxLog.TabIndex = 0;
			this._textBoxLog.Text = "";
			// 
			// _buttonImgArt
			// 
			this._buttonImgArt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgArt.AutoSize = true;
			this._buttonImgArt.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonImgArt.Image = global::Mtgdb.Downloader.Properties.Resources.art_24;
			this._buttonImgArt.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonImgArt.Location = new System.Drawing.Point(554, 50);
			this._buttonImgArt.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonImgArt.Name = "_buttonImgArt";
			this._buttonImgArt.Size = new System.Drawing.Size(131, 47);
			this._buttonImgArt.TabIndex = 5;
			this._buttonImgArt.Text = "Download\r\nartwork images";
			this._buttonImgArt.UseVisualStyleBackColor = true;
			// 
			// _buttonsMtgjson
			// 
			this._buttonsMtgjson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonsMtgjson.AutoSize = true;
			this._buttonsMtgjson.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonsMtgjson.Image = global::Mtgdb.Downloader.Properties.Resources.card_data_20;
			this._buttonsMtgjson.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonsMtgjson.Location = new System.Drawing.Point(422, 0);
			this._buttonsMtgjson.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonsMtgjson.Name = "_buttonsMtgjson";
			this._buttonsMtgjson.Size = new System.Drawing.Size(129, 47);
			this._buttonsMtgjson.TabIndex = 2;
			this._buttonsMtgjson.Text = "Update cards\r\nfrom mtgjson.com";
			this._buttonsMtgjson.UseVisualStyleBackColor = true;
			// 
			// _buttonImgMq
			// 
			this._buttonImgMq.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgMq.AutoSize = true;
			this._buttonImgMq.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonImgMq.Image = global::Mtgdb.Downloader.Properties.Resources.card_img_24;
			this._buttonImgMq.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonImgMq.Location = new System.Drawing.Point(422, 50);
			this._buttonImgMq.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonImgMq.Name = "_buttonImgMq";
			this._buttonImgMq.Size = new System.Drawing.Size(129, 47);
			this._buttonImgMq.TabIndex = 4;
			this._buttonImgMq.Text = "Download\r\nzoom images";
			this._buttonImgMq.UseVisualStyleBackColor = true;
			// 
			// _buttonImgLq
			// 
			this._buttonImgLq.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgLq.AutoSize = true;
			this._buttonImgLq.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonImgLq.Image = global::Mtgdb.Downloader.Properties.Resources.card_img_16;
			this._buttonImgLq.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonImgLq.Location = new System.Drawing.Point(290, 50);
			this._buttonImgLq.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonImgLq.Name = "_buttonImgLq";
			this._buttonImgLq.Size = new System.Drawing.Size(129, 47);
			this._buttonImgLq.TabIndex = 3;
			this._buttonImgLq.Text = "Download\r\nsmall images";
			this._buttonImgLq.UseVisualStyleBackColor = true;
			// 
			// _buttonDesktopShortcut
			// 
			this._buttonDesktopShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDesktopShortcut.AutoSize = true;
			this._buttonDesktopShortcut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonDesktopShortcut.Image = global::Mtgdb.Downloader.Properties.Resources.mtg_16;
			this._buttonDesktopShortcut.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonDesktopShortcut.Location = new System.Drawing.Point(0, 50);
			this._buttonDesktopShortcut.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonDesktopShortcut.Name = "_buttonDesktopShortcut";
			this._buttonDesktopShortcut.Size = new System.Drawing.Size(129, 47);
			this._buttonDesktopShortcut.TabIndex = 7;
			this._buttonDesktopShortcut.Text = "Create\r\ndesktop shortcut";
			this._buttonDesktopShortcut.UseVisualStyleBackColor = true;
			this._buttonDesktopShortcut.Click += new System.EventHandler(this.buttonDesktopShortcut);
			// 
			// _buttonEditConfig
			// 
			this._buttonEditConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonEditConfig.AutoSize = true;
			this._buttonEditConfig.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonEditConfig.Image = global::Mtgdb.Downloader.Properties.Resources.properties_16x16;
			this._buttonEditConfig.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonEditConfig.Location = new System.Drawing.Point(0, 0);
			this._buttonEditConfig.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonEditConfig.Name = "_buttonEditConfig";
			this._buttonEditConfig.Size = new System.Drawing.Size(129, 47);
			this._buttonEditConfig.TabIndex = 6;
			this._buttonEditConfig.Text = "Edit\r\nconfiguration";
			this._buttonEditConfig.UseVisualStyleBackColor = true;
			// 
			// _progressBar
			// 
			this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._progressBar.Location = new System.Drawing.Point(3, 250);
			this._progressBar.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(685, 23);
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBar.TabIndex = 8;
			this._progressBar.Visible = false;
			// 
			// _labelProgress
			// 
			this._labelProgress.AutoSize = true;
			this._labelProgress.Location = new System.Drawing.Point(3, 276);
			this._labelProgress.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this._labelProgress.Name = "_labelProgress";
			this._labelProgress.Size = new System.Drawing.Size(145, 13);
			this._labelProgress.TabIndex = 9;
			this._labelProgress.Text = "121 / 10030 files ready";
			this._labelProgress.Visible = false;
			// 
			// _tableLayoutRoot
			// 
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
			this._tableLayoutRoot.Size = new System.Drawing.Size(691, 392);
			this._tableLayoutRoot.TabIndex = 10;
			// 
			// _tableLayoutButtons
			// 
			this._tableLayoutButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
			this._tableLayoutButtons.Controls.Add(this._buttonsMtgjson, 5, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonEditConfig, 0, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonDesktopShortcut, 0, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonNotifications, 2, 1);
			this._tableLayoutButtons.Location = new System.Drawing.Point(3, 292);
			this._tableLayoutButtons.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this._tableLayoutButtons.Name = "_tableLayoutButtons";
			this._tableLayoutButtons.RowCount = 2;
			this._tableLayoutButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutButtons.Size = new System.Drawing.Size(688, 100);
			this._tableLayoutButtons.TabIndex = 11;
			// 
			// _buttonPrices
			// 
			this._buttonPrices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonPrices.AutoSize = true;
			this._buttonPrices.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonPrices.Image = global::Mtgdb.Downloader.Properties.Resources.price_16;
			this._buttonPrices.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonPrices.Location = new System.Drawing.Point(290, 0);
			this._buttonPrices.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonPrices.Name = "_buttonPrices";
			this._buttonPrices.Size = new System.Drawing.Size(129, 47);
			this._buttonPrices.TabIndex = 8;
			this._buttonPrices.Text = "Update\r\ncard prices";
			this._buttonPrices.UseVisualStyleBackColor = true;
			// 
			// _buttonNotifications
			// 
			this._buttonNotifications.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonNotifications.AutoSize = true;
			this._buttonNotifications.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonNotifications.Image = global::Mtgdb.Downloader.Properties.Resources.mailbox_26;
			this._buttonNotifications.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this._buttonNotifications.Location = new System.Drawing.Point(145, 50);
			this._buttonNotifications.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonNotifications.Name = "_buttonNotifications";
			this._buttonNotifications.Size = new System.Drawing.Size(129, 47);
			this._buttonNotifications.TabIndex = 9;
			this._buttonNotifications.Text = "Re-show\r\nnotifications";
			this._buttonNotifications.UseVisualStyleBackColor = true;
			// 
			// UpdateForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(699, 420);
			this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImageClose = global::Mtgdb.Downloader.Properties.Resources.close;
			this.ImageMaximize = global::Mtgdb.Downloader.Properties.Resources.maximize;
			this.ImageMinimize = global::Mtgdb.Downloader.Properties.Resources.minimize;
			this.ImageNormalize = global::Mtgdb.Downloader.Properties.Resources.normalize;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "UpdateForm";
			this.Text = "Mtgdb.Gui updater";
			this._panelClient.ResumeLayout(false);
			this._tableLayoutRoot.ResumeLayout(false);
			this._tableLayoutRoot.PerformLayout();
			this._tableLayoutButtons.ResumeLayout(false);
			this._tableLayoutButtons.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private Button _buttonApp;
		private RichTextBox _textBoxLog;
		private Button _buttonImgArt;
		private Button _buttonsMtgjson;
		private Button _buttonImgMq;
		private Button _buttonImgLq;
		private Button _buttonDesktopShortcut;
		private Button _buttonEditConfig;
		private ProgressBar _progressBar;
		private Label _labelProgress;
		private TableLayoutPanel _tableLayoutRoot;
		private TableLayoutPanel _tableLayoutButtons;
		private Button _buttonPrices;
		private Button _buttonNotifications;
	}
}