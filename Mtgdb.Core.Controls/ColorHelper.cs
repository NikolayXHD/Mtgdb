using System.Drawing;

namespace Mtgdb.Controls
{
	public static class ColorHelper
	{
		public static Color DistinctSystemBorder
		{
			get
			{
				Color c;

				var controlHsv = SystemColors.Control.ToHsv();
				var borderHsv = SystemColors.ActiveBorder.ToHsv();
				var frameHsv = SystemColors.WindowFrame.ToHsv();

				if (borderHsv == controlHsv && borderHsv != frameHsv)
					c = SystemColors.WindowFrame;
				else
					c = SystemColors.ActiveBorder;

				return c;
			}
		}
	}
}