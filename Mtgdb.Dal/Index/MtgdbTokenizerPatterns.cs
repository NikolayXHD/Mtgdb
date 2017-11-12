using System.Text.RegularExpressions;

namespace Mtgdb.Dal.Index
{
	public static class MtgdbTokenizerPatterns
	{
		private static readonly string _word = Regex.Escape(MtgdbTokenizer.WordChars);

		public static readonly Regex Word = new Regex($@"[\d\w{_word}]",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static readonly Regex Boundary = new Regex($@"(^|$|[^\d\w{_word}])",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);
	}
}