using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	partial class DeckLayout
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

		private FieldControl _fieldImage;

		private void InitializeComponent()
		{
			Mtgdb.Controls.SearchOptions searchOptions1 = new Mtgdb.Controls.SearchOptions();
			Mtgdb.Controls.ButtonOptions buttonOptions1 = new Mtgdb.Controls.ButtonOptions();
			this._fieldImage = new Mtgdb.Controls.FieldControl();
			this.SuspendLayout();
			//
			// _fieldImage
			//
			this._fieldImage.DataText = "";
			this._fieldImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldImage.Location = new System.Drawing.Point(0, 0);
			this._fieldImage.Margin = new System.Windows.Forms.Padding(0);
			this._fieldImage.Name = "_fieldImage";
			searchOptions1.Button = buttonOptions1;
			this._fieldImage.SearchOptions = searchOptions1;
			this._fieldImage.Size = new System.Drawing.Size(223, 311);
			this._fieldImage.TabIndex = 0;
			//
			// DeckLayout
			//
			this.Controls.Add(this._fieldImage);
			this.Name = "DeckLayout";
			this.Size = new System.Drawing.Size(223, 311);
			this.ResumeLayout(false);
		}

		#endregion
	}
}
