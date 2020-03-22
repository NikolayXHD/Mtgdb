using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;

namespace Mtgdb.Controls
{
	public static class BitmapExtensions
	{
		static BitmapExtensions()
		{
			string nativeImageField = Runtime.IsMono ? "nativeObject" : "nativeImage";
			_imageNativeImageField = typeof(Image).GetField(nativeImageField, BindingFlags.Instance | BindingFlags.NonPublic);
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

		public static Bitmap RotateFlipClone(this Bitmap value, RotateFlipType transform)
		{
			if (value == null)
				return null;

			value = (Bitmap) value.Clone();
			value.RotateFlip(transform);
			return value;
		}

		public static Rectangle GetRect(this Image image) =>
			new Rectangle(default, image.Size);


		public static bool IsDisposed(this Bitmap bmp)
		{
			var nativeImage = (IntPtr) _imageNativeImageField.GetValue(bmp);
			return nativeImage == IntPtr.Zero;
		}

		public static Bitmap HalfResizeDpi(this Bitmap original, bool preventMoire = false) =>
			resizeDpi(original, 0.5f, preventMoire).ApplyColorScheme();

		public static Bitmap ResizeDpi(this Bitmap original, float multiplier = 1f) =>
			resizeDpi(original, multiplier).ApplyColorScheme();

		private static Bitmap resizeDpi(this Bitmap original, float multiplier = 1f, bool preventMoire = false)
		{
			var originalSize = original.Size;
			var reducedSize = originalSize.MultiplyBy(multiplier).ByDpi().Round();

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

			using var chain = new BitmapTransformationChain(original, reThrow);
			for (int k = stepsCount - 1; k >= 0; k--)
			{
				Size currentSize = reducedSize.MultiplyBy(delta.MultiplyBy(k).Plus(1f)).Round();
				chain.ReplaceBy(bmp => bmp.FitIn(currentSize));
			}

			return chain.Result;
		}

		public static Bitmap ApplyColorScheme(this Bitmap bmp)
		{
			Bitmap transform()
			{
				if (bmp.IsDisposed())
					return bmp;

				var clone = (Bitmap) bmp.Clone();
				new ColorSchemeTransformation(clone).Execute();
				return clone;
			}

			var result = transform();
			ColorSchemeController.SystemColorsChanging += handler;
			return result;

			void handler()
			{
				if (bmp.IsDisposed())
				{
					ColorSchemeController.SystemColorsChanging -= handler;
					return;
				}

				using var transformed = transform();
				new BmpOverwrite(result, transformed).Execute();
			}
		}

		public static Bitmap ScaleBy(this Bitmap original, float factor) =>
			original.FitIn(original.Size.MultiplyBy(factor).Round());

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

		private static void reThrow(Exception ex) =>
			throw new ApplicationException("image transformation failed", ex);



		public static Func<Bitmap, Rectangle, Size, Bitmap> CustomScaleStrategy { get; set; }

		private static readonly FieldInfo _imageNativeImageField;
	}
}
