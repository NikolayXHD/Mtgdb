using System.Linq;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	internal static class GeneratedManaParser
	{
		private static readonly Regex _manaRegex = new Regex(
			@"(\byou get (?<e>\{e\}))|(?<any>\bmana ((to his or her mana pool )|(to your mana pool )|)of ((the chosen )|(any one )|(any )|(that ))\b)|(\badd(s|) ((an amount of )|([^ ]+ mana in any combination of )|(that much )|)(((?<w>\{w\})|(?<u>\{[u]\})|(?<b>\{[b]\})|(?<r>\{[r]\})|(?<g>\{[g]\})|(?<c>\{[c1-9]\}))(( and/or )|(, or )|(, )|( or )|))+)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly string[] _symbols =
		{
			"{W}",
			"{U}",
			"{B}",
			"{R}",
			"{G}",
			"{C}",
			"{E}"
		};

		public static string ParseGeneratedMana(string text)
		{
			if (string.IsNullOrEmpty(text))
				return string.Empty;

			var matches = _manaRegex.Matches(text);
			bool[] matched = new bool[_symbols.Length];

			foreach (Match match in matches)
			{
				if (match.Groups["any"].Success)
					matched[0] = matched[1] = matched[2] = matched[3] = matched[4] = true;
				else
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
				}
				
				if (match.Groups["c"].Success)
					matched[5] = true;

				if (match.Groups["e"].Success)
					matched[6] = true;
			}

			var result = string.Concat(
				Enumerable.Range(0, matched.Length)
					.Where(i => matched[i])
					.Select(i => _symbols[i]));

			return result;
		}
	}
}