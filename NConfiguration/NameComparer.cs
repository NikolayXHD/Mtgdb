using System;
using System.Collections.Generic;

namespace NConfiguration
{
	internal class NameComparer
	{
		public static readonly IEqualityComparer<string> Instance = StringComparer.OrdinalIgnoreCase;

		public static bool Equals(string x, string y)
		{
			return Instance.Equals(x, y);
		}
	}
}
