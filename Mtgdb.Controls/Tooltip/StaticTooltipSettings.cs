using System;
using System.Windows.Forms;

namespace Mtgdb.Controls
{
	public class StaticTooltipSettings
	{
		public Func<string> Title { get; set; }
		public Func<string> Text { get; set; }
		public Control[] Controls { get; set; }

		public bool IsEmpty => string.IsNullOrEmpty(Text()) && string.IsNullOrEmpty(Title());
	}
}