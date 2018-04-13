using System;
using System.Globalization;

namespace Mtgdb
{
	public static class Str
	{
		public static readonly StringComparer Comparer = new MtgStringComparer();
		public const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

		public static int Compare(string x, string y)
		{
			return Comparer.Compare(x, y);
		}

		public static bool Equals(string x, string y)
		{
			return Comparer.Equals(x, y);
		}

		public static readonly string Endl = Environment.NewLine;

		public static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
	}
}