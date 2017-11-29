using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Mtgdb.Dal;
using NLog;

namespace Mtgdb.Util
{
	public class ForgeIntegration
	{
		public ForgeIntegration(
			ImageRepository imageRepo, 
			CardRepository cardRepo, 
			ForgeIntegrationConfig config,
			ForgeSetRepository forgeSetRepository,
			ImageCache imageCache)
		{
			_imageRepo = imageRepo;
			_cardRepo = cardRepo;
			_forgeSetRepository = forgeSetRepository;
			_imageCache = imageCache;

			CardPicsPath = Environment.ExpandEnvironmentVariables(config.CardPicsPath);
			CardPicsBackupPath = Environment.ExpandEnvironmentVariables(config.CardPicsBackupPath);

			_verbose = config.Verbose == true;
		}

		public void Load()
		{
			Console.WriteLine("== Loading cards ==");
			_cardRepo.LoadFile();
			_cardRepo.Load();

			Console.WriteLine("== Loading pictures ==");
			_imageRepo.LoadFiles();
			_imageRepo.LoadSmall();
			_imageRepo.LoadZoom();

			_cardRepo.OnImagesLoaded();
		}

		public void OverrideForgePictures(string targetSetCode)
		{
			_forgeSetRepository.Load();
			
			if (!Directory.Exists(CardPicsPath))
			{
				Console.WriteLine($"== Aborting. Not found {CardPicsPath} ==");
				return;
			}

			if (!Directory.Exists(CardPicsBackupPath))
			{
				if (_verbose)
					Console.WriteLine($"== Creating backup directory {CardPicsBackupPath} ==");

				Directory.CreateDirectory(CardPicsBackupPath);
			}

			replaceExistingFiles(targetSetCode, small: false);
			addNewFiles(targetSetCode, small: false);
		}

		private void addNewFiles(string targetSetCode, bool small)
		{
			Console.WriteLine("== Adding missing files ==");

			foreach (var set in _cardRepo.SetsByCode.Values.OrderBy(s=>s.Code))
			{
				string setCode = set.Code;

				if (targetSetCode != null && !Str.Equals(setCode, targetSetCode))
					continue;

				string forgeSetCode = _forgeSetRepository.ToForgeSet(setCode);
				
				string subdir = Path.Combine(CardPicsPath, forgeSetCode);

				if (!Directory.Exists(subdir))
					continue;
				
				Console.WriteLine($"== Scanning {forgeSetCode} ... ==");

				var exported = new HashSet<string>(Str.Comparer);

				foreach (var card in set.Cards)
				{
					var model = _imageRepo.GetImagePrint(card, _cardRepo.GetReleaseDateSimilarity);

					if (model == null)
						continue;

					if (exported.Contains(model.FullPath))
						continue;

					exported.Add(model.FullPath);

					using (var original = ImageCache.Open(model))
					{
						string targetFullPath = getTargetPath(card, model, subdir, forge: true);

						if (!File.Exists(targetFullPath))
							addFile(original, model, targetFullPath, small, forge: true);
					}
				}
			}
		}

		private void replaceExistingFiles(string targetSetCode, bool small)
		{
			var subdirs = Directory.GetDirectories(CardPicsPath);
			foreach (string subdir in subdirs)
			{
				Console.WriteLine($"== Scanning {subdir} ... ==");

				string forgeSetCode = Path.GetFileName(subdir);

				if (forgeSetCode == null)
					continue;

				string setCode = _forgeSetRepository.FromForgeSet(forgeSetCode);
				
				if (targetSetCode != null && !Str.Equals(setCode, targetSetCode))
					continue;

				var oldFiles = Directory.GetFiles(subdir, "*.jpg", SearchOption.AllDirectories);

				foreach (string oldFile in oldFiles)
				{
					var model = new ImageModel(oldFile, CardPicsPath, setCode: setCode);

					var artist = _cardRepo.SetsByCode.TryGet(setCode)
						?.Cards
						.Where(_ => _.ImageNameBase == model.Name)
						.AtMax(_ => _.ImageName == model.ImageName)
						.FindOrDefault()
						?.Artist;

					var model2 = artist != null
						? new ImageModel(oldFile, CardPicsPath, setCode: setCode, artist: artist)
						: model;

					var replacementModel = _imageRepo.GetReplacementImage(model2, _cardRepo.GetReleaseDateSimilarity);

					if (replacementModel != null)
						replace(replacementModel, model, small);
				}
			}
		}

