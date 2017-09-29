using System;

namespace Mtgdb
{
	public class VersionComparer : StringComparer
	{
		private static readonly StringComparer _partsComparer = InvariantCultureIgnoreCase;

		public override int Compare(string x, string y)
		{
			if (x == null && y == null)
				return 0;
			if (x == null)
				return 1;
			if (y == null)
				return -1;

			var partsX = x.TrimStart('v').Split('.');
			var partsY = y.TrimStart('v').Split('.');

			int compareResult;

			int minLen = Math.Min(partsX.Length, partsY.Length);
			for (int i = 0; i < minLen; i++)
			{
				int vX, vY;
				if (int.TryParse(partsX[i], out vX) && int.TryParse(partsY[i], out vY))
					compareResult = vX.CompareTo(vY);
				else
					compareResult = _partsComparer.Compare(partsX[i], partsY[i]);
				if (compareResult != 0)
					return compareResult;
			}

			compareResult = partsX.Length.CompareTo(partsY.Length);
			return compareResult;
		}

		public override bool Equals(string x, string y)
		{
			return _partsComparer.Equals(x, y);
		}

		public override int GetHashCode(string obj)
		{
			return _partsComparer.GetHashCode(obj);
		}
	}
}