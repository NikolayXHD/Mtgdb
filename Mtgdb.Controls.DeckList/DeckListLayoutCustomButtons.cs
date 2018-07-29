using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mtgdb.Controls.Properties;

namespace Mtgdb.Controls
{
	internal static class DeckListLayoutCustomButtons
	{
		public static void SetCustomButtons(DeckListLayout layout)
		{
			layout.FieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomLeft,
				Icon = selectBitmap(Resources.Remove_16, Resources.Remove_32),
				ShowOnlyWhenHotTracked = false
			});

			layout.FieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = selectBitmap(Resources.Open_16, Resources.Open_32),
				ShowOnlyWhenHotTracked = false
			});

			layout.FieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = selectBitmap(Resources.Open_transformed_16, Resources.Open_transformed_32),
				ShowOnlyWhenHotTracked = false,
				Margin = new Size(4, 0)
			});

			layout.FieldSaved.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = selectBitmap(Resources.Add_16, Resources.Add_32),
				ShowOnlyWhenHotTracked = false
			});

			layout.FieldName.CustomButtons.Add(new ButtonOptions
			{
				Alignment = ContentAlignment.BottomRight,
				Icon = selectBitmap(Resources.Rename_16, Resources.Rename_32),
				ShowOnlyWhenHotTracked = false
			});
		}

		private static Bitmap selectBitmap(Bitmap defaultSized, Bitmap doubleSized)
		{
			if (Dpi.ScalePercent > 100)
				return doubleSized.HalfResizeDpi();

			return defaultSized.ResizeDpi();
		}

		public static bool IsRemoveButton(this HitInfo info) =>
			Str.Equals(info.FieldName, nameof(DeckModel.Saved)) && info.CustomButtonIndex == CustomButtonRemove;

		public static bool IsOpenButton(this HitInfo info) =>
			Str.Equals(info.FieldName, nameof(DeckModel.Saved)) && info.CustomButtonIndex == CustomButtonOpen;

		public static bool IsOpenTransformedButton(this HitInfo info) =>
			Str.Equals(info.FieldName, nameof(DeckModel.Saved)) && info.CustomButtonIndex == CustomButtonOpenTransformed;

		public static bool IsAddButton(this HitInfo info) =>
			Str.Equals(info.FieldName, nameof(DeckModel.Saved)) && info.CustomButtonIndex == CustomButtonAdd;

		public static bool IsRenameButton(this HitInfo info) =>
			Str.Equals(info.FieldName, nameof(DeckModel.Name)) && info.CustomButtonIndex == CustomButtonRename;

		public static IEnumerable<ButtonLayout> GetCustomButtons(IEnumerable<ButtonLayout> baseResult, FieldControl field, DeckModel deckModel)
		{
			if (!Str.Equals(field.FieldName, nameof(DeckModel.Saved)))
				return baseResult;

			var list = baseResult.ToList();

			if (deckModel.IsCurrent)
			{
				list[CustomButtonRemove].Size = Size.Empty;
				list[CustomButtonOpen].Size = Size.Empty;
				list[CustomButtonOpenTransformed].Size = Size.Empty;
			}
			else
				list[CustomButtonAdd].Size = Size.Empty;

			return list;
		}

		private const int CustomButtonRemove = 0;
		private const int CustomButtonOpen = 1;
		private const int CustomButtonOpenTransformed = 2;
		private const int CustomButtonAdd = 3;
		private const int CustomButtonRename = 0;
	}
}