		private void addFile(Bitmap original, ImageModel model, string target, bool small, bool forge)
		{
			_log.Debug($"\tCopying {target}");
			var bytes = transform(original, model, small, forge);

			if (bytes != null)
				File.WriteAllBytes(target, bytes);
		}

		private static string getTargetPath(Card card, ImageModel model, string subdir, bool forge)
		{
			var fileName = Path.GetFileName(model.FullPath);

			while (true)
			{
				if (removeExtension(ref fileName, ".jpg"))
					continue;

				if (removeExtension(ref fileName, ".png"))
					continue;

				if (removeExtension(ref fileName, ".xlhq"))
					continue;

				if (removeExtension(ref fileName, ".xhlq"))
					continue;

				if (removeExtension(ref fileName, ".full"))
					continue;
				
				break;
			}

			if (forge)
			{
				if (Str.Equals(card.Layout, "aftermath") && fileName.IndexOf("»", Str.Comparison) < 0)
				{
					int[] capitalIndices = Enumerable.Range(0, fileName.Length)
						.Where(i => char.IsUpper(fileName[i]))
						.ToArray();

					if (capitalIndices.Length == 2 && capitalIndices[0] == 0)
					{
						fileName =
							fileName.Substring(0, capitalIndices[1]) +
							"»" +
							fileName.Substring(capitalIndices[1]);
					}
				}

				fileName += ".full";
			}

			string targetFileName;
			if (forge || model.FullPath.EndsWith(".jpg", Str.Comparison))
				targetFileName = fileName + ".jpg";
			else if (model.FullPath.EndsWith(".png", Str.Comparison))
				targetFileName = fileName + ".png";
			else
				throw new NotSupportedException("only .png .jpg extensions are supported");

			var targetFullPath = Path.Combine(subdir, targetFileName);
			return targetFullPath;
		}

		private static bool removeExtension(ref string fileName, string ext)
		{
			if (fileName.EndsWith(ext, Str.Comparison))
			{
				fileName = fileName.Substring(0, fileName.Length - ext.Length);
				return true;
			}

			return false;
		}

		private void replace(ImageModel replacementModel, ImageModel model, bool small)
		{
			byte[] bytes;

			using (var original = ImageCache.Open(replacementModel))
				bytes = transform(original, replacementModel, small, forge: true);

			if (bytes == null)
				return;

			if (bytes.Length == new FileInfo(model.FullPath).Length)
			{
				if (_verbose)
					Console.WriteLine($"\tSkippng identical {replacementModel.FullPath}");

				return;
			}

			var setBackupDir = Path.Combine(CardPicsBackupPath, model.SetCode);
			if (!Directory.Exists(setBackupDir))
			{
				if (_verbose)
					Console.WriteLine($"\tCreating backup directory {setBackupDir}");
				Directory.CreateDirectory(setBackupDir);
			}

			string fileName = Path.GetFileName(model.FullPath);
			
			if (_verbose)
				Console.WriteLine($"\tReplacing {model.FullPath}");

			var backupFile = Path.Combine(setBackupDir, fileName);

			if (!File.Exists(backupFile))
				File.Move(model.FullPath, backupFile);

			File.WriteAllBytes(model.FullPath, bytes);
		}

		private byte[] transform(Bitmap original, ImageModel replacementModel, bool small, bool forge)
		{
			var size = small ? _imageCache.CardSize : _imageCache.ZoomedCardSize;

			var image = ImageCache.Transform(original, replacementModel, size, crop: forge);

			var result = saveToByteArray(replacementModel, image, forge);

			if (image != original)
				image.Dispose();

			return result;
		}

		private static byte[] saveToByteArray(ImageModel replacementModel, Bitmap image, bool forge)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			using (var stream = new MemoryStream())
			{
				try
				{
					if (forge || replacementModel.FullPath.EndsWith(".jpg", Str.Comparison))
					{
						var codec = codecs.First(_ => _.MimeType == "image/jpeg");
						var encoderParams = new EncoderParameters
						{
							Param = { [0] = new EncoderParameter(Encoder.Quality, (long) 90) }
						};

						image.Save(stream, codec, encoderParams);
					}
					else if (replacementModel.FullPath.EndsWith(".png", Str.Comparison))
					{
						image.Save(stream, ImageFormat.Png);
					}
					else
						throw new NotSupportedException("only .png .jpg extensions are supported");
				}
				catch
				{
					Console.WriteLine("Failed to save " + replacementModel.FullPath);
					return null;
				}

				var bytes = stream.ToArray();
				return bytes;
			}
		}

