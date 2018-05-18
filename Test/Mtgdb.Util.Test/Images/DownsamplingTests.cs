using System.Drawing;
using System.IO;
using ImageMagick;
using Mtgdb.Controls;
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

	[TestFixture]
	public class GraphicsTransformationTest
	{
		[Test]
		public void CreateImage()
		{
			var image = new Bitmap(200, 200);
			using (var g = Graphics.FromImage(image))
			{
				g.RotateTransform(90f);

				for (int i = -5; i <= 5; i ++)
					for (int j = -5; j <= 5; j++)
						g.DrawString($"{i}/{j}", new Font(FontFamily.GenericMonospace, 10), new SolidBrush(Color.Black), i * 50, j * 50);

				g.RotateTransform(-90f);
			}

			image.Save("d:\\temp\\transform.bmp");
		}
	}
}
