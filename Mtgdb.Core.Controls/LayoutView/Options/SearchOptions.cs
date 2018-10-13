using System.ComponentModel;

namespace Mtgdb.Controls
{
	public class SearchOptions
	{
		public SearchOptions()
		{
			Button = new ButtonOptions
			{
				Icon = Properties.Resources.search
			};
		}

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
		public ButtonOptions Button { get; set; }

		public ButtonLayout GetButtonLayout(FieldControl field)
		{
			if (!field.SearchOptions.Allow || !field.IsSearchVisible)
				return new ButtonLayout(type: ButtonType.Search);

			var fieldOptions = field.SearchOptions.Button;
			var generalOptions = Button;

			var icon = field.IsSearchHotTracked
				? fieldOptions.Icon ?? generalOptions.Icon
				: fieldOptions.IconTransp ?? generalOptions.IconTransp;

			var buttonAlignment = fieldOptions.Alignment ?? generalOptions.Alignment;
			var buttonMargin = fieldOptions.Margin ?? generalOptions.Margin;
			var breaksLayout = fieldOptions.BreaksLayout ?? generalOptions.BreaksLayout;

			return new ButtonLayout(icon, buttonMargin, buttonAlignment, breaksLayout, ButtonType.Search);
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