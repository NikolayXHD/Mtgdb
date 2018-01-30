using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Mtgdb.ImageProcessing;
using Mtgdb.Test;
using NLog;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class ImageDownloadingUtils: TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadModules();
			LoadCards();
			LogManager.Flush();
		}

		[TestCase("RIX"), Order(1)]
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

		// [TestCase(MagicspoilerDir, MagicspoilerPreprocessedDir, "XLN")]
		// [TestCase(GathererDir, GathererPreprocessedDir, "RIX.large"), Order(2)]
		[TestCase(GathererDir, GathererPreprocessedDir, "V17"), Order(2)]
		public void PreProcessImages(string sourceDir, string targetDir, string subdir)
		{
			var sourceImages = Directory.GetFiles(Path.Combine(sourceDir, subdir)).ToArray();

			foreach (var sourceImage in sourceImages)
			{
				var targetImage = sourceImage
					.Replace(sourceDir, targetDir);

				if (File.Exists(targetImage))
					continue;

				Directory.CreateDirectory(Path.GetDirectoryName(targetImage));
				WaifuScaler.Scale(sourceImage, targetImage);
			}
		}

		//[TestCase(GathererDir, "RIX.large", "RIX.jpg"), Order(3)]
		//[TestCase(GathererDir, "RIX", "RIX.jpg")]
		//[TestCase(GathererPreprocessedDir, "RIX.large", "RIX.jpg")]
		//[TestCase(GathererPreprocessedDir, "RIX", "RIX.jpg")]
		[TestCase(GathererDir, "V17", "V17.jpg")]
		[TestCase(GathererPreprocessedDir, "V17", "V17.jpg")]
		public void ConvertToJpg(string dir, string sourceSubdir, string targetSubdir)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
			var codec = codecs.First(_ => _.MimeType == "image/jpeg");
			var encoderParams = new EncoderParameters
			{
				Param = { [0] = new EncoderParameter(Encoder.Quality, (long) 90) }
			};

			var sourceImages = Directory.GetFiles(Path.Combine(dir, sourceSubdir)).ToArray();

			var targetDir = Path.Combine(dir, targetSubdir);
			Directory.CreateDirectory(targetDir);

			foreach (var sourceImage in sourceImages)
			{
				var targetImage = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(sourceImage) + ".jpg");

				if (File.Exists(targetImage))
					continue;

				new Bitmap(sourceImage).Save(targetImage, codec, encoderParams);
			}
		}

		private const string GathererDir = @"D:\Distrib\games\mtg\Gatherer.Original";
		private const string GathererPreprocessedDir = @"D:\Distrib\games\mtg\Gatherer.PreProcessed";

		private const string MagicspoilerDir = @"D:\Distrib\games\mtg\magicspoiler.original";
	}
}