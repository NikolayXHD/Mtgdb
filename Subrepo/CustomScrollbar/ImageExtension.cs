using System.Drawing;

namespace CustomScrollbar
{
	public static class ImageExtension
	{
		public static Rectangle GetRect(this Image image) =>
			new Rectangle(default, image.Size);
	}
}