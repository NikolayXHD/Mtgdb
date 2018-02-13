using System.Text.RegularExpressions;

namespace Mtgdb.Dal
{
	internal static class ImageNamePatcher
	{
		public static void Patch(Card card)
		{
			if (Str.Equals(card.SetCode, "ZEN"))
			{
				// Plains1a.xlhq.jpg -> Plains5.xlhq.jpg

				string modifiedName = null;

				if (card.ImageName.EndsWith("1a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '5';
				else if (card.ImageName.EndsWith("2a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '6';
				else if (card.ImageName.EndsWith("3a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '7';
				else if (card.ImageName.EndsWith("4a", Str.Comparison))
					modifiedName = card.ImageName.Substring(0, card.ImageName.Length - 2) + '8';

				if (modifiedName != null)
					card.ImageName = string.Intern(modifiedName);
			}
			else if (Str.Equals(card.SetCode, "AKH"))
			{
				if (Str.Equals(card.Rarity, "Basic Land"))
				{
					var parts = card.ImageName.SplitTalingNumber();
					card.ImageName = parts.Item1 + (1 + (parts.Item2 - 1 + 3) % 4);
				}
			}
			else if (Str.Equals(card.SetCode, "UST"))
			{
				if (Str.Equals(card.ImageName, "Rumors of My Death . . ."))
					card.ImageName = "_Rumors of My Death . . ._";
				else
					card.ImageName = _unstableSetPattern.Replace(card.ImageName, unstableSetReplacement);
			}
			else if (Str.Equals(card.SetCode, "UNH"))
			{
				if (Str.Equals(card.ImageName, "curse of the fire penguin creature"))
					card.ImageName = "curse of the fire penguin";
				else if (Str.Equals(card.ImageName, "whowhatwhenwherewhy"))
					card.ImageName = "who what when where why";
				else if (Str.Equals(card.ImageName, "richard garfield, ph.d."))
					card.ImageName = "richard garfield, ph.d";
				else if (Str.Equals(card.ImageName, "our market research shows that players like really long card names so we made"))
					card.ImageName = "Our Market Research...";
			}
			else if (Str.Equals(card.SetCode, "UGL"))
			{
				if (Str.Equals(card.ImageName, "b.f.m. (big furry monster)1") ||
					Str.Equals(card.ImageName, "b.f.m. (big furry monster)2"))
				{
					card.ImageName = "b.f.m. 1";
				}
				else if (Str.Equals(card.ImageName, "b.f.m. (big furry monster, right side)1") ||
					Str.Equals(card.ImageName, "b.f.m. (big furry monster, right side)2"))
				{
					card.ImageName = "b.f.m. 2";
				}
				else if (Str.Equals(card.ImageName, "the ultimate nightmare of wizards of the coastr customer service"))
					card.ImageName = "The Ultimate Nightmare of Wizards of the Coast Customer Service";
			}
			else if (Str.Equals(card.SetCode, "DD3_DVD"))
			{
				if (Str.Equals(card.ImageName, "swamp3"))
					card.ImageName = "swamp4";
				else if (Str.Equals(card.ImageName, "swamp4"))
					card.ImageName = "swamp3";
			}
			else if (Str.Equals(card.ImageName, "Sultai Ascendacy"))
			{
				card.ImageName = "Sultai Ascendancy";
			}
			else if (Str.Equals(card.ImageName, "Two-Headed Giant of Foriys"))
			{
				card.ImageName = "Two headed Giant of Foriys";
			}
			else if (Str.Equals(card.ImageName, "Will-O'-The-Wisp"))
			{
				card.ImageName = "Will O' The Wisp";
			}
		}

		private static string unstableSetReplacement(Match match)
		{
			char letterNumber = match.Groups["num"].Value[0];

			var result = (1 + letterNumber - 'a')
				.ToString(Str.Culture);

			return result;
		}

		private static readonly Regex _unstableSetPattern = new Regex(
			@" \((?<num>[a-f])\)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);
	}
}
