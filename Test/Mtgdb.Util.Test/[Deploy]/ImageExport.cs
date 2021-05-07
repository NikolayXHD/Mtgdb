using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Mtgdb.Data;
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

		public void ExportCardImages(
			FsPath directory,
			bool small,
			bool zoom,
			string setCodes,
			FsPath smallSubdir,
			FsPath zoomedSubdir,
			bool forceRemoveCorner,
			bool token)
		{
			var exportedSmall = new HashSet<FsPath>();
			var exportedZoomed = new HashSet<FsPath>();

			export(directory, setCodes, exportedSmall, exportedZoomed, small, zoom, smallSubdir, zoomedSubdir, matchingSet: true, forceRemoveCorner, token);
			export(directory, setCodes, exportedSmall, exportedZoomed, small, zoom, smallSubdir, zoomedSubdir, matchingSet: false, forceRemoveCorner, token);
		}



		private void export(
			FsPath directory,
			string setCodesStr,
			ISet<FsPath> exportedSmall,
			ISet<FsPath> exportedZoomed,
			bool small,
			bool zoomed,
			FsPath smallSubdir,
			FsPath zoomedSubdir,
			bool matchingSet,
			bool forceRemoveCorner,
			bool token)
		{
			var setCodes = setCodesStr?.Split(',').ToHashSet(StringComparer.OrdinalIgnoreCase);
			foreach ((string setCode, Set set) in _cardRepo.SetsByCode)
			{
				Console.WriteLine(setCode);

				if (setCodes?.Contains(setCode) == false)
					continue;

				FsPath smallSetSubdir = FsPath.None;
				FsPath zoomedSetSubdir = FsPath.None;

				if (small)
				{
					if (smallSubdir.HasValue())
						smallSetSubdir = directory.Join(smallSubdir).Join(setCode);
					else
						smallSetSubdir = directory.Join(setCode);

					smallSetSubdir = ensureSetSubdirectory(smallSetSubdir);
				}

				if (zoomed)
				{
					if (zoomedSubdir.HasValue())
						zoomedSetSubdir = directory.Join(zoomedSubdir).Join(setCode);
					else
						zoomedSetSubdir = directory.Join(setCode);

					zoomedSetSubdir = ensureSetSubdirectory(zoomedSetSubdir);
				}

				foreach (var card in set.Cards)
				{
					if (card.IsSingleSide() && card.Faces.Main != card)
						continue;

					if (card.IsToken != token)
						continue;

					Bitmap original = null;
					ImageModel modelSmall = null;

					if (small)
					{
						modelSmall = _imageRepo.GetSmallImage(card, _cardRepo.GetReleaseDateSimilarity);

						if (modelSmall != null
							&& Str.Equals(card.SetCode, modelSmall.ImageFile.SetCode) == matchingSet &&
							exportedSmall.Add(modelSmall.ImageFile.FullPath))
						{
							FsPath smallPath = getTargetPath(modelSmall.ImageFile, smallSetSubdir);

							if (!smallPath.IsFile() || card.Faces.Count > 1)
							{
								original = ImageLoader.Open(modelSmall);
								addFile(original, modelSmall.ImageFile, smallPath, small: true, forceRemoveCorner);
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
							FsPath zoomedPath = getTargetPath(modelZoom.ImageFile, zoomedSetSubdir);

							if (!zoomedPath.IsFile() || card.Faces.Count > 1)
							{
								if (original == null || modelSmall.ImageFile.FullPath != modelZoom.ImageFile.FullPath)
								{
									original?.Dispose();
									original = ImageLoader.Open(modelZoom);
								}

								addFile(original, modelZoom.ImageFile, zoomedPath, small: false, forceRemoveCorner);
							}
						}
					}

					original?.Dispose();
				}

				smallSetSubdir.DeleteEmptyDirectory();
				zoomedSetSubdir.DeleteEmptyDirectory();
			}
		}

		private static FsPath ensureSetSubdirectory(FsPath smallSet)
		{
			Console.WriteLine($"	Creating {smallSet} ...");

			try
			{
				smallSet.CreateDirectory();
			}
			catch (DirectoryNotFoundException)
			{
				// CON is banned as file / folder name in Windows
				smallSet = smallSet.Concat(" escape");
				smallSet.CreateDirectory();
			}

			return smallSet;
		}

		private static FsPath getTargetPath(ImageFile imageFile, FsPath subdir)
		{
			var fileName = imageFile.FullPath.Basename();

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

			string targetFileName;
			if (imageFile.FullPath.Value.EndsWith(".jpg", Str.Comparison))
				targetFileName = fileName + ".jpg";
			else if (imageFile.FullPath.Value.EndsWith(".png", Str.Comparison))
				targetFileName = fileName + ".png";
			else
				throw new NotSupportedException("only .png .jpg extensions are supported");

			var targetFullPath = subdir.Join(targetFileName);
			return targetFullPath;
		}

		private void addFile(Bitmap original, ImageFile imageFile, FsPath target, bool small, bool forceRemoveCorner)
		{
			_log.Debug($"\tCopying {target}");
			var bytes = transform(original, imageFile, small, forceRemoveCorner);

			if (bytes != null)
				target.WriteAllBytes(bytes);
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

		private byte[] transform(Bitmap original, ImageFile replacementImageFile, bool small, bool forceRemoveCorner)
		{
			var size = small ? _imageLoader.CardSize : _imageLoader.ZoomedCardSize;

			var chain = _imageLoader.Transform(original, size, forceRemoveCorner);

			if (!replacementImageFile.FullPath.Value.EndsWith(".png", Str.Comparison))
				chain.Update(bmp => new BmpAlphaToBackgroundColorTransformation(bmp, Color.White).Execute());

			using (chain)
			{
				var result = saveToByteArray(replacementImageFile, chain.Result);
				chain.DisposeDifferentResult();
				return result;
			}
		}

		private static byte[] saveToByteArray(ImageFile replacementImageFile, Bitmap image)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

			using var stream = new MemoryStream();
			try
			{
				if (replacementImageFile.FullPath.Value.EndsWith(".jpg", Str.Comparison))
				{
					var codec = codecs.First(_ => _.MimeType == "image/jpeg");
					var encoderParams = new EncoderParameters
					{
						Param = { [0] = new EncoderParameter(Encoder.Quality, (long) 90) }
					};

					image.Save(stream, codec, encoderParams);
				}
				else if (replacementImageFile.FullPath.Value.EndsWith(".png", Str.Comparison))
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


		private readonly ImageRepository _imageRepo;
		private readonly CardRepository _cardRepo;
		private readonly ImageLoader _imageLoader;

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}
