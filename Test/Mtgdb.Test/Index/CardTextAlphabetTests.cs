using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
		public void All_symbols_in_card_fields_are_known()
		{
			// ReSharper disable StringLiteralTypo
			var latin = new HashSet<char>("abcdefghijklmnopqrstuvwxyz");
			var cyrillic = new HashSet<char>("абвгдежзийклмнопрстуфхцчшщьыъэюя");
			var numbers = new HashSet<char>("01234567890");
			var knownSpecialChars = new HashSet<char>("ºß"); // artist name
			// ReSharper restore StringLiteralTypo

			var alphabet = new HashSet<char>();

			var languages = new HashSet<string>(CardLocalization.GetAllLanguages(), Str.Comparer);
			languages.Remove("cn");
			languages.Remove("tw");
			languages.Remove("jp");
			languages.Remove("kr");

			var empty = Enumerable.Empty<char>();

			foreach (var set in Repo.SetsByCode.Values)
			{
				alphabet.UnionWith(set.Name);
				alphabet.UnionWith(set.Code);
			}

			var failedCards = new List<(char[], Card)>();
			foreach (var card in Repo.Cards)
			{
				var cardChars =new HashSet<char>();
				cardChars.UnionWith(card.NameEn ?? empty);
				cardChars.UnionWith(card.TypeEn ?? empty);
				cardChars.UnionWith(card.FlavorEn ?? empty);
				cardChars.UnionWith(card.TextEn ?? empty);
				cardChars.UnionWith(card.OriginalText ?? empty);
				cardChars.UnionWith(card.OriginalType ?? empty);
				cardChars.UnionWith(card.Artist?.Where(_ => !_.IsCjk()) ?? empty);

				foreach (string lang in languages)
				{
					cardChars.UnionWith(card.GetName(lang) ?? empty);
					cardChars.UnionWith(card.GetType(lang) ?? empty);
					cardChars.UnionWith(card.GetFlavor(lang) ?? empty);
					cardChars.UnionWith(card.GetText(lang) ?? empty);
				}

				var badChars = cardChars.Where(isUnknownChar).Where(shouldBeConsidered).ToArray();
				if (badChars.Length > 0)
					failedCards.Add((badChars, card));

				alphabet.UnionWith(cardChars);
			}

			var chars = alphabet.Select(c=> char.ToLower(c, Str.Culture)).Distinct().OrderBy(c => c).ToArray();
			Log.Debug(() => new string(chars));

			var unknownChars = chars.Where(isUnknownChar).ToArray();
			Log.Debug(new string(unknownChars.ToArray()));

			var notConsideredChars = new string(unknownChars.Where(shouldBeConsidered).ToArray());

			Assert.That(failedCards, Is.Empty, "Bad symbols in {0} cards", failedCards.Count);

			Assert.That(notConsideredChars, Is.Empty);

			static bool shouldBeConsidered(char c) =>
				char.IsLetterOrDigit(c);

			bool isUnknownChar(char c)
			{
				c = char.ToLower(c);

				if (latin.Contains(c))
					return false;

				if (cyrillic.Contains(c))
					return false;

				if (numbers.Contains(c))
					return false;

				if (c == '\n')
					return false;

				if (c == '\r')
					return false;

				if (MtgAlphabet.Replacements.ContainsKey(c))
					return false;

				if (MtgAlphabet.ExtraWordChars.Contains(c))
					return false;

				if (MtgAlphabet.LeftDelimitersSet.Contains(c))
					return false;

				if (MtgAlphabet.RightDelimitersSet.Contains(c))
					return false;

				if (MtgAlphabet.SingletoneWordChars.Contains(c))
					return false;

				if (knownSpecialChars.Contains(c))
					return false;

				return true;
			}
		}

		[Test]
		public void All_symbols_in_text_are_known()
		{
			var known = new HashSet<char>("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
			known.UnionWith(MtgAlphabet.SingletoneWordChars);
			known.UnionWith(MtgAlphabet.ExtraWordChars);
			known.UnionWith(" .?!,;:'\"()[]{}\r\n");
			known.UnionWith("âáàãéíñöŠûúüπ");
			known.Add('−'); // planeswalker minus loyalty sign
			known.Add('☐'); // checkbox

			Repo.Cards.Select(_=>_.TextEn).Where(F.IsNotNull).Should()
				.NotContain(str => new string(str.Where(c => !known.Contains(c)).Distinct().ToArray()).Length > 0);

			Repo.Cards.Select(_=>_.NameEn).Where(F.IsNotNull).Should()
				.NotContain(str => new string(str.Where(c => !known.Contains(c)).Distinct().ToArray()).Length > 0);

			Repo.Cards.Select(_=>_.TypeEn).Where(F.IsNotNull).Should()
				.NotContain(str => new string(str.Where(c => !known.Contains(c)).Distinct().ToArray()).Length > 0);
		}
	}
}
