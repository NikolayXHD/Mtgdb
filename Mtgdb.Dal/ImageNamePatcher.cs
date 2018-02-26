using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	internal static class ImageNamePatcher
	{
		public static void Patch(Card card)
		{
			if (tryGetFixedName(card, out string fixedName))
			{
				card.ImageName = fixedName;
			}
			else if (_replacementBySet.TryGetValue(card.SetCode, out var regexReplacement))
			{
				var pattern = regexReplacement.Item1;
				var replacement = regexReplacement.Item2;

				card.ImageName = string.Intern(pattern.Replace(card.ImageName, replacement));
			}
			else if (Str.Equals(card.SetCode, "AKH"))
			{
				if (Str.Equals(card.Rarity, "Basic Land"))
				{
					var parts = card.ImageName.SplitTalingNumber();
					card.ImageName = string.Intern(
						parts.Item1 + (1 + (parts.Item2 - 1 + 3) % 4));
				}
			}
		}

		private static bool tryGetFixedName(Card card, out string fixedName)
		{
			if (_fixedNamesBySet[string.Empty].TryGetValue(card.ImageName, out fixedName))
				return true;

			if (_fixedNamesBySet.TryGetValue(card.SetCode, out var setFixedNames))
				return setFixedNames.TryGetValue(card.ImageName, out fixedName);

			return false;
		}

		private static string unstableSetReplacement(Match match)
		{
			char letterNumber = match.Groups["num"].Value[0];

			var result = (1 + letterNumber - 'a')
				.ToString(Str.Culture);

			return result;
		}

		private static string zendikarSetReplacement(Match match)
		{
			// Plains1a.xlhq.jpg -> Plains5.xlhq.jpg

			char letterNumber = match.Groups["num"].Value[0];

			var result = (4 + letterNumber - '0')
				.ToString(Str.Culture);

			return result;
		}

		private static readonly Dictionary<string, Tuple<Regex, MatchEvaluator>> _replacementBySet =
			new Dictionary<string, Tuple<Regex, MatchEvaluator>>(Str.Comparer)
			{
				["UST"] = new Tuple<Regex, MatchEvaluator>(
					new Regex(
						@" \((?<num>[a-f])\)$",
						RegexOptions.Compiled | RegexOptions.IgnoreCase),
					unstableSetReplacement),

				["ZEN"] = new Tuple<Regex, MatchEvaluator>(
					new Regex(
						@"(?<num>[1-4])a$",
						RegexOptions.Compiled | RegexOptions.IgnoreCase),
					zendikarSetReplacement)
			};

		private static readonly Dictionary<string, Dictionary<string, string>> _fixedNamesBySet = new Dictionary<string, Dictionary<string, string>>(Str.Comparer)
		{
			[string.Empty] = new Dictionary<string, string>(Str.Comparer)
			{
				["Sultai Ascendacy"] = "Sultai Ascendancy",
				["Two-Headed Giant of Foriys"] = "Two headed Giant of Foriys",
				["Will-O'-The-Wisp"] = "Will O' The Wisp"
			},

			["DD3_DVD"] = new Dictionary<string, string>(Str.Comparer)
			{
				["swamp3"] = "swamp4",
				["swamp4"] = "swamp3"
			},

			["UGL"] = new Dictionary<string, string>(Str.Comparer)
			{
				["b.f.m. (big furry monster)1"] = "b.f.m. 1",
				["b.f.m. (big furry monster)2"] = "b.f.m. 1",

				["b.f.m. (big furry monster, right side)1"] = "b.f.m. 2",
				["b.f.m. (big furry monster, right side)2"] = "b.f.m. 2",

				["the ultimate nightmare of wizards of the coastr customer service"] = "The Ultimate Nightmare of Wizards of the Coast Customer Service"
			},

			["UNH"] = new Dictionary<string, string>(Str.Comparer)
			{
				["curse of the fire penguin creature"] = "curse of the fire penguin",
				["whowhatwhenwherewhy"] = "who what when where why",
				["richard garfield, ph.d."] = "richard garfield, ph.d",
				["our market research shows that players like really long card names so we made"] = "Our Market Research..."
			},

			["UST"] = new Dictionary<string, string>(Str.Comparer)
			{
				["Rumors of My Death . . ."] = "_Rumors of My Death . . ._"
			}
		};
	}
}
