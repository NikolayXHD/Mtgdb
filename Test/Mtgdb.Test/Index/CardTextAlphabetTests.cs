using System.Collections.Generic;
using System.Linq;
using Mtgdb.Data;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class CardTextAlphabetTests : TestsBase
	{
		[OneTimeSetUp]
		public static void Setup()
		{
			LoadTranslations();
		}

		[Test]
		public void All_symbols_in_card_texts_are_considered_in_code()
		{
			var alphabet = new HashSet<char>();
				
			var languages = new HashSet<string>(CardLocalization.GetAllLanguages(), Str.Comparer);
			languages.Remove("cn");
			languages.Remove("tw");
			languages.Remove("jp");
			languages.Remove("kr");

			foreach (var set in Repo.SetsByCode.Values)
			{
				alphabet.UnionWith(set.Name);
				alphabet.UnionWith(set.Code);
			}

			foreach (var card in Repo.Cards)
			{
				alphabet.UnionWithNullable(card.NameEn);
				alphabet.UnionWithNullable(card.TypeEn);
				alphabet.UnionWithNullable(card.FlavorEn);
				alphabet.UnionWithNullable(card.TextEn);
				alphabet.UnionWithNullable(card.Artist);

				foreach (string lang in languages)
				{
					alphabet.UnionWithNullable(card.GetName(lang));
					alphabet.UnionWithNullable(card.GetType(lang));
					alphabet.UnionWithNullable(card.GetFlavor(lang));
					alphabet.UnionWithNullable(card.GetText(lang));
				}
			}

			var chars = alphabet.Select(c=> char.ToLower(c, Str.Culture)).Distinct().OrderBy(c => c).ToArray();

			Log.Debug(() => new string(chars));

			var specialChars = new List<char>();

			// ReSharper disable StringLiteralTypo
			var latin = new HashSet<char>("abcdefghijklmnopqrstuvwxyz");
			var cyrillic = new HashSet<char>("абвгдежзийклмнопрстуфхцчшщьыъэюя");
			var numbers = new HashSet<char>("01234567890");
			var knownSpecialChars = new HashSet<char>("ºß林泰玄");
			// ReSharper restore StringLiteralTypo

			foreach (char c in chars)
			{
				if (latin.Contains(c))
					continue;

				if (cyrillic.Contains(c))
					continue;

				if (numbers.Contains(c))
					continue;

				if (c == '\n')
					continue;

				if (c == '\r')
					continue;

				if (MtgAlphabet.Replacements.ContainsKey(c))
					continue;

				if (MtgAlphabet.WordCharsSet.Contains(c))
					continue;

				if (MtgAlphabet.LeftDelimitersSet.Contains(c))
					continue;

				if (MtgAlphabet.RightDelimitersSet.Contains(c))
					continue;

				if (MtgAlphabet.SingletoneWordChars.Contains(c))
					continue;

				if (knownSpecialChars.Contains(c))
					continue;

				specialChars.Add(c);
			}

			var specialCharsStr = new string(specialChars.ToArray());
			Log.Debug(specialCharsStr);

			var notConsideredChars = new string(specialCharsStr.Where(char.IsLetterOrDigit).ToArray());
			Assert.That(notConsideredChars, Is.Empty);
		}
	}
}