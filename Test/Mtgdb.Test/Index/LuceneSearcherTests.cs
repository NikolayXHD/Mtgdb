using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mtgdb.Dal;
using NLog;
using NUnit.Framework;
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Mtgdb.Test
{
	public class LuceneSearcherTests : IndexTestsBase
	{
		[TestCase(@"NameEn:Forest")]
		public void Search_by_NameEn(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn);
			Assert.That(cards.All(c => c.NameEn.IndexOf("Forest", Str.Comparison) >= 0));
		}

		[TestCase(@"TextEn:embalm")]
		public void Search_by_TextEn(string queryStr)
		{
			var cards = search(queryStr, c => c.TextEn + Environment.NewLine);
			Assert.That(cards.All(c => c.TextEn.IndexOf("embalm", Str.Comparison) >= 0));
		}

		[TestCase(@"FlavorEn:angel")]
		public void Search_by_FlavorEn(string queryStr)
		{
			var cards = search(queryStr, c => c.FlavorEn + Environment.NewLine);
			Assert.That(cards.All(c => c.FlavorEn.IndexOf("angel", Str.Comparison) >= 0));
		}

		[TestCase(@"SetName:""Battle for""")]
		[TestCase(@"SetName:""Battle zendikar""~1")]
		public void Search_by_SetName(string queryStr)
		{
			search(queryStr, c => c.SetName);
		}

		[Test]
		public void Search_by_SetCode()
		{
			const string queryStr = "SetCode:LEA";
			var cards = search(queryStr, c => c.SetCode);

			Assert.That(cards.All(c => c.SetCode.Equals("LEA", Str.Comparison)));
		}

		[Test]
		public void Search_by_SetCode_wildcard()
		{
			const string queryStr = "SetCode:LE?";
			var cards = search(queryStr, c => c.SetCode);

			Assert.That(cards.All(c => c.SetCode.StartsWith("LE", Str.Comparison)));
			Assert.That(cards.All(c => c.SetCode.Length == 3));
		}

		[TestCase(@"Artist:James")]
		public void Search_by_Artist(string queryStr)
		{
			var cards = search(queryStr, c => c.Artist);
			Assert.That(cards.All(c => c.Artist.IndexOf("James", Str.Comparison) >= 0));
		}

		[TestCase(@"TypeEn:Human")]
		[TestCase(@"TypeEn:""Legendary Human""~2")]
		public void Search_by_TypeEn(string queryStr)
		{
			search(queryStr, c => c.TypeEn);
		}

		[TestCase(@"Types:""Artifact Creature""")]
		[TestCase(@"Types:Creature AND Types:Artifact")]
		[TestCase(@"Types:*fact")]
		public void Search_by_Types(string queryStr)
		{
			search(queryStr, c => string.Join(" ", c.TypesArr ?? Enumerable.Empty<string>()));
		}

		[TestCase(@"Supertypes:Legendary")]
		public void Search_by_Supertypes(string queryStr)
		{
			var cards = search(queryStr, c => string.Join(" ", c.SupertypesArr ?? Enumerable.Empty<string>()));
			Assert.That(cards.All(c => c.SupertypesArr.Contains("Legendary", Str.Comparer)));
		}

		[TestCase(@"Subtypes:Human AND Subtypes: Rogue")]
		[TestCase(@"Subtypes:""Human Rogue""")]
		public void Search_by_Subtypes(string queryStr)
		{
			search(queryStr, c => string.Join(" ", c.SubtypesArr ?? Enumerable.Empty<string>()));
		}

		[TestCase(@"RestrictedIn:Vintage")]
		public void Search_by_RestrictedIn(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.RestrictedIn);

			Assert.That(cards.All(c => c.IsRestrictedIn("Vintage")));
		}

		[TestCase(@"BannedIn:Modern")]
		public void Search_by_BannedIn(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.BannedIn);

			Assert.That(cards.All(c => c.IsBannedIn("Modern")));
		}

		[TestCase(@"LegalIn:Standard")]
		public void Search_by_LegalIn(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.LegalIn);

			Assert.That(cards.All(c => c.IsLegalIn("Standard")));
		}

		[TestCase(@"Power:[1 TO 2]")]
		public void Search_by_Power_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			var mismatchingCard = cards.FirstOrDefault(_ => !
			(Str.Compare(_.Power, "1") >= 0 &&
			Str.Compare(_.Power, "2") <= 0));

			Assert.That(mismatchingCard, Is.Null);

			Assert.That(cards.Any(_ => _.Power == "11"));
		}

		[TestCase(@"Power:\*")]
		public void Search_by_Power_wildcard(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);
			Assert.That(cards.All(c=>c.Power == "*"));
		}

		[TestCase(@"Toughness:[1 TO 2]")]
		public void Search_by_Toughness_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			var mismatchingCard = cards.FirstOrDefault(_ => !
					(Str.Compare(_.Toughness, "1") >= 0 &&
					Str.Compare(_.Toughness, "2") <= 0));

			Assert.That(mismatchingCard, Is.Null);

			Assert.That(cards.Any(_ => _.Toughness == "11"));
		}

		[TestCase(@"Loyalty:[3 TO 4]")]
		public void Search_by_Loyalty_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.LoyaltyNum);

			Assert.That(cards.All(c =>
				c.LoyaltyNum.HasValue &&
				c.LoyaltyNum.Value >= 3 &&
				c.LoyaltyNum.Value <= 4));
		}

		[TestCase(@"Loyalty:3")]
		public void Search_by_Loyalty(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.LoyaltyNum);
			
			Assert.That(cards.All(c => c.LoyaltyNum == 3));
		}

		[TestCase(@"PowerNum:[1 TO 2]")]
		[TestCase(@"PowerNum:(1 OR 2)")]
		public void Search_by_PowerNum_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			Assert.That(cards.All(_ =>
				_.PowerNum.HasValue &&
				_.PowerNum.Value >= 1 &&
				_.PowerNum.Value <= 2));
		}

		[TestCase(@"PowerNum:2.0")]
		public void Search_by_PowerNum(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			Assert.That(cards.All(_ =>
				_.PowerNum.HasValue &&
				_.PowerNum.Value == 2f));
		}

		[TestCase(@"ToughnessNum:[1 TO 2]")]
		[TestCase(@"ToughnessNum:{1 TO 2}")]
		[TestCase(@"ToughnessNum:[1 TO ?]")]
		[TestCase(@"ToughnessNum:{1 TO ?}")]
		[TestCase(@"ToughnessNum:[1 TO *]")]
		[TestCase(@"ToughnessNum:{1 TO *}")]
		[TestCase(@"ToughnessNum:[? TO 2]")]
		[TestCase(@"ToughnessNum:{? TO 2}")]
		[TestCase(@"ToughnessNum:[* TO 2]")]
		[TestCase(@"ToughnessNum:{* TO 2}")]
		public void Search_by_ToughnessNum_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			if (queryStr.Contains("[1"))
				Assert.That(cards.All(c => c.ToughnessNum >= 1));
			else if (queryStr.Contains("{1"))
				Assert.That(cards.All(c => c.ToughnessNum > 1));
			else if (queryStr.Contains("[*") || queryStr.Contains("[?") || queryStr.Contains("{*") || queryStr.Contains("{?"))
				Assert.That(cards.Any(c => c.ToughnessNum == 0.0f));

			if (queryStr.Contains("2]"))
				Assert.That(cards.All(c => c.ToughnessNum <= 2));
			else if (queryStr.Contains("2}"))
				Assert.That(cards.All(c => c.ToughnessNum < 2));
			else if (queryStr.Contains("*]") || queryStr.Contains("?]") || queryStr.Contains("*}") || queryStr.Contains("?}"))
				Assert.That(cards.Any(c => c.ToughnessNum == 11.0f));
		}


		[TestCase(@"Cmc:5.0")]
		public void Search_by_Cmc(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Cmc + " " + c.ManaCost);
			Assert.That(cards.All(_ => _.Cmc == 5f));
		}

		[TestCase(@"GeneratedMana:\{E\}")]
		[TestCase(@"GeneratedMana:(*E* AND *W*)")]
		[TestCase(@"GeneratedMana:(\{E\} OR \{W\})")]
		public void Search_by_GeneratedMana(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.GeneratedMana);
		}

		[TestCase(@"ManaCost:\{B\}")]
		[TestCase(@"ManaCost:(*B* AND *W*)")]
		[TestCase(@"ManaCost:(*B* OR *W*)")]
		[TestCase(@"ManaCost:\{W\/P\}")]
		public void Search_by_ManaCost(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.ManaCost);
		}

		[TestCase(@"Rarity:mythic")]
		public void Search_by_Rarity(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Rarity);
			Assert.That(cards.All(c => Str.Equals(c.Rarity, "mythic rare")));
		}

		[TestCase(@"ReleaseDate:1993-*")]
		[TestCase(@"ReleaseDate:*-01-*")] // January
		[TestCase(@"ReleaseDate:*-*-23")] // 23 of any month
		public void Search_by_ReleaseDate(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.ReleaseDate);
		}

		[TestCase(@"PricingHigh:[100 TO *]")]
		[TestCase(@"PricingMid:[50 TO *]")]
		[TestCase(@"PricingLow:[10 TO *]")]
		public void Search_by_Pricing(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.PricingLow + " - " + c.PricingMid + " - " + c.PricingHigh);
		}

		[TestCase(@"angel")]
		[TestCase(@"demon")]
		public void Search_by_AnyField(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.TypeEn + Environment.NewLine + c.TextEn + Environment.NewLine, "en");

			Assert.That(cards.Any(c => c.TextEn.IndexOf(queryStr, Str.Comparison) < 0));
			Assert.That(cards.Any(c => c.Name.IndexOf(queryStr, Str.Comparison) < 0));
			Assert.That(cards.Any(c => c.TypeEn.IndexOf(queryStr, Str.Comparison) < 0));
		}

		[TestCase(@"Text:Демон", "ru")]
		public void Search_by_Text(string queryStr, string lang)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + Environment.NewLine + c.Localization.GetAbility(lang) + Environment.NewLine, lang);
			Assert.That(cards.All(c => c.GetText("ru").IndexOf("Демон", Str.Comparison) >= 0));
		}

		[TestCase(@"Flavor:Демон", "ru")]
		public void Search_by_Flavor(string queryStr, string lang)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + Environment.NewLine + c.Localization.GetFlavor(lang) + Environment.NewLine, lang);
			Assert.That(cards.All(c => c.GetFlavor("ru").IndexOf("Демон", Str.Comparison) >= 0));
		}

		[TestCase(@"Name:Демон", "ru")]
		public void Search_by_Name(string queryStr, string lang)
		{
			var cards = search(queryStr, c => c.Localization.GetName(lang), lang);
			Assert.That(cards.All(c => c.GetName("ru").IndexOf("Демон", Str.Comparison) >= 0));
		}

		[TestCase(@"Type:Демон", "ru")]
		public void Search_by_Type(string queryStr, string lang)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Localization.GetType(lang), lang);
			Assert.That(cards.All(c => c.GetType("ru").IndexOf("Демон", Str.Comparison) >= 0));
		}

		[TestCase(@"Color:white", "white")]
		[TestCase(@"Color:colorless", "colorless")]
		public void Search_by_Color(string queryStr, string expectedColor)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Color);
			Assert.That(cards.All(c => c.Color.IndexOf(expectedColor, Str.Comparison) >= 0));
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