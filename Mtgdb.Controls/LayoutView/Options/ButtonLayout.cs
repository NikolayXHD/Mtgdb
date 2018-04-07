using System.Drawing;

namespace Mtgdb.Controls
{
	public class ButtonLayout
	{
		public ButtonLayout(Bitmap icon = null, Size? margin = null, ContentAlignment? alignment = null, bool? breaksLayout = null, ButtonType type = ButtonType.Custom)
		{
			Icon = icon;
			Size = icon?.Size ?? Size.Empty;
			Margin = margin ?? new Size(2, 2);
			Alignment = alignment ?? ContentAlignment.TopRight;
			Type = type;
			BreaksLayout = breaksLayout ?? false;

			Location = Point.Empty;
		}

		public Bitmap Icon { get; }
		
		public Size Size { get; }
		public Size Margin { get; }
		public ContentAlignment Alignment { get; }
		public bool BreaksLayout { get; }

		public ButtonType Type { get; }

		public Point Location { get; set; }
		public Rectangle Bounds => new Rectangle(Location, Size);
	}
}