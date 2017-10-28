using System.Drawing;
using System.IO;
using ImageMagick;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DownsamplingTests
	{
		[TestCase("F:\\Repo\\Git\\Mtgdb\\Mtgdb.Gui\\Resources\\r.png")]
		public void Resample(string file)
		{
			using (var image = new MagickImage(file))
			{
				image.Resize(new Percentage(50));
				image.Write("D:\\temp\\img\\" + Path.GetFileName(file));
			}
		}
	}
}
