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

		public ButtonLayout GetButtonLayout(FieldControl field)
		{
			if (!field.SearchOptions.Allow || !field.IsSearchVisible)
				return new ButtonLayout(null, Size.Empty, Size.Empty, ContentAlignment.MiddleCenter, ButtonType.Search);

			var fieldOptions = field.SearchOptions.Button;
			var generalOptions = Button;

			var icon = field.IsSearchHotTracked
				? fieldOptions.Icon ?? generalOptions.Icon
				: fieldOptions.IconTransp ?? generalOptions.IconTransp;

			var buttonAlignment = fieldOptions.Alignment ?? generalOptions.Alignment ?? ContentAlignment.TopRight;
			var buttonMargin = fieldOptions.Margin ?? generalOptions.Margin ?? new Size(2, 2);

			return new ButtonLayout(icon, icon.Size, buttonMargin, buttonAlignment, ButtonType.Search);
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