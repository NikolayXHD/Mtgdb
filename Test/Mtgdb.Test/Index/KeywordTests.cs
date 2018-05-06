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

		[TestCase("Text: *absorb* AND NOT Keywords: absorb")]
		[TestCase("Text: *activat* AND NOT Keywords: activate")]
		[TestCase("Text: *affinity* AND NOT Keywords: affinity")]
		[TestCase("Text: *afflict* AND NOT Keywords: afflict AND NOT Name: afflict*")]
		[TestCase("Text: *aftermath* AND NOT Keywords: aftermath")]
		[TestCase("Text: *amplify* AND NOT Keywords: amplify")]
		[TestCase("Text: *annihilator* AND NOT Keywords: annihilator")]
		[TestCase("Text: ante* AND NOT (Keywords: ante OR Name: (ante* OR \"master of the wild hunt avatar\"))")]
		[TestCase("Text: *ascend* AND NOT Keywords: ascend AND NOT Name: (ascendant OR ascending OR ascendancy)")]
		[TestCase("Text: (*attach* AND NOT unattach*) AND NOT Keywords: attach")]
		[TestCase("Text: (*unattach*) AND NOT Keywords: unattach")]
		[TestCase("Text: ((\"attack if able\"~10 OR \"attacks if able\"~10) AND NOT (\"attack blocks if able\"~10 OR \"attacks blocks if able\"~10 OR \"attacks block if able\"~10 OR \"blocked if able attacks\"~10)) AND NOT Keywords: \"attack if able\"")]
		[TestCase("Text: (\"block if able\"~10 OR \"blocks if able\"~10 OR \"blocked if able~10\" OR \"able to block\" OR \"must be blocked\"~3 OR \"must block\"~3) AND NOT Keywords: \"block if able\"")]
		[TestCase("Text: (aura* AND swap*) AND NOT Keywords: \"aura swap\"")]
		[TestCase("Text: *awaken* AND NOT Keywords: awaken AND NOT Name: \"Awaken the Sky Tyrant\"")]
		[TestCase("Text: *banding* AND NOT Keywords: banding")]
		[TestCase("Text: (battle AND cry) AND NOT Keywords: \"battle cry\"")]
		[TestCase("Text: *bestow*  AND NOT Keywords: bestow")]
		[TestCase("Text: *bloodthirst*  AND NOT Keywords: bloodthirst AND NOT Name:bloodthirsty")]
		[TestCase("Keywords: bury")]
		[TestCase("Text: *bushido* AND NOT Keywords: bushido")]
		[TestCase("Text: *buyback* AND NOT Keywords: buyback")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		[TestCase("")]
		public void Keyword_pattern_Has_perfect_recall(string query, params string[] expectedExceptions)
		{
			var unexpectedCardNames = Searcher.SearchCards(query, "en")
				.Select(c => c.NameEn)
				.ToHashSet(Str.Comparer);

			unexpectedCardNames.ExceptWith(expectedExceptions);

			Assert.That(unexpectedCardNames, Is.Empty);
		}
	}
}