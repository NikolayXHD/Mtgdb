using System.Drawing;

namespace Mtgdb.Controls
{
	public class ButtonLayout : ILayoutElement
	{
		public ButtonLayout(Bitmap icon, Size size, Size margin, ContentAlignment alignment, ButtonType type)
		{
			Icon = icon;
			Size = size;
			Margin = margin;
			Alignment = alignment;
			Type = type;

			Location = Point.Empty;
		}

		public Bitmap Icon { get; }
		public Size Size { get; }
		public Size Margin { get; }
		public ContentAlignment Alignment { get; }
		public ButtonType Type { get; }

		public Point Location { get; set; }
		public Rectangle Bounds => new Rectangle(Location, Size);
	}
}