﻿using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Mtgdb.Controls;
using Mtgdb.Gui;
using NLog;

namespace Mtgdb.Dal
{
	public class ImageCache
	{
		public ImageCache(ImageCacheConfig config, SmallConfig smallConfig, ZoomedConfig zoomedConfig)
		{
			if (smallConfig.Width.HasValue && smallConfig.Height.HasValue)
				_cardSize = new Size(smallConfig.Width.Value, smallConfig.Height.Value);

			if (zoomedConfig.Width.HasValue && zoomedConfig.Height.HasValue)
				_zoomedCardSize = new Size(zoomedConfig.Width.Value, zoomedConfig.Height.Value);

			Capacity = config.GetCacheCapacity();
			_transparentCornersWhenNotZoomed = config.TransparentCornersWhenNotZoomed ?? true;
		}

		public Bitmap GetSmallImage(ImageModel model)
		{
			return GetImage(model, CardSize);
		}

		public Bitmap GetImage(ImageModel model, Size size)
		{
			if (model == null)
				return null;

			Bitmap image;
			lock (_imagesByPath)
				image = tryGetFromCache(model.FullPath);

			if (image != null)
				return image;

			if (!File.Exists(model.FullPath))
				return null;

			image = LoadImage(model, size, transparentCorners: _transparentCornersWhenNotZoomed, crop: false);

			lock (_imagesByPath)
				if (addFirst(model.FullPath, image))
					if (_ratings.Count >= Capacity)
						removeLast();

			return image;
		}

		public Bitmap LoadImage(ImageModel model, Size size, bool transparentCorners, bool crop, bool whiteCorner = false)
		{
			var original = Open(model);

			if (original == null)
				return null;

			var result = Transform(original, model, size, transparentCorners, crop, whiteCorner);

			if (result != original)
				original.Dispose();

			return result;
		}

		public static Bitmap Open(ImageModel model)
		{
			Bitmap original;

			try
			{
				original = new Bitmap(model.FullPath);
			}
			catch
			{
				// Некорректный файл изображения.
				// Штатная ситуация из за возможного прерывания скачивания файла.
				original = null;
			}

			return original;
		}

		public Bitmap Transform(Bitmap original, ImageModel model, Size size, bool transparentCorners, bool crop, bool whiteCorner)
		{
			Bitmap bitmap = original;

			if (crop || size != original.Size)
			{
				Size frame;
				if (crop)
				{
					var frameDetector = new BmpFrameDetector(original);
					frameDetector.Execute();
					frame = frameDetector.Frame;
				}
				else
					frame = new Size(0, 0);

				try
				{
					bitmap = original.FitIn(size, frame);
				}
				catch
				{
				}
			}

			if (!transparentCorners && !whiteCorner ||
			    size == _cardSize && model.IsPreprocessed ||
			    model.HasTransparentCorner ||
			    model.IsArt)
			{
				return bitmap;
			}

			var edited = new Bitmap(bitmap.Width, bitmap.Height);

			bool cornerRemoved;
			try
			{
				var gr = Graphics.FromImage(edited);
				gr.DrawImage(bitmap, new Rectangle(Point.Empty, bitmap.Size));

				var remover = new BmpCornerRemoval(edited, whiteCorner, allowSemitransparent: true);
				remover.Execute();
				cornerRemoved = remover.ImageChanged;
			}
			catch
			{
				cornerRemoved = false;
			}
			
			if (!cornerRemoved)
			{
				edited.Dispose();
				return bitmap;
			}

			if (bitmap != original)
				bitmap.Dispose();

			_log.Debug("Corners fixed: " + model.FullPath);

			return edited;
		}

		private Bitmap tryGetFromCache(string path)
		{
			ImageCacheEntry cacheEntry;
			if (!_imagesByPath.TryGetValue(path, out cacheEntry))
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

		private bool addFirst(string path, Bitmap image)
		{
			if (_imagesByPath.ContainsKey(path))
				return false;

			_ratings.AddFirst(path);
			_imagesByPath[path] = new ImageCacheEntry(image, _ratings.First);

			return true;
		}

		private void removeLast()
		{
			string keyToRemove = _ratings.Last.Value;
			_ratings.RemoveLast();
			_imagesByPath.Remove(keyToRemove);
		}

		private readonly Dictionary<string, ImageCacheEntry> _imagesByPath = new Dictionary<string, ImageCacheEntry>();
		private readonly LinkedList<string> _ratings = new LinkedList<string>();


		public static readonly Size SizeCropped = new Size(205, 293);
		private Size _cardSize = new Size(223, 311);
		private Size _zoomedCardSize = new Size(446, 622);

		public Size CardSize => _cardSize.ByDpi();
		public Size ZoomedCardSize => _zoomedCardSize.ByDpi();

		public int Capacity { get; }
		private readonly bool _transparentCornersWhenNotZoomed;
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}