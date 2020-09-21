using System.Drawing;
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
			Color c1 = Color.FromArgb(unchecked((int) 0xFF_FF_FF_FF));
			Color c2 = Color.FromArgb(unchecked((int) 0xFF_00_00_00));


			var transformation = new ColorSchemeTransformation(c1.ToHsv(), c2.ToHsv());
			var sourceColor = Color.FromArgb(unchecked((int) 0xFF_00_00_00));
			var transformed = transformation.TransformColor(sourceColor);

			Assert.That(transformed.ToArgb(), Is.EqualTo(c1.ToArgb()));
		}

		[Test]
		public void Black_is_transformed_to_first_color_In_byte_array()
		{
			Color c1 = Color.FromArgb(unchecked((int) 0xFF_FF_FF_FF));
			Color c2 = Color.FromArgb(unchecked((int) 0xFF_00_00_00));


			var transformation = new ColorSchemeTransformation(c1.ToHsv(), c2.ToHsv());
			var rgbValues = new byte[] { 0x00, 0x00, 0x00, 0xFF };

			transformation.Transform(rgbValues, 0);

			var expectedTransformed = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };

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
		public void Hsv_to_rgb_Is_reversible(
			[Values(0f / 360f, 60f / 360f, 120f / 360f, 180f / 360f, 240f / 360f, 300f / 360f)] float h)
		{
			var originalHsv = new HsvColor(h, 0.5f, 0.5f);
			var rgb = originalHsv.ToRgb();
			var hsv = rgb.ToHsv();
			Assert.That(hsv.H, Is.EqualTo(h).Within(0.02));
		}
	}
}
