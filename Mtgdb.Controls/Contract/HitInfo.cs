using System.Drawing;

namespace Mtgdb.Controls
{
	public class HitInfo
	{
		public HitInfo()
		{
		}

		public int RowHandle { get; set; }
		public Rectangle? CardBounds { get; set; }
		public Rectangle? FieldBounds { get; set; }
		public string FieldName { get; set; }
		public bool InBounds { get; set; }
		public bool IsSortButton { get; set; }

		internal LayoutControl Card { get; set; }
		internal FieldControl Field { get; set; }
	}
}