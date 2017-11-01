using System.IO;
using ImageMagick;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class DownsamplingTests
	{
		[TestCase(@"D:\Distrib\games\mtg\Mega\XLHQ\XLN - Ixalan\300DPI Cards\Adanto, the First Fort.xlhq.jpg")]
		public void Resample(string file)
		{
			string name = Path.GetFileNameWithoutExtension(file);
			string targetDir = "D:\\temp\\img\\";

			using (var image = new MagickImage(file))
			{
				image.Resize(new Percentage(50));
				image.Write(targetDir + name + "resized.jpg");
			}

			using (var image = new MagickImage(file))
			{
				image.Scale(new Percentage(50));
				image.Write(targetDir + name + "scaled.jpg");
			}
		}
	}
}
