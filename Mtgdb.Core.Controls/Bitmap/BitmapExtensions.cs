using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mtgdb.Controls
{
	public static class BitmapExtensions
	{
		public static Func<Bitmap, Rectangle, Size, Bitmap> CustomScaleStrategy { get; set; }

		public static Bitmap ResizeDpi(this Bitmap original, float multiplier = 1f)
		{
			return original.FitIn(original.Size.MultiplyBy(multiplier).ByDpi().Round());
		}

		public static Bitmap HalfResizeDpi(this Bitmap original, bool preventMoire = false)
		{
			var originalSize = original.Size;
			var reducedSize = originalSize.HalfByDpi();

			if (originalSize == reducedSize)
				return original;

			int stepsCount;

			if (preventMoire)
			{
				var originalToReducedRatio = originalSize.DivideBy(reducedSize);
				float maxRatio = originalToReducedRatio.Max();
				stepsCount = (int) Math.Round((maxRatio - 1) * 3f);
			}
			else
				stepsCount = 1;

			SizeF delta = originalSize.Minus(reducedSize).DivideBy(
				reducedSize.MultiplyBy(stepsCount));

			using (var chain = new BitmapTransformationChain(original, reThrow))
			{
				for (int k = stepsCount - 1; k >= 0; k--)
				{
					Size currentSize = reducedSize.MultiplyBy(delta.MultiplyBy(k).Plus(1f)).Round();
					chain.TransformCopying(bmp => bmp.FitIn(currentSize));
				}

				return chain.Result;
			}
		}

		public static Bitmap FitIn(this Bitmap original, Size size, Size frame = default)
		{
			var croppedSize = new Size(
				original.Width - 2 * frame.Width,
				original.Height - 2 * frame.Height);

			size = croppedSize.FitIn(size);

			if (size == original.Size && frame == default)
				return (Bitmap) original.Clone();

			var sourceRect = new Rectangle(new Point(frame), croppedSize);

			var result = scale(original, sourceRect, size);
			return result;
		}

		private static Bitmap scale(Bitmap original, Rectangle sourceRect, Size size)
		{
			if (CustomScaleStrategy != null)
				return CustomScaleStrategy(original, sourceRect, size);

			var result = new Bitmap(size.Width, size.Height);
			var scaler = new BmpScaler(result, original, sourceRect);
			scaler.Execute();
			return result;
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
				var destRect = new Rectangle(Point.Empty, image.Size);
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}

			return output;
		}

		public static Bitmap TransformColors(this Bitmap image, float saturation = 1f, float brightness = 1f)
		{
			const float rWgt = 0.3086f;
			const float gWgt = 0.6094f;
			const float bWgt = 0.0820f;

			var colorMatrix = new ColorMatrix();

			float baseSat = 1.0f - saturation;

			colorMatrix[0, 0] = baseSat * rWgt + saturation;
			colorMatrix[0, 1] = baseSat * rWgt;
			colorMatrix[0, 2] = baseSat * rWgt;
			colorMatrix[1, 0] = baseSat * gWgt;
			colorMatrix[1, 1] = baseSat * gWgt + saturation;
			colorMatrix[1, 2] = baseSat * gWgt;
			colorMatrix[2, 0] = baseSat * bWgt;
			colorMatrix[2, 1] = baseSat * bWgt;
			colorMatrix[2, 2] = baseSat * bWgt + saturation;

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
				var destRect = new Rectangle(Point.Empty, image.Size);
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
			}

			return output;
		}

		public static Bitmap Transform(this Bitmap value, RotateFlipType transform)
		{
			value = (Bitmap) value.Clone();
			value.RotateFlip(transform);
			return value;
		}

		private static void reThrow(Exception ex)
		{
			throw new Exception("image transformation failed", ex);
		}
	}
}