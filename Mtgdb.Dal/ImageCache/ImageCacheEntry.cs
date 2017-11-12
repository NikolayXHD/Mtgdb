using System;
using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Dal
{
	internal class ImageCacheEntry
	{
		public ImageCacheEntry(Bitmap image, LinkedListNode<Tuple<string, bool>> ratingEntry)
		{
			RatingEntry = ratingEntry;
			Image = image;
		}

		public Bitmap Image { get; }

		public LinkedListNode<Tuple<string, bool>> RatingEntry { get; }
	}
}