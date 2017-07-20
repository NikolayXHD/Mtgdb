using System.Linq;
using Mtgdb.Dal;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class KeywordTests
	{
		private CardRepository _repo;

		[OneTimeSetUp]
		public void Setup()
		{
			TestLoadingUtil.LoadModules();
			TestLoadingUtil.LoadCardRepository();
			_repo = TestLoadingUtil.CardRepository;
		}

		[Test]
		public void TestEmpty()
		{
			var card = _repo.SetsByCode["LEA"].CardsByName["Badlands"].First();
			var keywords = new CardKeywords();
			keywords.LoadKeywordsFrom(card);
			var values = keywords.KeywordsByProperty[nameof(KeywordDefinitions.ManaCost)];

			Assert.That(values, Is.Not.Null);
		}
	}
}