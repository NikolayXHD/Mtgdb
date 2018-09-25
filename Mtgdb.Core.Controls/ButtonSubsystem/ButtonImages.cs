using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class ButtonImages
	{
		public ButtonImages(Bitmap image, bool x2)
			:this(image, null, null, null, x2)
		{
		}

		public ButtonImages(Bitmap uncheckedImage, Bitmap checkedImage, bool x2)
			:this(uncheckedImage, checkedImage, null, null, x2)
		{
		}

		public ButtonImages(
			Bitmap uncheckedImage,
			Bitmap checkedImage,
			Bitmap uncheckedHoveredImage,
			Bitmap checkedHoveredImage,
			bool x2)
		{
			Func<Bitmap, Bitmap> scale;

			if (x2)
				scale = bitmap => bitmap?.HalfResizeDpi();
			else
				scale = bitmap => bitmap?.ResizeDpi();

			_unchecked = scale(uncheckedImage);
			_checked = scale(checkedImage);
			_uncheckedHovered = scale(uncheckedHoveredImage);
			_checkedHovered = scale(checkedHoveredImage);
		}

		private readonly Bitmap _unchecked;
		private readonly Bitmap _checked;
		private readonly Bitmap _checkedHovered;
		private readonly Bitmap _uncheckedHovered;

		public Bitmap GetImage(bool isChecked, bool isHovered)
		{
			if (isChecked)
			{
				if (isHovered)
					return _checkedHovered ?? _checked ?? _uncheckedHovered ?? _unchecked;

				return _checked ?? _unchecked;
			}
			else
			{
				if (isHovered)
					return _uncheckedHovered ?? _unchecked ?? _checkedHovered ?? _checked;

				return _unchecked ?? _checked;
			}
		}
	}
}