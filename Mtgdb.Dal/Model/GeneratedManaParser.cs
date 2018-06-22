using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Mtgdb.Dal
{
	internal static class GeneratedManaParser
	{
		public static IList<string> ParseGeneratedMana(Card c)
		{
			var text = c.TextEn;

			if (string.IsNullOrEmpty(text))
				return _empty;

			foreach (string harmfulExplanation in _harmfulExplanations)
				text = text.Replace(harmfulExplanation, string.Empty);

			bool[] matched = new bool[_symbols.Length];

			if (_anyPatterns.Any(_ => _.IsMatch(text)))
				matched[0] = matched[1] = matched[2] = matched[3] = matched[4] = matched[6] = true;

			foreach (var pattern in _specificPatterns)
			{
				var matches = pattern.Matches(text);

				foreach (Match match in matches)
				{
					if (match.Groups["w"].Success)
						matched[0] = true;
					if (match.Groups["u"].Success)
						matched[1] = true;
					if (match.Groups["b"].Success)
						matched[2] = true;
					if (match.Groups["r"].Success)
						matched[3] = true;
					if (match.Groups["g"].Success)
						matched[4] = true;
					if (match.Groups["c"].Success)
						matched[5] = true;
				}
			}

			if (matched.Contains(true))
				matched[7] = c.SupertypesArr?.Contains("Snow", Str.Comparer) == true;

			if (_ePatterns.Any(_ => _.IsMatch(text)))
				matched[8] = true;
			var result = 
				Enumerable.Range(0, matched.Length)
					.Where(i => matched[i])
					.Select(i => _symbols[i]).ToList();

			return result;
		}

		private static readonly string[] _harmfulExplanations = 
		{
			// convoke explanation
			"(Your creatures can help cast this spell. Each creature you tap while casting this spell pays for {1} or one mana of that creature's color.)"
		};

		private static readonly Regex[] _ePatterns =
		{
			new Regex(@"\byou get \{e\}",
				RegexOptions.Compiled | RegexOptions.IgnoreCase)
		};

		private static readonly Regex[] _anyPatterns =
		{
			new Regex(@"\bmana (to his or her mana pool |to your mana pool )?(of (the chosen|any one|any|that)|in any combination of colors|of a color of your choice|of one of the card's colors)",
				RegexOptions.Compiled | RegexOptions.IgnoreCase)
		};

		private static readonly Regex[] _specificPatterns =
		{
			new Regex(
				@"\b(adds?|produces?) (an amount of |an additional |([^ ]+ |that much )?mana in any combination of |that much )?(((?<w>\{w\})|(?<u>\{[u]\})|(?<b>\{[b]\})|(?<r>\{[r]\})|(?<g>\{[g]\})|(?<c>\{[c1-9]\}|colorless mana))( and\/or |, or |, | or )?)+",
				RegexOptions.Compiled | RegexOptions.IgnoreCase)
		};

		private static readonly string[] _symbols =
		{
			"{W}",
			"{U}",
			"{B}",
			"{R}",
			"{G}",
			"{C}",
			"{any}",
			"{S}",
			"{E}"
		};

		private static readonly IList<string> _empty = new List<string>();
	}
}