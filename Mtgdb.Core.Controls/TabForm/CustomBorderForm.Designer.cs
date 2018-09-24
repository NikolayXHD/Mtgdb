namespace Mtgdb.Controls
{
	partial class CustomBorderForm
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
			this._panelClient = new System.Windows.Forms.Panel();
			this._panelHeader = new Mtgdb.Controls.BorderedPanel();
			this.SuspendLayout();
			// 
			// _panelClient
			// 
			this._panelClient.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelClient.Location = new System.Drawing.Point(8, 28);
			this._panelClient.Margin = new System.Windows.Forms.Padding(0);
			this._panelClient.Name = "_panelClient";
			this._panelClient.Size = new System.Drawing.Size(784, 564);
			this._panelClient.TabIndex = 3;
			// 
			// _panelHeader
			// 
			this._panelHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelHeader.Location = new System.Drawing.Point(8, 8);
			this._panelHeader.Margin = new System.Windows.Forms.Padding(0);
			this._panelHeader.Name = "_panelHeader";
			this._panelHeader.PaintBackground = false;
			this._panelHeader.Size = new System.Drawing.Size(784, 20);
			this._panelHeader.TabIndex = 4;
			this._panelHeader.VisibleBorders = System.Windows.Forms.AnchorStyles.Bottom;
			// 
			// CustomBorderForm
			// 
			this.BackColor = System.Drawing.Color.Magenta;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.ControlBox = false;
			this.Controls.Add(this._panelHeader);
			this.Controls.Add(this._panelClient);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "CustomBorderForm";
			this.TransparencyKey = System.Drawing.Color.Magenta;
			this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Panel _panelClient;
		protected Mtgdb.Controls.BorderedPanel _panelHeader;
	}
}