using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Mtgdb.Controls;
using NLog;

namespace Mtgdb.Dal
{
	public class ImageCache
	{
		public static readonly object SyncRoot = new object();

		public ImageCache(ImageCacheConfig config)
		{
			Capacity = config.GetCacheCapacity();
		}

		public Bitmap GetSmallImage(ImageModel model)
		{
			if (model == null)
				return null;

			Bitmap image;
			lock (_imagesByPath)
				image = tryGetFromCache(model.ImageFile.FullPath, model.Rotation);

			if (image != null)
				return image;

			if (!File.Exists(model.ImageFile.FullPath))
				return null;

			image = LoadImage(model, CardSize);

			lock (_imagesByPath)
				if (addFirst(model.ImageFile.FullPath, model.Rotation, image))
					if (_ratings.Count >= Capacity)
						removeLast();

			return image;
		}

		public static Bitmap LoadImage(ImageModel model, Size size)
		{
			var original = Open(model);

			if (original == null)
				return null;

			var result = Transform(original, model.ImageFile, size, crop: false);

			if (result != original)
				original.Dispose();

			return result;
		}

		public static Bitmap Open(ImageModel model)
		{
			Bitmap original;

			try
			{
				byte[] bytes;

				lock (SyncRoot)
					bytes = File.ReadAllBytes(model.ImageFile.FullPath);

				original = new Bitmap(new MemoryStream(bytes));

				if (model.Rotation != RotateFlipType.RotateNoneFlipNone)
					original.RotateFlip(model.Rotation);
			}
			catch
			{
				// Некорректный файл изображения.
				// Штатная ситуация из за возможного прерывания скачивания файла.
				original = null;
			}

			return original;
		}

		public static Bitmap Transform(Bitmap original, ImageFile imageFile, Size size, bool crop)
		{
			Bitmap resizedBmp;

			if (!crop && size == original.Size || imageFile.IsArt && original.Size.FitsIn(size))
				resizedBmp = original;
			else
			{
				var frame = getFrame(original, crop);

				try
				{
					resizedBmp = original.FitIn(size, frame);
				}
				catch (Exception ex)
				{
					resizedBmp = original;
					_log.Error(ex);
				}
			}

			if (imageFile.IsArt || crop)
				return resizedBmp;

			var corneredBmp = (Bitmap) resizedBmp.Clone();

			bool cornerRemoved;
			try
			{
				var remover = new BmpCornerRemoval(corneredBmp);
				remover.Execute();
				cornerRemoved = remover.ImageChanged;
			}
			catch (Exception ex)
			{
				_log.Error(ex);
				cornerRemoved = false;
			}

			if (cornerRemoved)
			{
				if (resizedBmp != original)
					resizedBmp.Dispose();

				return corneredBmp;
			}
			else
			{
				corneredBmp.Dispose();
				return resizedBmp;
			}
		}

		private static Size getFrame(Bitmap original, bool crop)
		{
			Size frame;
			if (crop)
			{
				var frameDetector = new BmpFrameDetector(original);
				frameDetector.Execute();
				frame = frameDetector.Frame;
			}
			else
				frame = default(Size);
			return frame;
		}

		private Bitmap tryGetFromCache(string path, RotateFlipType rotations)
		{
			ImageCacheEntry cacheEntry;
			if (!_imagesByPath.TryGetValue(new Tuple<string, RotateFlipType>(path, rotations), out cacheEntry))
				return null;

			shiftFromLast(cacheEntry);

			return cacheEntry.Image;
		}

		private static void shiftFromLast(ImageCacheEntry cacheEntry)
		{
			var ratingEntry = cacheEntry.RatingEntry;
			var previousEntry = ratingEntry.Previous;

			if (previousEntry != null)
				ratingEntry.SwapWith(previousEntry);
		}

		private bool addFirst(string path, RotateFlipType rotations, Bitmap image)
		{
			var key = new Tuple<string, RotateFlipType>(path, rotations);

			if (_imagesByPath.ContainsKey(key))
				return false;

			_ratings.AddFirst(key);
			_imagesByPath[key] = new ImageCacheEntry(image, _ratings.First);

			return true;
		}

		private void removeLast()
		{
			var keyToRemove = _ratings.Last.Value;
			_ratings.RemoveLast();
			_imagesByPath.Remove(keyToRemove);
		}

		private readonly Dictionary<Tuple<string, RotateFlipType>, ImageCacheEntry> _imagesByPath =
			new Dictionary<Tuple<string, RotateFlipType>, ImageCacheEntry>();

		private readonly LinkedList<Tuple<string, RotateFlipType>> _ratings =
			new LinkedList<Tuple<string, RotateFlipType>>();


		public static readonly Size SizeCropped = new Size(424, 622);
		private Size _cardSize = new Size(223, 311);
		private Size _zoomedCardSize = new Size(446, 622);

		public Size CardSize => _cardSize.ByDpi();

		public Size ZoomedCardSize => _zoomedCardSize.ByDpi();

		public int Capacity { get; }
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}