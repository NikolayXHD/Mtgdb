using System.Text;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public static class RegexUtil
	{
		public static Regex CreateContainsRegex(string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			var builder = new StringBuilder();
			if (char.IsLetterOrDigit(value[0]))
				builder.Append("\\b");

			builder.Append(Regex.Escape(value));

			if (char.IsLetterOrDigit(value[value.Length - 1]))
				builder.Append("\\b");

			var result = new Regex(builder.ToString(),
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

			return result;
		}

		public static Regex CreateEqualsRegex(string value)
		{
			if (value == null)
				return null;

			var result = new Regex($@"^{Regex.Escape(value)}$",
				RegexOptions.Compiled | RegexOptions.IgnoreCase);

			return result;
		}
	}
}