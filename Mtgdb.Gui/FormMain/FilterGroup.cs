using System;

namespace Mtgdb.Gui
{
	internal enum FilterGroup
	{
		None = 0,
		Buttons,
		Find,
		Legality,
		Collection,
		Deck
	}

	internal static class FilterGroupExt
	{
		public static int Index(this FilterGroup value)
		{
			switch (value)
			{
				case FilterGroup.Buttons:
					return 0;
				case FilterGroup.Find:
					return 1;
				case FilterGroup.Legality:
					return 2;
				case FilterGroup.Collection:
					return 3;
				case FilterGroup.Deck:
					return 4;
				default:
					throw new ArgumentOutOfRangeException(nameof(value), value, message: null);
			}
		}
	}
}