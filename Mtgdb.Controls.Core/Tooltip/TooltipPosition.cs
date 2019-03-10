using System.Drawing;

namespace Mtgdb.Controls
{
	public class TooltipPosition
	{
		public TooltipPosition(Size size, int left, int top, Point offset)
		{
			Bounds = new Rectangle(left + offset.X, top + offset.Y, size.Width, size.Height);
		}

		public Rectangle Bounds { get; set; }
	}
}