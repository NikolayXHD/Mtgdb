using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Mtgdb.Controls;
using Mtgdb.Gui;

namespace Mtgdb.Dal
{
	public class ImageCache
	{
		public static readonly Size SizeCropped = new Size(246, 343);

		private readonly Dictionary<string, ImageCacheEntry> _imagesByPath = new Dictionary<string, ImageCacheEntry>();
		private readonly LinkedList<string> _ratings = new LinkedList<string>();

		public Size CardSizeDefault { get; } = new Size(215, 300);
		public Size CardSize { get; } = new Size(215, 300);
		public Size ZoomedCardSize { get; } = new Size(427, 596);

		public int Capacity { get; }
		private readonly bool _transparentCornersWhenNotZoomed;

		public ImageCache(ImageCacheConfig config, CardSizeConfig cardSizeConfig, ZoomedCardSizeConfig zoomedCardSizeConfig)
		{
			if (cardSizeConfig.Width.HasValue && cardSizeConfig.Height.HasValue)
				CardSize = new Size(cardSizeConfig.Width.Value, cardSizeConfig.Height.Value);

			if (zoomedCardSizeConfig.Width.HasValue && zoomedCardSizeConfig.Height.HasValue)
				ZoomedCardSize = new Size(zoomedCardSizeConfig.Width.Value, zoomedCardSizeConfig.Height.Value);

			Capacity = config.GetCacheCapacity();
			_transparentCornersWhenNotZoomed = config.TransparentCornersWhenNotZoomed ?? true;
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
			Bitmap bitmap;
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
					bitmap = original.Scale(size, frame);
				}
				catch
				{
					bitmap = original;
				}
			}
			else
				bitmap = original;

			if (!transparentCorners && !whiteCorner ||
			    size == CardSize && model.IsPreprocessed ||
			    model.HasTransparentCorner ||
			    model.IsArt)
			{
				return bitmap;
			}

			using (bitmap)
			{
				var edited = new Bitmap(bitmap.Size.Width, bitmap.Size.Height);

				try
				{
					var gr = Graphics.FromImage(edited);
					gr.DrawImage(bitmap, new Rectangle(new Point(0, 0), bitmap.Size));
					new BmpCornerRemoval(edited, whiteCorner, allowSemitransparent: size == CardSize).Execute();
					return edited;
				}
				catch
				{
					edited.Dispose();
					return null;
				}
			}
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
	}
}