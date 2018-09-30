using System.Drawing;
using Mtgdb.Controls;
using Mtgdb.Gui.Properties;

namespace Mtgdb.Gui
{
	public static class DeckEditorButtons
	{
		public static void SetupButtons(FieldControl field)
		{
			var margin = new Size(8, 0);

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.remove_four,
				Margin = margin
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.remove_one,
				Margin = margin,
				BreaksLayout = true
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.add_four,
				Margin = margin
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.add_one,
				Margin = margin
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.remove_four,
				Margin = margin
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.remove_one_collection,
				Margin = margin,
				BreaksLayout = true
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.add_four,
				Margin = margin
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.add_one_collection,
				Margin = margin
			});
		}

		public static bool IsDeck(int customButtonIndex) =>
			customButtonIndex < 4;

		public static int GetCountDelta(int customButtonIndex)
		{
			customButtonIndex = customButtonIndex % 4;

			switch (customButtonIndex)
			{
				case 0:
					return -4;
				case 1:
					return -1;
				case 2:
					return +4;
				case 3:
					return +1;

				default: return 0;
			}
		}
	}
}