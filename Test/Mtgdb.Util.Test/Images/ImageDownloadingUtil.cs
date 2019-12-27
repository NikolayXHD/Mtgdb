using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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

		[TestCase("c19 ", false, Clients.Scryfall)]
		[TestCase("cmb1", false, Clients.Scryfall)]
		[TestCase("eld ", false, Clients.Scryfall)]
		[TestCase("gn2 ", false, Clients.Scryfall)]
		[TestCase("ha1 ", false, Clients.Scryfall)]
		[TestCase("peld", false, Clients.Scryfall)]
		[TestCase("ptg ", false, Clients.Scryfall)]
		[TestCase("puma", false, Clients.Scryfall)]
		[TestCase("hho ", false, Clients.Scryfall)]
		public async Task DownloadGathererImages(string setCode, bool useCustomSet, Clients client)
		{
			setCode = setCode.Trim();
			var repo = new CardRepository();
			if (useCustomSet)
			{
				repo.SetsFile = null;
				repo.CustomSetCodes = new[] {setCode};
			}

			repo.FilterSetCode = code => Str.Equals(code, setCode);

			repo.LoadFile();
			repo.Load();

			var set = repo.SetsByCode[setCode];
			var setDirectory = Path.Combine(OriginalDir, set.Code);
			var setDirectoryPng = setDirectory + ".png";
			Directory.CreateDirectory(setDirectoryPng);

			foreach (var card in set.Cards)
			{
				string targetFile =
					Path.Combine(setDirectoryPng, card.ImageName + ".png");
				string processedFile =
					Path.Combine(setDirectory, card.ImageName + ".jpg");

				if (File.Exists(targetFile) || File.Exists(processedFile))
					continue;

				await _clientFactory[client]().DownloadCardImage(card, targetFile, CancellationToken.None);
			}
		}

		[TestCase("c19 ", true, false, false)]
		[TestCase("cmb1", true, false, false)]
		[TestCase("eld ", true, false, false)]
		[TestCase("gn2 ", true, false, false)]
		[TestCase("ha1 ", true, false, false)]
		[TestCase("peld", true, false, false)]
		[TestCase("ptg ", true, false, false)]
		[TestCase("puma", true, false, false)]
		[TestCase("hho ", true, false, false)]
		public void PreProcessImages(string set, bool fromPng, bool createZoom, bool keepExisting)
		{
			var smallDir = OriginalDir;
			var zoomDir = PreprocessedDir;
			set = set.Trim();
			var smallJpgDir = Path.Combine(smallDir, set);
			var zoomJpgDir = Path.Combine(zoomDir, set);

			if (!fromPng)
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
				var pngSubdir = set + ".png";
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

		// [TestCase(
		// 	HtmlDir + @"\eld_v2_Card_Image_Gallery_MAGIC_THE GATHERING.html",
		// 	GathererOriginalDir + @"\eld.png")]
		[TestCase(
			HtmlDir + @"\Throne of Eldraine Variants _ MAGIC_ THE GATHERING.html",
			OriginalDir + @"\celd.png")]
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

		private ImageCodecInfo _jpegCodec;
		private EncoderParameters _jpegEncoderParams;

		private const string OriginalDir =
			@"D:\Distrib\games\mtg\Gatherer.Original";

		private const string PreprocessedDir =
			@"D:\Distrib\games\mtg\Gatherer.PreProcessed";

		private const string HtmlDir = @"D:\temp\html";

		private static readonly Regex _imgTagPattern =
			new Regex(
				@"<img alt=""(?<name>[^""]+)"" src=""[^""]+\/(?<file>[^""]+)""");

		private static readonly Dictionary<Clients, Func<ImageDownloaderBase>> _clientFactory =
			new Dictionary<Clients, Func<ImageDownloaderBase>>
			{
				[Clients.Scryfall] = () => new ScryfallClient(),
				[Clients.Gatherer] = () => new GathererClient()
			};

		public enum Clients
		{
			Scryfall, Gatherer
		}
	}
}
