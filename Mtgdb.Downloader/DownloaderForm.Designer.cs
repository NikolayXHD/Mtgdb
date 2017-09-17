﻿using System.ComponentModel;
using System.Windows.Forms;

namespace Mtgdb.Downloader
{
	sealed partial class DownloaderForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloaderForm));
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
			this._panelClient.SuspendLayout();
			this._tableLayoutRoot.SuspendLayout();
			this._tableLayoutButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// _panelClient
			// 
			this._panelClient.Controls.Add(this._tableLayoutRoot);
			this._panelClient.Size = new System.Drawing.Size(712, 385);
			// 
			// _panelHeader
			// 
			this._panelHeader.Size = new System.Drawing.Size(619, 20);
			// 
			// _buttonApp
			// 
			this._buttonApp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonApp.AutoSize = true;
			this._buttonApp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonApp.Location = new System.Drawing.Point(590, 45);
			this._buttonApp.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonApp.Name = "_buttonApp";
			this._buttonApp.Size = new System.Drawing.Size(116, 42);
			this._buttonApp.TabIndex = 1;
			this._buttonApp.Text = "Check Mtgdb.Gui updates\r\n";
			this._buttonApp.UseVisualStyleBackColor = true;
			// 
			// _textBoxLog
			// 
			this._textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this._textBoxLog.Location = new System.Drawing.Point(3, 3);
			this._textBoxLog.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._textBoxLog.Name = "_textBoxLog";
			this._textBoxLog.ReadOnly = true;
			this._textBoxLog.Size = new System.Drawing.Size(706, 247);
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
			this._buttonImgArt.Location = new System.Drawing.Point(236, 0);
			this._buttonImgArt.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonImgArt.Name = "_buttonImgArt";
			this._buttonImgArt.Size = new System.Drawing.Size(115, 42);
			this._buttonImgArt.TabIndex = 5;
			this._buttonImgArt.Text = "Download artwork images";
			this._buttonImgArt.UseVisualStyleBackColor = true;
			// 
			// _buttonsMtgjson
			// 
			this._buttonsMtgjson.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonsMtgjson.AutoSize = true;
			this._buttonsMtgjson.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonsMtgjson.Location = new System.Drawing.Point(590, 0);
			this._buttonsMtgjson.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonsMtgjson.Name = "_buttonsMtgjson";
			this._buttonsMtgjson.Size = new System.Drawing.Size(116, 42);
			this._buttonsMtgjson.TabIndex = 2;
			this._buttonsMtgjson.Text = "Download cards from mtgjson.com";
			this._buttonsMtgjson.UseVisualStyleBackColor = true;
			// 
			// _buttonImgMq
			// 
			this._buttonImgMq.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgMq.AutoSize = true;
			this._buttonImgMq.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonImgMq.Location = new System.Drawing.Point(354, 45);
			this._buttonImgMq.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonImgMq.Name = "_buttonImgMq";
			this._buttonImgMq.Size = new System.Drawing.Size(115, 42);
			this._buttonImgMq.TabIndex = 4;
			this._buttonImgMq.Text = "Download average quality images";
			this._buttonImgMq.UseVisualStyleBackColor = true;
			// 
			// _buttonImgLq
			// 
			this._buttonImgLq.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonImgLq.AutoSize = true;
			this._buttonImgLq.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonImgLq.Location = new System.Drawing.Point(236, 45);
			this._buttonImgLq.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonImgLq.Name = "_buttonImgLq";
			this._buttonImgLq.Size = new System.Drawing.Size(115, 42);
			this._buttonImgLq.TabIndex = 3;
			this._buttonImgLq.Text = "Donwload low quality images";
			this._buttonImgLq.UseVisualStyleBackColor = true;
			// 
			// _buttonDesktopShortcut
			// 
			this._buttonDesktopShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonDesktopShortcut.AutoSize = true;
			this._buttonDesktopShortcut.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._buttonDesktopShortcut.Location = new System.Drawing.Point(0, 45);
			this._buttonDesktopShortcut.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonDesktopShortcut.Name = "_buttonDesktopShortcut";
			this._buttonDesktopShortcut.Size = new System.Drawing.Size(115, 42);
			this._buttonDesktopShortcut.TabIndex = 7;
			this._buttonDesktopShortcut.Text = "Create desktop shortcut";
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
			this._buttonEditConfig.Location = new System.Drawing.Point(0, 0);
			this._buttonEditConfig.Margin = new System.Windows.Forms.Padding(0, 0, 3, 3);
			this._buttonEditConfig.Name = "_buttonEditConfig";
			this._buttonEditConfig.Size = new System.Drawing.Size(115, 42);
			this._buttonEditConfig.TabIndex = 6;
			this._buttonEditConfig.Text = "Edit configuration";
			this._buttonEditConfig.UseVisualStyleBackColor = true;
			// 
			// _progressBar
			// 
			this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._progressBar.Location = new System.Drawing.Point(3, 253);
			this._progressBar.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(706, 23);
			this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this._progressBar.TabIndex = 8;
			this._progressBar.Visible = false;
			// 
			// _labelProgress
			// 
			this._labelProgress.AutoSize = true;
			this._labelProgress.Location = new System.Drawing.Point(3, 279);
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
			this._tableLayoutRoot.Controls.Add(this._tableLayoutButtons, 0, 3);
			this._tableLayoutRoot.Controls.Add(this._labelProgress, 0, 2);
			this._tableLayoutRoot.Controls.Add(this._progressBar, 0, 1);
			this._tableLayoutRoot.Controls.Add(this._textBoxLog, 0, 0);
			this._tableLayoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutRoot.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutRoot.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutRoot.Name = "_tableLayoutRoot";
			this._tableLayoutRoot.RowCount = 4;
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutRoot.Size = new System.Drawing.Size(712, 385);
			this._tableLayoutRoot.TabIndex = 10;
			// 
			// _tableLayoutButtons
			// 
			this._tableLayoutButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutButtons.ColumnCount = 6;
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this._tableLayoutButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
			this._tableLayoutButtons.Controls.Add(this._buttonEditConfig, 0, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonApp, 5, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonsMtgjson, 5, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonImgMq, 3, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonDesktopShortcut, 0, 1);
			this._tableLayoutButtons.Controls.Add(this._buttonImgArt, 2, 0);
			this._tableLayoutButtons.Controls.Add(this._buttonImgLq, 2, 1);
			this._tableLayoutButtons.Location = new System.Drawing.Point(3, 295);
			this._tableLayoutButtons.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
			this._tableLayoutButtons.Name = "_tableLayoutButtons";
			this._tableLayoutButtons.RowCount = 2;
			this._tableLayoutButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._tableLayoutButtons.Size = new System.Drawing.Size(709, 90);
			this._tableLayoutButtons.TabIndex = 11;
			// 
			// DownloaderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(720, 413);
			this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImageClose = global::Mtgdb.Downloader.Properties.Resources.close;
			this.ImageMaximize = global::Mtgdb.Downloader.Properties.Resources.maximize;
			this.ImageMinimize = global::Mtgdb.Downloader.Properties.Resources.minimize;
			this.ImageNormalize = global::Mtgdb.Downloader.Properties.Resources.normalize;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "DownloaderForm";
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
	}
}