using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Mtgdb.Controls;
using Mtgdb.Data;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class DeploymentUtils
	{
		private const string SetCodes = "pz2,g18";
		private const bool NonToken = true;
		private const bool Token = false;
		private const bool SelectSmall = true;
		private const bool SelectZoom = true;

		[Order(1)]
		// [TestCase(SetCodes, NonToken, Token)]
		public async Task DownloadGathererImages(string setCodesList, bool nonToken, bool token)
		{
			var client = new ScryfallClient();
			var setCodes = setCodesList?.Split(',');
			var repo = new CardRepository(new CardFormatter());
			if (setCodesList != null)
				repo.FilterSetCode = code => setCodes.Contains(code, Str.Comparer);

			repo.LoadFile();
			repo.Load();

			foreach (Set set in repo.SetsByCode.Values)
			{
				if (setCodes != null && !setCodes.Contains(set.Code, Str.Comparer))
					continue;

				foreach ((bool isToken, string typeSubdir, _) in getIsToken(nonToken, token))
				{
					var cards = set.List(isToken);
					if (cards.Count == 0)
						continue;

					string setSubdir = set.Code + (Str.Equals(set.Code, "con") ? " escape" : string.Empty);
					string downloadRootDir = RootDir.AddPath(_createZoom ? OriginalSubdir : PreProcessedSubdir);
					string setDirectory = Path.Combine(downloadRootDir, typeSubdir, setSubdir);
					string setDirectoryPng = setDirectory + ".png";

					bool dirExisted = Directory.Exists(setDirectoryPng);
					if (!dirExisted)
						Directory.CreateDirectory(setDirectoryPng);

					foreach (var card in cards)
					{
						string targetFile = Path.Combine(setDirectoryPng, card.ImageName + ".png");
						string processedFile = Path.Combine(setDirectory, card.ImageName + ".jpg");

						if (File.Exists(targetFile) || File.Exists(processedFile))
							continue;

						await client.DownloadCardImage(card, targetFile, CancellationToken.None);
					}

					if (!dirExisted && Directory.GetFileSystemEntries(setDirectoryPng).Length == 0)
						Directory.Delete(setDirectoryPng);
				}
			}
		}

		[Order(2)]
		// [TestCase(SetCodes,NonToken, Token)]
		public void PreProcessImages(string setCodesList, bool nonToken, bool token)
		{
			setupImageConversion();

			string smallDir = RootDir.AddPath(OriginalSubdir);
			string zoomDir = RootDir.AddPath(PreProcessedSubdir);
			string smallDirBak = BakDir.AddPath(OriginalSubdir);
			string zoomDirBak = BakDir.AddPath(PreProcessedSubdir);

			var setCodes = setCodesList?.Split(',');

			IEnumerable<string> getSetSubdirs(string typeSubdir)
			{
				string typeDir = Path.Combine(_createZoom ? smallDir : zoomDir, typeSubdir);
				return Directory.EnumerateDirectories(typeDir,
						_fromPng ? "*.png" : "*.*",
						SearchOption.TopDirectoryOnly)
					.Select(Path.GetFileName)
					.Distinct()
					.Select(subdir => Regex.Replace(subdir, @"\.png$", string.Empty));
			}

			foreach ((_, string typeSubdir, _) in getIsToken(nonToken, token))
			{
				foreach (string setSubdir in getSetSubdirs(typeSubdir))
				{
					if (setCodes != null && !setCodes.Contains(setSubdir, Str.Comparer))
						continue;

					string smallJpgDir = Path.Combine(smallDir, typeSubdir, setSubdir);
					string zoomJpgDir = Path.Combine(zoomDir, typeSubdir, setSubdir);

					if (!_fromPng)
					{
						if (!_createZoom)
							return;

						Directory.CreateDirectory(zoomJpgDir);
						foreach (string smallImg in Directory.GetFiles(smallJpgDir))
						{
							string zoomImg = smallImg.Replace(smallDir, zoomDir);
							if (_keepExisting && File.Exists(zoomImg))
								continue;
							WaifuScaler.Scale(smallImg, zoomImg);
						}
					}
					else
					{
						string setPngSubdir = setSubdir + ".png";
						string smallPngDir = Path.Combine(smallDir, typeSubdir, setPngSubdir);
						string smallPngDirBak = Path.Combine(smallDirBak, typeSubdir, setPngSubdir);
						string zoomPngDir = Path.Combine(zoomDir, typeSubdir, setPngSubdir);
						string zoomPngDirBak = Path.Combine(zoomDirBak, typeSubdir, setPngSubdir);

						var dirs = new List<(string pngDir, string pngDirBak, string jpgDir)>();
						if (_createZoom)
						{
							Directory.CreateDirectory(zoomPngDir);
							foreach (string smallImg in Directory.GetFiles(smallPngDir))
							{
								string zoomImg = smallImg.Replace(smallDir, zoomDir);
								string convertedImg = zoomImg
									.Replace(smallDir, zoomJpgDir)
									.Replace(".png", ".jpg");

								if (_keepExisting && F.Seq(zoomImg, convertedImg).Any(File.Exists))
									continue;

								WaifuScaler.Scale(smallImg, zoomImg);
							}

							dirs.Add((pngDir: smallPngDir, pngDirBak: smallPngDirBak, jpgDir: smallJpgDir));
						}

						dirs.Add((pngDir: zoomPngDir, pngDirBak: zoomPngDirBak, jpgDir: zoomJpgDir));

						foreach ((string pngDir, string pngDirBak, string jpgDir) in dirs)
						{
							Directory.CreateDirectory(jpgDir);

							var pngImages = Directory.GetFiles(pngDir).ToArray();

							foreach (string sourceImage in pngImages)
								convertToJpg(sourceImage, jpgDir, _keepExisting);

							moveDirectoryToBackup(pngDir, pngDirBak);
						}
					}
				}
			}
		}

		[Order(3)]
		//[TestCase(SetCodes, SelectSmall, SelectZoom, NonToken, Token)]
		public void SelectImages(string setCodes, bool small, bool zoom, bool nonToken, bool token)
		{
			setupExport();

			foreach ((bool isToken, _, string tokenSuffix) in getIsToken(nonToken, token))
				_export.ExportCardImages(TargetDir,
					small,
					zoom,
					setCodes,
					LqDir + tokenSuffix,
					MqDir + tokenSuffix,
					true,
					isToken);
		}

		[Order(4)]
		[TestCase(SetCodes, SelectSmall, SelectZoom, NonToken, Token)]
		public void SignImages(string setCodes, bool small, bool zoom, bool nonToken, bool token)
		{
			foreach (string qualityDir in getQualities(small, zoom))
			foreach ((_, _, string tokenSuffix) in getIsToken(nonToken, token))
			{
				string outputFile = getSignatureFile(qualityDir, tokenSuffix);
				string packagePath = TargetDir.AddPath(qualityDir + tokenSuffix);
				new ImageDirectorySigner().SignFiles(packagePath, outputFile, setCodes);
			}
		}

		[Order(5)]
		[TestCase(SetCodes, SelectSmall, SelectZoom, NonToken, Token)]
		public void ZipImages(string setCodes, bool small, bool zoom, bool nonToken, bool token)
		{
			foreach (string qualityDir in getQualities(small, zoom))
			foreach ((_, _, string tokenSuffix) in getIsToken(nonToken, token))
			{
				var list = setCodes?.Split(',').ToHashSet(Str.Comparer);
				string compressedRoot = TargetDir.AddPath(qualityDir + tokenSuffix + ZipDirSuffix);

				Directory.CreateDirectory(compressedRoot);

				string sourceRoot = TargetDir.AddPath(qualityDir + tokenSuffix);
				foreach (var subdir in new DirectoryInfo(sourceRoot).EnumerateDirectories())
				{
					if (list?.Contains(subdir.Name) == false)
						continue;

					var targetFile = new FileInfo(compressedRoot.AddPath(subdir.Name + SevenZipExtension));
					if (targetFile.Exists)
						targetFile.Delete();

					new SevenZip(false).Compress(subdir.FullName, targetFile.FullName)
						.Should().BeTrue();
				}

				FileInfo signatureFile = new FileInfo(getSignatureFile(qualityDir, tokenSuffix));
				FileInfo compressedSignatureFile = new FileInfo(Path.Combine(
					signatureFile.DirectoryName ?? throw new ApplicationException("Invalid signature file path " + signatureFile.FullName),
					Path.GetFileNameWithoutExtension(signatureFile.Name) + SevenZipExtension));

				if (compressedSignatureFile.Exists)
					compressedSignatureFile.Delete();

				new SevenZip(false).Compress(signatureFile.FullName, compressedSignatureFile.FullName).Should().BeTrue();
			}
		}



		private static void moveDirectoryToBackup(string dir, string dirBak)
		{
			if (Directory.Exists(dirBak))
				Directory.Delete(dirBak);
			Directory.CreateDirectory(dirBak.Parent());
			Directory.Move(dir, dirBak);
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

		private static IEnumerable<string> getQualities(bool small, bool zoom)
		{
			if (small)
				yield return LqDir;

			if (zoom)
				yield return MqDir;
		}

		private static IEnumerable<(bool IsToken, string SourceSubdir, string TargetDirSuffix)> getIsToken(bool nonToken, bool token)
		{
			if (nonToken)
				yield return (false, CardsSubdir, string.Empty);

			if (token)
				yield return (true, TokensSubdir, TokenSuffix);
		}

		private static string getSignatureFile(string qualityDir, string tokenSuffix)
		{
			string outputFile = TargetDir
				.AddPath(qualityDir + tokenSuffix + ListDirSuffix)
				.AddPath(Signer.SignaturesFile);
			return outputFile;
		}



		private void setupExport()
		{
			if (_setupExportComplete)
				return;

			var kernel = new StandardKernel();
			kernel.Load<CoreModule>();
			kernel.Load<DalModule>();
			kernel.Load<UtilModule>();

			var repo = kernel.Get<CardRepository>();
			repo.LoadFile();
			repo.Load();

			var imgRepo = kernel.Get<ImageRepository>();

			imgRepo.LoadFiles(Sequence.From("dev", "xlhq"));
			imgRepo.LoadSmall();
			imgRepo.LoadZoom();

			kernel.Bind<ImageExport>().ToSelf();
			_export = kernel.Get<ImageExport>();
			_setupExportComplete = true;
		}

		private void setupImageConversion()
		{
			if (_setupConversionComplete)
				return;

			BitmapExtensions.CustomScaleStrategy = ImageMagickScaler.Scale;

			var codecs = ImageCodecInfo.GetImageEncoders();
			_jpegCodec = codecs.First(_ => _.MimeType == "image/jpeg");
			_jpegEncoderParams = new EncoderParameters
			{
				Param =
				{
					[0] = new EncoderParameter(Encoder.Quality, 90L)
				}
			};

			_setupConversionComplete = true;
		}



		private static readonly bool _keepExisting = false;
		private static readonly bool _fromPng = true;
		private static readonly bool _createZoom = false;

		private ImageCodecInfo _jpegCodec;
		private EncoderParameters _jpegEncoderParams;
		private ImageExport _export;

		private const string BakDir = @"D:\Distrib\games\mtg\.bak";
		private const string RootDir = @"D:\Distrib\games\mtg";
		private const string TargetDir = RootDir + @"\" + TargetSubdir;

		private const string OriginalSubdir = "Gatherer.Original";
		private const string PreProcessedSubdir = "Gatherer.PreProcessed";
		private const string CardsSubdir = "cards";
		private const string TokensSubdir = "tokens";

		private const string TargetSubdir = @"Mtgdb.Pictures";
		private const string LqDir = "lq";
		private const string MqDir = "mq";
		private const string ListDirSuffix = "-list";
		private const string ZipDirSuffix = "-7z";
		private const string TokenSuffix = "-token";
		private const string SevenZipExtension = ".7z";

		private bool _setupExportComplete;
		private bool _setupConversionComplete;
	}
}
