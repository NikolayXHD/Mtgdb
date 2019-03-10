using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Mtgdb.Controls;
using NLog;

namespace Mtgdb.Data
{
	public class ImageLoader
	{
		public ImageLoader(UiConfigRepository configRepository)
		{
			_configRepository = configRepository;
			Dpi.Changed += ClearCache;
		}

		public void ClearCache()
		{
			lock (_sync)
			{
				_imagesByPath.Clear();
				_ratings.Clear();
			}
		}

		public Bitmap GetSmallImage(ImageModel model)
		{
			if (model == null)
				return null;

			Bitmap image;
			lock (_sync)
				image = tryGetFromCache(model.ImageFile.FullPath, model.Rotation);

			if (image != null)
			{
				FoundInCache?.Invoke();
				return image;
			}

			if (!File.Exists(model.ImageFile.FullPath))
				return null;

			image = LoadImage(model, CardSize);

			lock (_sync)
				if (addFirst(model.ImageFile.FullPath, model.Rotation, image))
					if (_ratings.Count >= _configRepository.Config.ImageCacheCapacity)
						removeLast();

			return image;
		}

		public Bitmap LoadImage(ImageModel model, Size size)
		{
			var original = Open(model);

			if (original == null)
				return null;

			var chain = model.ImageFile.IsArt
				? TransformArt(original, size)
				: Transform(original, size);

			using (chain)
			{
				chain.DisposeDifferentOriginal();
				return chain.Result;
			}
		}

		public static Bitmap Open(ImageModel model)
		{
			try
			{
				byte[] bytes;

				lock (SyncIo)
					bytes = File.ReadAllBytes(model.ImageFile.FullPath);

				var result = new Bitmap(new MemoryStream(bytes));

				if (model.Rotation != RotateFlipType.RotateNoneFlipNone)
					result.RotateFlip(model.Rotation);

				return result;
			}
			catch
			{
				// Incorrect image file.
				// This is expected to happen due to interrupting file download.
				return null;
			}
		}



		public BitmapTransformationChain TransformForgeImage(Bitmap original, Size size)
		{
			var frameDetector = new BmpFrameDetector(original, SizeCropped);
			frameDetector.Execute();
			var frame = frameDetector.Frame;

			var chain = new BitmapTransformationChain(original, logException);
			chain.ReplaceBy(_ => resize(_, size, frame));
			return chain;
		}

		public BitmapTransformationChain TransformArt(Bitmap original, Size size)
		{
			var chain = new BitmapTransformationChain(original, logException);

			if (!original.Size.FitsIn(size))
				chain.ReplaceBy(_ => resize(_, size));

			return chain;
		}

		public BitmapTransformationChain Transform(Bitmap original, Size size)
		{
			var chain = new BitmapTransformationChain(original, logException);
			chain.ReplaceBy(_ => resize(_, size));
			chain.Update(removeCorners);

			return chain;
		}



		private static Bitmap resize(Bitmap original, Size requiredSize, Size frame = default)
		{
			if (frame == default && original.Size == requiredSize)
				return original;

			return original.FitIn(requiredSize, frame);
		}

		private void removeCorners(Bitmap bitmap)
		{
			var remover = new BmpCornerRemoval(bitmap);
			remover.Execute();

			if (remover.ImageChanged)
				CornerRemoved?.Invoke();
		}



		private Bitmap tryGetFromCache(string path, RotateFlipType rotations)
		{
			if (!_imagesByPath.TryGetValue(new Tuple<string, RotateFlipType>(path, rotations), out var cacheEntry))
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

		private static void logException(Exception ex) => _logger.Error(ex);

		internal event Action CornerRemoved;
		internal event Action FoundInCache;

		public static readonly object SyncIo = new object();
		private static readonly object _sync = new object();

		private readonly Dictionary<Tuple<string, RotateFlipType>, ImageCacheEntry> _imagesByPath =
			new Dictionary<Tuple<string, RotateFlipType>, ImageCacheEntry>();

		private readonly LinkedList<Tuple<string, RotateFlipType>> _ratings =
			new LinkedList<Tuple<string, RotateFlipType>>();


		public static readonly Size SizeCropped = new Size(470, 659);
		private readonly Size _cardSize = new Size(223, 311);
		private readonly Size _zoomedCardSize = new Size(446, 622);

		public Size CardSize => _cardSize.ByDpi();
		public Size ZoomedCardSize => _zoomedCardSize.ByDpi();

		private readonly UiConfigRepository _configRepository;

		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
	}
}