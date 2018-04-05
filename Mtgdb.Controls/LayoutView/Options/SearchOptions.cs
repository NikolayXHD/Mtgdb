using System.ComponentModel;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class SearchOptions
	{
		public SearchOptions Clone()
		{
			return new SearchOptions
			{
				Allow = Allow,
				Button = Button.Clone()
			};
		}

		[Category("Settings")]
		[DefaultValue(true)]
		public bool Allow { get; set; } = true;

		[Category("Settings")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public ButtonOptions Button { get; set; } = new ButtonOptions();

		public Rectangle GetButtonBounds(FieldControl field, LayoutControl layout)
		{
			if (!field.SearchOptions.Allow || !field.IsSearchVisible)
				return Rectangle.Empty;

			var fieldBounds = field.Bounds;
			fieldBounds.Offset(layout.Location);

			var fieldOptions = field.SearchOptions.Button;
			var generalOptions = Button;

			var icon = fieldOptions.Icon ?? generalOptions.Icon;
			var buttonAlignment = fieldOptions.Alignment ?? generalOptions.Alignment ?? ContentAlignment.TopRight;
			var buttonMargin = fieldOptions.Margin ?? generalOptions.Margin ?? new Size(2, 2);

			var imageBounds = new Rectangle(fieldBounds.Location, icon.Size);

			if (ContentAlignmentRanges.AnyCenter.HasFlag(buttonAlignment))
				imageBounds.Offset((fieldBounds.Width - imageBounds.Width) / 2, 0);
			else if (ContentAlignmentRanges.AnyRight.HasFlag(buttonAlignment))
				imageBounds.Offset(fieldBounds.Width - imageBounds.Width - buttonMargin.Width, 0);
			else if (ContentAlignmentRanges.AnyLeft.HasFlag(buttonAlignment))
				imageBounds.Offset(buttonMargin.Width, 0);

			if (ContentAlignmentRanges.AnyMiddle.HasFlag(buttonAlignment))
				imageBounds.Offset(0, (fieldBounds.Height - imageBounds.Height) / 2);
			else if (ContentAlignmentRanges.AnyBottom.HasFlag(buttonAlignment))
				imageBounds.Offset(0, fieldBounds.Height - imageBounds.Height - buttonMargin.Height);
			else if (ContentAlignmentRanges.AnyTop.HasFlag(buttonAlignment))
				imageBounds.Offset(0, buttonMargin.Height);

			return imageBounds;
		}

		public Bitmap GetIcon(FieldControl field)
		{
			var fieldOptions = field.SearchOptions.Button;
			var generalOptions = Button;

			if (field.IsSearchHotTracked)
				return fieldOptions.Icon ?? generalOptions.Icon;

			return fieldOptions.IconTransp ?? generalOptions.IconTransp;
		}

		public bool IsButtonVisible(FieldControl field)
		{
			var fieldOptions = field.SearchOptions.Button;
			var generalOptions = Button;

			return Allow && field.SearchOptions.Allow && (
				field.IsHotTracked ||
				!(fieldOptions.ShowOnlyWhenHotTracked ?? generalOptions.ShowOnlyWhenHotTracked ?? true));
		}
	}
}