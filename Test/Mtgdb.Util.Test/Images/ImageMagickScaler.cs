using System.Drawing;
using ImageMagick;

namespace Mtgdb.Util
{
	public static class ImageMagickScaler
	{
		public static Bitmap Scale(Bitmap original, Rectangle sourceRect, Size targetSize)
		{
			using var magic = new MagickImage(original);
			if (sourceRect != new Rectangle(Point.Empty, original.Size))
				magic.Crop(new MagickGeometry(sourceRect));

			if (targetSize != sourceRect.Size)
				magic.Resize(new MagickGeometry(targetSize.Width, targetSize.Height));

			return magic.ToBitmap();
		}
	}
}