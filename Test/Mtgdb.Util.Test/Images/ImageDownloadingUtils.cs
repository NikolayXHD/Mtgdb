using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Mtgdb.ImageProcessing;
using Mtgdb.Test;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class ImageDownloadingUtils : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			_jpegCodec = codecs.First(_ => _.MimeType == "image/jpeg");
			_jpegEncoderParams = new EncoderParameters
			{
				Param =
				{
					[0] = new EncoderParameter(Encoder.Quality, 90L)
				}
			};

			LoadCards();
		}

		[TestCase("A25")]
		public void DownloadGathererImages(string setCode)
		{
			using (var client = new GathererClient())
			{
				var set = Repo.SetsByCode[setCode];
				foreach (var card in set.Cards)
					client.DownloadCardImage(card, GathererDir);
			}
		}

		[TestCase("xln")]
		public void DownloadMagicspoilerImages(string setCode)
		{
			using (var client = new MagicspoilerClient())
			{
				var set = Repo.SetsByCode[setCode];
				client.DownloadSet(set, MagicspoilerDir);
			}
		}

		[TestCase(GathererDir, GathererPreprocessedDir, "A25.large", PublishedDir, "A25")]
		public void PreProcessImages(
			string sourceDir,
			string targetDir,
			string subdir,
			string publishedDir,
			string publishedSubdir)
		{
			var sourceImages = Directory.GetFiles(Path.Combine(sourceDir, subdir)).ToArray();
			var publishDirectory = Path.Combine(PublishedDir, publishedSubdir);

			foreach (var sourceImage in sourceImages)
			{
				var targetImage = sourceImage
					.Replace(sourceDir, targetDir);

				if (!File.Exists(targetImage))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(targetImage));
					WaifuScaler.Scale(sourceImage, targetImage);
				}

				convertToJpg(targetImage, publishDirectory);
			}
		}

		//[TestCase(GathererPreprocessedDir, "A25.large", "A25")]
		[TestCase(BackupDir, "A25.png", "A25.jpg")]
		public void ConvertToJpg(string dir, string sourceSubdir, string targetSubdir)
		{
			var sourceImages = Directory.GetFiles(Path.Combine(dir, sourceSubdir)).ToArray();

			var targetDir = Path.Combine(dir, targetSubdir);
			Directory.CreateDirectory(targetDir);

			foreach (var sourceImage in sourceImages)
				convertToJpg(sourceImage, targetDir);
		}

		private void convertToJpg(string sourceImage, string targetDir)
		{
			var targetImage = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(sourceImage) + ".jpg");

			if (File.Exists(targetImage))
				return;

			new Bitmap(sourceImage).Save(targetImage, _jpegCodec, _jpegEncoderParams);
		}


		private ImageCodecInfo _jpegCodec;
		private EncoderParameters _jpegEncoderParams;

		private const string GathererDir = @"D:\Distrib\games\mtg\Gatherer.Original";
		private const string GathererPreprocessedDir = @"D:\Distrib\games\mtg\Gatherer.PreProcessed";
		private const string PublishedDir = @"D:\Distrib\games\mtg\Mega\Mtgdb.Pictures\mq";
		private const string BackupDir = @"D:\Distrib\games\mtg\.bak\Gatherer.Original";

		private const string MagicspoilerDir = @"D:\Distrib\games\mtg\magicspoiler.original";
	}
}