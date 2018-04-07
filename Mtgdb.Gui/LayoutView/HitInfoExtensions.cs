using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public static class HitInfoExtensions
	{
		public static bool IsOverImage(this HitInfo value)
		{
			return Str.Equals(value.FieldName, nameof(Card.Image)) && !IsOverButton(value);
		}

		public static bool IsOverButton(this HitInfo value)
		{
			return value.IsSearchButton || value.IsSortButton || value.CustomButtonIndex >= 0;
		}

		public static bool IsOverText(this HitInfo value)
		{
			return !string.IsNullOrEmpty(value.FieldName) && !IsOverButton(value) && !IsOverImage(value);
		}
	}
}