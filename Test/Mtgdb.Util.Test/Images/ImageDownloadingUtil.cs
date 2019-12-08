using System.Collections.Generic;
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

		[TestCase("eld", ".png", false)]
		public void DownloadGathererImages(string setCode, string extension, bool useCustomSet)
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

				string targetFile =
					Path.Combine(setDirectory, card.ImageName + extension);
				string processedFile =
					Path.Combine(setDirectory, card.ImageName + ".jpg");

				if (File.Exists(targetFile) || File.Exists(processedFile))
					continue;

				client.DownloadCardImage(card.MultiverseId.Value, targetFile);
			}
		}

//		[TestCase(
//			HtmlDir + @"\eld_v2_Card_Image_Gallery_MAGIC_THE GATHERING.html",
//			GathererOriginalDir + @"\eld.png")]
		[TestCase(
			HtmlDir + @"\Throne of Eldraine Variants _ MAGIC_ THE GATHERING.html",
			GathererOriginalDir + @"\celd.png")]
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

				var name = HttpUtility.HtmlDecode(match.Groups["name"].Value)
					.Replace(" // ", "");

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

		// [TestCase(GathererOriginalDir, GathererPreprocessedDir, /* png subdir */ "war.png", "war", /* createZoom */ false)]
		// [TestCase(GathererOriginalDir, GathererPreprocessedDir, /* png subdir */ null, "ss2", /* createZoom */ true)]
		// [TestCase(GathererOriginalDir, GathererPreprocessedDir, /* png subdir */ null, "htr17", /* createZoom */ true)]

		[TestCase(
			GathererOriginalDir, GathererPreprocessedDir, "eld.png", "eld",
			/* createZoom */ true,
			/*keepExisting*/ false
		)]
		[TestCase(
			GathererOriginalDir, GathererPreprocessedDir, "celd.png", "celd",
			/* createZoom */ true,
			/*keepExisting*/ false
		)]
		public void PreProcessImages(string smallDir, string zoomDir,
			string pngSubdir, string jpgSubdir,
			bool createZoom,
			bool keepExisting)
		{
			var smallJpgDir = Path.Combine(smallDir, jpgSubdir);
			var zoomJpgDir = Path.Combine(zoomDir, jpgSubdir);

			if (pngSubdir == null)
			{
				if (!createZoom)
					return;

				Directory.CreateDirectory(zoomJpgDir);
				foreach (var smallImg in Directory.GetFiles(smallJpgDir))
				{
					var zoomImg = smallImg.Replace(smallDir, zoomDir);
					if (keepExisting && File.Exists(zoomImg))
						continue;
					WaifuScaler.Scale(smallImg, zoomImg);
				}
			}
			else
			{
				string smallPngDir = Path.Combine(smallDir, pngSubdir);
				string zoomPngDir = Path.Combine(zoomDir, pngSubdir);

				var dirs = new List<(string pngDir, string jpgDir)>
				{
					(pngDir: smallPngDir, jpgDir: smallJpgDir)
				};

				if (createZoom)
				{
					Directory.CreateDirectory(zoomPngDir);
					foreach (var smallImg in Directory.GetFiles(smallPngDir))
					{
						var zoomImg = smallImg.Replace(smallDir, zoomDir);
						var convertedImg = zoomImg
							.Replace(smallDir, zoomJpgDir)
							.Replace(".png", ".jpg");

						if (keepExisting && F.Seq(zoomImg, convertedImg).Any(File.Exists))
							continue;

						WaifuScaler.Scale(smallImg, zoomImg);
					}

					dirs.Add((pngDir: zoomPngDir, jpgDir: zoomJpgDir));
				}

				foreach (var (pngDir, jpgDir) in dirs)
				{
					Directory.CreateDirectory(jpgDir);

					var pngImages = Directory.GetFiles(pngDir).ToArray();

					foreach (var sourceImage in pngImages)
						convertToJpg(sourceImage, jpgDir);
				}
			}

			void convertToJpg(string sourceImage, string targetDir)
			{
				var targetImage = Path.Combine(targetDir,
					Path.GetFileNameWithoutExtension(sourceImage) + ".jpg");

				if (keepExisting && File.Exists(targetImage))
					return;

				using var original = new Bitmap(sourceImage);
				new BmpAlphaToBackgroundColorTransformation(original, Color.White)
					.Execute();

				original.Save(targetImage, _jpegCodec, _jpegEncoderParams);
			}
		}

		private ImageCodecInfo _jpegCodec;
		private EncoderParameters _jpegEncoderParams;

		private const string GathererOriginalDir =
			@"D:\Distrib\games\mtg\Gatherer.Original";

		private const string GathererPreprocessedDir =
			@"D:\Distrib\games\mtg\Gatherer.PreProcessed";

		private const string HtmlDir = @"D:\temp\html";

		private static readonly Regex _imgTagPattern =
			new Regex(
				@"<img alt=""(?<name>[^""]+)"" src=""[^""]+\/(?<file>[^""]+)""");
	}
}