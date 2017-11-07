using System.Linq;
using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class KeywordTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadModules();
			LoadCards();
			LoadTranslations();
		}

		[Test]
		public void Lea_badlands_manacost_keywords()
		{
			var card = Repo.SetsByCode["LEA"].CardsByName["Badlands"].First();
			var keywords = new CardKeywords();
			keywords.LoadKeywordsFrom(card);
			var values = keywords.KeywordsByProperty[nameof(KeywordDefinitions.ManaCost)];

			Assert.That(values, Is.Not.Null);
			Assert.That(values, Is.Not.Empty);
		}
	}
}