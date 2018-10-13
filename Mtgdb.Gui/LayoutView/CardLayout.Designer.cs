using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls;

namespace Mtgdb.Gui
{
	partial class CardLayout
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
		private FieldControl _fieldName;
		private FieldControl _fieldManaCost;
		private FieldControl _fieldCmc;
		private FieldControl _fieldType;
		private FieldControl _fieldSetName;
		private FieldControl _fieldSetCode;
		private FieldControl _fieldText;
		private FieldControl _fieldFlavor;
		private FieldControl _fieldArtist;
		private FieldControl _fieldReleaseDate;
		private FieldControl _fieldRarity;
		private FieldControl _fieldPricingLow;
		private FieldControl _fieldPricingMid;
		private FieldControl _fieldPricingHigh;
		private FieldControl _fieldLoyalty;
		private FieldControl _fieldPower;
		private FieldControl _fieldToughness;
		private FieldControl _fieldRulings;

		private void InitializeComponent()
		{
			this._fieldImage = new Mtgdb.Controls.FieldControl();
			this._fieldName = new Mtgdb.Controls.FieldControl();
			this._fieldManaCost = new Mtgdb.Controls.FieldControl();
			this._fieldCmc = new Mtgdb.Controls.FieldControl();
			this._fieldType = new Mtgdb.Controls.FieldControl();
			this._fieldSetName = new Mtgdb.Controls.FieldControl();
			this._fieldSetCode = new Mtgdb.Controls.FieldControl();
			this._fieldText = new Mtgdb.Controls.FieldControl();
			this._fieldFlavor = new Mtgdb.Controls.FieldControl();
			this._fieldArtist = new Mtgdb.Controls.FieldControl();
			this._fieldReleaseDate = new Mtgdb.Controls.FieldControl();
			this._fieldRarity = new Mtgdb.Controls.FieldControl();
			this._fieldPricingLow = new Mtgdb.Controls.FieldControl();
			this._fieldPricingMid = new Mtgdb.Controls.FieldControl();
			this._fieldPricingHigh = new Mtgdb.Controls.FieldControl();
			this._fieldLoyalty = new Mtgdb.Controls.FieldControl();
			this._fieldPower = new Mtgdb.Controls.FieldControl();
			this._fieldToughness = new Mtgdb.Controls.FieldControl();
			this._fieldRulings = new Mtgdb.Controls.FieldControl();
			this._layout = new System.Windows.Forms.TableLayoutPanel();
			this._layout.SuspendLayout();
			this.SuspendLayout();
			//
			// _fieldImage
			//
			this._fieldImage.DataText = "";
			this._fieldImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldImage.Location = new System.Drawing.Point(0, 0);
			this._fieldImage.Margin = new System.Windows.Forms.Padding(0);
			this._fieldImage.Name = "_fieldImage";
			this._layout.SetRowSpan(this._fieldImage, 8);
			this._fieldImage.Size = new System.Drawing.Size(223, 311);
			this._fieldImage.TabIndex = 0;
			//
			// _fieldName
			//
			this._layout.SetColumnSpan(this._fieldName, 3);
			this._fieldName.DataText = "";
			this._fieldName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldName.Location = new System.Drawing.Point(225, 0);
			this._fieldName.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldName.Name = "_fieldName";
			this._fieldName.Padding = new System.Windows.Forms.Padding(2);
			this._fieldName.Size = new System.Drawing.Size(111, 19);
			this._fieldName.TabIndex = 1;
			//
			// _fieldManaCost
			//
			this._layout.SetColumnSpan(this._fieldManaCost, 3);
			this._fieldManaCost.DataText = "";
			this._fieldManaCost.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldManaCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._fieldManaCost.HorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Right;
			this._fieldManaCost.Location = new System.Drawing.Point(336, 0);
			this._fieldManaCost.Margin = new System.Windows.Forms.Padding(0);
			this._fieldManaCost.Name = "_fieldManaCost";
			this._fieldManaCost.Padding = new System.Windows.Forms.Padding(2);
			this._fieldManaCost.Size = new System.Drawing.Size(110, 19);
			this._fieldManaCost.TabIndex = 2;
			//
			// _fieldCmc
			//
			this._fieldCmc.DataText = "";
			this._fieldCmc.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldCmc.HorizontalAlignment = System.Windows.Forms.HorizontalAlignment.Right;
			this._fieldCmc.Location = new System.Drawing.Point(408, 19);
			this._fieldCmc.Margin = new System.Windows.Forms.Padding(0);
			this._fieldCmc.Name = "_fieldCmc";
			this._fieldCmc.Padding = new System.Windows.Forms.Padding(2);
			this._fieldCmc.Size = new System.Drawing.Size(38, 19);
			this._fieldCmc.TabIndex = 3;
			//
			// _fieldType
			//
			this._layout.SetColumnSpan(this._fieldType, 5);
			this._fieldType.DataText = "";
			this._fieldType.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldType.Location = new System.Drawing.Point(225, 19);
			this._fieldType.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldType.Name = "_fieldType";
			this._fieldType.Padding = new System.Windows.Forms.Padding(2);
			this._fieldType.Size = new System.Drawing.Size(183, 19);
			this._fieldType.TabIndex = 4;
			//
			// _fieldSetName
			//
			this._layout.SetColumnSpan(this._fieldSetName, 5);
			this._fieldSetName.DataText = "";
			this._fieldSetName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldSetName.Location = new System.Drawing.Point(259, 38);
			this._fieldSetName.Margin = new System.Windows.Forms.Padding(0);
			this._fieldSetName.Name = "_fieldSetName";
			this._fieldSetName.Padding = new System.Windows.Forms.Padding(2);
			this._fieldSetName.Size = new System.Drawing.Size(187, 19);
			this._fieldSetName.TabIndex = 5;
			//
			// _fieldSetCode
			//
			this._fieldSetCode.DataText = "";
			this._fieldSetCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldSetCode.Location = new System.Drawing.Point(225, 38);
			this._fieldSetCode.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldSetCode.Name = "_fieldSetCode";
			this._fieldSetCode.Padding = new System.Windows.Forms.Padding(2);
			this._fieldSetCode.Size = new System.Drawing.Size(34, 19);
			this._fieldSetCode.TabIndex = 6;
			//
			// _fieldText
			//
			this._layout.SetColumnSpan(this._fieldText, 6);
			this._fieldText.DataText = "";
			this._fieldText.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldText.Location = new System.Drawing.Point(225, 57);
			this._fieldText.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldText.Name = "_fieldText";
			this._fieldText.Padding = new System.Windows.Forms.Padding(2);
			this._fieldText.Size = new System.Drawing.Size(221, 130);
			this._fieldText.TabIndex = 7;
			//
			// _fieldFlavor
			//
			this._layout.SetColumnSpan(this._fieldFlavor, 6);
			this._fieldFlavor.DataText = "";
			this._fieldFlavor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldFlavor.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._fieldFlavor.Location = new System.Drawing.Point(225, 187);
			this._fieldFlavor.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldFlavor.Name = "_fieldFlavor";
			this._fieldFlavor.Padding = new System.Windows.Forms.Padding(2);
			this._fieldFlavor.Size = new System.Drawing.Size(221, 40);
			this._fieldFlavor.TabIndex = 8;
			//
			// _fieldArtist
			//
			this._layout.SetColumnSpan(this._fieldArtist, 2);
			this._fieldArtist.DataText = "";
			this._fieldArtist.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldArtist.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._fieldArtist.Location = new System.Drawing.Point(225, 227);
			this._fieldArtist.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldArtist.Name = "_fieldArtist";
			this._fieldArtist.Padding = new System.Windows.Forms.Padding(2);
			this._fieldArtist.Size = new System.Drawing.Size(75, 19);
			this._fieldArtist.TabIndex = 9;
			//
			// _fieldReleaseDate
			//
			this._layout.SetColumnSpan(this._fieldReleaseDate, 2);
			this._fieldReleaseDate.DataText = "";
			this._fieldReleaseDate.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldReleaseDate.Location = new System.Drawing.Point(300, 227);
			this._fieldReleaseDate.Margin = new System.Windows.Forms.Padding(0);
			this._fieldReleaseDate.Name = "_fieldReleaseDate";
			this._fieldReleaseDate.Padding = new System.Windows.Forms.Padding(2);
			this._fieldReleaseDate.Size = new System.Drawing.Size(72, 19);
			this._fieldReleaseDate.TabIndex = 10;
			//
			// _fieldRarity
			//
			this._layout.SetColumnSpan(this._fieldRarity, 2);
			this._fieldRarity.DataText = "";
			this._fieldRarity.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldRarity.Location = new System.Drawing.Point(372, 227);
			this._fieldRarity.Margin = new System.Windows.Forms.Padding(0);
			this._fieldRarity.Name = "_fieldRarity";
			this._fieldRarity.Padding = new System.Windows.Forms.Padding(2);
			this._fieldRarity.Size = new System.Drawing.Size(74, 19);
			this._fieldRarity.TabIndex = 11;
			//
			// _fieldPricingLow
			//
			this._fieldPricingLow.DataText = "";
			this._fieldPricingLow.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldPricingLow.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._fieldPricingLow.Location = new System.Drawing.Point(225, 246);
			this._fieldPricingLow.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldPricingLow.Name = "_fieldPricingLow";
			this._fieldPricingLow.Padding = new System.Windows.Forms.Padding(2);
			this._fieldPricingLow.Size = new System.Drawing.Size(34, 19);
			this._fieldPricingLow.TabIndex = 12;
			//
			// _fieldPricingMid
			//
			this._fieldPricingMid.DataText = "";
			this._fieldPricingMid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldPricingMid.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._fieldPricingMid.Location = new System.Drawing.Point(259, 246);
			this._fieldPricingMid.Margin = new System.Windows.Forms.Padding(0);
			this._fieldPricingMid.Name = "_fieldPricingMid";
			this._fieldPricingMid.Padding = new System.Windows.Forms.Padding(2);
			this._fieldPricingMid.Size = new System.Drawing.Size(41, 19);
			this._fieldPricingMid.TabIndex = 13;
			//
			// _fieldPricingHigh
			//
			this._fieldPricingHigh.DataText = "";
			this._fieldPricingHigh.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldPricingHigh.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._fieldPricingHigh.Location = new System.Drawing.Point(300, 246);
			this._fieldPricingHigh.Margin = new System.Windows.Forms.Padding(0);
			this._fieldPricingHigh.Name = "_fieldPricingHigh";
			this._fieldPricingHigh.Padding = new System.Windows.Forms.Padding(2);
			this._fieldPricingHigh.Size = new System.Drawing.Size(36, 19);
			this._fieldPricingHigh.TabIndex = 14;
			//
			// _fieldLoyalty
			//
			this._fieldLoyalty.DataText = "";
			this._fieldLoyalty.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldLoyalty.Location = new System.Drawing.Point(336, 246);
			this._fieldLoyalty.Margin = new System.Windows.Forms.Padding(0);
			this._fieldLoyalty.Name = "_fieldLoyalty";
			this._fieldLoyalty.Padding = new System.Windows.Forms.Padding(2);
			this._fieldLoyalty.Size = new System.Drawing.Size(36, 19);
			this._fieldLoyalty.TabIndex = 15;
			//
			// _fieldPower
			//
			this._fieldPower.DataText = "";
			this._fieldPower.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldPower.Location = new System.Drawing.Point(372, 246);
			this._fieldPower.Margin = new System.Windows.Forms.Padding(0);
			this._fieldPower.Name = "_fieldPower";
			this._fieldPower.Padding = new System.Windows.Forms.Padding(2);
			this._fieldPower.Size = new System.Drawing.Size(36, 19);
			this._fieldPower.TabIndex = 16;
			//
			// _fieldToughness
			//
			this._fieldToughness.DataText = "";
			this._fieldToughness.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldToughness.Location = new System.Drawing.Point(408, 246);
			this._fieldToughness.Margin = new System.Windows.Forms.Padding(0);
			this._fieldToughness.Name = "_fieldToughness";
			this._fieldToughness.Padding = new System.Windows.Forms.Padding(2);
			this._fieldToughness.Size = new System.Drawing.Size(38, 19);
			this._fieldToughness.TabIndex = 17;
			//
			// _fieldRulings
			//
			this._layout.SetColumnSpan(this._fieldRulings, 6);
			this._fieldRulings.DataText = "";
			this._fieldRulings.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fieldRulings.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this._fieldRulings.Location = new System.Drawing.Point(225, 265);
			this._fieldRulings.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
			this._fieldRulings.Name = "_fieldRulings";
			this._fieldRulings.Padding = new System.Windows.Forms.Padding(2);
			this._fieldRulings.Size = new System.Drawing.Size(221, 46);
			this._fieldRulings.TabIndex = 18;
			//
			// _layout
			//
			this._layout.BackColor = System.Drawing.Color.Transparent;
			this._layout.ColumnCount = 7;
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.139535F));
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.302326F));
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.139535F));
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.139535F));
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.139535F));
			this._layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.139535F));
			this._layout.Controls.Add(this._fieldImage, 0, 0);
			this._layout.Controls.Add(this._fieldRulings, 1, 7);
			this._layout.Controls.Add(this._fieldName, 1, 0);
			this._layout.Controls.Add(this._fieldToughness, 6, 6);
			this._layout.Controls.Add(this._fieldManaCost, 4, 0);
			this._layout.Controls.Add(this._fieldPower, 5, 6);
			this._layout.Controls.Add(this._fieldCmc, 6, 1);
			this._layout.Controls.Add(this._fieldLoyalty, 4, 6);
			this._layout.Controls.Add(this._fieldType, 1, 1);
			this._layout.Controls.Add(this._fieldPricingHigh, 3, 6);
			this._layout.Controls.Add(this._fieldSetCode, 1, 2);
			this._layout.Controls.Add(this._fieldPricingMid, 2, 6);
			this._layout.Controls.Add(this._fieldSetName, 2, 2);
			this._layout.Controls.Add(this._fieldPricingLow, 1, 6);
			this._layout.Controls.Add(this._fieldText, 1, 3);
			this._layout.Controls.Add(this._fieldRarity, 5, 5);
			this._layout.Controls.Add(this._fieldFlavor, 1, 4);
			this._layout.Controls.Add(this._fieldReleaseDate, 3, 5);
			this._layout.Controls.Add(this._fieldArtist, 1, 5);
			this._layout.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layout.Location = new System.Drawing.Point(0, 0);
			this._layout.Margin = new System.Windows.Forms.Padding(0);
			this._layout.Name = "_layout";
			this._layout.RowCount = 8;
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.430868F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.430868F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.430868F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.12218F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.86174F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.430868F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.430868F));
			this._layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.86174F));
			this._layout.Size = new System.Drawing.Size(446, 311);
			this._layout.TabIndex = 19;
			//
			// CardLayout
			//
			this.Controls.Add(this._layout);
			this.Name = "CardLayout";
			this.Size = new System.Drawing.Size(446, 311);
			this._layout.ResumeLayout(false);
			this.ResumeLayout(false);
		}

		#endregion

		private TableLayoutPanel _layout;
	}
}
