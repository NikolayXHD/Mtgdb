using System;
using System.Drawing;

namespace Mtgdb.Controls
{
	public class BmpOverwrite: BmpProcessor
	{
		public BmpOverwrite(Bitmap bmp, Bitmap replacement) : base(bmp)
		{
			if (replacement.Size != bmp.Size)
				throw new ArgumentException();

			_replacement = replacement;
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			using (var reader = new BmpReader(_replacement, new Rectangle(default, _replacement.Size)))
				reader.BgraValues.CopyTo(BgraValues, 0);
		}

		private readonly Bitmap _replacement;
	}
}