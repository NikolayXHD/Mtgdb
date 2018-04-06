using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public static class HitInfoExtensions
	{
		public static bool IsOverImage(this HitInfo value)
		{
			return IsImageField(value.FieldName) && !value.IsSearchButton && !value.IsSortButton;
		}

		public static bool IsImageField(string fieldName)
		{
			return Str.Equals(fieldName, nameof(Card.Image));
		}
	}
}