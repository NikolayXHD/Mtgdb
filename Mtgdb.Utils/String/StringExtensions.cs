using System;
using System.Globalization;
using System.Text;

namespace Mtgdb
{
	public static class StringExtensions
	{
		public static string RemoveDiacritics(this string value)
		{
			var normalizedString = value.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
				if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
					stringBuilder.Append(c);

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

		public static string TrimComment(this string name)
		{
			return name.Split(Sequence.Array(@"//"), StringSplitOptions.None)[0].TrimEnd();
		}

		public static Tuple<string, int> SplitTailingNumber(this string value)
		{
			if (value == null)
				return new Tuple<string, int>(null, 0);

			if (value.Length == 0)
				return new Tuple<string, int>(string.Empty, 0);

			if (!char.IsDigit(value[value.Length - 1]))
				return new Tuple<string, int>(value, 0);

			for (int i = value.Length - 2; i >= 0; i--)
				if (!char.IsDigit(value[i]))
				{
					string name = value.Substring(0, i + 1);

					string numberStr = value.Substring(i + 1);
					if (!int.TryParse(numberStr, out int number))
						return new Tuple<string, int>(value, 0);

					return new Tuple<string, int>(name, number);
				}

			return new Tuple<string, int>(string.Empty, 0);
		}

		public static (int? Number, string Letter) SplitTailingLetters(this string value)
		{
			if (value == null)
				return (null, null);
			if (value.Length == 0)
				return (null, string.Empty);

			int nonDigitIndex = -1;
			for (int i = 0; i < value.Length; i++)
				if (!char.IsDigit(value[i]))
				{
					nonDigitIndex = i;
					break;
				}

			if (nonDigitIndex == -1)
				return (int.Parse(value, CultureInfo.InvariantCulture), string.Empty);
			if (nonDigitIndex == 0)
				return (null, value);
			return (
				int.Parse(value.Substring(0, nonDigitIndex), CultureInfo.InvariantCulture),
				value.Substring(nonDigitIndex));
		}

		public static string NullIfEmpty(this string value) =>
			value == string.Empty
				? null
				: value;
	}
}
