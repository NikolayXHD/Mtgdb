using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mtgdb.Dal;
using NLog;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class LuceneSearcherTests : IndexTestsBase
	{
		[OneTimeSetUp]
		public new void Setup()
		{
			LoadPrices();
			LogManager.Flush();
		}

		[TestCase(@"NameEn:Forest", "Forest")]
		public void Search_by_NameEn(string queryStr, string expectedName)
		{
			var cards = search(queryStr, c => c.NameEn);

			foreach (var card in cards)
				Assert.That(card.NameEn, Contains.Substring(expectedName).IgnoreCase);
		}

		[TestCase(@"TextEn:embalm", "embalm")]
		public void Search_by_TextEn(string queryStr, string expectedText)
		{
			var cards = search(queryStr, c => c.TextEn + Environment.NewLine);

			foreach (var card in cards)
				Assert.That(card.TextEn, Contains.Substring(expectedText).IgnoreCase);
		}

		[TestCase(@"FlavorEn:angel", "angel")]
		public void Search_by_FlavorEn(string queryStr, string expectedFlavor)
		{
			var cards = search(queryStr, c => c.FlavorEn + Environment.NewLine);

			foreach (var card in cards)
				Assert.That(card.FlavorEn, Contains.Substring(expectedFlavor).IgnoreCase);
		}

		[TestCase(@"SetName:""Battle for""", "battle", "for")]
		[TestCase(@"SetName:""Battle zendikar""~1", "battle", "zendikar")]
		public void Search_by_SetName(string queryStr, params string[] expectedValues)
		{
			var cards = search(queryStr, c => c.SetName);

			Assert.That(expectedValues, Is.Not.Null);
			Assert.That(expectedValues, Is.Not.Empty);

			foreach (var card in cards)
				foreach (string name in expectedValues)
					Assert.That(card.SetName, Contains.Substring(name).IgnoreCase);
		}

		[TestCase("SetCode:LEA", "LEA")]
		public void Search_by_SetCode(string queryStr, string expectedSetCode)
		{
			var cards = search(queryStr, c => c.SetCode);

			foreach (var card in cards)
				StringAssert.AreEqualIgnoringCase(card.SetCode, expectedSetCode);
		}

		[TestCase("SetCode:LE?", "LE", 3)]
		public void Search_by_SetCode_wildcard(string queryStr, string expectedPrefix, int expectedLength)
		{
			var cards = search(queryStr, c => c.SetCode);

			foreach (var card in cards)
			{
				Assert.That(card.SetCode, Does.StartWith(expectedPrefix).IgnoreCase);
				Assert.That(card.SetCode.Length, Is.EqualTo(expectedLength));
			}
		}

		[TestCase(@"Artist:James", "James")]
		public void Search_by_Artist(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.Artist);

			foreach (var card in cards)
				Assert.That(card.Artist, Contains.Substring(expected).IgnoreCase);
		}

		[TestCase(@"TypeEn:Human", "Human")]
		[TestCase(@"TypeEn:""Legendary Human""~2", "Legendary", "Human")]
		public void Search_by_TypeEn(string queryStr, params string[] expectedValues)
		{
			var cards = search(queryStr, c => c.TypeEn);

			foreach (var card in cards)
				foreach (string name in expectedValues)
					Assert.That(card.TypeEn, Contains.Substring(name).IgnoreCase);
		}

		[TestCase(@"Types:""Artifact Creature""", "Artifact", "Creature")]
		[TestCase(@"Types:Creature AND Types:Artifact", "Artifact", "Creature")]
		public void Search_by_Types(string queryStr, params string[] allExpected)
		{
			var cards = search(queryStr, c => string.Join(" ", c.TypesArr ?? Enumerable.Empty<string>()));

			foreach (var card in cards)
				Assert.That(card.TypesArr, Is.SupersetOf(allExpected).IgnoreCase);
		}

		[TestCase(@"Types:*fact", "fact")]
		public void Search_by_Types_suffix(string queryStr, string suffix)
		{
			var cards = search(queryStr, c => string.Join(" ", c.TypesArr ?? Enumerable.Empty<string>()));

			foreach (var card in cards)
				Assert.That(card.TypesArr, Has.Some.EndsWith(suffix).IgnoreCase);
		}

		[TestCase(@"Supertypes:Legendary", "Legendary")]
		public void Search_by_Supertypes(string queryStr, string expected)
		{
			var cards = search(queryStr, c => string.Join(" ", c.SupertypesArr ?? Enumerable.Empty<string>()));

			foreach (var card in cards)
				Assert.That(card.SupertypesArr, Does.Contain(expected).IgnoreCase);
		}

		[TestCase(@"Subtypes:Human AND Subtypes: Rogue", "Human", "Rogue")]
		[TestCase(@"Subtypes:""Human Rogue""", "human", "rogue")]
		public void Search_by_Subtypes_all(string queryStr, params string[] allExpected)
		{
			var cards = search(queryStr, c => string.Join(" ", c.SubtypesArr ?? Enumerable.Empty<string>()));

			foreach (var card in cards)
				Assert.That(card.SubtypesArr, Is.SupersetOf(allExpected).IgnoreCase);
		}

		[TestCase(@"RestrictedIn:Vintage", "vintage")]
		public void Search_by_RestrictedIn(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.RestrictedIn);

			foreach (var card in cards)
				Assert.That(card.IsRestrictedIn(expected));
		}

		[TestCase(@"BannedIn:Modern", "Modern")]
		public void Search_by_BannedIn(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.BannedIn);

			foreach (var card in cards)
				Assert.That(card.IsBannedIn(expected));
		}

		[TestCase(@"LegalIn:Standard", "standard")]
		public void Search_by_LegalIn(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.LegalIn);

			foreach (var card in cards)
				Assert.That(card.IsLegalIn(expected));
		}

		[TestCase(@"Power:\*", "*")]
		public void Search_by_Power_star(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
				Assert.That(card.Power, Is.EqualTo(expected).IgnoreCase);
		}

		[TestCase(@"Power:*\*", "*")]
		public void Search_by_Power_star_suffix(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
				Assert.That(card.Power, Does.EndWith(expected).IgnoreCase);

			Assert.That(cards.Select(_ => _.Power).Distinct(), Has.Some.Not.StartsWith(expected));
		}

		[TestCase(@"Power:\*?", "*", 2)]
		public void Search_by_Power_star_prefix(string queryStr, string expectedPrefix, int expectedLength)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
			{
				Assert.That(card.Power, Does.StartWith(expectedPrefix).IgnoreCase);
				Assert.That(card.Power, Has.Length.EqualTo(expectedLength));
			}
		}

		[TestCase(@"Power:[1 TO 2]", "1", "2", "11")]
		public void Search_by_Power_range(string queryStr, string min, string max, string expectedSome)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
				Assert.That(card.Power, Is.GreaterThanOrEqualTo(min).And.LessThanOrEqualTo(max));

			Assert.That(
				cards.Select(c => c.Power).Distinct(),
				Does.Contain(expectedSome));
		}

		[TestCase(@"Toughness:[1 TO 2]", "1", "2", "11")]
		public void Search_by_Toughness_range(string queryStr, string min, string max, string expectedSome)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
				Assert.That(card.Toughness, Is.GreaterThanOrEqualTo(min).And.LessThanOrEqualTo(max));

			Assert.That(
				cards.Select(c => c.Toughness).Distinct(),
				Does.Contain(expectedSome));
		}

		[TestCase(@"Loyalty:[3 TO 4]", "3", "4")]
		public void Search_by_Loyalty_range(string queryStr, string min, string max)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.LoyaltyNum);

			foreach (var card in cards)
				Assert.That(card.Loyalty, Is.GreaterThanOrEqualTo(min).And.LessThanOrEqualTo(max));
		}

		[TestCase(@"PowerNum:[1 TO 2]", 1f, 2f)]
		[TestCase(@"PowerNum:(1 OR 2)", 1f, 2f)]
		public void Search_by_PowerNum_range(string queryStr, float min, float max)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
				Assert.That(card.PowerNum, Is.GreaterThanOrEqualTo(min).And.LessThanOrEqualTo(max));
		}

		[TestCase(@"PowerNum:2.0", 2f)]
		public void Search_by_PowerNum(string queryStr, float expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
				Assert.That(card.PowerNum, Is.EqualTo(expected));
		}

		[TestCase(@"ToughnessNum:[1 TO 2]", 1f, 2f, false)]
		[TestCase(@"ToughnessNum:{1 TO 2}", 1f, 2f, true)]
		[TestCase(@"ToughnessNum:[1 TO ?]", 1f, null, false)]
		[TestCase(@"ToughnessNum:{1 TO ?}", 1f, null, true)]
		[TestCase(@"ToughnessNum:[1 TO *]", 1f, null, false)]
		[TestCase(@"ToughnessNum:{1 TO *}", 1f, null, true)]
		[TestCase(@"ToughnessNum:[? TO 2]", null, 2f, false)]
		[TestCase(@"ToughnessNum:{? TO 2}", null, 2f, true)]
		[TestCase(@"ToughnessNum:[* TO 2]", null, 2f, false)]
		[TestCase(@"ToughnessNum:{* TO 2}", null, 2f, true)]
		public void Search_by_ToughnessNum_range(string queryStr, float? min, float? max, bool open)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			foreach (var card in cards)
			{
				if (min.HasValue)
				{
					if (open)
						Assert.That(card.ToughnessNum, Is.GreaterThan(min.Value));
					else
						Assert.That(card.ToughnessNum, Is.GreaterThanOrEqualTo(min.Value));
				}
				else
				{
					Assert.That(cards.Select(c => c.ToughnessNum).Distinct(), Has.Some.LessThanOrEqualTo(0f));
				}

				if (max.HasValue)
				{
					if (open)
						Assert.That(card.ToughnessNum, Is.LessThan(max.Value));
					else
						Assert.That(card.ToughnessNum, Is.LessThanOrEqualTo(max.Value));
				}
				else
				{
					Assert.That(cards.Select(c => c.ToughnessNum).Distinct(), Has.Some.GreaterThanOrEqualTo(10f));
				}
			}
		}


		[TestCase(@"Cmc:5.0", 5f)]
		public void Search_by_Cmc(string queryStr, float expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Cmc + " " + c.ManaCost);

			foreach (var card in cards)
				Assert.That(card.Cmc, Is.EqualTo(expected));
		}

		[TestCase(@"GeneratedMana:\{E\}", "{E}")]
		[TestCase(@"GeneratedMana:(*E* AND *W*)", "{E}", "{W}")]
		public void Search_by_GeneratedMana(string queryStr, params string[] allExpected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.GeneratedMana);

			foreach (var card in cards)
				foreach (string mana in allExpected)
					Assert.That(card.GeneratedMana, Does.Contain(mana).IgnoreCase);
		}

		[TestCase(@"ManaCost:\{B\}", "{B}")]
		[TestCase(@"ManaCost:(*B* AND *W*)", "B", "W")]
		[TestCase(@"ManaCost:\{W\/P\}", "{W/P}")]
		public void Search_by_ManaCost(string queryStr, params string[] allExpected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.ManaCost);

			foreach (var card in cards)
				foreach (string mana in allExpected)
					Assert.That(card.ManaCost, Does.Contain(mana).IgnoreCase);
		}

		[TestCase(@"Rarity:mythic", "mythic rare")]
		public void Search_by_Rarity(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Rarity);

			foreach (var card in cards)
				Assert.That(card.Rarity, Is.EqualTo(expected).IgnoreCase);
		}

		[TestCase(@"ReleaseDate:1993-*", "1993-")]
		[TestCase(@"ReleaseDate:*-01-*", "-01-")] // January
		[TestCase(@"ReleaseDate:*-*-23", "-23")] // 23 of any month
		public void Search_by_ReleaseDate(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.ReleaseDate);

			foreach (var card in cards)
				Assert.That(card.ReleaseDate, Does.Contain(expected).IgnoreCase);
		}

		[TestCase(@"PricingHigh:[100 TO *]", 100f)]
		public void Search_by_PricingHigh(string queryStr, float min)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.PricingHigh);

			foreach (var card in cards)
				Assert.That(card.PricingHigh, Is.GreaterThanOrEqualTo(min));
		}

		[TestCase(@"PricingMid:[50 TO *]", 50f)]
		public void Search_by_PricingMid(string queryStr, float min)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.PricingMid);

			foreach (var card in cards)
				Assert.That(card.PricingMid, Is.GreaterThanOrEqualTo(min));
		}

		[TestCase(@"PricingLow:[10 TO *]", 10f)]
		public void Search_by_PricingLow(string queryStr, float min)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.PricingLow);

			foreach (var card in cards)
				Assert.That(card.PricingLow, Is.GreaterThanOrEqualTo(min));
		}

		[TestCase(@"angel")]
		[TestCase(@"demon")]
		public void Search_by_AnyField(string queryStr)
		{
			var cards = search(queryStr, c => $"{c.NameEn}: {c.TypeEn}{c.TextEn}", "en");

			Assert.That(cards.Select(c => c.NameEn).Where(_ => _ != null).Distinct(), Has.Some.Contain(queryStr).IgnoreCase);
			Assert.That(cards.Select(c => c.TextEn).Where(_ => _ != null).Distinct(), Has.Some.Contain(queryStr).IgnoreCase);
			Assert.That(cards.Select(c => c.FlavorEn).Where(_ => _ != null).Distinct(), Has.Some.Contain(queryStr).IgnoreCase);
			Assert.That(cards.Select(c => c.TypeEn).Where(_ => _ != null).Distinct(), Has.Some.Contain(queryStr).IgnoreCase);
		}

		[TestCase(@"Text:Демон", "Демон", "ru")]
		public void Search_by_Text(string queryStr, string value, string lang)
		{
			var cards = search(queryStr, c => $"{c.NameEn}: {c.Localization.GetAbility(lang)}", lang);
			foreach (var card in cards)
				Assert.That(card.GetText(lang), Does.Contain(value).IgnoreCase);
		}

		[TestCase(@"Flavor:Демон", "Демон", "ru")]
		public void Search_by_Flavor(string queryStr, string value, string lang)
		{
			var cards = search(queryStr, c => $"{c.NameEn}: {c.Localization.GetFlavor(lang)}", lang);

			foreach (var card in cards)
				Assert.That(card.GetFlavor(lang), Does.Contain(value).IgnoreCase);
		}

		[TestCase(@"Name:Демон", "Демон", "ru")]
		public void Search_by_Name(string queryStr, string value, string lang)
		{
			var cards = search(queryStr, c => c.Localization.GetName(lang), lang);

			foreach (var card in cards)
				Assert.That(card.GetName(lang), Does.Contain(value).IgnoreCase);
		}

		[TestCase(@"Type:Демон", "Демон", "ru")]
		public void Search_by_Type(string queryStr, string value, string lang)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Localization.GetType(lang), lang);

			foreach (var card in cards)
				Assert.That(card.GetType(lang), Does.Contain(value).IgnoreCase);
		}

		[TestCase(@"Color:white", "white")]
		[TestCase(@"Color:colorless", "colorless")]
		public void Search_by_Color(string queryStr, string expectedColor)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Color);

			foreach (var card in cards)
				Assert.That(card.Color, Does.Contain(expectedColor).IgnoreCase);
		}

		[TestCase(@"nameen:/[ab]ngel.+/", "angel")]
		public void Search_by_regex(string queryStr, string expected)
		{
			var cards = search(queryStr, c => c.NameEn);

			foreach (var card in cards)
				Assert.That(card.NameEn, Does.Contain(expected).IgnoreCase);
		}

		private IList<Card> search(string queryStr, Func<Card, string> getter, string language = null)
		{
			var sw = new Stopwatch();
			sw.Start();

			var cards = Searcher.SearchCards(queryStr, language, Repo).ToList();

			sw.Stop();
			_log.Debug($"Found {cards.Count} cards in {sw.ElapsedMilliseconds} ms");

			Assert.That(cards, Is.Not.Null);
			Assert.That(cards, Is.Not.Empty);

			foreach (var card in cards)
				_log.Debug(getter(card));

			return cards;
		}

		[TearDown]
		public new void Teardown()
		{
			LogManager.Flush();
		}

		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
	}
}