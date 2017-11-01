using System.Drawing;
using System.IO;
using System.Linq;
using Mtgdb.ImageProcessing;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class GathererImagePreProcessor
	{
		private static readonly Size _croppedSize = new Size(393, 564);
		private static readonly Point _croppedLocation = new Point(17, 17);

		private static readonly Size _croppedSizeSmall = new Size(198, 284);
		private static readonly Point _croppedLocationSmall = new Point(9, 9);

		private const string GathererOriginalDir = @"D:\Distrib\games\mtg\Gatherer.Original";
		private const string GathererPreprocessedDir = @"D:\Distrib\games\mtg\Gatherer.PreProcessed";
		private static readonly string FramedImagesDir = @"D:\Distrib\games\mtg\Gatherer.Framed";

		[TestCase("cma")]
		public void DownloadImages(string setCode)
		{
			TestLoader.LoadModules();
			TestLoader.LoadCardRepository();
			
			using (var client = new GathererClient())
			{
				var set = TestLoader.CardRepository.SetsByCode[setCode];
				foreach (var card in set.Cards)
					client.DownloadCard(card, GathererOriginalDir);
			}
		}

		[TestCase("CMA", "*.png")]
		[TestCase("C17", "*.png")]
		[TestCase("XLN", "*.png")]
		public void PreProcessImages(string subdir, string pattern)
		{
			var sourceImages = Directory.GetFiles(
				Path.Combine(GathererOriginalDir, subdir),
				pattern,
				SearchOption.AllDirectories)
				.ToArray();

			foreach (var sourceImage in sourceImages)
			{
				var targetImage = sourceImage
					.Replace(GathererOriginalDir, GathererPreprocessedDir);

				if (File.Exists(targetImage))
					continue;

				Directory.CreateDirectory(Path.GetDirectoryName(targetImage));
				WaifuScaler.Scale(sourceImage, targetImage);
			}
		}

		[TestCase("CMA")]
		public void AddFrame(string setSubdir)
		{
			addFrame(
				Path.Combine("MQ", setSubdir),
				"frame.png",
				_croppedLocation,
				_croppedSize,
				Path.Combine(GathererPreprocessedDir, setSubdir));
		}
		[TestCase("CMA")]
		public void AddFrameSmall(string setSubdir)
		{
			addFrame(
				Path.Combine("LQ", setSubdir),
				"frame.small.png", 
				_croppedLocationSmall,
				_croppedSizeSmall,
				Path.Combine(GathererOriginalDir, setSubdir));
		}

		private static void addFrame(string targetSubdir, string frameFile, Point location, Size size, string originalDir)
		{
			string targetDir = Path.Combine(FramedImagesDir, targetSubdir);
			var frameImage = Image.FromFile(Path.Combine(FramedImagesDir, frameFile));

			var imageFiles = Directory.GetFiles(originalDir, "*.jpg", SearchOption.AllDirectories)
				.ToArray();

			foreach (var imageFile in imageFiles)
			{
				using (var image = new Bitmap(frameImage, frameImage.Width, frameImage.Height))
				using (var graphics = Graphics.FromImage(image))
				using (var sourceImage = Image.FromFile(imageFile))
				{
					graphics.DrawImage(sourceImage, new Rectangle(location, size));
					image.Save(Path.Combine(targetDir, Path.GetFileName(imageFile)));
				}
			}
		}
	}
}