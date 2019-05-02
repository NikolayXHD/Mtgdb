using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mtgdb.Ui
{
	public static class RegexUtil
	{
		public static Regex GetCached(string pattern)
		{
			if (_regexCache.TryGetValue(pattern, out var regex))
				return regex;

			regex = new Regex(pattern, RegexOptions.IgnoreCase);
			_regexCache.Add(pattern, regex);

			return regex;
		}

		private static readonly Dictionary<string, Regex> _regexCache = new Dictionary<string, Regex>();

		public static readonly Regex WhitespacePattern = new Regex(@"\s+");
	}
}