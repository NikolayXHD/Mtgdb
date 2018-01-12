using System.Collections.Generic;
using System.Drawing;
using Mtgdb.Controls;

namespace Mtgdb.Dal
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

			int apetureMin = -_size / 2;
			int apetureMax = _size / 2;

			var changed = new byte[RgbValues.Length];

			for (int x = 0; x < Rect.Width; ++x)
				for (int y = 0; y < Rect.Height; ++y)
				{
					_rValues.Clear();
					_gValues.Clear();
					_bValues.Clear();

					for (int dx = apetureMin; dx < apetureMax; ++dx)
					{
						int x2 = x + dx;
						if (x2 < 0 || x2 >= Rect.Width)
							continue;

						for (int dy = apetureMin; dy < apetureMax; ++dy)
						{
							int y2 = y + dy;

							if (y2 < 0 || y2 >= Rect.Height)
								continue;

							int l2 = GetLocation(x2, y2);
							_rValues.Add(RgbValues[l2]);
							_gValues.Add(RgbValues[l2 + 1]);
							_bValues.Add(RgbValues[l2 + 2]);
						}
					}

					_rValues.Sort();
					_gValues.Sort();
					_bValues.Sort();

					int l = GetLocation(x, y);
					changed[l] = _rValues[_rValues.Count / 2];
					changed[l + 1] = _gValues[_gValues.Count / 2];
					changed[l + 2] = _bValues[_bValues.Count / 2];
					changed[l + 3] = RgbValues[l + 3];
				}

			changed.CopyTo(RgbValues, 0);
		}

		private readonly int _size;
		private readonly List<byte> _rValues;
		private readonly List<byte> _gValues;
		private readonly List<byte> _bValues;
	}
}