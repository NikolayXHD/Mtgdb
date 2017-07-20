using Mtgdb.Controls;
using Mtgdb.Dal;

namespace Mtgdb.Gui
{
	public static class HitInfoExtensions
	{
		public static bool IsOverImage(this HitInfo value)
		{
			return value.CardBounds.HasValue && IsImageField(value.FieldName);
		}

		public static bool IsImageField(string fieldName)
		{
			return Str.Equals(fieldName, nameof(Card.Image)) || string.IsNullOrEmpty(fieldName);
		}
	}
}