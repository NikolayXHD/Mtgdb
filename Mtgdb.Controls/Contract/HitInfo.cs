using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class HitInfo
	{
		private const int EmptyRowHandle = -1;

		public HitInfo()
		{
			RowHandle = EmptyRowHandle;
		}

		internal void SetCard(LayoutControl card, int rowHandle, object dataSource)
		{
			Card = card;
			RowHandle = rowHandle;
			CardBounds = card?.Bounds;
			RowDataSource = dataSource;
		}

		internal void ClearCard()
		{
			SetCard(card: null, rowHandle: EmptyRowHandle, dataSource: null);
		}

		internal void SetField(FieldControl field, bool isSortButton, bool isSearchButton, int customButtonIndex, Rectangle? buttonBounds)
		{
			if (field != null && Card == null)
				throw new InvalidOperationException($"{nameof(Card)} must be set first");

			Field = field;
			FieldBounds = field?.Bounds.Plus(Card.Bounds.Location);
			FieldName = field?.FieldName;
			IsSortButton = isSortButton;
			IsSearchButton = isSearchButton;
			CustomButtonIndex = customButtonIndex;
			ButtonBounds = buttonBounds ?? Rectangle.Empty;
		}

		internal void ClearField()
		{
			SetField(field: null, isSortButton: false, isSearchButton: false, customButtonIndex: -1, buttonBounds: null);
		}

		public bool IsSomeButton => IsSearchButton || IsSortButton || CustomButtonIndex >= 0 || AlignButtonDirection.HasValue;

		public bool InBounds { get; set; }
		public Direction? AlignButtonDirection { get; set; }

		public int RowHandle { get; private set; }
		public object RowDataSource { get; private set; }
		internal LayoutControl Card { get; private set; }
		public Rectangle? CardBounds { get; private set; }
		public Rectangle ButtonBounds { get; private set; }

		internal FieldControl Field { get; private set; }
		public Rectangle? FieldBounds { get; private set; }
		public string FieldName { get; private set; }
		public bool IsSortButton { get; private set; }
		public bool IsSearchButton { get; private set; }
		public int CustomButtonIndex { get; set; }
	}
}