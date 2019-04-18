using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class ImageDownloadingUtil
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
		}

		[TestCase("RNA", true)]
		public void DownloadGathererImages(string setCode, bool useCustomSet)
		{
			var repo = new CardRepository();
			if (useCustomSet)
			{
				repo.SetsFile = null;
				repo.CustomSetCodes = new[] { setCode };
			}

			repo.FilterSetCode = code => Str.Equals(code, setCode);

			repo.LoadFile();
			repo.Load();

			var set = repo.SetsByCode[setCode];
			var client = new GathererClient();

			var setDirectory = Path.Combine(GathererOriginalDir, set.Code);
			Directory.CreateDirectory(setDirectory);

			foreach (var card in set.Cards)
			{
				if (!card.MultiverseId.HasValue)
					continue;

				string targetFile = Path.Combine(setDirectory, card.ImageName + ".png");
				string processedFile = Path.Combine(setDirectory, card.ImageName + ".jpg");

				if (File.Exists(targetFile) || File.Exists(processedFile))
					continue;

				client.DownloadCardImage(card.MultiverseId.Value, targetFile);
			}
		}

		// [TestCase("xln")]
		// public void DownloadMagicspoilerImages(string setCode)
		// {
		// 	var client = new MagicspoilerClient();
		// 	var set = Repo.SetsByCode[setCode];
		// 	client.DownloadSet(set, MagicspoilerDir);
		// }

		// private const string MagicspoilerDir = @"D:\Distrib\games\mtg\magicspoiler.original";

		[TestCase(
			HtmlDir + @"\War of the Spark   MAGIC  THE GATHERING.htm",
			GathererOriginalDir + @"\war.png")]
		public void RenameWizardsWebpageImages(string htmlPath, string targetDir)
		{
			var htmlFileName = Path.GetFileNameWithoutExtension(htmlPath);
			var directoryName = Path.GetDirectoryName(htmlPath);
			var filesDirectory = Path.Combine(directoryName, htmlFileName + "_files");

			var content = File.ReadAllText(htmlPath);
			var matches = _imgTagPattern.Matches(content);

			Directory.CreateDirectory(targetDir);
			foreach (Match match in matches)
			{
				string originalFileName = match.Groups["file"].Value;
				string ext = Path.GetExtension(originalFileName);

				var filePath = Path.Combine(filesDirectory, originalFileName);

				var name = HttpUtility.HtmlDecode(match.Groups["name"].Value).Replace(" // ", "");

				string defaultTargetPath = Path.Combine(targetDir, name + ext);

				bool defaultTargetExists = File.Exists(defaultTargetPath);

				if (defaultTargetExists || File.Exists(getTargetPath(1)))
				{
					if (defaultTargetExists)
						File.Move(defaultTargetPath, getTargetPath(1));

					for (int i = 2; i < 12; i++)
					{
						string targetPath = getTargetPath(i);
						if (!File.Exists(targetPath))
						{
							File.Copy(filePath, targetPath, overwrite: false);
							break;
						}
					}
				}
				else
				{
					File.Copy(filePath, defaultTargetPath, overwrite: false);
				}

				string getTargetPath(int num) =>
					Path.Combine(targetDir, name + num + ext);
			}
		}

		[TestCase(GathererOriginalDir, GathererPreprocessedDir, "war.png", "war")]
		[TestCase(GathererOriginalDir, GathererPreprocessedDir, "pwar.png", "pwar")]
		[TestCase(GathererOriginalDir, GathererPreprocessedDir, "mh1.png", "mh1")]
		public void PreProcessImages(
			string smallDir,
			string zoomDir,
			string pngSubdir,
			string jpgSubdir)
		{
			string smallPngDir = Path.Combine(smallDir, pngSubdir);
			string zoomPngDir = Path.Combine(zoomDir, pngSubdir);

			Directory.CreateDirectory(zoomPngDir);
			var smallImages = Directory.GetFiles(smallPngDir);

			foreach (var smallImage in smallImages)
			{
				var zoomImage = smallImage.Replace(smallDir, zoomDir);

				if (!File.Exists(zoomImage))
					WaifuScaler.Scale(smallImage, zoomImage);
			}

			var dirs = new[]
			{
				(pngDir: smallPngDir, jpgDir: Path.Combine(smallDir, jpgSubdir)),
				(pngDir: zoomPngDir, jpgDir: Path.Combine(zoomDir, jpgSubdir))
			};

			foreach (var _ in dirs)
			{
				Directory.CreateDirectory(_.jpgDir);

				var pngImages = Directory.GetFiles(_.pngDir).ToArray();

				foreach (var sourceImage in pngImages)
					convertToJpg(sourceImage, _.jpgDir);
			}
		}

		private void convertToJpg(string sourceImage, string targetDir)
		{
			var targetImage = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(sourceImage) + ".jpg");

			// if (File.Exists(targetImage))
			// 	return;

			using (var original = new Bitmap(sourceImage))
			{
				new BmpAlphaToBackgroundColorTransformation(original, Color.White)
					.Execute();

				original.Save(targetImage, _jpegCodec, _jpegEncoderParams);
			}
		}

		private ImageCodecInfo _jpegCodec;
		private EncoderParameters _jpegEncoderParams;

		private const string GathererOriginalDir = @"D:\Distrib\games\mtg\Gatherer.Original";
		private const string GathererPreprocessedDir = @"D:\Distrib\games\mtg\Gatherer.PreProcessed";
		private const string HtmlDir = @"D:\temp\html";

		private static readonly Regex _imgTagPattern = new Regex(@"<img alt=""(?<name>[^""]+)"" src=""[^""]+\/(?<file>[^""]+)""");
	}
}