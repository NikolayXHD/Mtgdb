using System;
using System.Drawing;
using System.Linq;

namespace Mtgdb.Controls
{
	public static class LayoutViewControlExtensions
	{
		public static void TransformIcons(
			this LayoutViewControl view,
			Func<Bitmap, Bitmap> transformation)
		{
			view.SortOptions.Icon = view.SortOptions.Icon?.Invoke0(transformation);
			view.SortOptions.AscIcon = view.SortOptions.AscIcon?.Invoke0(transformation);
			view.SortOptions.DescIcon = view.SortOptions.DescIcon?.Invoke0(transformation);

			view.SortOptions.IconTransp = view.SortOptions.IconTransp?.Invoke0(transformation);
			view.SortOptions.AscIconTransp = view.SortOptions.AscIconTransp?.Invoke0(transformation);
			view.SortOptions.DescIconTransp = view.SortOptions.DescIconTransp?.Invoke0(transformation);

			view.SearchOptions.Button.Icon = view.SearchOptions.Button.Icon?.Invoke0(transformation);
			view.SearchOptions.Button.IconTransp = view.SearchOptions.Button.IconTransp?.Invoke0(transformation);

			foreach (var pair in view.LayoutOptions.AlignmentIconsByDirection.ToArray())
				view.LayoutOptions.AlignmentIconsByDirection[pair.Key] = pair.Value?.Invoke0(transformation);

			foreach (var pair in view.LayoutOptions.AlignmentHoveredIconsByDirection.ToArray())
				view.LayoutOptions.AlignmentHoveredIconsByDirection[pair.Key] = pair.Value?.Invoke0(transformation);
		}

		public static void TransformFieldIcons(this LayoutViewControl view,
			Func<Bitmap, string, int, Bitmap> customButtonIcon,
			Func<Bitmap, Bitmap> searchIcon)
		{
			view.ProbeCardCreating += (s, c) =>
			{
				foreach (var field in c.Fields)
				{
					field.SearchOptions.Button.Icon = field.SearchOptions.Button.Icon?.Invoke0(searchIcon);
					for (int i = 0; i < field.CustomButtons.Count; i++)
					{
						var customButton = field.CustomButtons[i];
						customButton.Icon = customButton.Icon?.Invoke1(customButtonIcon, field.FieldName, i);
						customButton.IconTransp = customButton.IconTransp?.Invoke1(customButtonIcon, field.FieldName, i);
					}
				}
			};
		}
	}
}