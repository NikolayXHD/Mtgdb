using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public static class GeometryHelpers
	{
		public static Point Round(this PointF location)
		{
			return new Point((int) Math.Round(location.X), (int) Math.Round(location.Y));
		}

		public static int Round(this float value)
		{
			return (int) Math.Round(value);
		}

		public static SizeF ScaleBy(this SizeF original, SizeF scale)
		{
			return new SizeF(
				original.Width * scale.Width,
				original.Height * scale.Height);
		}

		public static Size ScaleBy(this Size original, SizeF scale)
		{
			return ScaleBy((SizeF) original, scale).ToSize();
		}

		public static SizeF Multiply(this SizeF size, float value)
		{
			return new SizeF(value * size.Width, value * size.Height);
		}

		public static SizeF Add(this SizeF size, SizeF value)
		{
			return new SizeF(size.Width + value.Width, size.Height + value.Height);
		}

		public static PointF Multiply(this PointF location, float value)
		{
			return new PointF(value * location.X, value * location.Y);
		}

		public static int SizeInPixels(this Font font)
		{
			return ((int)(font.SizeInPoints * 96 / 72)).ByDpiWidth();
		}
	}
}