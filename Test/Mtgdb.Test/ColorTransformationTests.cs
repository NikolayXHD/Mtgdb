using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Mtgdb.Controls;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class ColorTransformationTests
	{
		[Test]
		public void When_transforming_h_Then_s_and_v_remain_unchanged()
		{
			var source = Color.Gray;
			var result = source.TransformHsv(
				h: _ => _ + Color.LightBlue.RotationTo(Color.LavenderBlush));

			float ds = result.ToHsv().S - source.ToHsv().S;
			float dv = result.ToHsv().V - source.ToHsv().V;

			Assert.That(ds, Is.EqualTo(0).Within(0.02));
			Assert.That(dv, Is.EqualTo(0).Within(0.02));
		}

		[Test]
		public void Black_is_transformed_to_first_color()
		{
			Color c1 = Color.FromArgb(unchecked((int) 0xFF_FF_FF_00));
			Color c2 = Color.FromArgb(unchecked((int) 0xFF_00_FF_00));


			var transformation = new ColorSchemeTransformation(c1.ToHsv(), c2.ToHsv());
			var sourceColor = Color.FromArgb(unchecked((int) 0xFF_00_00_00));
			var transformed = transformation.Transform(sourceColor);

			Assert.That(transformed.ToArgb(), Is.EqualTo(unchecked((int) 0xFF_FF_FF_00)));
		}

		[Test]
		public void Black_is_transformed_to_first_color_In_byte_array()
		{
			Color c1 = Color.FromArgb(unchecked((int) 0xFF_FF_FF_00));
			Color c2 = Color.FromArgb(unchecked((int) 0xFF_00_FF_00));


			var transformation = new ColorSchemeTransformation(c1.ToHsv(), c2.ToHsv());
			var rgbValues = new byte[] { 0x00, 0x00, 0x00, 0xFF };

			transformation.Transform(rgbValues, 0);

			var expectedTransformed = new byte[] { 0xFF, 0xFF, 0x00, 0xFF };

			Assert.That(rgbValues, Is.EquivalentTo(expectedTransformed));
		}

		[TestCase(unchecked((int)0xFF_FF_00_00))]
		[TestCase(unchecked((int)0xFF_00_FF_00))]
		[TestCase(unchecked((int)0xFF_00_00_FF))]
		public void Rgb_to_hsv_Is_reversible(int argb)
		{
			int argbAfterReversal = Color.FromArgb(argb).ToHsv().ToRgb().ToArgb();
			Assert.That(argbAfterReversal, Is.EqualTo(argb));
		}

		[Test]
		public void Hsv_to_rgb_Is_reversible([Values(0f, 60f, 120f, 180f, 240f, 300f)]float h)
		{
			var hAfterReversal = new HsvColor(h, 1f, 1f).ToRgb().ToHsv().H;
			Assert.That(hAfterReversal, Is.EqualTo(h).Within(0.02));
		}

		[Test]
		public void Brightness_adaptation_Interpolates_hue_saturation_and_value(
			[Values(0f, 0.5f, 1f)] float v,
			[Values(90f)] float c1h,
			[Values(-30f, 30f)] float dh)
		{
			HsvColor c1 = new HsvColor(c1h, 1f, 1f);
			HsvColor c2 = new HsvColor(c1h + dh, 1f, 1f);

			var transformation = new ColorSchemeTransformation(c1, c2);

			var sourceColor = new HsvColor(0, 0, v);
			var resultColor = transformation.Transform(sourceColor);

			Assert.That(resultColor.H, Is.EqualTo(c1.H + v * (c2.H - c1.H)).Within(0.01f));
			Assert.That(resultColor.S, Is.EqualTo(c1.S + v * (c2.S - c1.S)).Within(0.01f));
			Assert.That(resultColor.V, Is.EqualTo(c1.V + v * (c2.V - c1.V)).Within(0.01f));
		}

		[Test]
		public void Brightness_adaptation_Transforms_image_file()
		{
			var bmp = new Bitmap(64, 1, PixelFormat.Format32bppArgb);
			var g = Graphics.FromImage(bmp);
			var rect = new Rectangle(default, bmp.Size);
			g.FillRectangle(new LinearGradientBrush(rect, Color.Red, Color.Red, LinearGradientMode.Horizontal), rect);


			bmp.Save(@"D:\temp\img\transform.src.png", ImageFormat.Png);

			HsvColor c1 = new HsvColor(60f, 1f, 1f);
			HsvColor c2 = new HsvColor(120f, 1f, 1f);
			var transformation = new ColorSchemeTransformation(c1, c2, bmp);

			transformation.Execute();

			bmp.Save(@"D:\temp\img\transform.png", ImageFormat.Png);
		}
	}
}
