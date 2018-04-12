using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class LuceneSpellcheckerTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadIndexes();
			Spellchecker.MaxCount = 50;
		}

		[TestCase("*", "d", "en", null)]
		[TestCase("NameEn", "neveinral", null, "nevinyrral")]
		[TestCase("NameEn", "viniral", null, "nevinyrral")]
		[TestCase("TextEn", "disk", null, "disk")]
		[TestCase("Name", "гел", "ru", "ангел")]
		[TestCase("*", "арха", "ru", "архангел")]
		[TestCase("*", "ange", "en", "angel")]
		[TestCase("layout", "aft", null, "aftermath")]
		[TestCase("generatedmana", "w", null, "{w}")]
		public void Suggest_text_values(string field, string value, string language, string expectedSuggest)
		{
			var list = suggest(field, value, language);

			if (expectedSuggest != null)
				Assert.That(list, Has.Some.Contain(expectedSuggest).IgnoreCase);
		}

		// caret positions
		// ===============
		[TestCase("■", "Artist:", "Types:")] // all fields
		[TestCase("PricingMid■:[", "PricingMid:")]
		[TestCase("PricingMid:■[", Float)]
		[TestCase("PricingMid:[■", Float)]
		[TestCase("PricingMid:[0 TO 1■] OR", Float)]
		[TestCase("PricingMid:[0 TO 1]■ OR", "Artist:", "Types:")] // all fields
		[TestCase("PricingMid:[0 TO 1] ■OR", "OR", "AND", "NOT")]
		[TestCase("PricingMid:[0 TO 1] O■R", "OR", "AND", "NOT")]
		[TestCase("PricingMid:[0 TO 1] OR■", "OR", "AND", "NOT")]
		// numeric
		// =======
		[TestCase("Cmc:■", "0", "0.5", "10")]
		[TestCase("Cmc:10■", "10", "1000000")]
		[TestCase("Cmc:z■")] // empty
		[TestCase("Hand:■", "-4", "3")]
		[TestCase("Hand:1■", "-1", "1")]
		[TestCase("Hand:z■")] // empty
		[TestCase("Life:■", "-8", "12")]
		[TestCase("Life:3■", "-3", "3", "30")]
		[TestCase("Life:z■")] // empty
		[TestCase("LoyaltyNum:■", "0", "7")]
		[TestCase("LoyaltyNum:2■", "2")]
		[TestCase("LoyaltyNum:z■")] // empty
		[TestCase("PowerNum:■", "-1", "0.5", "1.5", "2.5", "3.5", "9", "11")]
		[TestCase("PowerNum:9■", "9", "99")]
		[TestCase("PowerNum:z■")] // empty
		[TestCase("PricingHigh:■", "0.29", "0.35")]
		[TestCase("PricingHigh:1■", "0.51", "1")]
		[TestCase("PricingHigh:z■")] // empty
		[TestCase("PricingLow:■", "0.01", "0.15")]
		[TestCase("PricingLow:1■", "0.01", "0.1", "1")]
		[TestCase("PricingLow:z■")] // empty
		[TestCase("PricingMid:■", "0.14", "0.19")]
		[TestCase("PricingMid:2■", "0.2", "0.42")]
		[TestCase("PricingMid:z■")] // empty
		[TestCase("ToughnessNum:■", "-1", "0.5", "1.5", "2.5", "3.5", "9")]
		[TestCase("ToughnessNum:3■", "3", "3.5", "13")]
		[TestCase("ToughnessNum:z■")] // empty
		// legality
		// ========
		[TestCase("BannedIn:■", "commander", "vintage")]
		[TestCase("BannedIn:c■", "commander", "legacy", "urza block")]
		[TestCase("BannedIn:zzzz■")] // empty suggest
		[TestCase("LegalIn:■", "amonkhet block", "kaladesh block")]
		[TestCase("LegalIn:a■", "amonkhet block", "ixalan block")]
		[TestCase("LegalIn:r■", "ravnica block", "mirrodin block")]
		[TestCase("LegalIn:zzzz■")] // empty suggest
		[TestCase("RestrictedIn:■", "un-sets", "vintage")]
		[TestCase("RestrictedIn:u■", "un-sets")]
		[TestCase("RestrictedIn:v■", "vintage")]
		[TestCase("RestrictedIn:zzzz■")] // empty suggest
		// limited values
		// ==============
		// artist
		[TestCase("Artist:■", "aaron boyd", "al davidson")]
		[TestCase("Artist:a■", "alan pollack")]
		[TestCase("Artist:b■", "bastien l. deharme", "bob petillo")]
		[TestCase("Artist:wood■", "ash wood", "sam wood", "todd lockwood")]
		[TestCase("Artist:zzzz■")] // empty suggest
		// color
		[TestCase("Color:■", "black", "blue", "colorless", "green", "red", "white")]
		[TestCase("Color:b■", "black", "blue")]
		[TestCase("Color:z■")] // empty suggest
		// generated mana
		[TestCase("GeneratedMana:■", "{any}", "{b}", "{c}", "{e}", "{g}", "{r}", "{s}", "{u}", "{w}")]
		[TestCase("GeneratedMana:\\{■", "{any}", "{b}", "{c}", "{e}", "{g}", "{r}", "{s}", "{u}", "{w}")]
		[TestCase("GeneratedMana:a■", "{any}")]
		[TestCase("GeneratedMana:w\\}■", "{w}")]
		[TestCase("GeneratedMana:z■")] // empty suggest
		// keywords
		[TestCase("Keywords:■", "absorb", "awaken")]
		[TestCase("Keywords:a■", "absorb", "awaken")]
		[TestCase("Keywords:ack■", "buyback", "flashback", "attack each turn")]
		[TestCase("Keywords:zzz■")] // empty suggest
		// layout
		[TestCase("Layout:■", "aftermath", "double-faced", "flip", "leveler", "meld", "normal", "phenomenon", "plane", "scheme", "split", "token", "vanguard")]
		[TestCase("Layout:v■", "vanguard", "leveler")]
		[TestCase("Layout:z■")] // empty suggest
		// mana cost
		[TestCase("ManaCost:■", "{0}", "{1}", "{10}", "{1000000}", "{11}", "{16}", "{2/b}")]
		[TestCase("ManaCost:0■", "{0}", "{10}", "{1000000}")]
		[TestCase("ManaCost:0■", "{0}", "{10}", "{1000000}")]
		[TestCase("ManaCost:9■", "{9}")]
		[TestCase("ManaCost: \\/■", "{2/b}", "{2/g}", "{2/r}", "{2/u}", "{2/w}", "{b/g}", "{g/p}", "{r/g}", "{u/b}", "{w/b}", "{w/p}")]
		[TestCase("ManaCost:z■")] // empty suggest
		// power
		[TestCase("Power:■", "*", "*²", "+0", "+1", "0", "½", "1", "1½")]
		[TestCase("Power:1■", "1", "1+*", "10", "1½", "11", "15", "+1", "-1")]
		[TestCase("Power:9■", "9", "99")]
		[TestCase("Power:z■")]
		// rarity
		[TestCase("Rarity:■", "basic land", "common", "mythic rare", "rare", "special", "uncommon")]
		[TestCase("Rarity:u■", "uncommon")]
		[TestCase("Rarity:z■")]
		// release date
		[TestCase("ReleaseDate:■", "1993-08-05", "1994-08-08", "1996-06-10")]
		[TestCase("ReleaseDate:6■", "2003-05-26", "2016-08-26")]
		[TestCase("ReleaseDate:z■")]
		// set code
		[TestCase("SetCode:■", "10e", "5ed", "9ed", "aer")]
		[TestCase("SetCode:1■", "10e", "cm1", "cp1", "e01", "m11", "v11", "c13")]
		[TestCase("SetCode:gw■", "ogw")]
		[TestCase("SetCode:z■", "bfz", "zen")]
		[TestCase("SetCode:zzz■")]
		// set name
		[TestCase("SetName:■", "15th anniversary", "aether revolt", "amonkhet")]
		[TestCase("SetName:a■", "aether revolt", "alara reborn", "alliances", "amonkhet")]
		[TestCase("SetName:drit■", "eldritch moon")]
		[TestCase("SetName:222■")]
		// supertypes
		[TestCase("Supertypes:■", "basic", "legendary", "ongoing", "snow", "world")]
		[TestCase("Supertypes:w■", "snow", "world")]
		[TestCase("Supertypes:z■")]
		// toughness
		[TestCase("Toughness:■", "*", "*²", "+0", "+1", "0", "-0", "½", "1", "1½")]
		[TestCase("Toughness:1■", "1", "1+*", "10", "1½", "11", "13", "+1", "-1")]
		[TestCase("Toughness:9■", "9", "99")]
		[TestCase("Toughness:z■")]
		// non-limited-value fields
		// ========================
		// flavor
		[TestCase("FlavorEn:a■", "aaaaaaiiii", "al-abara")]
		[TestCase("FlavorEn:n■", "naktamun", "nagging")]
		[TestCase("FlavorEn:xxx■")]
		[TestCase("NameEn:■", "\"ach! hans, run!\"", "1996 world champion", "abandon hope")]
		[TestCase("NameEn:a■", "abzan ascendancy")]
		[TestCase("NameEn:d■", "damping field", "dauthi warlord")]
		[TestCase("NameEn:disp■", "dispatch", "dispel", "disperse", "displace")]
		[TestCase("NameEn:evinrral■", "nevinyrral's disk")]

		//[TestCase("Like:■")]
		//[TestCase("TextEn:■")]
		//[TestCase("OriginalText:■")]
		//[TestCase("OriginalType:■")]
		//[TestCase("Subtypes:■")]
		//[TestCase("TypeEn:■")]
		//[TestCase("Types:■")]
		public void Suggest_output_contains_expected_values(string queryWithCaret, params string[] expectedSuggests)
		{
			var languages = new[] { "en", "ru" };

			int caret = queryWithCaret.IndexOf(CaretIndicator);
			string query = queryWithCaret.Substring(0, caret) + queryWithCaret.Substring(caret + 1);

			var boolPattern = new Regex(@"\b(and|or|not)\b");

			var queryVariants = new[]
			{
				boolPattern.Replace(query.ToLower(Str.Culture), match => match.Value.ToUpper(Str.Culture)),
				query.ToUpper(Str.Culture)
			};

			foreach (string language in languages)
			{
				foreach (string queryVariant in queryVariants)
				{
					var list = suggestByInput(queryVariant, caret, language);

					foreach (string expectedSuggest in expectedSuggests)
					{
						if (expectedSuggest == Float)
							Assert.That(list.All(v => float.TryParse(v, out _)));
						else if (!string.IsNullOrEmpty(expectedSuggest))
							Assert.That(list, Has.Some.EqualTo(expectedSuggest).IgnoreCase);
						else
							Assert.That(list, Is.Empty);
					}
				}
			}
		}

		private IReadOnlyList<string> suggest(string field, string value, string language)
		{
			var sw = new Stopwatch();
			sw.Start();

			string query = $"{field}:{value}";

			var list = Spellchecker.Suggest(language, query, caret: query.Length).Values;

			sw.Stop();
			Log.Debug($"Suggest retrieved in {sw.ElapsedMilliseconds} ms");

			Assert.That(list, Is.Not.Null);
			Assert.That(list, Is.Not.Empty);

			foreach (string variant in list)
				Log.Debug(variant);

			return list;
		}

		private IReadOnlyList<string> suggestByInput(string query, int caret, string language)
		{
			var sw = new Stopwatch();
			sw.Start();

			var suggest = Spellchecker.Suggest(language, query, caret);

			sw.Stop();
			Log.Debug($"Suggest retrieved in {sw.ElapsedMilliseconds} ms");

			Assert.That(suggest, Is.Not.Null);

			var list = suggest.Values;

			Assert.That(list, Is.Not.Null);

			Log.Debug("Token: " + suggest.Token);
			Log.Debug("Suggest:");

			foreach (string variant in list)
				Log.Debug(variant);

			return list;
		}

		public const string Float = "{float}";
		public const char CaretIndicator = '■';
	}
}