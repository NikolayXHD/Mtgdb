using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mtgdb
{
	public static class StringExtensions
	{
		public static string Parent(this string dir)
		{
			return Path.GetDirectoryName(dir);
		}

		public static string LastPathSegment(this string dir)
		{
			return Path.GetFileName(dir);
		}

		public static string AddPath(this string original, string dir)
		{
			return Path.Combine(original, dir);
		}

		public static string RemoveDiacritics(this string value)
		{
			var normalizedString = value.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
				if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
					stringBuilder.Append(c);

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		public static string Truncate(this string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value)) return value;
			return value.Length <= maxLength ? value : value.Substring(0, maxLength);
		}

		public static string TrimComment(this string name)
		{
			return name.Split(new[] { @"//" }, StringSplitOptions.None)[0].TrimEnd();
		}

		public static Tuple<string, int> SplitTalingNumber(this string value)
		{
			if (value == null)
				return new Tuple<string, int>(null, 0);

			if (value.Length == 0)
				return new Tuple<string, int>(String.Empty, 0);

			if (!Char.IsDigit(value[value.Length - 1]))
				return new Tuple<string, int>(value, 0);

			for (int i = value.Length - 2; i >= 0; i--)
				if (!Char.IsDigit(value[i]))
					return new Tuple<string, int>(value.Substring(0, i + 1), Int32.Parse(value.Substring(i + 1)));

			return new Tuple<string, int>(String.Empty, 0);
		}

		public static string NullIfEmpty(this string value)
		{
			if (value == string.Empty)
				return null;

			return value;
		}
	}
}