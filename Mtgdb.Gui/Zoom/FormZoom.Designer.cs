namespace Mtgdb.Gui
{
	sealed partial class FormZoom
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
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._showInExplorerButton = new System.Windows.Forms.ToolStripMenuItem();
			this._openFileButton = new System.Windows.Forms.ToolStripMenuItem();
			this._showOtherSetsButton = new System.Windows.Forms.ToolStripMenuItem();
			this._showDuplicatesButton = new System.Windows.Forms.ToolStripMenuItem();
			this._hintButton = new System.Windows.Forms.ToolStripMenuItem();
			this._pictureBox = new System.Windows.Forms.PictureBox();
			this._contextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// _contextMenu
			// 
			this._contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._showInExplorerButton,
            this._openFileButton,
            this._showOtherSetsButton,
            this._showDuplicatesButton,
            this._hintButton});
			this._contextMenu.Name = "_contextMenu";
			this._contextMenu.ShowCheckMargin = true;
			this._contextMenu.Size = new System.Drawing.Size(393, 227);
			// 
			// _showInExplorerButton
			// 
			this._showInExplorerButton.Image = global::Mtgdb.Gui.Properties.Resources.open_32;
			this._showInExplorerButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._showInExplorerButton.Name = "_showInExplorerButton";
			this._showInExplorerButton.Size = new System.Drawing.Size(392, 38);
			this._showInExplorerButton.Text = "Show in explorer";
			this._showInExplorerButton.ToolTipText = "Opens the image directory in explorer with current image selected.\r\nSome times se" +
    "lecting fails. This is Microsoft\'s bug, not mine :)";
			this._showInExplorerButton.Click += new System.EventHandler(this.openInExplorerClick);
			// 
			// _openFileButton
			// 
			this._openFileButton.Image = global::Mtgdb.Gui.Properties.Resources.image_file_48;
			this._openFileButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._openFileButton.Name = "_openFileButton";
			this._openFileButton.Size = new System.Drawing.Size(392, 38);
			this._openFileButton.Text = "Open file";
			this._openFileButton.ToolTipText = "Open the image file by your default image viewer";
			this._openFileButton.Click += new System.EventHandler(this.openFileClick);
			// 
			// _showOtherSetsButton
			// 
			this._showOtherSetsButton.Checked = true;
			this._showOtherSetsButton.CheckOnClick = true;
			this._showOtherSetsButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this._showOtherSetsButton.Image = global::Mtgdb.Gui.Properties.Resources.clone_32;
			this._showOtherSetsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._showOtherSetsButton.Name = "_showOtherSetsButton";
			this._showOtherSetsButton.Size = new System.Drawing.Size(392, 38);
			this._showOtherSetsButton.Text = "Show variants from other sets";
			this._showOtherSetsButton.ToolTipText = "When checked you will be able to cycle between this card\'s images from other sets" +
    " too.";
			// 
			// _showDuplicatesButton
			// 
			this._showDuplicatesButton.Checked = true;
			this._showDuplicatesButton.CheckOnClick = true;
			this._showDuplicatesButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this._showDuplicatesButton.Image = global::Mtgdb.Gui.Properties.Resources.clone_32;
			this._showDuplicatesButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._showDuplicatesButton.Name = "_showDuplicatesButton";
			this._showDuplicatesButton.Size = new System.Drawing.Size(392, 38);
			this._showDuplicatesButton.Text = "Show variants from same set";
			this._showDuplicatesButton.ToolTipText = "Some cards have different images event within the same set.\r\nThis mostly relates " +
    "to basic lands.\r\nWhen checked you will be able to cycle through all card image v" +
    "aritants within a given set.";
			// 
			// _hintButton
			// 
			this._hintButton.Enabled = false;
			this._hintButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this._hintButton.Name = "_hintButton";
			this._hintButton.Size = new System.Drawing.Size(392, 38);
			this._hintButton.Text = "* Scroll to cycle between images";
			// 
			// _pictureBox
			// 
			this._pictureBox.ContextMenuStrip = this._contextMenu;
			this._pictureBox.Location = new System.Drawing.Point(0, 0);
			this._pictureBox.Margin = new System.Windows.Forms.Padding(0);
			this._pictureBox.Name = "_pictureBox";
			this._pictureBox.Size = new System.Drawing.Size(640, 917);
			this._pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this._pictureBox.TabIndex = 0;
			this._pictureBox.TabStop = false;
			// 
			// FormZoom
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(640, 917);
			this.Controls.Add(this._pictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.Name = "FormZoom";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this._contextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox _pictureBox;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
		private System.Windows.Forms.ToolStripMenuItem _showInExplorerButton;
		private System.Windows.Forms.ToolStripMenuItem _openFileButton;
		private System.Windows.Forms.ToolStripMenuItem _showDuplicatesButton;
		private System.Windows.Forms.ToolStripMenuItem _showOtherSetsButton;
		private System.Windows.Forms.ToolStripMenuItem _hintButton;
	}
}