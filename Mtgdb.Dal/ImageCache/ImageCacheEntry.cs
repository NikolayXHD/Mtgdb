using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Dal
{
	internal class ImageCacheEntry
	{
		public ImageCacheEntry(Bitmap image, LinkedListNode<string> ratingEntry)
		{
			RatingEntry = ratingEntry;
			Image = image;
		}

		public Bitmap Image { get; }

		public LinkedListNode<string> RatingEntry { get; }
	}
}