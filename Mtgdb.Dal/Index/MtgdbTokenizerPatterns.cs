using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Dal.Index
{
	public static class MtgdbTokenizerPatterns
	{
		public static IEnumerable<char> GetEquivalents(char c)
		{
			return (_equivalents.TryGet(c) ?? Enumerable.Empty<char>())
				.Concat(Enumerable.Repeat(c, 1));
		}

		public static readonly HashSet<char> WordCharsSet = new HashSet<char>("̈+-/_{}²½–—―−∞");

		public static readonly HashSet<char> SingletoneWordChars = new HashSet<char>("!\"#$%&'()*:<=>?@[\\]|¡£«®°´º»¿‚‘’“”„•●™");

		public const string CharPattern = ".";

		public static readonly Dictionary<char, char> Replacements = new Dictionary<char, char>
		{
			{ 'ё', 'е' },

			{ '‑', '-' },
			{ '–', '-' },
			{ '—', '-' },
			{ '―', '-' },
			{ '−', '-' },

			{ 'µ', 'm' },
			{ 'π', 'p' },
			{ '²', '2' },

			{ '•', '*' },
			{ '●', '*' },

			{ 'ß', 's' },
			{ 'æ', 'e' },
			{ 'œ', 'e' },

			{ 'à', 'a' },
			{ 'á', 'a' },
			{ 'â', 'a' },
			{ 'ã', 'a' },
			{ 'ä', 'a' },
			{ 'ç', 'c' },
			{ 'è', 'e' },
			{ 'é', 'e' },
			{ 'ê', 'e' },
			{ 'ë', 'e' },
			{ 'ì', 'i' },
			{ 'í', 'i' },
			{ 'î', 'i' },
			{ 'ï', 'i' },
			{ 'ñ', 'n' },
			{ 'ò', 'o' },
			{ 'ó', 'o' },
			{ 'ô', 'o' },
			{ 'õ', 'o' },
			{ 'ö', 'o' },
			{ 'ù', 'u' },
			{ 'ú', 'u' },
			{ 'û', 'u' },
			{ 'ü', 'u' },
			{ 'ć', 'c' },
			{ 'ł', 'l' },
			{ 'ń', 'n' },
			{ 'ŵ', 'w' },


			{ '´', '\'' },
			{ '«', '\'' },
			{ '»', '\'' },
			{ '‚', '\'' },
			{ '‘', '\'' },
			{ '’', '\'' },
			{ '“', '\'' },
			{ '”', '\'' },
			{ '„', '\'' },
			{ '"', '\'' },
		};

		private static readonly Dictionary<char, List<char>> _equivalents =
			Replacements
				.GroupBy(_ => _.Value)
				.ToDictionary(gr => gr.Key, gr => gr.Select(_ => _.Key).ToList());
	}
}