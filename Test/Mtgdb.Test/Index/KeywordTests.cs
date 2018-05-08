using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;
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

		[Test]
		public void TestSpanQuery()
		{
			var queries = new SpanQuery[]
			{
				new SpanTermQuery(new Term("texten", "energy")),
				new SpanMultiTermQueryWrapper<WildcardQuery>(new WildcardQuery(new Term("texten", "counter*")))
			};

			var query = new SpanNearQuery(queries, 1, true);

			var cards = Searcher.SearchCards(query).ToList();
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
		[TestCase(@"Text: (""/attacks?/ if able""~10 AND NOT ""/attacks?/ /block(s|ed)?/ if able""~10) AND NOT Keywords: ""attack if able""")]
		[TestCase(@"Text: (""/block(s|ed)?/ if able""~10 OR ""able to block"" OR ""must be blocked""~3 OR ""must block""~3) AND NOT Keywords: ""block if able""")]
		[TestCase(@"Text: (aura* AND swap*) AND NOT Keywords: ""aura swap""")]
		[TestCase(@"Text: *awaken* AND NOT Keywords: awaken AND NOT Name: ""Awaken the Sky Tyrant""")]
		[TestCase(@"Text: *banding* AND NOT Keywords: banding")]
		[TestCase(@"Text: (battle AND cry) AND NOT Keywords: ""battle cry""")]
		[TestCase(@"Text: *bestow*  AND NOT Keywords: bestow")]
		[TestCase(@"Text: *bloodthirst*  AND NOT Keywords: bloodthirst AND NOT Name:bloodthirsty")]
		[TestCase(@"Keywords: bury")]
		[TestCase(@"Text: *bushido* AND NOT Keywords: bushido")]
		[TestCase(@"Text: *buyback* AND NOT Keywords: buyback")]
		[TestCase(@"Text: (""(can't attack""~5 OR ""/(can)?not/ attack""~5) AND NOT Keywords: ""can't attack""")]
		[TestCase(@"Text: ""can't block""~5 AND NOT Keywords: ""can't block"" AND NOT Name: ""Wall of Air""")]
		[TestCase(@"Text: (""can't be blocked""~6 AND NOT ""works on creatures that can't be blocked"") AND NOT Keywords: ""can't be blocked""")]
		[TestCase(@"Text: ""be countered""~8 AND NOT Keywords: ""can't be countered""")]
		[TestCase(@"Text: ""be regenerated""~8 AND NOT Keywords: ""can't be regenerated""")]
		[TestCase(@"Text: (""can't block""~8 AND NOT ""can block"") AND NOT Keywords: ""can't block"" AND NOT Name: Tromokratis")]
		[TestCase(@"Text: *cascad* AND NOT Keywords: cascade AND NOT Name: ""Skyline Cascade""")]
		[TestCase(@"Text: (cast* AND NOT (castle OR castaway* OR castigator)) AND NOT Keywords: cast")]
		[TestCase(@"Text: *champion* AND NOT Keywords: champion AND NOT Name: champion")]
		[TestCase(@"Text: *changeling* AND NOT Keywords: changeling")]
		[TestCase(@"Text: *cipher* AND NOT Keywords: cipher")]
		[TestCase(@"Text: *cohort* AND NOT Keywords: cohort AND NOT Name: cohort")]
		[TestCase(@"Text: (*conspir* AND NOT conspiracy) AND NOT Keywords: conspire")]
		[TestCase(@"Text: *convok* AND NOT Keywords: convoke")]
		[TestCase(@"Text: (*copy* OR *copi* AND NOT cornucopia) AND NOT Keywords: copy")]
		[TestCase("Text: (counter* AND NOT (counterclockwise OR \"can\'t be countered\" OR \"/counters?/ (from OR on OR put OR aren't)\" OR \"(\\+1\\/\\+1 OR \\-1\\/\\-1 OR \\-2\\/\\-1 OR get OR more OR crank OR get OR lore OR poison OR experience OR energy) /counters?/\")) AND NOT Keywords: counter")]
		[TestCase(@"Text: (*creat* AND NOT *creature*) AND NOT Keywords: create")]
		[TestCase(@"Text: *crew* AND NOT Keywords: crew AND NOT Name: *crew*")]
		[TestCase(@"Text: (cumulativ* AND upkeep*) AND NOT Keywords: ""cumulative upkeep""")]
		[TestCase(@"Text: (*cycl* AND NOT (cyclo*)) AND NOT Keywords: cycling AND NOT Name: (cyclical OR ""cycle of life"" OR recycler)")]
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