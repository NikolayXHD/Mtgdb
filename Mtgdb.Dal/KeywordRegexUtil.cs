using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	public static class KeywordRegexUtil
	{
		public static Regex CreateContainsRegex(string value)
		{
			if (string.IsNullOrEmpty(value))
				return null;

			if (isRegexLiteral(value))
			{
				return new Regex(getRegexLiteralValue(value),
					/*RegexOptions.Compiled |*/ RegexOptions.IgnoreCase);
			}

			var builder = new StringBuilder();
			if (char.IsLetterOrDigit(value[0]))
				builder.Append("\\b");

			builder.Append(Regex.Escape(value));

			if (char.IsLetterOrDigit(value[value.Length - 1]))
				builder.Append("\\b");

			var result = new Regex(builder.ToString(),
				/*RegexOptions.Compiled |*/ RegexOptions.IgnoreCase);

			return result;
		}

		public static Regex CreateEqualsRegex(string value)
		{
			if (value == null)
				return null;

			if (isRegexLiteral(value))
			{
				return new Regex(getRegexLiteralValue(value),
					/*RegexOptions.Compiled |*/ RegexOptions.IgnoreCase);
			}

			var result = new Regex($@"^{Regex.Escape(value)}$",
				/*RegexOptions.Compiled |*/ RegexOptions.IgnoreCase);

			return result;
		}

		private static string getRegexLiteralValue(string value)
		{
			int beginsAt = value.IndexOf('/');
			int endsAt = value.LastIndexOf('/');

			string result = value.Substring(beginsAt + 1, endsAt - beginsAt - 1);
			return result;
		}

		private static bool isRegexLiteral(string value)
		{
			return value != null && value.IndexOf('/') == 0 && value.LastIndexOf('/') > 0;
		}

		public static IList<string> ToKeywordDisplayTexts(this IEnumerable<string> keywordValues)
		{
			return keywordValues.Select(GetKeywordDisplayText).ToArray();
		}

		public static IList<IList<string>> ToKeywordDisplayTexts(this IEnumerable<IList<string>> keywordValues)
		{
			return keywordValues.Select(ToKeywordDisplayTexts).ToArray();
		}

		public static string GetKeywordDisplayText(string value)
		{
			if (!isRegexLiteral(value))
				return value;

			int endsAt = value.LastIndexOf('/');

			if (endsAt == value.Length - 1)
				return getRegexLiteralValue(value);

			string result = value.Substring(endsAt + 1).Trim();

			return result;
		}
	}
}