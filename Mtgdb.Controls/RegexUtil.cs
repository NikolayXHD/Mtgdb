using System.Text.RegularExpressions;

namespace Mtgdb.Controls
{
	internal static class RegexUtil
	{
		public static readonly Regex WhitespacePattern = new Regex(@"\s+", RegexOptions.Compiled);
	}
}