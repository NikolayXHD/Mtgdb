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
using JetBrains.Annotations;
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
			var codecs = ImageCodecInfo.GetImageEncoders();
			_jpegCodec = codecs.First(_ => _.MimeType == "image/jpeg");
			_jpegEncoderParams = new EncoderParameters
			{
				Param =
				{
					[0] = new EncoderParameter(Encoder.Quality, 90L)
				}
			};
		}

		[TestCase(null, false, Clients.Scryfall, TokensSubdir)]
		public async Task DownloadGathererImages(string setCode, bool useCustomSet, Clients client, string type)
		{
			var repo = new CardRepository(new CardFormatter());
			if (setCode != null)
			{
				if (useCustomSet)
				{
					repo.SetsFile = null;
					repo.CustomSetCodes = new[] {setCode.Trim()};
				}

				repo.FilterSetCode = code => Str.Equals(code, setCode.Trim());
			}

			repo.LoadFile();
			repo.Load();

			foreach (Set set in repo.SetsByCode.Values)
			{
				if (setCode != null && Str.Equals(set.Code, setCode.Trim()))
					continue;

				foreach ((string typeSubdir, var listGetter) in _listBySubdir)
				{
					if (type != null && type != typeSubdir)
						continue;

					var cards = listGetter(set);
					if (cards.Count == 0)
						continue;

					string setSubdir = set.Code + (Str.Equals(set.Code, "con") ? " escape" : string.Empty);
					string setDirectory = Path.Combine(OriginalDir, typeSubdir, setSubdir);
					string setDirectoryPng = setDirectory + ".png";

					bool dirExisted = Directory.Exists(setDirectoryPng);
					if (!dirExisted)
						Directory.CreateDirectory(setDirectoryPng);

					foreach (var card in cards)
					{
						string targetFile = Path.Combine(setDirectoryPng, card.ImageName + ".png");
						string processedFile = Path.Combine(setDirectory, card.ImageName + ".jpg");

						if (_scope?.Invoke(card) == false || File.Exists(targetFile) || File.Exists(processedFile))
							continue;

						await _clientFactory[client]().DownloadCardImage(card, targetFile, CancellationToken.None);
					}

					if (!dirExisted && Directory.GetFileSystemEntries(setDirectoryPng).Length == 0)
						Directory.Delete(setDirectoryPng);
				}
			}
		}

		[TestCase(
			null,
			true,
			false,
			false,
			TokensSubdir)]
		public void PreProcessImages(string setCode, bool fromPng, bool createZoom, bool keepExisting, string type)
		{
			const string smallDir = OriginalDir;
			const string zoomDir = PreprocessedDir;

			foreach ((string typeSubdir, _) in _listBySubdir)
			{
				if (type != null && type != typeSubdir)
					continue;

				string typeDir = Path.Combine(smallDir, typeSubdir);
				foreach (string setDir in Directory.GetDirectories(typeDir))
				{
					string subdir = Path.GetFileName(setDir) ??
						throw new ApplicationException($"Null subdirectory in {typeDir}");

					string setSubdir = Regex.Replace(subdir, @"\.png$", string.Empty);

					if (setCode != null && !Str.Equals(setSubdir, setCode.Trim()))
						continue;

					string smallJpgDir = Path.Combine(smallDir, typeSubdir, setSubdir);
					string zoomJpgDir = Path.Combine(zoomDir, typeSubdir, setSubdir);

					if (!fromPng)
					{
						if (!createZoom)
							return;

						Directory.CreateDirectory(zoomJpgDir);
						foreach (string smallImg in Directory.GetFiles(smallJpgDir))
						{
							string zoomImg = smallImg.Replace(smallDir, zoomDir);
							if (keepExisting && File.Exists(zoomImg))
								continue;
							WaifuScaler.Scale(smallImg, zoomImg);
						}
					}
					else
					{
						string smallPngDir = Path.Combine(smallDir, typeSubdir, setSubdir + ".png");
						string zoomPngDir = Path.Combine(zoomDir, typeSubdir, setSubdir + ".png");

						var dirs = new List<(string pngDir, string jpgDir)>
						{
							(pngDir: smallPngDir, jpgDir: smallJpgDir)
						};

						if (createZoom)
						{
							Directory.CreateDirectory(zoomPngDir);
							foreach (string smallImg in Directory.GetFiles(smallPngDir))
							{
								string zoomImg = smallImg.Replace(smallDir, zoomDir);
								string convertedImg = zoomImg
									.Replace(smallDir, zoomJpgDir)
									.Replace(".png", ".jpg");

								if (keepExisting && F.Seq(zoomImg, convertedImg).Any(File.Exists))
									continue;

								WaifuScaler.Scale(smallImg, zoomImg);
							}

							dirs.Add((pngDir: zoomPngDir, jpgDir: zoomJpgDir));
						}

						foreach ((string pngDir, string jpgDir) in dirs)
						{
							Directory.CreateDirectory(jpgDir);

							var pngImages = Directory.GetFiles(pngDir).ToArray();

							foreach (string sourceImage in pngImages)
								convertToJpg(sourceImage, jpgDir, keepExisting);
						}
					}
				}
			}
		}

		[TestCase(HtmlDir + @"\Throne of Eldraine Variants _ MAGIC_ THE GATHERING.html", OriginalDir + @"\celd.png")]
		public void RenameWizardsWebpageImages(string htmlPath, string targetDir)
		{
			string htmlFileName = Path.GetFileNameWithoutExtension(htmlPath);
			string directoryName = Path.GetDirectoryName(htmlPath) ??
				throw new ArgumentException(htmlPath, nameof(htmlPath));
			string filesDirectory = Path.Combine(directoryName, htmlFileName + "_files");

			string content = File.ReadAllText(htmlPath);
			var matches = _imgTagPattern.Matches(content);

			Directory.CreateDirectory(targetDir);
			foreach (Match match in matches)
			{
				string originalFileName = match.Groups["file"].Value;
				string ext = Path.GetExtension(originalFileName);

				string filePath = Path.Combine(filesDirectory, originalFileName);

				string name = HttpUtility.HtmlDecode(match.Groups["name"].Value)
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



		private void convertToJpg([NotNull] string sourceImage, string targetDir, bool keepExisting)
		{
			string targetImage = Path.Combine(targetDir,
				Path.GetFileNameWithoutExtension(sourceImage) + ".jpg");

			if (keepExisting && File.Exists(targetImage))
				return;

			using var original = new Bitmap(sourceImage);
			new BmpAlphaToBackgroundColorTransformation(original, Color.White)
				.Execute();

			original.Save(targetImage, _jpegCodec, _jpegEncoderParams);
		}

		private ImageCodecInfo _jpegCodec;
		private EncoderParameters _jpegEncoderParams;

		private const string OriginalDir =
			@"D:\Distrib\games\mtg\Gatherer.Original";

		private const string PreprocessedDir =
			@"D:\Distrib\games\mtg\Gatherer.PreProcessed";

		private const string CardsSubdir = "cards";
		private const string TokensSubdir = "tokens";

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

		private static readonly Dictionary<string, Func<Set, IList<Card>>> _listBySubdir =
			new Dictionary<string, Func<Set,IList<Card>>>
			{
				[CardsSubdir] = set => set.ActualCards,
				[TokensSubdir] = set => set.Tokens,
			};

		public enum Clients
		{
			Scryfall, Gatherer
		}

		private static readonly Func<Card, bool> _scope = card => card.Side == "b";
	}
}
