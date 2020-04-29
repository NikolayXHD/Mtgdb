using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Data
{
	internal class ImageCacheEntry
	{
		public ImageCacheEntry(Bitmap image, LinkedListNode<Tuple<FsPath, RotateFlipType>> ratingEntry)
		{
			RatingEntry = ratingEntry;
			Image = image;
		}

		public Bitmap Image { get; }
		public LinkedListNode<Tuple<FsPath, RotateFlipType>> RatingEntry { get; }
	}
}
