using System.Linq;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class IndexCreationTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadTranslations();

			_luceneSearcher = new LuceneSearcher(Repo);
			_keywordSearcher = new KeywordSearcher();

			bool filterSet(Set set) => Str.Equals(set.Code, "ISD");

			_luceneSearcher.Spellchecker.FilterSet =
				_luceneSearcher.FilterSet =
					_keywordSearcher.FilterSet = filterSet;
		}

		[Test, Order(0)]
		public void KeywordSearcher_creates_index()
		{
			_keywordSearcher.IndexDirectoryParent += "-test";
			_keywordSearcher.InvalidateIndex();

			Assert.That(_keywordSearcher.IsUpToDate, Is.Not.True);

			_keywordSearcher.Load(Repo);

			Assert.That(_keywordSearcher.IsUpToDate);
		}

		[Test, Order(0)]
		public void LuceneSearcher_creates_index()
		{
			_luceneSearcher.IndexDirectoryParent += "-test";
			_luceneSearcher.InvalidateIndex();

			Assert.That(_luceneSearcher.IsUpToDate, Is.Not.True);

			_luceneSearcher.LoadIndex();

			Assert.That(_luceneSearcher.IsUpToDate);
		}

		[Test, Order(1)]
		public void LuceneSpellchecker_creates_index()
		{
			_luceneSearcher.Spellchecker.IndexDirectoryParent += "-test";
			_luceneSearcher.Spellchecker.InvalidateIndex();

			Assert.That(_luceneSearcher.Spellchecker.IsUpToDate, Is.Not.True);

			_luceneSearcher.LoadSpellcheckerIndex();

			Assert.That(_luceneSearcher.Spellchecker.IsUpToDate);
		}

		[Test, Order(1)]
		public void LuceneSearcher_searches()
		{
			var cards = _luceneSearcher.SearchCards("nameen:vampire", "en")
				.ToArray();

			Assert.That(cards, Is.Not.Null.And.Not.Empty);

			foreach (var card in cards)
				Assert.That(card.NameEn, Does.Contain("vampire").IgnoreCase);
		}

		[Test, Order(1)]
		public void KeywordSearcher_searches()
		{
			var cardIds = _keywordSearcher.GetCardIds(
					Enumerable.Empty<KeywordQueryTerm>(),
					Unit.Sequence(new KeywordQueryTerm
					{
						FieldName = "Cmc",
						Patterns = new[] { new Regex("^0$") },
						Values = new[] { "0" }
					}),
					Enumerable.Empty<KeywordQueryTerm>())
				.ToArray();

			Assert.That(cardIds, Is.Not.Null.And.Not.Empty);

			foreach (var id in cardIds)
			{
				var card = Repo.Cards[id];
				Assert.That(card.Cmc, Is.EqualTo(0));
			}
		}

		[Test, Order(2)]
		public void LuceneSpellchecker_searches()
		{
			string query = "nameen:vampire";

			var suggest = _luceneSearcher.Spellchecker.Suggest("en", query, caret: query.Length).Values;

			Assert.That(suggest, Is.Not.Null.And.Not.Empty);
			Assert.That(suggest, Has.Some.Contains("vampire").IgnoreCase);
		}

		private LuceneSearcher _luceneSearcher;
		private KeywordSearcher _keywordSearcher;
	}
}