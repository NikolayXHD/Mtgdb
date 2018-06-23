using System.Drawing;

namespace Mtgdb.Dal
{
	public class ImageModel
	{
		public ImageModel(ImageFile imageFile, RotateFlipType rotation = RotateFlipType.RotateNoneFlipNone)
		{
			ImageFile = imageFile;
			Rotation = rotation;
		}

		public ImageFile ImageFile { get; }
		public RotateFlipType Rotation { get; }
	}
}