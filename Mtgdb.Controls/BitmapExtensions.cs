using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mtgdb.Controls
{
	public static class BitmapExtensions
	{
		public static Bitmap Scale(this Bitmap original, Size size, Size frame)
		{
			var croppedSize = new Size(
				original.Width - 2 * frame.Width,
				original.Height - 2 * frame.Height);

			size = croppedSize.ZoomTo(size);

			var result = new Bitmap(size.Width, size.Height);

			try
			{
				var graphics = Graphics.FromImage(result);
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.DrawImage(
					original,
					destRect: new RectangleF(-0.5f, -0.5f, result.Width + 0.5f, result.Height + 0.5f),
					srcRect: new Rectangle(new Point(frame), croppedSize),
					srcUnit: GraphicsUnit.Pixel);
			}
			catch
			{
				result.Dispose();
				throw;
			}

			return result;
		}

		public static Size ZoomTo(this Size originalSize, Size viewPortSize)
		{
			var factor = Math.Min(
				(float) viewPortSize.Width/originalSize.Width,
				(float) viewPortSize.Height/originalSize.Height);

			var zoomed = new Size(
				(int) Math.Round(originalSize.Width*factor),
				(int) Math.Round(originalSize.Height*factor));

			return zoomed;
		}

		public static SizeF ZoomTo(this Size originalSize, SizeF viewPortSize)
		{
			var factor = Math.Min(
				viewPortSize.Width / originalSize.Width,
				viewPortSize.Height / originalSize.Height);

			var zoomed = new SizeF(
				originalSize.Width * factor,
				originalSize.Height * factor);

			return zoomed;
		}

		public static Rectangle ZoomTo(this Size original, Rectangle viewPort)
		{
			return new Rectangle(viewPort.Location, original.ZoomTo(viewPort.Size));
		}

		public static Bitmap SetOpacity(this Bitmap image, float opacity)
		{
			var colorMatrix = new ColorMatrix { Matrix33 = opacity };

			var imageAttributes = new ImageAttributes();

			imageAttributes.SetColorMatrix(
				colorMatrix,
				ColorMatrixFlag.Default,
				ColorAdjustType.Bitmap);

			var output = new Bitmap(image.Width, image.Height);

			using (var g = Graphics.FromImage(output))
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
				var destRect = new Rectangle(0, 0, image.Width, image.Height);
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}

			return output;
		}

		public static Bitmap TransformColors(this Bitmap image, float saturation = 1f, float brightness = 1f)
		{
			const float rwgt = 0.3086f;
			const float gwgt = 0.6094f;
			const float bwgt = 0.0820f;

			var colorMatrix = new ColorMatrix();

			float baseSat = 1.0f - saturation;

			colorMatrix[0, 0] = baseSat * rwgt + saturation;
			colorMatrix[0, 1] = baseSat * rwgt;
			colorMatrix[0, 2] = baseSat * rwgt;
			colorMatrix[1, 0] = baseSat * gwgt;
			colorMatrix[1, 1] = baseSat * gwgt + saturation;
			colorMatrix[1, 2] = baseSat * gwgt;
			colorMatrix[2, 0] = baseSat * bwgt;
			colorMatrix[2, 1] = baseSat * bwgt;
			colorMatrix[2, 2] = baseSat * bwgt + saturation;

			float adjustedBrightness = brightness - 1f;

			colorMatrix[4, 0] = adjustedBrightness;
			colorMatrix[4, 1] = adjustedBrightness;
			colorMatrix[4, 2] = adjustedBrightness;

			var imageAttributes = new ImageAttributes();

			imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

			var output = new Bitmap(image.Width, image.Height);
			using (var g = Graphics.FromImage(output))
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
				var destRect = new Rectangle(0, 0, image.Width, image.Height);
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}

			return output;
		}
	}
}