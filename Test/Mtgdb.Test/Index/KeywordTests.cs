using System.Linq;
using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class KeywordTests : TestsBase
	{
		[OneTimeSetUp]
		public static void Setup()
		{
			LoadTranslations();
			LoadIndexes();
		}

		[Test]
		public void Card_keywords_contain_expected_value(
			[Values("LEA")] string setcode,
			[Values("Badlands")] string name,
			[Values(nameof(KeywordDefinitions.ManaCost))]
			string field,
			[Values("")] string expectedValue)
		{
			var card = Repo.SetsByCode[setcode].CardsByName[name].First();
			var keywords = card.GetKeywords();

			if (string.IsNullOrEmpty(expectedValue))
			{
				Assert.That(keywords.KeywordsByProperty, Does.Not.ContainKey(field));
				return;
			}

			var values = keywords.KeywordsByProperty[field];

			Assert.That(values, Is.Not.Null);
			Assert.That(values, Does.Contain(expectedValue));
		}



		[TestCase(@"Text: *absorb* AND NOT Keywords: absorb")]
		[TestCase(@"Text: *activat* AND NOT Keywords: activate")]
		[TestCase(@"Text: *affinity* AND NOT Keywords: affinity")]
		[TestCase(@"Text: *afflict* AND NOT Keywords: afflict AND NOT Name: afflict*")]
		[TestCase(@"Text: *aftermath* AND NOT Keywords: aftermath")]
		[TestCase(@"Text: *amplify* AND NOT Keywords: amplify")]
		[TestCase(@"Text: *annihilator* AND NOT Keywords: annihilator")]
		[TestCase(@"Text: ante* AND NOT (Keywords: ante OR Name: (ante* OR ""master of the wild hunt avatar""))")]
		[TestCase(@"Text: *ascend* AND NOT Keywords: ascend AND NOT Name: (ascendant OR ascending OR ascendancy)")]
		[TestCase(@"Text: (*attach* AND NOT unattach*) AND NOT Keywords: attach")]
		[TestCase(@"Text: (*unattach*) AND NOT Keywords: unattach")]
		//
		[TestCase(@"Text: (""/attacks?/ if able""~10 AND NOT ""/attacks?/ /block(s|ed)?/ if able""~10) AND NOT Keywords: ""attack if able""")]
		[TestCase(@"Keywords: ""attack if able"" AND Text: (NOT (""/attacks?/ if able""~3 OR ""/attacks?/ (this OR next OR each) (turn OR combat) if able""~11))")]
		//
		[TestCase(@"Text: (""/block(s|ed)?/ if able""~10 OR ""able to block"" OR ""must be blocked""~3 OR ""must block""~3) AND NOT Keywords: ""block if able""")]
		[TestCase(
			@"Keywords: ""block if able"" AND NOT Text: (""must be blocked"" OR ""/blocks?/ (if OR do) (able OR so)""~6)")]
		[TestCase(@"Text: (aura* AND swap*) AND NOT Keywords: ""aura swap""")]
		[TestCase(@"Text: *awaken* AND NOT Keywords: awaken AND NOT Name: ""Awaken the Sky Tyrant""")]
		[TestCase(@"Text: *banding* AND NOT Keywords: banding")]
		[TestCase(@"Text: (battle AND cry) AND NOT Keywords: ""battle cry""")]
		[TestCase(@"Text: *bestow*  AND NOT Keywords: bestow")]
		[TestCase(@"Text: *bloodthirst*  AND NOT Keywords: bloodthirst AND NOT Name:bloodthirsty")]
		[TestCase(@"Keywords: bury")]
		[TestCase(@"Text: *bushido* AND NOT Keywords: bushido")]
		[TestCase(@"Text: *buyback* AND NOT Keywords: buyback")]
		//
		[TestCase(@"Text: (""(can't attack""~5 OR ""/(can)?not/ attack""~5) AND NOT Keywords: ""can't attack""")]
		[TestCase(@"Keywords: ""can't attack"" AND NOT Text: (""can't attack"" OR ""you can't be attacked"")")]
		//
		[TestCase(@"Text: ""can't block""~5 AND NOT Keywords: ""can't block"" AND NOT Name: ""Wall of Air""")]
		[TestCase(@"Keywords: ""can't block"" AND NOT Text: (""can't block"" OR ""can't attack or block"" OR ""can't attack, block"")")]
		//
		[TestCase(@"Text: (""can't be blocked""~6 AND NOT ""works on creatures that can't be blocked"") AND NOT Keywords: ""can't be blocked""")]
		[TestCase(@"Keywords: ""can't be blocked"" AND NOT Text: (""can't be blocked"" OR ""can't block or be blocked"")")]
		//
		[TestCase(@"Text: ""be countered""~8 AND NOT Keywords: ""can't be countered""")]
		[TestCase(@"Keywords: ""can't be countered"" AND NOT Text: ""can't be countered""")]
		//
		[TestCase(@"Text: ""be regenerated""~8 AND NOT Keywords: ""can't be regenerated""")]
		[TestCase(@"Keywords: ""can't be regenerated"" AND NOT Text: ""can't be regenerated""")]
		//
		[TestCase(@"Text: (""can't block""~8 AND NOT ""can block"") AND NOT Keywords: ""can't block"" AND NOT Name: Tromokratis")]
		[TestCase(@"Keywords: ""can't block"" AND NOT Text: (""can't block"" OR ""can't attack or block"" OR ""can't attack, block"")")]
		//
		[TestCase(@"Text: *cascad* AND NOT Keywords: cascade AND NOT Name: ""Skyline Cascade""")]
		[TestCase(@"Text: (cast* AND NOT (castle OR castaway* OR castigator)) AND NOT Keywords: cast")]
		[TestCase(@"Text: *champion* AND NOT Keywords: champion AND NOT Name: champion")]
		[TestCase(@"Text: *changeling* AND NOT Keywords: changeling")]
		[TestCase(@"Text: *cipher* AND NOT Keywords: cipher")]
		[TestCase(@"Text: *cohort* AND NOT Keywords: cohort AND NOT Name: cohort")]
		[TestCase(@"Text: (*conspir* AND NOT conspiracy) AND NOT Keywords: conspire")]
		[TestCase(@"Text: *convok* AND NOT Keywords: convoke")]
		[TestCase(@"Text: (*copy* OR *copi* AND NOT cornucopia) AND NOT Keywords: copy")]
		//
		[TestCase(@"Text: (/counter(s|ed)?/ AND NOT (""/counters?/ (from OR on OR put OR to OR are OR aren't OR can't)"" OR ""(/[0-9]/ OR poison OR energy OR experience OR lore) /counters?/"" OR ""can't be countered"")) AND NOT keywords: counter")]
		[TestCase(@"Keywords: counter AND NOT Text: ((countered AND NOT ""can't be countered"") OR ""counter it"" OR ""/counters?/ (target OR a OR all OR that OR the) (instant OR creature OR >) (/spells?/ OR /abilit(y|ies)/)""~3) AND NOT Name: (""Brain Gorgers"" OR Phantasmagorian OR ""Temporal Extortion"")")]
		//
		[TestCase(@"Text: (*creat* AND NOT *creature*) AND NOT Keywords: create")]
		//
		[TestCase(@"Text: *crew* AND NOT Keywords: crew AND NOT Name: *crew*")]
		[TestCase(@"Keywords: crew AND NOT Text: (""crew /[0-9]/"" OR ""crews a vehicle"")")]
		//
		[TestCase(@"Text: (cumulativ* AND upkeep*) AND NOT Keywords: ""cumulative upkeep""")]
		[TestCase(@"Text: (*cycl* AND NOT (cyclo*)) AND NOT Keywords: cycling AND NOT Name: (cyclical OR ""cycle of life"" OR recycler)")]
		[TestCase(@"Keywords: cycling AND NOT Text: (*cycling OR ""cycles a card"" OR ""can't cycle cards"" OR ""cycle or discard a card"" OR ""cycled or discarded"")")]
		[TestCase(@"Text: *dash* AND NOT Keywords: dash AND NOT Name: (dash OR dasher)")]
		[TestCase(@"Text: *deathtouch* AND NOT Keywords: deathtouch")]
		[TestCase(@"Text: *defender* AND NOT Keywords: defender AND NOT Name: defender*")]
		[TestCase(@"Text: *delirium* AND NOT Keywords: delirium")]
		[TestCase(@"Text: *delv* AND NOT Keywords: delve AND NOT Name: delver")]
		[TestCase(@"Text: (*destr* AND NOT indestructible) AND NOT Keywords: destroy AND NOT Name: destructi*")]
		[TestCase(@"Text: *dethron* AND NOT Keywords: dethrone")]
		[TestCase(@"Text: *devoid* AND NOT Keywords: devoid")]
		[TestCase(@"Text: *devour* AND NOT Keywords: devour AND NOT Name: devour*")]
		[TestCase(@"Text: *discard* AND NOT Keywords: discard")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		[TestCase(@"")]
		public void Keyword_pattern_Has_perfect_recall(string query)
		{
			var cardNames = Searcher.SearchCards(query, "en")
				.Select(c => c.NameEn)
				.ToHashSet(Str.Comparer);

			Assert.That(cardNames, Is.Empty);
		}
	}
}