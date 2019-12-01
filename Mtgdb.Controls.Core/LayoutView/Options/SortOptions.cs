using System;
using System.ComponentModel;
using System.Drawing;
using Mtgdb.Data;

namespace Mtgdb.Controls
{
	public class SortOptions
	{
		public SortOptions()
		{
			Icon = Properties.Resources.sort_none_hovered;
			AscIcon = Properties.Resources.sort_asc_hovered;
			DescIcon = Properties.Resources.sort_desc_hovered;
		}

		public bool IsButtonVisible(LayoutControl card, FieldControl field) =>
			Allow && card.ShowSortButton(field);

		public ButtonLayout GetButtonLayout(FieldControl field)
		{
			if (!field.AllowSort || !field.IsSortVisible)
				return new ButtonLayout(type: ButtonType.Sort);

			var icon = getIcon(field);

			return new ButtonLayout(icon, ButtonMargin, ButtonAlignment, breaksLayout: false, type: ButtonType.Sort);
		}

		private Bitmap getIcon(FieldControl field)
		{
			if (field.IsSortHotTracked)
			{
				switch (field.SortOrder)
				{
					case SortDirection.No:
						return Icon;
					case SortDirection.Asc:
						return AscIcon;
					case SortDirection.Desc:
						return DescIcon;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			switch (field.SortOrder)
			{
				case SortDirection.No:
					return IconTransp;
				case SortDirection.Asc:
					return AscIconTransp;
				case SortDirection.Desc:
					return DescIconTransp;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}



		[Category("Settings")]
		[DefaultValue(false)]
		public bool Allow { get; set; }



		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap IconTransp { get; set; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap AscIconTransp { get; set; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap DescIconTransp { get; set; }



		private Bitmap _icon;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap Icon
		{
			get => _icon;
			set
			{
				_icon = value;
				IconTransp ??= deriveTranspIcon(value);
			}
		}

		private Bitmap _ascIcon;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap AscIcon
		{
			get => _ascIcon;
			set
			{
				_ascIcon = value;
				AscIconTransp ??= deriveTranspIcon(value);
			}
		}

		private Bitmap _descIcon;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap DescIcon
		{
			get => _descIcon;
			set
			{
				_descIcon = value;
				DescIconTransp ??= deriveTranspIcon(value);
			}
		}

		private static Bitmap deriveTranspIcon(Bitmap value) =>
			value?.SetOpacity(1f - HotTrackOpacityDelta);

		[Category("Settings")]
		[DefaultValue(typeof(ContentAlignment), "TopRight")]
		public ContentAlignment ButtonAlignment { get; set; } = ContentAlignment.TopRight;

		[Category("Settings")]
		[DefaultValue(typeof(Size), "2, 2")]
		public Size ButtonMargin { get; set; } = new Size(2, 2);

		private const float HotTrackOpacityDelta = 0.15f;
	}
}
