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
			var opacityDelta = 0.2f;

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.remove_four,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.remove_one,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta,
				BreaksLayout = true
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.add_four,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = Resources.add_one,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta,
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.remove_four,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.remove_one_collection,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta,
				BreaksLayout = true
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.add_four,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta
			});

			field.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = Resources.add_one_collection,
				Margin = margin,
				HotTrackOpacityDelta = opacityDelta
			});
		}

		public static bool IsDeck(int customButtonIndex)
		{
			return customButtonIndex < 4;
		}

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