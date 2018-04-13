using System;
using System.Globalization;

namespace Mtgdb
{
	/// <summary>
	/// Compares whitespaces as smaller than other characters
	/// </summary>
	internal class MtgStringComparer : StringComparer
	{
		public override int Compare(string x, string y)
		{
			if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y))
				return OrdinalIgnoreCase.Compare(x, y);

			int min = Math.Min(x.Length, y.Length);

			for (int i = 0; i < min; i++)
			{
				int compareWhitespace = char.IsWhiteSpace(y[i]).CompareTo(char.IsWhiteSpace(x[i]));

				if (compareWhitespace != 0)
					return compareWhitespace;

				int compareDigit = char.IsDigit(y[i]).CompareTo(char.IsDigit(x[i]));

				if (compareDigit != 0)
					return compareDigit;

				int compareLetter = char.IsLetter(y[i]).CompareTo(char.IsLetter(x[i]));

				if (compareLetter != 0)
					return compareLetter;

				int compareChar = CultureInfo.InvariantCulture.CompareInfo.Compare(x, i, 1, y, i, 1, CompareOptions.IgnoreCase);

				if (compareChar != 0)
					return compareChar;
			}

			return x.Length.CompareTo(y.Length);
		}

		public override bool Equals(string x, string y)
		{
			return OrdinalIgnoreCase.Equals(x, y);
		}

		public override int GetHashCode(string obj)
		{
			return OrdinalIgnoreCase.GetHashCode(obj);
		}
	}
}