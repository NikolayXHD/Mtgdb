using System.Collections.Generic;
using System.Linq;

namespace Mtgdb.Data
{
	public static class MtgAlphabet
	{
		public static IEnumerable<char> GetEquivalents(char c)
		{
			return (_equivalents.TryGet(c) ?? Enumerable.Empty<char>()).Append(c);
		}

		public static readonly HashSet<char> RightDelimitersSet = new HashSet<char>("}");
		public static readonly HashSet<char> LeftDelimitersSet = new HashSet<char>("{");
		// slash included to treat {w/u} as single word
		public static readonly HashSet<char> WordCharsSet = new HashSet<char>("²½_/∞*+-");

		public static bool IsSingletoneWordChar(char c) =>
			SingletoneWordChars.Contains(c) || c.IsCj();

		public static readonly HashSet<char> SingletoneWordChars = new HashSet<char>(".?!:;,‑—–―−\"#$%&'()=@[\\]|¡£«®°´º»¿‚‘’“”„•●™");

		public const string CharPattern = ".";

		public static readonly Dictionary<char, char> Replacements = new Dictionary<char, char>
		{
			{ 'ё', 'е' },

			{ '‑', '-' },
			{ '–', '-' },
			{ '−', '-' },
			{ '—', '-' },
			{ '―', '-' },

			{ 'µ', 'm' },
			{ 'π', 'p' },
			{ '²', '2' },

			{ '•', '*' },
			{ '●', '*' },
			{ '★', '*' },

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
			{ 'ǵ', 'g' },
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
			{ '"', '\'' }
		};

		private static readonly Dictionary<char, List<char>> _equivalents =
			Replacements
				.GroupBy(_ => _.Value)
				.ToDictionary(gr => gr.Key, gr => gr.Select(_ => _.Key).ToList());
	}
}
