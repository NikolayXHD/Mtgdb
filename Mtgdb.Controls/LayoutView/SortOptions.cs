using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class SortOptions
	{
		[Category("Settings")]
		[DefaultValue(false)]
		public bool Allow { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap AscIcon { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap DescIcon { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap IconHovered { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap AscIconHovered { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap DescIconHovered { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap Icon { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(ContentAlignment), "TopRight")]
		public ContentAlignment ButtonAlignment { get; set; } = ContentAlignment.TopRight;

		[Category("Settings")]
		[DefaultValue(typeof(Size), "2, 2")]
		public Size ButtonMargin { get; set; } = new Size(2, 2);


		public Rectangle GetButtonBounds(FieldControl field, LayoutControl layout)
		{
			if (!field.AllowSort || !field.IsSortVisible)
				return Rectangle.Empty;

			var fieldBounds = field.Bounds;
			fieldBounds.Offset(layout.Location);

			var imageBounds = new Rectangle(fieldBounds.Location, Icon.Size);

			if (ContentAlignmentRanges.AnyCenter.HasFlag(ButtonAlignment))
				imageBounds.Offset((fieldBounds.Width - imageBounds.Width) / 2, 0);
			else if (ContentAlignmentRanges.AnyRight.HasFlag(ButtonAlignment))
				imageBounds.Offset(fieldBounds.Width - imageBounds.Width - ButtonMargin.Width, 0);
			else if (ContentAlignmentRanges.AnyLeft.HasFlag(ButtonAlignment))
				imageBounds.Offset(ButtonMargin.Width, 0);

			if (ContentAlignmentRanges.AnyMiddle.HasFlag(ButtonAlignment))
				imageBounds.Offset(0, (fieldBounds.Height - imageBounds.Height) / 2);
			else if (ContentAlignmentRanges.AnyBottom.HasFlag(ButtonAlignment))
				imageBounds.Offset(0, fieldBounds.Height - imageBounds.Height - ButtonMargin.Height);
			else if (ContentAlignmentRanges.AnyTop.HasFlag(ButtonAlignment))
				imageBounds.Offset(0, ButtonMargin.Height);

			return imageBounds;
		}

		public Bitmap GetIcon(FieldControl field)
		{
			if (field.IsSortHotTracked)
			{
				switch (field.SortOrder)
				{
					case SortOrder.None:
						return IconHovered;
					case SortOrder.Ascending:
						return AscIconHovered;
					case SortOrder.Descending:
						return DescIconHovered;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			switch (field.SortOrder)
			{
				case SortOrder.None:
					return Icon;
				case SortOrder.Ascending:
					return AscIcon;
				case SortOrder.Descending:
					return DescIcon;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}