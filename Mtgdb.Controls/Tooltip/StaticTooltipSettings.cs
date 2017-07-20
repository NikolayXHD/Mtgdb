using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class StaticTooltipSettings
	{
		public string Title { get; set; }
		public string Text { get; set; }
		public Control[] Controls { get; set; }

		public bool IsEmpty => string.IsNullOrEmpty(Text) && string.IsNullOrEmpty(Title);
	}
}