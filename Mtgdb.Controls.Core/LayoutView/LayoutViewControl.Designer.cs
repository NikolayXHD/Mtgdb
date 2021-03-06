﻿namespace Mtgdb.Controls
{
	partial class LayoutViewControl
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

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Scrollbar = new CustomScrollbar.Scrollbar();
			this.SuspendLayout();
			// 
			// Scrollbar
			// 
			this.Scrollbar.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right)));
			this.Scrollbar.LargeChange = 10;
			this.Scrollbar.Location = new System.Drawing.Point(133, 0);
			this.Scrollbar.Maximum = 100;
			this.Scrollbar.Minimum = 0;
			this.Scrollbar.Name = "Scrollbar";
			this.Scrollbar.PenWidth = 2;
			this.Scrollbar.Size = new System.Drawing.Size(17, 150);
			this.Scrollbar.SmallChange = 1;
			this.Scrollbar.TabIndex = 0;
			this.Scrollbar.Value = 0;
			// 
			// LayoutViewControl
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.Scrollbar);
			this.DoubleBuffered = true;
			this.Name = "LayoutViewControl";
			this.ResumeLayout(false);
		}

		internal CustomScrollbar.Scrollbar Scrollbar;

		#endregion
	}
}
