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
		public bool AllowSort { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap SortAscIcon { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap SortDescIcon { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap SortIconHovered { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap SortAscIconHovered { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap SortDescIconHovered { get; set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap SortIcon { get; set; }

		[Category("Settings")]
		[DefaultValue(typeof(ContentAlignment), "TopRight")]
		public ContentAlignment SortButtonAlignment { get; set; } = ContentAlignment.TopRight;

		[Category("Settings")]
		[DefaultValue(typeof(Size), "2, 2")]
		public Size SortButtonMargin { get; set; } = new Size(2, 2);


		public Rectangle GetSortButtonBounds(FieldControl field, LayoutControl layout)
		{
			if (!field.AllowSort || !field.IsSortVisible)
				return Rectangle.Empty;

			var fieldBounds = field.Bounds;
			fieldBounds.Offset(layout.Location);

			var imageBounds = new Rectangle(fieldBounds.Location, SortIcon.Size);

			if (AnyCenter.HasFlag(SortButtonAlignment))
				imageBounds.Offset((fieldBounds.Width - imageBounds.Width) / 2, 0);
			else if (AnyRight.HasFlag(SortButtonAlignment))
				imageBounds.Offset(fieldBounds.Width - imageBounds.Width - SortButtonMargin.Width, 0);
			else if (AnyLeft.HasFlag(SortButtonAlignment))
				imageBounds.Offset(SortButtonMargin.Width, 0);

			if (AnyMiddle.HasFlag(SortButtonAlignment))
				imageBounds.Offset(0, (fieldBounds.Height - imageBounds.Height) / 2);
			else if (AnyBottom.HasFlag(SortButtonAlignment))
				imageBounds.Offset(0, fieldBounds.Height - imageBounds.Height - SortButtonMargin.Height);
			else if (AnyTop.HasFlag(SortButtonAlignment))
				imageBounds.Offset(0, SortButtonMargin.Height);

			return imageBounds;
		}

		public Bitmap GetSortIcon(FieldControl field)
		{
			if (field.IsSortHotTracked)
			{
				switch (field.SortOrder)
				{
					case SortOrder.None:
						return SortIconHovered;
					case SortOrder.Ascending:
						return SortAscIconHovered;
					case SortOrder.Descending:
						return SortDescIconHovered;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			switch (field.SortOrder)
			{
				case SortOrder.None:
					return SortIcon;
				case SortOrder.Ascending:
					return SortAscIcon;
				case SortOrder.Descending:
					return SortDescIcon;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private const ContentAlignment AnyLeft = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
		private const ContentAlignment AnyCenter = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
		private const ContentAlignment AnyRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

		private const ContentAlignment AnyTop = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
		private const ContentAlignment AnyMiddle = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
		private const ContentAlignment AnyBottom = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;
	}
}