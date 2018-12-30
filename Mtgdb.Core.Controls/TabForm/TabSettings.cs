using System.Drawing;

namespace Mtgdb.Controls
{
	public class TabSettings
	{
		public TabSettings(string text)
		{
			HasText = true;
			Text = text;
		}

		public TabSettings(Bitmap icon)
		{
			HasIcon = true;
			Icon = icon;
		}

		public TabSettings(string text, Bitmap icon)
		{
			HasText = true;
			Text = text;

			HasIcon = true;
			Icon = icon;
		}

		public object Tag { get; set; }

		public string Text { get; }
		public bool HasText { get; }

		public Bitmap Icon { get; }
		public bool HasIcon { get; }

		public object TabId { get; set; }
	}
}