using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	public class DropDown : DropDownBase
	{
		public DropDown()
		{
			Padding = DefaultPadding;

			TextImageRelation = TextImageRelation.TextBeforeImage;
			TextAlign = StringAlignment.Near;

			var dropDownImg = Resources.drop_down_48.ScaleBy(0.5f);
			ButtonImages = new ButtonImages(dropDownImg, dropDownImg);
		}

		protected override void OnSelectedIndexChanged()
		{
			updateSelectedText();
			base.OnSelectedIndexChanged();
		}

		protected override Size MeasureMenuItem(ButtonBase menuItem)
		{
			var size = base.MeasureMenuItem(menuItem);

			if (size.Width < Width)
				return new Size(Width, size.Height);

			return size;
		}

		private void updateSelectedText()
		{
			if (SelectedIndex == -1)
				Text = EmptySelectionText;
			else if (SelectedIndex < Menu.Controls.Count)
				Text = Menu.Controls[SelectedIndex].Text;
			else
				throw new IndexOutOfRangeException();
		}


		private readonly string _emptySelectionText = string.Empty;
		public string EmptySelectionText
		{
			get => _emptySelectionText;
			set
			{
				if (_emptySelectionText == value)
					return;

				if (SelectedIndex == -1)
					updateSelectedText();
			}
		}



		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Bitmap Image
		{
			get => base.Image;
			set => base.Image = value;
		}

		protected override Padding DefaultPadding =>
			new Padding(4, 4, 0, 4);

		[DefaultValue(typeof(TextImageRelation), "TextBeforeImage")]
		public override TextImageRelation TextImageRelation
		{
			get => base.TextImageRelation;
			set => base.TextImageRelation = value;
		}

		[DefaultValue(typeof(StringAlignment), "Near")]
		public override StringAlignment TextAlign
		{
			get => base.TextAlign;
			set => base.TextAlign = value;
		}
	}
}