using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class LuceneSearcherTests
	{
		// ReSharper disable CompareOfFloatsByEqualityOperator
		private LuceneSearcher _searcher;
		private CardRepository _repository;

		[OneTimeSetUp]
		public void Setup()
		{
			TestLoader.LoadModules();
			TestLoader.LoadCardRepository();
			TestLoader.LoadLocalizations();
			TestLoader.LoadSearcher();

			_searcher = TestLoader.Searcher;
			_repository = TestLoader.CardRepository;
		}
		
		[OneTimeTearDown]
		public void Teardown()
		{
			_searcher.Dispose();
		}

		[TestCase(@"NameEn:Forest")]
		public void Search_by_NameEn(string queryStr)
		{
			search(queryStr, c => c.NameEn);
		}

		[TestCase(@"TextEn:embalm")]
		public void Search_by_TextEn(string queryStr)
		{
			search(queryStr, c => c.TextEn + Environment.NewLine);
		}

		[TestCase(@"FlavorEn:angel")]
		public void Search_by_FlavorEn(string queryStr)
		{
			search(queryStr, c => c.FlavorEn + Environment.NewLine);
		}

		[TestCase(@"SetName:""Battle for""")]
		[TestCase(@"SetName:""Battle zendikar""~1")]
		public void Search_by_SetName(string queryStr)
		{
			search(queryStr, c => c.SetName);
		}

		[TestCase(@"SetCode:LEA")]
		[TestCase(@"SetCode:LE?")]
		public void Search_by_SetCode(string queryStr)
		{
			search(queryStr, c => c.SetCode);
		}

		[TestCase(@"Artist:James")]
		public void Search_by_Artist(string queryStr)
		{
			search(queryStr, c => c.Artist);
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
			search(queryStr, c => string.Join(" ", c.SupertypesArr ?? Enumerable.Empty<string>()));
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
			search(queryStr, c => c.NameEn + ": " + c.RestrictedIn);
		}

		[TestCase(@"BannedIn:Modern")]
		public void Search_by_BannedIn(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.BannedIn);
		}

		[TestCase(@"LegalIn:Standard")]
		public void Search_by_LegalIn(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.LegalIn);
		}

		[TestCase(@"Power:[1 TO 2]")]
		public void Search_by_Power_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			Assert.That(cards.Any(_ => _.Power == "11"));
		}

		[TestCase(@"Toughness:[1 TO 2]")]
		public void Search_by_Toughness_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);

			Assert.That(cards.Any(_ => _.Toughness == "11"));
		}

		[TestCase(@"Loyalty:[3 TO 4]")]
		public void Search_by_Loyalty_range(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.LoyaltyNum);

			Assert.That(cards.All(c => c.LoyaltyNum.HasValue && c.LoyaltyNum >= 3 && c.LoyaltyNum <= 4));
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

			Assert.That(cards.All(_ => _.Power != "11"));
		}

		[TestCase(@"PowerNum:2.0")]
		public void Search_by_PowerNum(string queryStr)
		{
			var cards = search(queryStr, c => c.NameEn + ": " + c.Power + "/" + c.Toughness);
			Assert.That(cards.All(_ => _.PowerNum == 2f));
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

		[TestCase(@"GeneratedMana:E")]
		[TestCase(@"GeneratedMana:(E AND W)")]
		[TestCase(@"GeneratedMana:(E OR W)")]
		public void Search_by_GeneratedMana(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.GeneratedMana);
		}

		[TestCase(@"ManaCost:B")]
		[TestCase(@"ManaCost:(B AND W)")]
		[TestCase(@"ManaCost:(B OR W)")]
		[TestCase(@"ManaCost:W/P")]
		public void Search_by_ManaCost(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.ManaCost);
		}

		[TestCase(@"Rarity:mythic")]
		public void Search_by_Rarity(string queryStr)
		{
			search(queryStr, c => c.NameEn + ": " + c.Rarity);
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
			var cards = search(queryStr, c => c.NameEn + ": " + c.TypeEn + Environment.NewLine + c.TextEn + Environment.NewLine);

			Assert.That(cards.Any(c => c.TextEn.IndexOf(queryStr, Str.Comparison) < 0));
			Assert.That(cards.Any(c => c.Name.IndexOf(queryStr, Str.Comparison) < 0));
			Assert.That(cards.Any(c => c.TypeEn.IndexOf(queryStr, Str.Comparison) < 0));
		}

		[TestCase(@"Text:Демон", "ru")]
		public void Search_by_Text(string queryStr, string lang)
		{
			search(queryStr, c => c.NameEn + ": " + Environment.NewLine + c.Localization.GetAbility(lang) + Environment.NewLine, lang);
		}

		[TestCase(@"Flavor:Демон", "ru")]
		public void Search_by_Flavor(string queryStr, string lang)
		{
			search(queryStr, c => c.NameEn + ": " + Environment.NewLine + c.Localization.GetFlavor(lang) + Environment.NewLine, lang);
		}

		[TestCase(@"Name:Демон", "ru")]
		public void Search_by_Name(string queryStr, string lang)
		{
			search(queryStr, c => c.Localization.GetName(lang), lang);
		}

		[TestCase(@"Type:Демон", "ru")]
		public void Search_by_Type(string queryStr, string lang)
		{
			search(queryStr, c => c.NameEn + ": " + c.Localization.GetType(lang), lang);
		}

		private IList<Card> search(string queryStr, Func<Card, string> getter, string language = null)
		{
			var sw = new Stopwatch();
			sw.Start();

			var cards = _searcher.SearchCards(queryStr, language, _repository).ToList();

			sw.Stop();
			Console.WriteLine($"Found {cards.Count} cards in {sw.ElapsedMilliseconds} ms");

			Assert.That(cards, Is.Not.Null);
			Assert.That(cards, Is.Not.Empty);

			foreach (var card in cards)
				Console.WriteLine(getter(card));

			return cards;
		}
	}
}