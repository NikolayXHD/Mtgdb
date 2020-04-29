using System.Drawing;
using ImageMagick;
using Mtgdb.Dev;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class DownsamplingTests
	{
		// ReSharper disable StringLiteralTypo
		[Test]
		// ReSharper restore StringLiteralTypo
		public void Resample()
		{
			FsPath file = DevPaths.XlhqDir.Join("XLN - Ixalan", "300DPI Cards", "Adanto, the First Fort.xlhq.jpg");
			string name = file.Basename(extension: false);
			FsPath targetDir = DevPaths.DataDrive.Join("temp", "img");

			using (var image = new MagickImage(file.Value))
			{
				image.Resize(new Percentage(50));
				image.Write(targetDir.Join(name).Concat("resized.jpg").Value);
			}

			using (var image = new MagickImage(file.Value))
			{
				image.Scale(new Percentage(50));
				image.Write(targetDir.Join(name).Concat("scaled.jpg").Value);
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
					{
						using var font = new Font(FontFamily.GenericMonospace, 10);
						g.DrawString($"{i}/{j}", font, Brushes.Black, i * 50, j * 50);
					}

				g.RotateTransform(-90f);
			}

			image.Save(DevPaths.DataDrive.Join("temp", "transform.bmp").Value);
		}
	}
}