		public void ExportCardImages(string directory, bool small, bool zoomed, string code, string smallSubdir, string zoomedSubdir)
		{
			var exportedSmall = new HashSet<string>();
			var exportedZoomed = new HashSet<string>();

			export(directory, code, exportedSmall, exportedZoomed, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: true);
			export(directory, code, exportedSmall, exportedZoomed, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: false);
		}

		private void export(string directory, string code, HashSet<string> exportedSmall, HashSet<string> exportedZoomed, bool small, bool zoomed, string smallSubdir, string zoomedSubdir, bool matchingSet)
		{
			foreach (var entryBySetCode in _cardRepo.SetsByCode)
			{
				string setCode = entryBySetCode.Key;

				Console.WriteLine(setCode);

				if (code != null && !Str.Equals(code, setCode))
					continue;

				string smallSetSubdir = null;
				string zoomedSetSubdir = null;

				if (small)
				{
					if (!string.IsNullOrEmpty(smallSubdir))
						smallSetSubdir = Path.Combine(directory, smallSubdir, setCode);
					else
						smallSetSubdir = Path.Combine(directory, setCode);

					smallSetSubdir = ensureSetSubdirectory(smallSetSubdir);
				}

				if (zoomed)
				{
					if (!string.IsNullOrEmpty(zoomedSubdir))
						zoomedSetSubdir = Path.Combine(directory, zoomedSubdir, setCode);
					else
						zoomedSetSubdir = Path.Combine(directory, setCode);

					zoomedSetSubdir = ensureSetSubdirectory(zoomedSetSubdir);
				}

				foreach (var card in entryBySetCode.Value.Cards)
				{
					Bitmap original = null;
					ImageModel modelSmall = null;

					if (small)
					{
						modelSmall = _imageRepo.GetSmallImage(card, _cardRepo.GetReleaseDateSimilarity);

						if (modelSmall != null
								&& Str.Equals(card.SetCode, modelSmall.SetCode) == matchingSet &&
								exportedSmall.Add(modelSmall.FullPath))
						{
							string smallPath = getTargetPath(card, modelSmall, smallSetSubdir, forge: false);

							if (!File.Exists(smallPath))
							{
								original = ImageCache.Open(modelSmall);
								addFile(original, modelSmall, smallPath, small: true, forge: false);
							}
						}
					}

					if (zoomed)
					{
						var modelZoom = _imageRepo.GetImagePrint(card, _cardRepo.GetReleaseDateSimilarity);

						if (modelZoom != null &&
								Str.Equals(card.SetCode, modelZoom.SetCode) == matchingSet &&
								exportedZoomed.Add(modelZoom.FullPath))
						{
							string zoomedPath = getTargetPath(card, modelZoom, zoomedSetSubdir, forge: false);

							if (!File.Exists(zoomedPath))
							{
								if (original == null || modelSmall.FullPath != modelZoom.FullPath)
								{
									original?.Dispose();
									original = ImageCache.Open(modelZoom);
								}

								addFile(original, modelZoom, zoomedPath, small: false, forge: false);
							}
						}
					}

					original?.Dispose();
				}
			}
		}

		private string ensureSetSubdirectory(string smallSet)
		{
			if (_verbose)
				Console.WriteLine($"	Creating {smallSet} ...");

			try
			{
				Directory.CreateDirectory(smallSet);
			}
			catch (DirectoryNotFoundException)
			{
				// CON является недопустимым именем файла/папки
				smallSet += " escape";
				Directory.CreateDirectory(smallSet);
			}

			return smallSet;
		}



		private readonly ImageRepository _imageRepo;
		private readonly CardRepository _cardRepo;
		private readonly ForgeSetRepository _forgeSetRepository;
		private readonly ImageCache _imageCache;
		public readonly string CardPicsPath;
		public readonly string CardPicsBackupPath;
		private readonly bool _verbose;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
