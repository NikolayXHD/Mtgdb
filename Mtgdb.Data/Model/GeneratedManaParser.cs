using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Data
{
	internal static class GeneratedManaParser
	{
		public static (IList<string> Normal, IList<string> AnyColorExpanded) ParseGeneratedMana(Card c)
		{
			var text = c.TextEn;

			if (string.IsNullOrEmpty(text))
				return (_empty, _empty);

			foreach (string harmfulExplanation in _harmfulExplanations)
				text = text.Replace(harmfulExplanation, string.Empty);

			bool[] matchedExpanded = new bool[_symbols.Length];
			bool[] matched = new bool[_symbols.Length];

			if (_anyPatterns.Any(_ => _.IsMatch(text)))
				matchedExpanded[0] =
					matchedExpanded[1] =
						matchedExpanded[2] =
							matchedExpanded[3] =
								matchedExpanded[4] =
									matchedExpanded[6] =
										matched[6] = true;

			foreach (var pattern in _specificPatterns)
			{
				var matches = pattern.Matches(text);

				foreach (Match match in matches)
				{
					if (match.Groups["w"].Success)
						matched[0] = matchedExpanded[0] = true;
					if (match.Groups["u"].Success)
						matched[1] = matchedExpanded[1] = true;
					if (match.Groups["b"].Success)
						matched[2] = matchedExpanded[2] = true;
					if (match.Groups["r"].Success)
						matched[3] = 	matchedExpanded[3] = true;
					if (match.Groups["g"].Success)
						matched[4] = matchedExpanded[4] = true;
					if (match.Groups["c"].Success)
						matched[5] = matchedExpanded[5] = true;
				}
			}

			if (matchedExpanded.Contains(true))
				matched[7] = matchedExpanded[7] = c.SupertypesArr?.Contains("Snow", Str.Comparer) == true;

			if (_ePatterns.Any(_ => _.IsMatch(text)))
				matched[8] = matchedExpanded[8] = true;
			
			var resultExpanded = 
				Enumerable.Range(0, matchedExpanded.Length)
					.Where(i => matchedExpanded[i])
					.Select(i => _symbols[i]).ToList();

			var result = 
				Enumerable.Range(0, matched.Length)
					.Where(i => matched[i])
					.Select(i => _symbols[i]).ToList();

			return (result, resultExpanded);
		}

		private static readonly string[] _harmfulExplanations = 
		{
			// convoke explanation
			"(Your creatures can help cast this spell. Each creature you tap while casting this spell pays for {1} or one mana of that creature's color.)"
		};

		private static readonly Regex[] _ePatterns =
		{
			new Regex(@"\byou get \{e\}", RegexOptions.IgnoreCase)
		};

		private static readonly Regex[] _anyPatterns =
		{
			new Regex(@"\bmana (to his or her mana pool |to your mana pool )?(of (the chosen|any one|any|that)|in any combination of colors|of a color of your choice|of one of the card's colors)",
				RegexOptions.IgnoreCase)
		};

		private static readonly Regex[] _specificPatterns =
		{
			new Regex(
				@"\b(adds?|produces?) (an amount of |an additional |([^ ]+ |that much )?mana in any combination of |that much )?(((?<w>\{w\})|(?<u>\{[u]\})|(?<b>\{[b]\})|(?<r>\{[r]\})|(?<g>\{[g]\})|(?<c>\{[c1-9]\}|colorless mana))( and\/or |, or |, | or )?)+",
				RegexOptions.IgnoreCase)
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