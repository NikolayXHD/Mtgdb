using System.ComponentModel;
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
			this._buttonEditConfig = new System.Windows.Forms.Button();
			this._buttonDesktopShortcut = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _buttonApp
			// 
			this._buttonApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonApp.Location = new System.Drawing.Point(597, 367);
			this._buttonApp.Name = "_buttonApp";
			this._buttonApp.Size = new System.Drawing.Size(111, 38);
			this._buttonApp.TabIndex = 1;
			this._buttonApp.Text = "Check Mtgdb.Gui updates\r\n";
			this._buttonApp.UseVisualStyleBackColor = true;
			// 
			// _textBoxLog
			// 
			this._textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._textBoxLog.Location = new System.Drawing.Point(12, 12);
			this._textBoxLog.Name = "_textBoxLog";
			this._textBoxLog.ReadOnly = true;
			this._textBoxLog.Size = new System.Drawing.Size(696, 305);
			this._textBoxLog.TabIndex = 0;
			this._textBoxLog.Text = "";
			// 
			// _buttonImgArt
			// 
			this._buttonImgArt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonImgArt.Location = new System.Drawing.Point(246, 323);
			this._buttonImgArt.Name = "_buttonImgArt";
			this._buttonImgArt.Size = new System.Drawing.Size(111, 38);
			this._buttonImgArt.TabIndex = 5;
			this._buttonImgArt.Text = "Download artwork images";
			this._buttonImgArt.UseVisualStyleBackColor = true;
			// 
			// _buttonsMtgjson
			// 
			this._buttonsMtgjson.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonsMtgjson.Location = new System.Drawing.Point(597, 323);
			this._buttonsMtgjson.Name = "_buttonsMtgjson";
			this._buttonsMtgjson.Size = new System.Drawing.Size(111, 38);
			this._buttonsMtgjson.TabIndex = 2;
			this._buttonsMtgjson.Text = "Download cards from mtgjson.com";
			this._buttonsMtgjson.UseVisualStyleBackColor = true;
			// 
			// _buttonImgMq
			// 
			this._buttonImgMq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonImgMq.Location = new System.Drawing.Point(363, 367);
			this._buttonImgMq.Name = "_buttonImgMq";
			this._buttonImgMq.Size = new System.Drawing.Size(111, 38);
			this._buttonImgMq.TabIndex = 4;
			this._buttonImgMq.Text = "Download average quality images";
			this._buttonImgMq.UseVisualStyleBackColor = true;
			// 
			// _buttonImgLq
			// 
			this._buttonImgLq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonImgLq.Location = new System.Drawing.Point(246, 367);
			this._buttonImgLq.Name = "_buttonImgLq";
			this._buttonImgLq.Size = new System.Drawing.Size(111, 38);
			this._buttonImgLq.TabIndex = 3;
			this._buttonImgLq.Text = "Donwload low quality images";
			this._buttonImgLq.UseVisualStyleBackColor = true;
			// 
			// _buttonEditConfig
			// 
			this._buttonEditConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonEditConfig.Location = new System.Drawing.Point(12, 323);
			this._buttonEditConfig.Name = "_buttonEditConfig";
			this._buttonEditConfig.Size = new System.Drawing.Size(111, 38);
			this._buttonEditConfig.TabIndex = 6;
			this._buttonEditConfig.Text = "Edit configuration";
			this._buttonEditConfig.UseVisualStyleBackColor = true;
			// 
			// _buttonDesktopShortcut
			// 
			this._buttonDesktopShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonDesktopShortcut.Location = new System.Drawing.Point(12, 367);
			this._buttonDesktopShortcut.Name = "_buttonDesktopShortcut";
			this._buttonDesktopShortcut.Size = new System.Drawing.Size(111, 38);
			this._buttonDesktopShortcut.TabIndex = 7;
			this._buttonDesktopShortcut.Text = "Create desktop shortcut";
			this._buttonDesktopShortcut.UseVisualStyleBackColor = true;
			this._buttonDesktopShortcut.Click += new System.EventHandler(this.buttonDesktopShortcut);
			// 
			// DownloaderForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(720, 417);
			this.Controls.Add(this._buttonDesktopShortcut);
			this.Controls.Add(this._buttonEditConfig);
			this.Controls.Add(this._buttonImgMq);
			this.Controls.Add(this._buttonsMtgjson);
			this.Controls.Add(this._buttonImgArt);
			this.Controls.Add(this._buttonApp);
			this.Controls.Add(this._textBoxLog);
			this.Controls.Add(this._buttonImgLq);
			this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DownloaderForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Mtgdb.Gui updater";
			this.ResumeLayout(false);

		}

		#endregion
		private Button _buttonApp;
		private RichTextBox _textBoxLog;
		private Button _buttonImgArt;
		private Button _buttonsMtgjson;
		private Button _buttonImgMq;
		private Button _buttonImgLq;
		private Button _buttonEditConfig;
		private Button _buttonDesktopShortcut;
	}
}