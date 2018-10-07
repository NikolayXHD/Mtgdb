using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class ButtonImages
	{
		public ButtonImages(Bitmap image, bool x2)
			: this(image, null, x2)
		{
		}

		public ButtonImages(Bitmap uncheckedImage, Bitmap checkedImage, bool x2)
		{
			Func<Bitmap, Bitmap> scale;

			if (x2)
				scale = bitmap => bitmap?.Shift(new Point(-2, -2)).HalfResizeDpi();
			else
				scale = bitmap => bitmap?.Shift(new Point(-1, -1)).ResizeDpi();

			_unchecked = scale(uncheckedImage);
			_checked = scale(checkedImage);
		}

		private readonly Bitmap _unchecked;
		private readonly Bitmap _checked;

		public Bitmap GetImage(bool isChecked) =>
			isChecked
				? _checked ?? _unchecked
				: _unchecked ?? _checked;
	}
}