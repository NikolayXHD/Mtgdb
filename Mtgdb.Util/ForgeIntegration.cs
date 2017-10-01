using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Mtgdb.Dal;

namespace Mtgdb.Util
{
	public class ForgeIntegration
	{
		private readonly ImageRepository _imageRepo;
		private readonly CardRepository _cardRepo;
		private readonly ImageCache _imageCache;
		public readonly string CardPicsPath;
		public readonly string CardPicsBackupPath;
		private readonly bool _verbose;
		private bool _crop;
		private bool _whiteCorner;
		private Dictionary<string, string> _setByForgeSet;
		private Dictionary<string, string> _forgeSetBySet;

		public ForgeIntegration(
			ImageRepository imageRepo, 
			CardRepository cardRepo, 
			ForgeIntegrationConfig config,
			ImageCache imageCache)
		{
			_imageRepo = imageRepo;
			_cardRepo = cardRepo;
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
			_imageRepo.Load();
			_imageRepo.LoadZoom();

			_cardRepo.SelectCardImages(_imageRepo);
		}

		public void OverrideForgePictures(string setCode)
		{
			_crop = true;

			var lineParts = File.ReadAllLines(AppDir.Data.AddPath("forge_set_mapping.txt"))
				.Select(_ => _.Split('\t'))
				.ToArray();

			_setByForgeSet = lineParts
				.ToDictionary(_ => _[0], _ => _[1]);

			_forgeSetBySet = lineParts
				.ToDictionary(_ => _[1], _ => _[0]);

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

			replaceExistingFiles(setCode, small: false);
			addNewFiles(setCode, small: false);
		}

		private void addNewFiles(string setCode, bool small)
		{
			Console.WriteLine("== Adding missing files ==");

			foreach (var set in _cardRepo.SetsByCode.Values.OrderBy(s=>s.Code))
			{
				string modelSetCode = set.Code;

				if (setCode != null && !Str.Equals(modelSetCode, setCode))
					continue;

				string forgeSet;
				if (!_forgeSetBySet.TryGetValue(modelSetCode, out forgeSet))
					forgeSet = modelSetCode;

				string subdir = Path.Combine(CardPicsPath, forgeSet);

				if (!Directory.Exists(subdir))
					continue;
				
				Console.WriteLine($"== Scanning {forgeSet} ... ==");

				var models = set.Cards
					.Select(c => _imageRepo.GetImagePrint(c, _cardRepo.GetReleaseDateSimilarity))
					.Where(m => m != null)
					.Distinct()
					.ToList();

				foreach (var model in models)
					using (var original = ImageCache.Open(model))
					{
						string targetFullPath = getTargetPath(model, subdir, forge: true);

						if (!File.Exists(targetFullPath))
							addFile(original, model, targetFullPath, small, forge: true);
					}
			}
		}

		private void replaceExistingFiles(string code, bool small)
		{
			var subdirs = Directory.GetDirectories(CardPicsPath);
			foreach (string subdir in subdirs)
			{
				Console.WriteLine($"== Scanning {subdir} ... ==");

				string forgeSetCode = Path.GetFileName(subdir);

				if (forgeSetCode == null)
					continue;

				string mappedSetCode;
				if (!_setByForgeSet.TryGetValue(forgeSetCode, out mappedSetCode))
					mappedSetCode = forgeSetCode;
				
				if (code != null && !Str.Equals(mappedSetCode, code))
					continue;

				var oldFiles = Directory.GetFiles(subdir, "*.jpg", SearchOption.AllDirectories);

				foreach (string oldFile in oldFiles)
				{
					var model = new ImageModel(oldFile, CardPicsPath, setCode: mappedSetCode);

					var artist = _cardRepo.SetsByCode.TryGet(mappedSetCode)
						?.Cards
						.Where(_ => _.ImageNameBase == model.Name)
						.AtMax(_ => _.ImageName == model.ImageName)
						.FindOrDefault()
						?.Artist;

					var model2 = artist != null
						? new ImageModel(oldFile, CardPicsPath, setCode: mappedSetCode, artist: artist)
						: model;

					var replacementModel = _imageRepo.GetReplacementImage(model2, _cardRepo.GetReleaseDateSimilarity);

					if (replacementModel != null)
						replace(replacementModel, model, small);
				}
			}
		}

		private void addFile(Bitmap original, ImageModel model, string target, bool small, bool forge)
		{
			Console.WriteLine($"\tCopying {target}");

			var bytes = transform(original, model, small, forge);

			if (bytes != null)
				File.WriteAllBytes(target, bytes);
		}

		private static string getTargetPath(ImageModel model, string subdir, bool forge)
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

			string forgeAddition = forge ? ".full" : "";

			string targetFileName;
			if (forge || model.FullPath.EndsWith(".jpg", Str.Comparison))
				targetFileName = fileName + forgeAddition + ".jpg";
			else if (model.FullPath.EndsWith(".png", Str.Comparison))
				targetFileName = fileName + forgeAddition + ".png";
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
			var image = _imageCache.Transform(
				original,
				replacementModel,
				small ? _imageCache.CardSize : _imageCache.ZoomedCardSize,
				transparentCorners: false,
				crop: _crop,
				whiteCorner: _whiteCorner);

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
			_crop = false;
			_whiteCorner = true;

			var exportedSmall = new HashSet<string>();
			var exportedZoomed = new HashSet<string>();

			export(directory, code, exportedSmall, exportedZoomed, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: true);
			export(directory, code, exportedSmall, exportedZoomed, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: false);
		}

		private void export(string directory, string code, HashSet<string> exportedSmall, HashSet<string> exportedZoomed, bool small, bool zoomed, string smallSubdir, string zoomedSubdir, bool matchingSet)
		{
			//var withoutImage = _cardRepo.Cards.Where(_ => _.ImageModel == null).ToArray();

			foreach (var entryBySetCode in _cardRepo.SetsByCode)
			{
				string setCode = entryBySetCode.Key;

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
						modelSmall = _imageRepo.GetImageSmall(card, _cardRepo.GetReleaseDateSimilarity);

						if (modelSmall != null
								&& Str.Equals(card.SetCode, modelSmall.SetCode) == matchingSet &&
								exportedSmall.Add(modelSmall.FullPath))
						{
							string smallPath = getTargetPath(modelSmall, smallSetSubdir, forge: false);

							if (!File.Exists(smallPath))
							{
								original = ImageCache.Open(modelSmall);
								addFile(original, modelSmall, smallPath, small: true, forge: false);
							}
						}
					}

					ImageModel modelZoom = null;
					if (zoomed)
					{
						modelZoom = _imageRepo.GetImagePrint(card, _cardRepo.GetReleaseDateSimilarity);

						if (modelZoom != null &&
								Str.Equals(card.SetCode, modelZoom.SetCode) == matchingSet &&
								exportedZoomed.Add(modelZoom.FullPath))
						{
							string zoomedPath = getTargetPath(modelZoom, zoomedSetSubdir, forge: false);

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
	}
}
