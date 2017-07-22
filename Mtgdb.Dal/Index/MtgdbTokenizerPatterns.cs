using System.Text.RegularExpressions;

namespace Mtgdb.Dal.Index
{
	public static class MtgdbTokenizerPatterns
	{
		private static readonly string WStr = Regex.Escape(new string(MtgdbTokenizer.WChars));

		public static readonly Regex WPattern = new Regex($@"[\d\w{WStr}]",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static readonly Regex BPattern = new Regex($@"(^|$|[^\d\w{WStr}])",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);
	}
}