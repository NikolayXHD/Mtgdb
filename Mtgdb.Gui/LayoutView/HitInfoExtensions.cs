using Mtgdb.Controls;
using Mtgdb.Data;

namespace Mtgdb.Gui
{
	public static class HitInfoExtensions
	{
		public static bool IsOverImage(this HitInfo value) =>
			Str.Equals(value.FieldName, nameof(Card.Image)) && !value.IsSomeButton;

		public static bool IsOverText(this HitInfo value) =>
			!string.IsNullOrEmpty(value.FieldName) && !value.IsSomeButton && !IsOverImage(value);
	}
}