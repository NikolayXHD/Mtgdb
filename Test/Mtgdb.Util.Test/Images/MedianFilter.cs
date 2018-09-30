using System.Collections.Generic;
using System.Drawing;

namespace Mtgdb.Util
{
	public class MedianFilter : BmpProcessor
	{
		public MedianFilter(Bitmap bmp, int size) : base(bmp)
		{
			_size = size;

			_rValues = new List<byte>();
			_gValues = new List<byte>();
			_bValues = new List<byte>();
		}

		protected override void ExecuteRaw()
		{
			ImageChanged = true;

			int apertureMin = -_size / 2;
			int apertureMax = _size / 2;

			var changed = new byte[BgraValues.Length];

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					_rValues.Clear();
					_gValues.Clear();
					_bValues.Clear();

					for (int dx = apertureMin; dx < apertureMax; ++dx)
					{
						int x2 = x + dx;
						if (x2 < 0 || x2 >= Rect.Width)
							continue;

						for (int dy = apertureMin; dy < apertureMax; ++dy)
						{
							int y2 = y + dy;

							if (y2 < 0 || y2 >= Rect.Height)
								continue;

							int l2 = GetLocation(x2, y2);
							_bValues.Add(BgraValues[l2 + B]);
							_gValues.Add(BgraValues[l2 + G]);
							_rValues.Add(BgraValues[l2 + R]);
						}
					}

					_bValues.Sort();
					_gValues.Sort();
					_rValues.Sort();

					int l = GetLocation(x, y);
					changed[l + B] = _bValues[_bValues.Count / 2];
					changed[l + G] = _gValues[_gValues.Count / 2];
					changed[l + R] = _rValues[_rValues.Count / 2];
					changed[l + A] = BgraValues[l + A];
				}

			changed.CopyTo(BgraValues, 0);
		}

		private readonly int _size;
		private readonly List<byte> _bValues;
		private readonly List<byte> _gValues;
		private readonly List<byte> _rValues;
	}
}