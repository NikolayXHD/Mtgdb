using System.Drawing;
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
	}
}
