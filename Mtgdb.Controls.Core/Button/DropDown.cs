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
			TextPosition = StringAlignment.Near;
			ImagePosition = StringAlignment.Far;

			ImageUnchecked = Resources.drop_down_48;
			ImageScale = 0.5f;
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

		protected override void PaintDropDownIndication(Graphics g)
		{
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
			new Padding(2, 0, 0, 0);

		[DefaultValue(typeof(TextImageRelation), "TextBeforeImage")]
		public override TextImageRelation TextImageRelation
		{
			get => base.TextImageRelation;
			set => base.TextImageRelation = value;
		}

		[DefaultValue(typeof(StringAlignment), "Near")]
		public override StringAlignment TextPosition
		{
			get => base.TextPosition;
			set => base.TextPosition = value;
		}

		[DefaultValue(typeof(StringAlignment), "Far")]
		public override StringAlignment ImagePosition
		{
			get => base.ImagePosition;
			set => base.ImagePosition = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Bitmap ImageUnchecked
		{
			get => base.ImageUnchecked;
			set => base.ImageUnchecked = value;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Bitmap ImageChecked
		{
			get => base.ImageChecked;
			set => base.ImageChecked = value;
		}
	}
}