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

		private const string WordChars = @"&*+-/?_{}²½–—’•−∞";
		public static readonly HashSet<char> WordCharsSet = new HashSet<char>(WordChars);
		
		public const string CharPattern = ".";

		public static readonly Dictionary<char, char> Replacements = new Dictionary<char, char>
		{
			{ '−', '-' },
			{ '–', '-' },
			{ '—', '-' },
			{ 'û', 'u' },
			{ 'ö', 'o' },
			{ '’', '\'' }
		};

		private static readonly Dictionary<char, List<char>> _equivalents =
			Replacements
				.GroupBy(_ => _.Value)
				.ToDictionary(gr => gr.Key, gr => gr.Select(_ => _.Key).ToList());
	}
}