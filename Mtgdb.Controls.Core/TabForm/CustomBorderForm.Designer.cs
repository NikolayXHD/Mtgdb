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
			this._panelCaption = new Mtgdb.Controls.BorderedPanel();
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
			this._panelClient.TabIndex = 1;
			//
			// _panelCaption
			//
			this._panelCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
			this._panelCaption.Location = new System.Drawing.Point(8, 8);
			this._panelCaption.Margin = new System.Windows.Forms.Padding(0);
			this._panelCaption.Name = "_panelCaption";
			this._panelCaption.PaintBackground = false;
			this._panelCaption.Size = new System.Drawing.Size(784, 20);
			this._panelCaption.TabIndex = 0;
			this._panelCaption.VisibleBorders = System.Windows.Forms.AnchorStyles.Bottom;
			//
			// CustomBorderForm
			//
			this.BackColor = System.Drawing.Color.Magenta;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.Controls.Add(this._panelCaption);
			this.Controls.Add(this._panelClient);
			this.Name = "CustomBorderForm";
			this.TransparencyKey = System.Drawing.Color.Magenta;
			this.ResumeLayout(false);

		}

		#endregion

		protected System.Windows.Forms.Panel _panelClient;
		protected Mtgdb.Controls.BorderedPanel _panelCaption;
	}
}
