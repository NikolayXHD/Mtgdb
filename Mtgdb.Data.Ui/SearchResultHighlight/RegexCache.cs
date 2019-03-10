using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mtgdb.Ui
{
	public static class RegexCache
	{
		private static readonly Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();

		public static Regex Get(string pattern)
		{
			if (_regexCache.TryGetValue(pattern, out var regex))
				return regex;

			regex = new Regex(pattern, RegexOptions.IgnoreCase);
			_regexCache.Add(pattern, regex);

			return regex;
		}
	}
}