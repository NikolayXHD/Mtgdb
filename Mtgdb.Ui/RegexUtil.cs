using System.Text.RegularExpressions;

namespace Mtgdb.Ui
{
	internal static class RegexUtil
	{
		public static readonly Regex WhitespacePattern = new Regex(@"\s+");
	}
}