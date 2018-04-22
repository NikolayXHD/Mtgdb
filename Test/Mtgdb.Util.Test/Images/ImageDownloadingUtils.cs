using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

		[TestCase("DOM")]
		[TestCase("DDU")]
		public void DownloadGathererImages(string setCode)
		{
			var client = new GathererClient();
			var set = Repo.SetsByCode[setCode];
			foreach (var card in set.Cards)
				client.DownloadCardImage(card, GathererOriginalDir);
		}

		[TestCase("xln")]
		public void DownloadMagicspoilerImages(string setCode)
		{
			var client = new MagicspoilerClient();
			var set = Repo.SetsByCode[setCode];
			client.DownloadSet(set, MagicspoilerDir);
		}

		[TestCase(GathererOriginalDir, GathererPreprocessedDir, "DDU png")]
		[TestCase(GathererOriginalDir, GathererPreprocessedDir, "DOM png large")]
		public void PreProcessImages(
			string sourceDir,
			string targetDir,
			string subdir)
		{
			string sourceSubdir = Path.Combine(sourceDir, subdir);
			string targetSubdir = Path.Combine(targetDir, subdir);

			Directory.CreateDirectory(targetSubdir);

			var sourceImages = Directory.GetFiles(sourceSubdir);

			foreach (var sourceImage in sourceImages)
			{
				var targetImage = sourceImage.Replace(sourceDir, targetDir);

				if (!File.Exists(targetImage))
					WaifuScaler.Scale(sourceImage, targetImage);
			}
		}

		[TestCase(GathererOriginalDir, "DOM png", PublishedSmallDir, "DOM")]
		[TestCase(GathererPreprocessedDir, "DOM png", PublishedZoomDir, "DOM")]
		public void ConvertToJpg(string dir, string subdir, string targetDir, string targetSubdir)
		{
			var sourceImages = Directory.GetFiles(Path.Combine(dir, subdir)).ToArray();

			var target = Path.Combine(targetDir, targetSubdir);
			Directory.CreateDirectory(target);

			foreach (var sourceImage in sourceImages)
				convertToJpg(sourceImage, target);
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

		private const string GathererOriginalDir = @"D:\Distrib\games\mtg\Gatherer.Original";
		private const string GathererPreprocessedDir = @"D:\Distrib\games\mtg\Gatherer.PreProcessed";
		private const string PublishedZoomDir = @"D:\Distrib\games\mtg\Mega\Mtgdb.Pictures\mq";
		private const string PublishedSmallDir = @"D:\Distrib\games\mtg\Mega\Mtgdb.Pictures\lq";
		private const string BackupDir = @"D:\Distrib\games\mtg\.bak\Gatherer.Original";

		private const string MagicspoilerDir = @"D:\Distrib\games\mtg\magicspoiler.original";
	}
}