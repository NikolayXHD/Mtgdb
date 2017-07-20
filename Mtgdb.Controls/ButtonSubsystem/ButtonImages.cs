using System.Drawing;

namespace Mtgdb.Controls
{
	public class ButtonImages
	{
		public ButtonImages(Bitmap uncheckedImage, Bitmap uncheckedHoveredImage)
		{
			_unchecked = uncheckedImage;
			_uncheckedHovered = uncheckedHoveredImage;
		}

		public ButtonImages(Bitmap uncheckedImage, Bitmap checkedImage, Bitmap uncheckedHoveredImage, Bitmap checkedHoveredImage)
		{
			_unchecked = uncheckedImage;
			_checked = checkedImage;
			_uncheckedHovered = uncheckedHoveredImage;
			_checkedHovered = checkedHoveredImage;
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