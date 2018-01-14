using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class SearchOptions
	{
		[Category("Settings")]
		[DefaultValue(false)]
		public bool Allow { get; set; }



		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap IconTransp { get; private set; }

		[Category("Settings")]
		[DefaultValue(null)]
		public Bitmap Icon
		{
			get { return _icon; }
			set
			{
				_icon = value;
				IconTransp = value?.SetOpacity(0.75f);
			}
		}

		[Category("Settings")]
		[DefaultValue(typeof(ContentAlignment), "TopRight")]
		public ContentAlignment ButtonAlignment { get; set; } = ContentAlignment.TopRight;

		[Category("Settings")]
		[DefaultValue(typeof(Size), "20, 2")]
		public Size ButtonMargin { get; set; } = new Size(20, 2);

		public Rectangle GetButtonBounds(FieldControl field, LayoutControl layout)
		{
			if (!field.AllowSearch || !field.IsSearchVisible)
				return Rectangle.Empty;

			var fieldBounds = field.Bounds;
			fieldBounds.Offset(layout.Location);

			var imageBounds = new Rectangle(fieldBounds.Location, IconTransp.Size);

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
			if (field.IsSearchHotTracked)
				return Icon;

			return IconTransp;
		}

		private Bitmap _icon;
	}
}