using System.Linq;
using System.Text.RegularExpressions;
using Mtgdb.Dal;
using Mtgdb.Dal.Index;
using Mtgdb.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class IndexCreationTests : TestsBase
	{
		[OneTimeSetUp]
		public void Setup()
		{
			LoadTranslations();

			_cardSearcher = new CardSearcher(Repo, new CardDocumentAdapter());
			_keywordSearcher = new KeywordSearcher(Repo);

			bool filterSet(Set set) => Str.Equals(set.Code, "ISD");

			_cardSearcher.Spellchecker.FilterSet =
				_cardSearcher.FilterSet =
					_keywordSearcher.FilterSet = filterSet;
		}

		[Test, Order(0)]
		public void KeywordSearcher_creates_index()
		{
			_keywordSearcher.IndexDirectoryParent += "-test";
			_keywordSearcher.InvalidateIndex();

			Assert.That(_keywordSearcher.IsUpToDate, Is.Not.True);

			_keywordSearcher.Load();

			Assert.That(_keywordSearcher.IsUpToDate);
		}

		[Test, Order(0)]
		public void LuceneSearcher_creates_index()
		{
			_cardSearcher.IndexDirectoryParent += "-test";
			_cardSearcher.InvalidateIndex();

			Assert.That(_cardSearcher.IsUpToDate, Is.Not.True);

			_cardSearcher.LoadIndex();

			Assert.That(_cardSearcher.IsUpToDate);
		}

		[Test, Order(1)]
		public void LuceneSpellchecker_creates_index()
		{
			_cardSearcher.Spellchecker.IndexDirectoryParent += "-test";
			_cardSearcher.Spellchecker.InvalidateIndex();

			Assert.That(_cardSearcher.Spellchecker.IsUpToDate, Is.Not.True);

			_cardSearcher.LoadSpellcheckerIndex();

			Assert.That(_cardSearcher.Spellchecker.IsUpToDate);
		}

		[Test, Order(1)]
		public void LuceneSearcher_searches()
		{
			var cards = _cardSearcher.SearchCards("nameen:vampire", "en")
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
					Sequence.From(new KeywordQueryTerm
					{
						FieldName = "Cmc",
						Patterns = Array.From(new Regex("^0$")),
						Values = Array.From("0")
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

			var suggest = _cardSearcher.Spellchecker.Suggest("en", new TextInputState(query, query.Length, selectionLength: 0)).Values;

			Assert.That(suggest, Is.Not.Null.And.Not.Empty);
			Assert.That(suggest, Has.Some.Contains("vampire").IgnoreCase);
		}

		private CardSearcher _cardSearcher;
		private KeywordSearcher _keywordSearcher;
	}
}