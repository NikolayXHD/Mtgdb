using System.Drawing;

namespace Mtgdb.Controls
{
	public class ButtonImages
	{
		public static ButtonImages ScaleDpi(
			(Bitmap Image, Bitmap ImageDouble) uncheckedImg,
			(Bitmap Image, Bitmap ImageDouble) checkedImg = default)
		{
			bool forceLargeIcon = Dpi.ScalePercent > 100;

			return new ButtonImages
			{
				Unchecked = uncheckedImg.ImageDouble != null && (uncheckedImg.Image == null || forceLargeIcon)
					? uncheckedImg.ImageDouble.Shift(new Point(-2, -2)).HalfResizeDpi()
					: uncheckedImg.Image?.Shift(new Point(-1, -1)).ResizeDpi(),

				Checked = checkedImg.ImageDouble != null && (checkedImg.Image == null || forceLargeIcon)
					? checkedImg.ImageDouble.Shift(new Point(-2, -2)).HalfResizeDpi()
					: checkedImg.Image?.Shift(new Point(-1, -1)).ResizeDpi()
			};
		}

		private ButtonImages()
		{
		}

		public ButtonImages(Bitmap uncheckedImage, Bitmap checkedImage)
		{
			Unchecked = uncheckedImage;
			Checked = checkedImage;
		}

		private Bitmap Unchecked { get; set; }
		private Bitmap Checked { get; set; }

		public Bitmap GetImage(bool isChecked) =>
			isChecked
				? Checked ?? Unchecked
				: Unchecked ?? Checked;
	}
}