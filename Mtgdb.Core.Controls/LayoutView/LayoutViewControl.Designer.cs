namespace Mtgdb.Controls
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
			this._scrollBar = new System.Windows.Forms.VScrollBar();
			this.SuspendLayout();
			// 
			// _scrollBar
			// 
			this._scrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._scrollBar.Location = new System.Drawing.Point(133, 0);
			this._scrollBar.Name = "_scrollBar";
			this._scrollBar.Size = new System.Drawing.Size(17, 150);
			this._scrollBar.TabIndex = 0;
			// 
			// LayoutViewControl
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this._scrollBar);
			this.DoubleBuffered = true;
			this.Name = "LayoutViewControl";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.VScrollBar _scrollBar;
	}
}
