using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class FieldSortInfo
	{
		public FieldSortInfo(string fieldName, SortOrder sortOrder)
		{
			FieldName = fieldName;
			SortOrder = sortOrder;
		}

		public string FieldName { get; }

		public SortOrder SortOrder { get; internal set; }

		public override string ToString()
		{
			return $"{FieldName} {SortOrder}";
		}
	}
}