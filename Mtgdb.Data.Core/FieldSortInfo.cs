namespace Mtgdb.Data
{
	public class FieldSortInfo
	{
		public FieldSortInfo(string fieldName, SortDirection sortOrder)
		{
			FieldName = fieldName;
			SortOrder = sortOrder;
		}

		public string FieldName { get; }

		public SortDirection SortOrder { get; set; }

		public override string ToString()
		{
			string sortGlyph;
			switch (SortOrder)
			{
				case SortDirection.Asc:
					sortGlyph = "↑";
					break;
				case SortDirection.Desc:
					sortGlyph = "↓";
					break;
				default:
					sortGlyph = "↕";
					break;
			}

			return $"{FieldName} {sortGlyph}";
		}
	}
}