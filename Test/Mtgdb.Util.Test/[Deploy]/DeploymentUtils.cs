using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mtgdb.Controls;
using Mtgdb.Data;
using Mtgdb.Dev;
using Ninject;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class DeploymentUtils
	{
		private const string SetCodes = "slu,ss3,ha2,c20,iko";
		private const bool NonToken = true;
		private const bool Token = true;
		private const bool SelectSmall = true;
		private const bool SelectZoom = true;

		[Order(1)]
		[TestCase(SetCodes, NonToken, Token)]
		public async Task DownloadGathererImages(string setCodesList, bool nonToken, bool token)
		{
			var clients = new List<ImageDownloaderBase>(2)
			{
				// new GathererClient(),
				new ScryfallClient(),
			};

			var setCodes = setCodesList?.ToLower().Split(',');
			var repo = new CardRepository(new CardFormatter(), () => null)
			{
				FilterSetCode = setCode => setCodes?.Contains(setCode.ToLower()) != false
			};

			repo.LoadFile();
			repo.Load();

			foreach (Set set in repo.SetsByCode.Values)
			{
				if (setCodes != null && !setCodes.Contains(set.Code, Str.Comparer))
					continue;

				foreach ((bool isToken, FsPath typeSubdir, _) in getIsToken(nonToken, token))
				{
					var cards = set.List(isToken);
					if (cards.Count == 0)
						continue;

					var missingCardsByClient = clients.ToDictionary(_ => _, _ => 0);

					FsPath setSubdir = new FsPath(set.Code + (Str.Equals(set.Code, "con") ? " escape" : string.Empty));
					FsPath downloadRootDir = DevPaths.MtgContentDir.Join(_createZoom ? OriginalSubdir : PreProcessedSubdir);
					FsPath setDirectory = downloadRootDir.Join(typeSubdir, setSubdir);
					FsPath setDirectoryPng = setDirectory.Concat(".png");

					bool dirExisted = setDirectoryPng.IsDirectory();
					if (!dirExisted)
						setDirectoryPng.CreateDirectory();

					foreach (var card in cards)
					{
						FsPath targetFile = setDirectoryPng.Join(card.ImageName + ".png");
						FsPath processedFile = setDirectory.Join(card.ImageName + ".jpg");
						if (targetFile.IsFile() || processedFile.IsFile())
							continue;

						if (targetFile.Basename(extension: false).EndsWith("1"))
						{
							var unnumbered = targetFile.Rename(_ => _.Replace("1.png", ".png"));
							if (unnumbered.IsFile())
								unnumbered.MoveFileTo(targetFile);

							continue;
						}

						foreach (ImageDownloaderBase client in clients)
						{
							await client.DownloadCardImage(card, targetFile, CancellationToken.None);
							if (targetFile.IsFile())
								break;

							missingCardsByClient[client]++;
						}
					}

					if (!dirExisted && !setDirectoryPng.EnumerateFiles().Any())
						setDirectoryPng.DeleteDirectory();
				}
			}
		}

		[Order(2)]
		[TestCase(SetCodes, NonToken, Token)]
		public void PreProcessImages(string setCodesList, bool nonToken, bool token)
		{
			setupImageConversion();

			FsPath smallDir = DevPaths.GathererOriginalDir;
			FsPath zoomDir = DevPaths.GathererPreprocessedDir;
			FsPath smallDirBak = BakDir.Join(smallDir.Basename());
			FsPath zoomDirBak = BakDir.Join(zoomDir.Basename());

			var setCodes = setCodesList?.Split(',');

			IEnumerable<FsPath> getSetSubdirs(FsPath typeSubdir)
			{
				FsPath typeDir = smallDir.Join(typeSubdir);
				return typeDir
					.EnumerateDirectories(_fromPng ? "*.png" : "*", SearchOption.TopDirectoryOnly)
					.Select(_ => new FsPath(
						Regex.Replace(
							_.Basename(),
							@"\.png$",
							string.Empty)));
			}

			foreach ((_, FsPath typeSubdir, _) in getIsToken(nonToken, token))
			{
				foreach (FsPath setSubdir in getSetSubdirs(typeSubdir))
				{
					if (setCodes != null && !setCodes.Contains(setSubdir.Value, Str.Comparer))
						continue;

					FsPath smallJpgDir = smallDir.Join(typeSubdir, setSubdir);
					FsPath zoomJpgDir = zoomDir.Join(typeSubdir, setSubdir);

					if (!_fromPng)
					{
						if (!_createZoom)
							return;

						zoomJpgDir.CreateDirectory();
						foreach (FsPath smallImg in smallJpgDir.EnumerateFiles())
						{
							FsPath zoomImg = smallImg.ChangeDirectory(smallDir, zoomDir);
							if (_keepExisting && zoomImg.IsFile())
								continue;
							WaifuScaler.Scale(smallImg, zoomImg);
						}
					}
					else
					{
						FsPath setPngSubdir = setSubdir.Concat(".png");
						FsPath smallPngDir = smallDir.Join(typeSubdir, setPngSubdir);
						FsPath smallPngDirBak = smallDirBak.Join(typeSubdir, setPngSubdir);
						FsPath zoomPngDir = zoomDir.Join(typeSubdir, setPngSubdir);
						FsPath zoomPngDirBak = zoomDirBak.Join(typeSubdir, setPngSubdir);

						var dirs = new List<(FsPath pngDir, FsPath pngDirBak, FsPath jpgDir)>();
						if (_createZoom)
						{
							zoomPngDir.CreateDirectory();
							foreach (FsPath smallImg in smallPngDir.EnumerateFiles())
							{
								FsPath zoomImg = smallImg.ChangeDirectory(smallDir, zoomDir);
								FsPath convertedImg = zoomImg.ChangeDirectory(zoomPngDir, zoomJpgDir)
									.Rename(_ => _.Replace(".png", ".jpg"));

								if (_keepExisting && (zoomImg.IsFile() || convertedImg.IsFile()))
									continue;

								WaifuScaler.Scale(smallImg, zoomImg);
							}

							dirs.Add((pngDir: zoomPngDir, pngDirBak: zoomPngDirBak, jpgDir: zoomJpgDir));
						}

						dirs.Add((pngDir: smallPngDir, pngDirBak: smallPngDirBak, jpgDir: smallJpgDir));

						foreach ((FsPath pngDir, FsPath pngDirBak, FsPath jpgDir) in dirs)
						{
							jpgDir.CreateDirectory();

							var pngImages = pngDir.EnumerateFiles();
							foreach (FsPath sourceImage in pngImages)
								convertToJpg(sourceImage, jpgDir, _keepExisting);

							moveDirectoryToBackup(pngDir, pngDirBak);
						}
					}
				}
			}
		}

		[Order(3)]
		[TestCase(SetCodes, SelectSmall, SelectZoom, NonToken, Token)]
		public void SelectImages(string setCodes, bool small, bool zoom, bool nonToken, bool token)
		{
			setupExport();

			foreach ((bool isToken, _, FsPath tokenSuffix) in getIsToken(nonToken, token))
				_export.ExportCardImages(TargetDir,
					small,
					zoom,
					setCodes,
					LqDir.Concat(tokenSuffix),
					MqDir.Concat(tokenSuffix),
					true,
					isToken);
		}

		[Order(4)]
		[TestCase(SetCodes, SelectSmall, SelectZoom, NonToken, Token)]
		public void SignImages(string setCodes, bool small, bool zoom, bool nonToken, bool token)
		{
			foreach (FsPath qualityDir in getQualities(small, zoom))
			foreach ((_, _, FsPath tokenSuffix) in getIsToken(nonToken, token))
			{
				FsPath outputFile = getSignatureFile(qualityDir, tokenSuffix);
				FsPath packagePath = TargetDir.Join(qualityDir).Concat(tokenSuffix);
				new ImageDirectorySigner().SignFiles(packagePath, outputFile, setCodes);

				FsPath signatureFile = getSignatureFile(qualityDir, tokenSuffix);
				FsPath compressedSignatureFile = signatureFile
					.Parent()
					.Join(signatureFile.Basename(extension: false))
					.Concat(SevenZipExtension);

				if (compressedSignatureFile.IsFile())
					compressedSignatureFile.DeleteFile();

				new SevenZip(false).Compress(signatureFile, compressedSignatureFile)
					.Should().BeTrue();
			}
		}

		[Order(5)]
		[TestCase(SetCodes, SelectSmall, SelectZoom, NonToken, Token)]
		public void ZipImages(string setCodes, bool small, bool zoom, bool nonToken, bool token)
		{
			var list = setCodes?.Split(',').ToHashSet(Str.Comparer);

			foreach (FsPath qualityDir in getQualities(small, zoom))
			foreach ((_, _, FsPath tokenSuffix) in getIsToken(nonToken, token))
			{
				FsPath compressedRoot = TargetDir.Join(qualityDir).Concat(tokenSuffix).Concat(ZipDirSuffix);
				compressedRoot.CreateDirectory();

				FsPath sourceRoot = TargetDir.Join(qualityDir).Concat(tokenSuffix);
				foreach (var subdir in sourceRoot.EnumerateDirectories())
				{
					string subdirRelative = subdir.Basename();
					if (list?.Contains(subdirRelative) == false)
						continue;

					var targetFile = compressedRoot.Join(subdirRelative).Concat(SevenZipExtension);
					if (targetFile.IsFile())
						targetFile.DeleteFile();

					new SevenZip(false).Compress(subdir, targetFile)
						.Should().BeTrue();
				}
			}
		}

		private static void moveDirectoryToBackup(FsPath dir, FsPath dirBak)
		{
			if (dirBak.IsDirectory())
				dirBak.DeleteDirectory(recursive: true);
			dirBak.Parent().CreateDirectory();
			dir.MoveDirectoryTo(dirBak);
		}

		private void convertToJpg(FsPath sourceImage, FsPath targetDir, bool keepExisting)
		{
			FsPath targetImage = targetDir.Join(sourceImage.Basename(extension: false)).Concat(".jpg");
			if (keepExisting && targetImage.IsFile())
				return;

			using var original = new Bitmap(sourceImage.Value);
			new BmpAlphaToBackgroundColorTransformation(original, Color.White)
				.Execute();

			original.Save(targetImage.Value, _jpegCodec, _jpegEncoderParams);
		}

		private static IEnumerable<FsPath> getQualities(bool small, bool zoom)
		{
			if (small)
				yield return LqDir;

			if (zoom)
				yield return MqDir;
		}

		private static IEnumerable<(bool IsToken, FsPath SourceSubdir, FsPath TargetDirSuffix)> getIsToken(bool nonToken, bool token)
		{
			if (nonToken)
				yield return (false, CardsSubdir, FsPath.Empty);

			if (token)
				yield return (true, TokensSubdir, TokenSuffix);
		}

		private static FsPath getSignatureFile(FsPath qualityDir, FsPath tokenSuffix)
		{
			FsPath outputFile = TargetDir
				.Join(qualityDir).Concat(tokenSuffix).Concat(ListDirSuffix)
				.Join(Signer.SignaturesFile);
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


		private static readonly bool _keepExisting = true;
		private static readonly bool _fromPng = true;
		private static readonly bool _createZoom = true;

		private ImageCodecInfo _jpegCodec;
		private EncoderParameters _jpegEncoderParams;
		private ImageExport _export;

		private static readonly FsPath BakDir = DevPaths.MtgContentDir.Join(".bak");
		private static readonly FsPath TargetSubdir = new FsPath("Mtgdb.Pictures");
		private static readonly FsPath TargetDir = DevPaths.MtgContentDir.Join(TargetSubdir);

		private static readonly string OriginalSubdir = DevPaths.GathererOriginalDir.Basename();
		private static readonly string PreProcessedSubdir = DevPaths.GathererPreprocessedDir.Basename();
		private static readonly FsPath CardsSubdir = new FsPath("cards");
		private static readonly FsPath TokensSubdir = new FsPath("tokens");

		private static readonly FsPath LqDir = new FsPath("lq");
		private static readonly FsPath MqDir = new FsPath("mq");

		private static readonly FsPath ListDirSuffix = new FsPath("-list");
		private static readonly FsPath ZipDirSuffix = new FsPath("-7z");
		private static readonly FsPath TokenSuffix = new FsPath("-token");
		private static readonly FsPath SevenZipExtension = new FsPath(".7z");

		private bool _setupExportComplete;
		private bool _setupConversionComplete;
	}
}
