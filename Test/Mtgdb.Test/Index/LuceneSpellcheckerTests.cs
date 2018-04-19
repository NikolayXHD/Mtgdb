using System;
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
		[TestCase(EnRu, "■", "Artist:", "Types:")] // all fields
		[TestCase(EnRu, "PricingMid■:[", "PricingMid:")]
		[TestCase(EnRu, "PricingMid:■[", Float)]
		[TestCase(EnRu, "PricingMid:[■", Float)]
		[TestCase(EnRu, "PricingMid:[0 TO 1■] OR", Float)]
		[TestCase(EnRu, "PricingMid:[0 TO 1]■ OR", "Artist:", "Types:")] // all fields
		[TestCase(EnRu, "PricingMid:[0 TO 1] ■OR", "OR", "AND", "NOT")]
		[TestCase(EnRu, "PricingMid:[0 TO 1] O■R", "OR", "AND", "NOT")]
		[TestCase(EnRu, "PricingMid:[0 TO 1] OR■", "OR", "AND", "NOT")]
		// numeric
		// =======
		[TestCase(EnRu, "Cmc:■", "0", "0.5", "10")]
		[TestCase(EnRu, "Cmc:10■", "10", "1000000")]
		[TestCase(EnRu, "Cmc:z■")] // empty
		[TestCase(EnRu, "Hand:■", "-4", "3")]
		[TestCase(EnRu, "Hand:1■", "-1", "1")]
		[TestCase(EnRu, "Hand:z■")] // empty
		[TestCase(EnRu, "Life:■", "-8", "12")]
		[TestCase(EnRu, "Life:3■", "-3", "3", "30")]
		[TestCase(EnRu, "Life:z■")] // empty
		[TestCase(EnRu, "LoyaltyNum:■", "0", "7")]
		[TestCase(EnRu, "LoyaltyNum:2■", "2")]
		[TestCase(EnRu, "LoyaltyNum:z■")] // empty
		[TestCase(EnRu, "PowerNum:■", "-1", "0.5", "1.5", "2.5", "3.5", "9", "11")]
		[TestCase(EnRu, "PowerNum:9■", "9", "99")]
		[TestCase(EnRu, "PowerNum:z■")] // empty
		[TestCase(EnRu, "PricingHigh:■", "0.29", "0.35")]
		[TestCase(EnRu, "PricingHigh:1■", "0.51", "1")]
		[TestCase(EnRu, "PricingHigh:z■")] // empty
		[TestCase(EnRu, "PricingLow:■", "0.01", "0.15")]
		[TestCase(EnRu, "PricingLow:1■", "0.01", "0.1", "1")]
		[TestCase(EnRu, "PricingLow:z■")] // empty
		[TestCase(EnRu, "PricingMid:■", "0.14", "0.19")]
		[TestCase(EnRu, "PricingMid:2■", "0.2", "0.42")]
		[TestCase(EnRu, "PricingMid:z■")] // empty
		[TestCase(EnRu, "ToughnessNum:■", "-1", "0.5", "1.5", "2.5", "3.5", "9")]
		[TestCase(EnRu, "ToughnessNum:3■", "3", "3.5", "13")]
		[TestCase(EnRu, "ToughnessNum:z■")] // empty
		// legality
		// ========
		[TestCase(EnRu, "BannedIn:■", "commander", "vintage")]
		[TestCase(EnRu, "BannedIn:c■", "commander", "legacy", "urza block")]
		[TestCase(EnRu, "BannedIn:zzzz■")] // empty suggest
		[TestCase(EnRu, "LegalIn:■", "amonkhet block", "kaladesh block")]
		[TestCase(EnRu, "LegalIn:a■", "amonkhet block", "ixalan block")]
		[TestCase(EnRu, "LegalIn:r■", "ravnica block", "mirrodin block")]
		[TestCase(EnRu, "LegalIn:zzzz■")] // empty suggest
		[TestCase(EnRu, "RestrictedIn:■", "un-sets", "vintage")]
		[TestCase(EnRu, "RestrictedIn:u■", "un-sets")]
		[TestCase(EnRu, "RestrictedIn:v■", "vintage")]
		[TestCase(EnRu, "RestrictedIn:я■")] // empty suggest
		// limited values
		// ==============
		// artist
		[TestCase(EnRu, "Artist:■", "aaron boyd", "al davidson")]
		[TestCase(EnRu, "Artist:a■", "alan pollack")]
		[TestCase(EnRu, "Artist:b■", "bastien l. deharme", "bob petillo")]
		[TestCase(EnRu, "Artist:wood■", "ash wood", "sam wood", "todd lockwood")]
		[TestCase(EnRu, "Artist:я■")] // empty suggest
		// color
		[TestCase(EnRu, "Color:■", "black", "blue", "colorless", "green", "red", "white")]
		[TestCase(EnRu, "Color:b■", "black", "blue")]
		[TestCase(EnRu, "Color:я■")] // empty suggest
		// generated mana
		[TestCase(EnRu, "GeneratedMana:■", "{any}", "{b}", "{c}", "{e}", "{g}", "{r}", "{s}", "{u}", "{w}")]
		[TestCase(EnRu, "GeneratedMana:\\{■", "{any}", "{b}", "{c}", "{e}", "{g}", "{r}", "{s}", "{u}", "{w}")]
		[TestCase(EnRu, "GeneratedMana:a■", "{any}")]
		[TestCase(EnRu, "GeneratedMana:w\\}■", "{w}")]
		[TestCase(EnRu, "GeneratedMana:я■")] // empty suggest
		// keywords
		[TestCase(EnRu, "Keywords:■", "absorb", "awaken")]
		[TestCase(EnRu, "Keywords:a■", "absorb", "awaken")]
		[TestCase(EnRu, "Keywords:ack■", "buyback", "flashback", "attack each turn")]
		[TestCase(EnRu, "Keywords:я■")] // empty suggest
		// layout
		[TestCase(EnRu, "Layout:■", "aftermath", "double-faced", "flip", "leveler", "meld", "normal", "phenomenon", "plane", "scheme", "split", "token", "vanguard")]
		[TestCase(EnRu, "Layout:v■", "vanguard", "leveler")]
		[TestCase(EnRu, "Layout:z■")] // empty suggest
		// mana cost
		[TestCase(EnRu, "ManaCost:■", "{0}", "{1}", "{10}", "{1000000}", "{11}", "{16}", "{2/b}")]
		[TestCase(EnRu, "ManaCost:0■", "{0}", "{10}", "{1000000}")]
		[TestCase(EnRu, "ManaCost:0■", "{0}", "{10}", "{1000000}")]
		[TestCase(EnRu, "ManaCost:9■", "{9}")]
		[TestCase(EnRu, "ManaCost: \\/■", "{2/b}", "{2/g}", "{2/r}", "{2/u}", "{2/w}", "{b/g}", "{g/p}", "{r/g}", "{u/b}", "{w/b}", "{w/p}")]
		[TestCase(EnRu, "ManaCost:я■")] // empty suggest
		// power
		[TestCase(EnRu, "Power:■", "*", "*²", "+0", "+1", "0", "½", "1", "1½")]
		[TestCase(EnRu, "Power:\\*■", "*", "1+*", "*²", "2+*")]
		[TestCase(EnRu, "Power:1■", "1", "1+*", "10", "1½", "11", "15", "+1", "-1")]
		[TestCase(EnRu, "Power:9■", "9", "99")]
		[TestCase(EnRu, "Power:я■")]
		// rarity
		[TestCase(EnRu, "Rarity:■", "basic land", "common", "mythic rare", "rare", "special", "uncommon")]
		[TestCase(EnRu, "Rarity:u■", "uncommon")]
		[TestCase(EnRu, "Rarity:я■")]
		// release date
		[TestCase(EnRu, "ReleaseDate:■", "1993-08-05", "1994-08-08", "1996-06-10")]
		[TestCase(EnRu, "ReleaseDate:6■", "2003-05-26", "2016-08-26")]
		[TestCase(EnRu, "ReleaseDate: \\-02\\-27■", "2015-02-27")]
		[TestCase(EnRu, "ReleaseDate:я■")]
		// set code
		[TestCase(EnRu, "SetCode:■", "10e", "5ed", "9ed", "aer")]
		[TestCase(EnRu, "SetCode:1■", "10e", "cm1", "cp1", "e01", "m11", "v11", "c13")]
		[TestCase(EnRu, "SetCode:gw■", "ogw")]
		[TestCase(EnRu, "SetCode:z■", "bfz", "zen")]
		[TestCase(EnRu, "SetCode:я■")]
		// set name
		[TestCase(EnRu, "SetName:■", "15th anniversary", "aether revolt", "amonkhet")]
		[TestCase(EnRu, "SetName:a■", "aether revolt", "alara reborn", "alliances", "amonkhet")]
		[TestCase(EnRu, "SetName:drit■", "eldritch moon")]
		[TestCase(EnRu, "SetName:я■")]
		// supertypes
		[TestCase(EnRu, "Supertypes:■", "basic", "legendary", "ongoing", "snow", "world")]
		[TestCase(EnRu, "Supertypes:w■", "snow", "world")]
		[TestCase(EnRu, "Supertypes:я■")]
		//types
		[TestCase(EnRu,
			"Types:■",
			"artifact",
			"conspiracy",
			"creature",
			"eaturecray",
			"enchantment",
			"ever",
			"host",
			"instant",
			"land",
			"phenomenon",
			"plane",
			"planeswalker",
			"scariest",
			"scheme",
			"see",
			"sorcery",
			"tribal",
			"vanguard")]
		[TestCase(EnRu, "Types:a■", "artifact", "conspiracy", "vanguard")]
		[TestCase(EnRu, "Types:aa■", "artifact")]
		[TestCase(EnRu, "Types:aa■", "artifact")]
		[TestCase(EnRu, "Types:я■")]
		// toughness
		[TestCase(EnRu, "Toughness:■", "*", "*²", "+0", "+1", "0", "-0", "½", "1", "1½")]
		[TestCase(EnRu, "Toughness:1■", "1", "1+*", "10", "1½", "11", "13", "+1", "-1")]
		[TestCase(EnRu, "Toughness:9■", "9", "99")]
		[TestCase(EnRu, "Toughness:я■")]
		// non-limited-value fields
		// ========================
		// flavor
		[TestCase(EnRuEn, "FlavorEn:■", "000", "070")]
		[TestCase(EnRuEn, "FlavorEn:a■", "aaaaaaiiii", "abandon")]
		[TestCase(EnRuEn, "FlavorEn:disp■", "dispel")]
		[TestCase(EnRuEn, "FlavorEn:thal■", "thalakos")]
		[TestCase(EnRuEn, "FlavorEn:n■", "naktamun", "nagging")]
		[TestCase(EnRuEn, "FlavorEn:я■")]
		[TestCase(Ru, "Flavor:■", "1", "101", "106")]
		[TestCase(Ru, "Flavor:а■", "абзан", "абордаж", "аборигены")]
		[TestCase(Ru, "Flavor:б■", "б", "баан", "бабочку")]
		[TestCase(Ru, "Flavor:ям■", "ямах", "бурями")]
		[TestCase(Ru, "Flavor:яя■", "вдохновляя")]
		[TestCase(Ru, "Flavor:х■", "хаазды")]
		[TestCase(Ru, "Flavor:ххх■")]
		// name
		[TestCase(EnRuEn, "NameEn:■", "1996 world champion", "abandon hope")]
		[TestCase(EnRuEn, "NameEn:a■", "abzan ascendancy")]
		[TestCase(EnRuEn, "NameEn:d■", "damping field", "dack fayden")]
		[TestCase(EnRuEn, "NameEn:disp■", "dispatch", "dispel", "disperse", "displace")]
		[TestCase(EnRuEn, "NameEn:evinrral■", "nevinyrral's disk")]
		[TestCase(EnRuEn, "NameEn:я■")]
		[TestCase(Ru, "Name:■", "Аббат Крепости Керал")]
		[TestCase(Ru, "Name:а■", "Аббат Крепости Керал", "Абзанская проводница")]
		[TestCase(Ru, "Name:с■", "с неба", "саблегривая мастикора")]
		[TestCase(Ru, "Name:сме■", "смельчак с двойным клинком", "смена направления")]
		[TestCase(Ru, "Name:щ■", "щедрость луксы")]
		[TestCase(Ru, "Name:щщщ■")]
		// like
		[TestCase(EnRu, "Like:■", "1996 world champion", "abandon hope")]
		[TestCase(EnRu, "Like:a■", "abzan ascendancy")]
		[TestCase(EnRu, "Like:d■", "damping field", "dack fayden")]
		[TestCase(EnRu, "Like:disp■", "dispatch", "dispel", "disperse", "displace")]
		[TestCase(EnRu, "Like:evinrral■", "nevinyrral's disk")]
		// text
		[TestCase(EnRuEn, "TextEn:a■", "a", "aaah", "abandon")]
		[TestCase(EnRuEn, "TextEn:b■", "baboons", "back")]
		[TestCase(EnRuEn, "TextEn:dis■", "disappear", "disc")]
		[TestCase(EnRuEn, "TextEn:disp■", "dispatch", "dispeller", "dispute")]
		[TestCase(EnRuEn, "TextEn:я■")]
		[TestCase(Ru, "Text:а■", "а", "аббат", "авацина")]
		[TestCase(Ru, "Text:двиг■", "надвигающегося")]
		[TestCase(Ru, "Text:пол■", "поле", "полет")]
		[TestCase(Ru, "Text:ъъъ■")]
		// type
		[TestCase(EnRuEn, "TypeEn:a■", "advisor", "aetherborn", "ajani")]
		[TestCase(EnRuEn, "TypeEn:b■", "baddest", "badger")]
		[TestCase(EnRuEn, "TypeEn:dis■", "dinosaur", "advisor")]
		[TestCase(EnRuEn, "TypeEn:z■", "zendikar", "zombie")]
		[TestCase(EnRuEn, "TypeEn:za■", "lizard", "urza")]
		[TestCase(EnRuEn, "TypeEn:я■")]
		[TestCase(Ru, "Type:■", "аватар", "аджани")]
		[TestCase(Ru, "Type:а■", "аватар", "аджани", "архонт")]
		[TestCase(Ru, "Type:кры■", "крыса")]
		[TestCase(Ru, "Type:ъ■")]
		// original type
		[TestCase(EnRu, "OriginalType:a■", "abomination", "ajani", "ali")]
		[TestCase(EnRu, "OriginalType:b■", "baba", "baddest", "bears")]
		[TestCase(EnRu, "OriginalType:dis■", "dinosaur", "advisor", "advisors")]
		[TestCase(EnRu, "OriginalType:z■", "zombie", "eldrazi")]
		[TestCase(EnRu, "OriginalType:za■", "lizard", "wizards")]
		[TestCase(EnRu, "OriginalType:я■")]
		// subtypes
		[TestCase(EnRu, "Subtypes:a■", "advisor", "aetherborn", "ajani")]
		[TestCase(EnRu, "Subtypes:b■", "baddest", "badger")]
		[TestCase(EnRu, "Subtypes:dis■", "dinosaur", "advisor")]
		[TestCase(EnRu, "Subtypes:z■", "zendikar", "zombie")]
		[TestCase(EnRu, "Subtypes:za■", "lizard", "urza")]
		[TestCase(EnRu, "Subtypes:я■")]
		public void Suggest_output_contains_expected_values(string langs, string originalQuery, params string[] expectedSuggests)
		{
			string[] languages;
			string[] queryLanguageVariants;

			switch (langs)
			{
				case EnRu:
					languages = Array.From("en", "ru");
					queryLanguageVariants = Array.From(originalQuery, originalQuery);
					break;

				case EnRuEn:
					languages = Array.From("en", "ru", "en");

					string queryWithLocalizedField = _enSuffixPattern.Replace(originalQuery, string.Empty);
					Assert.That(queryWithLocalizedField, Is.Not.EqualTo(originalQuery));

					queryLanguageVariants = Array.From(originalQuery, originalQuery, queryWithLocalizedField);
					break;

				case Ru:
					languages = Array.From("ru");
					queryLanguageVariants = Array.From(originalQuery);
					break;

				default:
					throw new ArgumentException();
			}

			for (int i = 0; i < queryLanguageVariants.Length; i++)
			{
				string queryLanguageVariant = queryLanguageVariants[i];
				
				var queryCasingVariants = Array.From(
					_boolOperatorPattern.Replace(queryLanguageVariant.ToLower(Str.Culture), match => match.Value.ToUpper(Str.Culture)),
					queryLanguageVariant.ToUpper(Str.Culture));

				foreach (string queryCasingVariant in queryCasingVariants)
				{
					int caret = queryCasingVariant.IndexOf(CaretIndicator);
					string query = queryCasingVariant.Substring(0, caret) + queryCasingVariant.Substring(caret + 1);

					var list = suggestByInput(query, caret, languages[i]);

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
		private const string EnRu = "en|ru";
		private const string EnRuEn = "en|ru|en";
		private const string Ru = "ru";

		private static readonly Regex _enSuffixPattern = new Regex("(?<=\\bname|text|type|flavor)En(?=:)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private static readonly Regex _boolOperatorPattern = new Regex(@"\b(a■?n■?d|o■?r|n■?o■?t)\b",
			RegexOptions.Compiled);
	}
}