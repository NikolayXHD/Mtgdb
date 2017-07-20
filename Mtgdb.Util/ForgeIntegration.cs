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

		private void addNewFiles(string code, bool small)
		{
			var allImageModels = _imageRepo.GetAllImageModels();

			Console.WriteLine("== Adding missing files ==");

			var modelsBySetCode = allImageModels
				.GroupBy(_ => _.SetCode)
				.ToDictionary(gr=>gr.Key, gr=>gr.ToList());

			foreach (var entryBySetCode in modelsBySetCode.OrderBy(entry=>entry.Key))
			{
				string setCode = entryBySetCode.Key;

				if (code != null && !Str.Equals(setCode, code))
					continue;

				string subdir = Path.Combine(CardPicsPath, setCode);

				if (!Directory.Exists(subdir))
					continue;

				Console.WriteLine($"== Scanning {setCode} ... ==");

				foreach (var model in entryBySetCode.Value)
					using (var original = ImageCache.Open(model))
					{
						string targetFullPath = getTargetPath(model, subdir);

						if (!File.Exists(targetFullPath))
							addFile(original, model, small, targetFullPath);
					}
			}
		}

		private void replaceExistingFiles(string code, bool small)
		{
			var subdirs = Directory.GetDirectories(CardPicsPath);
			foreach (string subdir in subdirs)
			{
				Console.WriteLine($"== Scanning {subdir} ... ==");

				string setCode = Path.GetFileName(subdir);

				if (setCode == null)
					continue;

				if (code != null && !Str.Equals(setCode, code))
					continue;

				var oldFiles = Directory.GetFiles(subdir, "*.jpg", SearchOption.AllDirectories);

				foreach (string oldFile in oldFiles)
					replaceExistingFile(oldFile, small);
			}
		}

		private void addFile(Bitmap original, ImageModel model, bool small, string targetFullPath)
		{
			Console.WriteLine($"\tCopying {targetFullPath}");

			var bytes = transform(original, model, small);

			if (bytes != null)
				File.WriteAllBytes(targetFullPath, bytes);
		}

		private static string getTargetPath(ImageModel model, string subdir)
		{
			var fileName = Path.GetFileName(model.FullPath);

			while (true)
			{
				if (removeExtension(ref fileName, ".jpg"))
					continue;

				if (removeExtension(ref fileName, ".xlhq"))
					continue;

				if (removeExtension(ref fileName, ".xhlq"))
					continue;

				if (removeExtension(ref fileName, ".full"))
					continue;
				
				break;
			}

			var targetFileName = fileName + ".full.jpg";
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

		private void replaceExistingFile(string oldFile, bool small)
		{
			var model = new ImageModel(oldFile, CardPicsPath);
			var replacementModel = _imageRepo.GetReplacementImage(model, setCode => _cardRepo.GetReleaseDate(setCode));

			if (replacementModel == null)
				return;

			replace(replacementModel, model, small);
		}

		private void replace(ImageModel replacementModel, ImageModel model, bool small)
		{
			byte[] bytes;

			using (var original = ImageCache.Open(replacementModel))
				bytes = transform(original, replacementModel, small);

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
			if (fileName == null)
				return;

			if (_verbose)
				Console.WriteLine($"\tReplacing {model.FullPath}");

			var backupFile = Path.Combine(setBackupDir, fileName);

			if (!File.Exists(backupFile))
				File.Move(model.FullPath, backupFile);

			File.WriteAllBytes(model.FullPath, bytes);
		}

		private byte[] transform(Bitmap original, ImageModel replacementModel, bool small)
		{
			var image = _imageCache.Transform(original,
				replacementModel,
				small ? _imageCache.CardSize : _imageCache.ZoomedCardSize,
				transparentCorners: false,
				crop: _crop,
				whiteCorner: _whiteCorner);

			if (image == original)
				return saveToByteArray(replacementModel, image);

			using (image)
				return saveToByteArray(replacementModel, image);
		}

		private static byte[] saveToByteArray(ImageModel replacementModel, Bitmap image)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			var codec = codecs.First(_ => _.MimeType == "image/jpeg");

			var encoderParams = new EncoderParameters
			{
				Param = { [0] = new EncoderParameter(Encoder.Quality, (long) 90) }
			};

			byte[] bytes;
			using (var stream = new MemoryStream())
			{
				try
				{
					image.Save(stream, codec, encoderParams);
				}
				catch
				{
					Console.WriteLine("Failed to save " + replacementModel.FullPath);
					return null;
				}

				bytes = stream.ToArray();
			}

			return bytes;
		}

		public void ExportCardImages(string directory, bool small, bool zoomed, string code, string smallSubdir, string zoomedSubdir)
		{
			_crop = false;
			_whiteCorner = true;

			var exported = new HashSet<string>();

			export(directory, code, exported, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: true);
			export(directory, code, exported, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: false);
		}

		private void export(string directory, string code, HashSet<string> exported, bool small, bool zoomed, string smallSubdir, string zoomedSubdir, bool matchingSet)
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
					var model = _imageRepo.GetImagePrint(card, _cardRepo.GetReleaseDate);
					if (model != null && Str.Equals(card.SetCode, model.SetCode) == matchingSet && exported.Add(model.FullPath))
					{
						string zoomedPath = getTargetPath(model, zoomedSetSubdir);
						string smallPath = getTargetPath(model, smallSetSubdir);
						bool zoomedExists = File.Exists(zoomedPath);
						bool smallExists = File.Exists(smallPath);

						if (smallExists && zoomedExists)
							continue;

						using (var original = ImageCache.Open(model))
						{
							if (small && !smallExists)
								addFile(original, model, true, smallPath);

							if (zoomed && !zoomedExists)
								addFile(original, model, false, zoomedPath);
						}
					}
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
