using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mtgdb.Dal;
using NLog;

namespace Mtgdb.Util
{
	[UsedImplicitly]
	public class ImageExport
	{
		public ImageExport(
			ImageRepository imageRepo,
			CardRepository cardRepo,
			ImageLoader imageLoader)
		{
			_imageRepo = imageRepo;
			_cardRepo = cardRepo;
			_imageLoader = imageLoader;
		}

		public void Load(string[] enabledImageGroups = null)
		{
			Console.WriteLine("== Loading cards ==");
			_cardRepo.LoadFile();
			_cardRepo.Load();

			Console.WriteLine("== Loading pictures ==");
			_imageRepo.LoadFiles(enabledImageGroups);
			_imageRepo.LoadSmall();
			_imageRepo.LoadZoom();
		}

		private void addFile(Bitmap original, ImageFile imageFile, string target, bool small, bool forge)
		{
			_log.Debug($"\tCopying {target}");
			var bytes = transform(original, imageFile, small, forge);

			if (bytes != null)
				File.WriteAllBytes(target, bytes);
		}

		private static string getTargetPath(Card card, ImageFile imageFile, string subdir, bool forge)
		{
			var fileName = Path.GetFileName(imageFile.FullPath);

			while (true)
			{
				if (removeExtension(ref fileName, ".jpg"))
					continue;

				if (removeExtension(ref fileName, ".png"))
					continue;

				if (removeExtension(ref fileName, ".xlhq"))
					continue;

				// ReSharper disable once StringLiteralTypo
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
			if (forge || imageFile.FullPath.EndsWith(".jpg", Str.Comparison))
				targetFileName = fileName + ".jpg";
			else if (imageFile.FullPath.EndsWith(".png", Str.Comparison))
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

		private byte[] transform(Bitmap original, ImageFile replacementImageFile, bool small, bool isForge)
		{
			var size = small ? _imageLoader.CardSize : _imageLoader.ZoomedCardSize;

			var chain = isForge
				? _imageLoader.TransformForgeImage(original, size)
				: _imageLoader.Transform(original, size);

			using (chain)
			{
				var result = saveToByteArray(replacementImageFile, chain.Result, isForge);
				chain.DisposeDifferentResult();
				return result;
			}
		}

		private static byte[] saveToByteArray(ImageFile replacementImageFile, Bitmap image, bool forge)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			using (var stream = new MemoryStream())
			{
				try
				{
					if (forge || replacementImageFile.FullPath.EndsWith(".jpg", Str.Comparison))
					{
						var codec = codecs.First(_ => _.MimeType == "image/jpeg");
						var encoderParams = new EncoderParameters
						{
							Param = { [0] = new EncoderParameter(Encoder.Quality, (long) 90) }
						};

						image.Save(stream, codec, encoderParams);
					}
					else if (replacementImageFile.FullPath.EndsWith(".png", Str.Comparison))
					{
						image.Save(stream, ImageFormat.Png);
					}
					else
						throw new NotSupportedException("only .png .jpg extensions are supported");
				}
				catch
				{
					Console.WriteLine("Failed to save " + replacementImageFile.FullPath);
					return null;
				}

				var bytes = stream.ToArray();
				return bytes;
			}
		}

		public void ExportCardImages(string directory, bool small, bool zoomed, string code, string smallSubdir, string zoomedSubdir)
		{
			var exportedSmall = new HashSet<string>(Str.Comparer);
			var exportedZoomed = new HashSet<string>(Str.Comparer);

			export(directory, code, exportedSmall, exportedZoomed, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: true);
			export(directory, code, exportedSmall, exportedZoomed, small, zoomed, smallSubdir, zoomedSubdir, matchingSet: false);
		}

		private void export(
			string directory,
			string code,
			HashSet<string> exportedSmall,
			HashSet<string> exportedZoomed,
			bool small,
			bool zoomed,
			string smallSubdir,
			string zoomedSubdir,
			bool matchingSet)
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
							&& Str.Equals(card.SetCode, modelSmall.ImageFile.SetCode) == matchingSet &&
							exportedSmall.Add(modelSmall.ImageFile.FullPath))
						{
							string smallPath = getTargetPath(card, modelSmall.ImageFile, smallSetSubdir, forge: false);

							if (!File.Exists(smallPath))
							{
								original = ImageLoader.Open(modelSmall);
								addFile(original, modelSmall.ImageFile, smallPath, small: true, forge: false);
							}
						}
					}

					if (zoomed)
					{
						var modelZoom = _imageRepo.GetImagePrint(card, _cardRepo.GetReleaseDateSimilarity);

						if (modelZoom != null &&
							Str.Equals(card.SetCode, modelZoom.ImageFile.SetCode) == matchingSet &&
							exportedZoomed.Add(modelZoom.ImageFile.FullPath))
						{
							string zoomedPath = getTargetPath(card, modelZoom.ImageFile, zoomedSetSubdir, forge: false);

							if (!File.Exists(zoomedPath))
							{
								if (original == null || modelSmall.ImageFile.FullPath != modelZoom.ImageFile.FullPath)
								{
									original?.Dispose();
									original = ImageLoader.Open(modelZoom);
								}

								addFile(original, modelZoom.ImageFile, zoomedPath, small: false, forge: false);
							}
						}
					}

					original?.Dispose();
				}
			}
		}

		private static string ensureSetSubdirectory(string smallSet)
		{
			Console.WriteLine($"	Creating {smallSet} ...");

			try
			{
				Directory.CreateDirectory(smallSet);
			}
			catch (DirectoryNotFoundException)
			{
				// CON is banned as file / folder name in Windows
				smallSet += " escape";
				Directory.CreateDirectory(smallSet);
			}

			return smallSet;
		}


		private readonly ImageRepository _imageRepo;
		private readonly CardRepository _cardRepo;
		private readonly ImageLoader _imageLoader;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}